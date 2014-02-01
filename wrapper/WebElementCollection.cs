using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;

namespace SeleniumWrapper {

    [Guid("DE94EB33-C367-4870-8FBC-7C9763568492")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IWebElementCollection {

        IEnumerator GetEnumerator();

        [Description("Number of web elements")]
        int Count { get; }

        [Description("Get the WebElement at the provided index (Zero-based)")]
        WebElement this[int index] { get; }

        [Description("Return and array containing the text for each web element")]
        object[,] getData(int firstRowsToSkip = 0, int lastRowsToSkip = 0);
    }

    [Description("WebElementCollection")]
    [Guid("236BC66E-35F0-41B5-80D3-460D25F4D2B3")]
    [ComVisible(true), ComDefaultInterface(typeof(IWebElementCollection)), ClassInterface(ClassInterfaceType.None)]
    public class WebElementCollection : ArrayList, IWebElementCollection {

        private readonly WebDriver _webDriver;

        public WebElementCollection(int capacity) : base(capacity) { }

        public WebElementCollection(WebDriver webDriver, IList<OpenQA.Selenium.IWebElement> webElements)
            : base(webElements.Count) {
            _webDriver = webDriver;
            foreach (var ele in webElements)
                base.Add(new WebElement(webDriver, ele));
        }

        /// <summary>Get the WebElement at the provided index</summary>
        /// <param name="index">Zero based index</param>
        /// <returns>WebElement</returns>
        public new WebElement this[int index] {
            get { return (WebElement)base[index]; }
        }

        public void Add(WebElement webelement) {
            base.Add(webelement);
        }

        public void Add(WebDriver webDriver, IList<OpenQA.Selenium.IWebElement> webElements) {
            var elements = base.Capacity += webElements.Count;
            foreach (var ele in webElements)
                base.Add(new WebElement(webDriver, ele));
        }


        public object[,] getData(int firstRowsToSkip = 0, int lastRowsToSkip = 0) {
            int size = this.Count;
            int lastRow = size - lastRowsToSkip;
            int firstRow = firstRowsToSkip + 1;
            object[,] table = (object[,])Array.CreateInstance(typeof(object), new[] { size - firstRowsToSkip - lastRowsToSkip, 1 }, new[] { 1, 1 }); ;
            int r = 1;
            foreach (WebElement item in this) {
                if (r < firstRow || r > lastRow) continue;
                var value =  item._webElement.Text;
                double number;
                table[r++, 1] = double.TryParse(value, out number) ? (object)number : (object)value;
            }
            return table;
        }

    }

}
