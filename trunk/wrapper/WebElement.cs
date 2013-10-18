using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace SeleniumWrapper
{
    /// <summary>
    /// Defines the interface through which the user controls elements on the page. 
    /// </summary>

    [Description("Defines the interface through which the user controls elements on the page.")]
    [Guid("567CB939-FD53-4E83-A8D2-A693991646BE")]
    [ComVisible(true), ComDefaultInterface(typeof(IWebElement)), ClassInterface(ClassInterfaceType.None)]
    public class WebElement : IWebElement, Select
    {
        internal OpenQA.Selenium.IWebElement _webElement;
        internal OpenQA.Selenium.IWebDriver _webDriver;
        internal WebDriver _wd;

        internal WebElement(WebDriver webDriver, OpenQA.Selenium.IWebElement webElement) {
            this._wd = webDriver;
            _webDriver = _wd._webDriver;
            _webElement = webElement;
        }

        internal static WebElement[] GetWebElements(WebDriver webDriver, ReadOnlyCollection<OpenQA.Selenium.IWebElement> webElements){
            WebElement[] elements = new WebElement[webElements.Count];
            for(int i=0;i<webElements.Count;i++)
                elements[i] = new WebElement(webDriver, webElements[i]);
            return elements;
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
        public int[] Location {
            get { return new int[] { _webElement.Location.X, _webElement.Location.Y }; }
        }

        /// <summary>Whether the element is selected.</summary>
        public bool Selected {
            get { return _webElement.Selected; }
        }

        /// <summary>Returns the size of the element</summary>
        public int[] Size {
            get { return new int[] { _webElement.Size.Width, _webElement.Size.Height }; }
        }

        /// <summary>Gets this element’s tagName property.</summary>
        public string TagName {
            get { return _webElement.TagName; }
        }

        /// <summary>Cast the WebElement to a Select element</summary>
        public Select AsSelect { 
            get { return (Select)this; } 
        }

        /// <summary>Gets the text of the element.</summary>
        public string Text { 
            get { return _webElement.Text; } 
        }

        /// <summary>Assert the text of an element is equal to the expected</summary>
        /// <param name="expected">Expected text</param>
        public void assertText(string expected){
            string current = _webElement.Text;
            if (!Equals(current, expected)) throw new ApplicationException("KO, assertText failed ! expected=<" + expected.ToString() + "> result=<" + current.ToString() + "> "); 
        }

        /// <summary>Assert the text of an element is not equal to the expected</summary>
        /// <param name="expected">Expected text</param>
        public void assertNotText(string expected){
            string current = _webElement.Text;
            if (Equals(current, expected)) throw new ApplicationException("KO, assertNotText failed ! not expected=<" + expected.ToString() + "> result=<" + current.ToString() + "> "); 
        }

        /// <summary>Verifiy the element's text is equal to the expected and return the result</summary>
        /// <param name="text">Expected text</param>
        /// <returns>Result of the verification</returns>
        public string verifyText(string text){
            string current = _webElement.Text;
            if (!Equals(current, text)) return "KO, verifyText failed ! expected=<" + text.ToString() + "> result=<" + current.ToString() + "> "; 
            return null;
        }

        /// <summary>Verifiy the element's text is not equal to the expected and return the result</summary>
        /// <param name="text">Different text</param>
        /// <returns>Result of the verification</returns>
        public string verifyNotText(string text){
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
        public void assertAttribute(string attributeName, string expected){
            string current = _webElement.GetAttribute(attributeName);
            if (!Equals(current, expected)) throw new ApplicationException("KO, assertAttribute failed ! expected=<" + expected.ToString() + "> result=<" + current.ToString() + "> "); 
        }

        /// <summary>Return the verification result of an attribute value</summary>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="expected">Expected attribute</param>
        /// <returns>Result of the verification</returns>
        public string verifyAttribute(string attributeName, string expected){
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
        public void assertCssValue(string propertyName, string expected){
            string current = _webElement.GetCssValue(propertyName);
            if (!Equals(current, expected)) throw new ApplicationException("KO, assertAttribute failed ! expected=<" + expected.ToString() + "> result=<" + current.ToString() + "> "); 
        }

        /// <summary>Return the verification result of a CSS property</summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="expected">Expected CSS value</param>
        /// <returns>Result of the verification</returns>
        public string verifyCssValue(string propertyName, string expected){
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
        public void sendKeys(string keysOrModifier, [Optional][DefaultParameterValue("")]string keys)
        {
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
        public void clickAndHold(){
            new OpenQA.Selenium.Interactions.Actions(_webDriver).ClickAndHold(_webElement).Perform();
        }

        /// <summary>Release a click</summary>
        public void releaseClick() {
            new OpenQA.Selenium.Interactions.Actions(_webDriver).Release(_webElement).Perform();
        }

        /// <summary>Rigth clicks the element</summary>
        public void contextClick(){
            new OpenQA.Selenium.Interactions.Actions(_webDriver).ContextClick(_webElement).Perform();
        }

        /// <summary>Double clicks the element.</summary>
        public void doubleClick(){
            new OpenQA.Selenium.Interactions.Actions(_webDriver).DoubleClick(_webElement).Perform();
        }

        /// <summary>Drag and drop the element to another element</summary>
        /// <param name="target">Target WebElement</param>
        public void dragAndDropToWebElement(IWebElement target){
            new OpenQA.Selenium.Interactions.Actions(_webDriver).DragAndDrop(_webElement, ((WebElement)target)._webElement).Perform();
        }

        /// <summary>Drag and drop the element to an offset</summary>
        /// <param name="offsetX">Offset X</param>
        /// <param name="offsetY">Offset Y</param>
        public void dragAndDropToOffset(int offsetX, int offsetY){
            new OpenQA.Selenium.Interactions.Actions(_webDriver).DragAndDropToOffset(_webElement, offsetX, offsetY).Perform();
        }

        /// <summary>Press a key and hold</summary>
        /// <param name="theKey">Key</param>
        public void keyDown(string theKey){
            new OpenQA.Selenium.Interactions.Actions(_webDriver).KeyDown(_webElement, theKey).Perform();
        }

        /// <summary>Release a key</summary>
        /// <param name="theKey">Key</param>
        public void keyUp(string theKey){
            new OpenQA.Selenium.Interactions.Actions(_webDriver).KeyUp(_webElement, theKey).Perform();
        }

        /// <summary>Waits for an attribute</summary>
        /// <param name="attribute">Attribute</param>
        /// <param name="value">Value</param>
        public void waitForAttribute(string attribute, string value)
        {
            waitFor(delegate(){ return _webElement.GetAttribute(attribute) == value; });
        }

        /// <summary>Waits for a different attribute</summary>
        /// <param name="attribute">Attribute</param>
        /// <param name="value">Value</param>
        public void waitForNotAttribute(string attribute, string value)
        {
            waitFor(delegate() { return _webElement.GetAttribute(attribute) != value; });
        }

        /// <summary>Waits for a CSS property</summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="value">Value</param>
        public void waitForCssValue(string propertyName, string value)
        {
            waitFor(delegate() { return _webElement.GetCssValue(propertyName) == value; });            
        }

        /// <summary>Waits for a different CSS property</summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="value">Value</param>
        public void waitForNotCssValue(string propertyName, string value)
        {
            waitFor(delegate() { return _webElement.GetCssValue(propertyName) != value; });
        }

        /// <summary>Waits for text</summary>
        /// <param name="value">Value</param>
        public void waitForText(string value)
        {
            waitFor(delegate() { return _webElement.Text == value; });
        }

        /// <summary>Waits for a different text</summary>
        /// <param name="value">Value</param>
        public void waitForNotText(string value)
        {
            waitFor(delegate() { return _webElement.Text != value; });
        }

        private void waitFor(Func<bool> condition)
        {
            DateTime end = DateTime.Now.AddMilliseconds(this._wd._timeout);
            while(!this._wd._canceled && !condition()){
                if( DateTime.Now > end )
                    throw new Exception("Timeout reached!");
                Thread.Sleep(15);
            }
            this._wd.CheckCanceled();
        }

        /// <summary>Gets the screenshot of the current element</summary>
        /// <returns>Image</returns>
        public Image getScreenshot()
        {
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
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream()){
                        bmpCrop.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        return new Image(ms.ToArray());
                    }
        }

     #region Find Elements


        /// <summary>Find the first WebElement using the given method.</summary>
        /// <param name="by">Methode</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>WebElement</returns>
        public WebElement findElement(ref object by, [Optional][DefaultParameterValue(0)]int timeoutms)
        {
            if (((By)by).base_ == null) throw new NullReferenceException("The locating mechanism is null!");
            return this.findElement(((By)by).base_, timeoutms);
        }


        /// <summary>Finds the first element matching the specified name.</summary>
        /// <param name="name">Name</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByName(String name, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElement(OpenQA.Selenium.By.Name(name), timeoutms);
        }

        /// <summary>Finds the first element matching the specified XPath query.</summary>
        /// <param name="xpath">XPath</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByXPath(String xpath, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElement(OpenQA.Selenium.By.XPath(xpath), timeoutms);
        }

        /// <summary>Finds the first element matching the specified id.</summary>
        /// <param name="id">Id</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>WebElement</returns>
        public WebElement findElementById(String id, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElement(OpenQA.Selenium.By.Id(id), timeoutms);
        }

        /// <summary>Finds the first element matching the specified CSS class.</summary>
        /// <param name="classname">Classname</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByClassName(String classname, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElement(OpenQA.Selenium.By.ClassName(classname), timeoutms);
        }

        /// <summary>Finds the first element matching the specified CSS selector.</summary>
        /// <param name="cssselector">CSS seletor</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByCssSelector(String cssselector, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElement(OpenQA.Selenium.By.CssSelector(cssselector), timeoutms);
        }

        /// <summary>Finds the first element matching the specified link text.</summary>
        /// <param name="linktext">Link text</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByLinkText(String linktext, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElement(OpenQA.Selenium.By.LinkText(linktext), timeoutms);
        }

        /// <summary>Finds the first of elements that match the part of the link text supplied</summary>
        /// <param name="partiallinktext">Partial link text</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByPartialLinkText(String partiallinktext, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElement(OpenQA.Selenium.By.PartialLinkText(partiallinktext), timeoutms);
        }

        /// <summary>Finds the first element matching the specified tag name.</summary>
        /// <param name="tagname">Tag name</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>WebElement</returns>
        public WebElement findElementByTagName(String tagname, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElement(OpenQA.Selenium.By.TagName(tagname), timeoutms);
        }
		
	    private WebElement findElement(OpenQA.Selenium.By by, int timeoutms){
            object ret;
            if(timeoutms > 0)
                ret = this._wd.WaitUntilObject(() => _webElement.FindElement(by), timeoutms);
            else
                ret = _webElement.FindElement(by);
            return new WebElement(this._wd, (OpenQA.Selenium.IWebElement)ret);
        }

        /// <summary>Find all elements within the current context using the given mechanism.</summary>
        /// <param name="by">The locating mechanism to use</param>
        /// <param name="timeoutms">Optional timeout</param>
        /// <returns>A list of all WebElements, or an empty list if nothing matches</returns>
        public WebElement[] findElements(ref object by, int timeoutms)
        {
            if (((By)by).base_ == null) throw new NullReferenceException("The locating mechanism is null!");
            return findElements(((By)by).base_, timeoutms);
        }

        /// <summary>Finds elements matching the specified name.</summary>
        /// <param name="name">Name</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>Array of WebElement(s)</returns>
        public WebElement[] findElementsByName(String name, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElements(OpenQA.Selenium.By.Name(name), timeoutms);
        }

        /// <summary>Finds elements matching the specified XPath query.</summary>
        /// <param name="xpath">XPath</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>Array of WebElement(s)</returns>
        public WebElement[] findElementsByXPath(String xpath, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElements(OpenQA.Selenium.By.XPath(xpath), timeoutms);
        }

        /// <summary>Finds elements matching the specified id.</summary>
        /// <param name="id">Id</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>Array of WebElement(s)</returns>
        public WebElement[] findElementsById(String id, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElements(OpenQA.Selenium.By.Id(id), timeoutms);
        }

        /// <summary>Finds elements matching the specified CSS class.</summary>
        /// <param name="classname">Class name</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>Array of WebElement(s)</returns>
        public WebElement[] findElementsByClassName(String classname, [Optional][DefaultParameterValue(0)]int timeoutms){
			return this.findElements(OpenQA.Selenium.By.ClassName(classname), timeoutms);
        }

        /// <summary>Finds elements matching the specified CSS selector.</summary>
        /// <param name="cssselector">CSS selector</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>Array of WebElement(s)</returns>
        public WebElement[] findElementsByCssSelector(String cssselector, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElements(OpenQA.Selenium.By.CssSelector(cssselector), timeoutms);
        }

        /// <summary>Finds elements matching the specified link text.</summary>
        /// <param name="linktext">Link text</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>Array of WebElement(s)</returns>
        public WebElement[] findElementsByLinkText(String linktext, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElements(OpenQA.Selenium.By.LinkText(linktext), timeoutms);
        }

        /// <summary>Finds the first of elements that match the part of the link text supplied</summary>
        /// <param name="partiallinktext">Partial link text</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>Array of WebElement(s)</returns>
        public WebElement[] findElementsByPartialLinkText(String partiallinktext, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElements(OpenQA.Selenium.By.PartialLinkText(partiallinktext), timeoutms);
        }

        /// <summary>Finds elements matching the specified tag name.</summary>
        /// <param name="tagname">Tag name</param>
        /// <param name="timeoutms">Optional timeout in millisecond</param>
        /// <returns>Array of WebElement(s)</returns>
        public WebElement[] findElementsByTagName(String tagname, [Optional][DefaultParameterValue(0)]int timeoutms){
            return this.findElements(OpenQA.Selenium.By.TagName(tagname), timeoutms);
        }

	    private WebElement[] findElements(OpenQA.Selenium.By by, int timeoutms){
			if(timeoutms>0){
                Object ret = this._wd.WaitUntilObject(delegate(){
                    return _webElement.FindElements(by);
                }, timeoutms);
                return WebElement.GetWebElements(this._wd, (ReadOnlyCollection<OpenQA.Selenium.IWebElement>)ret);
			}
            return WebElement.GetWebElements(this._wd, _webElement.FindElements(by));
        }

     #endregion Find Elements

     #region Select WebElement

        /// <summary>Gets all of the selected options within the select element.</summary>
        public IWebElement[] AllSelectedOptions{
            get {  
                System.Collections.Generic.IList<OpenQA.Selenium.IWebElement> elements = new OpenQA.Selenium.Support.UI.SelectElement(_webElement).AllSelectedOptions; 
                IWebElement[] ret = new IWebElement[elements.Count];
                for(int i=0; i<elements.Count; i++)
                    ret[i] = new WebElement( this._wd, elements[i]);
                return ret;
            }
        }

        /// <summary></summary>
        public bool IsMultiple{
            get { return new OpenQA.Selenium.Support.UI.SelectElement(_webElement).IsMultiple; }
        }

        /// <summary>Returns a list of all options belonging to this select tag</summary>
        public IWebElement[] Options{
            get {  
                System.Collections.Generic.IList<OpenQA.Selenium.IWebElement> elements = new OpenQA.Selenium.Support.UI.SelectElement(_webElement).Options; 
                IWebElement[] ret = new IWebElement[elements.Count];
                for(int i=0; i<elements.Count; i++)
                    ret[i] = new WebElement( this._wd, elements[i]);
                return ret;
            }
        }

        /// <summary>The first selected option in this select tag (or the currently selected option in a normal select)</summary>
        public IWebElement SelectedOption{
            get { return new WebElement( this._wd, new OpenQA.Selenium.Support.UI.SelectElement(_webElement).SelectedOption); }
        }

        /// <summary>Select the option at the given index. This is done by examing the “index” attribute of an element, and not merely by counting.</summary>
        /// <param name="index">Index</param>
        public void selectByIndex(int index){
            new OpenQA.Selenium.Support.UI.SelectElement(_webElement).SelectByIndex(index);
        }

        /// <summary>Select all options that display text matching the argument.</summary>
        /// <param name="text"></param>
        public void selectByText(string text){
            new OpenQA.Selenium.Support.UI.SelectElement(_webElement).SelectByText(text);
        }

        /// <summary>Select all options that have a value matching the argument.</summary>
        /// <param name="value">Value</param>
        public void selectByValue(string value){
            new OpenQA.Selenium.Support.UI.SelectElement(_webElement).SelectByValue(value);
        }

        /// <summary>Clear all selected entries. This is only valid when the SELECT supports multiple selections. throws NotImplementedError If the SELECT does not support multiple selections</summary>
        public void deselectAll(){
            new OpenQA.Selenium.Support.UI.SelectElement(_webElement).DeselectAll();
        }

        /// <summary>Deselect the option at the given index. This is done by examing the “index” attribute of an element, and not merely by counting.</summary>
        /// <param name="index"></param>
        public void deselectByIndex(int index){
            new OpenQA.Selenium.Support.UI.SelectElement(_webElement).DeselectByIndex(index);
        }

        /// <summary>Deselect all options that display text matching the argument.</summary>
        /// <param name="text"></param>
        public void deselectByText(string text){
            new OpenQA.Selenium.Support.UI.SelectElement(_webElement).DeselectByText(text);
        }

        /// <summary>Deselect all options that have a value matching the argument. That is, when given “foo” this would deselect an option like:</summary>
        /// <param name="value"></param>
        public void deselectByValue(string value){
            new OpenQA.Selenium.Support.UI.SelectElement(_webElement).DeselectByValue(value);
        }

     #endregion Select WebElement

     #region Regex

        /// <summary>Indicates whether the regular expression finds a match in the input string using the regular expression specified in the pattern parameter.</summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>true if the regular expression finds a match; otherwise, false.</returns>
        public bool isMatch(string pattern){
            return Regex.IsMatch(_webElement.Text, pattern);
        }

        /// <summary>Searches the specified input string for an occurrence of the regular expression supplied in the pattern parameter.</summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>Matching strings</returns>
        public object match(string pattern){
            Match match = Regex.Match(_webElement.Text, pattern);
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
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="replacement">The replacement string.</param>
        /// <returns>A new string that is identical to the input string, except that a replacement string takes the place of each matched string.</returns>
        public string replace(string pattern, string replacement ){
            return Regex.Replace(_webElement.Text, pattern, replacement);
        }

     #endregion Regex

     #region Array data

        /// <summary>Return an array filled with datas from a Css query.</summary>
        /// <param name="rowsCssSelector">Css selector to locate rows</param>
        /// <param name="columnsCssSelector">Css selector to locate columns</param>
        /// <returns>Return a 2 dimention array</returns>
        /// <example>
        /// 
        /// This example copy an Html table to the current sheet.
        /// <code lang="vbs">
        ///     ret = selenium.findElementById("mytable").getArrayByCssSelector("tr", "th,td")
        ///     ActiveCell.Resize(UBound(ret) + 1, UBound(ret, 2) + 1) = ret
        /// </code>
        /// </example>
        public string[,] getArrayByCssSelector([Optional][DefaultParameterValue(null)]String rowsCssSelector, [Optional][DefaultParameterValue(null)]String columnsCssSelector){
            if(columnsCssSelector==null)
                return getArrayBy(_webElement, OpenQA.Selenium.By.CssSelector(rowsCssSelector));
            else
                return getArrayBy(_webElement, OpenQA.Selenium.By.CssSelector(rowsCssSelector), OpenQA.Selenium.By.CssSelector(columnsCssSelector));
        }

        /// <summary>Return an array filled with datas from an XPath query.</summary>
        /// <param name="rowXPath">XPath selector to locate rows</param>
        /// <param name="columnXPath">XPath selector to locate columns</param>
        /// <returns>Return a 2 dimention array</returns>
        /// <example>
        /// 
        /// This example copy an Html table to the current sheet.
        /// <code lang="vbs">
        ///     ret = selenium.findElementById("mytable").getArrayByXPath("//tr", "//td")
        ///     ActiveCell.Resize(UBound(ret) + 1, UBound(ret, 2) + 1) = ret
        /// </code>
        /// </example>
        public string[,] getArrayByXPath([Optional][DefaultParameterValue(null)]String rowXPath, [Optional][DefaultParameterValue(null)]String columnXPath){
            if(columnXPath==null)
                return getArrayBy(_webElement, OpenQA.Selenium.By.XPath(rowXPath));
            else
                return getArrayBy(_webElement, OpenQA.Selenium.By.XPath(rowXPath), OpenQA.Selenium.By.XPath(columnXPath));
        }

        private static string[,] getArrayBy(OpenQA.Selenium.IWebElement webElement, OpenQA.Selenium.By rowsBy){
            ReadOnlyCollection<OpenQA.Selenium.IWebElement> rows = webElement.FindElements(rowsBy);
            int nbRow = rows.Count;
            string[,] ret = new string[nbRow, 1];
            for(int r=0;r<nbRow;r++) 
                ret[r, 0] = rows[r].Text;
            return ret;
        }

        private static string[,] getArrayBy(OpenQA.Selenium.IWebElement webElement, OpenQA.Selenium.By rowsBy, OpenQA.Selenium.By columnsBy){
            string[,] ret = null;
            ReadOnlyCollection<OpenQA.Selenium.IWebElement> rows = webElement.FindElements(rowsBy);
            int nbRow = rows.Count;
            bool init=false;
            for(int r=0, nbCol=0;r<nbRow;r++) {
                ReadOnlyCollection<OpenQA.Selenium.IWebElement> cols = rows[r].FindElements(columnsBy);
                nbCol = cols.Count;
                if(!init) {
                    init= true;
                    ret = new string[nbRow, nbCol];
                }
                for(int c=0;c<nbCol;c++) 
                    ret[r, c] = cols[c].Text;
            }
            return ret;
        }

     #endregion Array data

    }
}
