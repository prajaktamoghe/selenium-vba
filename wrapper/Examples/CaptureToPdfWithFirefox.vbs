
' Date: 2013/02/02
' Author: Florent BREHERET
' Description: Search for "Eiffel tower", create a Pdf and insert a screen capture of the page result.

currentFolder = Replace(WScript.ScriptFullName,WScript.ScriptName,"")

Set driver = CreateObject("SeleniumWrapper.WebDriver")
Set Keys = CreateObject("SeleniumWrapper.Keys")
Set pdf = CreateObject("SeleniumWrapper.PdfFile")

'Start the browser
driver.start "firefox", "http://www.google.com"
driver.open "/"

'Define the PDF page size and margins
pdf.setPageSize 210, 297
pdf.setMargins 5, 5, 10, 15

'Add a title and informations to the PDF
pdf.addText "Selenium search result",20,,true,,,true
pdf.addVerticalSpace 5
pdf.addText "Description = " & "Search for Eiffel tower",9,"Blue"
pdf.addText "Base url = " & driver.Url ,9 , "Blue"

'Add a text paragraph to the PDF file
text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam sit amet libero arcu, et molestie purus. Ut in sem lacus, sit amet rhoncus erat. In aliquet arcu at nunc porta sollicitudin. Cras ante nisl, hendrerit quis bibendum quis, egestas vitae mi. Donec ac felis at eros placerat iaculis. Nam quam sapien, scelerisque vel bibendum et, mollis sit amet augue. Nullam egestas, lectus ut laoreet vulputate, neque quam vestibulum sapien, ut vehicula nunc metus et nulla. Curabitur ac lorem augue. Nullam quis justo eu arcu volutpat ultrices ac at orci."
pdf.addText text, 10

'Take a screenschot and add it to the PDF file
pdf.addImage driver.getScreenshot(), "Search page"

'Search for Eiffel tower, take a screenschot and add it to the PDF file
Set textbox = driver.findElementByName("q")
textbox.sendKeys "Eiffel tower"
textbox.sendKeys Keys.Return
driver.wait 500
pdf.addImage driver.getScreenshot(), "Web result page", true

'Go to the Image result page, take a screenschot and add it to the PDF file
driver.findElementByLinkText("Maps").click
driver.wait 500
pdf.addImage driver.getScreenshot(), "Maps result page", true

'Save the PDF
pdf.saveAs currentFolder & "my-capture.pdf"

'Stop the browser
driver.stop
