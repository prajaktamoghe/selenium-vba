using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace SeleniumWrapper
{
    internal class RemoteWebDriverCust : RemoteWebDriver, ITakesScreenshot 
    {
        public RemoteWebDriverCust(Uri uri, DesiredCapabilities lCapability) : base(uri,lCapability)
        {

        }

        public Screenshot GetScreenshot()
        { 
            return new Screenshot( (string)base.Execute(DriverCommand.Screenshot, null).Value ); 
        }
    }
}
