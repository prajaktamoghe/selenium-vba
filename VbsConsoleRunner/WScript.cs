using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace vbsc {

    [ComVisible(true)]
    public class WScript {

        private readonly string _filepath;
        public readonly ArrayList Arguments;

        public delegate void OnEcho_Delegate(object message);
        public event OnEcho_Delegate OnEcho;

        internal WScript(string filepath, string[] arguments) {
            _filepath = filepath;
            Arguments = new ArrayList(arguments ?? new string[0]);
        }

        internal WScript() {
            _filepath = null;
            Arguments = null;
        }

        public void Echo(object message) {
            if (OnEcho == null)
                return;
            var sb = new StringBuilder();
            if (!(message is string) && message is ICollection) {
                int i = 0;
                foreach (var v in (ICollection)message) {
                    if (++i > 20) {
                        sb.AppendLine("... (" + (((ICollection)message).Count - i + 1).ToString() + " more)");
                        break;
                    }
                    if (v is string)
                        sb.AppendLine('"' + (v ?? String.Empty).ToString() + '"');
                    else
                        sb.AppendLine((v ?? String.Empty).ToString());
                }
            } else {
                sb.AppendLine((message ?? String.Empty).ToString());
            }
            OnEcho(sb.ToString().CleanEnd());
        }

        public string FullName {
            get { return System.Reflection.Assembly.GetExecutingAssembly().Location; }
        }

        public string Path {
            get { return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); }
        }

        public string ScriptPath {
            get { return _filepath == null ? null : System.IO.Path.GetDirectoryName(_filepath); }
        }

        public string ScriptName {
            get { return _filepath == null ? null : System.IO.Path.GetFileName(_filepath); }
        }

        public string ScriptFullName {
            get { return _filepath ?? string.Empty; }
        }

        public TextWriter StdErr {
            get { return Console.Out; }
        }
        public TextReader StdIn {
            get { return Console.In; }
        }

        public TextWriter StdOut {
            get { return Console.Out; }
        }

        public void Sleep(int timems) {
            Thread.Sleep(timems);
        }

    }
}
