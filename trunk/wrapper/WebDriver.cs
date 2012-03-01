using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Text.RegularExpressions;

namespace SeleniumWrapper
{
    public enum Browser { Firefox , Ie, Chrome };

    [Guid("432b62a5-6f09-45ce-b10e-e3ccffab4234")]
    [ClassInterface(ClassInterfaceType.None)]
    public class WebDriver : IWebDriver
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

        public WebDriver(){
            this.Timeout = 30000;
            timerhotkey = new System.Timers.Timer(100);
            timerhotkey.Elapsed += new System.Timers.ElapsedEventHandler(TimerCheckHotKey);
        }

        private void TimerCheckHotKey(object source, ElapsedEventArgs e){
            if ((GetKeyState(0x1b) & 0x8000) != 0) {
                this.timerhotkey.Stop();
                this.thread.Abort();
            }
        }

        private string GetErrorPrifix(Action action){
            string lMethodname = Regex.Match(action.Method.Name, "<([^>]+)>").Groups[0].Value;
            return "Method " + lMethodname + " invocation failed !";
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
                }catch (System.Exception e) { this.error = GetErrorPrifix(this.action) + " expected=<"+ expected +"> \r\n" + e.Message; }
            }));
            this.thread.Start();
            this.timerhotkey.Start();
            if (!this.thread.Join(this.Timeout)) throw new ApplicationException(GetErrorPrifix(this.action) + " expected=<"+ expected +"> \r\nTimeout reached.");
            this.timerhotkey.Stop();
            if (this.error != null) throw new System.Exception(this.error);
        }

        private void InvokeAssert(Action action, Object expected, bool match){
            Object result = Invoke(action);
            if (match ^ ObjectEquals(result,expected)) throw new ApplicationException(GetErrorPrifix(this.action) + " expected=<" + expected.ToString() + "> result=<" + result.ToString() + "> ");
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
                    }catch(System.Exception e){ this.error = GetErrorPrifix(this.action) + "\r\n" + e.Message; }
                }));
            this.timerhotkey.Start();
            this.thread.Start();
            this.timerhotkey.Stop();
            if (!this.thread.Join(this.Timeout)) throw new ApplicationException(GetErrorPrifix(this.action) + " Timeout reached.");
            if (this.error != null) throw new ApplicationException(this.error);
            return this.result;
        }

        public void start(Browser browser, string url){
            switch (browser) {
                case Browser.Firefox:
                    Invoke(() => this.browserDriver = new OpenQA.Selenium.Firefox.FirefoxDriver()); break;
                case Browser.Chrome:
                    Invoke(() => this.browserDriver = new OpenQA.Selenium.Chrome.ChromeDriver()); break;
                case Browser.Ie:
                    Invoke(() => this.browserDriver = new OpenQA.Selenium.IE.InternetExplorerDriver()); break;
                default:
                    Invoke(() => this.browserDriver = new OpenQA.Selenium.IE.InternetExplorerDriver()); break;
            }
            Invoke(() => this.webDriver = new Selenium.WebDriverBackedSelenium(this.browserDriver, url));
            Invoke(() => webDriver.Start());
        }

        private static bool ObjectEquals(Object A, Object B) {
            if(A.GetType().IsArray){
                if (!B.GetType().IsArray) return false;
                String[] a1 = (String[])A;
                String[] a2 = (String[])B;
                if (a1.Length == a2.Length) {
                    for (int i = 0; i < a1.Length; i++) {
                        if (a1[i] != a2[i])  {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            }else{
                return A.Equals(B);
            }
        }

		public void setImplicitWait ( int timeoutMs) {
            this.webDriver.UnderlyingWebDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(timeoutMs));
        }

        public void setTimeout(String timeout) {
            this.Timeout = Int32.Parse(timeout);
            Invoke(() => webDriver.SetTimeout(timeout)); 
        }

        // Following funtion are automatically generated by reflexion

        #region Auto-Generated Code
        public void waitForPageToLoad(String timeout) { Invoke(() => webDriver.WaitForPageToLoad(timeout)); }
        public void waitForFrameToLoad(String frameAddress, String timeout) { Invoke(() => webDriver.WaitForFrameToLoad(frameAddress, timeout)); }
        public String getCookie() { return (String)Invoke(() => this.result = webDriver.GetCookie()); }
        public void assertCookie(String expected) { InvokeAssert(() => this.result = webDriver.GetCookie(), expected, true); }
        public void assertNotCookie(String expected) { InvokeAssert(() => this.result = webDriver.GetCookie(), expected, false); }
        public void waitForCookie(String expected) { InvokeWaitFor(() => this.result = webDriver.GetCookie(), expected, true); }
        public void waitForNotCookie(String expected) { InvokeWaitFor(() => this.result = webDriver.GetCookie(), expected, false); }
        public String getCookieByName(String name) { return (String)Invoke(() => this.result = webDriver.GetCookieByName(name)); }
        public void assertCookieByName(String name, String expected) { InvokeAssert(() => this.result = webDriver.GetCookieByName(name), expected, true); }
        public void assertNotCookieByName(String name, String expected) { InvokeAssert(() => this.result = webDriver.GetCookieByName(name), expected, false); }
        public void waitForCookieByName(String name, String expected) { InvokeWaitFor(() => this.result = webDriver.GetCookieByName(name), expected, true); }
        public void waitForNotCookieByName(String name, String expected) { InvokeWaitFor(() => this.result = webDriver.GetCookieByName(name), expected, false); }
        public Boolean isCookiePresent(String name) { return (Boolean)Invoke(() => this.result = webDriver.IsCookiePresent(name)); }
        public void assertCookiePresent(String name) { InvokeAssert(() => this.result = webDriver.IsCookiePresent(name), true, true); }
        public void waitForCookiePresent(String name) { InvokeWaitFor(() => this.result = webDriver.IsCookiePresent(name), true, true); }
        public void assertCookieNotPresent(String name) { InvokeAssert(() => this.result = webDriver.IsCookiePresent(name), false, true); }
        public void waitForCookieNotPresent(String name) { InvokeWaitFor(() => this.result = webDriver.IsCookiePresent(name), false, true); }
        public void createCookie(String nameValuePair, String optionsString) { Invoke(() => webDriver.CreateCookie(nameValuePair, optionsString)); }
        public void createCookieAndWait(String nameValuePair, String optionsString) { InvokeAndWait(() => webDriver.CreateCookie(nameValuePair, optionsString)); }
        public void deleteCookie(String name, String optionsString) { Invoke(() => webDriver.DeleteCookie(name, optionsString)); }
        public void deleteAllVisibleCookies() { Invoke(() => webDriver.DeleteAllVisibleCookies()); }
        public void setBrowserLogLevel(String logLevel) { Invoke(() => webDriver.SetBrowserLogLevel(logLevel)); }
        public void runScript(String script) { Invoke(() => webDriver.RunScript(script)); }
        public void runScriptAndWait(String script) { InvokeAndWait(() => webDriver.RunScript(script)); }
        public void addLocationStrategy(String strategyName, String functionDefinition) { Invoke(() => webDriver.AddLocationStrategy(strategyName, functionDefinition)); }
        public void addLocationStrategyAndWait(String strategyName, String functionDefinition) { InvokeAndWait(() => webDriver.AddLocationStrategy(strategyName, functionDefinition)); }
        public void captureEntirePageScreenshot(String filename, String kwargs) { Invoke(() => webDriver.CaptureEntirePageScreenshot(filename, kwargs)); }
        public void rollup(String rollupName, String kwargs) { Invoke(() => webDriver.Rollup(rollupName, kwargs)); }
        public void rollupAndWait(String rollupName, String kwargs) { InvokeAndWait(() => webDriver.Rollup(rollupName, kwargs)); }
        public void addScript(String scriptContent, String scriptTagId) { Invoke(() => webDriver.AddScript(scriptContent, scriptTagId)); }
        public void addScriptAndWait(String scriptContent, String scriptTagId) { InvokeAndWait(() => webDriver.AddScript(scriptContent, scriptTagId)); }
        public void removeScript(String scriptTagId) { Invoke(() => webDriver.RemoveScript(scriptTagId)); }
        public void removeScriptAndWait(String scriptTagId) { InvokeAndWait(() => webDriver.RemoveScript(scriptTagId)); }
        public void useXpathLibrary(String libraryName) { Invoke(() => webDriver.UseXpathLibrary(libraryName)); }
        public void useXpathLibraryAndWait(String libraryName) { InvokeAndWait(() => webDriver.UseXpathLibrary(libraryName)); }
        public void setContext(String context) { Invoke(() => webDriver.SetContext(context)); }
        public void attachFile(String fieldLocator, String fileLocator) { Invoke(() => webDriver.AttachFile(fieldLocator, fileLocator)); }
        public void attachFileAndWait(String fieldLocator, String fileLocator) { InvokeAndWait(() => webDriver.AttachFile(fieldLocator, fileLocator)); }
        public void captureScreenshot(String filename) { Invoke(() => webDriver.CaptureScreenshot(filename)); }
        public String captureScreenshotToString() { return (String)Invoke(() => this.result = webDriver.CaptureScreenshotToString()); }
        public String captureNetworkTraffic(String type) { return (String)Invoke(() => this.result = webDriver.CaptureNetworkTraffic(type)); }
        public String captureEntirePageScreenshotToString(String kwargs) { return (String)Invoke(() => this.result = webDriver.CaptureEntirePageScreenshotToString(kwargs)); }
        public void shutDownSeleniumServer() { Invoke(() => webDriver.ShutDownSeleniumServer()); }
        public void shutDownSeleniumServerAndWait() { InvokeAndWait(() => webDriver.ShutDownSeleniumServer()); }
        public String retrieveLastRemoteControlLogs() { return (String)Invoke(() => this.result = webDriver.RetrieveLastRemoteControlLogs()); }
        public void keyDownNative(String keycode) { Invoke(() => webDriver.KeyDownNative(keycode)); }
        public void keyDownNativeAndWait(String keycode) { InvokeAndWait(() => webDriver.KeyDownNative(keycode)); }
        public void keyUpNative(String keycode) { Invoke(() => webDriver.KeyUpNative(keycode)); }
        public void keyUpNativeAndWait(String keycode) { InvokeAndWait(() => webDriver.KeyUpNative(keycode)); }
        public void keyPressNative(String keycode) { Invoke(() => webDriver.KeyPressNative(keycode)); }
        public void keyPressNativeAndWait(String keycode) { InvokeAndWait(() => webDriver.KeyPressNative(keycode)); }
        public void setExtensionJs(String extensionJs) { Invoke(() => webDriver.SetExtensionJs(extensionJs)); }
        public void stop() { Invoke(() => webDriver.Stop()); }
        public void click(String locator) { Invoke(() => webDriver.Click(locator)); }
        public void clickAndWait(String locator) { InvokeAndWait(() => webDriver.Click(locator)); }
        public void doubleClick(String locator) { Invoke(() => webDriver.DoubleClick(locator)); }
        public void doubleClickAndWait(String locator) { InvokeAndWait(() => webDriver.DoubleClick(locator)); }
        public void contextMenu(String locator) { Invoke(() => webDriver.ContextMenu(locator)); }
        public void contextMenuAndWait(String locator) { InvokeAndWait(() => webDriver.ContextMenu(locator)); }
        public void clickAt(String locator, String coordString) { Invoke(() => webDriver.ClickAt(locator, coordString)); }
        public void clickAtAndWait(String locator, String coordString) { InvokeAndWait(() => webDriver.ClickAt(locator, coordString)); }
        public void doubleClickAt(String locator, String coordString) { Invoke(() => webDriver.DoubleClickAt(locator, coordString)); }
        public void doubleClickAtAndWait(String locator, String coordString) { InvokeAndWait(() => webDriver.DoubleClickAt(locator, coordString)); }
        public void contextMenuAt(String locator, String coordString) { Invoke(() => webDriver.ContextMenuAt(locator, coordString)); }
        public void contextMenuAtAndWait(String locator, String coordString) { InvokeAndWait(() => webDriver.ContextMenuAt(locator, coordString)); }
        public void fireEvent(String locator, String eventName) { Invoke(() => webDriver.FireEvent(locator, eventName)); }
        public void fireEventAndWait(String locator, String eventName) { InvokeAndWait(() => webDriver.FireEvent(locator, eventName)); }
        public void focus(String locator) { Invoke(() => webDriver.Focus(locator)); }
        public void focusAndWait(String locator) { InvokeAndWait(() => webDriver.Focus(locator)); }
        public void keyPress(String locator, String keySequence) { Invoke(() => webDriver.KeyPress(locator, keySequence)); }
        public void keyPressAndWait(String locator, String keySequence) { InvokeAndWait(() => webDriver.KeyPress(locator, keySequence)); }
        public void shiftKeyDown() { Invoke(() => webDriver.ShiftKeyDown()); }
        public void shiftKeyDownAndWait() { InvokeAndWait(() => webDriver.ShiftKeyDown()); }
        public void shiftKeyUp() { Invoke(() => webDriver.ShiftKeyUp()); }
        public void shiftKeyUpAndWait() { InvokeAndWait(() => webDriver.ShiftKeyUp()); }
        public void metaKeyDown() { Invoke(() => webDriver.MetaKeyDown()); }
        public void metaKeyDownAndWait() { InvokeAndWait(() => webDriver.MetaKeyDown()); }
        public void metaKeyUp() { Invoke(() => webDriver.MetaKeyUp()); }
        public void metaKeyUpAndWait() { InvokeAndWait(() => webDriver.MetaKeyUp()); }
        public void altKeyDown() { Invoke(() => webDriver.AltKeyDown()); }
        public void altKeyDownAndWait() { InvokeAndWait(() => webDriver.AltKeyDown()); }
        public void altKeyUp() { Invoke(() => webDriver.AltKeyUp()); }
        public void altKeyUpAndWait() { InvokeAndWait(() => webDriver.AltKeyUp()); }
        public void controlKeyDown() { Invoke(() => webDriver.ControlKeyDown()); }
        public void controlKeyDownAndWait() { InvokeAndWait(() => webDriver.ControlKeyDown()); }
        public void controlKeyUp() { Invoke(() => webDriver.ControlKeyUp()); }
        public void controlKeyUpAndWait() { InvokeAndWait(() => webDriver.ControlKeyUp()); }
        public void keyDown(String locator, String keySequence) { Invoke(() => webDriver.KeyDown(locator, keySequence)); }
        public void keyDownAndWait(String locator, String keySequence) { InvokeAndWait(() => webDriver.KeyDown(locator, keySequence)); }
        public void keyUp(String locator, String keySequence) { Invoke(() => webDriver.KeyUp(locator, keySequence)); }
        public void keyUpAndWait(String locator, String keySequence) { InvokeAndWait(() => webDriver.KeyUp(locator, keySequence)); }
        public void mouseOver(String locator) { Invoke(() => webDriver.MouseOver(locator)); }
        public void mouseOverAndWait(String locator) { InvokeAndWait(() => webDriver.MouseOver(locator)); }
        public void mouseOut(String locator) { Invoke(() => webDriver.MouseOut(locator)); }
        public void mouseOutAndWait(String locator) { InvokeAndWait(() => webDriver.MouseOut(locator)); }
        public void mouseDown(String locator) { Invoke(() => webDriver.MouseDown(locator)); }
        public void mouseDownAndWait(String locator) { InvokeAndWait(() => webDriver.MouseDown(locator)); }
        public void mouseDownRight(String locator) { Invoke(() => webDriver.MouseDownRight(locator)); }
        public void mouseDownRightAndWait(String locator) { InvokeAndWait(() => webDriver.MouseDownRight(locator)); }
        public void mouseDownAt(String locator, String coordString) { Invoke(() => webDriver.MouseDownAt(locator, coordString)); }
        public void mouseDownAtAndWait(String locator, String coordString) { InvokeAndWait(() => webDriver.MouseDownAt(locator, coordString)); }
        public void mouseDownRightAt(String locator, String coordString) { Invoke(() => webDriver.MouseDownRightAt(locator, coordString)); }
        public void mouseDownRightAtAndWait(String locator, String coordString) { InvokeAndWait(() => webDriver.MouseDownRightAt(locator, coordString)); }
        public void mouseUp(String locator) { Invoke(() => webDriver.MouseUp(locator)); }
        public void mouseUpAndWait(String locator) { InvokeAndWait(() => webDriver.MouseUp(locator)); }
        public void mouseUpRight(String locator) { Invoke(() => webDriver.MouseUpRight(locator)); }
        public void mouseUpRightAndWait(String locator) { InvokeAndWait(() => webDriver.MouseUpRight(locator)); }
        public void mouseUpAt(String locator, String coordString) { Invoke(() => webDriver.MouseUpAt(locator, coordString)); }
        public void mouseUpAtAndWait(String locator, String coordString) { InvokeAndWait(() => webDriver.MouseUpAt(locator, coordString)); }
        public void mouseUpRightAt(String locator, String coordString) { Invoke(() => webDriver.MouseUpRightAt(locator, coordString)); }
        public void mouseUpRightAtAndWait(String locator, String coordString) { InvokeAndWait(() => webDriver.MouseUpRightAt(locator, coordString)); }
        public void mouseMove(String locator) { Invoke(() => webDriver.MouseMove(locator)); }
        public void mouseMoveAndWait(String locator) { InvokeAndWait(() => webDriver.MouseMove(locator)); }
        public void mouseMoveAt(String locator, String coordString) { Invoke(() => webDriver.MouseMoveAt(locator, coordString)); }
        public void mouseMoveAtAndWait(String locator, String coordString) { InvokeAndWait(() => webDriver.MouseMoveAt(locator, coordString)); }
        public void type(String locator, String value) { Invoke(() => webDriver.Type(locator, value)); }
        public void typeAndWait(String locator, String value) { InvokeAndWait(() => webDriver.Type(locator, value)); }
        public void typeKeys(String locator, String value) { Invoke(() => webDriver.TypeKeys(locator, value)); }
        public void typeKeysAndWait(String locator, String value) { InvokeAndWait(() => webDriver.TypeKeys(locator, value)); }
        public void setSpeed(String value) { Invoke(() => webDriver.SetSpeed(value)); }
        public String getSpeed() { return (String)Invoke(() => this.result = webDriver.GetSpeed()); }
        public void assertSpeed(String expected) { InvokeAssert(() => this.result = webDriver.GetSpeed(), expected, true); }
        public void assertNotSpeed(String expected) { InvokeAssert(() => this.result = webDriver.GetSpeed(), expected, false); }
        public void waitForSpeed(String expected) { InvokeWaitFor(() => this.result = webDriver.GetSpeed(), expected, true); }
        public void waitForNotSpeed(String expected) { InvokeWaitFor(() => this.result = webDriver.GetSpeed(), expected, false); }
        public void check(String locator) { Invoke(() => webDriver.Check(locator)); }
        public void checkAndWait(String locator) { InvokeAndWait(() => webDriver.Check(locator)); }
        public void uncheck(String locator) { Invoke(() => webDriver.Uncheck(locator)); }
        public void uncheckAndWait(String locator) { InvokeAndWait(() => webDriver.Uncheck(locator)); }
        public void select(String selectLocator, String optionLocator) { Invoke(() => webDriver.Select(selectLocator, optionLocator)); }
        public void selectAndWait(String selectLocator, String optionLocator) { InvokeAndWait(() => webDriver.Select(selectLocator, optionLocator)); }
        public void addSelection(String locator, String optionLocator) { Invoke(() => webDriver.AddSelection(locator, optionLocator)); }
        public void addSelectionAndWait(String locator, String optionLocator) { InvokeAndWait(() => webDriver.AddSelection(locator, optionLocator)); }
        public void removeSelection(String locator, String optionLocator) { Invoke(() => webDriver.RemoveSelection(locator, optionLocator)); }
        public void removeSelectionAndWait(String locator, String optionLocator) { InvokeAndWait(() => webDriver.RemoveSelection(locator, optionLocator)); }
        public void removeAllSelections(String locator) { Invoke(() => webDriver.RemoveAllSelections(locator)); }
        public void removeAllSelectionsAndWait(String locator) { InvokeAndWait(() => webDriver.RemoveAllSelections(locator)); }
        public void submit(String formLocator) { Invoke(() => webDriver.Submit(formLocator)); }
        public void submitAndWait(String formLocator) { InvokeAndWait(() => webDriver.Submit(formLocator)); }
        public void open(String url) { Invoke(() => webDriver.Open(url)); }
        public void openWindow(String url, String windowID) { Invoke(() => webDriver.OpenWindow(url, windowID)); }
        public void selectWindow(String windowID) { Invoke(() => webDriver.SelectWindow(windowID)); }
        public void selectPopUp(String windowID) { Invoke(() => webDriver.SelectPopUp(windowID)); }
        public void selectPopUpAndWait(String windowID) { InvokeAndWait(() => webDriver.SelectPopUp(windowID)); }
        public void deselectPopUp() { Invoke(() => webDriver.DeselectPopUp()); }
        public void deselectPopUpAndWait() { InvokeAndWait(() => webDriver.DeselectPopUp()); }
        public void selectFrame(String locator) { Invoke(() => webDriver.SelectFrame(locator)); }
        public Boolean getWhetherThisFrameMatchFrameExpression(String currentFrameString, String target) { return (Boolean)Invoke(() => this.result = webDriver.GetWhetherThisFrameMatchFrameExpression(currentFrameString, target)); }
        public void assertWhetherThisFrameMatchFrameExpression(String currentFrameString, String target, Boolean expected) { InvokeAssert(() => this.result = webDriver.GetWhetherThisFrameMatchFrameExpression(currentFrameString, target), expected, true); }
        public void assertNotWhetherThisFrameMatchFrameExpression(String currentFrameString, String target, Boolean expected) { InvokeAssert(() => this.result = webDriver.GetWhetherThisFrameMatchFrameExpression(currentFrameString, target), expected, false); }
        public void waitForWhetherThisFrameMatchFrameExpression(String currentFrameString, String target, Boolean expected) { InvokeWaitFor(() => this.result = webDriver.GetWhetherThisFrameMatchFrameExpression(currentFrameString, target), expected, true); }
        public void waitForNotWhetherThisFrameMatchFrameExpression(String currentFrameString, String target, Boolean expected) { InvokeWaitFor(() => this.result = webDriver.GetWhetherThisFrameMatchFrameExpression(currentFrameString, target), expected, false); }
        public Boolean getWhetherThisWindowMatchWindowExpression(String currentWindowString, String target) { return (Boolean)Invoke(() => this.result = webDriver.GetWhetherThisWindowMatchWindowExpression(currentWindowString, target)); }
        public void assertWhetherThisWindowMatchWindowExpression(String currentWindowString, String target, Boolean expected) { InvokeAssert(() => this.result = webDriver.GetWhetherThisWindowMatchWindowExpression(currentWindowString, target), expected, true); }
        public void assertNotWhetherThisWindowMatchWindowExpression(String currentWindowString, String target, Boolean expected) { InvokeAssert(() => this.result = webDriver.GetWhetherThisWindowMatchWindowExpression(currentWindowString, target), expected, false); }
        public void waitForWhetherThisWindowMatchWindowExpression(String currentWindowString, String target, Boolean expected) { InvokeWaitFor(() => this.result = webDriver.GetWhetherThisWindowMatchWindowExpression(currentWindowString, target), expected, true); }
        public void waitForNotWhetherThisWindowMatchWindowExpression(String currentWindowString, String target, Boolean expected) { InvokeWaitFor(() => this.result = webDriver.GetWhetherThisWindowMatchWindowExpression(currentWindowString, target), expected, false); }
        public void waitForPopUp(String windowID, String timeout) { Invoke(() => webDriver.WaitForPopUp(windowID, timeout)); }
        public void chooseCancelOnNextConfirmation() { Invoke(() => webDriver.ChooseCancelOnNextConfirmation()); }
        public void chooseOkOnNextConfirmation() { Invoke(() => webDriver.ChooseOkOnNextConfirmation()); }
        public void chooseOkOnNextConfirmationAndWait() { InvokeAndWait(() => webDriver.ChooseOkOnNextConfirmation()); }
        public void answerOnNextPrompt(String answer) { Invoke(() => webDriver.AnswerOnNextPrompt(answer)); }
        public void goBack() { Invoke(() => webDriver.GoBack()); }
        public void goBackAndWait() { InvokeAndWait(() => webDriver.GoBack()); }
        public void refresh() { Invoke(() => webDriver.Refresh()); }
        public void refreshAndWait() { InvokeAndWait(() => webDriver.Refresh()); }
        public void close() { Invoke(() => webDriver.Close()); }
        public Boolean isAlertPresent() { return (Boolean)Invoke(() => this.result = webDriver.IsAlertPresent()); }
        public void assertAlertPresent() { InvokeAssert(() => this.result = webDriver.IsAlertPresent(), true, true); }
        public void waitForAlertPresent() { InvokeWaitFor(() => this.result = webDriver.IsAlertPresent(), true, true); }
        public void assertAlertNotPresent() { InvokeAssert(() => this.result = webDriver.IsAlertPresent(), false, true); }
        public void waitForAlertNotPresent() { InvokeWaitFor(() => this.result = webDriver.IsAlertPresent(), false, true); }
        public Boolean isPromptPresent() { return (Boolean)Invoke(() => this.result = webDriver.IsPromptPresent()); }
        public void assertPromptPresent() { InvokeAssert(() => this.result = webDriver.IsPromptPresent(), true, true); }
        public void waitForPromptPresent() { InvokeWaitFor(() => this.result = webDriver.IsPromptPresent(), true, true); }
        public void assertPromptNotPresent() { InvokeAssert(() => this.result = webDriver.IsPromptPresent(), false, true); }
        public void waitForPromptNotPresent() { InvokeWaitFor(() => this.result = webDriver.IsPromptPresent(), false, true); }
        public Boolean isConfirmationPresent() { return (Boolean)Invoke(() => this.result = webDriver.IsConfirmationPresent()); }
        public void assertConfirmationPresent() { InvokeAssert(() => this.result = webDriver.IsConfirmationPresent(), true, true); }
        public void waitForConfirmationPresent() { InvokeWaitFor(() => this.result = webDriver.IsConfirmationPresent(), true, true); }
        public void assertConfirmationNotPresent() { InvokeAssert(() => this.result = webDriver.IsConfirmationPresent(), false, true); }
        public void waitForConfirmationNotPresent() { InvokeWaitFor(() => this.result = webDriver.IsConfirmationPresent(), false, true); }
        public String getAlert() { return (String)Invoke(() => this.result = webDriver.GetAlert()); }
        public void assertAlert(String expected) { InvokeAssert(() => this.result = webDriver.GetAlert(), expected, true); }
        public void assertNotAlert(String expected) { InvokeAssert(() => this.result = webDriver.GetAlert(), expected, false); }
        public void waitForAlert(String expected) { InvokeWaitFor(() => this.result = webDriver.GetAlert(), expected, true); }
        public void waitForNotAlert(String expected) { InvokeWaitFor(() => this.result = webDriver.GetAlert(), expected, false); }
        public String getConfirmation() { return (String)Invoke(() => this.result = webDriver.GetConfirmation()); }
        public void assertConfirmation(String expected) { InvokeAssert(() => this.result = webDriver.GetConfirmation(), expected, true); }
        public void assertNotConfirmation(String expected) { InvokeAssert(() => this.result = webDriver.GetConfirmation(), expected, false); }
        public void waitForConfirmation(String expected) { InvokeWaitFor(() => this.result = webDriver.GetConfirmation(), expected, true); }
        public void waitForNotConfirmation(String expected) { InvokeWaitFor(() => this.result = webDriver.GetConfirmation(), expected, false); }
        public String getPrompt() { return (String)Invoke(() => this.result = webDriver.GetPrompt()); }
        public void assertPrompt(String expected) { InvokeAssert(() => this.result = webDriver.GetPrompt(), expected, true); }
        public void assertNotPrompt(String expected) { InvokeAssert(() => this.result = webDriver.GetPrompt(), expected, false); }
        public void waitForPrompt(String expected) { InvokeWaitFor(() => this.result = webDriver.GetPrompt(), expected, true); }
        public void waitForNotPrompt(String expected) { InvokeWaitFor(() => this.result = webDriver.GetPrompt(), expected, false); }
        public String getLocation() { return (String)Invoke(() => this.result = webDriver.GetLocation()); }
        public void assertLocation(String expected) { InvokeAssert(() => this.result = webDriver.GetLocation(), expected, true); }
        public void assertNotLocation(String expected) { InvokeAssert(() => this.result = webDriver.GetLocation(), expected, false); }
        public void waitForLocation(String expected) { InvokeWaitFor(() => this.result = webDriver.GetLocation(), expected, true); }
        public void waitForNotLocation(String expected) { InvokeWaitFor(() => this.result = webDriver.GetLocation(), expected, false); }
        public String getTitle() { return (String)Invoke(() => this.result = webDriver.GetTitle()); }
        public void assertTitle(String expected) { InvokeAssert(() => this.result = webDriver.GetTitle(), expected, true); }
        public void assertNotTitle(String expected) { InvokeAssert(() => this.result = webDriver.GetTitle(), expected, false); }
        public void waitForTitle(String expected) { InvokeWaitFor(() => this.result = webDriver.GetTitle(), expected, true); }
        public void waitForNotTitle(String expected) { InvokeWaitFor(() => this.result = webDriver.GetTitle(), expected, false); }
        public String getBodyText() { return (String)Invoke(() => this.result = webDriver.GetBodyText()); }
        public void assertBodyText(String expected) { InvokeAssert(() => this.result = webDriver.GetBodyText(), expected, true); }
        public void assertNotBodyText(String expected) { InvokeAssert(() => this.result = webDriver.GetBodyText(), expected, false); }
        public void waitForBodyText(String expected) { InvokeWaitFor(() => this.result = webDriver.GetBodyText(), expected, true); }
        public void waitForNotBodyText(String expected) { InvokeWaitFor(() => this.result = webDriver.GetBodyText(), expected, false); }
        public String getValue(String locator) { return (String)Invoke(() => this.result = webDriver.GetValue(locator)); }
        public void assertValue(String locator, String expected) { InvokeAssert(() => this.result = webDriver.GetValue(locator), expected, true); }
        public void assertNotValue(String locator, String expected) { InvokeAssert(() => this.result = webDriver.GetValue(locator), expected, false); }
        public void waitForValue(String locator, String expected) { InvokeWaitFor(() => this.result = webDriver.GetValue(locator), expected, true); }
        public void waitForNotValue(String locator, String expected) { InvokeWaitFor(() => this.result = webDriver.GetValue(locator), expected, false); }
        public String getText(String locator) { return (String)Invoke(() => this.result = webDriver.GetText(locator)); }
        public void assertText(String locator, String expected) { InvokeAssert(() => this.result = webDriver.GetText(locator), expected, true); }
        public void assertNotText(String locator, String expected) { InvokeAssert(() => this.result = webDriver.GetText(locator), expected, false); }
        public void waitForText(String locator, String expected) { InvokeWaitFor(() => this.result = webDriver.GetText(locator), expected, true); }
        public void waitForNotText(String locator, String expected) { InvokeWaitFor(() => this.result = webDriver.GetText(locator), expected, false); }
        public void highlight(String locator) { Invoke(() => webDriver.Highlight(locator)); }
        public void highlightAndWait(String locator) { InvokeAndWait(() => webDriver.Highlight(locator)); }
        public String getEval(String script) { return (String)Invoke(() => this.result = webDriver.GetEval(script)); }
        public void assertEval(String script, String expected) { InvokeAssert(() => this.result = webDriver.GetEval(script), expected, true); }
        public void assertNotEval(String script, String expected) { InvokeAssert(() => this.result = webDriver.GetEval(script), expected, false); }
        public void waitForEval(String script, String expected) { InvokeWaitFor(() => this.result = webDriver.GetEval(script), expected, true); }
        public void waitForNotEval(String script, String expected) { InvokeWaitFor(() => this.result = webDriver.GetEval(script), expected, false); }
        public Boolean isChecked(String locator) { return (Boolean)Invoke(() => this.result = webDriver.IsChecked(locator)); }
        public void assertChecked(String locator) { InvokeAssert(() => this.result = webDriver.IsChecked(locator), true, true); }
        public void waitForChecked(String locator) { InvokeWaitFor(() => this.result = webDriver.IsChecked(locator), true, true); }
        public void assertNotChecked(String locator) { InvokeAssert(() => this.result = webDriver.IsChecked(locator), false, true); }
        public void waitForNotChecked(String locator) { InvokeWaitFor(() => this.result = webDriver.IsChecked(locator), false, true); }
        public String getTable(String tableCellAddress) { return (String)Invoke(() => this.result = webDriver.GetTable(tableCellAddress)); }
        public void assertTable(String tableCellAddress, String expected) { InvokeAssert(() => this.result = webDriver.GetTable(tableCellAddress), expected, true); }
        public void assertNotTable(String tableCellAddress, String expected) { InvokeAssert(() => this.result = webDriver.GetTable(tableCellAddress), expected, false); }
        public void waitForTable(String tableCellAddress, String expected) { InvokeWaitFor(() => this.result = webDriver.GetTable(tableCellAddress), expected, true); }
        public void waitForNotTable(String tableCellAddress, String expected) { InvokeWaitFor(() => this.result = webDriver.GetTable(tableCellAddress), expected, false); }
        public String[] getSelectedLabels(String selectLocator) { return (String[])Invoke(() => this.result = webDriver.GetSelectedLabels(selectLocator)); }
        public void assertSelectedLabels(String selectLocator, String[] expected) { InvokeAssert(() => this.result = webDriver.GetSelectedLabels(selectLocator), expected, true); }
        public void assertNotSelectedLabels(String selectLocator, String[] expected) { InvokeAssert(() => this.result = webDriver.GetSelectedLabels(selectLocator), expected, false); }
        public void waitForSelectedLabels(String selectLocator, String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetSelectedLabels(selectLocator), expected, true); }
        public void waitForNotSelectedLabels(String selectLocator, String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetSelectedLabels(selectLocator), expected, false); }
        public String getSelectedLabel(String selectLocator) { return (String)Invoke(() => this.result = webDriver.GetSelectedLabel(selectLocator)); }
        public void assertSelectedLabel(String selectLocator, String expected) { InvokeAssert(() => this.result = webDriver.GetSelectedLabel(selectLocator), expected, true); }
        public void assertNotSelectedLabel(String selectLocator, String expected) { InvokeAssert(() => this.result = webDriver.GetSelectedLabel(selectLocator), expected, false); }
        public void waitForSelectedLabel(String selectLocator, String expected) { InvokeWaitFor(() => this.result = webDriver.GetSelectedLabel(selectLocator), expected, true); }
        public void waitForNotSelectedLabel(String selectLocator, String expected) { InvokeWaitFor(() => this.result = webDriver.GetSelectedLabel(selectLocator), expected, false); }
        public String[] getSelectedValues(String selectLocator) { return (String[])Invoke(() => this.result = webDriver.GetSelectedValues(selectLocator)); }
        public void assertSelectedValues(String selectLocator, String[] expected) { InvokeAssert(() => this.result = webDriver.GetSelectedValues(selectLocator), expected, true); }
        public void assertNotSelectedValues(String selectLocator, String[] expected) { InvokeAssert(() => this.result = webDriver.GetSelectedValues(selectLocator), expected, false); }
        public void waitForSelectedValues(String selectLocator, String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetSelectedValues(selectLocator), expected, true); }
        public void waitForNotSelectedValues(String selectLocator, String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetSelectedValues(selectLocator), expected, false); }
        public String getSelectedValue(String selectLocator) { return (String)Invoke(() => this.result = webDriver.GetSelectedValue(selectLocator)); }
        public void assertSelectedValue(String selectLocator, String expected) { InvokeAssert(() => this.result = webDriver.GetSelectedValue(selectLocator), expected, true); }
        public void assertNotSelectedValue(String selectLocator, String expected) { InvokeAssert(() => this.result = webDriver.GetSelectedValue(selectLocator), expected, false); }
        public void waitForSelectedValue(String selectLocator, String expected) { InvokeWaitFor(() => this.result = webDriver.GetSelectedValue(selectLocator), expected, true); }
        public void waitForNotSelectedValue(String selectLocator, String expected) { InvokeWaitFor(() => this.result = webDriver.GetSelectedValue(selectLocator), expected, false); }
        public String[] getSelectedIndexes(String selectLocator) { return (String[])Invoke(() => this.result = webDriver.GetSelectedIndexes(selectLocator)); }
        public void assertSelectedIndexes(String selectLocator, String[] expected) { InvokeAssert(() => this.result = webDriver.GetSelectedIndexes(selectLocator), expected, true); }
        public void assertNotSelectedIndexes(String selectLocator, String[] expected) { InvokeAssert(() => this.result = webDriver.GetSelectedIndexes(selectLocator), expected, false); }
        public void waitForSelectedIndexes(String selectLocator, String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetSelectedIndexes(selectLocator), expected, true); }
        public void waitForNotSelectedIndexes(String selectLocator, String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetSelectedIndexes(selectLocator), expected, false); }
        public String getSelectedIndex(String selectLocator) { return (String)Invoke(() => this.result = webDriver.GetSelectedIndex(selectLocator)); }
        public void assertSelectedIndex(String selectLocator, String expected) { InvokeAssert(() => this.result = webDriver.GetSelectedIndex(selectLocator), expected, true); }
        public void assertNotSelectedIndex(String selectLocator, String expected) { InvokeAssert(() => this.result = webDriver.GetSelectedIndex(selectLocator), expected, false); }
        public void waitForSelectedIndex(String selectLocator, String expected) { InvokeWaitFor(() => this.result = webDriver.GetSelectedIndex(selectLocator), expected, true); }
        public void waitForNotSelectedIndex(String selectLocator, String expected) { InvokeWaitFor(() => this.result = webDriver.GetSelectedIndex(selectLocator), expected, false); }
        public String[] getSelectedIds(String selectLocator) { return (String[])Invoke(() => this.result = webDriver.GetSelectedIds(selectLocator)); }
        public void assertSelectedIds(String selectLocator, String[] expected) { InvokeAssert(() => this.result = webDriver.GetSelectedIds(selectLocator), expected, true); }
        public void assertNotSelectedIds(String selectLocator, String[] expected) { InvokeAssert(() => this.result = webDriver.GetSelectedIds(selectLocator), expected, false); }
        public void waitForSelectedIds(String selectLocator, String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetSelectedIds(selectLocator), expected, true); }
        public void waitForNotSelectedIds(String selectLocator, String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetSelectedIds(selectLocator), expected, false); }
        public String getSelectedId(String selectLocator) { return (String)Invoke(() => this.result = webDriver.GetSelectedId(selectLocator)); }
        public void assertSelectedId(String selectLocator, String expected) { InvokeAssert(() => this.result = webDriver.GetSelectedId(selectLocator), expected, true); }
        public void assertNotSelectedId(String selectLocator, String expected) { InvokeAssert(() => this.result = webDriver.GetSelectedId(selectLocator), expected, false); }
        public void waitForSelectedId(String selectLocator, String expected) { InvokeWaitFor(() => this.result = webDriver.GetSelectedId(selectLocator), expected, true); }
        public void waitForNotSelectedId(String selectLocator, String expected) { InvokeWaitFor(() => this.result = webDriver.GetSelectedId(selectLocator), expected, false); }
        public Boolean isSomethingSelected(String selectLocator) { return (Boolean)Invoke(() => this.result = webDriver.IsSomethingSelected(selectLocator)); }
        public void assertSomethingSelected(String selectLocator) { InvokeAssert(() => this.result = webDriver.IsSomethingSelected(selectLocator), true, true); }
        public void waitForSomethingSelected(String selectLocator) { InvokeWaitFor(() => this.result = webDriver.IsSomethingSelected(selectLocator), true, true); }
        public void assertNotSomethingSelected(String selectLocator) { InvokeAssert(() => this.result = webDriver.IsSomethingSelected(selectLocator), false, true); }
        public void waitForNotSomethingSelected(String selectLocator) { InvokeWaitFor(() => this.result = webDriver.IsSomethingSelected(selectLocator), false, true); }
        public String[] getSelectOptions(String selectLocator) { return (String[])Invoke(() => this.result = webDriver.GetSelectOptions(selectLocator)); }
        public void assertSelectOptions(String selectLocator, String[] expected) { InvokeAssert(() => this.result = webDriver.GetSelectOptions(selectLocator), expected, true); }
        public void assertNotSelectOptions(String selectLocator, String[] expected) { InvokeAssert(() => this.result = webDriver.GetSelectOptions(selectLocator), expected, false); }
        public void waitForSelectOptions(String selectLocator, String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetSelectOptions(selectLocator), expected, true); }
        public void waitForNotSelectOptions(String selectLocator, String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetSelectOptions(selectLocator), expected, false); }
        public String getAttribute(String attributeLocator) { return (String)Invoke(() => this.result = webDriver.GetAttribute(attributeLocator)); }
        public void assertAttribute(String attributeLocator, String expected) { InvokeAssert(() => this.result = webDriver.GetAttribute(attributeLocator), expected, true); }
        public void assertNotAttribute(String attributeLocator, String expected) { InvokeAssert(() => this.result = webDriver.GetAttribute(attributeLocator), expected, false); }
        public void waitForAttribute(String attributeLocator, String expected) { InvokeWaitFor(() => this.result = webDriver.GetAttribute(attributeLocator), expected, true); }
        public void waitForNotAttribute(String attributeLocator, String expected) { InvokeWaitFor(() => this.result = webDriver.GetAttribute(attributeLocator), expected, false); }
        public Boolean isTextPresent(String pattern) { return (Boolean)Invoke(() => this.result = webDriver.IsTextPresent(pattern)); }
        public void assertTextPresent(String pattern) { InvokeAssert(() => this.result = webDriver.IsTextPresent(pattern), true, true); }
        public void waitForTextPresent(String pattern) { InvokeWaitFor(() => this.result = webDriver.IsTextPresent(pattern), true, true); }
        public void assertTextNotPresent(String pattern) { InvokeAssert(() => this.result = webDriver.IsTextPresent(pattern), false, true); }
        public void waitForTextNotPresent(String pattern) { InvokeWaitFor(() => this.result = webDriver.IsTextPresent(pattern), false, true); }
        public Boolean isElementPresent(String locator) { return (Boolean)Invoke(() => this.result = webDriver.IsElementPresent(locator)); }
        public void assertElementPresent(String locator) { InvokeAssert(() => this.result = webDriver.IsElementPresent(locator), true, true); }
        public void waitForElementPresent(String locator) { InvokeWaitFor(() => this.result = webDriver.IsElementPresent(locator), true, true); }
        public void assertElementNotPresent(String locator) { InvokeAssert(() => this.result = webDriver.IsElementPresent(locator), false, true); }
        public void waitForElementNotPresent(String locator) { InvokeWaitFor(() => this.result = webDriver.IsElementPresent(locator), false, true); }
        public Boolean isVisible(String locator) { return (Boolean)Invoke(() => this.result = webDriver.IsVisible(locator)); }
        public void assertVisible(String locator) { InvokeAssert(() => this.result = webDriver.IsVisible(locator), true, true); }
        public void waitForVisible(String locator) { InvokeWaitFor(() => this.result = webDriver.IsVisible(locator), true, true); }
        public void assertNotVisible(String locator) { InvokeAssert(() => this.result = webDriver.IsVisible(locator), false, true); }
        public void waitForNotVisible(String locator) { InvokeWaitFor(() => this.result = webDriver.IsVisible(locator), false, true); }
        public Boolean isEditable(String locator) { return (Boolean)Invoke(() => this.result = webDriver.IsEditable(locator)); }
        public void assertEditable(String locator) { InvokeAssert(() => this.result = webDriver.IsEditable(locator), true, true); }
        public void waitForEditable(String locator) { InvokeWaitFor(() => this.result = webDriver.IsEditable(locator), true, true); }
        public void assertNotEditable(String locator) { InvokeAssert(() => this.result = webDriver.IsEditable(locator), false, true); }
        public void waitForNotEditable(String locator) { InvokeWaitFor(() => this.result = webDriver.IsEditable(locator), false, true); }
        public String[] getAllButtons() { return (String[])Invoke(() => this.result = webDriver.GetAllButtons()); }
        public void assertAllButtons(String[] expected) { InvokeAssert(() => this.result = webDriver.GetAllButtons(), expected, true); }
        public void assertNotAllButtons(String[] expected) { InvokeAssert(() => this.result = webDriver.GetAllButtons(), expected, false); }
        public void waitForAllButtons(String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetAllButtons(), expected, true); }
        public void waitForNotAllButtons(String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetAllButtons(), expected, false); }
        public String[] getAllLinks() { return (String[])Invoke(() => this.result = webDriver.GetAllLinks()); }
        public void assertAllLinks(String[] expected) { InvokeAssert(() => this.result = webDriver.GetAllLinks(), expected, true); }
        public void assertNotAllLinks(String[] expected) { InvokeAssert(() => this.result = webDriver.GetAllLinks(), expected, false); }
        public void waitForAllLinks(String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetAllLinks(), expected, true); }
        public void waitForNotAllLinks(String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetAllLinks(), expected, false); }
        public String[] getAllFields() { return (String[])Invoke(() => this.result = webDriver.GetAllFields()); }
        public void assertAllFields(String[] expected) { InvokeAssert(() => this.result = webDriver.GetAllFields(), expected, true); }
        public void assertNotAllFields(String[] expected) { InvokeAssert(() => this.result = webDriver.GetAllFields(), expected, false); }
        public void waitForAllFields(String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetAllFields(), expected, true); }
        public void waitForNotAllFields(String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetAllFields(), expected, false); }
        public String[] getAttributeFromAllWindows(String attributeName) { return (String[])Invoke(() => this.result = webDriver.GetAttributeFromAllWindows(attributeName)); }
        public void assertAttributeFromAllWindows(String attributeName, String[] expected) { InvokeAssert(() => this.result = webDriver.GetAttributeFromAllWindows(attributeName), expected, true); }
        public void assertNotAttributeFromAllWindows(String attributeName, String[] expected) { InvokeAssert(() => this.result = webDriver.GetAttributeFromAllWindows(attributeName), expected, false); }
        public void waitForAttributeFromAllWindows(String attributeName, String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetAttributeFromAllWindows(attributeName), expected, true); }
        public void waitForNotAttributeFromAllWindows(String attributeName, String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetAttributeFromAllWindows(attributeName), expected, false); }
        public void dragdrop(String locator, String movementsString) { Invoke(() => webDriver.Dragdrop(locator, movementsString)); }
        public void dragdropAndWait(String locator, String movementsString) { InvokeAndWait(() => webDriver.Dragdrop(locator, movementsString)); }
        public void setMouseSpeed(String pixels) { Invoke(() => webDriver.SetMouseSpeed(pixels)); }
        public Decimal getMouseSpeed() { return (Decimal)Invoke(() => this.result = webDriver.GetMouseSpeed()); }
        public void assertMouseSpeed(Decimal expected) { InvokeAssert(() => this.result = webDriver.GetMouseSpeed(), expected, true); }
        public void assertNotMouseSpeed(Decimal expected) { InvokeAssert(() => this.result = webDriver.GetMouseSpeed(), expected, false); }
        public void waitForMouseSpeed(Decimal expected) { InvokeWaitFor(() => this.result = webDriver.GetMouseSpeed(), expected, true); }
        public void waitForNotMouseSpeed(Decimal expected) { InvokeWaitFor(() => this.result = webDriver.GetMouseSpeed(), expected, false); }
        public void dragAndDrop(String locator, String movementsString) { Invoke(() => webDriver.DragAndDrop(locator, movementsString)); }
        public void dragAndDropAndWait(String locator, String movementsString) { InvokeAndWait(() => webDriver.DragAndDrop(locator, movementsString)); }
        public void dragAndDropToObject(String locatorOfObjectToBeDragged, String locatorOfDragDestinationObject) { Invoke(() => webDriver.DragAndDropToObject(locatorOfObjectToBeDragged, locatorOfDragDestinationObject)); }
        public void dragAndDropToObjectAndWait(String locatorOfObjectToBeDragged, String locatorOfDragDestinationObject) { InvokeAndWait(() => webDriver.DragAndDropToObject(locatorOfObjectToBeDragged, locatorOfDragDestinationObject)); }
        public void windowFocus() { Invoke(() => webDriver.WindowFocus()); }
        public void windowFocusAndWait() { InvokeAndWait(() => webDriver.WindowFocus()); }
        public void windowMaximize() { Invoke(() => webDriver.WindowMaximize()); }
        public void windowMaximizeAndWait() { InvokeAndWait(() => webDriver.WindowMaximize()); }
        public String[] getAllWindowIds() { return (String[])Invoke(() => this.result = webDriver.GetAllWindowIds()); }
        public void assertAllWindowIds(String[] expected) { InvokeAssert(() => this.result = webDriver.GetAllWindowIds(), expected, true); }
        public void assertNotAllWindowIds(String[] expected) { InvokeAssert(() => this.result = webDriver.GetAllWindowIds(), expected, false); }
        public void waitForAllWindowIds(String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetAllWindowIds(), expected, true); }
        public void waitForNotAllWindowIds(String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetAllWindowIds(), expected, false); }
        public String[] getAllWindowNames() { return (String[])Invoke(() => this.result = webDriver.GetAllWindowNames()); }
        public void assertAllWindowNames(String[] expected) { InvokeAssert(() => this.result = webDriver.GetAllWindowNames(), expected, true); }
        public void assertNotAllWindowNames(String[] expected) { InvokeAssert(() => this.result = webDriver.GetAllWindowNames(), expected, false); }
        public void waitForAllWindowNames(String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetAllWindowNames(), expected, true); }
        public void waitForNotAllWindowNames(String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetAllWindowNames(), expected, false); }
        public String[] getAllWindowTitles() { return (String[])Invoke(() => this.result = webDriver.GetAllWindowTitles()); }
        public void assertAllWindowTitles(String[] expected) { InvokeAssert(() => this.result = webDriver.GetAllWindowTitles(), expected, true); }
        public void assertNotAllWindowTitles(String[] expected) { InvokeAssert(() => this.result = webDriver.GetAllWindowTitles(), expected, false); }
        public void waitForAllWindowTitles(String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetAllWindowTitles(), expected, true); }
        public void waitForNotAllWindowTitles(String[] expected) { InvokeWaitFor(() => this.result = webDriver.GetAllWindowTitles(), expected, false); }
        public String getHtmlSource() { return (String)Invoke(() => this.result = webDriver.GetHtmlSource()); }
        public void assertHtmlSource(String expected) { InvokeAssert(() => this.result = webDriver.GetHtmlSource(), expected, true); }
        public void assertNotHtmlSource(String expected) { InvokeAssert(() => this.result = webDriver.GetHtmlSource(), expected, false); }
        public void waitForHtmlSource(String expected) { InvokeWaitFor(() => this.result = webDriver.GetHtmlSource(), expected, true); }
        public void waitForNotHtmlSource(String expected) { InvokeWaitFor(() => this.result = webDriver.GetHtmlSource(), expected, false); }
        public void setCursorPosition(String locator, String position) { Invoke(() => webDriver.SetCursorPosition(locator, position)); }
        public Decimal getElementIndex(String locator) { return (Decimal)Invoke(() => this.result = webDriver.GetElementIndex(locator)); }
        public void assertElementIndex(String locator, Decimal expected) { InvokeAssert(() => this.result = webDriver.GetElementIndex(locator), expected, true); }
        public void assertNotElementIndex(String locator, Decimal expected) { InvokeAssert(() => this.result = webDriver.GetElementIndex(locator), expected, false); }
        public void waitForElementIndex(String locator, Decimal expected) { InvokeWaitFor(() => this.result = webDriver.GetElementIndex(locator), expected, true); }
        public void waitForNotElementIndex(String locator, Decimal expected) { InvokeWaitFor(() => this.result = webDriver.GetElementIndex(locator), expected, false); }
        public Boolean isOrdered(String locator1, String locator2) { return (Boolean)Invoke(() => this.result = webDriver.IsOrdered(locator1, locator2)); }
        public void assertOrdered(String locator1, String locator2) { InvokeAssert(() => this.result = webDriver.IsOrdered(locator1, locator2), true, true); }
        public void waitForOrdered(String locator1, String locator2) { InvokeWaitFor(() => this.result = webDriver.IsOrdered(locator1, locator2), true, true); }
        public void assertNotOrdered(String locator1, String locator2) { InvokeAssert(() => this.result = webDriver.IsOrdered(locator1, locator2), false, true); }
        public void waitForNotOrdered(String locator1, String locator2) { InvokeWaitFor(() => this.result = webDriver.IsOrdered(locator1, locator2), false, true); }
        public Decimal getElementPositionLeft(String locator) { return (Decimal)Invoke(() => this.result = webDriver.GetElementPositionLeft(locator)); }
        public void assertElementPositionLeft(String locator, Decimal expected) { InvokeAssert(() => this.result = webDriver.GetElementPositionLeft(locator), expected, true); }
        public void assertNotElementPositionLeft(String locator, Decimal expected) { InvokeAssert(() => this.result = webDriver.GetElementPositionLeft(locator), expected, false); }
        public void waitForElementPositionLeft(String locator, Decimal expected) { InvokeWaitFor(() => this.result = webDriver.GetElementPositionLeft(locator), expected, true); }
        public void waitForNotElementPositionLeft(String locator, Decimal expected) { InvokeWaitFor(() => this.result = webDriver.GetElementPositionLeft(locator), expected, false); }
        public Decimal getElementPositionTop(String locator) { return (Decimal)Invoke(() => this.result = webDriver.GetElementPositionTop(locator)); }
        public void assertElementPositionTop(String locator, Decimal expected) { InvokeAssert(() => this.result = webDriver.GetElementPositionTop(locator), expected, true); }
        public void assertNotElementPositionTop(String locator, Decimal expected) { InvokeAssert(() => this.result = webDriver.GetElementPositionTop(locator), expected, false); }
        public void waitForElementPositionTop(String locator, Decimal expected) { InvokeWaitFor(() => this.result = webDriver.GetElementPositionTop(locator), expected, true); }
        public void waitForNotElementPositionTop(String locator, Decimal expected) { InvokeWaitFor(() => this.result = webDriver.GetElementPositionTop(locator), expected, false); }
        public Decimal getElementWidth(String locator) { return (Decimal)Invoke(() => this.result = webDriver.GetElementWidth(locator)); }
        public void assertElementWidth(String locator, Decimal expected) { InvokeAssert(() => this.result = webDriver.GetElementWidth(locator), expected, true); }
        public void assertNotElementWidth(String locator, Decimal expected) { InvokeAssert(() => this.result = webDriver.GetElementWidth(locator), expected, false); }
        public void waitForElementWidth(String locator, Decimal expected) { InvokeWaitFor(() => this.result = webDriver.GetElementWidth(locator), expected, true); }
        public void waitForNotElementWidth(String locator, Decimal expected) { InvokeWaitFor(() => this.result = webDriver.GetElementWidth(locator), expected, false); }
        public Decimal getElementHeight(String locator) { return (Decimal)Invoke(() => this.result = webDriver.GetElementHeight(locator)); }
        public void assertElementHeight(String locator, Decimal expected) { InvokeAssert(() => this.result = webDriver.GetElementHeight(locator), expected, true); }
        public void assertNotElementHeight(String locator, Decimal expected) { InvokeAssert(() => this.result = webDriver.GetElementHeight(locator), expected, false); }
        public void waitForElementHeight(String locator, Decimal expected) { InvokeWaitFor(() => this.result = webDriver.GetElementHeight(locator), expected, true); }
        public void waitForNotElementHeight(String locator, Decimal expected) { InvokeWaitFor(() => this.result = webDriver.GetElementHeight(locator), expected, false); }
        public Decimal getCursorPosition(String locator) { return (Decimal)Invoke(() => this.result = webDriver.GetCursorPosition(locator)); }
        public void assertCursorPosition(String locator, Decimal expected) { InvokeAssert(() => this.result = webDriver.GetCursorPosition(locator), expected, true); }
        public void assertNotCursorPosition(String locator, Decimal expected) { InvokeAssert(() => this.result = webDriver.GetCursorPosition(locator), expected, false); }
        public void waitForCursorPosition(String locator, Decimal expected) { InvokeWaitFor(() => this.result = webDriver.GetCursorPosition(locator), expected, true); }
        public void waitForNotCursorPosition(String locator, Decimal expected) { InvokeWaitFor(() => this.result = webDriver.GetCursorPosition(locator), expected, false); }
        public String getExpression(String expression) { return (String)Invoke(() => this.result = webDriver.GetExpression(expression)); }
        public void assertExpression(String expression, String expected) { InvokeAssert(() => this.result = webDriver.GetExpression(expression), expected, true); }
        public void assertNotExpression(String expression, String expected) { InvokeAssert(() => this.result = webDriver.GetExpression(expression), expected, false); }
        public void waitForExpression(String expression, String expected) { InvokeWaitFor(() => this.result = webDriver.GetExpression(expression), expected, true); }
        public void waitForNotExpression(String expression, String expected) { InvokeWaitFor(() => this.result = webDriver.GetExpression(expression), expected, false); }
        public Decimal getXpathCount(String xpath) { return (Decimal)Invoke(() => this.result = webDriver.GetXpathCount(xpath)); }
        public void assertXpathCount(String xpath, Decimal expected) { InvokeAssert(() => this.result = webDriver.GetXpathCount(xpath), expected, true); }
        public void assertNotXpathCount(String xpath, Decimal expected) { InvokeAssert(() => this.result = webDriver.GetXpathCount(xpath), expected, false); }
        public void waitForXpathCount(String xpath, Decimal expected) { InvokeWaitFor(() => this.result = webDriver.GetXpathCount(xpath), expected, true); }
        public void waitForNotXpathCount(String xpath, Decimal expected) { InvokeWaitFor(() => this.result = webDriver.GetXpathCount(xpath), expected, false); }
        public Decimal getCssCount(String cssLocator) { return (Decimal)Invoke(() => this.result = webDriver.GetCSSCount(cssLocator)); }
        public void assertCssCount(String cssLocator, Decimal expected) { InvokeAssert(() => this.result = webDriver.GetCSSCount(cssLocator), expected, true); }
        public void assertNotCssCount(String cssLocator, Decimal expected) { InvokeAssert(() => this.result = webDriver.GetCSSCount(cssLocator), expected, false); }
        public void waitForCssCount(String cssLocator, Decimal expected) { InvokeWaitFor(() => this.result = webDriver.GetCSSCount(cssLocator), expected, true); }
        public void waitForNotCssCount(String cssLocator, Decimal expected) { InvokeWaitFor(() => this.result = webDriver.GetCSSCount(cssLocator), expected, false); }
        public void assignId(String locator, String identifier) { Invoke(() => webDriver.AssignId(locator, identifier)); }
        public void assignIdAndWait(String locator, String identifier) { InvokeAndWait(() => webDriver.AssignId(locator, identifier)); }
        public void allowNativeXpath(String allow) { Invoke(() => webDriver.AllowNativeXpath(allow)); }
        public void allowNativeXpathAndWait(String allow) { InvokeAndWait(() => webDriver.AllowNativeXpath(allow)); }
        public void ignoreAttributesWithoutValue(String ignore) { Invoke(() => webDriver.IgnoreAttributesWithoutValue(ignore)); }
        public void ignoreAttributesWithoutValueAndWait(String ignore) { InvokeAndWait(() => webDriver.IgnoreAttributesWithoutValue(ignore)); }
        public void waitForCondition(String script, String timeout) { Invoke(() => webDriver.WaitForCondition(script, timeout)); }
        #endregion

    }

}
