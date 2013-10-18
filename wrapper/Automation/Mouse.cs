using System;
using System.Runtime.InteropServices;
using SeleniumWrapper.Automation;
using System.ComponentModel;

namespace SeleniumWrapper.Automation {
    [Guid("4E4CF73C-2EE8-40A8-AA18-06ED518BE80E")]
    [ComVisible(true)]
    public enum MB : uint {
        Left = 0,
        Right = 1,
        Middle = 2,
        XButton1 = 3,
        XButton2 = 4
    }

    [Guid("3B896C95-634A-4DF9-A1BA-072757F7C638")]
    [ComVisible(true)]
    public interface IMouse {
        [Description("Click at the screen coordiante")]
        void click(int x, int y, MB button);

        [Description("Click at the active window coordiantes")]
        void clickWindow(int x, int y, [Optional][DefaultParameterValue(MB.Left)]MB button);

        [Description("Double click at the screen coordiantes")]
        void clickDouble(int x, int y, MB button);

        [Description("Double click at to the active window coordinates")]
        void clickWindowDouble(int x, int y, [Optional][DefaultParameterValue(MB.Left)]MB button);

        [Description("Weel the mouse")]
        void weel(int value);

        [Description("Use mouse to drag from point A to point B using screen coordinates")]
        void drag(int downX, int downY, int upX, int upY);

        [Description("Use mouse to drag from point A to point B using window's relative coordinates")]
        void dragWindow(int downX, int downY, int upX, int upY);


    }

    /// <summary>
    /// 
    /// </summary>

    [Guid("E98EDD10-404D-40F8-B6C6-619A1602E29B")]
    [ComVisible(true), ComDefaultInterface(typeof(IMouse)), ClassInterface(ClassInterfaceType.None)]
    public class Mouse : IMouse {
        public int Delay { get; set; }

        /// <summary>Mouse class</summary>
        public Mouse()
            : this(80) {

        }

        public Mouse(int delayMs) {
            this.Delay = delayMs;
        }

        /// <summary>Click at the screen coordiantes</summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="button"></param>
        public void click(int x, int y, [Optional][DefaultParameterValue(MB.Left)]MB button) {
            click(new Win32.POINT(x, y), button);
        }

        /// <summary>Click at the active window coordiantes</summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="button"></param>
        public void clickWindow(int x, int y, [Optional][DefaultParameterValue(MB.Left)]MB button) {
            var hwnd = Win32.GetForegroundWindow();
            click(NatHelper.GetWindowRect(hwnd).Location.Offset(x, y), button);
        }

        /// <summary>Clic at a point</summary>
        /// <param name="point">Point</param>
        /// <param name="button">Button</param>
        internal void click(Win32.POINT point, MB button) {
            try {
                Win32.BlockInput(true);
                SendMouse(Win32.MOUSEEVENTF.MOUSEEVENTF_MOVE, point);
                System.Threading.Thread.Sleep(5);
                switch (button) {
                    case MB.Left:
                        SendMouse(Win32.MOUSEEVENTF.MOUSEEVENTF_LEFTDOWN, point);
                        System.Threading.Thread.Sleep(10);
                        SendMouse(Win32.MOUSEEVENTF.MOUSEEVENTF_LEFTUP, point);
                        break;
                    case MB.Middle:
                        SendMouse(Win32.MOUSEEVENTF.MOUSEEVENTF_MIDDLEDOWN, point);
                        System.Threading.Thread.Sleep(10);
                        SendMouse(Win32.MOUSEEVENTF.MOUSEEVENTF_MIDDLEUP, point);
                        break;
                    case MB.Right:
                        SendMouse(Win32.MOUSEEVENTF.MOUSEEVENTF_RIGHTDOWN, point);
                        System.Threading.Thread.Sleep(10);
                        SendMouse(Win32.MOUSEEVENTF.MOUSEEVENTF_RIGHTUP, point);
                        break;
                    case MB.XButton1:
                    case MB.XButton2:
                        SendMouse(Win32.MOUSEEVENTF.MOUSEEVENTF_XDOWN, point);
                        System.Threading.Thread.Sleep(10);
                        SendMouse(Win32.MOUSEEVENTF.MOUSEEVENTF_XUP, point);
                        break;
                    default:
                        break;
                }
            } finally {
                Win32.BlockInput(false);
            }
        }

        /// <summary>Double click relative to the active window</summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="button">Button</param>
        public void clickWindowDouble(int x, int y, [Optional][DefaultParameterValue(MB.Left)]MB button) {
            var hwnd = Win32.GetForegroundWindow();
            clickDouble(NatHelper.GetWindowRect(hwnd).Location.Offset(x, y), button);
        }

        /// <summary>Double click at the screen coordiantes</summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="button">Button</param>
        public void clickDouble(int x, int y, [Optional][DefaultParameterValue(MB.Left)]MB button) {
            clickDouble(new Win32.POINT(x, y), button);
        }

        /// <summary>Double click relative to the active window</summary>
        /// <param name="point"></param>
        /// <param name="button"></param>
        internal void clickDouble(Win32.POINT point, MB button) {
            click(point, button);
            System.Threading.Thread.Sleep(100);
            click(point, button);
        }

        /// <summary>Weel the mouse</summary>
        /// <param name="value">Number of weels</param>
        public void weel(int value) {
            //User32.mouse_event(User32.MOUSEEVENTF_WHEEL, 0, 0, value, 0);
            var mouseInput = new Win32.INPUT {
                type = Win32.INPUTT.INPUTT_Mouse,
                mi = new Win32.MouseInputData {
                    mouseData = value,
                    dwFlags = Win32.MOUSEEVENTF.MOUSEEVENTF_WHEEL | Win32.MOUSEEVENTF.MOUSEEVENTF_ABSOLUTE
                }
            };
            Win32.SendInput(1, ref mouseInput, 0x1C);
            WaitMessageConsumed();
        }

        /// <summary>Use mouse to drag from point A to point B using window's relative coordinates</summary>
        /// <param name="downX">Start point X</param>
        /// <param name="downY">Start point Y</param>
        /// <param name="upX">End point X</param>
        /// <param name="upY">End point Y</param>
        /// <returns>Windows object</returns>
        public void dragWindow(int downX, int downY, int upX, int upY) {
            var hwnd = Win32.GetForegroundWindow();
            var point = NatHelper.GetWindowRect(hwnd).Location;
            drag(point.X + downX, point.Y + downY, point.X + upX, point.Y + upY);
        }

        /// <summary>Use mouse to drag from point A to point B using screen coordinates</summary>
        /// <param name="downX">Start point X</param>
        /// <param name="downY">Start point Y</param>
        /// <param name="upX">End point X</param>
        /// <param name="upY">End point Y</param>
        /// <returns>Windows object</returns>
        public void drag(int downX, int downY, int upX, int upY) {
            Win32.SetCursorPos(downX, downY);
            var mouseInput = new Win32.INPUT {
                type = Win32.INPUTT.INPUTT_Mouse,
                mi = new Win32.MouseInputData {
                    dwFlags = Win32.MOUSEEVENTF.MOUSEEVENTF_LEFTDOWN
                }
            };
            Win32.SendInput(1, ref mouseInput, 0x1C);
            System.Threading.Thread.Sleep(10);
            Win32.SetCursorPos(upX, upY);
            mouseInput.mi.dwFlags = Win32.MOUSEEVENTF.MOUSEEVENTF_LEFTUP;
            Win32.SendInput(1, ref mouseInput, 0x1C);
            WaitMessageConsumed();
        }

        /// <summary>Send a mouse event</summary>
        /// <param name="evt">Event</param>
        /// <param name="point">Point</param>
        private void SendMouse(Win32.MOUSEEVENTF evt, Win32.POINT point) {
            //compose data
            var input = new Win32.INPUT {
                type = 0,
                mi = new Win32.MouseInputData {
                    dwExtraInfo = IntPtr.Zero,
                    mouseData = 0,
                    time = 0,
                    dx = (int)(point.X * (65535f / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width)),
                    dy = (int)(point.Y * (65535f / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height)),
                    dwFlags = evt | Win32.MOUSEEVENTF.MOUSEEVENTF_ABSOLUTE
                }
            };

            //Send the data
            Win32.SendInput(1, ref input, 0x1C);
            WaitMessageConsumed();
        }

        /// <summary>Wait for the message to be consumed</summary>
        private void WaitMessageConsumed() {
            //Waits for the message loop to be empty
            var endtime = DateTime.Now.AddMilliseconds(5000);
            System.Threading.Thread.Sleep(this.Delay);
            while (true) {
                if (!Win32.GetInputState()) break;
                if (DateTime.Now > endtime) throw new TimeoutException("Failed to click at location");
                System.Threading.Thread.Sleep(20);
            }
            //Wait for input IDLE
            int pid;
            Win32.GetWindowThreadProcessId(Win32.GetForegroundWindow(), out pid);
            IntPtr process = IntPtr.Zero;
            try {
                process = Win32.OpenProcess(Win32.PROCESS.PROCESS_VM_READ, false, pid);
                Win32.WaitForInputIdle(process, 20000);
            } finally {
                if (process != IntPtr.Zero)
                    Win32.CloseHandle(process);
            }

        }

    }
}
