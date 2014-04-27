using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using System.Net;
using System.Text;
using System.IO;

namespace SeleniumWrapper {
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
    public partial class WebDriver : WebDriverCore, IWebDriver {

        private bool _hideCommandPromptWindow = true;

        //public delegate void EndOfCommandDelegate();
        //public event EndOfCommandDelegate EndOfCommand;

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
        public Actions Actions {
            get { return new Actions(WebDriver); }
        }

        /// <summary>Starts a new Selenium testing session</summary>
        /// <param name="browser">Name of the browser : firefox, ie, chrome, phantomjs</param>
        /// <param name="baseUrl">The base URL</param>
        /// <param name="useLastSession">Optional - Try to use the previous session if the browser is still openned (Excel only)</param>
        /// <example>
        /// <code lang="vbs">	
        ///     WebDriver driver = New WebDriver()
        ///     driver.start "firefox", "http://www.google.com"
        ///     driver.open "/"
        /// </code>
        /// </example>
        public void start(string browser, String baseUrl = null, bool useLastSession = false) {
            if (useLastSession && CopyStaticDriver(baseUrl)) return;
            _isStartedRemotely = false;
            var dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            DesiredCapabilities capa = new DesiredCapabilities(_capabilities);
            switch (browser.ToLower().Replace("*", "")) {
                case "firefox": case "ff":
                    WebDriver = new OpenQA.Selenium.Firefox.FirefoxDriver(getFirefoxOptions());
                    break;
                case "cr": case "chrome":
                    ChromeDriverService crService = ChromeDriverService.CreateDefaultService(dir);
                    crService.EnableVerboseLogging = false;
                    crService.SuppressInitialDiagnosticInformation = true;
                    crService.HideCommandPromptWindow = this._hideCommandPromptWindow;
                    WebDriver = new OpenQA.Selenium.Chrome.ChromeDriver(crService, getChromeOptions());
                    break;
                case "phantomjs": case "pjs":
                    var pjsService = PhantomJSDriverService.CreateDefaultService(dir);
                    pjsService.SuppressInitialDiagnosticInformation = true;
                    pjsService.HideCommandPromptWindow = this._hideCommandPromptWindow;
                    WebDriver = new OpenQA.Selenium.PhantomJS.PhantomJSDriver(pjsService, getPhantomJSOptions());
                    break;
                case "internetexplorer": case "iexplore": case "ie":
                case "internetexplorer64": case "iexplore64": case "ie64":
                    var ieService = OpenQA.Selenium.IE.InternetExplorerDriverService.CreateDefaultService(dir);
                    ieService.SuppressInitialDiagnosticInformation = true;
                    ieService.LoggingLevel = OpenQA.Selenium.IE.InternetExplorerDriverLogLevel.Error;
                    ieService.HideCommandPromptWindow = this._hideCommandPromptWindow;
                    WebDriver = new OpenQA.Selenium.IE.InternetExplorerDriver(ieService, getInternetExplorerOptions());
                    break;
                case "safari": case "sa":
                    WebDriver = new OpenQA.Selenium.Safari.SafariDriver(getSafariOptions());
                    break;
                default:
                    throw new ApplicationException("Browser <" + browser + "> is not available !  \nAvailable are Firefox, IE, Chrome, Safari and PhantomJS");
            }
            this.setTimeout(_timeout);
            if (!string.IsNullOrEmpty(baseUrl))
                _baseUrl = baseUrl.TrimEnd('/');
            _timerhotkey.Start();
        }

        /// <summary>Gets or sets a value indicating whether the command prompt window of the service should be hidden.</summary>
        public bool HideCommandPromptWindow {
            get{ return this._hideCommandPromptWindow; }
            set { this._hideCommandPromptWindow = value; }
        }

        /// <summary>Starts remotely a new Selenium testing session</summary>
        /// <param name="browser">Name of the browser : firefox, ie, chrome, phantomjs, htmlunit, htmlunitwithjavascript, android, ipad, opera</param>
        /// <param name="remoteAddress">Remote url address (ex : "http://localhost:4444/wd/hub")</param>
        /// <param name="baseUrl">Base URL</param>
        public void startRemotely(string browser, String remoteAddress, String baseUrl = null) {
            _isStartedRemotely = true;
            ICapabilities lCapability;
            browser = browser.ToLower().Replace("*", "");
            switch (browser) {
                case "firefox": case "ff":
                    lCapability = DesiredCapabilities.Firefox();
                    (lCapability as DesiredCapabilities).SetCapability("firefox_profile", getFirefoxOptions());
                    break;
                case "chrome": case "cr":
                    lCapability = getChromeOptions().ToCapabilities();
                    break;
                case "phantomjs": case "pjs":
                    lCapability = getPhantomJSOptions().ToCapabilities();
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
                        case "safari": lCapability = DesiredCapabilities.Safari(); break;
                        default: throw new ApplicationException("Remote browser <" + browser + "> is not available !  \nChoose between Firefox, IE, Chrome, HtmlUnit, HtmlUnitWithJavaScript, \nAndroid, IPad, Opera, PhantomJs");
                    }
                    if (_capabilities != null) {
                        foreach (KeyValuePair<string, object> capability in _capabilities)
                            (lCapability as DesiredCapabilities).SetCapability(capability.Key, capability.Value);
                    }
                    break;
            }
            if (string.IsNullOrEmpty(remoteAddress) || "localhost".Equals(remoteAddress))
                WebDriver = new RemoteWebDriver(lCapability);
            else
                WebDriver = new RemoteWebDriver(new Uri(remoteAddress), lCapability);
            this.setTimeout(_timeout);
            if(!string.IsNullOrEmpty(baseUrl))
                _baseUrl = baseUrl.TrimEnd('/');
            _timerhotkey.Start();
        }

        /// <summary>Ends the current Selenium testing session (normally killing the browser)</summary>
        public void stop() {
            _webDriverCoreStatic = null;
            try {
                _webDriver.Quit();
            } catch { }
            _timerhotkey.Stop();
        }

        /// <summary>Set a specific profile for the firefox webdriver</summary>
        /// <param name="nameOrDirectory">Profil name (Firefox only) or directory (Firefox and Chrome)</param>
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
        public void setProfile(string nameOrDirectory) {
            _profile = nameOrDirectory;
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

        /// <summary>Add an argument to be appended to the command line to launch the browser.</summary>
        /// <param name="argument">Argument</param>
        public void addArgument(string argument) {
            if (_arguments == null)
                _arguments = new List<string>();
            _arguments.Add(argument);
        }

        /// <summary>Set a specific proxy for the webdriver</summary>
        /// <param name="url">Proxy URL</param>
        /// <param name="isAutoConfigURL">Is an auto-config URL</param>
        public void setProxy(string url, bool isAutoConfigURL = false) {
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
            get(url);
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
            WebDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(timeoutms));
        }

        /// <summary> Specifies the amount of time that Selenium will wait for actions to complete. The default timeout is 30 seconds.</summary>
        /// <param name="timeoutms">timeout in milliseconds, after which an error is raised</param>
        public void setTimeout(int timeoutms) {
            _timeout = timeoutms;
            if (WebDriver != null) {
                try {
                    //todo : remove silent exception once the chrome issue is resolved (Issue 4448)
                    WebDriver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromMilliseconds(timeoutms));
                } catch (Exception) { }
                WebDriver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromMilliseconds(timeoutms));
                if(_webDriverBacked != null)
                    _webDriverBacked.SetTimeout(_timeout.ToString());
            }
        }

        /// <summary>Saves the entire contents of the current window canvas to a PNG file. Contrast this with the captureScreenshot command, which captures the contents of the OS viewport (i.e. whatever is currently being displayed on the monitor), and is implemented in the RC only. Currently this only works in Firefox when running in chrome mode, and in IE non-HTA using the EXPERIMENTAL \"Snapsie\" utility. The Firefox implementation is mostly borrowed from the Screengrab! Firefox extension. Please see http://www.screengrab.org and http://snapsie.sourceforge.net/ for details. the path to the file to persist the screenshot as. No filename extension will be appended by default. Directories will not be created if they do not exist, and an exception will be thrown, possibly by native code.a kwargs string that modifies the way the screenshot is captured. Example: \"background=#CCFFDD\" . Currently valid options: backgroundthe background CSS for the HTML document. This may be useful to set for capturing screenshots of less-than-ideal layouts, for example where absolute positioning causes the calculation of the canvas dimension to fail and a black background is exposed (possibly obscuring black text).</summary>
        public void captureEntirePageScreenshot(String filename, String kwargs = null) {
            getScreenshot().SaveAs(filename);
        }

        /// <summary>Undo the effect of calling chooseCancelOnNextConfirmation. Note that Selenium's overridden window.confirm() function will normally automatically return true, as if the user had manually clicked OK, so you shouldn't need to use this command unless for some reason you need to change your mind prior to the next confirmation. After any confirmation, Selenium will resume using the default behavior for future confirmations, automatically returning true (OK) unless/until you explicitly call chooseCancelOnNextConfirmation for each confirmation. Take note - every time a confirmation comes up, you must consume it with a corresponding getConfirmation, or else the next selenium operation will fail. </summary>
        public void chooseOkOnNextConfirmation() {
            InvokeWd(() => WebDriver.SwitchTo().Alert().Dismiss());
        }

        /// <summary>By default, Selenium's overridden window.confirm() function will return true, as if the user had manually clicked OK; after running this command, the next call to confirm() will return false, as if the user had clicked Cancel. Selenium will then resume using the default behavior for future confirmations, automatically returning true (OK) unless/until you explicitly call this command for each confirmation.  Take note - every time a confirmation comes up, you must consume it with a corresponding getConfirmation, or else the next selenium operation will fail.</summary>
        public void chooseCancelOnNextConfirmation() {
            InvokeWd(() => WebDriver.SwitchTo().Alert().Accept());
        }

        /// <summary>Resize currently selected window to take up the entire screen </summary>
        public void windowMaximize() {
            // With chrome: use maximize switch
            // With IE: use windowMaximize javascript function
            // With Firefox: use Manage().Window.Maximize() method
            if (_isStartedRemotely)
                WebDriver.Manage().Window.Maximize();
            else {
                string handle = WebDriver.CurrentWindowHandle;
                Utils.maximizeForegroundWindow();
            }

        }

        #region WebDriver Code   // Following funtion are webdriver related

        /// <summary>Sets the size of the outer browser window, including title bars and window borders.</summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void setWindowSize(int width, int height) {
            WebDriver.Manage().Window.Size = new System.Drawing.Size(width, height);
        }

        /// <summary>Sets the position of the browser window relative to the upper-left corner of the screen.</summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public void setWindowPosition(int x, int y) {
            WebDriver.Manage().Window.Position = new System.Drawing.Point(x, y);
        }

        /// <summary>Maximizes the current window if it is not already maximized.</summary>
        public void maximizeWindow() {
            WebDriver.Manage().Window.Maximize();
        }

        /// <summary>Loads a web page in the current browser session.</summary>
        /// <param name="url">URL</param>
        public void get(String url) {
            if (url.Contains("://")) {
                var endDomain = url.IndexOf('/', url.IndexOf('/') + 2);
                if (endDomain != -1)
                    _baseUrl = url.Substring(0, endDomain);
                else
                    _baseUrl = url;
            } else
                url = _baseUrl + '/' + url.TrimStart('/');
            InvokeWd(() => WebDriver.Navigate().GoToUrl(url));
        }

        /// <summary>Gets the source of the page last loaded by the browser.</summary>
        public String PageSource {
            get { return WebDriver.PageSource; }
        }

        /// <summary>Returns the current Url.</summary>
        public string Url {
            get { return WebDriver.Url; }
        }

        /// <summary>Goes one step backward in the browser history.</summary>
        public void back() {
            WebDriver.Navigate().Back();
        }

        /// <summary>Goes one step forward in the browser history.</summary>
        public void forward() {
            WebDriver.Navigate().Forward();
        }

        /// <summary>Closes the current window.</summary>
        public void close() {
            WebDriver.Close();
        }

        /// <summary>Returns the handle of the current window.</summary>
        public string WindowHandle {
            get { return WebDriver.CurrentWindowHandle; }
        }

        /// <summary>Returns the handles of all windows within the current session.</summary>
        public string[] WindowHandles {
            get {
                ReadOnlyCollection<string> collection = WebDriver.WindowHandles;
                string[] handles = new string[collection.Count];
                collection.CopyTo(handles, 0);
                return handles;
            }
        }

        /// <summary>Returns the element with focus, or BODY if nothing has focus.</summary>
        public WebElement ActiveElement {
            get {
                return new WebElement(this, WebDriver.SwitchTo().ActiveElement());
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
        public Object executeScript(String script, object arguments = null) {
            var argsEncapsulated = Encapsulate(arguments);
            object argsInArray =argsEncapsulated == null ? new object[0] : argsEncapsulated is object[] ? argsEncapsulated : new object[] {argsEncapsulated};
            return Decapsulate(((OpenQA.Selenium.IJavaScriptExecutor)WebDriver).ExecuteScript(script, argsInArray));
        }

        public void waitScriptCondition(String scriptCondition, object arguments = null, int timeoutms = 5000) {
            var argsEncapsulated = Encapsulate(arguments);
            object argsInArray = argsEncapsulated == null ? new object[0] : argsEncapsulated is object[] ? argsEncapsulated : new object[] { argsEncapsulated };
            var endTime = DateTime.Now.AddMilliseconds(timeoutms);
            string errorMsg = String.Empty;
            if (!scriptCondition.TrimStart().StartsWith("return"))
                scriptCondition = "return " + scriptCondition;

            if (!scriptCondition.TrimEnd().EndsWith(";"))
                scriptCondition = scriptCondition + ";";
            while (true) {
                try {
                    errorMsg = String.Empty;
                    var result = ((OpenQA.Selenium.IJavaScriptExecutor)WebDriver).ExecuteScript(scriptCondition, argsInArray);
                    if (result != null && result is bool && (bool)result == true) return;
                } catch (Exception ex) {
                    errorMsg = ex.Message;
                }
                this.CheckCanceled();
                if (DateTime.Now > endTime)
                    throw new TimeoutException("The operation has timed out! " + errorMsg);
                Thread.Sleep(30);
            }
        }

        /// <summary>Wait for a script object(defined and not null)</summary>
        /// <param name="objectName">Object name</param>
        /// <param name="timeoutms">Optional timeout</param>
        public void waitScriptObject(String objectName, int timeoutms = 5000) {
            var endTime = DateTime.Now.AddMilliseconds(timeoutms);
            string errorMsg = String.Empty;
            var variable = new StringBuilder();
            var objects = objectName.Split('.');
            for (int i=0; i<objects.Length; i++) {
                if (i != 0)
                    variable.Append('.');
                variable.Append(objects[i]);
                var script = "return typeof " + variable + " != 'undefined' && " + variable + "  != null;";
                while (true) {
                    errorMsg = String.Empty;
                    try {
                        if ((bool)((OpenQA.Selenium.IJavaScriptExecutor)WebDriver).ExecuteScript(script))
                            break;
                    } catch (Exception ex) {
                        errorMsg = ex.Message;
                    }
                    this.CheckCanceled();
                    if (DateTime.Now > endTime)
                        throw new TimeoutException("The operation has timed out! " + errorMsg);
                    Thread.Sleep(30);
                }
            }
        }

        private Object Encapsulate(Object value) {            
            if(value is WebElement)
                return ((WebElement)value)._webElement;
            if (value is ICollection) {
                var collection = (ICollection)value;
                var array = new object[collection.Count];
                int i = 0;
                foreach (object ele in collection)
                    array[i++] = Encapsulate(ele);
                return array;
            }
            return value;
        }

        private Object Decapsulate(Object value) {
            if (value is OpenQA.Selenium.IWebElement)
                return new WebElement(this, (OpenQA.Selenium.IWebElement)value);
            if (value is ReadOnlyCollection<OpenQA.Selenium.IWebElement>)
                return new WebElementCollection(this, (ReadOnlyCollection<OpenQA.Selenium.IWebElement>)value);
            if (value is System.Collections.IDictionary) {
                var dictionary = new Dictionary((value as System.Collections.IDictionary).Count);
                foreach (DictionaryEntry item in (value as System.Collections.IDictionary))
                    dictionary.Add(item.Key, Decapsulate(item.Value));
                return dictionary;
            }
            if (value is ICollection) {
                var arrayList = new object[(value as ICollection).Count];
                int i = 0;
                foreach (object ele in (value as ICollection))
                    arrayList[i++] = Decapsulate(ele);
                return arrayList;
            }
            if (value is long)
                return (int)(long)value;
            return value;
        }

        /// <summary>Find the first WebElement using the given method.</summary>
        /// <param name="by">Methode</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElement(By by, int timeoutms = 0) {
            return null;
            //if (by.base_ == null) throw new NullReferenceException("The locating mechanism is null!");
            //return findElement(by.base_, timeoutms);
        }

        /// <summary>Finds an element by name.</summary>
        /// <param name="name">The name of the element to find.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByName(String name, int timeoutms = 0) {
            return this.findElement(OpenQA.Selenium.By.Name(name), timeoutms);
        }

        /// <summary>Finds an element by XPath.</summary>
        /// <param name="xpath">The xpath locator of the element to find.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByXPath(String xpath, int timeoutms = 0) {
            return this.findElement(OpenQA.Selenium.By.XPath(xpath), timeoutms);
        }

        /// <summary>Finds an element by id.</summary>
        /// <param name="id">The id of the element to be found.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementById(String id, int timeoutms = 0) {
            return this.findElement(OpenQA.Selenium.By.Id(id), timeoutms);
        }

        /// <summary>Finds an element by class name.</summary>
        /// <param name="classname">The class name of the element to find.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByClassName(String classname, int timeoutms = 0) {
            return this.findElement(OpenQA.Selenium.By.ClassName(classname), timeoutms);
        }

        /// <summary>Finds an element by css selector.</summary>
        /// <param name="cssselector">The css selector to use when finding elements.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByCssSelector(String cssselector, int timeoutms = 0) {
            return this.findElement(OpenQA.Selenium.By.CssSelector(cssselector), timeoutms);
        }

        /// <summary>Finds an element by link text.</summary>
        /// <param name="linktext">The text of the element to be found.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByLinkText(String linktext, int timeoutms = 0) {
            return this.findElement(OpenQA.Selenium.By.LinkText(linktext), timeoutms);
        }

        /// <summary>Finds an element by a partial match of its link text.</summary>
        /// <param name="partiallinktext">The text of the element to partially match on.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByPartialLinkText(String partiallinktext, int timeoutms = 0) {
            return this.findElement(OpenQA.Selenium.By.PartialLinkText(partiallinktext), timeoutms);
        }

        /// <summary>Finds an element by tag name.</summary>
        /// <param name="tagname">The tag name of the element to find.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByTagName(String tagname, int timeoutms = 0) {
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
                return (Boolean)InvokeWd(() => WebDriverBacked.IsElementPresent((string)locator));
            else
                throw new ArgumentException("Locator has to be a 'String' or a 'By' object!");
        }

        private WebElement findElement(OpenQA.Selenium.By by, int timeoutms) {
            try {
                object ret;
                if (timeoutms == 0)
                    ret = WebDriver.FindElement(by);
                else
                    ret = this.WaitUntilObject(() => WebDriver.FindElement(by), timeoutms);
                return new WebElement(this, (OpenQA.Selenium.IWebElement)ret);
            } catch (Exception ex){
                if(ex is NoSuchElementException || ex is TimeoutException)
                    throw new Exception("Element not found. " + "Method=" + by.ToString().ToLower().Substring(3).Replace(": ", ", value="));
                throw;
            }
        }

        /// <summary>Find all elements within the current context using the given mechanism.</summary>
        /// <param name="by">The locating mechanism to use</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>A list of all WebElements, or an empty list if nothing matches</returns>
        public WebElementCollection findElements(By by, int timeoutms = 0) {
            if (by.base_ == null) throw new NullReferenceException("The locating mechanism is null!");
            return findElements(by.base_, timeoutms);
        }

        /// <summary>Finds elements by name.</summary>
        /// <param name="name">The name of the elements to find.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElementCollection findElementsByName(String name, int timeoutms = 0) {
            return this.findElements(OpenQA.Selenium.By.Name(name), timeoutms);
        }

        /// <summary>Finds multiple elements by xpath.</summary>
        /// <param name="xpath">The xpath locator of the elements to be found.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElementCollection findElementsByXPath(String xpath, int timeoutms = 0) {
            return this.findElements(OpenQA.Selenium.By.XPath(xpath), timeoutms);
        }

        /// <summary>Finds multiple elements by id.</summary>
        /// <param name="id">The id of the elements to be found.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElementCollection findElementsById(String id, int timeoutms = 0) {
            return this.findElements(OpenQA.Selenium.By.Id(id), timeoutms);
        }

        /// <summary>Finds elements by class name.</summary>
        /// <param name="classname">The class name of the elements to find.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElementCollection findElementsByClassName(String classname, int timeoutms = 0) {
            return this.findElements(OpenQA.Selenium.By.ClassName(classname), timeoutms);
        }

        /// <summary>Finds elements by css selector.</summary>
        /// <param name="cssselector">The css selector to use when finding elements.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElementCollection findElementsByCssSelector(String cssselector, int timeoutms = 0) {
            return this.findElements(OpenQA.Selenium.By.CssSelector(cssselector), timeoutms);
        }

        /// <summary>Finds elements by link text.</summary>
        /// <param name="linktext">The text of the elements to be found.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElementCollection findElementsByLinkText(String linktext, int timeoutms = 0) {
            return this.findElements(OpenQA.Selenium.By.LinkText(linktext), timeoutms);
        }

        /// <summary>Finds elements by a partial match of their link text.</summary>
        /// <param name="partiallinktext">The text of the element to partial match on.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElementCollection findElementsByPartialLinkText(String partiallinktext, int timeoutms = 0) {
            return this.findElements(OpenQA.Selenium.By.PartialLinkText(partiallinktext), timeoutms);
        }

        /// <summary>Finds elements by tag name.</summary>
        /// <param name="tagname">The tag name the use when finding elements.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Array of WebElements</returns>
        public WebElementCollection findElementsByTagName(String tagname, int timeoutms = 0) {
            return this.findElements(OpenQA.Selenium.By.TagName(tagname), timeoutms);
        }

        private WebElementCollection findElements(OpenQA.Selenium.By by, int timeoutms = 0) {
            try{
                if (timeoutms > 0) {
                    var ret = this.WaitUntilObject(() => WebDriver.FindElements(by), timeoutms);
                    return new WebElementCollection(this, (ReadOnlyCollection<OpenQA.Selenium.IWebElement>)ret);
                }
                return new WebElementCollection(this, WebDriver.FindElements(by));
            } catch (Exception ex) {
                if (ex is NoSuchElementException || ex is TimeoutException)
                    throw new Exception("Elements not found. " + "Method=" + by.ToString().ToLower().Substring(3).Replace(": ", ", value="));
                throw;
            }
        }

        /// <summary>Gets the screenshot of the current window</summary>
        /// <returns>Image</returns>
        public Image getScreenshot(int delayms = 0) {
            if (delayms != 0)
                Thread.Sleep(delayms);
            OpenQA.Selenium.Screenshot ret = ((OpenQA.Selenium.ITakesScreenshot)WebDriver).GetScreenshot();
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
        public void sendKeys(string keysOrModifier, string keys = null) {
            if (string.IsNullOrEmpty(keys))
                new OpenQA.Selenium.Interactions.Actions(WebDriver).SendKeys(keysOrModifier).Perform();
            else
                new OpenQA.Selenium.Interactions.Actions(WebDriver).KeyDown(keysOrModifier).SendKeys(keys).KeyUp(keysOrModifier).Build().Perform();
        }

        /// <summary>Sends keystrokes to the active application using the windows SendKeys methode.</summary>
        /// <param name="keys">The string of keystrokes to send.</param>
        public void sendKeysNat(string keys) {
            System.Windows.Forms.SendKeys.Send(keys);
        }

        public OpenQA.Selenium.Cookie getCookie(string name) {
            throw new Exception("Not implemented yet!");
        }

        public OpenQA.Selenium.Cookie[] getCookies() {
            throw new Exception("Not implemented yet!");
        }

        public string Title {
            get { return WebDriver.Title; }
        }

        /// <summary>Switches focus to the specified window.</summary>
        /// <param name="windowName">The name of the window to switch to.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Current web driver</returns>
        public WebDriver switchToWindow(string windowName, int timeoutms = 0) {
            if (timeoutms > 0)
                this.WaitUntilObject(() => WebDriver.SwitchTo().Window(windowName), timeoutms);
            else
                WebDriver.SwitchTo().Window(windowName);
            return this;
        }

        /// <summary>Switches focus to the specified frame, by index or name.</summary>
        /// <param name="index_or_name">The name of the window to switch to, or an integer representing the index to switch to.</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Current web driver</returns>
        public WebDriver switchToFrame(object index_or_name, int timeoutms = 0) {
            if (index_or_name is string) {
                string windowName = (string)index_or_name;
                if (timeoutms > 0)
                    this.WaitUntilObject(() => WebDriver.SwitchTo().Frame(windowName), timeoutms);
                else
                    WebDriver.SwitchTo().Frame(windowName);
            } else {
                int frameIndex = (int)index_or_name;
                if (timeoutms > 0)
                    this.WaitUntilObject(() => WebDriver.SwitchTo().Frame(frameIndex), timeoutms);
                else
                    WebDriver.SwitchTo().Frame(frameIndex);
            }
            return this;
        }

        /// <summary>Switches focus to an alert on the page.</summary>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Focused alert</returns>
        public Alert switchToAlert(int timeoutms = 0) {
            Object alert;
            if (timeoutms > 0) {
                alert = this.WaitUntilObject(WebDriver.SwitchTo().Alert, timeoutms);
            } else {
                try {
                    alert = WebDriver.SwitchTo().Alert();
                } catch (Exception) {
                    throw new Exception("Alert not found!");
                }
            }
            return new Alert(this, (OpenQA.Selenium.IAlert)alert);
        }

        /// <summary>Selects either the first frame on the page or the main document when a page contains iFrames.</summary>
        /// <returns>An WebDriver instance focused on the default frame.</returns>
        public WebDriver switchToDefaultContent() {
            WebDriver.SwitchTo().DefaultContent();
            return this;
        }

        #endregion WebDriver Code

        /// <summary>Get the page loading metrics in millisecond</summary>
        /// <returns>Url, Page loading(ms), Server waiting(ms), Server receiving(ms), DOM loading(ms)</returns>
        public object[,] getPerformanceTiming(){
            var res = (ICollection)(((OpenQA.Selenium.IJavaScriptExecutor)WebDriver).ExecuteScript("var t=window.performance.timing; return [t.loadEventEnd-t.navigationStart,t.redirectEnd-t.redirectStart,t.domainLookupEnd-t.domainLookupStart,t.connectEnd-t.connectStart,t.responseStart-t.requestStart,t.responseEnd-t.responseStart,t.loadEventStart-t.responseEnd,t.loadEventEnd-t.loadEventStart];"));
            var size = res.Count + 1;
            var table = (object[,])Array.CreateInstance(typeof(object), new[] { 1, size }, new[] { 1, 1 }); ;
            int i = 1;
            table[1, 1] = WebDriver.Url;
            foreach (object item in res)
                table[1, ++i] = (Int32)(long)item;
            return table;
        }

        public string[] getBrokenLinks() {
            return getBrokenElement("a", "href");
        }

        public string[] getBrokenImages() {
            return getBrokenElement("img", "src");
        }

        private string[] getBrokenElement(string tagName, string attribute) {
            var list = new List<string>();
            var elements = WebDriver.FindElements(OpenQA.Selenium.By.TagName(tagName));
            foreach (var element in elements) {
                var url = element.GetAttribute(attribute);
                try {
                    var request = WebRequest.Create(url) as HttpWebRequest;
                    request.Method = "HEAD";
                    if(((HttpWebResponse)request.GetResponse()).StatusCode != HttpStatusCode.OK)
                        list.Add(url);
                } catch {
                    list.Add(url);
                }
            }
            return list.ToArray();
        }
        #region Regex

        /// <summary>Indicates whether the regular expression finds a match in the specified source code using the regular expression specified in the pattern parameter.</summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>true if the regular expression finds a match; otherwise, false.</returns>
        public bool isMatchPageSource(string pattern) {
            return Regex.IsMatch(this.PageSource, pattern);
        }

        /// <summary>Searches the specified source code for an occurrence of the regular expression supplied in the pattern parameter.</summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>Matching strings</returns>
        public object matchPageSource(string pattern) {
            Match match = Regex.Match(this.PageSource, pattern);
            if (match.Groups == null)
                return match.Value;
            string[] lst = new string[match.Groups.Count];
            for (int i = 0; i < match.Groups.Count; i++)
                lst[i] = match.Groups[i].Value;
            return lst;
        }

        #endregion Regex

        #region Utility

        /// <summary>Set the text in the Clipboard</summary>
        /// <param name="text">Text</param>
        public void setClipBoard(string text) {
            Clipboard.SetText(text);
        }

        /// <summary>Get the text in the Clipboard</summary>
        public string getClipBoard() {
            return Clipboard.GetText();
        }

        #endregion Utility

    }

}
