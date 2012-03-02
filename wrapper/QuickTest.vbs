' Quick test with FireFox
' Author: Florent BREHERET
' Description: Ask the user to launch firefox and search for "Selenium IDE"

Dim selenium

If MsgBox("Launch Internet Explorer and search <Eiffel tower> ?", vbYesNo) = vbYes then

	Set selenium = CreateObject("SeleniumWrapper.WebDriver")
	selenium.start "ie", "http://www.google.com"
	selenium.open "/"
	selenium.type "name=q", "Eiffel tower"
	selenium.click "name=btnG"
	
End If