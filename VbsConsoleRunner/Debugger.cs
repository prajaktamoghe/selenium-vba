using System;
using vbsc;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace vbsc {

    class Debugger {

        public enum Cmd { Continue, Exit, Retry, Next }

        static Debugger() {
            EnableQuickMode();
        }

        public static Cmd Debug(Compiler compiler = null) {
            if (compiler == null) {
                compiler = new Compiler();
                compiler.Compile(new WScript());
            } else {
                StdInOut.ConsoleOut.WriteLine("Debug mode: type n for the next execution or evaluate the code.");
            }
            try {
                compiler.WScript.OnEcho += StdInOut.ConsoleOut.WriteLine;
                while (true) {
                    StdInOut.ConsoleOut.Write(">");
                    var code = new StringBuilder();
                    while (true) {
                        var line = StdInOut.ConsoleIn.ReadLine().Replace("print ", "echo ");
                        if (line.Equals("n")) return Cmd.Next;
                        if (line == null || line == string.Empty) break;
                        code.AppendLine(line);
                        if ((Control.ModifierKeys & Keys.Shift) != 0) break;
                        StdInOut.ConsoleOut.Write(" ");
                    }
                    if (code.Length != 0 && !compiler.AddCode(code.ToString(), true))
                        StdInOut.ConsoleOut.WriteLine(compiler.Error.Message);
                }
            } finally {
                compiler.WScript.OnEcho -= StdInOut.ConsoleOut.WriteLine;
            }
        }


        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetStdHandle(int handle);

        static void EnableQuickMode() {
            const int STD_INPUT_HANDLE = -10;
            const uint ENABLE_QUICK_EDIT_MODE = 0x40 | 0x80;
            uint mode;
            IntPtr handle = GetStdHandle(STD_INPUT_HANDLE);
            GetConsoleMode(handle, out mode);
            mode |= ENABLE_QUICK_EDIT_MODE;
            SetConsoleMode(handle, mode);
        }

    }
}
