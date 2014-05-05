using MSScriptControl;

namespace vbsc {

    class Compiler {

        public delegate void OnError_Delegate(ScriptError error);
        public event OnError_Delegate OnError;

        private readonly ScriptControlClass _compiler;
        private Script _script = null;
        private ProcedureItem _procedure = null;
        private ScriptError _error = null;
        private object _result = null;
        private int _err_line = 0;
        private WScript _wscript;

        public Compiler() {
            _compiler = new ScriptControlClass();
            _compiler.Timeout = -1;
            _compiler.Language = "VBScript";
            _compiler.UseSafeSubset = false;
            _compiler.AllowUI = true;
            _compiler.DScriptControlSource_Event_Error += OnError_Event;
        }

        private void Clear() {
            _error = null;
            _err_line = 0;
            _procedure = null;
            _result = null;
        }

        public ScriptError Error {
            get { return _error; }
        }

        public WScript WScript {
            get { return _wscript; }
        }

        public object Result {
            get { return _result; }
        }

        private void OnError_Event() {
            if (_error == null)
                _error = new ScriptError(_script, _procedure, _compiler.Error.Description);
            var line = _compiler.Error.Line;
            if (_script != null && line != 0 && line != _err_line)
                _error.StackTrace.Add(_script.GetTraceLineAt(line));
            _err_line = line;
        }

        public bool Compile(WScript wscript, Script script = null) {
            _wscript = wscript;
            if (_script != null)
                _compiler.Reset();
            _script = script;
            _compiler.AddObject("WScript", wscript, true);
            _compiler.AddCode("Function IIf(Expression, TruePart, FalsePart)\r\nIf Expression Then IIf = truepart Else IIf = falsepart\r\nEnd Function\r\n");
            if(script != null)
                return AddCode(script.GetCode());
            return true;
        }

        public bool AddCode(string code, bool noevent = false) {
            Clear();
            try {
                var content = code;
                _compiler.AddCode(content);
                return true;
            } catch {
                if (_error == null) throw;
                _error.FillParentTrace();
                if (OnError != null && !noevent)
                    OnError(_error);
                return false;
            }
        }

        public bool Run(ProcedureItem procedure, bool noevent = false) {
            Clear();
            if (procedure == null) return true;
            _procedure = procedure;
            try {
                _result = _compiler.Run(procedure.Name, procedure.Params);
                return true;
            } catch {
                if (_error == null) throw;
                if (OnError != null && !noevent)
                    OnError(_error);
            }
            return false;
        }

        public bool Eval(string expression) {
            Clear();
            try {
                _result = _compiler.Eval(expression);
                return true;
            } catch {
                if (_error == null) throw;
                return false;
            }
        }

        public Procedures Procedures {
            get { return _compiler.Procedures; }
        }

    }

}
