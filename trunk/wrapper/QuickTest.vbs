
' Author: Florent BREHERET
' Description: Ask the user to launch internet explorer and search for "Eiffel tower"

Dim obj, selenium, ret, title
on error resume next
set obj = CreateObject("SeleniumWrapper.Assembly")
If Err = 0 Then
	ret = obj.GetVersion()
	If Err = 0 Then
		If MsgBox("Version of SeleniumWrapper is : " & ret & vbCr & "Launch browser and search <Eiffel tower> ?", vbYesNo) = vbYes then
			on error goto 0
			Set selenium = CreateObject("SeleniumWrapper.WebDriver")
			selenium.start "firefox", "http://www.google.com"
			selenium.windowMaximize
			selenium.open "/"
			selenium.type "name=q", "Eiffel tower"
			selenium.wait 500
			'selenium.click "name=btnG"
			selenium.keyPress "name=q", "\13"
			selenium.wait 100
			selenium.getScreenshot().Copy
			title = selenium.getTitle()
			wscript.echo "Page title was : " & title & vbCr & "A website screenshoot has been sent to the clipboard" 
			selenium.stop
		End If
	Else
		wscript.echo "Test failed !   " & vbCr & Err.Description
	End If
Else
	wscript.echo "Test failed !   " & vbCr & "Failed to create an object from SeleniumWrapper    " & vbCr & Err.Description	
End If