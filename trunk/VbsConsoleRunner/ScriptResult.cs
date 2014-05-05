using System.Text;
using System;

namespace vbsc {

    abstract class ScriptResult {

        public Script Script;
        public ProcedureItem Procedure;

        public ScriptResult(Script script, ProcedureItem procedure) {
            Script = script;
            Procedure = procedure;
        }

        public abstract bool Succeed { get;}

        public abstract string SourceName { get;}

    }

}
