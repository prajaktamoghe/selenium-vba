
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace SeleniumWrapper
{
    /// <summary> Low-level keyboard intercept class to trap and suppress system keys. </summary>
    public class KeyboardHook : IDisposable {

        public delegate void EscapeKeyPressedEventHandler();
        private IntPtr hookID = IntPtr.Zero;
        private User32.HookHandlerDelegate proc;

        public event EscapeKeyPressedEventHandler escapeKeyPressed;

        /// <summary>
        /// Sets up a keyboard hook to trap all keystrokes without 
        /// passing any to other applications.
        /// </summary>
        public KeyboardHook() {
            proc = new User32.HookHandlerDelegate(HookCallback);
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule) {
                hookID = User32.SetWindowsHookEx(User32.WH_KEYBOARD_LL, proc,  User32.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        /// <summary>
        /// Processes the key event captured by the hook.
        /// </summary>
        private IntPtr HookCallback( int nCode, IntPtr wParam, ref User32.KBDLLHOOKSTRUCT lParam) {
            if (nCode >= 0)  {
                if (wParam == (IntPtr)User32.WM_KEYDOWN || wParam == (IntPtr)User32.WM_SYSKEYDOWN) {
                    if (lParam.vkCode == 0x1B){
                        if (escapeKeyPressed != null) escapeKeyPressed();
                    }
                }
            }
            return User32.CallNextHookEx(hookID, nCode, wParam, ref lParam);
        }

        /// <summary>
        /// Releases the keyboard hook.
        /// </summary>
        public void Dispose() {
            User32.UnhookWindowsHookEx(hookID);
        }

    }


    [ComVisibleAttribute(false), System.Security.SuppressUnmanagedCodeSecurity()]
    internal class User32
    {

        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_SYSKEYDOWN = 0x104;
        public const int WM_SYSKEYUP = 0x105;
        public const int WH_KEYBOARD_LL = 13;

        internal struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            int scanCode;
            public int flags;
            int time;
            int dwExtraInfo;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookHandlerDelegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        internal delegate IntPtr HookHandlerDelegate(int nCode, IntPtr wParam, ref User32.KBDLLHOOKSTRUCT lParam);

    }
}

