using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SeleniumWrapper
{
    [Guid("D1AF1FB8-A183-464A-95FC-A3DE5685A27E")]
    [ComVisible(true)]
    public interface IBy{

        By ClassName(string classNameToFind);

        By CssSelector(string cssSelectorToFind);

        By Id(string idToFind);

        By LinkText(string linkTextToFind);

        By Name(string nameToFind);

        By PartialLinkText(string partialLinkTextToFind);

        By TagName(string tagNameToFind);

        By XPath(string xpathToFind);

    }

    [Description("Screenshot of a web page")]
    [Guid("038A7E6F-8F7F-40E5-B6C4-80B2B91F0D44")]
    [ComVisible(true), ComDefaultInterface(typeof(IBy)), ClassInterface(ClassInterfaceType.None)]
    public class By : IBy
    {
        internal OpenQA.Selenium.By base_;

        public By()
        {
        }

        private By(OpenQA.Selenium.By by)
        {
            this.base_ = by;
        }

        public By ClassName(string classNameToFind){
            return new By(OpenQA.Selenium.By.ClassName(classNameToFind));
        }

        public By CssSelector(string cssSelectorToFind){
            return new By(OpenQA.Selenium.By.CssSelector(cssSelectorToFind));
        }

        public By Id(string idToFind){
            return new By(OpenQA.Selenium.By.Id(idToFind));
        }

        public By LinkText(string linkTextToFind){
            return new By(OpenQA.Selenium.By.LinkText(linkTextToFind));
        }

        public By Name(string nameToFind){
            return new By(OpenQA.Selenium.By.Name(nameToFind));
        }

        public By PartialLinkText(string partialLinkTextToFind){
            return new By(OpenQA.Selenium.By.PartialLinkText(partialLinkTextToFind));
        }

        public By TagName(string tagNameToFind){
            return new By(OpenQA.Selenium.By.TagName(tagNameToFind));
        }

        public By XPath(string xpathToFind){
            return new By(OpenQA.Selenium.By.XPath(xpathToFind));
        }

    }
}
