
namespace vbsc {

    class TraceLine {

        public readonly Script Script;
        public readonly int LineNumber;
        public readonly string LineOfCode;

        public TraceLine(Script script, int line) {
            Script = script;
            LineNumber = line;
            LineOfCode = Script.TextOriginal.GetLineAt(LineNumber);
        }

        public override string ToString() {
            return "at " + Script.Path + " line" + LineNumber.ToString() + "\r\n " + LineOfCode;
        }
    }

}
