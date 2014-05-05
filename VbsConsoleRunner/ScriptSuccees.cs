using System.Text;

namespace vbsc {

    class ScriptSuccees : ScriptResult {

        public ScriptSuccees(Script script, ProcedureItem procedure = null)
            : base(script, procedure) {
        }
        
        public override bool Succeed {
            get { return true; }
        }

        public override string SourceName {
            get {
                var title = new StringBuilder();
                if (this.Procedure != null)
                    title.Append(this.Procedure.SourceName + " in ");
                title.Append(this.Script.SourceName);
                return title.ToString();
            }
        }
    }

}
