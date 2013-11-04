using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SeleniumWrapper
{
    [Guid("DAF1B336-87D4-4C10-8316-BBE48A6D09DB")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IActions
    {
        [Description("Clicks an element.")]
        Actions click([Optional][DefaultParameterValue(null)] object webelement);

        [Description("Holds down the left mouse button on an element.")]
        Actions clickAndHold([Optional][DefaultParameterValue(null)] object webelement);

        [Description("Performs a context-click (right click) on an element.")]
        Actions contextClick([Optional][DefaultParameterValue(null)] object webelement);

        [Description("Double-clicks an element.")]
        Actions doubleClick([Optional][DefaultParameterValue(null)] object webelement);

        [Description("Holds down the left mouse button on the source element, then moves to the target element and releases the mouse button.")]
        Actions dragAndDrop(object webelement_source, object webelement_target);

        [Description("Holds down the left mouse button on the source element, then moves to the target element and releases the mouse button. ")]
        Actions dragAndDropByOffset(object webelement_source, int offset_x, int offset_y);

        [Description("Sends a key press only, without releasing it. Should only be used with modifier keys (Control, Alt and Shift).")]
        Actions keyDown(string key, [Optional][DefaultParameterValue(null)] object webelement);

        [Description("Releases a modifier key.")]
        Actions keyUp(string key, [Optional][DefaultParameterValue(null)] object webelement);

        [Description("Moving the mouse to an offset from current mouse position.")]
        Actions moveByOffset(int offset_x, int offset_y);

        [Description("Moving the mouse to the middle of an element.")]
        Actions moveToElement( object webelement);

        [Description("Performs all stored Actions.")]
        void perform();

        [Description("Releasing a held mouse button.")]
        Actions releaseMouse();

        [Description("Sends keys to current focused element or provided element.")]
        Actions sendKeys(string keys, [Optional][DefaultParameterValue(null)] object webelement);
    }

}
