using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SeleniumWrapper {

    /// <summary>
    /// The user-facing API for emulating complex user gestures. Use this class rather than using the Keyboard or Mouse directly. Implements the builder pattern: Builds a CompositeAction containing all actions specified by the method calls.
    /// </summary>
    [Description("The user-facing API for emulating complex user gestures. Use this class rather than using the Keyboard or Mouse directly. Implements the builder pattern: Builds a CompositeAction containing all actions specified by the method calls.")]
    [Guid("ED2E6AB4-D901-469C-829B-FB8601D6B166")]
    [ComVisible(true), ComDefaultInterface(typeof(IActions)), ClassInterface(ClassInterfaceType.None)]
    public class Actions : IActions {
        private OpenQA.Selenium.Interactions.Actions _actions = null;

        public Actions(OpenQA.Selenium.IWebDriver _webDriver) {
            _actions = new OpenQA.Selenium.Interactions.Actions(_webDriver);
        }

        /// <summary>Clicks an element.</summary>
        /// <param name="webelement">The element to click. If None, clicks on current mouse position.</param>
        /// <returns></returns>
        public Actions click(WebElement webelement = null) {
            if (webelement == null)
                _actions.Click();
            else
                _actions.Click(webelement._webElement);
            return this;
        }

        /// <summary>Holds down the left mouse button on an element.</summary>
        /// <param name="webelement">The element to mouse down. If None, clicks on current mouse position.</param>
        /// <returns></returns>
        public Actions clickAndHold(WebElement webelement = null) {
            if (webelement == null)
                _actions.ClickAndHold();
            else
                _actions.ClickAndHold(webelement._webElement);
            return this;
        }

        /// <summary>Performs a context-click (right click) on an element.</summary>
        /// <param name="webelement"> The element to context-click. If None, clicks on current mouse position.</param>
        /// <returns></returns>
        public Actions contextClick(WebElement webelement = null) {
            if (webelement == null)
                _actions.ContextClick();
            else
                _actions.ContextClick(webelement._webElement);
            return this;
        }

        /// <summary>Double-clicks an element.</summary>
        /// <param name="webelement">The element to double-click. If None, clicks on current mouse position.</param>
        /// <returns></returns>
        public Actions doubleClick(WebElement webelement = null) {
            if (webelement == null)
                _actions.DoubleClick();
            else
                _actions.DoubleClick(webelement._webElement);
            return this;
        }

        /// <summary>Holds down the left mouse button on the source element, then moves to the target element and releases the mouse button.</summary>
        /// <param name="webelement_source">The element to mouse down.</param>
        /// <param name="webelement_target">The element to mouse up.</param>
        /// <returns></returns>
        public Actions dragAndDrop(WebElement webelement_source, WebElement webelement_target) {
            _actions.DragAndDrop(webelement_source._webElement, webelement_target._webElement);
            return this;
        }

        /// <summary>Holds down the left mouse button on the source element, then moves to the target element and releases the mouse button.</summary>
        /// <param name="webelement_source">The element to mouse down.</param>
        /// <param name="offset_x">X offset to move to.</param>
        /// <param name="offset_y">Y offset to move to.</param>
        /// <returns></returns>
        public Actions dragAndDropByOffset(WebElement webelement_source, int offset_x, int offset_y) {
            _actions.DragAndDropToOffset(webelement_source._webElement, offset_x, offset_y);
            return this;
        }

        /// <summary>Sends a key press only, without releasing it. Should only be used with modifier keys (Control, Alt and Shift).</summary>
        /// <param name="key">The modifier key to send. Values are defined in Keys class.</param>
        /// <param name="webelement">The element to send keys. If None, sends a key to current focused element.</param>
        /// <returns></returns>
        public Actions keyDown(string key, WebElement webelement = null) {
            if (webelement == null)
                _actions.KeyDown(key);
            else
                _actions.KeyDown(webelement._webElement, key);
            return this;
        }

        /// <summary>Releases a modifier key.</summary>
        /// <param name="key">The modifier key to send. Values are defined in Keys class.</param>
        /// <param name="webelement">The element to send keys. If None, sends a key to current focused element.</param>
        /// <returns></returns>
        public Actions keyUp(string key, WebElement webelement = null) {
            if (webelement == null)
                _actions.KeyUp(key);
            else
                _actions.KeyUp(webelement._webElement, key);
            return this;
        }

        /// <summary>Moving the mouse to an offset from current mouse position.</summary>
        /// <param name="offset_x">X offset to move to.</param>
        /// <param name="offset_y">Y offset to move to.</param>
        /// <returns></returns>
        public Actions moveByOffset(int offset_x, int offset_y) {
            _actions.MoveByOffset(offset_x, offset_y);
            return this;
        }

        /// <summary>Moving the mouse to the middle of an element.</summary>
        /// <param name="webelement">The element to move to.</param>
        /// <returns></returns>
        public Actions moveToElement(WebElement webelement) {
            _actions.MoveToElement(webelement._webElement);
            return this;
        }

        /// <summary>Releasing a held mouse button.</summary>
        /// <returns></returns>
        public Actions releaseMouse() {
            _actions.Release();
            return this;
        }

        /// <summary>Sends keys to an element.</summary>
        /// <param name="keys"></param>
        /// <param name="webelement">Element to send keys. If None, send keys to the current mouse position.</param>
        /// <returns></returns>
        public Actions sendKeys(string keys, WebElement webelement = null) {
            if (webelement == null)
                _actions.SendKeys(keys);
            else
                _actions.SendKeys(webelement._webElement, keys);
            return this;
        }

        /// <summary>Performs all stored Actions.</summary>
        public void perform() {
            this._actions.Build().Perform();
            this._actions = null;
        }

    }
}
