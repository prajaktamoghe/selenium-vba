
' Description: Template for VBS

Class Scripts

	Dim wd, Assert, Keys

	public Sub ScriptA
		Set wd = CreateObject("SeleniumWrapper.WebDriver")
		Set Assert = CreateObject("SeleniumWrapper.Assert")
		Set Keys = CreateObject("SeleniumWrapper.Keys")
		wd.Start "firefox", "http://www.google.com"
		wd.setImplicitWait 5000
		wd.open "/"
		Assert.Equals wd.Title, "Google"
		wd.Type "name=q", "Selenium"
		wd.Click "name=btnG"
	End Sub

	Private Sub Class_Terminate
		wd.stop
	End Sub
	
End Class

With New Scripts
	.Run
End With