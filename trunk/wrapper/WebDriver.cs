using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Remote;
using System.ComponentModel;
using OpenQA.Selenium;

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
    [Guid("432b62a5-6f09-45ce-b10e-e3ccffab4234"), ClassInterface(ClassInterfaceType.None)]
    public partial class WebDriver : IWebDriver
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern short GetKeyState(int virtualKeyCode);

        OpenQA.Selenium.IWebDriver browserDriver;
        Selenium.WebDriverBackedSelenium webDriver;
        Object result;
        Action action;
        String error;
        int Timeout;
        System.Threading.Thread thread;
        System.Timers.Timer timerhotkey;
        String baseUrl;

        public WebDriver(){
            this.Timeout = 30000;
            this.timerhotkey = new System.Timers.Timer(200);
            this.timerhotkey.Elapsed += new System.Timers.ElapsedEventHandler(TimerCheckHotKey);
            AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
        }

        private void AppDomain_UnhandledException(Object sender, UnhandledExceptionEventArgs e){
            this.error = ((Exception)e.ExceptionObject).Message;
            if(this.thread.IsAlive){
                this.thread.Abort();
                this.timerhotkey.Stop();
                Thread.CurrentThread.Join();
            }
        }

        private void TimerCheckHotKey(object source, ElapsedEventArgs e){
            if ((GetKeyState(0x1b) & 0x8000) != 0) {
                this.timerhotkey.Stop();
                this.thread.Abort();
            }
        }

        private string GetErrorPrifix(Action action){
            string lMethodname = Regex.Match(action.Method.Name, "<([^>]+)>").Groups[0].Value;
            return "Method " + lMethodname + " failed !";
        }

        private void InvokeWaitFor(Action action, Object expected, bool match){
            this.action = action;
            this.error = null;
            this.thread = new System.Threading.Thread(new System.Threading.ThreadStart(() =>{
                try {
                    action();
                    while(match ^ ObjectEquals(result,expected)){
                        Thread.Sleep(10);
                        action();
                    }
                }catch (System.Exception e) {
                    this.error = GetErrorPrifix(this.action) + " \r\n expected=<" + expected + "> \r\n" + (this.error != null ? this.error : e.Message); 
                }
            }));
            this.thread.Start();
            this.timerhotkey.Start();
            bool succed = this.thread.Join(this.Timeout);
            this.timerhotkey.Stop();
            if (!succed) throw new ApplicationException(GetErrorPrifix(this.action) + " \r\n expected=<" + expected + "> \r\nTimeout reached.");
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

        private Object Invoke(Action action){
            this.action = action;
            this.error = null;
            this.thread = new System.Threading.Thread(new System.Threading.ThreadStart(() => {
                    try{
                        this.action();
                    }catch(System.Exception e){ 
                        this.error = GetErrorPrifix(this.action) + "\r\n" + ( this.error != null ? this.error : e.Message); 
                    }
                }));
            this.timerhotkey.Start();
            this.thread.Start();
            bool succed = this.thread.Join(this.Timeout);
            this.timerhotkey.Stop();
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
            switch (browser.ToLower()) {
                case "ff":
                case "firefox": 
                    Invoke(() => this.browserDriver = new OpenQA.Selenium.Firefox.FirefoxDriver());
                    break;
                case "internetexplorer":
                case "iexplore":
                case "ie":
                    Invoke(() => this.browserDriver = new OpenQA.Selenium.IE.InternetExplorerDriver(directory));
                    break;
                case "cr":
                case "chrome":
                    Invoke(() => this.browserDriver = new OpenQA.Selenium.Chrome.ChromeDriver(directory));
                    break;
                default:
                    throw new ApplicationException("Browser <" + browser + "> is not available !  \r\nChoose between Firefox, IE and Chrome");
            }
            Invoke(() => this.webDriver = new Selenium.WebDriverBackedSelenium(this.browserDriver, url));
            Invoke(() => webDriver.Start());
            this.baseUrl = url.TrimEnd('/');
        }

        public void open(String url){
            if(!url.Contains("://")){
                 url = this.baseUrl + '/' + url.TrimStart('/');
            }
            Invoke(() => webDriver.Open(url));
        }

        /// <summary>Starts remotely a new Selenium testing session</summary>
        /// <param name="browser">Name of the browser : firefox, ie, chrome, htmlunit, htmlunitwithjavascript, android, ipad, opera</param>
        /// <param name="remoteAddress">Remote url address (ex : "http://localhost:4444/")</param>
        /// <param name="url">Base URL</param>
        /// <param name="javascriptEnabled">Optional argument to enable or disable javascript. Default is true</param>
        /// <param name="capabilities">Optional capabilities. ex : "version=3.6,plateform=LINUX"</param>
        public void startRemotely(string browser, String remoteAddress, String url, [Optional][DefaultParameterValue(true)]Boolean javascriptEnabled, [Optional][DefaultParameterValue("")]String capabilities){
            DesiredCapabilities lCapability;
            switch (browser.ToLower()) {
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
            if(!String.IsNullOrEmpty(capabilities)){
                var strCapabilities = capabilities.Split(',');
                foreach( string strCapability in strCapabilities){
                    var res = strCapability.Split('=');
                    if(lCapability.HasCapability(res[0])){
                        lCapability.SetCapability(res[0],res[1]);
                    }else{
                        throw new ApplicationException("Capability <" + res[0] + "> doesn't exit !  ");
                    }
                }
            }
            lCapability.IsJavaScriptEnabled = javascriptEnabled;
            this.browserDriver = new RemoteWebDriverCust(new Uri(remoteAddress), lCapability);
            Invoke(() => this.webDriver = new Selenium.WebDriverBackedSelenium(this.browserDriver, url));
            Invoke(() => webDriver.Start());
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
            this.webDriver.UnderlyingWebDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(timeout_ms));
        }

        /// <summary> Specifies the amount of time that Selenium will wait for actions to complete. The default timeout is 30 seconds.</summary>
        /// <param name="timeout_ms">timeout in milliseconds, after which an error is raised</param>
        public void setTimeout(String timeout_ms) {
            this.Timeout = Int32.Parse(timeout_ms);
            Invoke(() => webDriver.SetTimeout(timeout_ms)); 
        }

        /// <summary>Capture a screenshot to the Clipboard</summary>
        public void captureScreenshotToClipboard() {
            try{
                System.Drawing.Image image;
                System.Windows.Forms.Clipboard.Clear();
                byte[] res = (byte[])Invoke(() => this.result = ((OpenQA.Selenium.ITakesScreenshot)browserDriver).GetScreenshot().AsByteArray);
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
            Object result = ((OpenQA.Selenium.IJavaScriptExecutor)this.browserDriver).ExecuteScript("try{" + script + "}catch(e){return 'error:'+e.message;}", arguments);
            if (result != null && result.ToString().StartsWith("error:") ) throw new ApplicationException("JavaScript " + result.ToString());
            return result;
        }

    }

}
