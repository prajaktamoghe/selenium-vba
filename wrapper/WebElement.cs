using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections;
using OpenQA.Selenium;

namespace SeleniumWrapper {
    /// <summary>
    /// Defines the interface through which the user controls elements on the page. 
    /// </summary>

    [Description("Defines the interface through which the user controls elements on the page.")]
    [Guid("567CB939-FD53-4E83-A8D2-A693991646BE")]
    [ComVisible(true), ComDefaultInterface(typeof(IWebElement)), ClassInterface(ClassInterfaceType.None)]
    public class WebElement : IWebElement, Select, Table {

        internal OpenQA.Selenium.IWebElement _webElement;
        internal OpenQA.Selenium.IWebDriver _webDriver;
        internal WebDriver _wd;

        internal WebElement(WebDriver webDriver, OpenQA.Selenium.IWebElement webElement) {
            this._wd = webDriver;
            _webDriver = _wd.WebDriver;
            _webElement = webElement;
        }

        /// <summary>Whether the element would be visible to a user</summary>
        public bool Displayed {
            get { return _webElement.Displayed; }
        }

        /// <summary>Whether the element is enabled.</summary>
        public bool Enabled {
            get { return _webElement.Enabled; }
        }

        /// <summary>Returns the location of the element in the renderable canvas</summary>
        public Point Location {
            get { return new Point { X = _webElement.Location.X, Y = _webElement.Location.Y }; }
        }

        /// <summary>Whether the element is selected.</summary>
        public bool Selected {
            get { return _webElement.Selected; }
        }

        /// <summary>Returns the size of the element</summary>
        public Size Size {
            get { return new Size { Width = _webElement.Size.Width, Height = _webElement.Size.Height }; }
        }

        /// <summary>Gets this element’s tagName property.</summary>
        public string TagName {
            get { return _webElement.TagName; }
        }

        /// <summary>Cast the WebElement to a Select element</summary>
        public Select AsSelect {
            get { return (Select)this; }
        }

        /// <summary>Cast the WebElement to a Select element</summary>
        public Table AsTable {
            get { return (Table)this; }
        }

        /// <summary>Gets the text of the element.</summary>
        public string Text {
            get { return _webElement.Text; }
        }

        /// <summary>Assert the text of an element is equal to the expected</summary>
        /// <param name="expected">Expected text</param>
        public void assertText(string expected) {
            string current = _webElement.Text;
            if (!Equals(current, expected)) throw new ApplicationException("KO, assertText failed ! expected=<" + expected.ToString() + "> result=<" + current.ToString() + "> ");
        }

        /// <summary>Assert the text of an element is not equal to the expected</summary>
        /// <param name="expected">Expected text</param>
        public void assertNotText(string expected) {
            string current = _webElement.Text;
            if (Equals(current, expected)) throw new ApplicationException("KO, assertNotText failed ! not expected=<" + expected.ToString() + "> result=<" + current.ToString() + "> ");
        }

        /// <summary>Verifiy the element's text is equal to the expected and return the result</summary>
        /// <param name="text">Expected text</param>
        /// <returns>Result of the verification</returns>
        public string verifyText(string text) {
            string current = _webElement.Text;
            if (!Equals(current, text)) return "KO, verifyText failed ! expected=<" + text.ToString() + "> result=<" + current.ToString() + "> ";
            return null;
        }

        /// <summary>Verifiy the element's text is not equal to the expected and return the result</summary>
        /// <param name="text">Different text</param>
        /// <returns>Result of the verification</returns>
        public string verifyNotText(string text) {
            string current = _webElement.Text;
            if (Equals(current, text)) return "KO, verifyNotText failed ! not expected=<" + text.ToString() + "> result=<" + current.ToString() + "> ";
            return null;
        }

        /// <summary>Gets the attribute value.</summary>
        /// <param name="attributeName">Attribute name</param>
        /// <returns>Attribute</returns>
        public String getAttribute(string attributeName) {
            return _webElement.GetAttribute(attributeName);
        }

        /// <summary>Assert an attribute value</summary>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="expected">Expected attribute</param>
        public void assertAttribute(string attributeName, string expected) {
            string current = _webElement.GetAttribute(attributeName);
            if (!Equals(current, expected)) throw new ApplicationException("KO, assertAttribute failed ! expected=<" + expected.ToString() + "> result=<" + current.ToString() + "> ");
        }

        /// <summary>Return the verification result of an attribute value</summary>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="expected">Expected attribute</param>
        /// <returns>Result of the verification</returns>
        public string verifyAttribute(string attributeName, string expected) {
            string current = _webElement.GetAttribute(attributeName);
            if (!Equals(current, expected)) return "KO, verifyAttribute failed ! expected=<" + expected.ToString() + "> result=<" + current.ToString() + "> ";
            return null;
        }

        /// <summary>Returns the value of a CSS property</summary>
        /// <param name="propertyName">Property name</param>
        /// <returns>CSS value</returns>
        public String getCssValue(string propertyName) {
            return _webElement.GetCssValue(propertyName);
        }

        /// <summary>Assert a CSS property</summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="expected">Expected CSS value</param>
        public void assertCssValue(string propertyName, string expected) {
            string current = _webElement.GetCssValue(propertyName);
            if (!Equals(current, expected)) throw new ApplicationException("KO, assertAttribute failed ! expected=<" + expected.ToString() + "> result=<" + current.ToString() + "> ");
        }

        /// <summary>Return the verification result of a CSS property</summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="expected">Expected CSS value</param>
        /// <returns>Result of the verification</returns>
        public string verifyCssValue(string propertyName, string expected) {
            string current = _webElement.GetCssValue(propertyName);
            if (!Equals(current, expected)) return "KO, verifyAttribute failed ! expected=<" + expected.ToString() + "> result=<" + current.ToString() + "> ";
            return null;
        }

        /// <summary>Clears the text if it’s a text entry element.</summary>
        public WebElement clear() {
            _webElement.Clear();
            return this;
        }

        /// <summary>Clicks the element.</summary>
        public void click() {
            _webElement.Click();
        }

        /// <summary>Clicks at the element offset.</summary>
        /// <param name="offset_x">Offset X</param>
        /// <param name="offset_y">Offset Y</param>
        public void clickByOffset(int offset_x, int offset_y) {
            new OpenQA.Selenium.Interactions.Actions(_webDriver).MoveToElement(_webElement).MoveByOffset(offset_x, offset_y).Perform();
        }

        /// <summary>Simulates typing into the element.</summary>
        /// <param name="keysOrModifier">Sequence of keys or a modifier key(Control,Shift...) if the sequence is in keysToSendEx</param>
        /// <param name="keys">Optional - Sequence of keys if keysToSend contains modifier key(Control,Shift...)</param>
        /// <example>
        /// To send mobile to an element :
        /// <code lang="vbs">
        ///     driver.findElementsById("id").sendKeys "mobile"
        /// </code>
        /// To send ctrl+a to an element :
        /// <code lang="vbs">
        ///     driver.findElementsById("id").sendKeys Keys.Control, "a"
        /// </code>
        /// </example>
        public void sendKeys(string keysOrModifier, [Optional][DefaultParameterValue("")]string keys) {
            if (string.IsNullOrEmpty(keys))
                _webElement.SendKeys(keysOrModifier);
            else
                new OpenQA.Selenium.Interactions.Actions(_webDriver).KeyDown(keysOrModifier).SendKeys(keys).KeyUp(keysOrModifier).Build().Perform();
        }

        /// <summary>Submits a form.</summary>
        public void submit() {
            _webElement.Submit();
        }

        /// <summary>Clicks the element and hold.</summary>
        public void clickAndHold() {
            new OpenQA.Selenium.Interactions.Actions(_webDriver).ClickAndHold(_webElement).Perform();
        }

        /// <summary>Release a click</summary>
        public void releaseClick() {
            new OpenQA.Selenium.Interactions.Actions(_webDriver).Release(_webElement).Perform();
        }

        /// <summary>Rigth clicks the element</summary>
        public void contextClick() {
            new OpenQA.Selenium.Interactions.Actions(_webDriver).ContextClick(_webElement).Perform();
        }

        /// <summary>Double clicks the element.</summary>
        public void doubleClick() {
            new OpenQA.Selenium.Interactions.Actions(_webDriver).DoubleClick(_webElement).Perform();
        }

        /// <summary>Drag and drop the element to another element</summary>
        /// <param name="target">Target WebElement</param>
        public void dragAndDropToWebElement(WebElement target) {
            new OpenQA.Selenium.Interactions.Actions(_webDriver).DragAndDrop(_webElement, target._webElement).Perform();
        }

        /// <summary>Drag and drop the element to an offset</summary>
        /// <param name="offsetX">Offset X</param>
        /// <param name="offsetY">Offset Y</param>
        public void dragAndDropToOffset(int offsetX, int offsetY) {
            new OpenQA.Selenium.Interactions.Actions(_webDriver).DragAndDropToOffset(_webElement, offsetX, offsetY).Perform();
        }

        /// <summary>Press a key and hold</summary>
        /// <param name="theKey">Key</param>
        public void keyDown(string theKey) {
            new OpenQA.Selenium.Interactions.Actions(_webDriver).KeyDown(_webElement, theKey).Perform();
        }

        /// <summary>Release a key</summary>
        /// <param name="theKey">Key</param>
        public void keyUp(string theKey) {
            new OpenQA.Selenium.Interactions.Actions(_webDriver).KeyUp(_webElement, theKey).Perform();
        }


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate bool WebElementCallBack(ref WebElement webelement);

        /// <summary>Waits for a function to return true. VBScript: Function WaitEx(webdriver), VBA: Function WaitEx(webdriver As WebDriver) As Boolean </summary>
        /// <param name="function">Function reference.  VBScript: wd.WaitFor GetRef(\"WaitEx\")  VBA: wd.WaitFor AddressOf WaitEx)</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>Current WebDriver</returns>
        public WebElement WaitFor(object procedure, int timeoutms = 6000) {
            var we = this;
            if (procedure is int) {
                var proc = (WebElementCallBack)Marshal.GetDelegateForFunctionPointer(new IntPtr((int)procedure), typeof(WebElementCallBack));
                _wd.WaitNotNullOrTrue(() => proc.Invoke(ref we), timeoutms);
            } else {
                var type = procedure.GetType();
                _wd.WaitNotNullOrTrue(() => type.InvokeMember(string.Empty, System.Reflection.BindingFlags.InvokeMethod, null, null, new []{we}), timeoutms);
            }
            return we;
        }

        /// <summary>Waits for an attribute</summary>
        /// <param name="attribute">Attribute</param>
        /// <param name="value">Value</param>
        public WebElement waitForAttribute(string attribute, string pattern, int timeoutms = -1) {
            var regex = new Regex(pattern);
            waitFor(() => regex.IsMatch(_webElement.GetAttribute(attribute)), timeoutms);
            return this;
        }

        /// <summary>Waits for a different attribute</summary>
        /// <param name="attribute">Attribute</param>
        /// <param name="value">Value</param>
        public WebElement waitForNotAttribute(string attribute, string pattern, int timeoutms = -1) {
            var regex = new Regex(pattern);
            waitFor(() => !regex.IsMatch(_webElement.GetAttribute(attribute)), timeoutms);
            return this;
        }

        /// <summary>Waits for a CSS property</summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="value">Value</param>
        public WebElement waitForCssValue(string propertyName, string value, int timeoutms = -1) {
            waitFor(() => _webElement.GetCssValue(propertyName) == value, timeoutms);
            return this;
        }

        /// <summary>Waits for a different CSS property</summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="value">Value</param>
        public WebElement waitForNotCssValue(string propertyName, string value, int timeoutms = -1) {
            waitFor(() => _webElement.GetCssValue(propertyName) != value, timeoutms);
            return this;
        }

        /// <summary>Waits for text</summary>
        /// <param name="value">Value</param>
        public WebElement waitForText(string pattern, int timeoutms = -1) {
            var regex = new Regex(pattern);
            if (_webElement.TagName.ToLower() == "input")
                waitFor(() => regex.IsMatch(_webElement.GetAttribute("value")), timeoutms);
            else
                waitFor(() => regex.IsMatch(_webElement.Text), timeoutms);
            return this;
        }

        /// <summary>Waits for a different text</summary>
        /// <param name="value">Value</param>
        public WebElement waitForNotText(string pattern, int timeoutms = -1) {
            var regex = new Regex(pattern);
            if (_webElement.TagName.ToLower() == "input")
                waitFor(() => !regex.IsMatch(_webElement.GetAttribute("value")), timeoutms);
            else
                waitFor(() => !regex.IsMatch(_webElement.Text), timeoutms);
            return this;
        }

        public WebElement WaitForSelection(bool selected = true, int timeoutms = -1) {
            waitFor(() => _webElement.Selected == selected, timeoutms);
            return this;
        }

        public WebElement WaitForEnabled(bool enabled = true, int timeoutms = -1) {
            waitFor(() => _webElement.Enabled == enabled, timeoutms);
            return this;
        }

        public WebElement WaitForDisplayed(bool displayed = true, int timeoutms = -1) {
            waitFor(() => _webElement.Displayed == displayed, timeoutms);
            return this;
        }

        private void waitFor(Func<bool> condition, int timeout = -1) {
            DateTime end = DateTime.Now.AddMilliseconds(timeout == -1 ? this._wd._timeout : timeout);
            while (!this._wd._canceled && !condition()) {
                if (DateTime.Now > end)
                    throw new Exception("Timeout reached!");
                Thread.Sleep(15);
            }
            this._wd.CheckCanceled();
        }


        /// <summary>Gets the screenshot of the current element</summary>
        /// <returns>Image</returns>
        public Image getScreenshot() {
            /*
            OpenQA.Selenium.Screenshot ret = ((OpenQA.Selenium.ITakesScreenshot)this.webElement).GetScreenshot();
            if (ret == null) throw new ApplicationException("Method <getScreenshot> failed !\nReturned value is empty");
            return new Image(ret.AsByteArray);
            */

            System.Drawing.Rectangle cropRect = new System.Drawing.Rectangle(_webElement.Location, _webElement.Size);
            byte[] imageBytes = ((OpenQA.Selenium.ITakesScreenshot)_webDriver).GetScreenshot().AsByteArray;
            if (imageBytes == null) throw new ApplicationException("Method <captureScreenshotToPdf> failed !\nReturned value is empty");

            System.Drawing.Image srcImage;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(imageBytes))
                srcImage = System.Drawing.Image.FromStream(ms);
            using (System.Drawing.Bitmap srcBitmap = new System.Drawing.Bitmap(srcImage))
            using (System.Drawing.Bitmap bmpCrop = srcBitmap.Clone(cropRect, srcBitmap.PixelFormat))
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream()) {
                bmpCrop.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return new Image(ms.ToArray());
            }
        }

        #region Find Elements


        /// <summary>Find the first WebElement using the given method.</summary>
        /// <param name="by">Methode</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElement(By by, int timeoutms = 0) {
            if (by._by == null) throw new NullReferenceException("The locating mechanism is null!");
            return this.findElement(by._by, timeoutms);
        }

        /// <summary>Finds the first element matching the specified name.</summary>
        /// <param name="name">Name</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByName(String name, int timeoutms = 0) {
            return this.findElement(OpenQA.Selenium.By.Name(name), timeoutms);
        }

        /// <summary>Finds the first element matching the specified XPath query.</summary>
        /// <param name="xpath">XPath</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByXPath(String xpath, int timeoutms = 0) {
            return this.findElement(OpenQA.Selenium.By.XPath(xpath), timeoutms);
        }

        /// <summary>Finds the first element matching the specified id.</summary>
        /// <param name="id">Id</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>WebElement</returns>
        public WebElement findElementById(String id, int timeoutms = 0) {
            return this.findElement(OpenQA.Selenium.By.Id(id), timeoutms);
        }

        /// <summary>Finds the first element matching the specified CSS class.</summary>
        /// <param name="classname">Classname</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByClassName(String classname, int timeoutms = 0) {
            return this.findElement(OpenQA.Selenium.By.ClassName(classname), timeoutms);
        }

        /// <summary>Finds the first element matching the specified CSS selector.</summary>
        /// <param name="cssselector">CSS seletor</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByCssSelector(String cssselector, int timeoutms = 0) {
            return this.findElement(OpenQA.Selenium.By.CssSelector(cssselector), timeoutms);
        }

        /// <summary>Finds the first element matching the specified link text.</summary>
        /// <param name="linktext">Link text</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByLinkText(String linktext, int timeoutms = 0) {
            return this.findElement(OpenQA.Selenium.By.LinkText(linktext), timeoutms);
        }

        /// <summary>Finds the first of elements that match the part of the link text supplied</summary>
        /// <param name="partiallinktext">Partial link text</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByPartialLinkText(String partiallinktext, int timeoutms = 0) {
            return this.findElement(OpenQA.Selenium.By.PartialLinkText(partiallinktext), timeoutms);
        }

        /// <summary>Finds the first element matching the specified tag name.</summary>
        /// <param name="tagname">Tag name</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByTagName(String tagname, int timeoutms = 0) {
            return this.findElement(OpenQA.Selenium.By.TagName(tagname), timeoutms);
        }

        private WebElement findElement(OpenQA.Selenium.By by, int timeoutms) {
            try {
                if (timeoutms < 1)
                    return new WebElement(this._wd,_webElement.FindElement(by));
                return new WebElement(this._wd, this._wd.WaitNoException(() => _webElement.FindElement(by), timeoutms));
            } catch (Exception ex) {
                if (ex is NoSuchElementException || ex is TimeoutException)
                    throw new Exception("Element not found. " + "Method=" + by.ToString().ToLower().Substring(3).Replace(": ", ", value="));
                throw;
            }
        }

        /// <summary>Find all elements within the current context using the given mechanism.</summary>
        /// <param name="by">The locating mechanism to use</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>A list of all WebElements, or an empty list if nothing matches</returns>
        public WebElementCollection findElements(By by, int timeoutms = 0) {
            if (((By)by)._by == null) throw new NullReferenceException("The locating mechanism is null!");
            return findElements(((By)by)._by, timeoutms);
        }

        /// <summary>Finds elements matching the specified name.</summary>
        /// <param name="name">Name</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>Array of WebElement(s)</returns>
        public WebElementCollection findElementsByName(String name, int timeoutms = 0) {
            return this.findElements(OpenQA.Selenium.By.Name(name), timeoutms);
        }

        /// <summary>Finds elements matching the specified XPath query.</summary>
        /// <param name="xpath">XPath</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>Array of WebElement(s)</returns>
        public WebElementCollection findElementsByXPath(String xpath, int timeoutms = 0) {
            return this.findElements(OpenQA.Selenium.By.XPath(xpath), timeoutms);
        }

        /// <summary>Finds elements matching the specified id.</summary>
        /// <param name="id">Id</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>Array of WebElement(s)</returns>
        public WebElementCollection findElementsById(String id, int timeoutms = 0) {
            return this.findElements(OpenQA.Selenium.By.Id(id), timeoutms);
        }

        /// <summary>Finds elements matching the specified CSS class.</summary>
        /// <param name="classname">Class name</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>Array of WebElement(s)</returns>
        public WebElementCollection findElementsByClassName(String classname, int timeoutms = 0) {
            return this.findElements(OpenQA.Selenium.By.ClassName(classname), timeoutms);
        }

        /// <summary>Finds elements matching the specified CSS selector.</summary>
        /// <param name="cssselector">CSS selector</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>Array of WebElement(s)</returns>
        public WebElementCollection findElementsByCssSelector(String cssselector, int timeoutms = 0) {
            return this.findElements(OpenQA.Selenium.By.CssSelector(cssselector), timeoutms);
        }

        /// <summary>Finds elements matching the specified link text.</summary>
        /// <param name="linktext">Link text</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>Array of WebElement(s)</returns>
        public WebElementCollection findElementsByLinkText(String linktext, int timeoutms = 0) {
            return this.findElements(OpenQA.Selenium.By.LinkText(linktext), timeoutms);
        }

        /// <summary>Finds the first of elements that match the part of the link text supplied</summary>
        /// <param name="partiallinktext">Partial link text</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>Array of WebElement(s)</returns>
        public WebElementCollection findElementsByPartialLinkText(String partiallinktext, int timeoutms = 0) {
            return this.findElements(OpenQA.Selenium.By.PartialLinkText(partiallinktext), timeoutms);
        }

        /// <summary>Finds elements matching the specified tag name.</summary>
        /// <param name="tagname">Tag name</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>Array of WebElement(s)</returns>
        public WebElementCollection findElementsByTagName(String tagname, int timeoutms = 0) {
            return this.findElements(OpenQA.Selenium.By.TagName(tagname), timeoutms);
        }

        private WebElementCollection findElements(OpenQA.Selenium.By by, int timeoutms) {
            try {
                if (timeoutms < 1)
                    return new WebElementCollection(this._wd, _webElement.FindElements(by));
                return new WebElementCollection(this._wd, this._wd.WaitNotNullOrTrue(() => {
                    var elts = _webElement.FindElements(by);
                    return elts.Count == 0 ? null : elts;
                }, timeoutms));
            } catch (Exception ex) {
                if (ex is NoSuchElementException || ex is TimeoutException)
                    throw new Exception("Elements not found. " + "Method=" + by.ToString().ToLower().Substring(3).Replace(": ", ", value="));
                throw;
            }
        }

        #endregion Find Elements

        #region Select WebElement

        /// <summary>Gets all of the selected options within the select element.</summary>
        public WebElementCollection AllSelectedOptions {
            get {
                return new WebElementCollection(this._wd, new OpenQA.Selenium.Support.UI.SelectElement(_webElement).AllSelectedOptions);
            }
        }

        /// <summary></summary>
        public bool IsMultiple {
            get { return new OpenQA.Selenium.Support.UI.SelectElement(_webElement).IsMultiple; }
        }

        /// <summary>Returns a list of all options belonging to this select tag</summary>
        public WebElementCollection Options {
            get {
                return new WebElementCollection(this._wd, new OpenQA.Selenium.Support.UI.SelectElement(_webElement).Options);
            }
        }

        /// <summary>The first selected option in this select tag (or the currently selected option in a normal select)</summary>
        public IWebElement SelectedOption {
            get { return new WebElement(this._wd, new OpenQA.Selenium.Support.UI.SelectElement(_webElement).SelectedOption); }
        }

        /// <summary>Select the option at the given index. This is done by examing the “index” attribute of an element, and not merely by counting.</summary>
        /// <param name="index">Index</param>
        public void selectByIndex(int index) {
            new OpenQA.Selenium.Support.UI.SelectElement(_webElement).SelectByIndex(index);
        }

        /// <summary>Select all options that display text matching the argument.</summary>
        /// <param name="text"></param>
        public void selectByText(string text) {
            new OpenQA.Selenium.Support.UI.SelectElement(_webElement).SelectByText(text);
        }

        /// <summary>Select all options that have a value matching the argument.</summary>
        /// <param name="value">Value</param>
        public void selectByValue(string value) {
            new OpenQA.Selenium.Support.UI.SelectElement(_webElement).SelectByValue(value);
        }

        /// <summary>Clear all selected entries. This is only valid when the SELECT supports multiple selections. throws NotImplementedError If the SELECT does not support multiple selections</summary>
        public void deselectAll() {
            new OpenQA.Selenium.Support.UI.SelectElement(_webElement).DeselectAll();
        }

        /// <summary>Deselect the option at the given index. This is done by examing the “index” attribute of an element, and not merely by counting.</summary>
        /// <param name="index"></param>
        public void deselectByIndex(int index) {
            new OpenQA.Selenium.Support.UI.SelectElement(_webElement).DeselectByIndex(index);
        }

        /// <summary>Deselect all options that display text matching the argument.</summary>
        /// <param name="text"></param>
        public void deselectByText(string text) {
            new OpenQA.Selenium.Support.UI.SelectElement(_webElement).DeselectByText(text);
        }

        /// <summary>Deselect all options that have a value matching the argument. That is, when given “foo” this would deselect an option like:</summary>
        /// <param name="value"></param>
        public void deselectByValue(string value) {
            new OpenQA.Selenium.Support.UI.SelectElement(_webElement).DeselectByValue(value);
        }

        #endregion Select WebElement

        #region Regex

        /// <summary>Indicates whether the regular expression finds a match in the input string using the regular expression specified in the pattern parameter.</summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>true if the regular expression finds a match; otherwise, false.</returns>
        public bool isMatch(string pattern) {
            return Regex.IsMatch(_webElement.Text, pattern);
        }

        /// <summary>Searches the specified input string for an occurrence of the regular expression supplied in the pattern parameter.</summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>Matching strings</returns>
        public object match(string pattern) {
            Match match = Regex.Match(_webElement.Text, pattern);
            if (match.Groups != null) {
                string[] lst = new string[match.Groups.Count];
                for (int i = 0; i < match.Groups.Count; i++)
                    lst[i] = match.Groups[i].Value;
                return lst;
            } else {
                return match.Value;
            }
        }

        /// <summary>Takes the web element text, replaces all strings that match a specified regular expression with a specified replacement string and return the result.</summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="replacement">The replacement string.</param>
        /// <returns>A new string that is identical to the input string, except that a replacement string takes the place of each matched string.</returns>
        public string replace(string pattern, string replacement) {
            return Regex.Replace(_webElement.Text, pattern, replacement);
        }

        #endregion Regex

        /// <summary>Get the data from an HTML table</summary>
        /// <param name="firstRowsToSkip">First row(s) to skip. Ex : 2 will skip the first two rows</param>
        /// <param name="lastRowsToSkip">Last row(s) to skip. Ex : 2 will skip the last two rows</param>
        /// <returns>Excel array</returns>
        public object[,] getData(int firstRowsToSkip = 0, int lastRowsToSkip = 0) {
            if (_webElement.TagName.ToLower() != "table")
                throw new Exception("This element is not a table!");
            var data = (ICollection)((OpenQA.Selenium.IJavaScriptExecutor)_webDriver).ExecuteScript("var r=arguments[0].rows;var a=[r.length];for(var i=0;i<r.length;i++){var c=r[i].cells;var b=[c.length];for(var j=0;j<c.length;j++)b[j]=c[j].textContent;a[i]=b;}return a;", _webElement);
            if(data == null) return null;
            int dim1 = data.Count;
            int dim2 = 0;
            int lastRow = dim1 - lastRowsToSkip;
            int firstRow = firstRowsToSkip + 1;
            object[,] table = null;
            int r = 0, c;
            foreach (ICollection row in data) {
                r++;
                if (r < firstRow || r > lastRow)
                    continue;
                if (table == null) {
                    dim2 = row.Count;
                    table = (object[,])Array.CreateInstance(typeof(object), new[] { dim1, dim2 }, new[] { 1, 1 });
                }
                c = 0;
                foreach (string cell in row) {
                    var value = cell.Trim();
                    double number;
                    table[r, ++c] = double.TryParse(value, out number) ? (object)number : (object)value;
                }
            }
            return table;
        }

    }
}
