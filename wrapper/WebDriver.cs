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
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.PhantomJS;

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

        internal static OpenQA.Selenium.IWebDriver currentWebDriver;
        internal OpenQA.Selenium.IWebDriver webDriver;
        internal int timeout;
        internal bool canceled;
        Selenium.WebDriverBackedSelenium webDriverBacked;
        Dictionary<string, object> capabilities;
        Dictionary<string, object> preferences;
        List<string> extensions;
        string profile;
        Proxy proxy { get; set; }
        bool isStartedRemotely;
        Thread thread;
        System.Action action;
        Object result;
        String error;
        System.Timers.Timer timerhotkey;
        String baseUrl;

        public WebDriver(){
            this.timeout = 30000;
            this.timerhotkey = new System.Timers.Timer(200);
            this.timerhotkey.Elapsed += new System.Timers.ElapsedEventHandler(TimerCheckHotKey);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppDomain_UnhandledException);
            Utils.runShellCommand(@"FOR /D %A IN (%TEMP%\anonymous*) DO RD /S /Q ""%A"" & FOR /D %A IN (%TEMP%\scoped_dir*) DO RD /S /Q ""%A"" & DEL /q /f %TEMP%\IE*.tmp");
            this.capabilities = new Dictionary<string, object>();
            if(this.delegate_function!=null) this.delegate_function();
        }

        private void AppDomain_UnhandledException(Object sender, UnhandledExceptionEventArgs e){
            Exception exception = (Exception)e.ExceptionObject;
            if(!(exception is ThreadAbortException)){
                this.error = exception.GetType().Name + ": " + exception.Message;
                this.thread.Abort();
            }
        }

        ~WebDriver(){
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.timerhotkey.Stop();
            if (this.thread != null) this.thread.Abort();
        }

        private void TimerCheckHotKey(object source, ElapsedEventArgs e){
            if (Utils.isEscapeKeyPressed()) {
                this.canceled = true;
                this.thread.Abort();
            }
        }

        System.Action delegate_function;

        public void RegisterFunction([MarshalAs(UnmanagedType.FunctionPtr)] System.Action doevents_function)
        {
            this.delegate_function = doevents_function;
        }

        private string GetErrorPrifix(System.Action action){
            string lMethodname = Regex.Match(action.Method.Name, "<([^>]+)>").Groups[0].Value;
            return "Method " + lMethodname + " failed !";
        }

        private Object InvokeWd(System.Action action){
            this.action = action;
            this.result = null;
            this.error = null;
            this.thread = new System.Threading.Thread( (System.Threading.ThreadStart)delegate {
                    try{
                        this.action();
                    }catch(System.Exception ex){ 
                        if(!(ex is ThreadAbortException)) this.error = ex.Message; 
                    }
                });
            this.thread.Start();
            bool succeed = this.thread.Join(this.timeout + 1000);
            this.CheckCanceled();
            if (!succeed) throw new ApplicationException(GetErrorPrifix(this.action) + "\nTimed out running command after " + this.timeout + " milliseconds");
            if (this.error != null) throw new ApplicationException(GetErrorPrifix(this.action) + "\n" + this.error);
            //if (EndOfCommand != null) EndOfCommand();
            return this.result;
        }

        private void InvokeWdWaitFor(System.Action action, Object expected, bool match){
            this.action = action;
            this.result = null;
            this.error = null;
            this.thread = new System.Threading.Thread( (System.Threading.ThreadStart)delegate{
                    try {
                        this.action();
                        while( !this.canceled && (match ^ Utils.ObjectEquals(this.result,expected)) ){
                            Thread.Sleep(20);
                            this.action();
                        }
                    }catch(System.Exception ex){ 
                        if(!(ex is ThreadAbortException)) this.error = ex.Message; 
                    }
                });
            this.thread.Start();
            bool succed = this.thread.Join(this.timeout + 1000);
            this.CheckCanceled();
            if (!succed) throw new ApplicationException(GetErrorPrifix(this.action) + "\nexpected" + (match ? "=" : "!=") + "<" + expected.ToString() + ">\nresult=<" + this.result.ToString() + ">\nTimed out running command after " + this.timeout + " milliseconds");
            if (this.error != null) throw new ApplicationException(GetErrorPrifix(this.action) + " expected" + (match ? "=" : "!=") + "<" + expected.ToString() + "> result=<" + this.result.ToString() + ">\n" + this.error);
            //if (EndOfCommand != null) EndOfCommand();
        }

        private void InvokeWdAssert(System.Action action, Object expected, bool match){
            Object result = InvokeWd(action);
            if (match ^ Utils.ObjectEquals(result, expected)) throw new ApplicationException(GetErrorPrifix(action) + "\nexpected" + (match ? "=" : "!=") + "<" + expected.ToString() + ">\nresult=<" + result.ToString() + "> ");
        }

        private String InvokeWdVerify(System.Action action, Object expected, bool match){
            Object result = InvokeWd(action);
            if (match ^ Utils.ObjectEquals(result, expected)) {
                return "KO, " + GetErrorPrifix(action) + " expected" + (match ? "=" : "!=") + "<" + expected.ToString() + "> result=<" + result.ToString() + "> ";
            }else{
                return "OK";
            }
        }

        private void InvokeWdAndWait(System.Action action){
            InvokeWd(action);
            waitForPageToLoad(this.timeout);
        }

        /// <summary>Repeatedly applies this instance's input value to the given function until one of the following</summary>
        /// <param name="function"></param>
        /// <param name="timeoutms"></param>
        /// <returns></returns>
        public object WaitUntilObject(Func<object> function, int timeoutms)
        {
            var endTime = DateTime.Now.AddMilliseconds(timeoutms);
            while (true){
                if( DateTime.Now > endTime ){
                    throw new TimeoutException( "The operation has timed out!" );
                }
                try{
                    var result = function();
                    if(result!=null) return result;
                }catch (Exception){ }
                this.CheckCanceled();
                Thread.Sleep(30);
            }
        }

        internal void CheckCanceled()
        {
            if (this.canceled){
                this.canceled = false;
                throw new ApplicationException("Code execution has been interrupted");
            }
        }

        // http://code.google.com/p/selenium/wiki/DesiredCapabilities#Proxy_JSON_Object

        private FirefoxProfile getFirefoxOptions()
        {
            FirefoxProfile firefoxProfile;
            if(this.profile!=null){
                if( System.IO.Directory.Exists(this.profile) ){
                    firefoxProfile = new FirefoxProfile(this.profile);
                }else{
                    FirefoxProfileManager profilManager = new FirefoxProfileManager();
                    firefoxProfile = profilManager.GetProfile(this.profile);
                }
            }else{
                firefoxProfile = new FirefoxProfile();
            }
            if(this.preferences!=null){
                foreach (KeyValuePair<string, object> pref in this.preferences){
                    if(pref.Value is string){
                        firefoxProfile.SetPreference(pref.Key, (string)pref.Value);
                    }else if(pref.Value is short){
                        firefoxProfile.SetPreference(pref.Key, Convert.ToInt32(pref.Value));
                    }else if(pref.Value is bool){
                        firefoxProfile.SetPreference(pref.Key, (bool)pref.Value);
                    }
                }
            }
            if(this.extensions!=null){
                foreach (string ext in this.extensions){
                    firefoxProfile.AddExtension(ext);
                }
            }
            if(this.proxy!=null){
                firefoxProfile.SetProxyPreferences(proxy);
            }
            firefoxProfile.AcceptUntrustedCertificates = true;
            return firefoxProfile;
        }

        private DesiredCapabilities getFirefoxCapabilities(){           
            DesiredCapabilities lCapability = DesiredCapabilities.Firefox();
            lCapability.SetCapability( "firefox_profile", getFirefoxOptions() );
            return lCapability;
        }

        private ChromeOptions getChromeOptions()
        {
            ChromeOptions chromeOptions= new ChromeOptions();
            if(this.profile!=null){
                chromeOptions.AddArgument("user-data-dir=" + this.profile);
            }
            if(this.preferences!=null){
                chromeOptions.AddAdditionalCapability("chrome.prefs", this.preferences);
            }
            if(this.extensions!=null){
                chromeOptions.AddExtensions(this.extensions);
            }
            if(this.proxy!=null){
                chromeOptions.AddArgument("--proxy-server=" + proxy.HttpProxy);
                //chromeOptions.AddAdditionalCapability("proxy", proxy);
            }
            foreach (KeyValuePair<string, object> capability in this.capabilities){
                switch(capability.Key){
                    case "chrome.binary":
                        chromeOptions.BinaryLocation = (string)capability.Value;
                        break;
                    case "chrome.extensions":
                        chromeOptions.AddExtensions( Utils.CastToString((IEnumerable)capability.Value) );
                        break;
                    case "chrome.switches":
                        chromeOptions.AddArguments( Utils.CastToString( (IEnumerable)capability.Value ) );
                        break;
                    default:
                        chromeOptions.AddAdditionalCapability(capability.Key, capability.Value);
                        break;
                }
            }
            return chromeOptions;
        }

        private DesiredCapabilities getChromeCapabilities()
        {
            DesiredCapabilities chromeCapabilities = DesiredCapabilities.Chrome();
            ArrayList switches = new ArrayList();
            if(this.profile!=null){
                switches.Add("--user-data-dir=" + this.profile);
            }
            if(this.preferences!=null){
                chromeCapabilities.SetCapability("chrome.prefs", this.preferences);
            }
            if(this.extensions!=null){
                chromeCapabilities.SetCapability("chrome.extensions", this.extensions);
            }
            if(this.proxy!=null){
                switches.Add("--proxy-server=" + proxy.HttpProxy);
            }
            foreach (KeyValuePair<string, object> capability in this.capabilities){
                if( capability.Key == "chrome.switches") {
                    switches.AddRange( (IList)capability.Value );
                }else{
                    chromeCapabilities.SetCapability(capability.Key, capability.Value);
                }
            }
            chromeCapabilities.SetCapability("chrome.switches", switches);
            return chromeCapabilities;
        }

        private InternetExplorerOptions getInternetExplorerOptions()
        {
            InternetExplorerOptions ieoptions = new InternetExplorerOptions();
            if(this.preferences != null) throw new Exception("Preference configuration is not available for InternetExplorerDriver!");
            if(this.extensions != null) throw new Exception("Preference configuration is not available for InternetExplorerDriver!");
            if(this.proxy!=null) throw new Exception("Proxy configuration is not available for InternetExplorerDriver!");
            foreach (KeyValuePair<string, object> capability in this.capabilities){
                switch(capability.Key){
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

        private PhantomJSOptions getPhantomJSOptions()
        {
            var phantomjsOptions = new PhantomJSOptions();
            if(this.preferences != null) throw new Exception("Preference configuration is not available for InternetExplorerDriver!");
            if(this.extensions != null) throw new Exception("Preference configuration is not available for InternetExplorerDriver!");
            if(this.proxy!=null) throw new Exception("Proxy configuration is not available for InternetExplorerDriver!");
            foreach (var capability in this.capabilities)
                phantomjsOptions.AddAdditionalCapability(capability.Key, capability.Value);
            return phantomjsOptions;
        }

        public int Timeout{
            get{return this.timeout;}
            set{this.setTimeout(value);}
        }

        /// <summary>Starts a new Selenium testing session</summary>
        /// <param name="browser">Name of the browser : firefox, ie, chrome, phantomjs</param>
        /// <param name="url">The base URL</param>
        /// <param name="directory">Optional - Directory path for drivers or binaries</param>
        /// <example>
        ///     WebDriver driver = New WebDriver()
        ///     driver.start "firefox", "http://www.google.com"
        ///     driver.open "/"
        /// </example>
        public void start(string browser, String url, [Optional][DefaultParameterValue("")]String directory){
            this.isStartedRemotely = false;
            if(!String.IsNullOrEmpty(directory)){
                if(!System.IO.Directory.Exists(directory)) throw new ApplicationException("Direcory not found : " + directory);
            }else{
                directory =  System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            DesiredCapabilities capa = new DesiredCapabilities(this.capabilities);
            switch (browser.ToLower().Replace("*", "")) {
                case "firefox": case "ff":
                    this.webDriver = new OpenQA.Selenium.Firefox.FirefoxDriver( getFirefoxOptions() );
                    break;
                case "cr": case "chrome":
                    //ChromeDriverService crService = ChromeDriverService.CreateDefaultService(directory);
                    //crService.SuppressInitialDiagnosticInformation = true;
                    //crService.LogPath = null;
                    //this.webDriver = new OpenQA.Selenium.Chrome.ChromeDriver(crService, getChromeOptions());
                    this.webDriver = new OpenQA.Selenium.Chrome.ChromeDriver(directory, getChromeOptions());
                    break;
                case "phantomjs": case "pjs":
                    if (this.profile != null) throw new Exception("Profile configuration is not available for PhantomJS driver!");
                    if (this.preferences != null) throw new Exception("Preference configuration is not available for PhantomJS driver!");
                    if (this.extensions != null) throw new Exception("Extension configuration is not available for PhantomJS driver!");
                    if (this.proxy != null) throw new Exception("Proxy configuration is not available for PhantomJS driver!");
                    this.webDriver = new OpenQA.Selenium.PhantomJS.PhantomJSDriver(directory, getPhantomJSOptions());
                    break;
                case "internetexplorer": case "iexplore": case "ie":
                case "internetexplorer64": case "iexplore64": case "ie64":
                    if(browser.EndsWith("64")) directory += @"\ie64";
                    if(this.profile != null) throw new Exception("Profile configuration is not available for InternetExplorerDriver!");
                    if(this.preferences != null) throw new Exception("Preference configuration is not available for InternetExplorerDriver!");
                    if (this.extensions != null) throw new Exception("Extension configuration is not available for InternetExplorerDriver!");
                    if(this.proxy!=null) throw new Exception("Proxy configuration is not available for InternetExplorerDriver!");
                    this.webDriver = new OpenQA.Selenium.IE.InternetExplorerDriver(directory, getInternetExplorerOptions());
                    break;
                case "safari": case "sa":
                    throw new Exception("SafariDriver is not yet implemented. Use remote driver insted.");
                default:
                    throw new ApplicationException("Browser <" + browser + "> is not available !  \nAvailable are Firefox, IE, Chrome and PhantomJS");
            }
            WebDriver.currentWebDriver = this.webDriver;
            this.webDriverBacked = new Selenium.WebDriverBackedSelenium(this.webDriver, url);
            InvokeWd(() => webDriverBacked.Start());
            this.setTimeout(this.timeout);
            this.baseUrl = url.TrimEnd('/');
            this.timerhotkey.Start();
        }

        /// <summary>Starts remotely a new Selenium testing session</summary>
        /// <param name="browser">Name of the browser : firefox, ie, chrome, phantomjs, htmlunit, htmlunitwithjavascript, android, ipad, opera</param>
        /// <param name="remoteAddress">Remote url address (ex : "http://localhost:4444/wd/hub")</param>
        /// <param name="url">Base URL</param>
        public void startRemotely(string browser, String remoteAddress, String url){
            this.isStartedRemotely = true;
            DesiredCapabilities lCapability;
            browser = browser.ToLower().Replace("*", "");
            switch (browser) {
                case "ff": case "firefox": 
                    lCapability = DesiredCapabilities.Firefox();
                    lCapability.SetCapability( "firefox_profile", getFirefoxOptions() );
                    break;
                case "cr": case "chrome":
                    lCapability = getChromeCapabilities();
                    break;
                case "phantomjs": case "pjs":
                    lCapability = (DesiredCapabilities)getPhantomJSOptions().ToCapabilities();
                    break;
                default:
                    switch (browser) {
                        case "internetexplorer": case "iexplore": case "ie": lCapability = DesiredCapabilities.InternetExplorer();break;
                        case "htmlunit": lCapability = DesiredCapabilities.HtmlUnit(); break;
                        case "htmlunitwithjavascript": lCapability = DesiredCapabilities.HtmlUnitWithJavaScript(); break;
                        case "android": lCapability = DesiredCapabilities.Android(); break;
                        case "ipad": lCapability = DesiredCapabilities.IPad(); break;
                        case "opera": lCapability = DesiredCapabilities.Opera(); break;
                        default: throw new ApplicationException("Remote browser <" + browser + "> is not available !  \nChoose between Firefox, IE, Chrome, HtmlUnit, HtmlUnitWithJavaScript, \nAndroid, IPad, Opera, PhantomJs");
                    }
                    if(this.capabilities!=null){
                        foreach (KeyValuePair<string, object> capability in this.capabilities){
                            lCapability.SetCapability(capability.Key, capability.Value);
                        }
                    }
                    break;
            }
            WebDriver.currentWebDriver = this.webDriver;
            this.webDriver = new RemoteWebDriver(new Uri(remoteAddress), lCapability);
            this.webDriverBacked = new Selenium.WebDriverBackedSelenium(this.webDriver, url);
            InvokeWd(() => webDriverBacked.Start());
            this.setTimeout(this.timeout);
            this.baseUrl = url.TrimEnd('/');
            this.timerhotkey.Start();
        }

		/// <summary>Ends the current Selenium testing session (normally killing the browser)</summary>
        public void stop()
        {
            this.webDriverBacked.Stop();
            this.timerhotkey.Stop();
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
            this.profile = directory;
        }

        /// <summary>Set a specific preference for the firefox webdriver</summary>
        /// <param name="key">Préférence key</param>
        /// <param name="value">Préférence value</param>
        public void setPreference(string key, object value) {
            if (this.preferences == null) this.preferences = new Dictionary<string,object>();
            this.preferences.Add(key, value);
        }

        /// <summary>Set a specific capability for the webdriver</summary>
        /// <param name="key">Capability key</param>
        /// <param name="value">Capability value</param>
        public void setCapability(string key, object value) {
            this.capabilities[key] = value;
        }
        
        /// <summary>Add an extension to the browser (For Firefox and Chrome only)</summary>
        /// <param name="extensionPath">Path to the extension</param>
        public void addExtension(string extensionPath) {
            if (this.extensions == null) this.extensions = new List<string>();
            this.extensions.Add(extensionPath);
        }

        /// <summary>Set a specific proxy for the webdriver</summary>
        /// <param name="url">Proxy URL</param>
        /// <param name="isAutoConfigURL">Is an auto-config URL</param>
        public void setProxy(string url, [Optional][DefaultParameterValue(false)]bool isAutoConfigURL) {
            this.proxy = new Proxy();
            if(isAutoConfigURL)
                this.proxy.ProxyAutoConfigUrl = url;
            else{
                this.proxy.HttpProxy = url;
                this.proxy.FtpProxy = url;
                this.proxy.SslProxy = url;
            }
        }

        /// <summary>"Opens an URL in the test frame. This accepts both relative and absolute URLs."</summary>
        /// <param name="url">URL</param>
        public void open(String url){
            if(!url.Contains("://")){
                 url = this.baseUrl + '/' + url.TrimStart('/');
            }
            InvokeWd(() => webDriverBacked.Open(url));
        }

        /// <summary>Wait the specified time in millisecond before executing the next command</summary>
        /// <param name="timems">Time to wait in millisecond</param>
        public void sleep(int timems)
        {
            var endTime = DateTime.Now.AddMilliseconds(timems);
            do{
                Thread.Sleep(30);
                this.CheckCanceled();
            }while(DateTime.Now < endTime);
        }

        /// <summary>Wait the specified time in millisecond before executing the next command</summary>
        /// <param name="timems">Time to wait in millisecond</param>
        public void wait(int timems)
        {
            this.sleep(timems);
        }

        /// <summary>Wait the specified time in millisecond before executing the next command</summary>
        /// <param name="timems">Time to wait in millisecond</param>
        public void pause(int timems)
        {
            this.sleep(timems);
        }

        /// <summary>Specifies the amount of time the driver should wait when searching for an element if it is not immediately present.</summary>
        /// <param name="timeoutms">timeout in millisecond</param>
		public void setImplicitWait(int timeoutms) {
            this.webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(timeoutms));
        }

        /// <summary> Specifies the amount of time that Selenium will wait for actions to complete. The default timeout is 30 seconds.</summary>
        /// <param name="timeoutms">timeout in milliseconds, after which an error is raised</param>
        public void setTimeout(int timeoutms) {
            this.timeout = timeoutms;
            if(this.webDriver!=null){
                try{
                    //todo : remove silent exception once the chrome issue is resolved (Issue 4448)
                    this.webDriver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromMilliseconds(timeoutms));
                }catch(Exception){}
                this.webDriver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromMilliseconds(timeoutms));
                this.webDriverBacked.SetTimeout(this.timeout.ToString());
            }
        }

        /// <summary>Deprecated. Use copyScreenshot instead</summary>
        public void captureScreenshotToClipboard() {
            throw new Exception("captureScreenshotToClipboard is deprecated, use copyScreenshot instead");
        }

        /// <summary>Deprecated. Use getScreenshot instead</summary>
        public object captureScreenshotToImage(){
            throw new Exception("captureScreenshotToImage is deprecated, use getScreenshot instead");
        }

        /// <summary>Undo the effect of calling chooseCancelOnNextConfirmation. Note that Selenium's overridden window.confirm() function will normally automatically return true, as if the user had manually clicked OK, so you shouldn't need to use this command unless for some reason you need to change your mind prior to the next confirmation. After any confirmation, Selenium will resume using the default behavior for future confirmations, automatically returning true (OK) unless/until you explicitly call chooseCancelOnNextConfirmation for each confirmation. Take note - every time a confirmation comes up, you must consume it with a corresponding getConfirmation, or else the next selenium operation will fail. </summary>
        public void chooseOkOnNextConfirmation() {
            InvokeWd(() => this.webDriver.SwitchTo().Alert().Dismiss()); 
        }

        /// <summary>By default, Selenium's overridden window.confirm() function will return true, as if the user had manually clicked OK; after running this command, the next call to confirm() will return false, as if the user had clicked Cancel. Selenium will then resume using the default behavior for future confirmations, automatically returning true (OK) unless/until you explicitly call this command for each confirmation.  Take note - every time a confirmation comes up, you must consume it with a corresponding getConfirmation, or else the next selenium operation will fail.</summary>
        public void chooseCancelOnNextConfirmation() {
            InvokeWd(() => this.webDriver.SwitchTo().Alert().Accept()); 
        }

        /// <summary>Resize currently selected window to take up the entire screen </summary>
        public void windowMaximize() {
            // With chrome: use maximize switch
            // With IE: use windowMaximize javascript function
            // With Firefox: use Manage().Window.Maximize() method
            if(this.isStartedRemotely){
                InvokeWd(() => this.webDriver.Manage().Window.Maximize());
                //InvokeWd(() => this.webDriverBacked.WindowMaximize());
            }else{
                string handle = this.webDriver.CurrentWindowHandle;
                Utils.maximizeForegroundWindow();
            }
            
        }

      #region WebDriver Code   // Following funtion are webdriver related

        /// <summary>Loads a web page in the current browser session.</summary>
        /// <param name="url">URL</param>
        public void get(String url){
            if(!url.Contains("://")){
                 url = this.baseUrl + '/' + url.TrimStart('/');
            }
            InvokeWd(() => this.webDriver.Navigate().GoToUrl(url));
        }

        /// <summary>Gets the source of the page last loaded by the browser.</summary>
        public String PageSource{
            get{ return this.webDriver.PageSource; }
        }

        /// <summary>Returns the current Url.</summary>
        public string Url
        {
            get { return this.webDriver.Url; }
        }

        /// <summary>Goes one step backward in the browser history.</summary>
        public void back()
        {
            this.webDriver.Navigate().Back();
        }

        /// <summary>Goes one step forward in the browser history.</summary>
        public void forward()
        {
            this.webDriver.Navigate().Forward();
        }

        /// <summary>Closes the current window.</summary>
        public void close() { 
            this.webDriver.Close();
        }

        /// <summary>Returns the handle of the current window.</summary>
        public string WindowHandle
        {
            get{ return this.webDriver.CurrentWindowHandle; }
        }

        /// <summary>Returns the handles of all windows within the current session.</summary>
        public string[] WindowHandles
        {
            get{ 
                ReadOnlyCollection<string> collection = this.webDriver.WindowHandles;
                string[] handles = new string[collection.Count];
                collection.CopyTo(handles,0);
                return handles;
            }
        }

        /// <summary>Returns the element with focus, or BODY if nothing has focus.</summary>
        public WebElement ActiveElement
        {
            get{ 
                return new WebElement(this, this.webDriver.SwitchTo().ActiveElement());
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
        public Object executeScript(String script, [Optional][DefaultParameterValue(null)]Object arguments)
        {
            Object result = ((OpenQA.Selenium.IJavaScriptExecutor)this.webDriver).ExecuteScript("try{" + script + "}catch(e){return 'error:'+e.message;}", arguments);
            if (result != null && result.ToString().StartsWith("error:")) throw new ApplicationException("JavaScript " + result.ToString());
            return result;
        }

        /// <summary>Find the first WebElement using the given method.</summary>
        /// <param name="by">Methode</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElement(ref Object by, [Optional][DefaultParameterValue(0)]int timeoutms)
        {
            if (((By)by).base_ == null) throw new NullReferenceException("The locating mechanism is null!");
            return findElement(((By)by).base_, timeoutms);
        }

        /// <summary>Finds an element by name.</summary>
        /// <param name="name">The name of the element to find.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByName(String name, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElement(OpenQA.Selenium.By.Name(name), timeoutms);
        }

        /// <summary>Finds an element by XPath.</summary>
        /// <param name="xpath">The xpath locator of the element to find.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByXPath(String xpath, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElement(OpenQA.Selenium.By.XPath(xpath), timeoutms);
        }

        /// <summary>Finds an element by id.</summary>
        /// <param name="id">The id of the element to be found.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementById(String id, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElement(OpenQA.Selenium.By.Id(id), timeoutms);
        }

        /// <summary>Finds an element by class name.</summary>
        /// <param name="classname">The class name of the element to find.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByClassName(String classname, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElement(OpenQA.Selenium.By.ClassName(classname), timeoutms);
        }

        /// <summary>Finds an element by css selector.</summary>
        /// <param name="cssselector">The css selector to use when finding elements.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByCssSelector(String cssselector, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElement(OpenQA.Selenium.By.CssSelector(cssselector), timeoutms);
        }

        /// <summary>Finds an element by link text.</summary>
        /// <param name="linktext">The text of the element to be found.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByLinkText(String linktext, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElement(OpenQA.Selenium.By.LinkText(linktext), timeoutms);
        }

        /// <summary>Finds an element by a partial match of its link text.</summary>
        /// <param name="partiallinktext">The text of the element to partially match on.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByPartialLinkText(String partiallinktext, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElement(OpenQA.Selenium.By.PartialLinkText(partiallinktext), timeoutms);
        }

        /// <summary>Finds an element by tag name.</summary>
        /// <param name="tagname">The tag name of the element to find.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByTagName(String tagname, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElement(OpenQA.Selenium.By.TagName(tagname), timeoutms);
        }

        public bool isElementPresent(ref object by, [Optional][DefaultParameterValue(0)]int timeoutms){
            try{
                return findElement(((By)by).base_, timeoutms) != null; 
            }catch(Exception){
                return false;
            }
        }

	    private WebElement findElement(OpenQA.Selenium.By by, int timeoutms){
            object ret;
			if(timeoutms>0){
                ret = this.WaitUntilObject(()=>this.webDriver.FindElement(by), timeoutms);
			}else{
                ret = this.webDriver.FindElement(by);
            }
            return new WebElement(this, (OpenQA.Selenium.IWebElement)ret);
        }

        /// <summary>Find all elements within the current context using the given mechanism.</summary>
        /// <param name="by">The locating mechanism to use</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>A list of all WebElements, or an empty list if nothing matches</returns>
	    public WebElement[] findElements(ref object by, int timeoutms){
            if(((By)by).base_ == null) throw new NullReferenceException("The locating mechanism is null!");
            return findElements(((By)by).base_, timeoutms);
        }

        /// <summary>Finds elements by name.</summary>
        /// <param name="name">The name of the elements to find.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElement[] findElementsByName(String name, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElements(OpenQA.Selenium.By.Name(name), timeoutms);
        }

        /// <summary>Finds multiple elements by xpath.</summary>
        /// <param name="xpath">The xpath locator of the elements to be found.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElement[] findElementsByXPath(String xpath, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElements(OpenQA.Selenium.By.XPath(xpath), timeoutms);
        }

        /// <summary>Finds multiple elements by id.</summary>
        /// <param name="id">The id of the elements to be found.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElement[] findElementsById(String id, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElements(OpenQA.Selenium.By.Id(id), timeoutms);
        }

        /// <summary>Finds elements by class name.</summary>
        /// <param name="classname">The class name of the elements to find.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElement[] findElementsByClassName(String classname, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElements(OpenQA.Selenium.By.ClassName(classname), timeoutms);
        }

        /// <summary>Finds elements by css selector.</summary>
        /// <param name="cssselector">The css selector to use when finding elements.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElement[] findElementsByCssSelector(String cssselector, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElements(OpenQA.Selenium.By.CssSelector(cssselector), timeoutms);
        }

        /// <summary>Finds elements by link text.</summary>
        /// <param name="linktext">The text of the elements to be found.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElement[] findElementsByLinkText(String linktext, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElements(OpenQA.Selenium.By.LinkText(linktext), timeoutms);
        }

        /// <summary>Finds elements by a partial match of their link text.</summary>
        /// <param name="partiallinktext">The text of the element to partial match on.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElement[] findElementsByPartialLinkText(String partiallinktext, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElements(OpenQA.Selenium.By.PartialLinkText(partiallinktext), timeoutms);
        }

        /// <summary>Finds elements by tag name.</summary>
        /// <param name="tagname">The tag name the use when finding elements.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElement[] findElementsByTagName(String tagname, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElements(OpenQA.Selenium.By.TagName(tagname), timeoutms);
        }

	    private WebElement[] findElements(OpenQA.Selenium.By by, int timeoutms){
			if(timeoutms>0){
                Object ret = this.WaitUntilObject(delegate(){
                    return this.webDriver.FindElements(by);
                }, timeoutms);
                return WebElement.GetWebElements(this, (ReadOnlyCollection<OpenQA.Selenium.IWebElement>)ret);
			}
            return WebElement.GetWebElements(this, this.webDriver.FindElements(by));
        }

        /// <summary>Gets the screenshot of the current window</summary>
        /// <returns>Image</returns>
        public Image getScreenshot()
        {
            OpenQA.Selenium.Screenshot ret = ((OpenQA.Selenium.ITakesScreenshot)webDriver).GetScreenshot();
            if (ret == null) throw new ApplicationException("Method <captureScreenshotToPdf> failed !\nReturned value is empty");
            return new Image(ret.AsByteArray);
        }

        /// <summary>Sends a sequence of keystrokes to the browser.</summary>
        /// <param name="keysToSend">Sequence of keys</param>
        public void sendKeys(string keysToSend){
            new OpenQA.Selenium.Interactions.Actions(this.webDriver).SendKeys(keysToSend).Perform();;
        }

        public Cookie getCookie(string name)
        {
            throw new Exception("Not implemented yet!");
        }

        public Cookie[] getCookies()
        {
            throw new Exception("Not implemented yet!");
        }

        public string Title
        {
            get { return this.webDriver.Title; }
        }

        /// <summary>Switches focus to the specified window.</summary>
        /// <param name="windowName">The name of the window to switch to.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Current web driver</returns>
        public WebDriver switchToWindow(string windowName, [Optional][DefaultParameterValue(0)]int timeoutms)
        {
            if (timeoutms > 0){
                this.WaitUntilObject(delegate(){
                    return this.webDriver.SwitchTo().Window(windowName);
                }, timeoutms);
            }else{
                this.webDriver.SwitchTo().Window(windowName);
            }
            return this;
        }

        /// <summary>Switches focus to the specified frame, by index or name.</summary>
        /// <param name="index_or_name">The name of the window to switch to, or an integer representing the index to switch to.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Current web driver</returns>
        public WebDriver switchToFrame(object index_or_name, [Optional][DefaultParameterValue(0)]int timeoutms)
        {
            if (index_or_name is string){
                string windowName = (string)index_or_name;
                if (timeoutms > 0){
                    this.WaitUntilObject(()=>this.webDriver.SwitchTo().Frame(windowName), timeoutms);
                }else{
                    this.webDriver.SwitchTo().Frame(windowName);
                }
            }else{
                int frameIndex = (int)index_or_name;
                if (timeoutms > 0){
                    this.WaitUntilObject(()=>this.webDriver.SwitchTo().Frame(frameIndex), timeoutms);
                }else{
                    this.webDriver.SwitchTo().Frame(frameIndex);
                }
            }
            return this;
        }

        /// <summary>Switches focus to an alert on the page.</summary>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Focused alert</returns>
        public Alert switchToAlert([Optional][DefaultParameterValue(0)]int timeoutms)
        {
            Object alert;
            if (timeoutms > 0){
                alert = this.WaitUntilObject( ()=>this.webDriver.SwitchTo().Alert(), timeoutms);
            }else{
                try{
                    alert = this.webDriver.SwitchTo().Alert();
                }catch(Exception){
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
        public bool isMatch(string pattern){
            return Regex.IsMatch(this.PageSource, pattern);
        }

        /// <summary>Searches the specified source code for an occurrence of the regular expression supplied in the pattern parameter.</summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>Matching strings</returns>
        public object match(string pattern){
            Match match = Regex.Match(this.PageSource, pattern);
            if(match.Groups != null){
                string[] lst = new string[match.Groups.Count];
                for(int i=0;i<match.Groups.Count;i++)
                    lst[i] = match.Groups[i].Value;
                return lst;
            }else{
                return match.Value;
            }
        }

      #endregion Regex

    }

}
