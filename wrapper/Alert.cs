using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SeleniumWrapper
{
    [Guid("EF8F7E6E-CADB-498F-9AE9-B51CB7C5A694")]
    [ComVisible(true)]
    public interface IAlert
    {
        [Description("Dismisses the alert.")]
        void Dismiss();

        [Description("Accepts the alert.")]
        void Accept();

        [Description("Sends keys to the alert.")]
        void SendKeys(string keysToSend);

        [Description("Gets the text of the alert.")]
        string Text{get;}
    }

    [Description("")]
    [Guid("3ACCA460-93E3-4CB9-9934-2845E23A4514")]
    [ComVisible(true), ComDefaultInterface(typeof(IAlert)), ClassInterface(ClassInterfaceType.None)]
    public class Alert : IAlert
    {
        private WebDriver webDriver;
        private OpenQA.Selenium.IAlert alert;

        /// <summary></summary>
        /// <param name="webDriver"></param>
        internal Alert(WebDriver webDriver)
        {
            this.webDriver = webDriver;
            try{
                this.alert = webDriver.webDriver.SwitchTo().Alert();
            }catch(Exception){
                throw new Exception("There is not alert present!");
            }
        }

        internal static bool isAlertPresent(WebDriver webDriver)
        {
            try{
                return webDriver.webDriver.SwitchTo().Alert() != null;
            }catch(Exception){
                throw new Exception("There is not alert present!");
            }
        }

        /// <summary>Dismisses the alert.</summary>
        public void Dismiss()
        {
            this.alert.Dismiss();
        }

        /// <summary>Accepts the alert.</summary>
        public void Accept()
        {
            this.alert.Accept();
        }

        /// <summary>Sends keys to the alert.</summary>
        /// <param name="keysToSend"></param>
        public void SendKeys(string keysToSend)
        {
            this.alert.SendKeys(keysToSend);
        }

        public string Text
        {
            get{ return this.alert.Text; }
        }
    }
}
