using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SeleniumWrapper
{
    [Guid("DB0ACE39-3B49-49CA-9CDF-8B145197B76C")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IWebElement
    {

        [Description("Cast the WebElement to a Select element ")]
        Select AsSelect { get; }

        [Description("Cast the WebElement to a Table element ")]
        Table AsTable { get; }

        [Description("Clears the text if it’s a text entry element.")]
        WebElement clear();

        [Description("Clicks the element.")]
        void click();

        [Description("Clicks at the element offset.")]
        void clickByOffset(int offset_x, int offset_y);

        [Description("Clicks the element and hold.")]
        void clickAndHold();

        [Description("Release a click")]
        void releaseClick();

        [Description("Rigth clicks the element")]
        void contextClick();

        [Description("Whether the element would be visible to a user")]
        bool Displayed { get; }

        [Description("Double clicks the element.")]
        void doubleClick();

        [Description("Drag and drop the element to an offset")]
        void dragAndDropToOffset(int offsetX, int offsetY);

        [Description("Drag and drop the element to another element")]
        void dragAndDropToWebElement([MarshalAs(UnmanagedType.IDispatch)]WebElement target);

        [Description("Whether the element is enabled.")]
        bool Enabled { get; }

        [Description("Gets the attribute value.")]
        string getAttribute(string attributeName);

        [Description("Assert an attribute value")]
        void assertAttribute(string attributeName, string expected);

        [Description("Return the verification result of an attribute value")]
        string verifyAttribute(string attributeName, string expected);

        [Description("Returns the value of a CSS property")]
        string getCssValue(string propertyName);

        [Description("Return the verification result of a CSS property")]
        string verifyCssValue(string propertyName, string expected);

        [Description("Assert a CSS property")]
        void assertCssValue(string propertyName, string expected);

        [Description("Press a key and hold")]
        void keyDown(string theKey);

        [Description("Release a key")]
        void keyUp(string theKey);

        [Description("Returns the location of the element in the renderable canvas")]
        Point Location { get; }

        [Description("Whether the element is selected.")]
        bool Selected { get; }

        [Description("Simulates typing into the element.")]
        void sendKeys(string keysOrModifier, string keys = null);

        [Description("Returns the size of the element")]
        Size Size { get; }

        [Description("Submits a form.")]
        void submit();

        [Description("Gets this element’s tagName property.")]
        string TagName { get; }

        [Description("Gets the text of the element.")]
        string Text { get; }

        [Description("Assert the text of an element is equal to the expected")]
        void assertText(string expected);

        [Description("Assert the text of an element is not equal to the expected")]
        void assertNotText(string expected);

        [Description("Verifiy the element's text is not equal to the expected and return the result")]
        string verifyNotText(string expected);

        [Description("Verifiy the element's text is equal to the expected and return the result")]
        string verifyText(string expected);

        [Description("Indicates whether the regular expression finds a match in the input string")]
        bool isMatch(string pattern);

        [Description("Searches the input string for an occurrence of a regular expression with a specified input string")]
        object match(string pattern);

        [Description("Within a specified input string, replaces all strings that match a regular expression pattern with a specified replacement string.")]
        string replace(string pattern, string replacement);

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

        [Description("Wait for call to a procedure to return true. The procedure receives a WebElement as argument and returns a Boolean.")]
        WebElement WaitFor(object procedure, int timeoutms = -1);

        [Description("Wait for an attribute")]
        WebElement waitForAttribute(string attribute, string pattern, int timeoutms = -1);

        [Description("Wait for a CSS property")]
        WebElement waitForCssValue(string propertyName, string value, int timeoutms = -1);

        [Description("Wait for a web element to be displayed or not. Default is displayed.")]
        WebElement WaitForDisplayed(bool displayed = true, int timeoutms = -1);

        [Description("Wait for a web element to be enabled or not. Default is enabled.")]
        WebElement WaitForEnabled(bool enabled = true, int timeoutms = -1);

        [Description("Wait for a different attribute")]
        WebElement waitForNotAttribute(string attribute, string pattern, int timeoutms = -1);

        [Description("Wait for a different CSS property")]
        WebElement waitForNotCssValue(string propertyName, string value, int timeoutms = -1);
        
        [Description("Wait for a different text")]
        WebElement waitForNotText(string pattern, int timeoutms = -1);

        [Description("Wait for a selection to true or false. Default is true.")]
        WebElement WaitForSelection(bool selected = true, int timeoutms = -1);
        
        [Description("Wait for text")]
        WebElement waitForText(string pattern, int timeoutms = -1);

        [Description("Gets the screenshot of the current element")]
        Image getScreenshot();
    }

}
