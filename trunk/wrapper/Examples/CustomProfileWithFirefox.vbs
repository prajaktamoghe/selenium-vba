
' Date: 21/08/2012
' Author: Florent BREHERET
' Description: Automatically download a file with firefox
' Remarks : The profile directory can be copied from the user temp folder (run %temp%) before
'           the WebDriver is stopped. It's also possible to create a new Firefox profile by
'           launching firefox with the "-p" switch (firefox.exe -p).

currentFolder = Replace(WScript.ScriptFullName,WScript.ScriptName,"")

Set selenium = CreateObject("SeleniumWrapper.WebDriver")
selenium.setProfile "Selenium"
selenium.start "firefox", "http://www.google.com"
selenium.open "/"
