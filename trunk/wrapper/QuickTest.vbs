
' Author: Florent BREHERET
' Description: Ask the user to launch internet explorer and search for "Eiffel tower"

Dim selenium

If MsgBox("Launch Internet Explorer and search <Eiffel tower> ?", vbYesNo) = vbYes then

	Set selenium = CreateObject("SeleniumWrapper.WebDriver")
	selenium.start "ie", "http://www.google.com"
	selenium.open "/"
	selenium.type "name=q", "Eiffel tower"
	selenium.click "name=btnG"
	selenium.captureScreenshotToClipboard
	msgbox "verify the page title equals Pisa tower : " & chr(13) & selenium.verifyTitle("Pisa tower")
End If