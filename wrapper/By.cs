using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SeleniumWrapper
{
    [Guid("D1AF1FB8-A183-464A-95FC-A3DE5685A27E")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IBy{

        [Description("")]
        By ClassName(string classNameToFind);

        [Description("")]
        By CssSelector(string cssSelectorToFind);

        [Description("")]
        By Id(string idToFind);

        [Description("")]
        By LinkText(string linkTextToFind);

        [Description("")]
        By Name(string nameToFind);

        [Description("")]
        By PartialLinkText(string partialLinkTextToFind);

        [Description("")]
        By TagName(string tagNameToFind);

        [Description("")]
        By XPath(string xpathToFind);

    }


    /// <summary>
    /// Provides a mechanism by which to find elements within a document.
    /// </summary>

    [Description("Provides a mechanism by which to find elements within a document.")]
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

        /// <summary></summary>
        /// <param name="classNameToFind"></param>
        /// <returns></returns>
        public By ClassName(string classNameToFind){
            return new By(OpenQA.Selenium.By.ClassName(classNameToFind));
        }

        /// <summary></summary>
        /// <param name="cssSelectorToFind"></param>
        /// <returns></returns>
        public By CssSelector(string cssSelectorToFind){
            return new By(OpenQA.Selenium.By.CssSelector(cssSelectorToFind));
        }

        /// <summary></summary>
        /// <param name="idToFind"></param>
        /// <returns></returns>
        public By Id(string idToFind){
            return new By(OpenQA.Selenium.By.Id(idToFind));
        }

        /// <summary></summary>
        /// <param name="linkTextToFind"></param>
        /// <returns></returns>
        public By LinkText(string linkTextToFind){
            return new By(OpenQA.Selenium.By.LinkText(linkTextToFind));
        }

        /// <summary></summary>
        /// <param name="nameToFind"></param>
        /// <returns></returns>
        public By Name(string nameToFind){
            return new By(OpenQA.Selenium.By.Name(nameToFind));
        }

        /// <summary></summary>
        /// <param name="partialLinkTextToFind"></param>
        /// <returns></returns>
        public By PartialLinkText(string partialLinkTextToFind){
            return new By(OpenQA.Selenium.By.PartialLinkText(partialLinkTextToFind));
        }

        /// <summary></summary>
        /// <param name="tagNameToFind"></param>
        /// <returns></returns>
        public By TagName(string tagNameToFind){
            return new By(OpenQA.Selenium.By.TagName(tagNameToFind));
        }

        /// <summary></summary>
        /// <param name="xpathToFind"></param>
        /// <returns></returns>
        public By XPath(string xpathToFind){
            return new By(OpenQA.Selenium.By.XPath(xpathToFind));
        }

    }
}
