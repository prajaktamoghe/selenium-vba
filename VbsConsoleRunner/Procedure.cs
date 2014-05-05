using System;
using System.Collections.Generic;
using System.Text;

namespace vbsc {

    struct ProcedureStringParams {
        public string Params;
        public int Line;
    }

    class ProcedureItem {

        public string Name;
        public object[] Params;

        public ProcedureItem(string name, object[] parameters = null) {
            Name = name;
            Params = parameters ?? new object[0];
        }

        public string SourceName{
            get {
                if (Params.Length == 0) return Name;
                if(Name.ToLower() == "onerror")
                    return Name + "(\"" + Params[0] + "\", )";
                return Name + '(' + Params.Join(',', 20) + ')';
            }
        }
    }

    class ProcedureList : List<ProcedureItem> {

        public ProcedureItem ProcInitialize = null;
        public ProcedureItem ProcTerminate = null;
        public ProcedureItem ProcSetup = null;
        public ProcedureItem ProcTearDown = null;
        public ProcedureItem ProcOnError = null;

        public void Add(string procedure, object[] parameters) {
            base.Add(new ProcedureItem(procedure, parameters));
        }

        public new ProcedureItem this[int index] {
            get { return base[index]; }
        }

    }
}
