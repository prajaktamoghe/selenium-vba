
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace Selenium
{
	[ComVisible(true)]
    //[ClassInterface(ClassInterfaceType.AutoDual)]
    [ClassInterface(ClassInterfaceType.None)]
    [ComSourceInterfaces(typeof(IEvents))]
    public class Driver : IDriver
	{

		[ComVisible(false)]
		public delegate void EventInfo(Evt evtype, string message);
        public event EventInfo DriverEvent;	
		protected HttpCommandProcessor commandProcessor;

		public Driver()
		{
			this.commandProcessor = new HttpCommandProcessor();
			this.commandProcessor.SendEventInfo += new HttpCommandProcessor.EventInfoHandler(ReceivedEventInfo);
			this.commandProcessor.ExceptionLevel = Err.e4verify ;
			this.commandProcessor.EventLevel = Evt.e4cmdrunned ;			
		}
		
        public void ReceivedEventInfo(Evt eventlevel, Evt evtype, string message)
        {
			if (DriverEvent != null){
				if( evtype <= eventlevel){
					DriverEvent(evtype, message );
				}
			}
        }
		
		public string ErrInfo
		{
			get { return this.commandProcessor.remoteCommand.ErrorInfo; }
		}
		
		public Err ErrType
		{
			get { return this.commandProcessor.remoteCommand.ErrorType; }
		}
		
		public string ErrCmd
		{
			get { return this.commandProcessor.remoteCommand.Command; }
		}
		
		public string[] ErrArgs
		{
			get { return this.commandProcessor.remoteCommand.Args; }
		}
		
		public string EvCmd
		{
			get { return this.commandProcessor.remoteCommand.Command; }
		}
		
		public string[] EvArgs
		{
			get { return this.commandProcessor.remoteCommand.Args; }
		}		
		
		public void setDriverErrorLevel(Err Value)
		{
			this.commandProcessor.ExceptionLevel = Value;		
		}
		
		public void setDriverEventsLevel(Evt Value)
		{
			this.commandProcessor.EventLevel = Value;		
		}
		
		public void setExtensionJs(String extensionJs)
		{
			commandProcessor.SetExtensionJs(extensionJs);
			commandProcessor.DoCommand("setExtensionJs", new String[] {extensionJs,});
		}

		public void start(String serverHost, int serverPort, String browserString, String browserURL)
		{
			commandProcessor.Start(serverHost, serverPort, browserString, browserURL);
		}
		
		public void stop()
		{
			commandProcessor.Stop();
		}
		
		public string DoCommand(string command, string locator, string value)
		{
			return commandProcessor.GetString(command, new String[] {locator,value,});
		}		
		public void setAjaxCondition(string condition){
			//ex : setAjaxCondition "Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack()==false"
			commandProcessor.DoCommand("setAjaxCondition", new String[] {condition,});
		}
		
		public void addCustomRequestHeader(string key,string value){
			commandProcessor.DoCommand("addCustomRequestHeader", new String[] {key,value,});
		}
		public void addLocationStrategy(string strategyName,string functionDefinition){
			commandProcessor.DoCommand("addLocationStrategy", new String[] {strategyName,functionDefinition,});
		}
		public void addScript(string scriptContent,string scriptTagId){
			commandProcessor.DoCommand("addScript", new String[] {scriptContent,scriptTagId,});
		}
		public void addSelection(string locator,string optionLocator){
			commandProcessor.DoCommand("addSelection", new String[] {locator,optionLocator,});
		}
		public void allowNativeXpath(string allow){
			commandProcessor.DoCommand("allowNativeXpath", new String[] {allow,});
		}
		public void altKeyDown(){
			commandProcessor.DoCommand("altKeyDown", new String[] {});
		}
		public void altKeyDownAndWait(){
			commandProcessor.DoCommand("altKeyDownAndWait", new String[] {});
		}
		public void altKeyUp(){
			commandProcessor.DoCommand("altKeyUp", new String[] {});
		}
		public void altKeyUpAndWait(){
			commandProcessor.DoCommand("altKeyUpAndWait", new String[] {});
		}
		public void answerOnNextPrompt(string answer){
			commandProcessor.DoCommand("answerOnNextPrompt", new String[] {answer,});
		}
		public void answerOnNextPromptAndWait(string answer){
			commandProcessor.DoCommand("answerOnNextPromptAndWait", new String[] {answer,});
		}
		public void assignId(string locator,string identifier){
			commandProcessor.DoCommand("assignId", new String[] {locator,identifier,});
		}
		public void assignIdAndWait(string locator,string identifier){
			commandProcessor.DoCommand("assignIdAndWait", new String[] {locator,identifier,});
		}
		public void attachFile(string fieldLocator,string fileLocator){
			commandProcessor.DoCommand("attachFile", new String[] {fieldLocator,fileLocator,});
		}
		public void attachFileAndWait(string fieldLocator,string fileLocator){
			commandProcessor.DoCommand("attachFileAndWait", new String[] {fieldLocator,fileLocator,});
		}
		public void captureEntirePageScreenshot(string filename,string kwargs){
			commandProcessor.DoCommand("captureEntirePageScreenshot", new String[] {filename,kwargs,});
		}
		public void captureEntirePageScreenshotAndWait(string filename,string kwargs){
			commandProcessor.DoCommand("captureEntirePageScreenshotAndWait", new String[] {filename,kwargs,});
		}
		public string captureEntirePageScreenshotToString(string kwargs){
			return commandProcessor.GetString("captureEntirePageScreenshotToString", new String[] {kwargs,});
		}
		public string captureEntirePageScreenshotToStringAndWait(string kwargs){
			return commandProcessor.GetString("captureEntirePageScreenshotToStringAndWait", new String[] {kwargs,});
		}
		public string captureNetworkTraffic(string type){
			return commandProcessor.GetString("captureNetworkTraffic", new String[] {type,});
		}
		public string captureNetworkTrafficAndWait(string type){
			return commandProcessor.GetString("captureNetworkTrafficAndWait", new String[] {type,});
		}
		public void captureScreenshot(string filename){
			commandProcessor.DoCommand("captureScreenshot", new String[] {filename,});
		}
		public void captureScreenshotAndWait(string filename){
			commandProcessor.DoCommand("captureScreenshotAndWait", new String[] {filename,});
		}
		public string captureScreenshotToString(){
			return commandProcessor.GetString("captureScreenshotToString", new String[] {});
		}
		public string captureScreenshotToStringAndWait(){
			return commandProcessor.GetString("captureScreenshotToStringAndWait", new String[] {});
		}
		public void check(string locator){
			commandProcessor.DoCommand("check", new String[] {locator,});
		}
		public void checkAndWait(string locator){
			commandProcessor.DoCommand("checkAndWait", new String[] {locator,});
		}
		public void chooseCancelOnNextConfirmation(){
			commandProcessor.DoCommand("chooseCancelOnNextConfirmation", new String[] {});
		}
		public void chooseCancelOnNextConfirmationAndWait(){
			commandProcessor.DoCommand("chooseCancelOnNextConfirmationAndWait", new String[] {});
		}
		public void chooseOkOnNextConfirmation(){
			commandProcessor.DoCommand("chooseOkOnNextConfirmation", new String[] {});
		}
		public void chooseOkOnNextConfirmationAndWait(){
			commandProcessor.DoCommand("chooseOkOnNextConfirmationAndWait", new String[] {});
		}
		public void click(string locator){
			commandProcessor.DoCommand("click", new String[] {locator,});
		}
		public void clickAndWait(string locator){
			commandProcessor.DoCommand("clickAndWait", new String[] {locator,});
		}
		public void clickAt(string locator,string coordString){
			commandProcessor.DoCommand("clickAt", new String[] {locator,coordString,});
		}
		public void clickAtAndWait(string locator,string coordString){
			commandProcessor.DoCommand("clickAtAndWait", new String[] {locator,coordString,});
		}
		public void close(){
			commandProcessor.DoCommand("close", new String[] {});
		}
		public void closeAndWait(){
			commandProcessor.DoCommand("closeAndWait", new String[] {});
		}
		public void contextMenu(string locator){
			commandProcessor.DoCommand("contextMenu", new String[] {locator,});
		}
		public void contextMenuAndWait(string locator){
			commandProcessor.DoCommand("contextMenuAndWait", new String[] {locator,});
		}
		public void contextMenuAt(string locator,string coordString){
			commandProcessor.DoCommand("contextMenuAt", new String[] {locator,coordString,});
		}
		public void contextMenuAtAndWait(string locator,string coordString){
			commandProcessor.DoCommand("contextMenuAtAndWait", new String[] {locator,coordString,});
		}
		public void controlKeyDown(){
			commandProcessor.DoCommand("controlKeyDown", new String[] {});
		}
		public void controlKeyDownAndWait(){
			commandProcessor.DoCommand("controlKeyDownAndWait", new String[] {});
		}
		public void controlKeyUp(){
			commandProcessor.DoCommand("controlKeyUp", new String[] {});
		}
		public void controlKeyUpAndWait(){
			commandProcessor.DoCommand("controlKeyUpAndWait", new String[] {});
		}
		public void createCookie(string nameValuePair,string optionsString){
			commandProcessor.DoCommand("createCookie", new String[] {nameValuePair,optionsString,});
		}
		public void createCookieAndWait(string nameValuePair,string optionsString){
			commandProcessor.DoCommand("createCookieAndWait", new String[] {nameValuePair,optionsString,});
		}
		public void deleteAllVisibleCookies(){
			commandProcessor.DoCommand("deleteAllVisibleCookies", new String[] {});
		}
		public void deleteCookie(string name,string optionsString){
			commandProcessor.DoCommand("deleteCookie", new String[] {name,optionsString,});
		}
		public void deselectPopUp(){
			commandProcessor.DoCommand("deselectPopUp", new String[] {});
		}
		public void deselectPopUpAndWait(){
			commandProcessor.DoCommand("deselectPopUpAndWait", new String[] {});
		}
		public void doubleClick(string locator){
			commandProcessor.DoCommand("doubleClick", new String[] {locator,});
		}
		public void doubleClickAndWait(string locator){
			commandProcessor.DoCommand("doubleClickAndWait", new String[] {locator,});
		}
		public void doubleClickAt(string locator,string coordString){
			commandProcessor.DoCommand("doubleClickAt", new String[] {locator,coordString,});
		}
		public void doubleClickAtAndWait(string locator,string coordString){
			commandProcessor.DoCommand("doubleClickAtAndWait", new String[] {locator,coordString,});
		}
		public void dragAndDrop(string locator,string movementsString){
			commandProcessor.DoCommand("dragAndDrop", new String[] {locator,movementsString,});
		}
		public void dragAndDropAndWait(string locator,string movementsString){
			commandProcessor.DoCommand("dragAndDropAndWait", new String[] {locator,movementsString,});
		}
		public void dragAndDropToObject(string locatorOfObjectToBeDragged,string locatorOfDragDestinationObject){
			commandProcessor.DoCommand("dragAndDropToObject", new String[] {locatorOfObjectToBeDragged,locatorOfDragDestinationObject,});
		}
		public void dragAndDropToObjectAndWait(string locatorOfObjectToBeDragged,string locatorOfDragDestinationObject){
			commandProcessor.DoCommand("dragAndDropToObjectAndWait", new String[] {locatorOfObjectToBeDragged,locatorOfDragDestinationObject,});
		}
		public void dragdrop(string locator,string movementsString){
			commandProcessor.DoCommand("dragdrop", new String[] {locator,movementsString,});
		}
		public void dragdropAndWait(string locator,string movementsString){
			commandProcessor.DoCommand("dragdropAndWait", new String[] {locator,movementsString,});
		}
		public void fireEvent(string locator,string eventName){
			commandProcessor.DoCommand("fireEvent", new String[] {locator,eventName,});
		}
		public void fireEventAndWait(string locator,string eventName){
			commandProcessor.DoCommand("fireEventAndWait", new String[] {locator,eventName,});
		}
		public void focus(string locator){
			commandProcessor.DoCommand("focus", new String[] {locator,});
		}
		public void focusAndWait(string locator){
			commandProcessor.DoCommand("focusAndWait", new String[] {locator,});
		}
		public string getAlert(){
			return commandProcessor.GetString("getAlert", new String[] {});
		}
		public void verifyAlert(string pattern){
			commandProcessor.VerifyTrue("verifyAlert", new String[] {pattern,});
		}
		public void verifyNotAlert(string pattern){
			commandProcessor.VerifyTrue("verifyNotAlert", new String[] {pattern,});
		}
		public void assertAlert(string pattern){
			commandProcessor.AssertTrue("assertAlert", new String[] {pattern,});
		}
		public void assertNotAlert(string pattern){
			commandProcessor.AssertTrue("assertNotAlert", new String[] {pattern,});
		}
		public void waitForAlert(string pattern){
			commandProcessor.DoCommand("waitForAlert", new String[] {pattern,});
		}
		public void waitForNotAlert(string pattern){
			commandProcessor.DoCommand("waitForNotAlert", new String[] {pattern,});
		}
		public void storeAlert(string variableName){
			commandProcessor.DoCommand("storeAlert", new String[] {variableName,});
		}
		public string[] getAllButtons(){
			return commandProcessor.GetStringArray("getAllButtons", new String[] {});
		}
		public void verifyAllButtons(string pattern){
			commandProcessor.VerifyTrue("verifyAllButtons", new String[] {pattern,});
		}
		public void verifyNotAllButtons(string pattern){
			commandProcessor.VerifyTrue("verifyNotAllButtons", new String[] {pattern,});
		}
		public void assertAllButtons(string pattern){
			commandProcessor.AssertTrue("assertAllButtons", new String[] {pattern,});
		}
		public void assertNotAllButtons(string pattern){
			commandProcessor.AssertTrue("assertNotAllButtons", new String[] {pattern,});
		}
		public void waitForAllButtons(string pattern){
			commandProcessor.DoCommand("waitForAllButtons", new String[] {pattern,});
		}
		public void waitForNotAllButtons(string pattern){
			commandProcessor.DoCommand("waitForNotAllButtons", new String[] {pattern,});
		}
		public void storeAllButtons(string variableName){
			commandProcessor.DoCommand("storeAllButtons", new String[] {variableName,});
		}
		public string[] getAllFields(){
			return commandProcessor.GetStringArray("getAllFields", new String[] {});
		}
		public void verifyAllFields(string pattern){
			commandProcessor.VerifyTrue("verifyAllFields", new String[] {pattern,});
		}
		public void verifyNotAllFields(string pattern){
			commandProcessor.VerifyTrue("verifyNotAllFields", new String[] {pattern,});
		}
		public void assertAllFields(string pattern){
			commandProcessor.AssertTrue("assertAllFields", new String[] {pattern,});
		}
		public void assertNotAllFields(string pattern){
			commandProcessor.AssertTrue("assertNotAllFields", new String[] {pattern,});
		}
		public void waitForAllFields(string pattern){
			commandProcessor.DoCommand("waitForAllFields", new String[] {pattern,});
		}
		public void waitForNotAllFields(string pattern){
			commandProcessor.DoCommand("waitForNotAllFields", new String[] {pattern,});
		}
		public void storeAllFields(string variableName){
			commandProcessor.DoCommand("storeAllFields", new String[] {variableName,});
		}
		public string[] getAllLinks(){
			return commandProcessor.GetStringArray("getAllLinks", new String[] {});
		}
		public void verifyAllLinks(string pattern){
			commandProcessor.VerifyTrue("verifyAllLinks", new String[] {pattern,});
		}
		public void verifyNotAllLinks(string pattern){
			commandProcessor.VerifyTrue("verifyNotAllLinks", new String[] {pattern,});
		}
		public void assertAllLinks(string pattern){
			commandProcessor.AssertTrue("assertAllLinks", new String[] {pattern,});
		}
		public void assertNotAllLinks(string pattern){
			commandProcessor.AssertTrue("assertNotAllLinks", new String[] {pattern,});
		}
		public void waitForAllLinks(string pattern){
			commandProcessor.DoCommand("waitForAllLinks", new String[] {pattern,});
		}
		public void waitForNotAllLinks(string pattern){
			commandProcessor.DoCommand("waitForNotAllLinks", new String[] {pattern,});
		}
		public void storeAllLinks(string variableName){
			commandProcessor.DoCommand("storeAllLinks", new String[] {variableName,});
		}
		public string[] getAllWindowIds(){
			return commandProcessor.GetStringArray("getAllWindowIds", new String[] {});
		}
		public void verifyAllWindowIds(string pattern){
			commandProcessor.VerifyTrue("verifyAllWindowIds", new String[] {pattern,});
		}
		public void verifyNotAllWindowIds(string pattern){
			commandProcessor.VerifyTrue("verifyNotAllWindowIds", new String[] {pattern,});
		}
		public void assertAllWindowIds(string pattern){
			commandProcessor.AssertTrue("assertAllWindowIds", new String[] {pattern,});
		}
		public void assertNotAllWindowIds(string pattern){
			commandProcessor.AssertTrue("assertNotAllWindowIds", new String[] {pattern,});
		}
		public void waitForAllWindowIds(string pattern){
			commandProcessor.DoCommand("waitForAllWindowIds", new String[] {pattern,});
		}
		public void waitForNotAllWindowIds(string pattern){
			commandProcessor.DoCommand("waitForNotAllWindowIds", new String[] {pattern,});
		}
		public void storeAllWindowIds(string variableName){
			commandProcessor.DoCommand("storeAllWindowIds", new String[] {variableName,});
		}
		public string[] getAllWindowNames(){
			return commandProcessor.GetStringArray("getAllWindowNames", new String[] {});
		}
		public void verifyAllWindowNames(string pattern){
			commandProcessor.VerifyTrue("verifyAllWindowNames", new String[] {pattern,});
		}
		public void verifyNotAllWindowNames(string pattern){
			commandProcessor.VerifyTrue("verifyNotAllWindowNames", new String[] {pattern,});
		}
		public void assertAllWindowNames(string pattern){
			commandProcessor.AssertTrue("assertAllWindowNames", new String[] {pattern,});
		}
		public void assertNotAllWindowNames(string pattern){
			commandProcessor.AssertTrue("assertNotAllWindowNames", new String[] {pattern,});
		}
		public void waitForAllWindowNames(string pattern){
			commandProcessor.DoCommand("waitForAllWindowNames", new String[] {pattern,});
		}
		public void waitForNotAllWindowNames(string pattern){
			commandProcessor.DoCommand("waitForNotAllWindowNames", new String[] {pattern,});
		}
		public void storeAllWindowNames(string variableName){
			commandProcessor.DoCommand("storeAllWindowNames", new String[] {variableName,});
		}
		public string[] getAllWindowTitles(){
			return commandProcessor.GetStringArray("getAllWindowTitles", new String[] {});
		}
		public void verifyAllWindowTitles(string pattern){
			commandProcessor.VerifyTrue("verifyAllWindowTitles", new String[] {pattern,});
		}
		public void verifyNotAllWindowTitles(string pattern){
			commandProcessor.VerifyTrue("verifyNotAllWindowTitles", new String[] {pattern,});
		}
		public void assertAllWindowTitles(string pattern){
			commandProcessor.AssertTrue("assertAllWindowTitles", new String[] {pattern,});
		}
		public void assertNotAllWindowTitles(string pattern){
			commandProcessor.AssertTrue("assertNotAllWindowTitles", new String[] {pattern,});
		}
		public void waitForAllWindowTitles(string pattern){
			commandProcessor.DoCommand("waitForAllWindowTitles", new String[] {pattern,});
		}
		public void waitForNotAllWindowTitles(string pattern){
			commandProcessor.DoCommand("waitForNotAllWindowTitles", new String[] {pattern,});
		}
		public void storeAllWindowTitles(string variableName){
			commandProcessor.DoCommand("storeAllWindowTitles", new String[] {variableName,});
		}
		public string getAttribute(string attributeLocator){
			return commandProcessor.GetString("getAttribute", new String[] {attributeLocator,});
		}
		public void verifyAttribute(string attributeLocator,string pattern){
			commandProcessor.VerifyTrue("verifyAttribute", new String[] {attributeLocator,pattern,});
		}
		public void verifyNotAttribute(string attributeLocator,string pattern){
			commandProcessor.VerifyTrue("verifyNotAttribute", new String[] {attributeLocator,pattern,});
		}
		public void assertAttribute(string attributeLocator,string pattern){
			commandProcessor.AssertTrue("assertAttribute", new String[] {attributeLocator,pattern,});
		}
		public void assertNotAttribute(string attributeLocator,string pattern){
			commandProcessor.AssertTrue("assertNotAttribute", new String[] {attributeLocator,pattern,});
		}
		public void waitForAttribute(string attributeLocator,string pattern){
			commandProcessor.DoCommand("waitForAttribute", new String[] {attributeLocator,pattern,});
		}
		public void waitForNotAttribute(string attributeLocator,string pattern){
			commandProcessor.DoCommand("waitForNotAttribute", new String[] {attributeLocator,pattern,});
		}
		public void storeAttribute(string attributeLocator,string variableName){
			commandProcessor.DoCommand("storeAttribute", new String[] {attributeLocator,variableName,});
		}
		public string[] getAttributeFromAllWindows(string attributeName){
			return commandProcessor.GetStringArray("getAttributeFromAllWindows", new String[] {attributeName,});
		}
		public void verifyAttributeFromAllWindows(string attributeName,string pattern){
			commandProcessor.VerifyTrue("verifyAttributeFromAllWindows", new String[] {attributeName,pattern,});
		}
		public void verifyNotAttributeFromAllWindows(string attributeName,string pattern){
			commandProcessor.VerifyTrue("verifyNotAttributeFromAllWindows", new String[] {attributeName,pattern,});
		}
		public void assertAttributeFromAllWindows(string attributeName,string pattern){
			commandProcessor.AssertTrue("assertAttributeFromAllWindows", new String[] {attributeName,pattern,});
		}
		public void assertNotAttributeFromAllWindows(string attributeName,string pattern){
			commandProcessor.AssertTrue("assertNotAttributeFromAllWindows", new String[] {attributeName,pattern,});
		}
		public void waitForAttributeFromAllWindows(string attributeName,string pattern){
			commandProcessor.DoCommand("waitForAttributeFromAllWindows", new String[] {attributeName,pattern,});
		}
		public void waitForNotAttributeFromAllWindows(string attributeName,string pattern){
			commandProcessor.DoCommand("waitForNotAttributeFromAllWindows", new String[] {attributeName,pattern,});
		}
		public void storeAttributeFromAllWindows(string attributeName,string variableName){
			commandProcessor.DoCommand("storeAttributeFromAllWindows", new String[] {attributeName,variableName,});
		}
		public string getBodyText(){
			return commandProcessor.GetString("getBodyText", new String[] {});
		}
		public void verifyBodyText(string pattern){
			commandProcessor.VerifyTrue("verifyBodyText", new String[] {pattern,});
		}
		public void verifyNotBodyText(string pattern){
			commandProcessor.VerifyTrue("verifyNotBodyText", new String[] {pattern,});
		}
		public void assertBodyText(string pattern){
			commandProcessor.AssertTrue("assertBodyText", new String[] {pattern,});
		}
		public void assertNotBodyText(string pattern){
			commandProcessor.AssertTrue("assertNotBodyText", new String[] {pattern,});
		}
		public void waitForBodyText(string pattern){
			commandProcessor.DoCommand("waitForBodyText", new String[] {pattern,});
		}
		public void waitForNotBodyText(string pattern){
			commandProcessor.DoCommand("waitForNotBodyText", new String[] {pattern,});
		}
		public void storeBodyText(string variableName){
			commandProcessor.DoCommand("storeBodyText", new String[] {variableName,});
		}
		public string getConfirmation(){
			return commandProcessor.GetString("getConfirmation", new String[] {});
		}
		public void verifyConfirmation(string pattern){
			commandProcessor.VerifyTrue("verifyConfirmation", new String[] {pattern,});
		}
		public void verifyNotConfirmation(string pattern){
			commandProcessor.VerifyTrue("verifyNotConfirmation", new String[] {pattern,});
		}
		public void assertConfirmation(string pattern){
			commandProcessor.AssertTrue("assertConfirmation", new String[] {pattern,});
		}
		public void assertNotConfirmation(string pattern){
			commandProcessor.AssertTrue("assertNotConfirmation", new String[] {pattern,});
		}
		public void waitForConfirmation(string pattern){
			commandProcessor.DoCommand("waitForConfirmation", new String[] {pattern,});
		}
		public void waitForNotConfirmation(string pattern){
			commandProcessor.DoCommand("waitForNotConfirmation", new String[] {pattern,});
		}
		public void storeConfirmation(string variableName){
			commandProcessor.DoCommand("storeConfirmation", new String[] {variableName,});
		}
		public string getCookie(){
			return commandProcessor.GetString("getCookie", new String[] {});
		}
		public void verifyCookie(string pattern){
			commandProcessor.VerifyTrue("verifyCookie", new String[] {pattern,});
		}
		public void verifyNotCookie(string pattern){
			commandProcessor.VerifyTrue("verifyNotCookie", new String[] {pattern,});
		}
		public void assertCookie(string pattern){
			commandProcessor.AssertTrue("assertCookie", new String[] {pattern,});
		}
		public void assertNotCookie(string pattern){
			commandProcessor.AssertTrue("assertNotCookie", new String[] {pattern,});
		}
		public void waitForCookie(string pattern){
			commandProcessor.DoCommand("waitForCookie", new String[] {pattern,});
		}
		public void waitForNotCookie(string pattern){
			commandProcessor.DoCommand("waitForNotCookie", new String[] {pattern,});
		}
		public void storeCookie(string variableName){
			commandProcessor.DoCommand("storeCookie", new String[] {variableName,});
		}
		public string getCookieByName(string name){
			return commandProcessor.GetString("getCookieByName", new String[] {name,});
		}
		public void verifyCookieByName(string name,string pattern){
			commandProcessor.VerifyTrue("verifyCookieByName", new String[] {name,pattern,});
		}
		public void verifyNotCookieByName(string name,string pattern){
			commandProcessor.VerifyTrue("verifyNotCookieByName", new String[] {name,pattern,});
		}
		public void assertCookieByName(string name,string pattern){
			commandProcessor.AssertTrue("assertCookieByName", new String[] {name,pattern,});
		}
		public void assertNotCookieByName(string name,string pattern){
			commandProcessor.AssertTrue("assertNotCookieByName", new String[] {name,pattern,});
		}
		public void waitForCookieByName(string name,string pattern){
			commandProcessor.DoCommand("waitForCookieByName", new String[] {name,pattern,});
		}
		public void waitForNotCookieByName(string name,string pattern){
			commandProcessor.DoCommand("waitForNotCookieByName", new String[] {name,pattern,});
		}
		public void storeCookieByName(string name,string variableName){
			commandProcessor.DoCommand("storeCookieByName", new String[] {name,variableName,});
		}
		public decimal getCursorPosition(string locator){
			return commandProcessor.GetNumber("getCursorPosition", new String[] {locator,});
		}
		public void verifyCursorPosition(string locator,string pattern){
			commandProcessor.VerifyTrue("verifyCursorPosition", new String[] {locator,pattern,});
		}
		public void verifyNotCursorPosition(string locator,string pattern){
			commandProcessor.VerifyTrue("verifyNotCursorPosition", new String[] {locator,pattern,});
		}
		public void assertCursorPosition(string locator,string pattern){
			commandProcessor.AssertTrue("assertCursorPosition", new String[] {locator,pattern,});
		}
		public void assertNotCursorPosition(string locator,string pattern){
			commandProcessor.AssertTrue("assertNotCursorPosition", new String[] {locator,pattern,});
		}
		public void waitForCursorPosition(string locator,string pattern){
			commandProcessor.DoCommand("waitForCursorPosition", new String[] {locator,pattern,});
		}
		public void waitForNotCursorPosition(string locator,string pattern){
			commandProcessor.DoCommand("waitForNotCursorPosition", new String[] {locator,pattern,});
		}
		public void storeCursorPosition(string locator,string variableName){
			commandProcessor.DoCommand("storeCursorPosition", new String[] {locator,variableName,});
		}
		public decimal getElementHeight(string locator){
			return commandProcessor.GetNumber("getElementHeight", new String[] {locator,});
		}
		public void verifyElementHeight(string locator,string pattern){
			commandProcessor.VerifyTrue("verifyElementHeight", new String[] {locator,pattern,});
		}
		public void verifyNotElementHeight(string locator,string pattern){
			commandProcessor.VerifyTrue("verifyNotElementHeight", new String[] {locator,pattern,});
		}
		public void assertElementHeight(string locator,string pattern){
			commandProcessor.AssertTrue("assertElementHeight", new String[] {locator,pattern,});
		}
		public void assertNotElementHeight(string locator,string pattern){
			commandProcessor.AssertTrue("assertNotElementHeight", new String[] {locator,pattern,});
		}
		public void waitForElementHeight(string locator,string pattern){
			commandProcessor.DoCommand("waitForElementHeight", new String[] {locator,pattern,});
		}
		public void waitForNotElementHeight(string locator,string pattern){
			commandProcessor.DoCommand("waitForNotElementHeight", new String[] {locator,pattern,});
		}
		public void storeElementHeight(string locator,string variableName){
			commandProcessor.DoCommand("storeElementHeight", new String[] {locator,variableName,});
		}
		public decimal getElementIndex(string locator){
			return commandProcessor.GetNumber("getElementIndex", new String[] {locator,});
		}
		public void verifyElementIndex(string locator,string pattern){
			commandProcessor.VerifyTrue("verifyElementIndex", new String[] {locator,pattern,});
		}
		public void verifyNotElementIndex(string locator,string pattern){
			commandProcessor.VerifyTrue("verifyNotElementIndex", new String[] {locator,pattern,});
		}
		public void assertElementIndex(string locator,string pattern){
			commandProcessor.AssertTrue("assertElementIndex", new String[] {locator,pattern,});
		}
		public void assertNotElementIndex(string locator,string pattern){
			commandProcessor.AssertTrue("assertNotElementIndex", new String[] {locator,pattern,});
		}
		public void waitForElementIndex(string locator,string pattern){
			commandProcessor.DoCommand("waitForElementIndex", new String[] {locator,pattern,});
		}
		public void waitForNotElementIndex(string locator,string pattern){
			commandProcessor.DoCommand("waitForNotElementIndex", new String[] {locator,pattern,});
		}
		public void storeElementIndex(string locator,string variableName){
			commandProcessor.DoCommand("storeElementIndex", new String[] {locator,variableName,});
		}
		public decimal getElementPositionLeft(string locator){
			return commandProcessor.GetNumber("getElementPositionLeft", new String[] {locator,});
		}
		public void verifyElementPositionLeft(string locator,string pattern){
			commandProcessor.VerifyTrue("verifyElementPositionLeft", new String[] {locator,pattern,});
		}
		public void verifyNotElementPositionLeft(string locator,string pattern){
			commandProcessor.VerifyTrue("verifyNotElementPositionLeft", new String[] {locator,pattern,});
		}
		public void assertElementPositionLeft(string locator,string pattern){
			commandProcessor.AssertTrue("assertElementPositionLeft", new String[] {locator,pattern,});
		}
		public void assertNotElementPositionLeft(string locator,string pattern){
			commandProcessor.AssertTrue("assertNotElementPositionLeft", new String[] {locator,pattern,});
		}
		public void waitForElementPositionLeft(string locator,string pattern){
			commandProcessor.DoCommand("waitForElementPositionLeft", new String[] {locator,pattern,});
		}
		public void waitForNotElementPositionLeft(string locator,string pattern){
			commandProcessor.DoCommand("waitForNotElementPositionLeft", new String[] {locator,pattern,});
		}
		public void storeElementPositionLeft(string locator,string variableName){
			commandProcessor.DoCommand("storeElementPositionLeft", new String[] {locator,variableName,});
		}
		public decimal getElementPositionTop(string locator){
			return commandProcessor.GetNumber("getElementPositionTop", new String[] {locator,});
		}
		public void verifyElementPositionTop(string locator,string pattern){
			commandProcessor.VerifyTrue("verifyElementPositionTop", new String[] {locator,pattern,});
		}
		public void verifyNotElementPositionTop(string locator,string pattern){
			commandProcessor.VerifyTrue("verifyNotElementPositionTop", new String[] {locator,pattern,});
		}
		public void assertElementPositionTop(string locator,string pattern){
			commandProcessor.AssertTrue("assertElementPositionTop", new String[] {locator,pattern,});
		}
		public void assertNotElementPositionTop(string locator,string pattern){
			commandProcessor.AssertTrue("assertNotElementPositionTop", new String[] {locator,pattern,});
		}
		public void waitForElementPositionTop(string locator,string pattern){
			commandProcessor.DoCommand("waitForElementPositionTop", new String[] {locator,pattern,});
		}
		public void waitForNotElementPositionTop(string locator,string pattern){
			commandProcessor.DoCommand("waitForNotElementPositionTop", new String[] {locator,pattern,});
		}
		public void storeElementPositionTop(string locator,string variableName){
			commandProcessor.DoCommand("storeElementPositionTop", new String[] {locator,variableName,});
		}
		public decimal getElementWidth(string locator){
			return commandProcessor.GetNumber("getElementWidth", new String[] {locator,});
		}
		public void verifyElementWidth(string locator,string pattern){
			commandProcessor.VerifyTrue("verifyElementWidth", new String[] {locator,pattern,});
		}
		public void verifyNotElementWidth(string locator,string pattern){
			commandProcessor.VerifyTrue("verifyNotElementWidth", new String[] {locator,pattern,});
		}
		public void assertElementWidth(string locator,string pattern){
			commandProcessor.AssertTrue("assertElementWidth", new String[] {locator,pattern,});
		}
		public void assertNotElementWidth(string locator,string pattern){
			commandProcessor.AssertTrue("assertNotElementWidth", new String[] {locator,pattern,});
		}
		public void waitForElementWidth(string locator,string pattern){
			commandProcessor.DoCommand("waitForElementWidth", new String[] {locator,pattern,});
		}
		public void waitForNotElementWidth(string locator,string pattern){
			commandProcessor.DoCommand("waitForNotElementWidth", new String[] {locator,pattern,});
		}
		public void storeElementWidth(string locator,string variableName){
			commandProcessor.DoCommand("storeElementWidth", new String[] {locator,variableName,});
		}
		public string getEval(string script){
			return commandProcessor.GetString("getEval", new String[] {script,});
		}
		public void verifyEval(string script,string pattern){
			commandProcessor.VerifyTrue("verifyEval", new String[] {script,pattern,});
		}
		public void verifyNotEval(string script,string pattern){
			commandProcessor.VerifyTrue("verifyNotEval", new String[] {script,pattern,});
		}
		public void assertEval(string script,string pattern){
			commandProcessor.AssertTrue("assertEval", new String[] {script,pattern,});
		}
		public void assertNotEval(string script,string pattern){
			commandProcessor.AssertTrue("assertNotEval", new String[] {script,pattern,});
		}
		public void waitForEval(string script,string pattern){
			commandProcessor.DoCommand("waitForEval", new String[] {script,pattern,});
		}
		public void waitForNotEval(string script,string pattern){
			commandProcessor.DoCommand("waitForNotEval", new String[] {script,pattern,});
		}
		public void storeEval(string script,string variableName){
			commandProcessor.DoCommand("storeEval", new String[] {script,variableName,});
		}
		public string getExpression(string expression){
			return commandProcessor.GetString("getExpression", new String[] {expression,});
		}
		public void verifyExpression(string expression,string pattern){
			commandProcessor.VerifyTrue("verifyExpression", new String[] {expression,pattern,});
		}
		public void verifyNotExpression(string expression,string pattern){
			commandProcessor.VerifyTrue("verifyNotExpression", new String[] {expression,pattern,});
		}
		public void assertExpression(string expression,string pattern){
			commandProcessor.AssertTrue("assertExpression", new String[] {expression,pattern,});
		}
		public void assertNotExpression(string expression,string pattern){
			commandProcessor.AssertTrue("assertNotExpression", new String[] {expression,pattern,});
		}
		public void waitForExpression(string expression,string pattern){
			commandProcessor.DoCommand("waitForExpression", new String[] {expression,pattern,});
		}
		public void waitForNotExpression(string expression,string pattern){
			commandProcessor.DoCommand("waitForNotExpression", new String[] {expression,pattern,});
		}
		public void storeExpression(string expression,string variableName){
			commandProcessor.DoCommand("storeExpression", new String[] {expression,variableName,});
		}
		public string getHtmlSource(){
			return commandProcessor.GetString("getHtmlSource", new String[] {});
		}
		public void verifyHtmlSource(string pattern){
			commandProcessor.VerifyTrue("verifyHtmlSource", new String[] {pattern,});
		}
		public void verifyNotHtmlSource(string pattern){
			commandProcessor.VerifyTrue("verifyNotHtmlSource", new String[] {pattern,});
		}
		public void assertHtmlSource(string pattern){
			commandProcessor.AssertTrue("assertHtmlSource", new String[] {pattern,});
		}
		public void assertNotHtmlSource(string pattern){
			commandProcessor.AssertTrue("assertNotHtmlSource", new String[] {pattern,});
		}
		public void waitForHtmlSource(string pattern){
			commandProcessor.DoCommand("waitForHtmlSource", new String[] {pattern,});
		}
		public void waitForNotHtmlSource(string pattern){
			commandProcessor.DoCommand("waitForNotHtmlSource", new String[] {pattern,});
		}
		public void storeHtmlSource(string variableName){
			commandProcessor.DoCommand("storeHtmlSource", new String[] {variableName,});
		}
		public string getLocation(){
			return commandProcessor.GetString("getLocation", new String[] {});
		}
		public void verifyLocation(string pattern){
			commandProcessor.VerifyTrue("verifyLocation", new String[] {pattern,});
		}
		public void verifyNotLocation(string pattern){
			commandProcessor.VerifyTrue("verifyNotLocation", new String[] {pattern,});
		}
		public void assertLocation(string pattern){
			commandProcessor.AssertTrue("assertLocation", new String[] {pattern,});
		}
		public void assertNotLocation(string pattern){
			commandProcessor.AssertTrue("assertNotLocation", new String[] {pattern,});
		}
		public void waitForLocation(string pattern){
			commandProcessor.DoCommand("waitForLocation", new String[] {pattern,});
		}
		public void waitForNotLocation(string pattern){
			commandProcessor.DoCommand("waitForNotLocation", new String[] {pattern,});
		}
		public void storeLocation(string variableName){
			commandProcessor.DoCommand("storeLocation", new String[] {variableName,});
		}
		public string getLog(){
			return commandProcessor.GetString("getLog", new String[] {});
		}
		public void verifyLog(string pattern){
			commandProcessor.VerifyTrue("verifyLog", new String[] {pattern,});
		}
		public void verifyNotLog(string pattern){
			commandProcessor.VerifyTrue("verifyNotLog", new String[] {pattern,});
		}
		public void assertLog(string pattern){
			commandProcessor.AssertTrue("assertLog", new String[] {pattern,});
		}
		public void assertNotLog(string pattern){
			commandProcessor.AssertTrue("assertNotLog", new String[] {pattern,});
		}
		public void waitForLog(string pattern){
			commandProcessor.DoCommand("waitForLog", new String[] {pattern,});
		}
		public void waitForNotLog(string pattern){
			commandProcessor.DoCommand("waitForNotLog", new String[] {pattern,});
		}
		public void storeLog(string variableName){
			commandProcessor.DoCommand("storeLog", new String[] {variableName,});
		}
		public decimal getMouseSpeed(){
			return commandProcessor.GetNumber("getMouseSpeed", new String[] {});
		}
		public void verifyMouseSpeed(string pattern){
			commandProcessor.VerifyTrue("verifyMouseSpeed", new String[] {pattern,});
		}
		public void verifyNotMouseSpeed(string pattern){
			commandProcessor.VerifyTrue("verifyNotMouseSpeed", new String[] {pattern,});
		}
		public void assertMouseSpeed(string pattern){
			commandProcessor.AssertTrue("assertMouseSpeed", new String[] {pattern,});
		}
		public void assertNotMouseSpeed(string pattern){
			commandProcessor.AssertTrue("assertNotMouseSpeed", new String[] {pattern,});
		}
		public void waitForMouseSpeed(string pattern){
			commandProcessor.DoCommand("waitForMouseSpeed", new String[] {pattern,});
		}
		public void waitForNotMouseSpeed(string pattern){
			commandProcessor.DoCommand("waitForNotMouseSpeed", new String[] {pattern,});
		}
		public void storeMouseSpeed(string variableName){
			commandProcessor.DoCommand("storeMouseSpeed", new String[] {variableName,});
		}
		public string getPrompt(){
			return commandProcessor.GetString("getPrompt", new String[] {});
		}
		public void verifyPrompt(string pattern){
			commandProcessor.VerifyTrue("verifyPrompt", new String[] {pattern,});
		}
		public void verifyNotPrompt(string pattern){
			commandProcessor.VerifyTrue("verifyNotPrompt", new String[] {pattern,});
		}
		public void assertPrompt(string pattern){
			commandProcessor.AssertTrue("assertPrompt", new String[] {pattern,});
		}
		public void assertNotPrompt(string pattern){
			commandProcessor.AssertTrue("assertNotPrompt", new String[] {pattern,});
		}
		public void waitForPrompt(string pattern){
			commandProcessor.DoCommand("waitForPrompt", new String[] {pattern,});
		}
		public void waitForNotPrompt(string pattern){
			commandProcessor.DoCommand("waitForNotPrompt", new String[] {pattern,});
		}
		public void storePrompt(string variableName){
			commandProcessor.DoCommand("storePrompt", new String[] {variableName,});
		}
		public string getSelectedId(string selectLocator){
			return commandProcessor.GetString("getSelectedId", new String[] {selectLocator,});
		}
		public void verifySelectedId(string selectLocator,string pattern){
			commandProcessor.VerifyTrue("verifySelectedId", new String[] {selectLocator,pattern,});
		}
		public void verifyNotSelectedId(string selectLocator,string pattern){
			commandProcessor.VerifyTrue("verifyNotSelectedId", new String[] {selectLocator,pattern,});
		}
		public void assertSelectedId(string selectLocator,string pattern){
			commandProcessor.AssertTrue("assertSelectedId", new String[] {selectLocator,pattern,});
		}
		public void assertNotSelectedId(string selectLocator,string pattern){
			commandProcessor.AssertTrue("assertNotSelectedId", new String[] {selectLocator,pattern,});
		}
		public void waitForSelectedId(string selectLocator,string pattern){
			commandProcessor.DoCommand("waitForSelectedId", new String[] {selectLocator,pattern,});
		}
		public void waitForNotSelectedId(string selectLocator,string pattern){
			commandProcessor.DoCommand("waitForNotSelectedId", new String[] {selectLocator,pattern,});
		}
		public void storeSelectedId(string selectLocator,string variableName){
			commandProcessor.DoCommand("storeSelectedId", new String[] {selectLocator,variableName,});
		}
		public string[] getSelectedIds(string selectLocator){
			return commandProcessor.GetStringArray("getSelectedIds", new String[] {selectLocator,});
		}
		public void verifySelectedIds(string selectLocator,string pattern){
			commandProcessor.VerifyTrue("verifySelectedIds", new String[] {selectLocator,pattern,});
		}
		public void verifyNotSelectedIds(string selectLocator,string pattern){
			commandProcessor.VerifyTrue("verifyNotSelectedIds", new String[] {selectLocator,pattern,});
		}
		public void assertSelectedIds(string selectLocator,string pattern){
			commandProcessor.AssertTrue("assertSelectedIds", new String[] {selectLocator,pattern,});
		}
		public void assertNotSelectedIds(string selectLocator,string pattern){
			commandProcessor.AssertTrue("assertNotSelectedIds", new String[] {selectLocator,pattern,});
		}
		public void waitForSelectedIds(string selectLocator,string pattern){
			commandProcessor.DoCommand("waitForSelectedIds", new String[] {selectLocator,pattern,});
		}
		public void waitForNotSelectedIds(string selectLocator,string pattern){
			commandProcessor.DoCommand("waitForNotSelectedIds", new String[] {selectLocator,pattern,});
		}
		public void storeSelectedIds(string selectLocator,string variableName){
			commandProcessor.DoCommand("storeSelectedIds", new String[] {selectLocator,variableName,});
		}
		public string getSelectedIndex(string selectLocator){
			return commandProcessor.GetString("getSelectedIndex", new String[] {selectLocator,});
		}
		public void verifySelectedIndex(string selectLocator,string pattern){
			commandProcessor.VerifyTrue("verifySelectedIndex", new String[] {selectLocator,pattern,});
		}
		public void verifyNotSelectedIndex(string selectLocator,string pattern){
			commandProcessor.VerifyTrue("verifyNotSelectedIndex", new String[] {selectLocator,pattern,});
		}
		public void assertSelectedIndex(string selectLocator,string pattern){
			commandProcessor.AssertTrue("assertSelectedIndex", new String[] {selectLocator,pattern,});
		}
		public void assertNotSelectedIndex(string selectLocator,string pattern){
			commandProcessor.AssertTrue("assertNotSelectedIndex", new String[] {selectLocator,pattern,});
		}
		public void waitForSelectedIndex(string selectLocator,string pattern){
			commandProcessor.DoCommand("waitForSelectedIndex", new String[] {selectLocator,pattern,});
		}
		public void waitForNotSelectedIndex(string selectLocator,string pattern){
			commandProcessor.DoCommand("waitForNotSelectedIndex", new String[] {selectLocator,pattern,});
		}
		public void storeSelectedIndex(string selectLocator,string variableName){
			commandProcessor.DoCommand("storeSelectedIndex", new String[] {selectLocator,variableName,});
		}
		public string[] getSelectedIndexes(string selectLocator){
			return commandProcessor.GetStringArray("getSelectedIndexes", new String[] {selectLocator,});
		}
		public void verifySelectedIndexes(string selectLocator,string pattern){
			commandProcessor.VerifyTrue("verifySelectedIndexes", new String[] {selectLocator,pattern,});
		}
		public void verifyNotSelectedIndexes(string selectLocator,string pattern){
			commandProcessor.VerifyTrue("verifyNotSelectedIndexes", new String[] {selectLocator,pattern,});
		}
		public void assertSelectedIndexes(string selectLocator,string pattern){
			commandProcessor.AssertTrue("assertSelectedIndexes", new String[] {selectLocator,pattern,});
		}
		public void assertNotSelectedIndexes(string selectLocator,string pattern){
			commandProcessor.AssertTrue("assertNotSelectedIndexes", new String[] {selectLocator,pattern,});
		}
		public void waitForSelectedIndexes(string selectLocator,string pattern){
			commandProcessor.DoCommand("waitForSelectedIndexes", new String[] {selectLocator,pattern,});
		}
		public void waitForNotSelectedIndexes(string selectLocator,string pattern){
			commandProcessor.DoCommand("waitForNotSelectedIndexes", new String[] {selectLocator,pattern,});
		}
		public void storeSelectedIndexes(string selectLocator,string variableName){
			commandProcessor.DoCommand("storeSelectedIndexes", new String[] {selectLocator,variableName,});
		}
		public string getSelectedLabel(string selectLocator){
			return commandProcessor.GetString("getSelectedLabel", new String[] {selectLocator,});
		}
		public void verifySelectedLabel(string selectLocator,string pattern){
			commandProcessor.VerifyTrue("verifySelectedLabel", new String[] {selectLocator,pattern,});
		}
		public void verifyNotSelectedLabel(string selectLocator,string pattern){
			commandProcessor.VerifyTrue("verifyNotSelectedLabel", new String[] {selectLocator,pattern,});
		}
		public void assertSelectedLabel(string selectLocator,string pattern){
			commandProcessor.AssertTrue("assertSelectedLabel", new String[] {selectLocator,pattern,});
		}
		public void assertNotSelectedLabel(string selectLocator,string pattern){
			commandProcessor.AssertTrue("assertNotSelectedLabel", new String[] {selectLocator,pattern,});
		}
		public void waitForSelectedLabel(string selectLocator,string pattern){
			commandProcessor.DoCommand("waitForSelectedLabel", new String[] {selectLocator,pattern,});
		}
		public void waitForNotSelectedLabel(string selectLocator,string pattern){
			commandProcessor.DoCommand("waitForNotSelectedLabel", new String[] {selectLocator,pattern,});
		}
		public void storeSelectedLabel(string selectLocator,string variableName){
			commandProcessor.DoCommand("storeSelectedLabel", new String[] {selectLocator,variableName,});
		}
		public string[] getSelectedLabels(string selectLocator){
			return commandProcessor.GetStringArray("getSelectedLabels", new String[] {selectLocator,});
		}
		public void verifySelectedLabels(string selectLocator,string pattern){
			commandProcessor.VerifyTrue("verifySelectedLabels", new String[] {selectLocator,pattern,});
		}
		public void verifyNotSelectedLabels(string selectLocator,string pattern){
			commandProcessor.VerifyTrue("verifyNotSelectedLabels", new String[] {selectLocator,pattern,});
		}
		public void assertSelectedLabels(string selectLocator,string pattern){
			commandProcessor.AssertTrue("assertSelectedLabels", new String[] {selectLocator,pattern,});
		}
		public void assertNotSelectedLabels(string selectLocator,string pattern){
			commandProcessor.AssertTrue("assertNotSelectedLabels", new String[] {selectLocator,pattern,});
		}
		public void waitForSelectedLabels(string selectLocator,string pattern){
			commandProcessor.DoCommand("waitForSelectedLabels", new String[] {selectLocator,pattern,});
		}
		public void waitForNotSelectedLabels(string selectLocator,string pattern){
			commandProcessor.DoCommand("waitForNotSelectedLabels", new String[] {selectLocator,pattern,});
		}
		public void storeSelectedLabels(string selectLocator,string variableName){
			commandProcessor.DoCommand("storeSelectedLabels", new String[] {selectLocator,variableName,});
		}
		public string getSelectedValue(string selectLocator){
			return commandProcessor.GetString("getSelectedValue", new String[] {selectLocator,});
		}
		public void verifySelectedValue(string selectLocator,string pattern){
			commandProcessor.VerifyTrue("verifySelectedValue", new String[] {selectLocator,pattern,});
		}
		public void verifyNotSelectedValue(string selectLocator,string pattern){
			commandProcessor.VerifyTrue("verifyNotSelectedValue", new String[] {selectLocator,pattern,});
		}
		public void assertSelectedValue(string selectLocator,string pattern){
			commandProcessor.AssertTrue("assertSelectedValue", new String[] {selectLocator,pattern,});
		}
		public void assertNotSelectedValue(string selectLocator,string pattern){
			commandProcessor.AssertTrue("assertNotSelectedValue", new String[] {selectLocator,pattern,});
		}
		public void waitForSelectedValue(string selectLocator,string pattern){
			commandProcessor.DoCommand("waitForSelectedValue", new String[] {selectLocator,pattern,});
		}
		public void waitForNotSelectedValue(string selectLocator,string pattern){
			commandProcessor.DoCommand("waitForNotSelectedValue", new String[] {selectLocator,pattern,});
		}
		public void storeSelectedValue(string selectLocator,string variableName){
			commandProcessor.DoCommand("storeSelectedValue", new String[] {selectLocator,variableName,});
		}
		public string[] getSelectedValues(string selectLocator){
			return commandProcessor.GetStringArray("getSelectedValues", new String[] {selectLocator,});
		}
		public void verifySelectedValues(string selectLocator,string pattern){
			commandProcessor.VerifyTrue("verifySelectedValues", new String[] {selectLocator,pattern,});
		}
		public void verifyNotSelectedValues(string selectLocator,string pattern){
			commandProcessor.VerifyTrue("verifyNotSelectedValues", new String[] {selectLocator,pattern,});
		}
		public void assertSelectedValues(string selectLocator,string pattern){
			commandProcessor.AssertTrue("assertSelectedValues", new String[] {selectLocator,pattern,});
		}
		public void assertNotSelectedValues(string selectLocator,string pattern){
			commandProcessor.AssertTrue("assertNotSelectedValues", new String[] {selectLocator,pattern,});
		}
		public void waitForSelectedValues(string selectLocator,string pattern){
			commandProcessor.DoCommand("waitForSelectedValues", new String[] {selectLocator,pattern,});
		}
		public void waitForNotSelectedValues(string selectLocator,string pattern){
			commandProcessor.DoCommand("waitForNotSelectedValues", new String[] {selectLocator,pattern,});
		}
		public void storeSelectedValues(string selectLocator,string variableName){
			commandProcessor.DoCommand("storeSelectedValues", new String[] {selectLocator,variableName,});
		}
		public string[] getSelectOptions(string selectLocator){
			return commandProcessor.GetStringArray("getSelectOptions", new String[] {selectLocator,});
		}
		public void verifySelectOptions(string selectLocator,string pattern){
			commandProcessor.VerifyTrue("verifySelectOptions", new String[] {selectLocator,pattern,});
		}
		public void verifyNotSelectOptions(string selectLocator,string pattern){
			commandProcessor.VerifyTrue("verifyNotSelectOptions", new String[] {selectLocator,pattern,});
		}
		public void assertSelectOptions(string selectLocator,string pattern){
			commandProcessor.AssertTrue("assertSelectOptions", new String[] {selectLocator,pattern,});
		}
		public void assertNotSelectOptions(string selectLocator,string pattern){
			commandProcessor.AssertTrue("assertNotSelectOptions", new String[] {selectLocator,pattern,});
		}
		public void waitForSelectOptions(string selectLocator,string pattern){
			commandProcessor.DoCommand("waitForSelectOptions", new String[] {selectLocator,pattern,});
		}
		public void waitForNotSelectOptions(string selectLocator,string pattern){
			commandProcessor.DoCommand("waitForNotSelectOptions", new String[] {selectLocator,pattern,});
		}
		public void storeSelectOptions(string selectLocator,string variableName){
			commandProcessor.DoCommand("storeSelectOptions", new String[] {selectLocator,variableName,});
		}
		public string getSpeed(){
			return commandProcessor.GetString("getSpeed", new String[] {});
		}
		public void verifySpeed(string pattern){
			commandProcessor.VerifyTrue("verifySpeed", new String[] {pattern,});
		}
		public void verifyNotSpeed(string pattern){
			commandProcessor.VerifyTrue("verifyNotSpeed", new String[] {pattern,});
		}
		public void assertSpeed(string pattern){
			commandProcessor.AssertTrue("assertSpeed", new String[] {pattern,});
		}
		public void assertNotSpeed(string pattern){
			commandProcessor.AssertTrue("assertNotSpeed", new String[] {pattern,});
		}
		public void waitForSpeed(string pattern){
			commandProcessor.DoCommand("waitForSpeed", new String[] {pattern,});
		}
		public void waitForNotSpeed(string pattern){
			commandProcessor.DoCommand("waitForNotSpeed", new String[] {pattern,});
		}
		public void storeSpeed(string variableName){
			commandProcessor.DoCommand("storeSpeed", new String[] {variableName,});
		}
		public string getTable(string tableCellAddress){
			return commandProcessor.GetString("getTable", new String[] {tableCellAddress,});
		}
		public void verifyTable(string tableCellAddress,string pattern){
			commandProcessor.VerifyTrue("verifyTable", new String[] {tableCellAddress,pattern,});
		}
		public void verifyNotTable(string tableCellAddress,string pattern){
			commandProcessor.VerifyTrue("verifyNotTable", new String[] {tableCellAddress,pattern,});
		}
		public void assertTable(string tableCellAddress,string pattern){
			commandProcessor.AssertTrue("assertTable", new String[] {tableCellAddress,pattern,});
		}
		public void assertNotTable(string tableCellAddress,string pattern){
			commandProcessor.AssertTrue("assertNotTable", new String[] {tableCellAddress,pattern,});
		}
		public void waitForTable(string tableCellAddress,string pattern){
			commandProcessor.DoCommand("waitForTable", new String[] {tableCellAddress,pattern,});
		}
		public void waitForNotTable(string tableCellAddress,string pattern){
			commandProcessor.DoCommand("waitForNotTable", new String[] {tableCellAddress,pattern,});
		}
		public void storeTable(string tableCellAddress,string variableName){
			commandProcessor.DoCommand("storeTable", new String[] {tableCellAddress,variableName,});
		}
		public string getText(string locator){
			return commandProcessor.GetString("getText", new String[] {locator,});
		}
		public void verifyText(string locator,string pattern){
			commandProcessor.VerifyTrue("verifyText", new String[] {locator,pattern,});
		}
		public void verifyNotText(string locator,string pattern){
			commandProcessor.VerifyTrue("verifyNotText", new String[] {locator,pattern,});
		}
		public void assertText(string locator,string pattern){
			commandProcessor.AssertTrue("assertText", new String[] {locator,pattern,});
		}
		public void assertNotText(string locator,string pattern){
			commandProcessor.AssertTrue("assertNotText", new String[] {locator,pattern,});
		}
		public void waitForText(string locator,string pattern){
			commandProcessor.DoCommand("waitForText", new String[] {locator,pattern,});
		}
		public void waitForNotText(string locator,string pattern){
			commandProcessor.DoCommand("waitForNotText", new String[] {locator,pattern,});
		}
		public void storeText(string locator,string variableName){
			commandProcessor.DoCommand("storeText", new String[] {locator,variableName,});
		}
		public string getTitle(){
			return commandProcessor.GetString("getTitle", new String[] {});
		}
		public void verifyTitle(string pattern){
			commandProcessor.VerifyTrue("verifyTitle", new String[] {pattern,});
		}
		public void verifyNotTitle(string pattern){
			commandProcessor.VerifyTrue("verifyNotTitle", new String[] {pattern,});
		}
		public void assertTitle(string pattern){
			commandProcessor.AssertTrue("assertTitle", new String[] {pattern,});
		}
		public void assertNotTitle(string pattern){
			commandProcessor.AssertTrue("assertNotTitle", new String[] {pattern,});
		}
		public void waitForTitle(string pattern){
			commandProcessor.DoCommand("waitForTitle", new String[] {pattern,});
		}
		public void waitForNotTitle(string pattern){
			commandProcessor.DoCommand("waitForNotTitle", new String[] {pattern,});
		}
		public void storeTitle(string variableName){
			commandProcessor.DoCommand("storeTitle", new String[] {variableName,});
		}
		public string getValue(string locator){
			return commandProcessor.GetString("getValue", new String[] {locator,});
		}
		public void verifyValue(string locator,string pattern){
			commandProcessor.VerifyTrue("verifyValue", new String[] {locator,pattern,});
		}
		public void verifyNotValue(string locator,string pattern){
			commandProcessor.VerifyTrue("verifyNotValue", new String[] {locator,pattern,});
		}
		public void assertValue(string locator,string pattern){
			commandProcessor.AssertTrue("assertValue", new String[] {locator,pattern,});
		}
		public void assertNotValue(string locator,string pattern){
			commandProcessor.AssertTrue("assertNotValue", new String[] {locator,pattern,});
		}
		public void waitForValue(string locator,string pattern){
			commandProcessor.DoCommand("waitForValue", new String[] {locator,pattern,});
		}
		public void waitForNotValue(string locator,string pattern){
			commandProcessor.DoCommand("waitForNotValue", new String[] {locator,pattern,});
		}
		public void storeValue(string locator,string variableName){
			commandProcessor.DoCommand("storeValue", new String[] {locator,variableName,});
		}
		public bool getWhetherThisFrameMatchFrameExpression(string currentFrameString,string target){
			return commandProcessor.GetBoolean("getWhetherThisFrameMatchFrameExpression", new String[] {currentFrameString,target,});
		}
		public void verifyWhetherThisFrameMatchFrameExpression(string currentFrameString,string target){
			commandProcessor.VerifyTrue("verifyWhetherThisFrameMatchFrameExpression", new String[] {currentFrameString,target,});
		}
		public void verifyNotWhetherThisFrameMatchFrameExpression(string currentFrameString,string target){
			commandProcessor.VerifyTrue("verifyNotWhetherThisFrameMatchFrameExpression", new String[] {currentFrameString,target,});
		}
		public void assertWhetherThisFrameMatchFrameExpression(string currentFrameString,string target){
			commandProcessor.AssertTrue("assertWhetherThisFrameMatchFrameExpression", new String[] {currentFrameString,target,});
		}
		public void assertNotWhetherThisFrameMatchFrameExpression(string currentFrameString,string target){
			commandProcessor.AssertTrue("assertNotWhetherThisFrameMatchFrameExpression", new String[] {currentFrameString,target,});
		}
		public void waitForWhetherThisFrameMatchFrameExpression(string currentFrameString,string target){
			commandProcessor.DoCommand("waitForWhetherThisFrameMatchFrameExpression", new String[] {currentFrameString,target,});
		}
		public void waitForNotWhetherThisFrameMatchFrameExpression(string currentFrameString,string target){
			commandProcessor.DoCommand("waitForNotWhetherThisFrameMatchFrameExpression", new String[] {currentFrameString,target,});
		}
		public void storeWhetherThisFrameMatchFrameExpression(string currentFrameString,string target){
			commandProcessor.DoCommand("storeWhetherThisFrameMatchFrameExpression", new String[] {currentFrameString,target,});
		}
		public bool getWhetherThisWindowMatchWindowExpression(string currentWindowString,string target){
			return commandProcessor.GetBoolean("getWhetherThisWindowMatchWindowExpression", new String[] {currentWindowString,target,});
		}
		public void verifyWhetherThisWindowMatchWindowExpression(string currentWindowString,string target){
			commandProcessor.VerifyTrue("verifyWhetherThisWindowMatchWindowExpression", new String[] {currentWindowString,target,});
		}
		public void verifyNotWhetherThisWindowMatchWindowExpression(string currentWindowString,string target){
			commandProcessor.VerifyTrue("verifyNotWhetherThisWindowMatchWindowExpression", new String[] {currentWindowString,target,});
		}
		public void assertWhetherThisWindowMatchWindowExpression(string currentWindowString,string target){
			commandProcessor.AssertTrue("assertWhetherThisWindowMatchWindowExpression", new String[] {currentWindowString,target,});
		}
		public void assertNotWhetherThisWindowMatchWindowExpression(string currentWindowString,string target){
			commandProcessor.AssertTrue("assertNotWhetherThisWindowMatchWindowExpression", new String[] {currentWindowString,target,});
		}
		public void waitForWhetherThisWindowMatchWindowExpression(string currentWindowString,string target){
			commandProcessor.DoCommand("waitForWhetherThisWindowMatchWindowExpression", new String[] {currentWindowString,target,});
		}
		public void waitForNotWhetherThisWindowMatchWindowExpression(string currentWindowString,string target){
			commandProcessor.DoCommand("waitForNotWhetherThisWindowMatchWindowExpression", new String[] {currentWindowString,target,});
		}
		public void storeWhetherThisWindowMatchWindowExpression(string currentWindowString,string target){
			commandProcessor.DoCommand("storeWhetherThisWindowMatchWindowExpression", new String[] {currentWindowString,target,});
		}
		public decimal getXpathCount(string xpath){
			return commandProcessor.GetNumber("getXpathCount", new String[] {xpath,});
		}
		public void verifyXpathCount(string xpath,string pattern){
			commandProcessor.VerifyTrue("verifyXpathCount", new String[] {xpath,pattern,});
		}
		public void verifyNotXpathCount(string xpath,string pattern){
			commandProcessor.VerifyTrue("verifyNotXpathCount", new String[] {xpath,pattern,});
		}
		public void assertXpathCount(string xpath,string pattern){
			commandProcessor.AssertTrue("assertXpathCount", new String[] {xpath,pattern,});
		}
		public void assertNotXpathCount(string xpath,string pattern){
			commandProcessor.AssertTrue("assertNotXpathCount", new String[] {xpath,pattern,});
		}
		public void waitForXpathCount(string xpath,string pattern){
			commandProcessor.DoCommand("waitForXpathCount", new String[] {xpath,pattern,});
		}
		public void waitForNotXpathCount(string xpath,string pattern){
			commandProcessor.DoCommand("waitForNotXpathCount", new String[] {xpath,pattern,});
		}
		public void storeXpathCount(string xpath,string variableName){
			commandProcessor.DoCommand("storeXpathCount", new String[] {xpath,variableName,});
		}
		public decimal GetCSSCount(string cssLocator){
			return commandProcessor.GetNumber("getCssCount", new String[] {cssLocator,});
		}
		public void goBack(){
			commandProcessor.DoCommand("goBack", new String[] {});
		}
		public void goBackAndWait(){
			commandProcessor.DoCommand("goBackAndWait", new String[] {});
		}
		public void highlight(string locator){
			commandProcessor.DoCommand("highlight", new String[] {locator,});
		}
		public void highlightAndWait(string locator){
			commandProcessor.DoCommand("highlightAndWait", new String[] {locator,});
		}
		public void ignoreAttributesWithoutValue(string ignore){
			commandProcessor.DoCommand("ignoreAttributesWithoutValue", new String[] {ignore,});
		}
		public void ignoreAttributesWithoutValueAndWait(string ignore){
			commandProcessor.DoCommand("ignoreAttributesWithoutValueAndWait", new String[] {ignore,});
		}
		public bool isAlertPresent(){
			return commandProcessor.GetBoolean("isAlertPresent", new String[] {});
		}
		public void verifyAlertPresent(){
			commandProcessor.VerifyTrue("verifyAlertPresent", new String[] {});
		}
		public void verifyAlertNotPresent(){
			commandProcessor.VerifyTrue("verifyAlertNotPresent", new String[] {});
		}
		public void assertAlertPresent(){
			commandProcessor.AssertTrue("assertAlertPresent", new String[] {});
		}
		public void assertAlertNotPresent(){
			commandProcessor.AssertTrue("assertAlertNotPresent", new String[] {});
		}
		public void storeAlertPresent(string variableName){
			commandProcessor.DoCommand("storeAlertPresent", new String[] {variableName,});
		}
		public void waitForAlertPresent(){
			commandProcessor.DoCommand("waitForAlertPresent", new String[] {});
		}
		public void waitForNotAlertPresent(){
			commandProcessor.DoCommand("waitForNotAlertPresent", new String[] {});
		}
		public bool isChecked(string locator){
			return commandProcessor.GetBoolean("isChecked", new String[] {locator,});
		}
		public void verifyChecked(string locator){
			commandProcessor.VerifyTrue("verifyChecked", new String[] {locator,});
		}
		public void verifyNotChecked(string locator){
			commandProcessor.VerifyTrue("verifyNotChecked", new String[] {locator,});
		}
		public void assertChecked(string locator){
			commandProcessor.AssertTrue("assertChecked", new String[] {locator,});
		}
		public void assertNotChecked(string locator){
			commandProcessor.AssertTrue("assertNotChecked", new String[] {locator,});
		}
		public void storeChecked(string locator,string variableName){
			commandProcessor.DoCommand("storeChecked", new String[] {locator,variableName,});
		}
		public void waitForChecked(string locator){
			commandProcessor.DoCommand("waitForChecked", new String[] {locator,});
		}
		public void waitForNotChecked(string locator){
			commandProcessor.DoCommand("waitForNotChecked", new String[] {locator,});
		}
		public bool isConfirmationPresent(){
			return commandProcessor.GetBoolean("isConfirmationPresent", new String[] {});
		}
		public void verifyConfirmationPresent(){
			commandProcessor.VerifyTrue("verifyConfirmationPresent", new String[] {});
		}
		public void verifyConfirmationNotPresent(){
			commandProcessor.VerifyTrue("verifyConfirmationNotPresent", new String[] {});
		}
		public void assertConfirmationPresent(){
			commandProcessor.AssertTrue("assertConfirmationPresent", new String[] {});
		}
		public void assertConfirmationNotPresent(){
			commandProcessor.AssertTrue("assertConfirmationNotPresent", new String[] {});
		}
		public void storeConfirmationPresent(string variableName){
			commandProcessor.DoCommand("storeConfirmationPresent", new String[] {variableName,});
		}
		public void waitForConfirmationPresent(){
			commandProcessor.DoCommand("waitForConfirmationPresent", new String[] {});
		}
		public void waitForNotConfirmationPresent(){
			commandProcessor.DoCommand("waitForNotConfirmationPresent", new String[] {});
		}
		public bool isCookiePresent(string name){
			return commandProcessor.GetBoolean("isCookiePresent", new String[] {name,});
		}
		public void verifyCookiePresent(string name){
			commandProcessor.VerifyTrue("verifyCookiePresent", new String[] {name,});
		}
		public void verifyNotCookiePresent(string name){
			commandProcessor.VerifyTrue("verifyNotCookiePresent", new String[] {name,});
		}
		public void assertCookiePresent(string name){
			commandProcessor.AssertTrue("assertCookiePresent", new String[] {name,});
		}
		public void assertNotCookiePresent(string name){
			commandProcessor.AssertTrue("assertNotCookiePresent", new String[] {name,});
		}
		public void storeCookiePresent(string name,string variableName){
			commandProcessor.DoCommand("storeCookiePresent", new String[] {name,variableName,});
		}
		public void waitForCookiePresent(string name){
			commandProcessor.DoCommand("waitForCookiePresent", new String[] {name,});
		}
		public void waitForNotCookiePresent(string name){
			commandProcessor.DoCommand("waitForNotCookiePresent", new String[] {name,});
		}
		public bool isEditable(string locator){
			return commandProcessor.GetBoolean("isEditable", new String[] {locator,});
		}
		public void verifyEditable(string locator){
			commandProcessor.VerifyTrue("verifyEditable", new String[] {locator,});
		}
		public void verifyNotEditable(string locator){
			commandProcessor.VerifyTrue("verifyNotEditable", new String[] {locator,});
		}
		public void assertEditable(string locator){
			commandProcessor.AssertTrue("assertEditable", new String[] {locator,});
		}
		public void assertNotEditable(string locator){
			commandProcessor.AssertTrue("assertNotEditable", new String[] {locator,});
		}
		public void storeEditable(string locator,string variableName){
			commandProcessor.DoCommand("storeEditable", new String[] {locator,variableName,});
		}
		public void waitForEditable(string locator){
			commandProcessor.DoCommand("waitForEditable", new String[] {locator,});
		}
		public void waitForNotEditable(string locator){
			commandProcessor.DoCommand("waitForNotEditable", new String[] {locator,});
		}
		public bool isElementPresent(string locator){
			return commandProcessor.GetBoolean("isElementPresent", new String[] {locator,});
		}
		public void verifyElementPresent(string locator){
			commandProcessor.VerifyTrue("verifyElementPresent", new String[] {locator,});
		}
		public void verifyElementNotPresent(string locator){
			commandProcessor.VerifyTrue("verifyElementNotPresent", new String[] {locator,});
		}
		public void assertElementPresent(string locator){
			commandProcessor.AssertTrue("assertElementPresent", new String[] {locator,});
		}
		public void assertElementNotPresent(string locator){
			commandProcessor.AssertTrue("assertElementNotPresent", new String[] {locator,});
		}
		public void storeElementPresent(string locator,string variableName){
			commandProcessor.DoCommand("storeElementPresent", new String[] {locator,variableName,});
		}
		public void waitForElementPresent(string locator){
			commandProcessor.DoCommand("waitForElementPresent", new String[] {locator,});
		}
		public void waitForNotElementPresent(string locator){
			commandProcessor.DoCommand("waitForNotElementPresent", new String[] {locator,});
		}
		public bool isOrdered(string locator1,string locator2){
			return commandProcessor.GetBoolean("isOrdered", new String[] {locator1,locator2,});
		}
		public void verifyOrdered(string locator1,string locator2){
			commandProcessor.VerifyTrue("verifyOrdered", new String[] {locator1,locator2,});
		}
		public void verifyNotOrdered(string locator1,string locator2){
			commandProcessor.VerifyTrue("verifyNotOrdered", new String[] {locator1,locator2,});
		}
		public void assertOrdered(string locator1,string locator2){
			commandProcessor.AssertTrue("assertOrdered", new String[] {locator1,locator2,});
		}
		public void assertNotOrdered(string locator1,string locator2){
			commandProcessor.AssertTrue("assertNotOrdered", new String[] {locator1,locator2,});
		}
		public void storeOrdered(string locator1,string locator2){
			commandProcessor.DoCommand("storeOrdered", new String[] {locator1,locator2,});
		}
		public void waitForOrdered(string locator1,string locator2){
			commandProcessor.DoCommand("waitForOrdered", new String[] {locator1,locator2,});
		}
		public void waitForNotOrdered(string locator1,string locator2){
			commandProcessor.DoCommand("waitForNotOrdered", new String[] {locator1,locator2,});
		}
		public bool isPromptPresent(){
			return commandProcessor.GetBoolean("isPromptPresent", new String[] {});
		}
		public void verifyPromptPresent(){
			commandProcessor.VerifyTrue("verifyPromptPresent", new String[] {});
		}
		public void verifyNotPromptPresent(){
			commandProcessor.VerifyTrue("verifyNotPromptPresent", new String[] {});
		}
		public void assertPromptPresent(){
			commandProcessor.AssertTrue("assertPromptPresent", new String[] {});
		}
		public void assertNotPromptPresent(){
			commandProcessor.AssertTrue("assertNotPromptPresent", new String[] {});
		}
		public void storePromptPresent(string variableName){
			commandProcessor.DoCommand("storePromptPresent", new String[] {variableName,});
		}
		public void waitForPromptPresent(){
			commandProcessor.DoCommand("waitForPromptPresent", new String[] {});
		}
		public void waitForNotPromptPresent(){
			commandProcessor.DoCommand("waitForNotPromptPresent", new String[] {});
		}
		public bool isSomethingSelected(string selectLocator){
			return commandProcessor.GetBoolean("isSomethingSelected", new String[] {selectLocator,});
		}
		public void verifySomethingSelected(string selectLocator){
			commandProcessor.VerifyTrue("verifySomethingSelected", new String[] {selectLocator,});
		}
		public void verifyNotSomethingSelected(string selectLocator){
			commandProcessor.VerifyTrue("verifyNotSomethingSelected", new String[] {selectLocator,});
		}
		public void assertSomethingSelected(string selectLocator){
			commandProcessor.AssertTrue("assertSomethingSelected", new String[] {selectLocator,});
		}
		public void assertNotSomethingSelected(string selectLocator){
			commandProcessor.AssertTrue("assertNotSomethingSelected", new String[] {selectLocator,});
		}
		public void storeSomethingSelected(string selectLocator,string variableName){
			commandProcessor.DoCommand("storeSomethingSelected", new String[] {selectLocator,variableName,});
		}
		public void waitForSomethingSelected(string selectLocator){
			commandProcessor.DoCommand("waitForSomethingSelected", new String[] {selectLocator,});
		}
		public void waitForNotSomethingSelected(string selectLocator){
			commandProcessor.DoCommand("waitForNotSomethingSelected", new String[] {selectLocator,});
		}
		public bool isTextPresent(string pattern){
			return commandProcessor.GetBoolean("isTextPresent", new String[] {pattern,});
		}
		public void verifyTextPresent(string pattern){
			commandProcessor.VerifyTrue("verifyTextPresent", new String[] {pattern,});
		}
		public void verifyTextNotPresent(string pattern){
			commandProcessor.VerifyTrue("verifyTextNotPresent", new String[] {pattern,});
		}
		public void assertTextPresent(string pattern){
			commandProcessor.AssertTrue("assertTextPresent", new String[] {pattern,});
		}
		public void assertTextNotPresent(string pattern){
			commandProcessor.AssertTrue("assertTextNotPresent", new String[] {pattern,});
		}
		public void storeTextPresent(string pattern,string variableName){
			commandProcessor.DoCommand("storeTextPresent", new String[] {pattern,variableName,});
		}
		public void waitForTextPresent(string pattern){
			commandProcessor.DoCommand("waitForTextPresent", new String[] {pattern,});
		}
		public void waitForTextNotPresent(string pattern){
			commandProcessor.DoCommand("waitForTextNotPresent", new String[] {pattern,});
		}
		public bool isVisible(string locator){
			return commandProcessor.GetBoolean("isVisible", new String[] {locator,});
		}
		public void verifyVisible(string locator){
			commandProcessor.VerifyTrue("verifyVisible", new String[] {locator,});
		}
		public void verifyNotVisible(string locator){
			commandProcessor.VerifyTrue("verifyNotVisible", new String[] {locator,});
		}
		public void assertVisible(string locator){
			commandProcessor.AssertTrue("assertVisible", new String[] {locator,});
		}
		public void assertNotVisible(string locator){
			commandProcessor.AssertTrue("assertNotVisible", new String[] {locator,});
		}
		public void storeVisible(string locator,string variableName){
			commandProcessor.DoCommand("storeVisible", new String[] {locator,variableName,});
		}
		public void waitForVisible(string locator){
			commandProcessor.DoCommand("waitForVisible", new String[] {locator,});
		}
		public void waitForNotVisible(string locator){
			commandProcessor.DoCommand("waitForNotVisible", new String[] {locator,});
		}
		public void keyDown(string locator,string keySequence){
			commandProcessor.DoCommand("keyDown", new String[] {locator,keySequence,});
		}
		public void keyDownAndWait(string locator,string keySequence){
			commandProcessor.DoCommand("keyDownAndWait", new String[] {locator,keySequence,});
		}
		public void keyDownNative(string keycode){
			commandProcessor.DoCommand("keyDownNative", new String[] {keycode,});
		}
		public void keyDownNativeAndWait(string keycode){
			commandProcessor.DoCommand("keyDownNativeAndWait", new String[] {keycode,});
		}
		public void keyPress(string locator,string keySequence){
			commandProcessor.DoCommand("keyPress", new String[] {locator,keySequence,});
		}
		public void keyPressAndWait(string locator,string keySequence){
			commandProcessor.DoCommand("keyPressAndWait", new String[] {locator,keySequence,});
		}
		public void keyPressNative(string keycode){
			commandProcessor.DoCommand("keyPressNative", new String[] {keycode,});
		}
		public void keyPressNativeAndWait(string keycode){
			commandProcessor.DoCommand("keyPressNativeAndWait", new String[] {keycode,});
		}
		public void keyUp(string locator,string keySequence){
			commandProcessor.DoCommand("keyUp", new String[] {locator,keySequence,});
		}
		public void keyUpAndWait(string locator,string keySequence){
			commandProcessor.DoCommand("keyUpAndWait", new String[] {locator,keySequence,});
		}
		public void keyUpNative(string keycode){
			commandProcessor.DoCommand("keyUpNative", new String[] {keycode,});
		}
		public void keyUpNativeAndWait(string keycode){
			commandProcessor.DoCommand("keyUpNativeAndWait", new String[] {keycode,});
		}
		public void metaKeyDown(){
			commandProcessor.DoCommand("metaKeyDown", new String[] {});
		}
		public void metaKeyDownAndWait(){
			commandProcessor.DoCommand("metaKeyDownAndWait", new String[] {});
		}
		public void metaKeyUp(){
			commandProcessor.DoCommand("metaKeyUp", new String[] {});
		}
		public void metaKeyUpAndWait(){
			commandProcessor.DoCommand("metaKeyUpAndWait", new String[] {});
		}
		public void mouseDown(string locator){
			commandProcessor.DoCommand("mouseDown", new String[] {locator,});
		}
		public void mouseDownAndWait(string locator){
			commandProcessor.DoCommand("mouseDownAndWait", new String[] {locator,});
		}
		public void mouseDownAt(string locator,string coordString){
			commandProcessor.DoCommand("mouseDownAt", new String[] {locator,coordString,});
		}
		public void mouseDownAtAndWait(string locator,string coordString){
			commandProcessor.DoCommand("mouseDownAtAndWait", new String[] {locator,coordString,});
		}
		public void mouseDownRight(string locator){
			commandProcessor.DoCommand("mouseDownRight", new String[] {locator,});
		}
		public void mouseDownRightAndWait(string locator){
			commandProcessor.DoCommand("mouseDownRightAndWait", new String[] {locator,});
		}
		public void mouseDownRightAt(string locator,string coordString){
			commandProcessor.DoCommand("mouseDownRightAt", new String[] {locator,coordString,});
		}
		public void mouseDownRightAtAndWait(string locator,string coordString){
			commandProcessor.DoCommand("mouseDownRightAtAndWait", new String[] {locator,coordString,});
		}
		public void mouseMove(string locator){
			commandProcessor.DoCommand("mouseMove", new String[] {locator,});
		}
		public void mouseMoveAndWait(string locator){
			commandProcessor.DoCommand("mouseMoveAndWait", new String[] {locator,});
		}
		public void mouseMoveAt(string locator,string coordString){
			commandProcessor.DoCommand("mouseMoveAt", new String[] {locator,coordString,});
		}
		public void mouseMoveAtAndWait(string locator,string coordString){
			commandProcessor.DoCommand("mouseMoveAtAndWait", new String[] {locator,coordString,});
		}
		public void mouseOut(string locator){
			commandProcessor.DoCommand("mouseOut", new String[] {locator,});
		}
		public void mouseOutAndWait(string locator){
			commandProcessor.DoCommand("mouseOutAndWait", new String[] {locator,});
		}
		public void mouseOver(string locator){
			commandProcessor.DoCommand("mouseOver", new String[] {locator,});
		}
		public void mouseOverAndWait(string locator){
			commandProcessor.DoCommand("mouseOverAndWait", new String[] {locator,});
		}
		public void mouseUp(string locator){
			commandProcessor.DoCommand("mouseUp", new String[] {locator,});
		}
		public void mouseUpAndWait(string locator){
			commandProcessor.DoCommand("mouseUpAndWait", new String[] {locator,});
		}
		public void mouseUpAt(string locator,string coordString){
			commandProcessor.DoCommand("mouseUpAt", new String[] {locator,coordString,});
		}
		public void mouseUpAtAndWait(string locator,string coordString){
			commandProcessor.DoCommand("mouseUpAtAndWait", new String[] {locator,coordString,});
		}
		public void mouseUpRight(string locator){
			commandProcessor.DoCommand("mouseUpRight", new String[] {locator,});
		}
		public void mouseUpRightAndWait(string locator){
			commandProcessor.DoCommand("mouseUpRightAndWait", new String[] {locator,});
		}
		public void mouseUpRightAt(string locator,string coordString){
			commandProcessor.DoCommand("mouseUpRightAt", new String[] {locator,coordString,});
		}
		public void mouseUpRightAtAndWait(string locator,string coordString){
			commandProcessor.DoCommand("mouseUpRightAtAndWait", new String[] {locator,coordString,});
		}
		public void open(string url){
			commandProcessor.DoCommand("open", new String[] {url,});
		}
		public void openAndWait(string url){
			commandProcessor.DoCommand("openAndWait", new String[] {url,});
		}
		public void open(string url,string ignoreResponseCode){
			commandProcessor.DoCommand("open", new String[] {url,ignoreResponseCode,});
		}
		public void openAndWait(string url,string ignoreResponseCode){
			commandProcessor.DoCommand("openAndWait", new String[] {url,ignoreResponseCode,});
		}
		public void openWindow(string url,string windowID){
			commandProcessor.DoCommand("openWindow", new String[] {url,windowID,});
		}
		public void openWindowAndWait(string url,string windowID){
			commandProcessor.DoCommand("openWindowAndWait", new String[] {url,windowID,});
		}
		public void refresh(){
			commandProcessor.DoCommand("refresh", new String[] {});
		}
		public void refreshAndWait(){
			commandProcessor.DoCommand("refreshAndWait", new String[] {});
		}
		public void removeAllSelections(string locator){
			commandProcessor.DoCommand("removeAllSelections", new String[] {locator,});
		}
		public void removeAllSelectionsAndWait(string locator){
			commandProcessor.DoCommand("removeAllSelectionsAndWait", new String[] {locator,});
		}
		public void removeScript(string scriptTagId){
			commandProcessor.DoCommand("removeScript", new String[] {scriptTagId,});
		}
		public void removeScriptAndWait(string scriptTagId){
			commandProcessor.DoCommand("removeScriptAndWait", new String[] {scriptTagId,});
		}
		public void removeSelection(string locator,string optionLocator){
			commandProcessor.DoCommand("removeSelection", new String[] {locator,optionLocator,});
		}
		public void removeSelectionAndWait(string locator,string optionLocator){
			commandProcessor.DoCommand("removeSelectionAndWait", new String[] {locator,optionLocator,});
		}
		public string retrieveLastRemoteControlLogs(){
			return commandProcessor.GetString("retrieveLastRemoteControlLogs", new String[] {});
		}
		public void rollup(string rollupName,string kwargs){
			commandProcessor.DoCommand("rollup", new String[] {rollupName,kwargs,});
		}
		public void rollupAndWait(string rollupName,string kwargs){
			commandProcessor.DoCommand("rollupAndWait", new String[] {rollupName,kwargs,});
		}
		public void runScript(string script){
			commandProcessor.DoCommand("runScript", new String[] {script,});
		}
		public void runScriptAndWait(string script){
			commandProcessor.DoCommand("runScriptAndWait", new String[] {script,});
		}
		public void select(string selectLocator,string optionLocator){
			commandProcessor.DoCommand("select", new String[] {selectLocator,optionLocator,});
		}
		public void selectAndWait(string selectLocator,string optionLocator){
			commandProcessor.DoCommand("selectAndWait", new String[] {selectLocator,optionLocator,});
		}
		public void selectFrame(string locator){
			commandProcessor.DoCommand("selectFrame", new String[] {locator,});
		}
		public void selectFrameAndWait(string locator){
			commandProcessor.DoCommand("selectFrameAndWait", new String[] {locator,});
		}
		public void selectPopUp(string windowID){
			commandProcessor.DoCommand("selectPopUp", new String[] {windowID,});
		}
		public void selectPopUpAndWait(string windowID){
			commandProcessor.DoCommand("selectPopUpAndWait", new String[] {windowID,});
		}
		public void selectWindow(string windowID){
			commandProcessor.DoCommand("selectWindow", new String[] {windowID,});
		}
		public void selectWindowAndWait(string windowID){
			commandProcessor.DoCommand("selectWindowAndWait", new String[] {windowID,});
		}
		public void setBrowserLogLevel(string logLevel){
			commandProcessor.DoCommand("setBrowserLogLevel", new String[] {logLevel,});
		}
		public void setContext(string context){
			commandProcessor.DoCommand("setContext", new String[] {context,});
		}
		public void setCursorPosition(string locator,string position){
			commandProcessor.DoCommand("setCursorPosition", new String[] {locator,position,});
		}
		public void setMouseSpeed(string pixels){
			commandProcessor.DoCommand("setMouseSpeed", new String[] {pixels,});
		}
		public void setSpeed(string value){
			commandProcessor.DoCommand("setSpeed", new String[] {value,});
		}
		public void setTimeout(string timeout){
			commandProcessor.DoCommand("setTimeout", new String[] {timeout,});
		}
		public void shiftKeyDown(){
			commandProcessor.DoCommand("shiftKeyDown", new String[] {});
		}
		public void shiftKeyDownAndWait(){
			commandProcessor.DoCommand("shiftKeyDownAndWait", new String[] {});
		}
		public void shiftKeyUp(){
			commandProcessor.DoCommand("shiftKeyUp", new String[] {});
		}
		public void shiftKeyUpAndWait(){
			commandProcessor.DoCommand("shiftKeyUpAndWait", new String[] {});
		}
		public void showContextualBanner(){
			commandProcessor.DoCommand("showContextualBanner", new String[] {});
		}
		public void showContextualBannerAndWait(){
			commandProcessor.DoCommand("showContextualBannerAndWait", new String[] {});
		}
		public void showContextualBanner(string className,string methodName){
			commandProcessor.DoCommand("showContextualBanner", new String[] {className,methodName,});
		}
		public void showContextualBannerAndWait(string className,string methodName){
			commandProcessor.DoCommand("showContextualBannerAndWait", new String[] {className,methodName,});
		}
		public void shutDownSeleniumServer(){
			commandProcessor.DoCommand("shutDownSeleniumServer", new String[] {});
		}
		public void shutDownSeleniumServerAndWait(){
			commandProcessor.DoCommand("shutDownSeleniumServerAndWait", new String[] {});
		}
		public void submit(string formLocator){
			commandProcessor.DoCommand("submit", new String[] {formLocator,});
		}
		public void submitAndWait(string formLocator){
			commandProcessor.DoCommand("submitAndWait", new String[] {formLocator,});
		}
		public void type(string locator,string value){
			commandProcessor.DoCommand("type", new String[] {locator,value,});
		}
		public void typeAndWait(string locator,string value){
			commandProcessor.DoCommand("typeAndWait", new String[] {locator,value,});
		}
		public void typeKeys(string locator,string value){
			commandProcessor.DoCommand("typeKeys", new String[] {locator,value,});
		}
		public void typeKeysAndWait(string locator,string value){
			commandProcessor.DoCommand("typeKeysAndWait", new String[] {locator,value,});
		}
		public void uncheck(string locator){
			commandProcessor.DoCommand("uncheck", new String[] {locator,});
		}
		public void uncheckAndWait(string locator){
			commandProcessor.DoCommand("uncheckAndWait", new String[] {locator,});
		}
		public void useXpathLibrary(string libraryName){
			commandProcessor.DoCommand("useXpathLibrary", new String[] {libraryName,});
		}
		public void waitForCondition(string script,string timeout){
			commandProcessor.DoCommand("waitForCondition", new String[] {script,timeout,});
		}
		public void waitForFrameToLoad(string frameAddress,string timeout){
			commandProcessor.DoCommand("waitForFrameToLoad", new String[] {frameAddress,timeout,});
		}
		public void waitForPageToLoad(string timeout){
			commandProcessor.DoCommand("waitForPageToLoad", new String[] {timeout,});
		}
		public void waitForPopUp(string windowID,string timeout){
			commandProcessor.DoCommand("waitForPopUp", new String[] {windowID,timeout,});
		}
		public void windowFocus(){
			commandProcessor.DoCommand("windowFocus", new String[] {});
		}
		public void windowFocusAndWait(){
			commandProcessor.DoCommand("windowFocusAndWait", new String[] {});
		}
		public void windowMaximize(){
			commandProcessor.DoCommand("windowMaximize", new String[] {});
		}
		public void windowMaximizeAndWait(){
			commandProcessor.DoCommand("windowMaximizeAndWait", new String[] {});
		}
		public string[] getXpathAttribute(string xpath){
			return commandProcessor.GetStringArray("getXpathAttribute", new String[] {xpath,});
		}
		public void verifyXpathAttribute(string xpath,string pattern){
			commandProcessor.VerifyTrue("verifyXpathAttribute", new String[] {xpath,pattern,});
		}
		public void verifyNotXpathAttribute(string xpath,string pattern){
			commandProcessor.VerifyTrue("verifyNotXpathAttribute", new String[] {xpath,pattern,});
		}
		public void assertXpathAttribute(string xpath,string pattern){
			commandProcessor.AssertTrue("assertXpathAttribute", new String[] {xpath,pattern,});
		}
		public void assertNotXpathAttribute(string xpath,string pattern){
			commandProcessor.AssertTrue("assertNotXpathAttribute", new String[] {xpath,pattern,});
		}
		public void waitForXpathAttribute(string xpath,string pattern){
			commandProcessor.DoCommand("waitForXpathAttribute", new String[] {xpath,pattern,});
		}
		public void waitForNotXpathAttribute(string xpath,string pattern){
			commandProcessor.DoCommand("waitForNotXpathAttribute", new String[] {xpath,pattern,});
		}
		public void storeXpathAttribute(string xpath,string variableName){
			commandProcessor.DoCommand("storeXpathAttribute", new String[] {xpath,variableName,});
		}
		public string[] getXpathValue(string xpath){
			return commandProcessor.GetStringArray("getXpathValue", new String[] {xpath,});
		}
		public void verifyXpathValue(string xpath,string pattern){
			commandProcessor.VerifyTrue("verifyXpathValue", new String[] {xpath,pattern,});
		}
		public void verifyNotXpathValue(string xpath,string pattern){
			commandProcessor.VerifyTrue("verifyNotXpathValue", new String[] {xpath,pattern,});
		}
		public void assertXpathValue(string xpath,string pattern){
			commandProcessor.AssertTrue("assertXpathValue", new String[] {xpath,pattern,});
		}
		public void assertNotXpathValue(string xpath,string pattern){
			commandProcessor.AssertTrue("assertNotXpathValue", new String[] {xpath,pattern,});
		}
		public void waitForXpathValue(string xpath,string pattern){
			commandProcessor.DoCommand("waitForXpathValue", new String[] {xpath,pattern,});
		}
		public void waitForNotXpathValue(string xpath,string pattern){
			commandProcessor.DoCommand("waitForNotXpathValue", new String[] {xpath,pattern,});
		}
		public void storeXpathValue(string xpath,string variableName){
			commandProcessor.DoCommand("storeXpathValue", new String[] {xpath,variableName,});
		}
		public void pause(string waitTime){
			commandProcessor.DoCommand("pause", new String[] {waitTime,});
		}
		public void pauseAndWait(string waitTime){
			commandProcessor.DoCommand("pauseAndWait", new String[] {waitTime,});
		}

		
	}
}