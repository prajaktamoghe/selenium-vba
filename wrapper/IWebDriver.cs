using System;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace SeleniumWrapper
{
    [Guid("24cd39f2-f552-4a61-82fe-cc6284398aa5")]
    [ComVisible(true)]
    public partial interface IWebDriver
    {
        [Description("Specifies the amount of time the driver should wait when searching for an element if it is not immediately present.")]
        void setImplicitWait(int timeout_ms);

        [Description("Specifies the amount of time that Selenium will wait for actions to complete. The default timeout is 30 seconds.")]
        void setTimeout(int timeout_ms);

        [Description("Set a specific preference for Firefox")]
        void setPreference (string parameter, object value);

        [Description("Starts a new Selenium session")]
        void start(String browser, String url, [Optional][DefaultParameterValue("")]String directory);

        [Description("Starts remotely a new Selenium session")]
        void startRemotely(String browser, String remoteAddress, String url, [Optional][DefaultParameterValue(true)]Boolean javascriptEnabled);

        [Description("Opens an URL in the test frame. This accepts both relative and absolute URLs.")]
        void open(String url);

        [Description("Wait the specified time in millisecond before executing the next command")]
        void wait(int time_ms);

        [Description("Wait the specified time in millisecond before executing the next command")]
        void pause(int time_ms);

        [Description("Returns a string with the result of the verification ( <OK> or <KO, ...> )")]
        String verifyEqual(Object expected, Object current);

        [Description("Returns a string with the result of the verification ( <OK> or <KO, ...> )")]
        String verifyNotEqual(Object expected, Object current);

        [Description("Raise an error if the assertion fails")]
        void assertEqual(Object expected, Object current);

        [Description("Raise an error if the assertion fails")]
        void assertNotEqual(Object expected, Object current);

        [Description("Capture a screenshot to the Clipboard")]
        void captureScreenshotToClipboard();

        [Description("Capture a screenshot")]
        [return: MarshalAs(UnmanagedType.IUnknown)]
        object captureScreenshotToImage();

        [Description("Execute JavaScrip on the page")]
        Object executeScript(String script, [Optional][DefaultParameterValue(null)]Object arguments);

        [Description("Undo the effect of calling chooseCancelOnNextConfirmation. Note that Selenium's overridden window.confirm() function will normally automatically return true, as if the user had manually clicked OK, so you shouldn't need to use this command unless for some reason you need to change your mind prior to the next confirmation. After any confirmation, Selenium will resume using the default behavior for future confirmations, automatically returning true (OK) unless/until you explicitly call chooseCancelOnNextConfirmation for each confirmation.  Take note - every time a confirmation comes up, you must consume it with a corresponding getConfirmation, or else the next selenium operation will fail.")]
        void chooseOkOnNextConfirmation();

        [Description("By default, Selenium's overridden window.confirm() function will return true, as if the user had manually clicked OK; after running this command, the next call to confirm() will return false, as if the user had clicked Cancel. Selenium will then resume using the default behavior for future confirmations, automatically returning true (OK) unless/until you explicitly call this command for each confirmation.  Take note - every time a confirmation comes up, you must consume it with a corresponding getConfirmation, or else the next selenium operation will fail.")]
        void chooseCancelOnNextConfirmation();

        [Description("Resize currently selected window to take up the entire screen ")]
        void windowMaximize();

        [Description("Returns the page source")]
        String pageSource { get; }

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

        [Description("Load a new web page in the current browser window.")]
        void get(String url);

        [Description("Sends a sequence of keystrokes to the browser.")]
        void sendKeys(string keysToSend);

        [Description("Indicates whether the regular expression finds a match in the input string")]
        bool isMatch(string input, string pattern);

        [Description("Searches the input string for an occurrence of a regular expression with a specified input string")]
        object match(string input, string pattern);

        [Description("Within a specified input string, replaces all strings that match a regular expression pattern with a specified replacement string.")]
        string replace(string value, string pattern, string replacement);

    }
}
