using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Remote;
using System.ComponentModel;
using OpenQA.Selenium;
using Action = System.Action;
using iTextSharp.text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;

namespace SeleniumWrapper
{
    /// <summary>Wrap the WebDriverBackedSelenium class </summary>
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

    [Description("")]
    [Guid("432b62a5-6f09-45ce-b10e-e3ccffab4234")]
    [ComVisible(true), ComDefaultInterface(typeof(IWebDriver)), ClassInterface(ClassInterfaceType.None)]
    public partial class WebDriver : IDisposable, IWebDriver
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern short GetKeyState(int virtualKeyCode);

        OpenQA.Selenium.IWebDriver webDriver;
        Selenium.WebDriverBackedSelenium webDriverBacked;
        Object result;
        Action action;
        String error;
        int Timeout;
        System.Timers.Timer timerhotkey;
        String baseUrl;
        bool canceled = false;
        ManualResetEvent waiter;
        Dictionary<string, object> preferences=null;

        public WebDriver(){
            waiter = new ManualResetEvent(false);
            this.Timeout = 30000;
            this.timerhotkey = new System.Timers.Timer(200);
            this.timerhotkey.Elapsed += new System.Timers.ElapsedEventHandler(TimerCheckHotKey);
            AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
        }

        [STAThread]
        private void AppDomain_UnhandledException(Object sender, UnhandledExceptionEventArgs e){
            this.error = ((Exception)e.ExceptionObject).Message;
            this.timerhotkey.Stop();
            waiter.Set();
            Thread.CurrentThread.Join();
        }

        ~WebDriver(){
            Dispose();
        }

        public void Dispose(){

        }

        private void TimerCheckHotKey(object source, ElapsedEventArgs e){
            if ((GetKeyState(0x1b) & 0x8000) != 0) {
                this.timerhotkey.Stop();
                waiter.Set();
                this.canceled = true;
            }
        }

        private string GetErrorPrifix(Action action){
            string lMethodname = Regex.Match(action.Method.Name, "<([^>]+)>").Groups[0].Value;
            return "Method " + lMethodname + " failed !";
        }

        [STAThread]
        private void InvokeWaitFor(Action action, Object expected, bool match){
            this.action = action;
            this.error = null;
            this.canceled = false;
            waiter.Reset();
            action.BeginInvoke((IAsyncResult iar) => {
                try{
                    action.EndInvoke(iar);
                    while(match ^ ObjectEquals(result,expected)){
                        Thread.Sleep(10);
                        action();
                    }
                }catch(Exception ex){
                    this.error = GetErrorPrifix(this.action) + " \r\n expected=<" + expected + "> \r\n" + (this.error != null ? this.error : ex.Message); 
                }
                waiter.Set();
            }, null);
            this.timerhotkey.Start();
            bool succed = waiter.WaitOne(this.Timeout);
            this.timerhotkey.Stop();
            if (this.canceled) throw new ApplicationException("Execution cancelled !");
            if (!succed) throw new ApplicationException(GetErrorPrifix(this.action) + " \r\nTimeout reached.");
            if (this.error != null) throw new ApplicationException(this.error);
        }

        private void InvokeAssert(Action action, Object expected, bool match){
            Object result = Invoke(action);
            if (match ^ ObjectEquals(result,expected)) throw new ApplicationException(GetErrorPrifix(this.action) + " expected=<" + expected.ToString() + "> result=<" + result.ToString() + "> ");
        }

        private String InvokeVerify(Action action, Object expected, bool match){
            Object result = Invoke(action);
            if (match ^ ObjectEquals(result, expected)) {
                return "KO, " + GetErrorPrifix(this.action) + " expected=<" + expected.ToString() + "> result=<" + result.ToString() + "> ";
            }else{
                return "OK";
            }
        }

        private void InvokeAndWait(Action action){
            Invoke(action);
            waitForPageToLoad( this.Timeout.ToString());
        }

        [STAThread]
        private Object Invoke(Action action){
            this.action = action;
            this.error = null;
            this.canceled = false;
            waiter.Reset();
            action.BeginInvoke((IAsyncResult iar) => {
                try{
                    action.EndInvoke(iar);
                }catch(Exception ex){
                    this.error = GetErrorPrifix(this.action) + "\r\n" + ex.Message;
                }
                waiter.Set();
            }, null);
            this.timerhotkey.Start();
            bool succed = waiter.WaitOne(this.Timeout);
            this.timerhotkey.Stop();
            if (this.canceled) throw new ApplicationException("Execution cancelled !");
            if (!succed) throw new ApplicationException(GetErrorPrifix(this.action) + " Timeout reached.");
            if (this.error != null) throw new ApplicationException(this.error);
            return this.result;
        }

        /// <summary>Starts a new Selenium testing session</summary>
        /// <param name="browser">Name of the browser : firefox, ie, chrome</param>
        /// <param name="url">The base URL</param>
        /// <param name="directory">Optional - Directory path for drivers or binaries</param>
        public void start(string browser, String url, [Optional][DefaultParameterValue("")]String directory){
            if(!String.IsNullOrEmpty(directory)){
                if(!System.IO.Directory.Exists(directory)) throw new ApplicationException("Direcory not found : " + directory);
            }else{
                directory =  System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            switch (browser.ToLower().Replace("*", "")) {
                case "ff":
                case "firefox": 
                    if(this.preferences!=null){
                        FirefoxProfile firefoxProfile = new FirefoxProfile();
                        foreach (var pair in this.preferences){
                            if(pair.Value is string){
                                firefoxProfile.SetPreference(pair.Key, (string)pair.Value);
                            }else if(pair.Value is short){
                                firefoxProfile.SetPreference(pair.Key, Convert.ToInt32(pair.Value));
                            }else if(pair.Value is bool){
                                firefoxProfile.SetPreference(pair.Key, (bool)pair.Value);
                            }
                        }
                        Invoke(() => this.webDriver = new OpenQA.Selenium.Firefox.FirefoxDriver(firefoxProfile));
                    }else{
                        Invoke(() => this.webDriver = new OpenQA.Selenium.Firefox.FirefoxDriver());
                    }
                    break;
                case "internetexplorer":
                case "iexplore":
                case "ie":
                    Invoke(() => this.webDriver = new OpenQA.Selenium.IE.InternetExplorerDriver(directory));
                    break;
                case "internetexplorer64":
                case "iexplore64":
                case "ie64":
                    Invoke(() => this.webDriver = new OpenQA.Selenium.IE.InternetExplorerDriver(directory + @"\ie64"));
                    break;
                case "cr":
                case "chrome":
                    if(this.preferences!=null){
                        ChromeOptions chromeOptions= new ChromeOptions();
                        foreach (var pair in this.preferences){
                            if(pair.Key == "chrome.switches"){
                                chromeOptions.AddArgument((string)pair.Value);
                            }
                        }
                        Invoke(() => this.webDriver = new OpenQA.Selenium.Chrome.ChromeDriver(directory, chromeOptions));
                    }else{
                        Invoke(() => this.webDriver = new OpenQA.Selenium.Chrome.ChromeDriver(directory));
                    }
                    break;
                case "sa":
                case "safari":
                    Invoke(() => this.webDriver = new OpenQA.Selenium.Safari.SafariDriver());
                    break;
                default:
                    throw new ApplicationException("Browser <" + browser + "> is not available !  \r\nChoose between Firefox, IE and Chrome");
            }
            Invoke(() => this.webDriverBacked = new Selenium.WebDriverBackedSelenium(this.webDriver, url));
            Invoke(() => webDriverBacked.Start());
            this.baseUrl = url.TrimEnd('/');
        }

        public void open(String url){
            if(!url.Contains("://")){
                 url = this.baseUrl + '/' + url.TrimStart('/');
            }
            Invoke(() => webDriverBacked.Open(url));
        }

        /// <summary>Starts remotely a new Selenium testing session</summary>
        /// <param name="browser">Name of the browser : firefox, ie, chrome, htmlunit, htmlunitwithjavascript, android, ipad, opera</param>
        /// <param name="remoteAddress">Remote url address (ex : "http://localhost:4444/wd/hub")</param>
        /// <param name="url">Base URL</param>
        /// <param name="javascriptEnabled">Optional argument to enable or disable javascript. Default is true</param>
        public void startRemotely(string browser, String remoteAddress, String url, [Optional][DefaultParameterValue(true)]Boolean javascriptEnabled){
            DesiredCapabilities lCapability;
            switch (browser.ToLower().Replace("*", "")) {
                case "ff":
                case "firefox": lCapability = DesiredCapabilities.Firefox(); break;
                case "cr":
                case "chrome": lCapability = DesiredCapabilities.Chrome(); break;
                case "internetexplorer":
                case "iexplore":
                case "ie": lCapability = DesiredCapabilities.InternetExplorer(); break;
                case "htmlunit": lCapability = DesiredCapabilities.HtmlUnit(); break;
                case "htmlunitwithjavascript": lCapability = DesiredCapabilities.HtmlUnitWithJavaScript(); break;
                case "android": lCapability = DesiredCapabilities.Android(); break;
                case "ipad": lCapability = DesiredCapabilities.IPad(); break;
                case "opera": lCapability = DesiredCapabilities.Opera(); break;
                default: throw new ApplicationException("Remote browser <" + browser + "> is not available !  \r\nChoose between Firefox, IE, Chrome, HtmlUnit, HtmlUnitWithJavaScript, \r\nAndroid, IPad, Opera");
            }
            if(this.preferences!=null){
                foreach (var pair in this.preferences){
                    if(lCapability.HasCapability(pair.Key)){
                        lCapability.SetCapability(pair.Key, pair.Value);
                    }else{
                        throw new ApplicationException("Preference <" + pair.Key + "> doesn't exit !  ");
                    }
                }
            }
            lCapability.IsJavaScriptEnabled = javascriptEnabled;
            this.webDriver = new RemoteWebDriverCust(new Uri(remoteAddress), lCapability);
            Invoke(() => this.webDriverBacked = new Selenium.WebDriverBackedSelenium(this.webDriver, url));
            Invoke(() => webDriverBacked.Start());
        }

        /// <summary>Set a specific preference for FireFox</summary>
        /// <param name="parameter">parameter</param>
        /// <param name="value">value</param>
        public void setPreference (string parameter, object value) {
            if (this.preferences == null) this.preferences = new Dictionary<string, object>();
            this.preferences.Add(parameter, value);
        }

        /// <summary>Wait the specified time in millisecond before executing the next command</summary>
        /// <param name="time_ms">Time to wait in millisecond</param>
        public void wait (int time_ms) {
            Thread.Sleep(time_ms);
        }

        /// <summary>Wait the specified time in millisecond before executing the next command</summary>
        /// <param name="time_ms">Time to wait in millisecond</param>
        public void pause (int time_ms) {
            Thread.Sleep(time_ms);
        }

        /// <summary>Test that two objects are equal and return the result</summary>
        /// <param name="expected">expected object. Can be a string, number, array...</param>
        /// <param name="current">current object. Can be a string, number, array...</param>
        /// <returns>Returns the result of the verification : "Ok" if the verification is true or "KO, message" if false</returns>
        public String verifyEqual(Object expected, Object current) {
            if ( ! ObjectEquals(expected, current)) {
                return "KO, assertEqual failed ! expected=<" + expected.ToString() + "> result=<" + current.ToString() + "> ";
            }else{
                return "OK";
            }
        }

        /// <summary>Test that two objects are not equal and return the result</summary>
        /// <param name="expected">expected object. Can be a string, number, array...</param>
        /// <param name="current">current object. Can be a string, number, array...</param>
        /// <returns>Returns the result of the verification : "Ok" if the verification is true or "KO, message" if false</returns>
        public String verifyNotEqual(Object expected, Object current) {
            if ( ObjectEquals(expected, current)) {
                return "KO, verifyNotEqual failed ! expected=<" + expected.ToString() + "> result=<" + current.ToString() + "> ";
            }else{
                return "OK";
            }
        }

        /// <summary>Test that two objects are equal and raise an exception if the result is false</summary>
        /// <param name="expected">expected object. Can be a string, number, array...</param>
        /// <param name="current">current object. Can be a string, number, array...</param>
        public void assertEqual(Object expected, Object current) {
            if ( ! ObjectEquals(expected, current)) {
                throw new ApplicationException( "KO, assertEqual failed ! expected=<" + expected.ToString() + "> result=<" + current.ToString() + "> " ); 
            }
        }

        /// <summary>Test that two objects are not equal and raise an exception if the result is false</summary>
        /// <param name="expected">expected object. Can be a string, number, array...</param>
        /// <param name="current">current object. Can be a string, number, array...</param>
        public void assertNotEqual(Object expected, Object current) {
            if ( ObjectEquals(expected, current)) {
                throw new ApplicationException( "KO, assertNotEqual failed ! expected=<" + expected.ToString() + "> result=<" + current.ToString() + "> ");
            }
        }

        private static bool ObjectEquals(Object A, Object B) {
            if(A.GetType().IsArray){
                if (!B.GetType().IsArray) return false;
                String[] a1 = (String[])A;
                String[] a2 = (String[])B;
                if (a1.Length == a2.Length) {
                    for (int i = 0; i < a1.Length; i++) {
                        if ( ! Equals( a1[i], a2[i] )){
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            }else{
                return Equals(A, B);
            }
        }

        /// <summary>Specifies the amount of time the driver should wait when searching for an element if it is not immediately present.</summary>
        /// <param name="timeout_ms">timeout in millisecond</param>
		public void setImplicitWait ( int timeout_ms) {
            this.webDriverBacked.UnderlyingWebDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(timeout_ms));
        }

        /// <summary> Specifies the amount of time that Selenium will wait for actions to complete. The default timeout is 30 seconds.</summary>
        /// <param name="timeout_ms">timeout in milliseconds, after which an error is raised</param>
        public void setTimeout(String timeout_ms) {
            this.Timeout = Int32.Parse(timeout_ms);
            Invoke(() => webDriverBacked.SetTimeout(timeout_ms)); 
        }

        /// <summary>Capture a screenshot to the Clipboard</summary>
        public void captureScreenshotToClipboard() {
            try{
                System.Drawing.Image image;
                System.Windows.Forms.Clipboard.Clear();
                byte[] res = (byte[])Invoke(() => this.result = ((OpenQA.Selenium.ITakesScreenshot)webDriver).GetScreenshot().AsByteArray);
                //string base64String = (string)Invoke(() => this.result = webDriver.CaptureScreenshotToString());
                if (res ==null || res.Length == 0) throw new ApplicationException("Method <captureScreenshotToClipboard> failed !\r\nReturned value is empty");
                //using (System.IO.MemoryStream ms = new System.IO.MemoryStream(Convert.FromBase64String(base64String))){
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(res)){
                    image = System.Drawing.Image.FromStream(ms);
                }
                System.Windows.Forms.Clipboard.SetImage(image);
            }catch(Exception ex){
                throw new ApplicationException(ex.Message);
            }
        }

        public object  captureScreenshotToImage(){
            object ret = Invoke(() => this.result = ((OpenQA.Selenium.ITakesScreenshot)webDriver).GetScreenshot().AsByteArray);
            if (ret == null) throw new ApplicationException("Method <captureScreenshotToPdf> failed !\r\nReturned value is empty");
            return ret;
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
		public Object executeScript(String script, [Optional][DefaultParameterValue(null)]Object arguments){
            Object result = ((OpenQA.Selenium.IJavaScriptExecutor)this.webDriver).ExecuteScript("try{" + script + "}catch(e){return 'error:'+e.message;}", arguments);
            if (result != null && result.ToString().StartsWith("error:") ) throw new ApplicationException("JavaScript " + result.ToString());
            return result;
        }

        // Following funtion are webdriver related
        #region WebDriver Code

        public String pageSource{
            get{ return this.webDriver.PageSource; }
        }

        public WebElement findElementByName(String name, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElement(OpenQA.Selenium.By.Name(name), timeoutms);
        }

        public WebElement findElementByXPath(String xpath, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElement(OpenQA.Selenium.By.XPath(xpath), timeoutms);
        }

        public WebElement findElementById(String id, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElement(OpenQA.Selenium.By.Id(id), timeoutms);
        }

        public WebElement findElementByClassName(String classname, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElement(OpenQA.Selenium.By.ClassName(classname), timeoutms);
        }

        public WebElement findElementByCssSelector(String cssselector, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElement(OpenQA.Selenium.By.CssSelector(cssselector), timeoutms);
        }

        public WebElement findElementByLinkText(String linktext, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElement(OpenQA.Selenium.By.LinkText(linktext), timeoutms);
        }

        public WebElement findElementByPartialLinkText(String partiallinktext, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElement(OpenQA.Selenium.By.PartialLinkText(partiallinktext), timeoutms);
        }

        public WebElement findElementByTagName(String tagname, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElement(OpenQA.Selenium.By.TagName(tagname), timeoutms);
        }
		
	    private WebElement findElement(By by, int timeoutms){
			if(timeoutms>0){
				var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(this.webDriver, TimeSpan.FromMilliseconds(timeoutms));
                return new WebElement(this.webDriver, wait.Until(drv => drv.FindElement(by)));
			}
            return new WebElement(this.webDriver, this.webDriver.FindElement(by));
        }

        public WebElement[] findElementsByName(String name, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElements(OpenQA.Selenium.By.Name(name), timeoutms);
        }

        public WebElement[] findElementsByXPath(String xpath, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElements(OpenQA.Selenium.By.XPath(xpath), timeoutms);
        }

        public WebElement[] findElementsById(String id, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElements(OpenQA.Selenium.By.Id(id), timeoutms);
        }

        public WebElement[] findElementsByClassName(String classname, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElements(OpenQA.Selenium.By.ClassName(classname), timeoutms);
        }

        public WebElement[] findElementsByCssSelector(String cssselector, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElements(OpenQA.Selenium.By.CssSelector(cssselector), timeoutms);
        }

        public WebElement[] findElementsByLinkText(String linktext, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElements(OpenQA.Selenium.By.LinkText(linktext), timeoutms);
        }

        public WebElement[] findElementsByPartialLinkText(String partiallinktext, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElements(OpenQA.Selenium.By.PartialLinkText(partiallinktext), timeoutms);
        }

        public WebElement[] findElementsByTagName(String tagname, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElements(OpenQA.Selenium.By.TagName(tagname), timeoutms);
        }

	    private WebElement[] findElements(By by, int timeoutms){
			if(timeoutms>0){
				var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(this.webDriver, TimeSpan.FromMilliseconds(timeoutms));
                return WebElement.GetWebElements(this.webDriver, wait.Until(drv => drv.FindElements(by)));
			}
            return WebElement.GetWebElements(this.webDriver, this.webDriver.FindElements(by));
        }

        public void get(String url){
            if(!url.Contains("://")){
                 url = this.baseUrl + '/' + url.TrimStart('/');
            }
            Invoke(() => this.webDriver.Navigate().GoToUrl(url));
        }

        public void sendKeys(string keysToSend){
            new OpenQA.Selenium.Interactions.Actions(this.webDriver).SendKeys(keysToSend).Perform();;
        }

        #endregion

        #region Regex

        /// <summary>Indicates whether the regular expression finds a match in the input string using the regular expression specified in the pattern parameter.</summary>
        /// <param name="input">The string to search for a match.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>true if the regular expression finds a match; otherwise, false.</returns>
        public bool isMatch(string input, string pattern){
            return Regex.IsMatch(input, pattern);
        }

        /// <summary>Searches the specified input string for an occurrence of the regular expression supplied in the pattern parameter.</summary>
        /// <param name="input">The string to search for a match.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>Matching strings</returns>
        public object match(string input, string pattern){
            Match match = Regex.Match(input, pattern);
            if(match.Groups != null){
                string[] lst = new string[match.Groups.Count];
                for(int i=0;i<match.Groups.Count;i++)
                    lst[i] = match.Groups[i].Value;
                return lst;
            }else{
                return match.Value;
            }
        }

        /// <summary>Within a specified input string, replaces all strings that match a specified regular expression with a specified replacement string.</summary>
        /// <param name="input">The string to search for a match.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="replacement">The replacement string.</param>
        /// <returns>A new string that is identical to the input string, except that a replacement string takes the place of each matched string.</returns>
        public string replace(string input, string pattern, string replacement ){
            return Regex.Replace(input, pattern, replacement);
        }

        #endregion


    }

}
