using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace SeleniumWrapper {

    [Guid("1F96D1DD-EF65-4D27-A7FF-0EA52ACB97D1")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IAssert {
        [Description("")]
        void True(bool value, string failmessage = null);

        [Description("")]
        void False(bool value, string failmessage = null);

        [Description("Raise an error if the assertion fails")]
        void Equals(Object expected, Object current, string failmessage = null);

        [Description("Raise an error if the assertion fails")]
        void NotEquals(Object expected, Object current, string failmessage = null);

        [Description("")]
        void Matches(string text, string pattern, string failmessage = null);

        [Description("")]
        void NotMatches(string text, string pattern, string failmessage = null);

        [Description("")]
        void Contains(string text, string value, string failmessage = null);

        [Description("")]
        void Fail(string message = null);
    }

    /// <summary>Testing functions. Throws an exception if the condition is not met</summary>
    /// <example>
    /// 
    /// The following example asserts the page title.
    /// <code lang="vbs">	
    /// Set driver = CreateObject("SeleniumWrapper.WebDriver")
    /// Set Assert = CreateObject("SeleniumWrapper.Assert")
    /// driver.start "firefox", "http://www.google.com"
    /// driver.open "/"
    /// Assert.Equals "Google", driver.Title
    /// driver.stop
    /// </code>
    /// 
    /// <code lang="vbs">	
    /// Public Sub TestCase()
    ///   Dim driver As New SeleniumWrapper.WebDriver, Assert as New SeleniumWrapper.Assert
    ///   driver.start "firefox", "http://www.google.com"
    ///   driver.open "/"
    ///   Assert.Equals "Google", driver.Title
    ///   driver.stop
    /// End Sub
    /// </code>
    /// 
    /// </example>
    ///

    [Description("Testing functions. Throws an exception if the condition is not met")]
    [Guid("98E110D3-BD3D-4620-AFDA-6AAF7EDD33D6")]
    [ComVisible(true), ComDefaultInterface(typeof(IAssert)), ClassInterface(ClassInterfaceType.None)]
    public class Assert : IAssert {
        /// <summary></summary>
        /// <param name="value"></param>
        /// <param name="failmessage"></param>
        public void True(bool value, string failmessage = null) {
            if (value != true)
                //throw new COMException("Assert.True failed!" + "\n" + failmessage, 20000);
                throw new ApplicationException("Assert.True failed!" + "\n" + failmessage);
        }

        /// <summary></summary>
        /// <param name="value"></param>
        /// <param name="failmessage"></param>
        public void False(bool value, string failmessage = null) {
            if (value != false)
                throw new ApplicationException("Assert.False failed!" + "\n" + failmessage);
        }

        /// <summary>Test that two objects are equal and raise an exception if the result is false</summary>
        /// <param name="expected">expected object. Can be a string, number, array...</param>
        /// <param name="current">current object. Can be a string, number, array...</param>
        /// <param name="failmessage"></param>
        public void Equals(Object expected, Object current, string failmessage = null) {
            if (!Utils.ObjectEquals(expected, current))
                throw new ApplicationException("Assert.Equals failed!\n" + (String.IsNullOrEmpty(failmessage) ? "exp=<" + Utils.Truncate(expected.ToString()) + "> got=<" + Utils.Truncate(current.ToString()) + ">" : failmessage));
        }

        /// <summary>Test that two objects are not equal and raise an exception if the result is false</summary>
        /// <param name="expected">expected object. Can be a string, number, array...</param>
        /// <param name="current">current object. Can be a string, number, array...</param>
        /// <param name="failmessage"></param>
        public void NotEquals(Object expected, Object current, string failmessage = null) {
            if (Utils.ObjectEquals(expected, current))
                throw new ApplicationException("Assert.NotEquals failed!\n" + (String.IsNullOrEmpty(failmessage) ? "exp=<" + Utils.Truncate(expected.ToString()) + "> got=<" + Utils.Truncate(current.ToString()) + ">" : failmessage));
        }

        /// <summary></summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <param name="failmessage"></param>
        public void Matches(string input, string pattern, string failmessage = null) {
            if (!Regex.IsMatch(input, pattern))
                throw new ApplicationException("Assert.Matches failed!\n" + (String.IsNullOrEmpty(failmessage) ? "txt=<" + Utils.Truncate(input) + "> pat=<" + pattern + ">" : failmessage));
        }

        /// <summary></summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <param name="failmessage"></param>
        public void NotMatches(string input, string pattern, string failmessage = null) {
            if (Regex.IsMatch(input, pattern))
                throw new ApplicationException("Assert.NotMatches failed!\n" + (String.IsNullOrEmpty(failmessage) ? "txt=<" + Utils.Truncate(input) + "> pat=<" + pattern + ">" : failmessage));
        }

        /// <summary></summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <param name="failmessage"></param>
        public void Contains(string text, string value, string failmessage = null) {
            if (!text.Contains(value))
                throw new ApplicationException("Assert.Contains failed!\n" + (String.IsNullOrEmpty(failmessage) ? "val=<" + Utils.Truncate(text) + "> str=<" + value + ">" : failmessage));
        }

        /// <summary></summary>
        /// <param name="message"></param>
        public void Fail(string message = null) {
            throw new ApplicationException(message);
        }

    }
}
