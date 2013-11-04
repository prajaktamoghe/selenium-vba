using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SeleniumWrapper {

    [Guid("DE94EB33-C367-4870-8FBC-7C9763568492")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface WebElementCollectionCOM{

        IEnumerator GetEnumerator();

        int Count { get; }

        new WebElement this[int index] { get; }

    }

    [Description("WebElementCollection")]
    [Guid("236BC66E-35F0-41B5-80D3-460D25F4D2B3")]
    [ComVisible(true), ComDefaultInterface(typeof(WebElementCollectionCOM)), ClassInterface(ClassInterfaceType.None)]
    public class WebElementCollection : ArrayList, WebElementCollectionCOM {

        public WebElementCollection(int capacity) : base(capacity) { }

        public WebElementCollection(WebDriver webDriver, ReadOnlyCollection<OpenQA.Selenium.IWebElement> webElements)
            : base(webElements.Count) {
            foreach (var ele in webElements)
                base.Add(new WebElement(webDriver, ele));
        }

        public new WebElement this[int index] {
            get { return (WebElement)base[index]; }
        }

        public void Add(WebElement webelement) {
            base.Add(webelement);
        }

        public void Add( WebDriver webDriver, ReadOnlyCollection<OpenQA.Selenium.IWebElement> webElements) {
            var elements = base.Capacity += webElements.Count;
            foreach (var ele in webElements)
                base.Add(new WebElement(webDriver, ele));
        }

    }

}
