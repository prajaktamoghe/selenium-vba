using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace vbsc {

    static class StdInOut {

        public static bool HideInfo { get; set; }

        private static readonly StringWriter _stdout;
        private static readonly TextWriter _console_stdout;
        private static readonly TextWriter _console_stderr;
        private static readonly TextReader _console_stdin;
        private static readonly StringBuilder _logfile_text;

        static StdInOut() {
            HideInfo = false;
            _stdout = new StringWriter();
            _console_stdout = Console.Out;
            _console_stderr = Console.Error;
            _console_stdin = Console.In;
            Console.SetError(_stdout);
            Console.SetOut(_stdout);
            _logfile_text = new StringBuilder();
        }

        public static TextWriter ConsoleOut {
            get{ return _console_stdout; }
        }

        public static TextReader ConsoleIn {
            get { return _console_stdin; }
        }

        public static void LogLine(string text, bool fileonly = false) {
            if (!fileonly) {
                lock (_console_stdout){
                    _console_stdout.WriteLine(text);
                }
            }
            _logfile_text.AppendLine(text);
        }

        public static void LogStart(DateTime starttime) {
            LogLine("START " + starttime.ToString("yyyy/MM/dd HH:mm:ss") + "\r\n");
        }

        public static void LogException(Exception ex, string[] arguments) {
            var sb = new StringBuilder();
            sb.AppendLine("[Exception] " + System.Reflection.Assembly.GetExecutingAssembly().Location);
            sb.AppendLine("------------------------------------------------------------------");
            sb.AppendLine(ex.Message.Trim('\r', '\n', ' '));
            sb.AppendLine("\r\nArguments:" + FormatArguments(arguments));
            sb.AppendLine("\r\nStackTrace:\r\n" + ex.StackTrace.ToString());
            sb.AppendLine("\r\n******************************************************************");
            sb.Append("FAILED");
            LogLine(sb.ToString());
        }

        public static void LogError(string title, string info) {
            var sb = new StringBuilder();
            sb.AppendLine("[Error] " + title);
            sb.AppendLine("------------------------------------------------------------------");
            sb.AppendLine(info);
            sb.AppendLine("\r\n******************************************************************");
            sb.Append("FAILED");
            LogLine(sb.ToString());
        }

        private static string FormatArguments(string[] arguments) {
            var sb = new StringBuilder();
            if (arguments.Length != 0) {
                foreach (var arg in arguments) {
                    if (arg.Trim().Length != 0)
                        sb.Append("\r\n " + arg);
                }
            }
            return sb.ToString();
        }

        internal static void LogScriptException(Script script, Exception ex) {
            LogLine(
                "\r\n[Exception] " + script.SourceName
                + "\r\n------------------------------------------------------------------"
                + "\r\n" + ex.Message.Trim('\r', '\n', ' ')
                + "\r\n\r\nStackTrace:\r\n" + ex.StackTrace.ToString()
                + "\r\n"
            );
        }

        internal static void LogScriptError(string source, string text) {
            LogLine(
                "\r\n[Fail] " + source
                + "\r\n------------------------------------------------------------------"
                + "\r\n" + text.CleanEnd()
                + "\r\n"
            );
        }

        internal static void LogScriptInfo(string source, string text) {
            LogLine(
                "\r\n[Info] " + source
                + "\r\n------------------------------------------------------------------"
                + "\r\n" + text.CleanEnd()
                + "\r\n"
            , HideInfo);
        }

        public static void SaveTo(string filepath) {
            using (var writer = new StreamWriter(new FileStream(filepath, FileMode.Append, FileAccess.Write, FileShare.None)))
                writer.Write(_logfile_text.ToString() + "\r\n\r\n");
        }

        internal static void LogResults(List<ScriptResult> results, DateTime starttime, DateTime endtime) {
            var sb_succeed = new StringBuilder();
            var sb_error = new StringBuilder();
            int nbrunned = 0, nberror = 0;
            foreach (var result in results) {
                nbrunned++;
                (result.Succeed ? sb_succeed : sb_error).AppendLine(' ' + result.SourceName);
                if (!result.Succeed)
                    nberror++;
            }
            var sb_results = new StringBuilder();
            sb_results.Append("\r\n[Results summary]");
            sb_results.Append("\r\n------------------------------------------------------------------\r\n");
            if (sb_error.Length != 0)
                sb_results.Append("Failed:\r\n" + sb_error.ToString());
            if (sb_succeed.Length != 0)
                sb_results.Append("Succeed:\r\n" + sb_succeed.ToString());
            sb_results.Append("\r\n******************************************************************\r\n");
            sb_results.Append(string.Format("{0}: {1} failed / {2} runned in {3}s",
                    nberror == 0 ? "SUCCEED" : "FAILED",
                    nberror,
                    nbrunned,
                    Math.Round(endtime.Subtract(starttime).TotalSeconds, 2)
                ));
            LogLine(sb_results.ToString());
        }

    }
}
