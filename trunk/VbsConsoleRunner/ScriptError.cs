using System.Collections.Generic;
using System.Text;

namespace vbsc {

    class ScriptError : ScriptResult {

        public List<TraceLine> StackTrace { get; private set; }
        public string Message { get; private set; }
        public string Information { get; private set; }

        public ScriptError(Script script, ProcedureItem procedure, string err_message)
            : base(script, procedure) {
            StackTrace = new List<TraceLine>();
            Message = err_message.CleanEnd();
            Information = string.Empty;
        }

        public override bool Succeed {
            get { return false; }
        }

        public override string SourceName {
            get {
                var title = new StringBuilder();
                if (this.Procedure != null)
                    title.Append(this.Procedure.SourceName + ' ');
                title.Append("line" + this.LineNumber.ToString() + " in " +  this.Script.SourceName);
                return title.ToString();
            }
        }

        internal void AddTraceLine(Script script, int line_number) {
            StackTrace.Add(new TraceLine(script, line_number));
        }

        internal void FillParentTrace() {
            var last_trace = StackTrace.GetLast();
            if (last_trace == null) return;
            var script = last_trace.Script;
            while (script.ParentScript != null) {
                AddTraceLine(script.ParentScript, script.ParentLineNumber);
                script = script.ParentScript;
            }
        }

        private int _error_line = 0;

        public int LineNumber {
            get {
                if (this._error_line == 0) {
                    foreach (var trace in StackTrace) {
                        if (trace.Script.Path == Script.Path) {
                            this._error_line = trace.LineNumber;
                            break;
                        }
                    }
                }
                return this._error_line;
            }
        }

        internal void AddInformation(string p) {
            this.Information = new StringBuilder(this.Information).AppendLine(p.CleanEnd()).ToString(); ;
        }

        public override string ToString() {
            var text = new StringBuilder();
            text.AppendLine(this.Message.CleanEnd());
            foreach (var trace in StackTrace) {
                text.AppendLine(" at " + trace.Script.Path + " line" + trace.LineNumber + ":");
                text.AppendLine("  " + trace.LineOfCode);
            }
            if (this.Information.Length != 0)
                text.Append(this.Information.CleanEnd());
            return text.ToString();
        }

    }
}
