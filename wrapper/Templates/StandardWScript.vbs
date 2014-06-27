
Class Scripts

	Dim wd, Assert, Keys

	public Sub Run
		Set wd = CreateObject("SeleniumWrapper.WebDriver")
		Set Assert = CreateObject("SeleniumWrapper.Assert")
		Set Keys = CreateObject("SeleniumWrapper.Keys")
		wd.Start "firefox", "http://www.google.com"
		wd.setImplicitWait 5000
		wd.open "/"
		Assert.Equals wd.Title, "Google"
	End Sub

	Private Sub Class_Terminate
		wd.stop
	End Sub
	
End Class

Set script = New Scripts: script.Run