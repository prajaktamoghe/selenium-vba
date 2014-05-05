using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MSScriptControl;
using System.IO;

namespace vbsc {

    class ScriptRunner {

        public List<ScriptResult> Results { get; private set; }

        private readonly Compiler _compiler;
        private bool _debug;

        public ScriptRunner(bool debug) {
            _compiler = new Compiler();
            _debug = debug;
            _compiler.OnError += OnError;
            this.Results = new List<ScriptResult>();
        }

        private void OnError(ScriptError error) {
            this.Results.Add(error);
            error.Script.Succeed = false;
            StdInOut.LogScriptError(error.SourceName, error.ToString());
            if (_debug)
                Debugger.Debug(_compiler);
        }

        private void OnSuccess(ScriptSuccees succees) {
            this.Results.Add(succees);
        }

        private void OnInfo(Script script, string message) {
            if(message.Length != 0)
                StdInOut.LogScriptInfo(script.SourceName, message);
        }

        public void Execute(Script script, Regex procedure_pattern) {
            var textwriter = new StringWriter();
            var wscript = new WScript(script.Path, script.Arguments);
            wscript.OnEcho += textwriter.WriteLine;
            if (!_compiler.Compile(wscript, script))
                return;
            var list_procedures = ListProcedures(script, procedure_pattern);
            if (_compiler.Run(list_procedures.ProcInitialize)) {
                foreach (var procedure in list_procedures) {
                    if (_compiler.Run(list_procedures.ProcSetup)) {
                        if (_compiler.Run(procedure, true)) {
                            OnSuccess(new ScriptSuccees(script, procedure));
                        } else {
                            var error = _compiler.Error;
                            if (list_procedures.ProcOnError != null) {
                                list_procedures.ProcOnError.Params = new object[] { procedure.Name, _compiler.Error.ToString() };
                                if (_compiler.Run(list_procedures.ProcOnError))
                                    error.AddInformation((string)_compiler.Result);
                            }
                            OnError(error);
                        }
                        _compiler.Run(list_procedures.ProcTearDown);
                    }
                }
                _compiler.Run(list_procedures.ProcTerminate);
            }
            if (list_procedures.Count == 0 && script.Succeed)
                OnSuccess(new ScriptSuccees(script));
            OnInfo(script, textwriter.ToString().CleanEnd());
        }


        private ProcedureList ListProcedures(Script script, Regex pattern) {
            var procedures = new ProcedureList();
            foreach (IScriptProcedure proc in _compiler.Procedures) {
                var proc_name = proc.Name;
                switch (proc_name.ToLower()) {
                    case "initialize": procedures.ProcInitialize = new ProcedureItem(proc_name); break;
                    case "terminate": procedures.ProcTerminate = new ProcedureItem(proc_name); break;
                    case "setup": procedures.ProcSetup = new ProcedureItem(proc_name); break;
                    case "teardown": procedures.ProcTearDown = new ProcedureItem(proc_name); break;
                    case "onerror": procedures.ProcOnError = new ProcedureItem(proc_name); break;
                    case "iif": break;
                    default:
                        if (proc.HasReturnValue) continue;
                        if (!pattern.IsMatch(proc_name)) continue;
                        ProcedureStringParams proc_params_str;
                        if (script.ProceduresParams.TryGetValue(proc_name, out proc_params_str)) {
                            if (!_compiler.Eval("Array(" + proc_params_str.Params + ')')) {
                                var error = new ScriptError(script, new ProcedureItem(proc_name), "Invalide array: " + proc_params_str.Params);
                                error.AddTraceLine(script, proc_params_str.Line);
                                OnError(error);
                                continue;
                            }
                            var proc_params = (object[])_compiler.Result;
                            foreach (var p in proc_params) {
                                var proc_args = p is object[] ? (object[])p : new object[1]{ p };
                                if (proc_args.Length == proc.NumArgs) {
                                    procedures.Add(proc_name, proc_args);
                                } else {
                                    var error = new ScriptError(script, new ProcedureItem(proc_name), string.Format("Procedure {0} requires {1} argument(s). {2} provied.", proc_name, proc.NumArgs, proc_args.Length));
                                    error.AddTraceLine(script, script.TextFormated.GetLineNumber("(Sub|Function).\b" + proc_name + "\b"));
                                    OnError(error);
                                    break;
                                }
                            }
                        } else if (proc.NumArgs == 0) {
                            procedures.Add(proc_name, new object[0]);
                        }
                        break;
                }
            }
            return procedures;
        }

    }
}
