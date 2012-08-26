
' Date: 21/08/2012
' Author: Florent BREHERET
' Description: Search for "Eiffel tower", create a Pdf and insert a screen capture of the page result.

Set selenium = CreateObject("SeleniumWrapper.WebDriver")
Set pdf = CreateObject("SeleniumWrapper.PdfFile")
currentFolder = Replace(WScript.ScriptFullName,WScript.ScriptName,"")

selenium.start "ff", "http://www.google.com"
selenium.open "/"
selenium.type "q", "Eiffel tower"
selenium.keyPressAndWait "q", "\10"

pdf.addText "Simple text"
pdf.addHtml "<font color=""#0000FF""><b><i>Italic blue text</i></b></font>"
pdf.addHtml "<br><OL><LI>First step</LI><LI>second step</LI><LI>third step</LI></OL>"
pdf.addImage selenium.captureScreenshotToImage(), "Search for Eiffel tower"
pdf.saveAs currentFolder & "selenium-capture.pdf"
selenium.stop
