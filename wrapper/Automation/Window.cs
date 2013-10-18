using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Automation;

namespace SeleniumWrapper.Automation
{
    [Guid("08A3C8C1-AC37-4B75-AE63-140DEED0A20F")]
    [ComVisible(true), ComDefaultInterface(typeof(Automation.IWindow)), ClassInterface(ClassInterfaceType.None)]
    public class Window : Automation.IWindow
    {
        #region singleton

        /// <summary>Get the foreground window</summary>
        /// <returns>Window object</returns>
        public static Window GetActive(){
            return new Window(Win32.GetForegroundWindow());
        }

        /// <summary>Get the desktop window</summary>
        /// <returns>Window object</returns>
        public static Window GetDesktop(){
            return new Window(Win32.GetDesktopWindow());
        }

        /// <summary>Get a top window by title</summary>
        /// <param name="title">Windown title. Wildcard character can be used</param>
        /// <returns>Window object</returns>
        /// <example>
        /// GetFromTitle("* - Notepad")
        /// </example>
        public static Window FromTitle(string title, int timeoutms = 1000)
        {
            try{
                return new Window( NatHelper.WaitNot( ()=>NatHelper.GetWindowByTitle(title), IntPtr.Zero, timeoutms) );
            }catch(TimeoutException ex){
                throw new Exception("Failed to fin the window named '" + title + "'", ex);
            }
        }

        /// <summary>Get the window from the program name</summary>
        /// <param name="program">Program name</param>
        /// <returns>Window object</returns>
        /// <example>
        /// FromProgram("notepad").Activate();
        /// </example>
        public static Window FromProgram(string program) {
            var processes = System.Diagnostics.Process.GetProcessesByName(program.Replace(".exe", ""));
            if (processes == null || processes.Length == 0)
                throw new ApplicationException("Failed to find the process named \"" + program + "\"");
            return new Window(processes[0].MainWindowHandle);
        }

        #endregion

        private static Keyboard _keyboard;
        private static Mouse _mouse;
        private static int _delay;

        private IntPtr _hwnd;

        #region constructors

        /// <summary>Create a Window instance attached to physical window</summary>
        /// <param name="hwnd">Window handle</param>
        public Window(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                throw new Exception("Window handle is null");
            _hwnd = hwnd;
            if(_keyboard==null)
                _keyboard = new Keyboard();
            if(_mouse == null)
                _mouse = new Mouse();
        }

        #endregion

        /// <summary>Delay to wait after executing each methode. Default is 0ms</summary>
        public int Delay {
            get { return _delay; }
            set { _delay = value; }
        }

        /// <summary>Delay to wait after a mouse method. Default is 80ms</summary>
        public int DelayMouse {
            get { return _mouse.Delay; }
            set { _mouse.Delay = value; }
        }

        /// <summary>Delay to wait after a keyboard method. Default is 80ms</summary>
        public int DelayKeyboard {
            get { return _keyboard.Delay; }
            set { _keyboard.Delay = value; }
        }

        /// <summary>Delay in millisecond to wait before sending the next character. Default is 6ms</summary>
        public int DelayBetweenCharacters {
            get { return _keyboard.DelayBetweenCharacter; }
            set { _keyboard.DelayBetweenCharacter = value; }
        }

        /// <summary>Handle of the attached window</summary>
        public IntPtr Handle {
            get { return _hwnd; }
        }

        /// <summary>Title of the window</summary>
        public String Title {
            get { return NatHelper.GetText(_hwnd); }
        }

        /// <summary>Brings the window foreground</summary>
        /// <param name="timeoutms">Timeout in millisecond</param>
        /// <returns>Window object</returns>
        public Window activate([Optional][DefaultParameterValue(5000)]int timeoutms) {
            if (_hwnd == IntPtr.Zero)
                throw new Exception("Select a window firt using GetActive, FromTitle or GetDesktop");
            Win32.SetForegroundWindow(_hwnd);
            Thread.Sleep(20 + _delay);
            try {
                NatHelper.Wait( ()=>Win32.GetForegroundWindow(), _hwnd, timeoutms);
            } catch (TimeoutException ex) {
                throw new Exception("Failed to activate the window", ex);
            }
            return this;
        }

        /// <summary>Send keys to the active window (+=SHIFT ^=CTRL %=MENU #=WIN (...)=Group {...}=Command)</summary>
        /// <param name="keysOrmodifiers">Keys or modifiers </param>
        /// <param name="keys">Keys if keysOrmodifiers contains keys, nothing otherwise </param>
        /// <returns>Window object</returns>
        public Window sendKeys(String keysOrmodifiers, [Optional][DefaultParameterValue("")]String keys) {
            checkForegroundWindow();
            _keyboard.SendKeys(keysOrmodifiers, keys);
            Thread.Sleep(_delay);
            return this;
        }

        /// <summary>Click at the location relative to the window position</summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="button">Optional - Button to click. Click left by default</param>
        /// <returns>Window object</returns>
        public Window clickAt(int x, int y, [Optional][DefaultParameterValue(MB.Left)]MB button) {
            checkForegroundWindow();
            _mouse.click(NatHelper.GetWindowRect(_hwnd).Location.Offset(x, y), button);
            Thread.Sleep(_delay);
            return this;
        }

        /// <summary>Click double at the location relative to the window position</summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="button">Optional - Button to click. Click left by default</param>
        /// <returns>Window object</returns>
        public Window clickAtDouble(int x, int y, [Optional][DefaultParameterValue(MB.Left)]MB button) {
            checkForegroundWindow();
            _mouse.clickDouble(NatHelper.GetWindowRect(_hwnd).Location.Offset(x, y), button);
            Thread.Sleep(_delay);
            return this;
        }

        /// <summary>Click on an element</summary>
        /// <param name="element">Element text, path or image</param>
        /// <param name="button">Optional - Click left button by default</param>
        /// <param name="timeoutms">Optional - Timeout in millisecond</param>
        /// <returns>Window object</returns>
        public Window click(string element, [Optional][DefaultParameterValue(MB.Left)]MB button, [Optional][DefaultParameterValue(5000)]int timeoutms) {
            checkForegroundWindow();
            try {
                var autEle = NatHelper.WaitNot(() => NatHelper.FindFirstByPath(_hwnd, element), null, timeoutms);
                var rect = autEle.Current.BoundingRectangle;
                _mouse.click(new Win32.RECT((int)rect.Left, (int)rect.Top, (int)rect.Width, (int)rect.Height).Center, button);
                Thread.Sleep(_delay);
                return this;
            } catch (TimeoutException ex) {
                throw new Exception("No element found matching the pattern \"" + element + "\"", ex);
            }
        }

        /// <summary>Send text to an element</summary>
        /// <param name="element">Element text, path or image</param>
        /// <param name="text">Text</param>
        /// <param name="timeoutms">Optional - Timeout in millisecond</param>
        /// <returns></returns>
        public Window type(string element, string text, [Optional][DefaultParameterValue(5000)]int timeoutms) {
            checkForegroundWindow();
            try {
                AutomationElement autEle = NatHelper.WaitNot(() => NatHelper.FindFirstByPath(_hwnd, element), null, timeoutms / 2 );
                autEle.SetFocus();
                NatHelper.Wait(() => AutomationElement.FocusedElement, autEle, timeoutms / 2);
                _keyboard.SendChars(text);
                Thread.Sleep(_delay);
                return this;
            } catch (TimeoutException ex) {
                throw new Exception("No element found matching the pattern \"" + element + "\"", ex);
            }
        }

        /// <summary>Weel the mouse</summary>
        /// <param name="value">Number of wheels</param>
        /// <returns>Window object</returns>
        public Window weel(int value) {
            checkForegroundWindow();
            _mouse.weel(value);
            Thread.Sleep(_delay);
            return this;
        }

        /// <summary>Use mouse to drag from point A to point B relative to the window</summary>
        /// <param name="downX">Start point X</param>
        /// <param name="downY">Start point Y</param>
        /// <param name="upX">End point X</param>
        /// <param name="upY">End point Y</param>
        /// <returns>Windows object</returns>
        public Window dragFromTo(int downX, int downY, int upX, int upY) {
            checkForegroundWindow();
            var loc = NatHelper.GetWindowRect(_hwnd);
            _mouse.drag(loc.Left + downX, loc.Top + downY, loc.Left + upX, loc.Top + upY);
            Thread.Sleep(_delay);
            return this;
        }

        /// <summary>Wait for a new window having the provided title.</summary>
        /// <param name="title">Optional - Title to wait for. If undefined, it will accept all new windows</param>
        /// <param name="timeoutms">Optional - Timeout in millisecond</param>
        /// <returns>Window object</returns>
        public Window waitWindow([Optional][DefaultParameterValue(null)]string title, [Optional][DefaultParameterValue(5000)]int timeoutms) {
            IntPtr hwnd;
            try {
                hwnd = NatHelper.WaitNot(() => Win32.GetForegroundWindow(), _hwnd, timeoutms);
            } catch (TimeoutException ex) {
                throw new Exception("Failed to detect a new window", ex);
            }
            try {
                if (title != null)
                    NatHelper.Wait(() => NatHelper.WildcardMatch(title, NatHelper.GetText(hwnd)), true, timeoutms);
                return new Window(hwnd);
            } catch (TimeoutException ex) {
                throw new Exception("Failed to fin the window named \"" + title + "\"", ex);
            }
        }

        /// <summary>Close the window</summary>
        /// <param name="timeoutms">Timeout in millisecond</param>
        /// <returns>Window object</returns>
        public Window close([Optional][DefaultParameterValue(5000)]int timeoutms) {
            checkForegroundWindow();
            Win32.PostMessage(_hwnd, 0x10 /*WM_CLOSE*/, IntPtr.Zero, IntPtr.Zero);
            IntPtr hwnd;
            try {
                hwnd = NatHelper.WaitNot(() => Win32.GetForegroundWindow(), _hwnd, timeoutms);
            } catch (TimeoutException ex) {
                throw new Exception("Failed to close the window", ex);
            }
            Thread.Sleep(_delay);
            return this;
            //Win32.IsWindow(_hwnd)
        }

        /// <summary>Minimize the window</summary>
        /// <returns>Window object</returns>
        public Window minimize() {
            checkForegroundWindow();
            Win32.ShowWindow(_hwnd, Win32.SW.SW_MINIMIZE);
            try {
                NatHelper.WaitNot(() => Win32.GetWindowLong(_hwnd, Win32.GWL.GWL_STYLE) & (int)Win32.WS.WS_MINIMIZE, 0);
            } catch (TimeoutException ex) {
                throw new Exception("Failed to minimize the window", ex);
            }
            Thread.Sleep(_delay);
            return this;
        }

        /// <summary>Maximize the window</summary>
        /// <returns>Window object</returns>
        public Window maximize(long timeout) {
            checkForegroundWindow();
            Win32.ShowWindow(_hwnd, Win32.SW.SW_MAXIMIZE);
            try {
                NatHelper.WaitNot(() => Win32.GetWindowLong(_hwnd, Win32.GWL.GWL_STYLE) & (int)Win32.WS.WS_MAXIMIZE, 0);
            } catch (TimeoutException ex) {
                throw new Exception("Failed to minimize the window", ex);
            }
            Thread.Sleep(_delay);
            return this;
        }

        /// <summary>Restore the window</summary>
        /// <returns>Window object</returns>
        public Window restore() {
            checkForegroundWindow();
            Win32.ShowWindow(_hwnd, Win32.SW.SW_RESTORE);
            try {
                NatHelper.Wait(() => Win32.GetWindowLong(_hwnd, Win32.GWL.GWL_STYLE) & (int)(Win32.WS.WS_MAXIMIZE | Win32.WS.WS_MINIMIZE), 0);
            } catch (TimeoutException ex) {
                throw new Exception("Failed to minimize the window", ex);
            }
            Thread.Sleep(_delay);
            return this;
        }

        /// <summary>Kill the application owning the window</summary>
        /// <param name="timeoutms">Optional - Timeout in millisecond</param>
        public void kill([Optional][DefaultParameterValue(10000)]int timeoutms) {
            if (_hwnd == IntPtr.Zero)
                throw new Exception("The handle is null for this Window object");
            var pid = NatHelper.GetProcessId(_hwnd);
            using (var processe = System.Diagnostics.Process.GetProcessById(pid)) {
                processe.Kill();
            }
            Thread.Sleep(_delay);
        }

        /// <summary>Get the element's text having the focus</summary>
        /// <returns>Text</returns>
        public string getFocusedElementText() {
            Thread.Sleep(_delay);
            checkForegroundWindow();
            var element = AutomationElement.FocusedElement;
            if (element == null)
                throw new Exception("Failed to identify the focused element");
            return element.GetText();
        }

        /// <summary>Get the text of the element at the provided path</summary>
        /// <returns>Text</returns>
        public string getText(string path, [DefaultParameterValue(10000)]int timeoutms) {
            Thread.Sleep(_delay);
            checkForegroundWindow();
            AutomationElement autEle = NatHelper.WaitNot(() => NatHelper.FindFirstByPath(_hwnd, path), null, timeoutms);
            var hwnd = NatHelper.FindFirstByPath(_hwnd, path);
            if (autEle == null)
                throw new Exception("Failed to find the element: " + path);
            return autEle.GetText();
        }

        /// <summary>The the text at the provided location</summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>Text</returns>
        public string getTextAt(int x, int y) {
            Thread.Sleep(_delay);
            checkForegroundWindow();
            var point = NatHelper.GetWindowRect(_hwnd).Location.Offset(x, y);
            var element = AutomationElement.FromPoint(new System.Windows.Point(point.X, point.Y));
            if (element == null)
                throw new Exception("Failed to locate the element at location X=" + point.X.ToString() + " Y=" + point.Y.ToString());
            return element.GetText();
        }

        /// <summary>Wait the provided time in millisecond</summary>
        /// <param name="timems">Time in millisecond</param>
        /// <returns>Window object</returns>
        public Window wait(int timems) {
            Thread.Sleep(timems);
            return this;
        }

        /// <summary>Resize the window</summary>
        /// <param name="width">Width</param>
        /// <param name="height">Heigth</param>
        /// <returns>Window object</returns>
        public Window resize(int width, int height) {
            checkForegroundWindow();
            var rect = NatHelper.GetWindowRect(_hwnd);
            Win32.MoveWindow(_hwnd, rect.Left, rect.Top, width, height, true);
            Thread.Sleep(_delay);
            return this;
        }

        /// <summary>Relocate the window</summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>Window object</returns>
        public Window relocate(int x, int y) {
            checkForegroundWindow();
            var rect = NatHelper.GetWindowRect(_hwnd);
            Win32.MoveWindow(_hwnd, x, y, rect.Width, rect.Height, true);
            Thread.Sleep(_delay);
            return this;
        }

        /// <summary>Verify the window object is attached to a window</summary>
        private void checkForegroundWindow()
        {
            var hwnd = Win32.GetForegroundWindow();
            if ( hwnd != _hwnd)
                throw new Exception("The active window (" + NatHelper.GetText(hwnd).Truncate(20) + ") is differente from this window (" + Title.Truncate(10) + "). Use the methode WaitNewOne or FromTitle");
        }

    }
}
