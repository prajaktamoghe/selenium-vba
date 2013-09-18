using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SeleniumWrapper
{
    [Guid("0CBCED71-4792-46BD-A527-8663BF7D9592")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface WebDriverEvents
    {
        //void EndOfCommand();
    }

    [Guid("24cd39f2-f552-4a61-82fe-cc6284398aa5")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IWebDriver
    {
        [Description("Specifies the amount of time the driver should wait when searching for an element if it is not immediately present.")]
        void setImplicitWait(int timeoutms);

        [Description("Specifies the amount of time that Selenium will wait for actions to complete. The default timeout is 30 seconds.")]
        void setTimeout(int timeoutms);

        [Description("Specifies or get the amount of time that Selenium will wait for actions to complete. The default timeout is 30 seconds.")]
        int Timeout{get;set;}

        [Description("Set a specific profile directory (Firefox and Chrome) or profile name (Firefox only)")]
        void setProfile(string directory);

        [Description("Set a specific preference")]
        void setPreference(string key, object value);

        [Description("Set a specific capability")]
        void setCapability(string key, object value);

        [Description("Add an extension to the browser (For Firefox and Chrome only)")]
        void addExtension(string extensionPath);

        [Description("Set a specific proxy")]
        void setProxy(string url, [Optional][DefaultParameterValue(false)]bool isAutoConfigURL);

        [Description("Starts a new Selenium session")]
        void start(String browser, String url, [Optional][DefaultParameterValue("")]String directory);

        [Description("Starts remotely a new Selenium session")]
        void startRemotely(String browser, String remoteAddress, String url);

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

        [Description("Saves the entire contents of the current window canvas to a PNG file. Contrast this with the captureScreenshot command, which captures the contents of the OS viewport (i.e. whatever is currently being displayed on the monitor), and is implemented in the RC only. Currently this only works in Firefox when running in chrome mode, and in IE non-HTA using the EXPERIMENTAL \"Snapsie\" utility. The Firefox implementation is mostly borrowed from the Screengrab! Firefox extension. Please see http://www.screengrab.org and http://snapsie.sourceforge.net/ for details. the path to the file to persist the screenshot as. No filename extension will be appended by default. Directories will not be created if they do not exist, and an exception will be thrown, possibly by native code.a kwargs string that modifies the way the screenshot is captured. Example: \"background=#CCFFDD\" . Currently valid options: backgroundthe background CSS for the HTML document. This may be useful to set for capturing screenshots of less-than-ideal layouts, for example where absolute positioning causes the calculation of the canvas dimension to fail and a black background is exposed (possibly obscuring black text).")]
        void captureEntirePageScreenshot(String filename, [Optional][DefaultParameterValue("")]String kwargs);

        [Description("Capture a screenshot")]
        Image getScreenshot();

        [Description("Execute JavaScrip on the page")]
        Object executeScript(String script, [Optional][DefaultParameterValue(null)]Object arguments);

        [Description("Undo the effect of calling chooseCancelOnNextConfirmation. Note that Selenium's overridden window.confirm() function will normally automatically return true, as if the user had manually clicked OK, so you shouldn't need to use this command unless for some reason you need to change your mind prior to the next confirmation. After any confirmation, Selenium will resume using the default behavior for future confirmations, automatically returning true (OK) unless/until you explicitly call chooseCancelOnNextConfirmation for each confirmation.  Take note - every time a confirmation comes up, you must consume it with a corresponding getConfirmation, or else the next selenium operation will fail.")]
        void chooseOkOnNextConfirmation();

        [Description("By default, Selenium's overridden window.confirm() function will return true, as if the user had manually clicked OK; after running this command, the next call to confirm() will return false, as if the user had clicked Cancel. Selenium will then resume using the default behavior for future confirmations, automatically returning true (OK) unless/until you explicitly call this command for each confirmation.  Take note - every time a confirmation comes up, you must consume it with a corresponding getConfirmation, or else the next selenium operation will fail.")]
        void chooseCancelOnNextConfirmation();

        [Description("Resize currently selected window to take up the entire screen ")]
        void windowMaximize();

        [Description("Returns the page source")]
        String PageSource { get; }

        [Description("Find the first WebElement using the given method.")]
        WebElement findElement(ref Object by, [Optional][DefaultParameterValue(0)]int timeoutms);

        [Description("Finds the first element matching the specified name.")]
        WebElement findElementByName(String name, [Optional][DefaultParameterValue(0)]int timeoutms);

        [Description("Finds the first element matching the specified XPath query.")]
        WebElement findElementByXPath(String xpath, [Optional][DefaultParameterValue(0)]int timeoutms);

        [Description("Finds the first element matching the specified id.")]
        WebElement findElementById(String id, [Optional][DefaultParameterValue(0)]int timeoutms);

        [Description("Finds the first element matching the specified CSS class.")]
        WebElement findElementByClassName(String classname, [Optional][DefaultParameterValue(0)]int timeoutms);

        [Description("Finds the first element matching the specified CSS selector.")]
        WebElement findElementByCssSelector(String cssselector, [Optional][DefaultParameterValue(0)]int timeoutms);

        [Description("Finds the first element matching the specified link text.")]
        WebElement findElementByLinkText(String linktext, [Optional][DefaultParameterValue(0)]int timeoutms);

        [Description("Finds the first of elements that match the part of the link text supplied")]
        WebElement findElementByPartialLinkText(String partiallinktext, [Optional][DefaultParameterValue(0)]int timeoutms);

        [Description("Finds the first element matching the specified tag name.")]
        WebElement findElementByTagName(String tagname, [Optional][DefaultParameterValue(0)]int timeoutms);
        
        [Description("Indicates whether a WebElement is present using the given method.")]
        bool isElementPresent(ref object by, [Optional][DefaultParameterValue(0)]int timeoutms);

        [Description("Find all elements within the current context using the given mechanism.")]
        WebElement[] findElements(ref object by, int timeoutms);

        [Description("Finds elements matching the specified name.")]
        WebElement[] findElementsByName(String name, [Optional][DefaultParameterValue(0)]int timeoutms);

        [Description("Finds elements matching the specified XPath query.")]
        WebElement[] findElementsByXPath(String xpath, [Optional][DefaultParameterValue(0)]int timeoutms);

        [Description("Finds elements matching the specified id.")]
        WebElement[] findElementsById(String id, [Optional][DefaultParameterValue(0)]int timeoutms);

        [Description("Finds elements matching the specified CSS class.")]
        WebElement[] findElementsByClassName(String classname, [Optional][DefaultParameterValue(0)]int timeoutms);

        [Description("Finds elements matching the specified CSS selector.")]
        WebElement[] findElementsByCssSelector(String cssselector, [Optional][DefaultParameterValue(0)]int timeoutms);

        [Description("Finds elements matching the specified link text.")]
        WebElement[] findElementsByLinkText(String linktext, [Optional][DefaultParameterValue(0)]int timeoutms);

        [Description("Finds the first of elements that match the part of the link text supplied")]
        WebElement[] findElementsByPartialLinkText(String partiallinktext, [Optional][DefaultParameterValue(0)]int timeoutms);

        [Description("Finds elements matching the specified tag name.")]
        WebElement[] findElementsByTagName(String tagname, [Optional][DefaultParameterValue(0)]int timeoutms);

        [Description("Switches focus to the specified window.")]
        WebDriver switchToWindow(string windowName, [Optional][DefaultParameterValue(0)]int timeoutms);

        [Description("Switches focus to the specified frame, by index or name.")]
        WebDriver switchToFrame(object index_or_name, [Optional][DefaultParameterValue(0)]int timeoutms);

        [Description("Switches focus to an alert on the page.")]
        Alert switchToAlert([Optional][DefaultParameterValue(0)]int timeoutms);

        [Description("Returns the page tile")]
        string Title{get;}

        [Description("Load a new web page in the current browser window.")]
        void get(String url);

        [Description("Sends a sequence of keystrokes to the browser.")]
        void sendKeys(string keysToSend);

        [Description("Returns the current Url.")]
        string Url{get;}

        [Description("Indicates whether the regular expression finds a match in the input string")]
        bool isMatch(string pattern);

        [Description("Searches the input string for an occurrence of a regular expression with a specified input string")]
        object match(string pattern);
    }
}
