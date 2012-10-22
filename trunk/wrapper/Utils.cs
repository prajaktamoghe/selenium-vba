using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SeleniumWrapper
{
    static class Utils
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern short GetKeyState(int virtualKeyCode);

        [DllImport("User32.dll")]
        private static extern int SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern int GetForegroundWindow();

        internal static void maximizeForegroundWindow(){
            ShowWindow(GetForegroundWindow(), 3 /*SW_SHOWMAXIMIZED*/);
        }

        internal static bool isEscapeKeyPressed(){
            return (GetKeyState(0x1b) & 0x8000) != 0;
        }

        internal static bool runShellCommand(string command){
            System.Diagnostics.Process p = new System.Diagnostics.Process {
                StartInfo = new System.Diagnostics.ProcessStartInfo {
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = (@"CMD /C " + command)
                }
            };
            p.Start();
            p.WaitForExit(5000);
            if (p.HasExited == false){ //Check to see if the process is still running.
                if (p.Responding){//Test to see if the process is hung up.
                    p.CloseMainWindow();//Process was responding; close the main window.
                    return false;
                }else{
                    p.Kill(); //Process was not responding; force the process to close.
                    return false;
                }
            }
            return true;
        }





    }
}
