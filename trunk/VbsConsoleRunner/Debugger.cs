using System;
using vbsc;
using System.Text;

namespace vbsc {

    class Debugger {

        public enum Cmd { Continue, Exit, Retry, Next }

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
                    var cmd = StdInOut.ConsoleIn.ReadLine().Replace("print ", "echo ");
                    if (cmd.Equals("n")) return Cmd.Next;
                    if (!compiler.AddCode(cmd, true))
                        StdInOut.ConsoleOut.WriteLine(compiler.Error.Message);
                }
            } finally {
                compiler.WScript.OnEcho -= StdInOut.ConsoleOut.WriteLine;
            }
        }

    }
}
