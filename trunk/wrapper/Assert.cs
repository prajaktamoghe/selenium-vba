using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace SeleniumWrapper
{
    [Guid("1F96D1DD-EF65-4D27-A7FF-0EA52ACB97D1")]
    [ComVisible(true)]
    public interface IAssert
    {
        [Description("")]
        void True(bool value, [Optional][DefaultParameterValue("")]string failmessage);

        [Description("")]
        void False(bool value, [Optional][DefaultParameterValue("")]string failmessage);

        [Description("Raise an error if the assertion fails")]
        void Equals(Object expected, Object current, [Optional][DefaultParameterValue("")]string failmessage);

        [Description("Raise an error if the assertion fails")]
        void NotEquals(Object expected, Object current, [Optional][DefaultParameterValue("")]string failmessage);

        [Description("")]
        void Matches(string input, string pattern, [Optional][DefaultParameterValue("")]string failmessage);

        [Description("")]
        void NotMatches(string input, string pattern, [Optional][DefaultParameterValue("")]string failmessage);

        [Description("")]
        void Contains(string input, string text, [Optional][DefaultParameterValue("")]string failmessage);

        [Description("")]
        void Fail([Optional][DefaultParameterValue(null)]string message);
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
    public class Assert : IAssert
    {
        /// <summary></summary>
        /// <param name="value"></param>
        /// <param name="failmessage"></param>
        public void True(bool value, [Optional][DefaultParameterValue("")]string failmessage)
        {
            if (value != true) throw new Exception("Assert.True failed!" + "\n" + failmessage);
        }

        /// <summary></summary>
        /// <param name="value"></param>
        /// <param name="failmessage"></param>
        public void False(bool value, [Optional][DefaultParameterValue("")]string failmessage)
        {
            if (value != false) throw new Exception("Assert.False failed!" + "\n" + failmessage);
        }

        /// <summary>Test that two objects are equal and raise an exception if the result is false</summary>
        /// <param name="expected">expected object. Can be a string, number, array...</param>
        /// <param name="current">current object. Can be a string, number, array...</param>
        /// <param name="failmessage"></param>
        public void Equals(Object expected, Object current, [Optional][DefaultParameterValue("")]string failmessage)
        {
            if ( ! Utils.ObjectEquals(expected, current)) {
                throw new ApplicationException("Assert.Equals failed!\n" + (failmessage!="" ? failmessage : "expected=<" + Utils.Truncate(expected.ToString()) + "> result=<" + Utils.Truncate(current.ToString()) + "> " + "\n") ); 
            }
        }

        /// <summary>Test that two objects are not equal and raise an exception if the result is false</summary>
        /// <param name="expected">expected object. Can be a string, number, array...</param>
        /// <param name="current">current object. Can be a string, number, array...</param>
        /// <param name="failmessage"></param>
        public void NotEquals(Object expected, Object current, [Optional][DefaultParameterValue("")]string failmessage)
        {
            if ( Utils.ObjectEquals(expected, current)) {
                throw new ApplicationException("Assert.NotEquals failed!\n" + (failmessage!="" ? failmessage : "expected=<" + Utils.Truncate(expected.ToString()) + "> result=<" + Utils.Truncate(current.ToString()) + "> ") );
            }
        }

        /// <summary></summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <param name="failmessage"></param>
        public void Matches(string input, string pattern, [Optional][DefaultParameterValue("")]string failmessage)
        {
            if(!Regex.IsMatch(input, pattern)){
                throw new ApplicationException("Assert.Matches failed!\n" + (failmessage!="" ? failmessage : "input=<" + Utils.Truncate(input) + "> pattern=<" + pattern + "> ") );
            }
        }

        /// <summary></summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <param name="failmessage"></param>
        public void NotMatches(string input, string pattern, [Optional][DefaultParameterValue("")]string failmessage)
        {
            if(Regex.IsMatch(input, pattern)){
                throw new ApplicationException("Assert.NotMatches failed!\n" + (failmessage!="" ? failmessage : "input=<" + Utils.Truncate(input) + "> pattern=<" + pattern + "> ") );
            }
        }

        /// <summary></summary>
        /// <param name="input"></param>
        /// <param name="text"></param>
        /// <param name="failmessage"></param>
        public void Contains(string input, string text, [Optional][DefaultParameterValue("")]string failmessage)
        {
            if (!input.Contains(text))
            {
                throw new ApplicationException("Assert.Contains failed!\n" + (failmessage != "" ? failmessage : "input=<" + Utils.Truncate(input) + "> text=<" + text + "> "));
            }
        }

        /// <summary></summary>
        /// <param name="message"></param>
        public void Fail([Optional][DefaultParameterValue(null)]string message)
        {
            throw new ApplicationException(message);
        }

    }
}
