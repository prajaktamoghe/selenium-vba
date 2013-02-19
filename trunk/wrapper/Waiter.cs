using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SeleniumWrapper
{
    [Guid("159495B0-A903-4FA5-873E-384C7E50EFA8")]
    [ComVisible(true)]
    public interface IWaiter{

        [Description("Returns a boolean to continue waiting and throws an exception if the timeout is reached")]
        bool Until(bool condition, [Optional][DefaultParameterValue("")]string timeoutmessage);

        [Description("Returns a boolean to continue waiting and throws an exception if the timeout is reached")]
        bool UntilNot(bool condition, [Optional][DefaultParameterValue("")]string timeoutmessage);

        [Description("Set the waiter timeout. Default is 30000 milliseconds")]
        int Timeout{get; set;}
    }


    /// <summary>Waiting functions to keep the visual basic editor from not responding</summary>
    /// <example>
    /// 
    /// <code lang="vbs">	
    /// Public Sub TestCase()
    ///   Dim driver As New SeleniumWrapper.WebDriver, Waiter as New SeleniumWrapper.Assert
    ///   driver.start "firefox", "http://www.google.com"
    ///   driver.open "/"
    ///   Waiter.Timeout = 5000
    ///   While Waiter.Wait(driver.Title = "MyTitle"): DoEvents: Wend
    ///   driver.stop
    /// End Sub
    /// </code>
    /// 
    /// </example>
    ///

    [Description("Waiting functions to keep the visual basic editor from not responding")]
    [Guid("4A1829E7-800A-450E-86F9-7D30CBC3F6BB")]
    [ComVisible(true), ComDefaultInterface(typeof(IWaiter)), ClassInterface(ClassInterfaceType.None)]
    public class Waiter : IWaiter
    {
        private object end;
        private double timeout = 30000;
        
        /// <summary>Waiter timeout in millisecond. Default is 30000 milliseconds</summary>
        public int Timeout
        {
            get{ return (int)this.timeout; }
            set{ this.timeout = value; }
        }


        /// <summary>Returns true once the time to wait is over</summary>
        /// <param name="timems">Time to wait in milliseconde</param>
        /// <returns>Returns false if the time to wait is over</returns>
        /// <example>
        /// 
        /// <code lang="vbs">	
        /// Public Sub TC002()
        ///   Dim driver As New SeleniumWrapper.WebDriver, Waiter As New Waiter
        ///   driver.Start "firefox", "http://www.mywebsite.com/"   'Launch Firefox
        ///   driver.Open "/"
        ///   While Waiter.Sleep(5000): DoEvents: Wend
        ///   ...
        /// End Sub
        /// </code>
        /// 
        /// </example>
        public bool Sleep(int timems){
            if(this.end == null){
                this.end = DateTime.Now.AddMilliseconds(timems);
            }else{
                if( DateTime.Now > (DateTime)this.end ){
                    this.end = null;
                    return false;
                }
                System.Threading.Thread.Sleep(25);
            }
            return true;
        }

        /// <summary>Returns a boolean to continue waiting and throws an exception if the timeout is reached</summary>
        /// <param name="condition">Condition or time to wait</param>
        /// <param name="timeoutmessage">mesage in case of timeout</param>
        /// <returns>Returns false if the condition is met or time to wait finished</returns>
        /// <example>
        /// 
        /// <code lang="vbs">	
        /// Public Sub TC002()
        ///   Dim driver As New SeleniumWrapper.WebDriver, Waiter As New Waiter
        ///   driver.Start "firefox", "http://www.mywebsite.com/"   'Launch Firefox
        ///   driver.Open "/"
        ///   Waiter.Timeout = 5000
        ///   While Waiter.Wait(driver.Title = "MyTitle"): DoEvents: Wend
        ///   ...
        /// End Sub
        /// </code>
        /// 
        /// </example>
        public bool Until(bool condition, [Optional][DefaultParameterValue(null)]string timeoutmessage){
            if(this.end == null){
                this.end = DateTime.Now.AddMilliseconds(this.timeout);
            }else{
                if( DateTime.Now > (DateTime)this.end ){
                    this.end = null;
                    throw new TimeoutException(String.IsNullOrEmpty(timeoutmessage) ? "The operation has timed out!" : timeoutmessage);
                }
                System.Threading.Thread.Sleep(25);
            }
            if((bool)condition){
                this.end = null;
                return false;
            }else{
                return true;
            }
        }

        /// <summary>Returns a boolean to continue waiting and throws an exception if the timeout is reached</summary>
        /// <param name="condition">Condition</param>
        /// <param name="timeoutmessage">mesage in case of timeout</param>
        /// <returns>Returns false if the condition is not met</returns>
        public bool UntilNot(bool condition, [Optional][DefaultParameterValue("")]string timeoutmessage)
        {
            return this.Until(!condition, timeoutmessage);
        }
    }
}
