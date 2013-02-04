
' Date: 27/01/2013
' Author: Florent BREHERET
' Description: Automatically download a file with Chrome (Doesn't work as it's not implemented in the .Net driver!)

currentFolder = Replace(WScript.ScriptFullName,WScript.ScriptName,"")

Set selenium = CreateObject("SeleniumWrapper.WebDriver")

Set options = CreateObject("System.Collections.Hashtable")
options.Add "default_directory", "C:\temp"
options.Add "directory_upgrade", true
options.Add "extensions_to_open", ""
options.Add "prompt_for_download", false

Set download = CreateObject("System.Collections.Hashtable")
download.Add "download", options

selenium.setCapability "chrome.prefs", download
selenium.start "chrome", "http://www.google.com"
selenium.open "http://selenium-vba.googlecode.com/svn/trunk/wrapper/Examples/TestsWithExcel.xls"
selenium.wait 2000
selenium.stop
