using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace SeleniumWrapper
{
    [Guid("DB0ACE39-3B49-49CA-9CDF-8B145197B76C")]
    [ComVisible(true)]
    public interface IWebElement{

        [Description("Gets all of the selected options within the select element.")]
        IWebElement[] allSelectedOptions { get; }
        [Description("")]
        void clear();
        [Description("")]
        void click();
        [Description("")]
        void clickAndHold();
        [Description("")]
        void contextClick();
        [Description("")]
        void deselectAll();
        [Description("")]
        void deselectByIndex(int index);
        [Description("")]
        void deselectByText(string text);
        [Description("")]
        void deselectByValue(string value);
        [Description("")]
        bool displayed { get; }
        [Description("")]
        void doubleClick();
        [Description("")]
        void dragAndDropToOffset(int offsetX, int offsetY);
        [Description("")]
        void dragAndDropToWebElement(IWebElement target);
        [Description("")]
        bool enabled { get; }
        [Description("")]
        string getAttribute(string attributeName);
        [Description("")]
        void assertAttribute(string attributeName, string expected);
        [Description("")]
        string verifyAttribute(string attributeName, string expected);
        [Description("")]
        string getCssValue(string propertyName);
        [Description("")]
        string verifyCssValue(string propertyName, string expected);
        [Description("")]
        void assertCssValue(string propertyName, string expected);
        [Description("")]
        bool isMultiple { get; }
        [Description("")]
        void keyDown(string theKey);
        [Description("")]
        void keyUp(string theKey);
        [Description("")]
        int[] location { get; }
        [Description("")]
        void moveToElement();
        [Description("")]
        void moveToElementOffset(IWebElement toElement, int offsetX, int offsetY);
        [Description("")]
        IWebElement[] options { get; }
        [Description("")]
        void selectByIndex(int index);
        [Description("")]
        void selectByText(string text);
        [Description("")]
        void selectByValue(string value);
        [Description("")]
        bool selected { get; }
        [Description("")]
        IWebElement selectedOption { get; }
        [Description("")]
        void sendKeys(string text);
        [Description("")]
        int[] size { get; }
        [Description("")]
        void submit();
        [Description("")]
        string tagName { get; }
        [Description("")]
        string text { get; }
        [Description("")]
        void assertText(string expected);
        [Description("")]
        void assertNotText(string expected);
        [Description("")]
        string verifyNotText(string expected);
        [Description("")]
        string verifyText(string expected);
        [Description("Indicates whether the regular expression finds a match in the input string")]
        bool isMatch(string pattern);
        [Description("Searches the input string for an occurrence of a regular expression with a specified input string")]
        object match(string pattern);
        [Description("Within a specified input string, replaces all strings that match a regular expression pattern with a specified replacement string.")]
        string replace(string pattern, string replacement);
        [Description("Return an array filled with datas from a Css query. Ex: ret = selenium.findElementById(\"mytable\").getArrayByCssSelector(\"tr\", \"th,td\") ")]
        string[,] getArrayByCssSelector([Optional][DefaultParameterValue(null)]string rowsCssSelector, [Optional][DefaultParameterValue(null)]string columnsCssSelector);
        [Description("Return an array filled with datas from an XPath query. Ex: ret = selenium.findElementById(\"mytable\").getArrayByXPath(\"//tr\", \"//td\") ")]
        string[,] getArrayByXPath([Optional][DefaultParameterValue(null)]string rowXPath, [Optional][DefaultParameterValue(null)]string columnXPath);

    }

    [Guid("567CB939-FD53-4E83-A8D2-A693991646BE")]
    [ComVisible(true), ComDefaultInterface(typeof(IWebElement)), ClassInterface(ClassInterfaceType.None)]
    public class WebElement : IWebElement
    {
        internal OpenQA.Selenium.IWebElement we;
        internal OpenQA.Selenium.IWebDriver wd;

        internal WebElement(OpenQA.Selenium.IWebDriver webDriver, OpenQA.Selenium.IWebElement webElement) {
            this.we = webElement;
            this.wd = webDriver;
        }

        public bool displayed { get { return this.we.Displayed; } }
        public bool enabled { get { return this.we.Enabled; } }
        public int[] location { get { return new int[] { this.we.Location.X, this.we.Location.Y }; } }
        public bool selected { get { return this.we.Selected; } }
        public int[] size { get { return new int[] { this.we.Size.Width, this.we.Size.Height }; } }
        public string tagName { get { return this.we.TagName; } }

        public string text { 
            get { return this.we.Text; } 
        }

        public void assertText(string expected){
            string current = this.we.Text;
            if (!Equals(current, expected)) throw new ApplicationException("KO, assertText failed ! expected=<" + expected.ToString() + "> result=<" + current.ToString() + "> "); 
        }

        public void assertNotText(string expected){
            string current = this.we.Text;
            if (Equals(current, expected)) throw new ApplicationException("KO, assertNotText failed ! not expected=<" + expected.ToString() + "> result=<" + current.ToString() + "> "); 
        }

        public string verifyText(string expected){
            string current = this.we.Text;
            if (!Equals(current, expected)) return "KO, verifyText failed ! expected=<" + expected.ToString() + "> result=<" + current.ToString() + "> "; 
            return null;
        }

        public string verifyNotText(string expected){
            string current = this.we.Text;
            if (Equals(current, expected)) return "KO, verifyNotText failed ! not expected=<" + expected.ToString() + "> result=<" + current.ToString() + "> "; 
            return null;
        }

        public String getAttribute(string attributeName) { 
            return this.we.GetAttribute(attributeName); 
        }

        public void assertAttribute(string attributeName, string expected){
            string current = this.we.GetAttribute(attributeName);
            if (!Equals(current, expected)) throw new ApplicationException("KO, assertAttribute failed ! expected=<" + expected.ToString() + "> result=<" + current.ToString() + "> "); 
        }

        public string verifyAttribute(string attributeName, string expected){
            string current = this.we.GetAttribute(attributeName);
            if (!Equals(current, expected)) return "KO, verifyAttribute failed ! expected=<" + expected.ToString() + "> result=<" + current.ToString() + "> "; 
            return null;
        }

        public String getCssValue(string propertyName) { 
            return this.we.GetCssValue(propertyName);
        }

        public void assertCssValue(string propertyName, string expected){
            string current = this.we.GetCssValue(propertyName);
            if (!Equals(current, expected)) throw new ApplicationException("KO, assertAttribute failed ! expected=<" + expected.ToString() + "> result=<" + current.ToString() + "> "); 
        }

        public string verifyCssValue(string propertyName, string expected){
            string current = this.we.GetCssValue(propertyName);
            if (!Equals(current, expected)) return "KO, verifyAttribute failed ! expected=<" + expected.ToString() + "> result=<" + current.ToString() + "> "; 
            return null;
        }

        public void clear() { 
            this.we.Clear();
        }

        public void click() { 
            this.we.Click(); 
        }

        public void sendKeys(string text) { 
            this.we.SendKeys(text); 
        }

        public void submit() { 
            this.we.Submit();
        }

        public void clickAndHold(){
            new OpenQA.Selenium.Interactions.Actions(this.wd).ClickAndHold(this.we).Perform();
        }

        public void contextClick(){
            new OpenQA.Selenium.Interactions.Actions(this.wd).ContextClick(this.we).Perform();
        }

        public void doubleClick(){
            new OpenQA.Selenium.Interactions.Actions(this.wd).DoubleClick(this.we).Perform();
        }

        public void dragAndDropToWebElement(IWebElement target){
            new OpenQA.Selenium.Interactions.Actions(this.wd).DragAndDrop(this.we, ((WebElement)target).we).Perform();
        }

        public void dragAndDropToOffset(int offsetX, int offsetY){
            new OpenQA.Selenium.Interactions.Actions(this.wd).DragAndDropToOffset(this.we, offsetX, offsetY).Perform();
        }

        public void keyDown(string theKey){
            new OpenQA.Selenium.Interactions.Actions(this.wd).KeyDown(this.we, theKey).Perform();
        }

        public void keyUp(string theKey){
            new OpenQA.Selenium.Interactions.Actions(this.wd).KeyUp(this.we, theKey).Perform();
        }

        public void moveToElement(){
            new OpenQA.Selenium.Interactions.Actions(this.wd).MoveToElement(this.we).Perform();
        }

        public void moveToElementOffset(IWebElement toElement, int offsetX, int offsetY){
            new OpenQA.Selenium.Interactions.Actions(this.wd).MoveToElement(this.we, offsetX, offsetY).Perform();
        }

        public IWebElement[] allSelectedOptions{
            get {  
                System.Collections.Generic.IList<OpenQA.Selenium.IWebElement> elements = new OpenQA.Selenium.Support.UI.SelectElement(this.we).AllSelectedOptions; 
                IWebElement[] ret = new IWebElement[elements.Count];
                for(int i=0; i<elements.Count; i++){
                    ret[i] = new WebElement( this.wd, elements[i]);
                }
                return ret;
            }
        }

        public bool isMultiple{
            get { return new OpenQA.Selenium.Support.UI.SelectElement(this.we).IsMultiple; }
        }

        public IWebElement[] options{
            get {  
                System.Collections.Generic.IList<OpenQA.Selenium.IWebElement> elements = new OpenQA.Selenium.Support.UI.SelectElement(this.we).Options; 
                IWebElement[] ret = new IWebElement[elements.Count];
                for(int i=0; i<elements.Count; i++){
                    ret[i] = new WebElement( this.wd, elements[i]);
                }
                return ret;
            }
        }

        public IWebElement selectedOption{
            get { return new WebElement( this.wd, new OpenQA.Selenium.Support.UI.SelectElement(this.we).SelectedOption); }
        }

        public void deselectAll(){
            new OpenQA.Selenium.Support.UI.SelectElement(this.we).DeselectAll();
        }

        public void deselectByIndex(int index){
            new OpenQA.Selenium.Support.UI.SelectElement(this.we).DeselectByIndex(index);
        }

        public void deselectByText(string text){
            new OpenQA.Selenium.Support.UI.SelectElement(this.we).DeselectByText(text);
        }

        public void deselectByValue(string value){
            new OpenQA.Selenium.Support.UI.SelectElement(this.we).DeselectByValue(value);
        }

        public void selectByIndex(int index){
            new OpenQA.Selenium.Support.UI.SelectElement(this.we).SelectByIndex(index);
        }

        public void selectByText(string text){
            new OpenQA.Selenium.Support.UI.SelectElement(this.we).SelectByText(text);
        }

        public void selectByValue(string value){
            new OpenQA.Selenium.Support.UI.SelectElement(this.we).SelectByValue(value);
        }


        #region Regex

        /// <summary>Indicates whether the regular expression finds a match in the input string using the regular expression specified in the pattern parameter.</summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>true if the regular expression finds a match; otherwise, false.</returns>
        public bool isMatch(string pattern){
            return Regex.IsMatch(this.we.Text, pattern);
        }

        /// <summary>Searches the specified input string for an occurrence of the regular expression supplied in the pattern parameter.</summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>Matching strings</returns>
        public object match(string pattern){
            Match match = Regex.Match(this.we.Text, pattern);
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
            return Regex.Replace(this.we.Text, pattern, replacement);
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
            if(columnsCssSelector==null){
                return getArrayBy(this.we, By.CssSelector(rowsCssSelector));
            }else{
                return getArrayBy(this.we, By.CssSelector(rowsCssSelector), By.CssSelector(columnsCssSelector));
            }
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
            if(columnXPath==null){
                return getArrayBy(this.we, By.XPath(rowXPath));
            }else{
                return getArrayBy(this.we, By.XPath(rowXPath), By.XPath(columnXPath));
            }
        }

        private static string[,] getArrayBy(OpenQA.Selenium.IWebElement webElement, By rowsBy){
            ReadOnlyCollection<OpenQA.Selenium.IWebElement> rows = webElement.FindElements(rowsBy);
            int nbRow = rows.Count;
            string[,] ret = new string[nbRow, 1];
            for(int r=0;r<nbRow;r++) {
                ret[r, 0] = rows[r].Text;
            }
            return ret;
        }

        private static string[,] getArrayBy(OpenQA.Selenium.IWebElement webElement, By rowsBy, By columnsBy){
            string[,] ret = null;
            ReadOnlyCollection<OpenQA.Selenium.IWebElement> rows = webElement.FindElements(rowsBy);
            int nbRow = rows.Count;
            bool init=false;
            for(int r=0, nbCol=0;r<nbRow;r++) {
                ReadOnlyCollection<OpenQA.Selenium.IWebElement> cols = rows[r].FindElements(columnsBy);
                if(!init) {
                    init= true;
                    nbCol = cols.Count;
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
