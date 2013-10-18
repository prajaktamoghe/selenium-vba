using System;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace SeleniumWrapper.Automation {

    [Guid("0FD5E6B7-C319-43FD-8791-DF3030F83A78")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IWindow {

        [Description("Delay to wait after executing each methode. Default is 0ms")]
        int Delay { get; set; }

        [Description("Delay to wait after a mouse method. Default is 80ms")]
        int DelayMouse { get; set; }

        [Description("Delay to wait after a keyboard method. Default is 80ms")]
        int DelayKeyboard { get; set; }

        [Description("Delay in millisecond to wait before sending the next character. Default is 6ms")]
        int DelayBetweenCharacters { get; set; }

        [Description("Handle of the attached window")]
        IntPtr Handle { get; }

        [Description("Title of the window")]
        String Title { get; }
        
        [Description("Brings the window foreground")]
        Window activate([Optional][DefaultParameterValue(5000)]int timeoutms);
        
        [Description("Send keys to the active window (+=SHIFT ^=CTRL %=MENU #=WIN (...)=Group {...}=Command)")]
        Window sendKeys(String keysOrmodifiers, [Optional][DefaultParameterValue(null)]String keys);

        [Description("Send text to an element")]
        Window type(string element, string text, [Optional][DefaultParameterValue(5000)]int timeoutms);
        
        [Description("Click at the location relative to the window position")]
        Window clickAt(int x, int y, [Optional][DefaultParameterValue(MB.Left)]MB button);
        
        [Description("Click double at the location relative to the window position")]
        Window clickAtDouble(int x, int y, [Optional][DefaultParameterValue(MB.Left)]MB button);

        [Description("Click on an element")]
        Window click(string element, [Optional][DefaultParameterValue(MB.Left)]MB button, [Optional][DefaultParameterValue(5000)]int timeoutms);
        
        [Description("Weel the mouse")]
        Window weel(int value);
        
        [Description("Use mouse to drag from point A to point B relative to the window")]
        Window dragFromTo(int downX, int downY, int upX, int upY);
        
        [Description("Wait for a new window having the provided title.")]
        Window waitWindow([Optional][DefaultParameterValue(null)]string title, [Optional][DefaultParameterValue(5000)]int timeoutms);
        
        [Description("Close the window")]
        Window close([Optional][DefaultParameterValue(5000)]int timeoutms);

        [Description("Minimize the window")]
        Window minimize();

        [Description("Maximize the window")]
        Window maximize(long timeout);

        [Description("Restore the window")]
        Window restore();

        [Description("Kill the application owning the window")]
        void kill([Optional][DefaultParameterValue(10000)]int timeoutms);

        [Description("Get the text of the element at the provided path")]
        string getText(string path, [DefaultParameterValue(10000)]int timeoutms);

        [Description("Get the element's text having the focus")]
        string getFocusedElementText();

        [Description("The the text at the provided location")]
        string getTextAt(int x, int y);

        [Description("Wait the provided time in millisecond")]
        Window wait(int timems);

        [Description("Resize the window")]
        Window resize(int width, int height);

        [Description("Relocate the window")]
        Window relocate(int x, int y);

    }
}
