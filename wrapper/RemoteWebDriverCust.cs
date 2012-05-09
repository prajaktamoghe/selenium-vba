using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;

namespace SeleniumWrapper
{
    internal class RemoteWebDriverCust : RemoteWebDriver, ITakesScreenshot 
    {
        public RemoteWebDriverCust(Uri uri, DesiredCapabilities lCapability) : base(uri,lCapability)
        {

        }

        public Screenshot GetScreenshot()
        { 
            Response screenshotResponse = this.Execute(DriverCommand.Screenshot, null); 
            string base64 = screenshotResponse.Value.ToString(); 
            return new Screenshot(base64); 
        }
    }
}
