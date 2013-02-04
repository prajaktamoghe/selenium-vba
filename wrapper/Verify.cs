using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace SeleniumWrapper
{
    /// <summary>Testing functions. Return the résult of the verification</summary>
    /// <example>
    /// 
    /// The following example asserts the page title.
    /// <code lang="vbs">	
    /// Set driver = CreateObject("SeleniumWrapper.WebDriver")
    /// Set Verify = CreateObject("SeleniumWrapper.Verify")
    /// driver.start "firefox", "http://www.google.com"
    /// driver.open "/"
    /// wscript.echo Assert.Verify("Google", driver.Title)
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

    [Guid("AEBDE123-87BA-4BB6-A8E7-495CC9DBFB96")]
    [ComVisible(true)]
    public interface IVerify
    {
        [Description("")]
        string True(bool value, [Optional][DefaultParameterValue("")]string failmessage);

        [Description("")]
        string False(bool value, [Optional][DefaultParameterValue("")]string failmessage);

        [Description("Returns a string with the result of the verification ( <OK> or <KO, ...> )")]
        string Equals(Object expected, Object current, [Optional][DefaultParameterValue("")]string failmessage);

        [Description("Returns a string with the result of the verification ( <OK> or <KO, ...> )")]
        string NotEquals(Object expected, Object current, [Optional][DefaultParameterValue("")]string failmessage);

        [Description("")]
        string Matches(string input, string pattern, [Optional][DefaultParameterValue("")]string failmessage);

        [Description("")]
        string Contains(string input, string text, [Optional][DefaultParameterValue("")]string failmessage);

        [Description("")]
        string NotMatches(string input, string pattern, [Optional][DefaultParameterValue("")]string failmessage);
    }

    [Description("Assertion tool class")]
    [Guid("1EA5B911-25A8-493D-B882-B0C8C528C673")]
    [ComVisible(true), ComDefaultInterface(typeof(IVerify)), ClassInterface(ClassInterfaceType.None)]
    public class Verify : IVerify
    {
        /// <summary></summary>
        /// <param name="value"></param>
        /// <param name="failmessage"></param>
        /// <returns></returns>
        public string True(bool value, [Optional][DefaultParameterValue(null)]string failmessage){
            if (value==true) {
                return "OK";
            }else{
                return String.IsNullOrEmpty(failmessage) ? "KO, Verify.True failed!" : failmessage;
            }
        }

        /// <summary></summary>
        /// <param name="value"></param>
        /// <param name="failmessage"></param>
        /// <returns></returns>
        public string False(bool value, [Optional][DefaultParameterValue(null)]string failmessage){
            if (value==false) {
                return "OK";
            }else{
                return String.IsNullOrEmpty(failmessage) ? "KO, Verify.False failed!" : failmessage;
            }
        }

        /// <summary>Test that two objects are equal and raise an exception if the result is false</summary>
        /// <param name="expected">expected object. Can be a string, number, array...</param>
        /// <param name="current">current object. Can be a string, number, array...</param>
        /// <param name="failmessage">Message to return if the verification fails...</param>
        public string Equals(Object expected, Object current, [Optional][DefaultParameterValue(null)]string failmessage) {
            if ( Utils.ObjectEquals(expected, current)) {
                return "OK";
            }else{
                return String.IsNullOrEmpty(failmessage) ? "KO, Verify.Equals failed! expected=<" + Utils.Truncate(expected.ToString()) + "> result=<" + Utils.Truncate(current.ToString()) + ">" : failmessage; 
            }
        }

        /// <summary>Test that two objects are not equal and raise an exception if the result is false</summary>
        /// <param name="expected">expected object. Can be a string, number, array...</param>
        /// <param name="current">current object. Can be a string, number, array...</param>
        /// <param name="failmessage">Message to return if the verification fails...</param>
        public string NotEquals(Object expected, Object current, [Optional][DefaultParameterValue(null)]string failmessage) {
            if ( Utils.ObjectEquals(expected, current)) {
                return String.IsNullOrEmpty(failmessage) ? "KO, Verify.NotEquals failed! expected=<" + Utils.Truncate(expected.ToString()) + "> result=<" + Utils.Truncate(current.ToString()) + ">" : failmessage;
            }else{
                return "OK";
            }
        }

        /// <summary></summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <param name="failmessage"></param>
        /// <returns></returns>
        public string Matches(string input, string pattern, [Optional][DefaultParameterValue(null)]string failmessage){
            if(Regex.IsMatch(input, pattern)){
                return "OK";
            }else{
                return String.IsNullOrEmpty(failmessage) ? "KO, Verify.Matches failed! input=<" + Utils.Truncate(input.ToString()) + "> pattern=<" + pattern + ">" : failmessage;
            }
        }

        /// <summary></summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <param name="failmessage"></param>
        /// <returns></returns>
        public string NotMatches(string input, string pattern, [Optional][DefaultParameterValue(null)]string failmessage){
            if(Regex.IsMatch(input, pattern)){
                return String.IsNullOrEmpty(failmessage) ? "KO, Verify.NotMatches failed! input=<" + Utils.Truncate(input.ToString()) + "> pattern=<" + pattern + ">" : failmessage;
                
            }else{
                return "OK";
            }
        }

        /// <summary></summary>
        /// <param name="input"></param>
        /// <param name="text"></param>
        /// <param name="failmessage"></param>
        /// <returns></returns>
        public string Contains(string input, string text, [Optional][DefaultParameterValue(null)]string failmessage)
        {
            if (input.Contains(text)){
                return "OK";
            }else{
                return String.IsNullOrEmpty(failmessage) ? "KO, Verify.Contains failed! input=<" + Utils.Truncate(input) + "> text=<" + text + ">" : failmessage;
            }
        }

    }
}
