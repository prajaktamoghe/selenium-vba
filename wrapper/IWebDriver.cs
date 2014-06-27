using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;

namespace SeleniumWrapper {
    [Guid("0CBCED71-4792-46BD-A527-8663BF7D9592")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface WebDriverEvents {
        //void EndOfCommand();
    }

    [Guid("24cd39f2-f552-4a61-82fe-cc6284398aa5")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public partial interface IWebDriver {

        void SetDoEvents(object procedure);

        [Description("Get the Actions class")]
        Actions Actions { get; }

        [Description("Specifies the amount of time the driver should wait when searching for an element if it is not immediately present.")]
        void setImplicitWait(int timeoutms);

        [Description("Specifies the amount of time that Selenium will wait for actions to complete. The default timeout is 30 seconds.")]
        void setTimeout(int timeoutms);

        [Description("Specifies or get the amount of time that Selenium will wait for actions to complete. The default timeout is 30 seconds.")]
        int Timeout { get; set; }

        [Description("Set a specific profile directory (Firefox and Chrome) or profile name (Firefox only)")]
        void setProfile(string directory);

        [Description("Set a specific preference")]
        void setPreference(string key, object value);

        [Description("Set a specific capability")]
        void setCapability(string key, object value);

        [Description("Add an extension to the browser (For Firefox and Chrome only)")]
        void addExtension(string extensionPath);

        [Description("Add an argument to be appended to the command line to launch the browser")]
        void addArgument(string argument);

        [Description("Set a specific proxy")]
        void setProxy(string url, bool isAutoConfigURL = false);

        [Description("Gets or sets a value indicating whether the command prompt window of the service should be hidden.")]
        bool HideCommandPromptWindow { get; set; }

        [Description("Starts a new Selenium session")]
        void start(String browser, String baseUrl = null, bool useLastSession = false);

        [Description("Starts remotely a new Selenium session")]
        void startRemotely(String browser, String remoteAddress, String baseUrl = null);

        [Description("Ends the current Selenium testing session (normally killing the browser)")]
        void stop();

        [Description("Opens an URL in the test frame. This accepts both relative and absolute URLs.")]
        void open(String url);

        [Description("Wait the specified time in millisecond before executing the next command")]
        void wait(int timems);

        [Description("Wait the specified time in millisecond before executing the next command")]
        void pause(int timems);

        [Description("Wait the specified time in millisecond before executing the next command")]
        void sleep(int timems);

        [Description("Wait for the callback procedure to return true")]
        WebDriver WaitFor(object procedure, int timeoutms = 6000);

        [Description("Saves the entire contents of the current window canvas to a PNG file. Contrast this with the captureScreenshot command, which captures the contents of the OS viewport (i.e. whatever is currently being displayed on the monitor), and is implemented in the RC only. Currently this only works in Firefox when running in chrome mode, and in IE non-HTA using the EXPERIMENTAL \"Snapsie\" utility. The Firefox implementation is mostly borrowed from the Screengrab! Firefox extension. Please see http://www.screengrab.org and http://snapsie.sourceforge.net/ for details. the path to the file to persist the screenshot as. No filename extension will be appended by default. Directories will not be created if they do not exist, and an exception will be thrown, possibly by native code.a kwargs string that modifies the way the screenshot is captured. Example: \"background=#CCFFDD\" . Currently valid options: backgroundthe background CSS for the HTML document. This may be useful to set for capturing screenshots of less-than-ideal layouts, for example where absolute positioning causes the calculation of the canvas dimension to fail and a black background is exposed (possibly obscuring black text).")]
        void captureEntirePageScreenshot(String filename, String kwargs = null);

        [Description("Capture a screenshot")]
        Image getScreenshot(int delayms = 0);

        [Description("Execute JavaScrip on the page")]
        Object executeScript(String script, object arguments = null);

        [Description("Execute JavaScrip on the page until it doesn't throw an error")]
        Object waitForScriptSuccees(String script, object arguments = null, int timeoutms = 5000);

        [Description("Wait for a script condition to return true")]
        void waitForScriptCondition(String scriptCondition, object arguments = null, int timeoutms = 5000);

        [Description("Wait for a script object(defined and not null)")]
        void waitForScriptObject(String objectName, int timeoutms = 5000);

        [Description("Undo the effect of calling chooseCancelOnNextConfirmation. Note that Selenium's overridden window.confirm() function will normally automatically return true, as if the user had manually clicked OK, so you shouldn't need to use this command unless for some reason you need to change your mind prior to the next confirmation. After any confirmation, Selenium will resume using the default behavior for future confirmations, automatically returning true (OK) unless/until you explicitly call chooseCancelOnNextConfirmation for each confirmation.  Take note - every time a confirmation comes up, you must consume it with a corresponding getConfirmation, or else the next selenium operation will fail.")]
        void chooseOkOnNextConfirmation();

        [Description("By default, Selenium's overridden window.confirm() function will return true, as if the user had manually clicked OK; after running this command, the next call to confirm() will return false, as if the user had clicked Cancel. Selenium will then resume using the default behavior for future confirmations, automatically returning true (OK) unless/until you explicitly call this command for each confirmation.  Take note - every time a confirmation comes up, you must consume it with a corresponding getConfirmation, or else the next selenium operation will fail.")]
        void chooseCancelOnNextConfirmation();

        [Description("Resize currently selected window to take up the entire screen ")]
        void windowMaximize();

        [Description("Returns the page source")]
        String PageSource { get; }

        [Description("Find the first WebElement using the given method.")]
        WebElement findElement([MarshalAs(UnmanagedType.IDispatch)]By by, int timeoutms = 0);

        [Description("Finds the first element matching the specified name.")]
        WebElement findElementByName(String name, int timeoutms = 0);

        [Description("Finds the first element matching the specified XPath query.")]
        WebElement findElementByXPath(String xpath, int timeoutms = 0);

        [Description("Finds the first element matching the specified id.")]
        WebElement findElementById(String id, int timeoutms = 0);

        [Description("Finds the first element matching the specified CSS class.")]
        WebElement findElementByClassName(String classname, int timeoutms = 0);

        [Description("Finds the first element matching the specified CSS selector.")]
        WebElement findElementByCssSelector(String cssselector, int timeoutms = 0);

        [Description("Finds the first element matching the specified link text.")]
        WebElement findElementByLinkText(String linktext, int timeoutms = 0);

        [Description("Finds the first of elements that match the part of the link text supplied")]
        WebElement findElementByPartialLinkText(String partiallinktext, int timeoutms = 0);

        [Description("Finds the first element matching the specified tag name.")]
        WebElement findElementByTagName(String tagname, int timeoutms = 0);

        [Description("Indicates whether a WebElement is present using the given method.")]
        bool isElementPresent(object locator);

        [Description("Find all elements within the current context using the given mechanism.")]
        WebElementCollection findElements([MarshalAs(UnmanagedType.IDispatch)]By by, int timeoutms = 0);

        [Description("Finds elements matching the specified name.")]
        WebElementCollection findElementsByName(String name, int timeoutms = 0);

        [Description("Finds elements matching the specified XPath query.")]
        WebElementCollection findElementsByXPath(String xpath, int timeoutms = 0);

        [Description("Finds elements matching the specified id.")]
        WebElementCollection findElementsById(String id, int timeoutms = 0);

        [Description("Finds elements matching the specified CSS class.")]
        WebElementCollection findElementsByClassName(String classname, int timeoutms = 0);

        [Description("Finds elements matching the specified CSS selector.")]
        WebElementCollection findElementsByCssSelector(String cssselector, int timeoutms = 0);

        [Description("Finds elements matching the specified link text.")]
        WebElementCollection findElementsByLinkText(String linktext, int timeoutms = 0);

        [Description("Finds the first of elements that match the part of the link text supplied")]
        WebElementCollection findElementsByPartialLinkText(String partiallinktext, int timeoutms = 0);

        [Description("Finds elements matching the specified tag name.")]
        WebElementCollection findElementsByTagName(String tagname, int timeoutms = 0);

        [Description("Waits for an element to be removed")]
        void WaitNotElement(By by, int timeoutms = -1);

        [Description("Waits for the title to match the pattern")]
        void WaitTitleMatches(string pattern, int timeoutms = -1);

        [Description("Switches focus to the specified window.")]
        WebDriver switchToWindow(string windowName, int timeoutms = 0);

        [Description("Switches focus to the specified frame, by index, name or WebElement.")]
        WebDriver switchToFrame(object index_name_element, int timeoutms = 0);

        [Description("Switches focus to an alert on the page.")]
        Alert switchToAlert(int timeoutms = 0);

        [Description("Selects either the first frame on the page or the main document when a page contains iFrames.")]
        WebDriver switchToDefaultContent();

        [Description("Returns the page tile")]
        string Title { get; }

        [Description("Sets the size of the outer browser window, including title bars and window borders.")]
        void setWindowSize(int width, int height);

        [Description("Sets the position of the browser window relative to the upper-left corner of the screen.")]
        void setWindowPosition(int x, int y);

        [Description("Maximizes the current window if it is not already maximized")]
        void maximizeWindow();

        [Description("Load a new web page in the current browser window.")]
        void get(String url);

        [Description("Sends a sequence of keystrokes to the browser.")]
        void sendKeys(string keysOrModifier, string keys = null);

        [Description("Sends keystrokes to the active application using the windows SendKeys methode.")]
        void sendKeysNat(string keys);

        [Description("Returns the current Url.")]
        string Url { get; }

        [Description("Goes one step backward in the browser history.")]
        void back();

        [Description("Goes one step forward in the browser history.")]
        void forward();

        [Description("Closes the current window.")]
        void close();

        [Description("Returns the handle of the current window.")]
        string WindowHandle { get; }

        [Description("Returns the handles of all windows within the current session.")]
        string[] WindowHandles { get; }

        [Description("Returns the element with focus, or BODY if nothing has focus.")]
        WebElement ActiveElement { get; }

        [Description("Indicates whether the regular expression finds a match in the input string")]
        bool isMatchPageSource(string pattern);

        [Description("Searches the input string for an occurrence of a regular expression with a specified input string")]
        object matchPageSource(string pattern);

        [Description("Set text in the Clipboard")]
        void setClipBoard(string text);

        [Description("Get text from the Clipboard")]
        string getClipBoard();

        [Description("Get the page loading metrics in millisecond. [Page loading, Server waiting, Server receiving, DOM loading]")]
        object[,] getPerformanceTiming();
    }
}
