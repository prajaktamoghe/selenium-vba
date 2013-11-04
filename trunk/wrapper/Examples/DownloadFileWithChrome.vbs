
' Date: 04/11/2013
' Author: Florent BREHERET
' Description: Example of command line argument and preference for chrome

currentFolder = Replace(WScript.ScriptFullName,WScript.ScriptName,"")

Set driver = CreateObject("SeleniumWrapper.WebDriver")

'To start chrome maximized
driver.addArgument "--start-maximized"

'To automatically download  files to a directory
driver.setPreference "download.default_directory", currentFolder & "chrome_download"
driver.setPreference "download.directory_upgrade", true
driver.setPreference "download.extensions_to_open", ""
driver.setPreference "download.prompt_for_download", false

driver.start "chrome", "http://selenium-vba.googlecode.com"
driver.open "/svn/trunk/wrapper/Examples/TestsWithExcel.xls"
driver.wait 2000
driver.stop