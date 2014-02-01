' Date: 2014/01/24
' Author: Florent BREHERET
' Description: Compare the rendering of two web pages

Set driver = CreateObject("SeleniumWrapper.WebDriver")
driver.start "chrome", "http://www.google.co.uk"	'Starts the browser
driver.maximizeWindow
driver.open "/"		'Opens google search version UK
Set imageA = driver.getScreenshot(500)		'Captures the rendering
imageA.saveAs "image-a.png"

driver.open "http://www.google.fr"		'Opens google search version UK
Set imageB = driver.getScreenshot(500)		'Captures the rendering
imageB.saveAs "image-b.png"
driver.stop

Set imageDiff = imageA.compareTo(imageB)	'Compares images and place the result in a new image
imageDiff.saveAs "image-diff.png"

imageA.Dispose
imageB.Dispose

wscript.echo imageDiff.UnmatchedRatio * 100 & "% of non matching pixels"	'Displays the comparison result
imageDiff.Dispose


'Set pdf = CreateObject("SeleniumWrapper.PdfFile")	'Creates a PDF file
''Add a title and informations to the PDF
'pdf.addText "Selenium compare result",20,,true,,,true	'Adds a title to the Pdf
'pdf.addImage imageDiff, "Search page"	'Adds the image to the Pdf
'pdf.saveAs "page-compare.pdf"	'Saves the PDF
