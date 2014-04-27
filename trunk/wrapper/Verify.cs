using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace SeleniumWrapper {
    [Guid("AEBDE123-87BA-4BB6-A8E7-495CC9DBFB96")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IVerify {
        [Description("")]
        string True(bool value, string failmessage = null);

        [Description("")]
        string False(bool value, string failmessage = null);

        [Description("Returns a string with the result of the verification ( <OK> or <KO, ...> )")]
        string Equals(Object expected, Object current, string failmessage = null);

        [Description("Returns a string with the result of the verification ( <OK> or <KO, ...> )")]
        string NotEquals(Object expected, Object current, string failmessage = null);

        [Description("")]
        string Matches(string text, string pattern, string failmessage = null);

        [Description("")]
        string Contains(string text, string value, string failmessage = null);

        [Description("")]
        string NotMatches(string text, string pattern, string failmessage = null);
    }

    /// <summary>Testing functions. Return the résult of the verification</summary>
    /// <example>
    /// 
    /// The following example asserts the page title.
    /// <code lang="vbs">	
    /// Set driver = CreateObject("SeleniumWrapper.WebDriver")
    /// Set Verify = CreateObject("SeleniumWrapper.Verify")
    /// driver.start "firefox", "http://www.google.com"
    /// driver.open "/"
    /// wscript.echo Verify.Equals("Google", driver.Title)
    /// driver.stop
    /// </code>
    /// 
    /// <code lang="vbs">	
    /// Public Sub TestCase()
    ///   Dim driver As New SeleniumWrapper.WebDriver, Verify as New SeleniumWrapper.Assert
    ///   driver.start "firefox", "http://www.google.com"
    ///   driver.open "/"
    ///   Range("A1") = Verify.Equals("Google", driver.Title)
    ///   driver.stop
    /// End Sub
    /// </code>
    /// 
    /// </example>
    ///

    [Description("Testing functions. Return the résult of the verification")]
    [Guid("1EA5B911-25A8-493D-B882-B0C8C528C673")]
    [ComVisible(true), ComDefaultInterface(typeof(IVerify)), ClassInterface(ClassInterfaceType.None)]
    public class Verify : IVerify {
        /// <summary></summary>
        /// <param name="value"></param>
        /// <param name="failmessage"></param>
        /// <returns></returns>
        public string True(bool value, string failmessage = null) {
            if (value == true)
                return "OK";
            return String.IsNullOrEmpty(failmessage) ? "KO, Verify.True failed!" : failmessage;
        }

        /// <summary></summary>
        /// <param name="value"></param>
        /// <param name="failmessage"></param>
        /// <returns></returns>
        public string False(bool value, string failmessage = null) {
            if (value == false)
                return "OK";
            return String.IsNullOrEmpty(failmessage) ? "KO, Verify.False failed!" : failmessage;
        }

        /// <summary>Test that two objects are equal and raise an exception if the result is false</summary>
        /// <param name="expected">expected object. Can be a string, number, array...</param>
        /// <param name="current">current object. Can be a string, number, array...</param>
        /// <param name="failmessage">Message to return if the verification fails...</param>
        public string Equals(Object expected, Object current, string failmessage = null) {
            if (Utils.ObjectEquals(expected, current))
                return "OK";
            return String.IsNullOrEmpty(failmessage) ? "KO, Verify.Equals failed! exp=<" + Utils.Truncate(expected.ToString()) + "> got=<" + Utils.Truncate(current.ToString()) + ">" : failmessage;
        }

        /// <summary>Test that two objects are not equal and raise an exception if the result is false</summary>
        /// <param name="expected">expected object. Can be a string, number, array...</param>
        /// <param name="current">current object. Can be a string, number, array...</param>
        /// <param name="failmessage">Message to return if the verification fails...</param>
        public string NotEquals(Object expected, Object current, string failmessage = null) {
            if (Utils.ObjectEquals(expected, current))
                return String.IsNullOrEmpty(failmessage) ? "KO, Verify.NotEquals failed! exp=<" + Utils.Truncate(expected.ToString()) + "> got=<" + Utils.Truncate(current.ToString()) + ">" : failmessage;
            return "OK";
        }

        /// <summary></summary>
        /// <param name="text"></param>
        /// <param name="pattern"></param>
        /// <param name="failmessage"></param>
        /// <returns></returns>
        public string Matches(string text, string pattern, string failmessage = null) {
            if (Regex.IsMatch(text, pattern))
                return "OK";
            return String.IsNullOrEmpty(failmessage) ? "KO, Verify.Matches failed! txt=<" + Utils.Truncate(text.ToString()) + "> pat=<" + pattern + ">" : failmessage;
        }

        /// <summary></summary>
        /// <param name="text"></param>
        /// <param name="pattern"></param>
        /// <param name="failmessage"></param>
        /// <returns></returns>
        public string NotMatches(string text, string pattern, string failmessage = null) {
            if (Regex.IsMatch(text, pattern))
                return String.IsNullOrEmpty(failmessage) ? "KO, Verify.NotMatches failed! txt=<" + Utils.Truncate(text.ToString()) + "> pat=<" + pattern + ">" : failmessage;
            return "OK";
        }

        /// <summary></summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <param name="failmessage"></param>
        /// <returns></returns>
        public string Contains(string text, string value, string failmessage = null) {
            if (text.Contains(value))
                return "OK";
            return String.IsNullOrEmpty(failmessage) ? "KO, Verify.Contains failed! txt=<" + Utils.Truncate(text) + "> val=<" + value + ">" : failmessage;
        }

    }
}
