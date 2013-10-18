using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Automation.Text;

namespace SeleniumWrapper.Automation {
    public static class NatHelper {

        /// <summary>Wait for the action function to return the value</summary>
        /// <param name="action">Action to execute. (T)=>... </param>
        /// <param name="value">Value that will be tested</param>
        /// <param name="timeout">Timeout in millisecond</param>
        /// <returns>The result of the action</returns>
        public static T Wait<T>(Func<T> action, T value, int timeout = 10000) {
            var endTime = DateTime.Now.AddMilliseconds(timeout);
            while (true) {
                T ret = action();
                if (object.Equals( ret, value)) return ret;
                if (DateTime.Now > endTime) throw new TimeoutException();
                Thread.Sleep(20);
            }
        }

        /// <summary>Wait for the action function to return other than the value</summary>
        /// <param name="action">Action to execute. (T)=>... </param>
        /// <param name="value">Value that will be tested</param>
        /// <param name="timeout">Timeout in millisecond</param>
        /// <returns>The result of the action</returns>
        public static T WaitNot<T>(Func<T> action, T value, int timeout = 10000) {
            var endTime = DateTime.Now.AddMilliseconds(timeout);
            while (true) {
                T ret = action();
                if (!object.Equals(ret, value)) return ret;
                if (DateTime.Now > endTime) throw new TimeoutException();
                Thread.Sleep(20);
            }
        }

        /// <summary>Test if a string matches a pattern by using wildcard pattern</summary>
        /// <param name="pattern">Wildcard pattern (*=any characters ?=one character)</param>
        /// <param name="text">Text to test</param>
        /// <returns>True if the string matches the pattern, false otherwise</returns>
        public static bool WildcardMatch(String pattern, String text) {
            if (pattern.Length == 0 && text.Length == 0)
                return true;
            if (text.Length == 0 && pattern[0] != '*')
                return false;
            if (pattern.Length == 0 && text.Length != 0)
                return false;
            if (pattern[0] == '*' && pattern.Length != 1 && text.Length == 0)
                return false;

            if (pattern[0] == '*' && pattern.Length == 1 && text.Length == 0)
                return true;
            else if (pattern[0] == '?' || pattern[0] == text[0])
                return WildcardMatch(pattern.Substring(1), text.Substring(1));
            else if (pattern[0] == '*')
                return WildcardMatch(pattern, text.Substring(1)) || WildcardMatch(pattern.Substring(1), text);
            else if (pattern[0] != text[0])
                return false;

            return false;
        }

        /// <summary>Get an element text</summary>
        /// <param name="element">AutomationElement</param>
        /// <returns>Text</returns>
        public static string GetText(this AutomationElement element) {
            if ((bool)element.GetCurrentPropertyValue(AutomationElement.IsTextPatternAvailableProperty)) {
                object obj;
                TextPatternRange documentRange = null;
                if (element.TryGetCurrentPattern(TextPattern.Pattern, out obj)) {
                    documentRange = ((TextPattern)obj).DocumentRange;
                    return documentRange.GetText(-1);
                }
            }
            return (string)element.GetCurrentPropertyValue(AutomationElement.NameProperty);
        }

        /// <summary>Get the process id of a window</summary>
        /// <param name="hWnd">Window handle</param>
        /// <returns>Process id</returns>
        public static int GetProcessId(IntPtr hWnd) {
            int pid;
            Win32.GetWindowThreadProcessId(hWnd, out pid);
            return pid;
        }

        /// <summary>Get a window's text</summary>
        /// <param name="hWnd">Window's handle</param>
        /// <returns>Text</returns>
        public static String GetText(IntPtr hWnd) {
            int length = Win32.SendMessage(hWnd, 0x0E /*WM_GETTEXTLENGTH*/, 0, 0);
            var sb = new StringBuilder(length + 1);
            Win32.SendMessage(hWnd, 0x0D /*WM_GETTEXT*/, sb.Capacity, sb);
            return sb.ToString();
        }

        /// <summary>Set a window text</summary>
        /// <param name="hWnd">Window's handle</param>
        /// <param name="text">Text to set</param>
        public static void SetText(IntPtr hWnd, string text) {
            /*
            var sb = new StringBuilder(text, text.Length + 1);
            SendMessage(hWnd, 0x0C /WM_SETTEXT/, IntPtr.Zero, sb);
            */
            IntPtr textPtr = Marshal.StringToCoTaskMemUni(text);
            Win32.SendMessage(hWnd, 0x0C /*WM_SETTEXT*/, IntPtr.Zero, textPtr);
            Marshal.FreeCoTaskMem(textPtr);
        }

        /// <summary>Get the handle of the element having the focus</summary>
        /// <param name="winHwnd">Window's handle</param>
        /// <returns>Element handle</returns>
        public static IntPtr GetFocusedElement(IntPtr winHwnd) {
            int pid;
            uint threadId = Win32.GetWindowThreadProcessId(winHwnd, out pid);
            var info = new Win32.GUITHREADINFO();
            info.cbSize = Marshal.SizeOf(info);
            Win32.GetGUIThreadInfo(threadId, out info);
            return info.hwndFocus;
        }

        /// <summary>Get an element by searching on the text</summary>
        /// <param name="hwndParent">Window's handle</param>
        /// <param name="text">Text to search</param>
        /// <returns>Element's handle</returns>
        public static IntPtr GetElementByText(IntPtr hwndParent, string text) {
            var matchHwndList = new List<IntPtr>();
            Win32.EnumChildProc callback = (hwnd, lParam) => {
                if (Win32.IsWindowVisible(hwnd)) {
                    if (WildcardMatch(text, NatHelper.GetText(hwnd))) {
                        matchHwndList.Add(hwnd);
                        return false;
                    }
                }
                return true;
            };
            Win32.EnumChildWindows(hwndParent, callback, IntPtr.Zero);
            if (matchHwndList.Count != 0)
                return matchHwndList[0];
            return IntPtr.Zero;
        }

        /// <summary>Get the window having the provided title</summary>
        /// <param name="title">Title to search</param>
        /// <returns>Window's handle</returns>
        public static IntPtr GetWindowByTitle(string title) {
            var foregroundHwnd = Win32.GetForegroundWindow();
            string foregroundText = NatHelper.GetText(foregroundHwnd);
            if (foregroundText == title)
                return foregroundHwnd;

            var findHwnd = Win32.FindWindow(null, title);
            if (findHwnd != IntPtr.Zero)
                return findHwnd;
            var matchHwndList = new List<IntPtr>();
            Win32.EnumWindowsProc callback = (hwnd, lParam) => {
                if (Win32.IsWindowVisible(hwnd)) {
                    if (WildcardMatch(title, NatHelper.GetText(hwnd))) {
                        matchHwndList.Add(hwnd);
                        return false;
                    }
                }
                return true;
            };
            Win32.EnumWindows(callback, new IntPtr());
            if (matchHwndList.Count != 0)
                return matchHwndList[0];
            return IntPtr.Zero;
        }

        /// <summary>Get the window(s rectangle</summary>
        /// <param name="hWnd">Window's handle</param>
        /// <returns>Rectangle</returns>
        public static Win32.RECT GetWindowRect(IntPtr hWnd) {
            Win32.RECT rect;
            Win32.GetWindowRect(hWnd, out rect);
            return rect;
        }

        /// <summary>Get the process identifier from the program name</summary>
        /// <param name="program">Programe name</param>
        /// <returns>Process ID</returns>
        public static int GetProcessId(String program) {
            IntPtr handle = IntPtr.Zero;
            String applicationName = program.ToLower();
            try {
                handle = Win32.CreateToolhelp32Snapshot(Win32.TH32CS.TH32CS_SNAPPROCESS, 0);
                Win32.PROCESSENTRY32 info;
                info.dwSize = (uint)Marshal.SizeOf(typeof(Win32.PROCESSENTRY32));
                int first = Win32.Process32First(handle, out info);
                if (first == 0) throw new Exception("Can not find first process.");
                do {
                    if (string.Compare(info.szExeFile, applicationName, true) == 0)
                        return info.th32ProcessID;
                } while (Win32.Process32Next(handle, out info) != 0);
            } finally {
                if (handle != IntPtr.Zero) Win32.CloseHandle(handle);
            }
            return 0;
        }

        /// <summary>Terminate a process</summary>
        /// <param name="pid">Process identifier</param>
        /// <param name="timeout">Timeout</param>
        /// <returns>True if succeed, false otherwise</returns>
        public static bool KillProcess(int pid, int timeout) {
            IntPtr hProcess = IntPtr.Zero;
            try {
                hProcess = Win32.OpenProcess(Win32.PROCESS.PROCESS_TERMINATE, false, pid);
                Win32.TerminateProcess(hProcess, (uint)0);
                return Win32.WaitForSingleObject(hProcess, (uint)timeout) == (uint)Win32.WAIT.WAIT_FAILED;
            } finally {
                if (hProcess != IntPtr.Zero) Win32.CloseHandle(hProcess);
            }
        }

        /// <summary>Find an element using a path</summary>
        /// <param name="hwnd">Window's handle</param>
        /// <param name="path">path or element text</param>
        /// <returns>Element</returns>
        public static AutomationElement FindFirstByPath(IntPtr hwnd, string path) {
            var win = AutomationElement.FromHandle(hwnd);
            AutomationElement element = win.FindFirstByPath(path);
            if(element==null){
                //Search on windows attached to the desktop and having the same pid
                foreach (AutomationElement ele in win.FindOtherTopElements()) {
                    element = ele.FindFirstByPath(path);
                    if(element!=null) break;
                }
            }
            return element;
        }

    }
}
