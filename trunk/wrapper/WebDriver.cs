using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using System.Text;

namespace SeleniumWrapper
{
    /// <summary>Defines the interface through which the user controls the browser using WebDriver (Selenium 2) and Selenium RC (Selenium 1) commands.</summary>
    /// <example>
    /// 
    /// This example asks the user to launch internet explorer and search for "Eiffel tower".
    /// <code lang="vbs">	
    /// Set selenium = CreateObject("SeleniumWrapper.WebDriver")
    /// selenium.start "ie", "http://www.google.com"
    /// selenium.open "/"
    /// selenium.type "name=q", "Eiffel tower"
    /// selenium.click "name=btnG"
    /// </code>
    /// 
    /// The following example read and write datas in an Excel range named "MyValues".
    /// Each link in first column is clicked and the page title is is compared with the second column.
    /// The verification result is set in the third column.
    /// <code lang="vbs">	
    /// Public Sub TC002()
    ///   Dim selenium As New SeleniumWrapper.WebDriver, r As Range
    ///   selenium.Start "firefox", "http://www.mywebsite.com/"   'Launch Firefox
    ///   selenium.Open "/"
    ///   For Each r In Range("MyValues").Rows    'Loop for each row in the range named "MyValues"
    ///           'Click on the link defined in the first column of "MyValues"
    ///       selenium.Click r.Cells(1, 1)
    ///          'Compare the page title with the one in the second column of "MyValues"
    ///          '  And past the verification result in the third column of  "MyValues"
    ///       r.Cells(1, 3) = selenium.verifyTitle(r.Cells(1, 2))
    ///   Next
    /// End Sub
    /// </code>
    /// 
    /// </example>
    ///

    [Description("Defines the interface through which the user controls the browser using WebDriver (Selenium 2) and Selenium RC (Selenium 1) commands.")]
    [Guid("432b62a5-6f09-45ce-b10e-e3ccffab4234")]
    [ComVisible(true), ComDefaultInterface(typeof(IWebDriver)), ComSourceInterfaces(typeof(WebDriverEvents)), ClassInterface(ClassInterfaceType.None)]
    public partial class WebDriver : IDisposable, IWebDriver
    {
        //public delegate void EndOfCommandDelegate();
        //public event EndOfCommandDelegate EndOfCommand;

        public delegate Object ActionResult();
        public delegate void ActionVoid();

        internal static OpenQA.Selenium.IWebDriver CurrentWebDriver;

        internal OpenQA.Selenium.IWebDriver _webDriver;
        internal int _timeout;
        internal bool _canceled;
        Selenium.WebDriverBackedSelenium _webDriverBacked;
        Dictionary<string, object> _capabilities;
        Dictionary<string, object> _preferences;
        List<string> _extensions;
        string _profile;
        Proxy _proxy { get; set; }
        bool _isStartedRemotely;
        Thread _thread;
        String _error;
        System.Timers.Timer _timerhotkey;
        String _baseUrl;

        public WebDriver() {
            _timeout = 30000;
            _timerhotkey = new System.Timers.Timer(200);
            _timerhotkey.Elapsed += new System.Timers.ElapsedEventHandler(TimerCheckHotKey);
            _capabilities = new Dictionary<string, object>();
            if (this.delegate_function != null) this.delegate_function();
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppDomain_UnhandledException);
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => Utils.runShellCommand(@"FOR /D %A IN (%TEMP%\anonymous*) DO RD /S /Q ""%A"" & FOR /D %A IN (%TEMP%\scoped_dir*) DO RD /S /Q ""%A"" & DEL /q /f %TEMP%\IE*.tmp");
        }

        private void AppDomain_UnhandledException(Object sender, UnhandledExceptionEventArgs e) {
            Exception exception = (Exception)e.ExceptionObject;
            if (!(exception is ThreadAbortException)) {
                _error = exception.GetType().Name + ": " + exception.Message;
                _thread.Abort();
            }
        }

        ~WebDriver() {
            Dispose(true);
        }

        public void Dispose() {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing) {
            _timerhotkey.Stop();
            if (_thread != null) _thread.Abort();
        }

        private void TimerCheckHotKey(object source, ElapsedEventArgs e) {
            if (Utils.isEscapeKeyPressed()) {
                _canceled = true;
                _thread.Abort();
            }
        }

        System.Action delegate_function;

        public void RegisterFunction([MarshalAs(UnmanagedType.FunctionPtr)] System.Action doevents_function) {
            this.delegate_function = doevents_function;
        }

        private string GetErrorPrifix(string methodeName) {
            string lMethodname = Regex.Match(methodeName, "<([^>]+)>").Groups[0].Value;
            return "Method " + lMethodname + " failed !";
        }

        private void InvokeWd(ActionVoid action) {
            _error = null;
            _thread = new System.Threading.Thread((System.Threading.ThreadStart)delegate {
                try {
                    action();
                } catch (System.Exception ex) {
                    if (!(ex is ThreadAbortException)) _error = ex.GetType().ToString() + ": " + ex.Message;
                }
            });
            _thread.Start();
            bool succeed = _thread.Join(_timeout + 1000);
            this.CheckCanceled();
            if (!succeed) throw new ApplicationException(GetErrorPrifix(action.Method.Name) + "\nTimed out running command after " + _timeout + " milliseconds");
            if (_error != null) throw new ApplicationException(GetErrorPrifix(action.Method.Name) + "\n" + _error);
        }

        private Object InvokeWd(ActionResult action) {
            object result = null;
            _error = null;
            _thread = new System.Threading.Thread((System.Threading.ThreadStart)delegate {
                try {
                    result = action();
                } catch (System.Exception ex) {
                    if (!(ex is ThreadAbortException)) _error = ex.Message;
                }
            });
            _thread.Start();
            bool succeed = _thread.Join(_timeout + 1000);
            this.CheckCanceled();
            if (!succeed) throw new ApplicationException(GetErrorPrifix(action.Method.Name) + "\nTimed out running command after " + _timeout + " milliseconds");
            if (_error != null) throw new ApplicationException(GetErrorPrifix(action.Method.Name) + "\n" + _error);
            //if (EndOfCommand != null) EndOfCommand();
            return result;
        }

        private void InvokeWdWaitFor(ActionResult action, Object expected, bool match) {
            object result = null;
            _error = null;
            _thread = new System.Threading.Thread((System.Threading.ThreadStart)delegate{
                while (!_canceled && (match ^ Utils.ObjectEquals(result, expected))) {
                    try {
                        result = action();
                    }catch (Exception ex) {
                        if (ex is ThreadAbortException) break;
                        _error = ex.Message;
                    }
                    Thread.Sleep(100);
                }
            });
            _thread.Start();
            bool succed = _thread.Join(_timeout + 1000);
            this.CheckCanceled();
            if(!succed || _error != null){
                var sb = new StringBuilder();
                sb.Append(GetErrorPrifix(action.Method.Name));
                if(expected!=null)
                    sb.Append(" Expected" + (match ? "=" : "!=") + "<" + expected.ToString() + "> result=<" + (result ?? "null").ToString() + ">.");
                if(!succed)
                    sb.Append(" Timed out after " + _timeout + " ms.");
                if(_error != null)
                    sb.Append(_error);
                throw new ApplicationException(sb.ToString());
            }
        }

        private void InvokeWdAssert(ActionResult action, Object expected, bool match) {
            Object result = InvokeWd(action);
            if (match ^ Utils.ObjectEquals(result, expected)) throw new ApplicationException(GetErrorPrifix(action.Method.Name) + "\nexpected" + (match ? "=" : "!=") + "<" + expected.ToString() + ">\nresult=<" + result.ToString() + "> ");
        }

        private String InvokeWdVerify(ActionResult action, Object expected, bool match) {
            Object result = InvokeWd(action);
            if (match ^ Utils.ObjectEquals(result, expected)) {
                return "KO, " + GetErrorPrifix(action.Method.Name) + " expected" + (match ? "=" : "!=") + "<" + expected.ToString() + "> result=<" + result.ToString() + "> ";
            } else {
                return "OK";
            }
        }

        private void InvokeWdAndWait(ActionVoid action) {
            InvokeWd(action);
            waitForPageToLoad(_timeout);
        }

        /// <summary>Repeatedly applies this instance's input value to the given function until one of the following</summary>
        /// <param name="function"></param>
        /// <param name="timeoutms"></param>
        /// <returns></returns>
        internal object WaitUntilObject(ActionResult function, int timeoutms) {
            var endTime = DateTime.Now.AddMilliseconds(timeoutms);
			string errorMsg = String.Empty;
            while (true) {
                try {
					errorMsg = String.Empty;
                    var result = function();
                    if (result != null) return result;
                } catch (Exception ex) { 
					errorMsg = ex.Message;
				}
                this.CheckCanceled();
                if (DateTime.Now > endTime)
                    throw new TimeoutException("The operation has timed out! " + errorMsg);
                Thread.Sleep(30);
            }
        }

        internal void CheckCanceled() {
            if (_canceled) {
                _canceled = false;
                throw new ApplicationException("Code execution has been interrupted");
            }
        }

        // http://code.google.com/p/selenium/wiki/DesiredCapabilities#Proxy_JSON_Object

        private FirefoxProfile getFirefoxOptions() {
            FirefoxProfile firefoxProfile;
            if (_profile != null) {
                if (System.IO.Directory.Exists(_profile)) {
                    firefoxProfile = new FirefoxProfile(_profile);
                } else {
                    FirefoxProfileManager profilManager = new FirefoxProfileManager();
                    firefoxProfile = profilManager.GetProfile(_profile);
                }
            } else {
                firefoxProfile = new FirefoxProfile();
            }
            if (_preferences != null) {
                foreach (KeyValuePair<string, object> pref in _preferences) {
                    if (pref.Value is string)
                        firefoxProfile.SetPreference(pref.Key, (string)pref.Value);
                    else if (pref.Value is short)
                        firefoxProfile.SetPreference(pref.Key, Convert.ToInt32(pref.Value));
                    else if (pref.Value is bool)
                        firefoxProfile.SetPreference(pref.Key, (bool)pref.Value);
                }
            }
            if (_extensions != null) {
                foreach (string ext in _extensions)
                    firefoxProfile.AddExtension(ext);
            }
            if (_proxy != null)
                firefoxProfile.SetProxyPreferences(_proxy);
            firefoxProfile.AcceptUntrustedCertificates = true;
            return firefoxProfile;
        }

        private DesiredCapabilities getFirefoxCapabilities() {
            DesiredCapabilities lCapability = DesiredCapabilities.Firefox();
            lCapability.SetCapability("firefox_profile", getFirefoxOptions());
            return lCapability;
        }

        private ChromeOptions getChromeOptions() {
            ChromeOptions chromeOptions = new ChromeOptions();
            if (_profile != null)
                chromeOptions.AddArgument("user-data-dir=" + _profile);
            if (_preferences != null)
                chromeOptions.AddAdditionalCapability("chrome.prefs", _preferences);
            if (_extensions != null)
                chromeOptions.AddExtensions(_extensions);
            if (_proxy != null)
                chromeOptions.AddArgument("--proxy-server=" + _proxy.HttpProxy);
            //chromeOptions.AddAdditionalCapability("proxy", proxy);
            foreach (KeyValuePair<string, object> capability in _capabilities) {
                switch (capability.Key) {
                    case "chrome.binary":
                        chromeOptions.BinaryLocation = (string)capability.Value;
                        break;
                    case "chrome.extensions":
                        chromeOptions.AddExtensions(Utils.CastToString((IEnumerable)capability.Value));
                        break;
                    case "chrome.switches":
                        chromeOptions.AddArguments(Utils.CastToString((IEnumerable)capability.Value));
                        break;
                    default:
                        chromeOptions.AddAdditionalCapability(capability.Key, capability.Value);
                        break;
                }
            }
            return chromeOptions;
        }

        private DesiredCapabilities getChromeCapabilities() {
            DesiredCapabilities chromeCapabilities = DesiredCapabilities.Chrome();
            ArrayList switches = new ArrayList();
            if (_profile != null)
                switches.Add("--user-data-dir=" + _profile);
            if (_preferences != null)
                chromeCapabilities.SetCapability("chrome.prefs", _preferences);
            if (_extensions != null)
                chromeCapabilities.SetCapability("chrome.extensions", _extensions);
            if (_proxy != null)
                switches.Add("--proxy-server=" + _proxy.HttpProxy);
            foreach (KeyValuePair<string, object> capability in _capabilities) {
                if (capability.Key == "chrome.switches")
                    switches.AddRange((IList)capability.Value);
                else
                    chromeCapabilities.SetCapability(capability.Key, capability.Value);
            }
            chromeCapabilities.SetCapability("chrome.switches", switches);
            return chromeCapabilities;
        }

        private InternetExplorerOptions getInternetExplorerOptions() {
            InternetExplorerOptions ieoptions = new InternetExplorerOptions();
            if (_preferences != null) throw new Exception("Preference configuration is not available for InternetExplorerDriver!");
            if (_extensions != null) throw new Exception("Preference configuration is not available for InternetExplorerDriver!");
            if (_proxy != null) throw new Exception("Proxy configuration is not available for InternetExplorerDriver!");
            foreach (KeyValuePair<string, object> capability in _capabilities) {
                switch (capability.Key) {
                    case "ignoreProtectedModeSettings": ieoptions.IntroduceInstabilityByIgnoringProtectedModeSettings = (bool)capability.Value; break;
                    case "ignoreZoomSetting": ieoptions.IgnoreZoomLevel = (bool)capability.Value; break;
                    case "initialBrowserUrl": ieoptions.InitialBrowserUrl = (string)capability.Value; break;
                    case "elementScrollBehavior": ieoptions.ElementScrollBehavior = (InternetExplorerElementScrollBehavior)capability.Value; break;
                    case "unexpectedAlertBehaviour": ieoptions.UnexpectedAlertBehavior = (InternetExplorerUnexpectedAlertBehavior)capability.Value; break;
                    case "nativeEvents": ieoptions.EnableNativeEvents = (bool)capability.Value; break;
                    default: ieoptions.AddAdditionalCapability(capability.Key, capability.Value); break;
                }
            }
            return ieoptions;
        }

        private PhantomJSOptions getPhantomJSOptions() {
            var phantomjsOptions = new PhantomJSOptions();
            if (_profile != null) throw new Exception("Profile configuration is not available for PhantomJS driver!");
            if (_preferences != null) throw new Exception("Preference configuration is not available for PhantomJS!");
            if (_extensions != null) throw new Exception("Extension configuration is not available for PhantomJS!");
            if (_proxy != null) throw new Exception("Proxy configuration is not available for InternetExplorerDriver!");
            foreach (var capability in _capabilities)
                phantomjsOptions.AddAdditionalCapability(capability.Key, capability.Value);
            return phantomjsOptions;
        }

        public int Timeout {
            get { return _timeout; }
            set { this.setTimeout(value); }
        }

        /// <summary>Get the actions class</summary>
        /// <example>
        /// <code lang="vbs">	
        ///     WebDriver driver = New WebDriver()
        ///     driver.start "firefox", "http://www.google.com"
        ///     driver.open "/"
        ///     driver.Actions.keyDown(Keys.Control).sendKeys("a").perform
        /// </code>
        /// </example>
        public Actions Actions
        {
            get { return new Actions(_webDriver); }
        }

        /// <summary>Starts a new Selenium testing session</summary>
        /// <param name="browser">Name of the browser : firefox, ie, chrome, phantomjs</param>
        /// <param name="baseUrl">The base URL</param>
        /// <param name="directory">Optional - Directory path for drivers or binaries</param>
        /// <example>
        /// <code lang="vbs">	
        ///     WebDriver driver = New WebDriver()
        ///     driver.start "firefox", "http://www.google.com"
        ///     driver.open "/"
        /// </code>
        /// </example>
        public void start(string browser, String baseUrl, [Optional][DefaultParameterValue("")]String directory) {
            _isStartedRemotely = false;
            if (!String.IsNullOrEmpty(directory)) {
                if (!System.IO.Directory.Exists(directory))
                    throw new ApplicationException("Direcory not found : " + directory);
            } else {
                directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            DesiredCapabilities capa = new DesiredCapabilities(_capabilities);
            switch (browser.ToLower().Replace("*", "")) {
                case "firefox":
                case "ff":
                    _webDriver = new OpenQA.Selenium.Firefox.FirefoxDriver(getFirefoxOptions());
                    break;
                case "cr":
                case "chrome":
                    ChromeDriverService crService = ChromeDriverService.CreateDefaultService(directory);
                    _webDriver = new OpenQA.Selenium.Chrome.ChromeDriver(crService, getChromeOptions());
                    //_webDriver = new OpenQA.Selenium.Chrome.ChromeDriver(directory, getChromeOptions());
                    break;
                case "phantomjs":
                case "pjs":
                    var pjsService = PhantomJSDriverService.CreateDefaultService(directory);
                    _webDriver = new OpenQA.Selenium.PhantomJS.PhantomJSDriver(pjsService, getPhantomJSOptions());
                    //_webDriver = new OpenQA.Selenium.PhantomJS.PhantomJSDriver(directory, getPhantomJSOptions());
                    break;
                case "internetexplorer":
                case "iexplore":
                case "ie":
                case "internetexplorer64":
                case "iexplore64":
                case "ie64":
                    if (browser.EndsWith("64")) directory += @"\ie64";
                    if (_profile != null) throw new Exception("Profile configuration is not available for InternetExplorerDriver!");
                    if (_preferences != null) throw new Exception("Preference configuration is not available for InternetExplorerDriver!");
                    if (_extensions != null) throw new Exception("Extension configuration is not available for InternetExplorerDriver!");
                    if (_proxy != null) throw new Exception("Proxy configuration is not available for InternetExplorerDriver!");
                    _webDriver = new OpenQA.Selenium.IE.InternetExplorerDriver(directory, getInternetExplorerOptions());
                    break;
                case "safari":
                case "sa":
                    throw new Exception("SafariDriver is not yet implemented. Use remote driver insted.");
                default:
                    throw new ApplicationException("Browser <" + browser + "> is not available !  \nAvailable are Firefox, IE, Chrome and PhantomJS");
            }
            WebDriver.CurrentWebDriver = _webDriver;
            _webDriverBacked = new Selenium.WebDriverBackedSelenium(_webDriver, baseUrl);
            InvokeWd(() => _webDriverBacked.Start());
            this.setTimeout(_timeout);
            _baseUrl = baseUrl.TrimEnd('/');
            _timerhotkey.Start();
        }

        /// <summary>Starts remotely a new Selenium testing session</summary>
        /// <param name="browser">Name of the browser : firefox, ie, chrome, phantomjs, htmlunit, htmlunitwithjavascript, android, ipad, opera</param>
        /// <param name="remoteAddress">Remote url address (ex : "http://localhost:4444/wd/hub")</param>
        /// <param name="baseUrl">Base URL</param>
        public void startRemotely(string browser, String remoteAddress, String baseUrl) {
            _isStartedRemotely = true;
            DesiredCapabilities lCapability;
            browser = browser.ToLower().Replace("*", "");
            switch (browser) {
                case "ff":
                case "firefox":
                    lCapability = DesiredCapabilities.Firefox();
                    lCapability.SetCapability("firefox_profile", getFirefoxOptions());
                    break;
                case "cr":
                case "chrome":
                    lCapability = getChromeCapabilities();
                    break;
                case "phantomjs":
                case "pjs":
                    lCapability = (DesiredCapabilities)getPhantomJSOptions().ToCapabilities();
                    break;
                default:
                    switch (browser) {
                        case "internetexplorer":
                        case "iexplore":
                        case "ie": lCapability = DesiredCapabilities.InternetExplorer(); break;
                        case "htmlunit": lCapability = DesiredCapabilities.HtmlUnit(); break;
                        case "htmlunitwithjavascript": lCapability = DesiredCapabilities.HtmlUnitWithJavaScript(); break;
                        case "android": lCapability = DesiredCapabilities.Android(); break;
                        case "ipad": lCapability = DesiredCapabilities.IPad(); break;
                        case "opera": lCapability = DesiredCapabilities.Opera(); break;
                        default: throw new ApplicationException("Remote browser <" + browser + "> is not available !  \nChoose between Firefox, IE, Chrome, HtmlUnit, HtmlUnitWithJavaScript, \nAndroid, IPad, Opera, PhantomJs");
                    }
                    if (_capabilities != null) {
                        foreach (KeyValuePair<string, object> capability in _capabilities)
                            lCapability.SetCapability(capability.Key, capability.Value);
                    }
                    break;
            }
            WebDriver.CurrentWebDriver = _webDriver;
            _webDriver = new RemoteWebDriverCust(new Uri(remoteAddress), lCapability);
            _webDriverBacked = new Selenium.WebDriverBackedSelenium(_webDriver, baseUrl);
            InvokeWd(() => _webDriverBacked.Start());
            this.setTimeout(_timeout);
            _baseUrl = baseUrl.TrimEnd('/');
            _timerhotkey.Start();
        }

        /// <summary>Ends the current Selenium testing session (normally killing the browser)</summary>
        public void stop() {
            _webDriverBacked.Stop();
            _timerhotkey.Stop();
        }

        /// <summary>Set a specific profile for the firefox webdriver</summary>
        /// <param name="directory">Profile directory (Firefox and Chrome) or profil name (Firefox only)</param>
        /// <remarks>The profile directory can be copied from the user temp folder (run %temp%) before the WebDriver is stopped. It's also possible to create a new Firefox profile by launching firefox with the "-p" switch (firefox.exe -p).</remarks>
        /// <example>
        /// <code lang="vbs">
        ///   Dim driver As New SeleniumWrapper.WebDriver
        ///   driver.setProfile "Selenium"  'Firefox only. Profile created by running "..\firefox.exe -p"
        ///   driver.Start "firefox", "http://www.google.com"
        ///   ...
        /// </code>
        /// <code lang="vbs">
        ///   Dim driver As New SeleniumWrapper.WebDriver
        ///   driver.setProfile "C:\MyProfil"   'For Chrome and Firefox only
        ///   driver.Start "firefox", "http://www.google.com"
        ///   ...
        /// </code>
        /// </example>
        public void setProfile(string directory) {
            _profile = directory;
        }

        /// <summary>Set a specific preference for the firefox webdriver</summary>
        /// <param name="key">Préférence key</param>
        /// <param name="value">Préférence value</param>
        public void setPreference(string key, object value) {
            if (_preferences == null) _preferences = new Dictionary<string, object>();
            _preferences.Add(key, value);
        }

        /// <summary>Set a specific capability for the webdriver</summary>
        /// <param name="key">Capability key</param>
        /// <param name="value">Capability value</param>
        public void setCapability(string key, object value) {
            _capabilities[key] = value;
        }

        /// <summary>Add an extension to the browser (For Firefox and Chrome only)</summary>
        /// <param name="extensionPath">Path to the extension</param>
        public void addExtension(string extensionPath) {
            if (_extensions == null)
                _extensions = new List<string>();
            _extensions.Add(extensionPath);
        }

        /// <summary>Set a specific proxy for the webdriver</summary>
        /// <param name="url">Proxy URL</param>
        /// <param name="isAutoConfigURL">Is an auto-config URL</param>
        public void setProxy(string url, [Optional][DefaultParameterValue(false)]bool isAutoConfigURL) {
            _proxy = new Proxy();
            if (isAutoConfigURL) {
                _proxy.ProxyAutoConfigUrl = url;
            } else {
                _proxy.HttpProxy = url;
                _proxy.FtpProxy = url;
                _proxy.SslProxy = url;
            }
        }

        /// <summary>"Opens an URL in the test frame. This accepts both relative and absolute URLs."</summary>
        /// <param name="url">URL</param>
        public void open(String url) {
            if (!url.Contains("://"))
                url = _baseUrl + '/' + url.TrimStart('/');
            InvokeWd(() => _webDriverBacked.Open(url));
        }

        /// <summary>Wait the specified time in millisecond before executing the next command</summary>
        /// <param name="timems">Time to wait in millisecond</param>
        public void sleep(int timems) {
            var endTime = DateTime.Now.AddMilliseconds(timems);
            do {
                Thread.Sleep(30);
                this.CheckCanceled();
            } while (DateTime.Now < endTime);
        }

        /// <summary>Wait the specified time in millisecond before executing the next command</summary>
        /// <param name="timems">Time to wait in millisecond</param>
        public void wait(int timems) {
            this.sleep(timems);
        }

        /// <summary>Wait the specified time in millisecond before executing the next command</summary>
        /// <param name="timems">Time to wait in millisecond</param>
        public void pause(int timems) {
            this.sleep(timems);
        }

        /// <summary>Specifies the amount of time the driver should wait when searching for an element if it is not immediately present.</summary>
        /// <param name="timeoutms">timeout in millisecond</param>
        public void setImplicitWait(int timeoutms) {
            _webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(timeoutms));
        }

        /// <summary> Specifies the amount of time that Selenium will wait for actions to complete. The default timeout is 30 seconds.</summary>
        /// <param name="timeoutms">timeout in milliseconds, after which an error is raised</param>
        public void setTimeout(int timeoutms) {
            _timeout = timeoutms;
            if (_webDriver != null) {
                try {
                    //todo : remove silent exception once the chrome issue is resolved (Issue 4448)
                    _webDriver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromMilliseconds(timeoutms));
                } catch (Exception) { }
                _webDriver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromMilliseconds(timeoutms));
                _webDriverBacked.SetTimeout(_timeout.ToString());
            }
        }

        /// <summary>Saves the entire contents of the current window canvas to a PNG file. Contrast this with the captureScreenshot command, which captures the contents of the OS viewport (i.e. whatever is currently being displayed on the monitor), and is implemented in the RC only. Currently this only works in Firefox when running in chrome mode, and in IE non-HTA using the EXPERIMENTAL \"Snapsie\" utility. The Firefox implementation is mostly borrowed from the Screengrab! Firefox extension. Please see http://www.screengrab.org and http://snapsie.sourceforge.net/ for details. the path to the file to persist the screenshot as. No filename extension will be appended by default. Directories will not be created if they do not exist, and an exception will be thrown, possibly by native code.a kwargs string that modifies the way the screenshot is captured. Example: \"background=#CCFFDD\" . Currently valid options: backgroundthe background CSS for the HTML document. This may be useful to set for capturing screenshots of less-than-ideal layouts, for example where absolute positioning causes the calculation of the canvas dimension to fail and a black background is exposed (possibly obscuring black text).</summary>
        public void captureEntirePageScreenshot(String filename, [Optional][DefaultParameterValue("")]String kwargs) {
            getScreenshot().SaveAs(filename);
        }

        /// <summary>Undo the effect of calling chooseCancelOnNextConfirmation. Note that Selenium's overridden window.confirm() function will normally automatically return true, as if the user had manually clicked OK, so you shouldn't need to use this command unless for some reason you need to change your mind prior to the next confirmation. After any confirmation, Selenium will resume using the default behavior for future confirmations, automatically returning true (OK) unless/until you explicitly call chooseCancelOnNextConfirmation for each confirmation. Take note - every time a confirmation comes up, you must consume it with a corresponding getConfirmation, or else the next selenium operation will fail. </summary>
        public void chooseOkOnNextConfirmation() {
            InvokeWd(() => _webDriver.SwitchTo().Alert().Dismiss());
        }

        /// <summary>By default, Selenium's overridden window.confirm() function will return true, as if the user had manually clicked OK; after running this command, the next call to confirm() will return false, as if the user had clicked Cancel. Selenium will then resume using the default behavior for future confirmations, automatically returning true (OK) unless/until you explicitly call this command for each confirmation.  Take note - every time a confirmation comes up, you must consume it with a corresponding getConfirmation, or else the next selenium operation will fail.</summary>
        public void chooseCancelOnNextConfirmation() {
            InvokeWd(() => _webDriver.SwitchTo().Alert().Accept());
        }

        /// <summary>Resize currently selected window to take up the entire screen </summary>
        public void windowMaximize() {
            // With chrome: use maximize switch
            // With IE: use windowMaximize javascript function
            // With Firefox: use Manage().Window.Maximize() method
            if (_isStartedRemotely)
                InvokeWd(() => _webDriver.Manage().Window.Maximize());
            else {
                string handle = _webDriver.CurrentWindowHandle;
                Utils.maximizeForegroundWindow();
            }

        }

        #region WebDriver Code   // Following funtion are webdriver related

        /// <summary>Loads a web page in the current browser session.</summary>
        /// <param name="url">URL</param>
        public void get(String url) {
            if (!url.Contains("://"))
                url = _baseUrl + '/' + url.TrimStart('/');
            InvokeWd(() => _webDriver.Navigate().GoToUrl(url));
        }

        /// <summary>Gets the source of the page last loaded by the browser.</summary>
        public String PageSource {
            get { return _webDriver.PageSource; }
        }

        /// <summary>Returns the current Url.</summary>
        public string Url {
            get { return _webDriver.Url; }
        }

        /// <summary>Goes one step backward in the browser history.</summary>
        public void back() {
            _webDriver.Navigate().Back();
        }

        /// <summary>Goes one step forward in the browser history.</summary>
        public void forward() {
            _webDriver.Navigate().Forward();
        }

        /// <summary>Closes the current window.</summary>
        public void close() {
            _webDriver.Close();
        }

        /// <summary>Returns the handle of the current window.</summary>
        public string WindowHandle {
            get { return _webDriver.CurrentWindowHandle; }
        }

        /// <summary>Returns the handles of all windows within the current session.</summary>
        public string[] WindowHandles {
            get {
                ReadOnlyCollection<string> collection = _webDriver.WindowHandles;
                string[] handles = new string[collection.Count];
                collection.CopyTo(handles, 0);
                return handles;
            }
        }

        /// <summary>Returns the element with focus, or BODY if nothing has focus.</summary>
        public WebElement ActiveElement {
            get {
                return new WebElement(this, _webDriver.SwitchTo().ActiveElement());
            }
        }

        /// <summary>Execute JavaScrip on the page</summary>
        /// <param name="script">Javascript code</param>
        /// <param name="arguments">Arguments to pass to the script</param>
        /// <returns>Returns the value returned by the script</returns>
        /// <example>
        /// 
        /// This example set the page title and return the new page title.
        /// <code lang="vbs">
        /// Debug.Print executeScript("document.title = arguments[0]; return document.title;", "My New Title")
        /// </code>
        /// </example>
        public Object executeScript(String script, [Optional][DefaultParameterValue(null)]Object arguments) {
            //Object result = ((OpenQA.Selenium.IJavaScriptExecutor)_webDriver).ExecuteScript("try{" + script + "}catch(e){return 'error:'+e.message;}", arguments);
            //if (result != null && result.ToString().StartsWith("error:")) throw new ApplicationException("JavaScript " + result.ToString());
            if (arguments is WebElement)
                arguments = ((WebElement)arguments)._webElement;
            else if (arguments.GetType().IsArray){
                var argArray = new object[((object[])arguments).Length];
                for(int i=0; i<argArray.Length; i++)
                    argArray[i] = ((object[])arguments)[i] is WebElement ? ((WebElement)((object[])arguments)[i])._webElement : ((object[])arguments)[i];
                arguments = argArray;
            }
            var ret = ((OpenQA.Selenium.IJavaScriptExecutor)_webDriver).ExecuteScript(script, arguments);
            if (ret is OpenQA.Selenium.IWebElement)
                return new WebElement(this, (OpenQA.Selenium.IWebElement)ret );
            else if (ret.GetType().IsArray){
                var retArray = new object[ ((object[])ret).Length];
                for(int i=0; i<retArray.Length; i++)
                    retArray[i] = ((object[])ret)[i] is OpenQA.Selenium.IWebElement ? new WebElement(this, (OpenQA.Selenium.IWebElement)((object[])ret)[i]) : ((object[])ret)[i];
                return retArray;
            }else if(ret is ReadOnlyCollection<OpenQA.Selenium.IWebElement>)
                return WebElement.GetWebElements(this, (ReadOnlyCollection<OpenQA.Selenium.IWebElement>)ret);
            return null;
        }

        /// <summary>Find the first WebElement using the given method.</summary>
        /// <param name="by">Methode</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElement(Object by, [Optional][DefaultParameterValue(0)]int timeoutms) {
            if (((By)by).base_ == null) throw new NullReferenceException("The locating mechanism is null!");
            return findElement(((By)by).base_, timeoutms);
        }

        /// <summary>Finds an element by name.</summary>
        /// <param name="name">The name of the element to find.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByName(String name, [Optional][DefaultParameterValue(0)]int timeoutms) {
            return this.findElement(OpenQA.Selenium.By.Name(name), timeoutms);
        }

        /// <summary>Finds an element by XPath.</summary>
        /// <param name="xpath">The xpath locator of the element to find.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByXPath(String xpath, [Optional][DefaultParameterValue(0)]int timeoutms) {
            return this.findElement(OpenQA.Selenium.By.XPath(xpath), timeoutms);
        }

        /// <summary>Finds an element by id.</summary>
        /// <param name="id">The id of the element to be found.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementById(String id, [Optional][DefaultParameterValue(0)]int timeoutms) {
            return this.findElement(OpenQA.Selenium.By.Id(id), timeoutms);
        }

        /// <summary>Finds an element by class name.</summary>
        /// <param name="classname">The class name of the element to find.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByClassName(String classname, [Optional][DefaultParameterValue(0)]int timeoutms) {
            return this.findElement(OpenQA.Selenium.By.ClassName(classname), timeoutms);
        }

        /// <summary>Finds an element by css selector.</summary>
        /// <param name="cssselector">The css selector to use when finding elements.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByCssSelector(String cssselector, [Optional][DefaultParameterValue(0)]int timeoutms) {
            return this.findElement(OpenQA.Selenium.By.CssSelector(cssselector), timeoutms);
        }

        /// <summary>Finds an element by link text.</summary>
        /// <param name="linktext">The text of the element to be found.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByLinkText(String linktext, [Optional][DefaultParameterValue(0)]int timeoutms) {
            return this.findElement(OpenQA.Selenium.By.LinkText(linktext), timeoutms);
        }

        /// <summary>Finds an element by a partial match of its link text.</summary>
        /// <param name="partiallinktext">The text of the element to partially match on.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByPartialLinkText(String partiallinktext, [Optional][DefaultParameterValue(0)]int timeoutms) {
            return this.findElement(OpenQA.Selenium.By.PartialLinkText(partiallinktext), timeoutms);
        }

        /// <summary>Finds an element by tag name.</summary>
        /// <param name="tagname">The tag name of the element to find.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByTagName(String tagname, [Optional][DefaultParameterValue(0)]int timeoutms) {
            return this.findElement(OpenQA.Selenium.By.TagName(tagname), timeoutms);
        }

        /// <summary>"Verifies that the specified element is somewhere on the page."</summary>
        /// <param name="locator">An element loctor. String or By object</param>
        /// <returns>true if the element is present, false otherwise</returns>
        public bool isElementPresent(object locator) {
            if (locator is By)
                try {
                    return findElement(((By)locator).base_, 0) != null;
                } catch (Exception) {
                    return false;
                }
            else if (locator is string)
                return (Boolean)InvokeWd(() => _webDriverBacked.IsElementPresent((string)locator));
            else
                throw new ArgumentException("Locator has to be a 'String' or a 'By' object!");
        }

        private WebElement findElement(OpenQA.Selenium.By by, int timeoutms) {
            object ret;
            if (timeoutms > 0)
                ret = this.WaitUntilObject(() => _webDriver.FindElement(by), timeoutms);
            else
                ret = _webDriver.FindElement(by);
            return new WebElement(this, (OpenQA.Selenium.IWebElement)ret);
        }

        /// <summary>Find all elements within the current context using the given mechanism.</summary>
        /// <param name="by">The locating mechanism to use</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>A list of all WebElements, or an empty list if nothing matches</returns>
        public WebElement[] findElements(object by, [Optional][DefaultParameterValue(0)]int timeoutms) {
            if (((By)by).base_ == null) throw new NullReferenceException("The locating mechanism is null!");
            return findElements(((By)by).base_, timeoutms);
        }

        /// <summary>Finds elements by name.</summary>
        /// <param name="name">The name of the elements to find.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElement[] findElementsByName(String name, [Optional][DefaultParameterValue(0)]int timeoutms) {
            return this.findElements(OpenQA.Selenium.By.Name(name), timeoutms);
        }

        /// <summary>Finds multiple elements by xpath.</summary>
        /// <param name="xpath">The xpath locator of the elements to be found.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElement[] findElementsByXPath(String xpath, [Optional][DefaultParameterValue(0)]int timeoutms) {
            return this.findElements(OpenQA.Selenium.By.XPath(xpath), timeoutms);
        }

        /// <summary>Finds multiple elements by id.</summary>
        /// <param name="id">The id of the elements to be found.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElement[] findElementsById(String id, [Optional][DefaultParameterValue(0)]int timeoutms) {
            return this.findElements(OpenQA.Selenium.By.Id(id), timeoutms);
        }

        /// <summary>Finds elements by class name.</summary>
        /// <param name="classname">The class name of the elements to find.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElement[] findElementsByClassName(String classname, [Optional][DefaultParameterValue(0)]int timeoutms) {
            return this.findElements(OpenQA.Selenium.By.ClassName(classname), timeoutms);
        }

        /// <summary>Finds elements by css selector.</summary>
        /// <param name="cssselector">The css selector to use when finding elements.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElement[] findElementsByCssSelector(String cssselector, [Optional][DefaultParameterValue(0)]int timeoutms) {
            return this.findElements(OpenQA.Selenium.By.CssSelector(cssselector), timeoutms);
        }

        /// <summary>Finds elements by link text.</summary>
        /// <param name="linktext">The text of the elements to be found.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElement[] findElementsByLinkText(String linktext, [Optional][DefaultParameterValue(0)]int timeoutms) {
            return this.findElements(OpenQA.Selenium.By.LinkText(linktext), timeoutms);
        }

        /// <summary>Finds elements by a partial match of their link text.</summary>
        /// <param name="partiallinktext">The text of the element to partial match on.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElement[] findElementsByPartialLinkText(String partiallinktext, [Optional][DefaultParameterValue(0)]int timeoutms) {
            return this.findElements(OpenQA.Selenium.By.PartialLinkText(partiallinktext), timeoutms);
        }

        /// <summary>Finds elements by tag name.</summary>
        /// <param name="tagname">The tag name the use when finding elements.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElement[] findElementsByTagName(String tagname, [Optional][DefaultParameterValue(0)]int timeoutms) {
            return this.findElements(OpenQA.Selenium.By.TagName(tagname), timeoutms);
        }

        private WebElement[] findElements(OpenQA.Selenium.By by, int timeoutms) {
            if (timeoutms > 0) {
                var ret = this.WaitUntilObject(() => _webDriver.FindElements(by), timeoutms);
                return WebElement.GetWebElements(this, (ReadOnlyCollection<OpenQA.Selenium.IWebElement>)ret);
            }
            return WebElement.GetWebElements(this, _webDriver.FindElements(by));
        }

        /// <summary>Gets the screenshot of the current window</summary>
        /// <returns>Image</returns>
        public Image getScreenshot() {
            OpenQA.Selenium.Screenshot ret = ((OpenQA.Selenium.ITakesScreenshot)_webDriver).GetScreenshot();
            if (ret == null) throw new ApplicationException("Method <getScreenshot> failed !\nReturned value is empty");
            return new Image(ret.AsByteArray);
        }

        /// <summary>Sends a sequence of keystrokes to the browser.</summary>
        /// <param name="keysOrModifier">Sequence of keys or a modifier key(Control,Shift...) if the sequence is in keysToSendEx</param>
        /// <param name="keys">Optional - Sequence of keys if keysToSend contains modifier key(Control,Shift...)</param>
        /// <example>
        /// To send mobile to the window :
        /// <code lang="vbs">
        ///     driver.sendKeys "mobile"
        /// </code>
        /// To send ctrl+a to the window :
        /// <code lang="vbs">
        ///     driver.sendKeys Keys.Control, "a"
        /// </code>
        /// </example>
        public void sendKeys(string keysOrModifier, [Optional][DefaultParameterValue("")]string keys)
        {
            if(string.IsNullOrEmpty(keys))
                new OpenQA.Selenium.Interactions.Actions(_webDriver).SendKeys(keysOrModifier).Perform();
            else
                new OpenQA.Selenium.Interactions.Actions(_webDriver).KeyDown(keysOrModifier).SendKeys(keys).KeyUp(keysOrModifier).Build().Perform();

        }

        public Cookie getCookie(string name) {
            throw new Exception("Not implemented yet!");
        }

        public Cookie[] getCookies() {
            throw new Exception("Not implemented yet!");
        }

        public string Title {
            get { return _webDriver.Title; }
        }

        /// <summary>Switches focus to the specified window.</summary>
        /// <param name="windowName">The name of the window to switch to.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Current web driver</returns>
        public WebDriver switchToWindow(string windowName, [Optional][DefaultParameterValue(0)]int timeoutms) {
            if (timeoutms > 0)
                this.WaitUntilObject(() => _webDriver.SwitchTo().Window(windowName), timeoutms);
            else
                _webDriver.SwitchTo().Window(windowName);
            return this;
        }

        /// <summary>Switches focus to the specified frame, by index or name.</summary>
        /// <param name="index_or_name">The name of the window to switch to, or an integer representing the index to switch to.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Current web driver</returns>
        public WebDriver switchToFrame(object index_or_name, [Optional][DefaultParameterValue(0)]int timeoutms) {
            if (index_or_name is string) {
                string windowName = (string)index_or_name;
                if (timeoutms > 0)
                    this.WaitUntilObject(() => _webDriver.SwitchTo().Frame(windowName), timeoutms);
                else
                    _webDriver.SwitchTo().Frame(windowName);
            } else {
                int frameIndex = (int)index_or_name;
                if (timeoutms > 0)
                    this.WaitUntilObject(() => _webDriver.SwitchTo().Frame(frameIndex), timeoutms);
                else
                    _webDriver.SwitchTo().Frame(frameIndex);
            }
            return this;
        }

        /// <summary>Switches focus to an alert on the page.</summary>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Focused alert</returns>
        public Alert switchToAlert([Optional][DefaultParameterValue(0)]int timeoutms) {
            Object alert;
            if (timeoutms > 0) {
                alert = this.WaitUntilObject(_webDriver.SwitchTo().Alert, timeoutms);
            } else {
                try {
                    alert = _webDriver.SwitchTo().Alert();
                } catch (Exception) {
                    throw new Exception("Alert not found!");
                }
            }
            return new Alert(this, (OpenQA.Selenium.IAlert)alert);
        }

        #endregion WebDriver Code

        #region Regex

        /// <summary>Indicates whether the regular expression finds a match in the specified source code using the regular expression specified in the pattern parameter.</summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>true if the regular expression finds a match; otherwise, false.</returns>
        public bool isMatch(string pattern) {
            return Regex.IsMatch(this.PageSource, pattern);
        }

        /// <summary>Searches the specified source code for an occurrence of the regular expression supplied in the pattern parameter.</summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>Matching strings</returns>
        public object match(string pattern) {
            Match match = Regex.Match(this.PageSource, pattern);
            if (match.Groups == null)
                return match.Value;
            string[] lst = new string[match.Groups.Count];
            for (int i = 0; i < match.Groups.Count; i++)
                lst[i] = match.Groups[i].Value;
            return lst;
        }

        #endregion Regex

    }

}
