
' Description: Template for VBS

currentFolder = Replace(WScript.ScriptFullName,WScript.ScriptName,"")

Set driver = CreateObject("SeleniumWrapper.WebDriver")
Set Assert = CreateObject("SeleniumWrapper.Assert")
Set Verify = CreateObject("SeleniumWrapper.Verify")
Set Keys = CreateObject("SeleniumWrapper.Keys")
Set By = CreateObject("SeleniumWrapper.By")

driver.start "firefox", "http://www.google.com"
driver.setImplicitWait 5000
driver.open "/"

Assert.Equals driver.Title, "Google"

'Stop the browser
driver.stop
