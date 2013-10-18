using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SeleniumWrapper.Automation
{
    /// <summary>Native functions from User32 and Kernel32</summary>
    public static class Win32
    {
        /// <summary>The POINT structure defines the x- and y- coordinates of a point.</summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int X, int Y){
                this.X = X;
                this.Y = Y;
            }

            public POINT Offset(int X, int Y) { return new POINT { X = this.X + X, Y = this.Y + Y }; }
        }

        /// <summary>The RECT structure defines the coordinates of the upper-left and lower-right corners of a rectangle.</summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public static RECT Empty = new RECT();

            public RECT(int left, int top, int width, int heigth)
            {
                Left = left;
                Top = top;
                Right = left + width;
                Bottom = top + heigth;
            }
            public int Height { get { return Bottom - Top; } }
            public int Width { get { return Right - Left; } }
            public POINT Location { get { return new POINT(Left, Top); } }
            public POINT Center { get { return new POINT(this.Left + (this.Width / 2), this.Top + (this.Height / 2)); } }
            public override bool Equals(object obj) {
                if (this.GetType() != obj.GetType()) return false;
                return this.Left == ((RECT)obj).Left && this.Right == ((RECT)obj).Right && this.Top == ((RECT)obj).Top && this.Bottom == ((RECT)obj).Bottom;
            }
        }

        /// <summary>
        /// Retrieves a handle to the foreground window (the window with which the user is currently working). The system assigns a slightly higher priority to the thread that creates the foreground window than it does to other threads.
        /// </summary>
        /// <returns>The return value is a handle to the foreground window. The foreground window can be NULL in certain circumstances, such as when a window is losing activation.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Retrieves the dimensions of the bounding rectangle of the specified window. The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen.
        /// </summary>
        /// <param name="hwnd">A handle to the window.</param>
        /// <param name="lprect">A pointer to a RECT structure that receives the screen coordinates of the upper-left and lower-right corners of the window.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lprect);

        /// <summary>
        /// Moves the cursor to the specified screen coordinates. If the new coordinates are not within the screen rectangle set by the most recent ClipCursor function call, the system automatically adjusts the coordinates so that the cursor stays within the rectangle.
        /// </summary>
        /// <param name="x">The new x-coordinate of the cursor, in screen coordinates.</param>
        /// <param name="y">The new y-coordinate of the cursor, in screen coordinates.</param>
        /// <returns>Returns nonzero if successful or zero otherwise. To get extended error information, call GetLastError.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetCursorPos(int x, int y);

        /// <summary>
        /// Retrieves a handle to the desktop window. The desktop window covers the entire screen. The desktop window is the area on top of which other windows are painted.
        /// </summary>
        /// <returns>The return value is a handle to the desktop window.</returns>
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();

        /// <summary>Determines whether there are mouse-button or keyboard messages in the calling thread's message queue.</summary>
        /// <returns>
        /// If the queue contains one or more new mouse-button or keyboard messages, the return value is nonzero.
        /// If there are no new mouse-button or keyboard messages in the queue, the return value is zero
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool GetInputState();

        /// <summary>Blocks keyboard and mouse input events from reaching applications.</summary>
        /// <param name="fBlockIt">The function's purpose. If this parameter is TRUE, keyboard and mouse input events are blocked. If this parameter is FALSE, keyboard and mouse events are unblocked. Note that only the thread that blocked input can successfully unblock input.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("user32.dll")]
        public static extern bool BlockInput(bool fBlockIt);

        public enum MOUSEEVENTF : uint
        {
            MOUSEEVENTF_MOVE = 0x0001,
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            MOUSEEVENTF_LEFTUP = 0x0004,
            MOUSEEVENTF_RIGHTDOWN = 0x0008,
            MOUSEEVENTF_RIGHTUP = 0x0010,
            MOUSEEVENTF_MIDDLEDOWN = 0x0020,
            MOUSEEVENTF_MIDDLEUP = 0x0040,
            MOUSEEVENTF_XDOWN = 0x0080,
            MOUSEEVENTF_XUP = 0x0100,
            MOUSEEVENTF_WHEEL = 0x0800,
            MOUSEEVENTF_VIRTUALDESK = 0x4000,
            MOUSEEVENTF_ABSOLUTE = 0x8000
        }

        /// <summary></summary>
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct MouseInputData
        {
            public int dx;
            public int dy;
            public int mouseData;
            public MOUSEEVENTF dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        public enum KEYEVENTF : uint
        {
            KEYEVENTF_NONE = 0x0000,
            KEYEVENTF_EXTENDEDKEY = 0x0001,  //If specified, the scan code was preceded by a prefix byte that has the value 0xE0 (224).
            KEYEVENTF_KEYUP = 0x0002,        //the key is being released. If not specified, the key is being pressed.
            KEYEVENTF_UNICODE = 0x0004,       //the system synthesizes a VK_PACKET keystroke. The wVk parameter must be zero. This flag can only be combined with the KEYEVENTF_KEYUP flag. For more information, see the Remarks section.
            KEYEVENTF_SCANCODE = 0x0008     // wScan identifies the key and wVk is ignored.
        }

        /// <summary></summary>
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct KeyboardInputData
        {
            public ushort wVk;          //A virtual-key code. The code must be a value in the range 1 to 254.
            public ushort wScan;        //A hardware scan code for the key.
            public KEYEVENTF dwFlags;   //The extended-key flag, event-injected flag, context code, and transition-state flag. This member is specified as follows.
            public uint time;
            public IntPtr dwExtraInfo;
        }

        /// <summary></summary>
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct HardwareInputData
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        public enum INPUTT : int
        {
            INPUTT_Mouse = 0,
            INPUTT_Keyboard = 1,
            INPUTT_Hardware = 2
        }

        /// <summary></summary>
        [Serializable]
        [StructLayout(LayoutKind.Explicit, Size = 28)]
        public struct INPUT
        {
            [FieldOffset(0)]
            public INPUTT type;
            [FieldOffset(4)]
            public MouseInputData mi;
            [FieldOffset(4)]
            public KeyboardInputData ki;
            [FieldOffset(4)]
            public HardwareInputData hi;
        }

        /// <summary>Translates a character to the corresponding virtual-key code and shift state for the current keyboard.</summary>
        /// <param name="ch">The character to be translated into a virtual-key code.</param>
        /// <returns>
        /// If the function succeeds, the low-order byte of the return value contains the virtual-key code and the high-order byte contains the shift state, which can be a combination of the following flag bits.
        ///  1 : Either SHIFT key is pressed.
        ///  2 : Either CTRL key is pressed.
        ///  4 : Either ALT key is pressed.
        ///  8 : The Hankaku key is pressed
        ///  16 : Reserved (defined by the keyboard layout driver).
        ///  32 : Reserved (defined by the keyboard layout driver).
        /// </returns>
        [DllImport("user32.dll")]
        public static extern short VkKeyScan(char ch);

        /// <summary></summary>
        public enum MAPVK : uint
        {
            MAPVK_VK_TO_CHAR = 2,   //uCode is a virtual-key code and is translated into an unshifted character value in the low-order word of the return value. Dead keys (diacritics) are indicated by setting the top bit of the return value. If there is no translation, the function returns 0.
            MAPVK_VK_TO_VSC = 0,    //uCode is a virtual-key code and is translated into a scan code. If it is a virtual-key code that does not distinguish between left- and right-hand keys, the left-hand scan code is returned. If there is no translation, the function returns 0.
            MAPVK_VSC_TO_VK = 1,    //uCode is a scan code and is translated into a virtual-key code that does not distinguish between left- and right-hand keys. If there is no translation, the function returns 0.
            MAPVK_VSC_TO_VK_EX = 3, //uCode is a scan code and is translated into a virtual-key code that distinguishes between left- and right-hand keys. If there is no translation, the function returns 0.
            MAPVK_VK_TO_VSC_EX = 4  //he uCode parameter is a virtual-key code and is translated into a scan code. If it is a virtual-key code that does not distinguish between left- and right-hand keys, the left-hand scan code is returned. If the scan code is an extended scan code, the high byte of the uCode value can contain either 0xe0 or 0xe1 to specify the extended scan code. If there is no translation, the function returns 0.
        }


        /// <summary>
        /// Retrieves the status of the specified virtual key. The status specifies whether the key is up, down, or toggled (on, off—alternating each time the key is pressed).
        /// </summary>
        /// <param name="vKey">
        /// A virtual key. If the desired virtual key is a letter or digit (A through Z, a through z, or 0 through 9), nVirtKey must be set to the ASCII value of that character. For other keys, it must be a virtual-key code.
        /// If a non-English keyboard layout is used, virtual keys with values in the range ASCII A through Z and 0 through 9 are used to specify most of the character keys. For example, for the German keyboard layout, the virtual key of value ASCII O (0x4F) refers to the "o" key, whereas VK_OEM_1 refers to the "o with umlaut" key.
        /// </param>
        /// <returns>
        /// The return value specifies the status of the specified virtual key, as follows:
        ///  - If the high-order bit is 1, the key is down; otherwise, it is up.
        ///  - If the low-order bit is 1, the key is toggled. A key, such as the CAPS LOCK key, is toggled if it is turned on. The key is off and untoggled if the low-order bit is 0. A toggle key's indicator light (if any) on the keyboard will be on when the key is toggled, and off when the key is untoggled.
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern short GetKeyState(int vKey);

        /// <summary>
        /// Translates (maps) a virtual-key code into a scan code or character value, or translates a scan code into a virtual-key code.
        /// To specify a handle to the keyboard layout to use for translating the specified code, use the MapVirtualKeyEx function.
        /// </summary>
        /// <param name="uCode">The virtual key code or scan code for a key. How this value is interpreted depends on the value of the uMapType parameter.</param>
        /// <param name="uMapType">The translation to be performed. The value of this parameter depends on the value of the uCode parameter.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, MAPVK uMapType);

        /// <summary>
        /// Translates (maps) a virtual-key code into a scan code or character value, or translates a scan code into a virtual-key code. The function translates the codes using the input language and an input locale identifier.
        /// </summary>
        /// <param name="uCode">
        /// The virtual-key code or scan code for a key. How this value is interpreted depends on the value of the uMapType parameter.
        /// Starting with Windows Vista, the high byte of the uCode value can contain either 0xe0 or 0xe1 to specify the extended scan code.
        /// </param>
        /// <param name="uMapType">The translation to perform. The value of this parameter depends on the value of the uCode parameter.</param>
        /// <param name="dwhkl">Input locale identifier to use for translating the specified code. This parameter can be any input locale identifier previously returned by the LoadKeyboardLayout function.</param>
        /// <returns>The return value is either a scan code, a virtual-key code, or a character value, depending on the value of uCode and uMapType. If there is no translation, the return value is zero.</returns>
        [DllImport("user32.dll")]
        public static extern uint MapVirtualKeyEx(uint uCode, MAPVK uMapType, IntPtr dwhkl);

        [DllImport("user32.dll")]
        public static extern IntPtr GetKeyboardLayout(uint idThread);

        /// <summary>
        /// Synthesizes keystrokes, mouse motions, and button clicks.
        /// </summary>
        /// <param name="nInputs">The number of structures in the pInputs array.</param>
        /// <param name="pInputs">An array of INPUT structures. Each structure represents an event to be inserted into the keyboard or mouse input stream.</param>
        /// <param name="cbSize">The size, in bytes, of an INPUT structure. If cbSize is not the size of an INPUT structure, the function fails.</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        /// <summary>
        /// Retrieves the identifier of the thread that created the specified window and, optionally, the identifier of the process that created the window.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="lpdwProcessId">A pointer to a variable that receives the process identifier. If this parameter is not NULL, GetWindowThreadProcessId copies the identifier of the process to the variable; otherwise, it does not.</param>
        /// <returns>The return value is the identifier of the thread that created the window.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);


        /// <summary>
        /// Waits until the specified process has finished processing its initial input and is waiting for user input with no input pending, or until the time-out interval has elapsed.
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="dwMilliseconds"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int WaitForInputIdle(IntPtr hProcess, int dwMilliseconds);

        /// <summary>process-specific access rights.</summary>
        public enum PROCESS : uint
        {
            PROCESS_ALL_ACCESS = 0x001F0FFF,
            PROCESS_CREATE_PROCESS = 0x0080,
            PROCESS_CREATE_THREAD = 0x0002,
            PROCESS_DUP_HANDLE = 0x0040,
            PROCESS_QUERY_INFORMATION = 0x0400,
            PROCESS_QUERY_LIMITED_INFORMATION = 0x1000,
            PROCESS_SET_INFORMATION = 0x0200,
            PROCESS_SET_QUOTA = 0x0100,
            PROCESS_SUSPEND_RESUME = 0x0800,
            PROCESS_TERMINATE = 0x0001,
            PROCESS_VM_OPERATION = 0x0008,
            PROCESS_VM_READ = 0x0010,
            PROCESS_VM_WRITE = 0x0020,
            PROCESS_SYNCHRONIZE = 0x00100000,
        }


        /// <summary>
        /// Opens an existing local process object.
        /// </summary>
        /// <param name="dwDesiredAccess">The access to the process object. This access right is checked against the security descriptor for the process. This parameter can be one or more of the process access rights.</param>
        /// <param name="bInheritHandle">If this value is TRUE, processes created by this process will inherit the handle. Otherwise, the processes do not inherit this handle.</param>
        /// <param name="dwProcessId">The identifier of the local process to be opened.</param>
        /// <returns>If the function succeeds, the return value is an open handle to the specified process.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(PROCESS dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hSnapshot);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr intPtr, uint Msg, int textLength, StringBuilder text);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr intPtr, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr PostMessage(IntPtr intPtr, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr intPtr, uint Msg, int wParam, int lParam);

        /// <summary>Contains information about a GUI thread.</summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct GUITHREADINFO
        {
            public int cbSize;
            public uint flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public RECT rcCaret;
        }

        /// <summary>Retrieves information about the active window or a specified GUI thread.</summary>
        /// <param name="idThread">The identifier for the thread for which information is to be retrieved. To retrieve this value, use the GetWindowThreadProcessId function. If this parameter is NULL, the function returns information for the foreground thread.</param>
        /// <param name="lpgui">A pointer to a GUITHREADINFO structure that receives information describing the thread. Note that you must set the cbSize member to sizeof(GUITHREADINFO) before calling this function.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetGUIThreadInfo(uint idThread, out GUITHREADINFO lpgui);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        /// <summary>Determines whether the specified window handle identifies an existing window.</summary>
        /// <param name="hWnd">A handle to the window to be tested.</param>
        /// <returns>If the window handle identifies an existing window, the return value is nonzero.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(IntPtr hWnd);

        /// <summary>
        /// An application-defined callback function used with the EnumWindows or EnumDesktopWindows function. It receives top-level window handles. The WNDENUMPROC type defines a pointer to this callback function. EnumWindowsProc is a placeholder for the application-defined function name.
        /// </summary>
        /// <param name="hwnd">A handle to a top-level window.</param>
        /// <param name="lParam">The application-defined value given in EnumWindows or EnumDesktopWindows.</param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        /// <summary>
        /// Enumerates all top-level windows on the screen by passing the handle to each window, in turn, to an application-defined callback function. EnumWindows continues until the last top-level window is enumerated or the callback function returns FALSE.
        /// </summary>
        /// <param name="lpEnumFunc">A pointer to an application-defined callback function. For more information, see EnumWindowsProc.</param>
        /// <param name="lParam">An application-defined value to be passed to the callback function.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        /// <summary>
        /// An application-defined callback function used with the EnumChildWindows function. It receives the child window handles. The WNDENUMPROC type defines a pointer to this callback function. EnumChildProc is a placeholder for the application-defined function name.
        /// </summary>
        /// <param name="hWnd">A handle to a child window of the parent window specified in EnumChildWindows.</param>
        /// <param name="lParam">The application-defined value given in EnumChildWindows.</param>
        /// <returns>To continue enumeration, the callback function must return TRUE; to stop enumeration, it must return FALSE.</returns>
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate bool EnumChildProc(IntPtr hWnd, IntPtr lParam);

        /// <summary>
        /// Enumerates the child windows that belong to the specified parent window by passing the handle to each child window, in turn, to an application-defined callback function. EnumChildWindows continues until the last child window is enumerated or the callback function returns FALSE.
        /// </summary>
        /// <param name="hwndParent">A handle to the parent window whose child windows are to be enumerated. If this parameter is NULL, this function is equivalent to EnumWindows.</param>
        /// <param name="lpEnumFunc">A pointer to an application-defined callback function. For more information, see EnumChildProc.</param>
        /// <param name="lParam">An application-defined value to be passed to the callback function.</param>
        /// <returns>The return value is not used.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwndParent, EnumChildProc lpEnumFunc, IntPtr lParam);

        /// <summary>
        /// Determines the visibility state of the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window to be tested.</param>
        /// <returns>If the specified window, its parent window, its parent's parent window, and so forth, have the WS_VISIBLE style, the return value is nonzero. Otherwise, the return value is zero.</returns>
        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        // URL: http://msdn.microsoft.com/en-us/library/windows/desktop/ff468926(v=vs.85).aspx

        public enum GWL : int
        {
            GWL_EXSTYLE = -20,  //Sets a new extended window style.
            GWL_STYLE = -16,    //Sets a new application instance handle.
            GWL_WNDPROC = -4,   //Sets a new identifier of the child window. The window cannot be a top-level window.
            GWL_HINSTANCE = -6, //Sets a new window style.
            GWL_ID = -12,       //Sets the user data associated with the window. This data is intended for use by the application that created the window. Its value is initially zero.
            GWL_USERDATA = -21  //Sets a new address for the window procedure. You cannot change this attribute if the window does not belong to the same process as the calling thread.
        }

        /// <summary>
        /// Retrieves information about the specified window. The function also retrieves the 32-bit (DWORD) value at the specified offset into the extra window memory.
        /// Note  If you are retrieving a pointer or a handle, this function has been superseded by the GetWindowLongPtr function. (Pointers and handles are 32 bits on 32-bit Windows and 64 bits on 64-bit Windows.) To write code that is compatible with both 32-bit and 64-bit versions of Windows, use GetWindowLongPtr.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="nIndex"></param>
        /// <returns>The zero-based offset to the value to be retrieved. Valid values are in the range zero through the number of bytes of extra window memory, minus four; for example, if you specified 12 or more bytes of extra memory, a value of 8 would be an index to the third 32-bit integer. To retrieve any other value, specify one of the following values.</returns>
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong32(IntPtr hWnd, GWL nIndex);
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        public static extern int GetWindowLong64(IntPtr hWnd, GWL nIndex);

        public static int GetWindowLong(IntPtr hWnd, GWL nIndex) {
            if (IntPtr.Size == 8)
                return GetWindowLong64(hWnd, nIndex);
            else
                return GetWindowLong32(hWnd, nIndex);
        }

        public enum WS : long {
            WS_BORDER = 0x00800000L,	    // The window has a thin-line border.
            WS_CAPTION = 0x00C00000L,	    // The window has a title bar (includes the WS_BORDER style).
            WS_CHILD = 0x40000000L,	        // The window is a child window. A window with this style cannot have a menu bar. This style cannot be used with the WS_POPUP style.
            WS_CHILDWINDOW = 0x40000000L,	// Same as the WS_CHILD style.
            WS_CLIPCHILDREN = 0x02000000L,	// Excludes the area occupied by child windows when drawing occurs within the parent window. This style is used when creating the parent window.
            WS_CLIPSIBLINGS = 0x04000000L,	// Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message, the WS_CLIPSIBLINGS style clips all other overlapping child windows out of the region of the child window to be updated. If WS_CLIPSIBLINGS is not specified and child windows overlap, it is possible, when drawing within the client area of a child window, to draw within the client area of a neighboring child window.
            WS_DISABLED = 0x08000000L,	    // The window is initially disabled. A disabled window cannot receive input from the user. To change this after a window has been created, use the EnableWindow function.
            WS_DLGFRAME = 0x00400000L,	    // The window has a border of a style typically used with dialog boxes. A window with this style cannot have a title bar.
            WS_GROUP = 0x00020000L,	        // The window is the first control of a group of controls. The group consists of this first control and all controls defined after it, up to the next control with the WS_GROUP style. The first control in each group usually has the WS_TABSTOP style so that the user can move from group to group. The user can subsequently change the keyboard focus from one control in the group to the next control in the group by using the direction keys. You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function.
            WS_HSCROLL = 0x00100000L,	    // The window has a horizontal scroll bar.
            WS_ICONIC = 0x20000000L,	    // The window is initially minimized. Same as the WS_MINIMIZE style.
            WS_MAXIMIZE = 0x01000000L,	    // The window is initially maximized.
            WS_MAXIMIZEBOX = 0x00010000L,	// The window has a maximize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.
            WS_MINIMIZE = 0x20000000L,	    // The window is initially minimized. Same as the WS_ICONIC style.
            WS_MINIMIZEBOX = 0x00020000L,	// The window has a minimize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.
            WS_OVERLAPPED = 0x00000000L,	// The window is an overlapped window. An overlapped window has a title bar and a border. Same as the WS_TILED style. WS_OVERLAPPEDWINDOW (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX) The window is an overlapped window. Same as the WS_TILEDWINDOW style.
            WS_POPUP = 0x80000000L,	        // The windows is a pop-up window. This style cannot be used with the WS_CHILD style. WS_POPUPWINDOW (WS_POPUP | WS_BORDER | WS_SYSMENU) The window is a pop-up window. The WS_CAPTION and WS_POPUPWINDOW styles must be combined to make the window menu visible.
            WS_SIZEBOX = 0x00040000L,	    // The window has a sizing border. Same as the WS_THICKFRAME style.
            WS_SYSMENU = 0x00080000L,	    // The window has a window menu on its title bar. The WS_CAPTION style must also be specified.
            WS_TABSTOP = 0x00010000L,	    // The window is a control that can receive the keyboard focus when the user presses the TAB key. Pressing the TAB key changes the keyboard focus to the next control with the WS_TABSTOP style. You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function. For user-created windows and modeless dialogs to work with tab stops, alter the message loop to call the IsDialogMessage function.
            WS_THICKFRAME = 0x00040000L,	// The window has a sizing border. Same as the WS_SIZEBOX style.
            WS_TILED = 0x00000000L,	        // The window is an overlapped window. An overlapped window has a title bar and a border. Same as the WS_OVERLAPPED style. WS_TILEDWINDOW (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX) The window is an overlapped window. Same as the WS_OVERLAPPEDWINDOW style.
            WS_VISIBLE = 0x10000000L,	    // The window is initially visible. This style can be turned on and off by using the ShowWindow or SetWindowPos function.
            WS_VSCROLL = 0x00200000L,	    // The window has a vertical scroll bar.
        }

        /// <summary>
        /// Sets the specified window's show state.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="nCmdShow">Controls how the window is to be shown. This parameter is ignored the first time an application calls ShowWindow, if the program that launched the application provides a STARTUPINFO structure. Otherwise, the first time ShowWindow is called, the value should be the value obtained by the WinMain function in its nCmdShow parameter. In subsequent calls, this parameter can be one of the following values.</param>
        /// <returns>If the window was previously visible, the return value is nonzero.</returns>
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, SW nCmdShow);

        /// <summary>
        /// Changes the position and dimensions of the specified window. For a top-level window, the position and dimensions are relative to the upper-left corner of the screen. For a child window, they are relative to the upper-left corner of the parent window's client area.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="X">The new position of the left side of the window.</param>
        /// <param name="Y">The new position of the top of the window.</param>
        /// <param name="nWidth">The new width of the window.</param>
        /// <param name="nHeight">The new height of the window.</param>
        /// <param name="bRepaint">Indicates whether the window is to be repainted. If this parameter is TRUE, the window receives a message. If the parameter is FALSE, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of moving a child window.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        /// <summary>
        /// The current show state of the window.
        /// </summary>
        public enum SW : int {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11
        }

        public enum TH32CS : uint {
            TH32CS_SNAPHEAPLIST = 0x00000001,
            TH32CS_SNAPPROCESS = 0x00000002,
            TH32CS_SNAPTHREAD = 0x00000004,
            TH32CS_SNAPMODULE = 0x00000008,
            TH32CS_SNAPMODULE32 = 0x00000010,
            TH32CS_SNAPALL = 15,
            TH32CS_INHERIT = 0x80000000
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateToolhelp32Snapshot(TH32CS dwFlags, uint th32ProcessID);

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESSENTRY32 {
            public uint dwSize;
            public uint cntUsage;
            public int th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        }

        [DllImport("kernel32.dll")]
        public static extern int Process32First(IntPtr hSnapshot, out PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        public static extern int Process32Next(IntPtr hSnapshot, out PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        public enum WAIT : uint {
            WAIT_TIMEOUT = 0x00000102,
            WAIT_FAILED = 0xFFFFFFFF
        }
    }
}
