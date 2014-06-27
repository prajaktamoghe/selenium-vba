#Include "Include.vbs"

Dim wd, Assert, Keys

Sub Initialize
	echo "Initialize was called"
	Set wd = CreateObject("SeleniumWrapper.WebDriver")
	Set Assert = CreateObject("SeleniumWrapper.Assert")
	Set Keys = CreateObject("SeleniumWrapper.Keys")
	wd.Start "firefox", "http://www.google.com"
End Sub

Sub Terminate
	wd.stop
End Sub

Sub SetUp
	echo "SetUp was called"
	wd.open "/"
End Sub

Sub TearDown
	echo "TearDown was called"
End Sub

Function OnError(procedure, description)
	echo "OnError was called. Error:" & procedure & " " & description
	OnError = "SC: " & wd.getScreenshot().SaveAs("Images\image_{TIME}.png")
End Function


public Sub ProcedureA
	echo "ProcedureA was called"
	Err.Raise 1, "", "Test error"
End Sub

public Sub ProcedureB
	echo "ProcedureB was called and function from Include.vbs returned " & GetValue()
End Sub

[With("firefox", "chrome")]
Sub ProcedureC(param)
	echo "ProcedureC was called with parameter: " & param
End Sub
