using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace SeleniumWrapper
{
    [Guid("DAF1B336-87D4-4C10-8316-BBE48A6D09DB")]
    [ComVisible(true)]
    public interface IActions
    {
        [Description("Clicks an element.")]
        Actions click([Optional][DefaultParameterValue(null)]ref object webelement);

        [Description("Holds down the left mouse button on an element.")]
        Actions clickAndHold([Optional][DefaultParameterValue(null)]ref object webelement);

        [Description("Performs a context-click (right click) on an element.")]
        Actions contextClick([Optional][DefaultParameterValue(null)]ref object webelement);

        [Description("Double-clicks an element.")]
        Actions doubleClick([Optional][DefaultParameterValue(null)]ref object webelement);

        [Description("Holds down the left mouse button on the source element, then moves to the target element and releases the mouse button.")]
        Actions dragAndDrop(ref object webelement_source, ref object webelement_target);

        [Description("Holds down the left mouse button on the source element, then moves to the target element and releases the mouse button. ")]
        Actions dragAndDropByOffset(ref object webelement_source, int offset_x, int offset_y);

        [Description("Sends a key press only, without releasing it. Should only be used with modifier keys (Control, Alt and Shift).")]
        Actions keyDown(string key, [Optional][DefaultParameterValue(null)]ref object webelement);

        [Description("Releases a modifier key.")]
        Actions key_up(string key, [Optional][DefaultParameterValue(null)]ref object webelement);

        [Description("Moving the mouse to an offset from current mouse position.")]
        Actions moveByOffset(int offset_x, int offset_y);

        [Description("Moving the mouse to the middle of an element.")]
        Actions moveToElement(ref object webelement);

        [Description("Performs all stored Actions.")]
        void perform();

        [Description("Releasing a held mouse button.")]
        Actions releaseMouse();

        [Description("Sends keys to current focused element or provided element.")]
        Actions sendKeys(string keys, [Optional][DefaultParameterValue(null)]ref object webelement);
    }

    /// <summary>
    /// The user-facing API for emulating complex user gestures. Use this class rather than using the Keyboard or Mouse directly. Implements the builder pattern: Builds a CompositeAction containing all actions specified by the method calls.
    /// </summary>

    [Description("The user-facing API for emulating complex user gestures. Use this class rather than using the Keyboard or Mouse directly. Implements the builder pattern: Builds a CompositeAction containing all actions specified by the method calls.")]
    [Guid("ED2E6AB4-D901-469C-829B-FB8601D6B166")]
    [ComVisible(true), ComDefaultInterface(typeof(IActions)), ClassInterface(ClassInterfaceType.None)]
    public class Actions : IActions
    {
        private OpenQA.Selenium.Interactions.Actions actions;

        private void instantiateActions()
        {
            if(this.actions==null)
                this.actions = new OpenQA.Selenium.Interactions.Actions(WebDriver.CurrentWebDriver);
        }

        /// <summary>Clicks an element.</summary>
        /// <param name="webelement">The element to click. If None, clicks on current mouse position.</param>
        /// <returns></returns>
        public Actions click([Optional][DefaultParameterValue(null)]ref object webelement)
        {
            instantiateActions();
            if(webelement==null)
                actions.Click();
            else
                actions.Click(((WebElement)webelement).webElement);
            return this;
        }

        /// <summary>Holds down the left mouse button on an element.</summary>
        /// <param name="webelement">The element to mouse down. If None, clicks on current mouse position.</param>
        /// <returns></returns>
        public Actions clickAndHold([Optional][DefaultParameterValue(null)]ref object webelement)
        {
            instantiateActions();
            if(webelement==null)
                actions.ClickAndHold();
            else
                actions.ClickAndHold(((WebElement)webelement).webElement);
            return this;
        }

        /// <summary>Performs a context-click (right click) on an element.</summary>
        /// <param name="webelement"> The element to context-click. If None, clicks on current mouse position.</param>
        /// <returns></returns>
        public Actions contextClick([Optional][DefaultParameterValue(null)]ref object webelement)
        {
            instantiateActions();
            if(webelement==null)
                actions.ContextClick();
            else
                actions.ContextClick(((WebElement)webelement).webElement);
            return this;
        }

        /// <summary>Double-clicks an element.</summary>
        /// <param name="webelement">The element to double-click. If None, clicks on current mouse position.</param>
        /// <returns></returns>
        public Actions doubleClick([Optional][DefaultParameterValue(null)]ref object webelement)
        {
            instantiateActions();
            if(webelement==null)
                actions.DoubleClick();
            else
                actions.DoubleClick(((WebElement)webelement).webElement);
            return this;
        }

        /// <summary>Holds down the left mouse button on the source element, then moves to the target element and releases the mouse button.</summary>
        /// <param name="webelement_source">The element to mouse down.</param>
        /// <param name="webelement_target">The element to mouse up.</param>
        /// <returns></returns>
        public Actions dragAndDrop(ref object webelement_source, ref object webelement_target)
        {
            instantiateActions();
            actions.DragAndDrop(((WebElement)webelement_source).webElement, ((WebElement)webelement_target).webElement);
            return this;
        }

        /// <summary>Holds down the left mouse button on the source element, then moves to the target element and releases the mouse button.</summary>
        /// <param name="webelement_source">The element to mouse down.</param>
        /// <param name="offset_x">X offset to move to.</param>
        /// <param name="offset_y">Y offset to move to.</param>
        /// <returns></returns>
        public Actions dragAndDropByOffset(ref object webelement_source, int offset_x, int offset_y)
        {
            instantiateActions();
            actions.DragAndDropToOffset(((WebElement)webelement_source).webElement, offset_x, offset_y);
            return this;
        }

        /// <summary>Sends a key press only, without releasing it. Should only be used with modifier keys (Control, Alt and Shift).</summary>
        /// <param name="key">The modifier key to send. Values are defined in Keys class.</param>
        /// <param name="webelement">The element to send keys. If None, sends a key to current focused element.</param>
        /// <returns></returns>
        public Actions keyDown(string key, [Optional][DefaultParameterValue(null)]ref object webelement)
        {
            instantiateActions();
            if(webelement==null)
                actions.KeyDown(key);
            else
                actions.KeyDown(((WebElement)webelement).webElement, key);
            return this;
        }

        /// <summary>Releases a modifier key.</summary>
        /// <param name="key">The modifier key to send. Values are defined in Keys class.</param>
        /// <param name="webelement">The element to send keys. If None, sends a key to current focused element.</param>
        /// <returns></returns>
        public Actions key_up(string key, [Optional][DefaultParameterValue(null)]ref object webelement)
        {
            instantiateActions();
            if(webelement==null)
                actions.KeyUp(key);
            else
                actions.KeyUp(((WebElement)webelement).webElement, key);
            return this;
        }

        /// <summary>Moving the mouse to an offset from current mouse position.</summary>
        /// <param name="offset_x">X offset to move to.</param>
        /// <param name="offset_y">Y offset to move to.</param>
        /// <returns></returns>
        public Actions moveByOffset(int offset_x, int offset_y)
        {
            instantiateActions();
            actions.MoveByOffset(offset_x, offset_y);
            return this;
        }

        /// <summary>Moving the mouse to the middle of an element.</summary>
        /// <param name="webelement">The element to move to.</param>
        /// <returns></returns>
        public Actions moveToElement(ref object webelement)
        {
            instantiateActions();
            actions.MoveToElement(((WebElement)webelement).webElement);
            return this;
        }

        /// <summary>Releasing a held mouse button.</summary>
        /// <returns></returns>
        public Actions releaseMouse()
        {
            instantiateActions();
            actions.Release();
            return this;
        }

        /// <summary>Sends keys to an element.</summary>
        /// <param name="keys"></param>
        /// <param name="webelement">Element to send keys. If None, send keys to the current mouse position.</param>
        /// <returns></returns>
        public Actions sendKeys(string keys, [Optional][DefaultParameterValue(null)]ref object webelement)
        {
            instantiateActions();
            if(webelement==null)
                actions.SendKeys(keys);
            else
                actions.SendKeys(((WebElement)webelement).webElement, keys);
            return this;
        }

        /// <summary>Performs all stored Actions.</summary>
        public void perform()
        {
            this.actions.Build().Perform();
            this.actions = null;
        }

    }
}
