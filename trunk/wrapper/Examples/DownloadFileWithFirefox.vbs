
' Date: 21/08/2012
' Author: Florent BREHERET
' Description: Automatically download a file with firefox

currentFolder = Replace(WScript.ScriptFullName,WScript.ScriptName,"")

Set selenium = CreateObject("SeleniumWrapper.WebDriver")
selenium.setPreference "browser.download.folderList", 2
selenium.setPreference "pref.downloads.disable_button.edit_actions", 2
selenium.setPreference "browser.download.dir", currentFolder & "firefox_download"
selenium.setPreference "browser.helperApps.neverAsk.saveToDisk", "application/octet-stream,application/pdf"

selenium.start "firefox", "http://www.google.com"
selenium.open "http://selenium-vba.googlecode.com/svn/trunk/wrapper/Examples/TestsWithExcel.xls"
selenium.wait 2000
selenium.stop
