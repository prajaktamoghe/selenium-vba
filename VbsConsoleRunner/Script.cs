using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace vbsc {

    class Script {

        public readonly string Path;
        public readonly string[] Arguments;
        public readonly string Param;
        public readonly string SourceName;
        public readonly string Directory;
        public readonly string TextOriginal;
        public readonly int LineCount;

        public bool Succeed = true;
        public string TextFormated;

        public List<Script> ChildenScripts;
        public Script ParentScript;
        public int ParentLineNumber;

        public Dictionary<string, ProcedureStringParams> ProceduresParams;


        public Script(string script_path, string[] arguments, string param = null)
            :this(script_path){
            SourceName += (param == null ? string.Empty : '(' + param + ')');
            Arguments = arguments;
            Param = param;
            FormatScript(null, 0);
        }

        private Script(string script_path, Script parent_script, int parent_line_number)
            : this(script_path) {
            FormatScript(parent_script, parent_line_number);
        }

        private Script(string script_path) {
            Path = script_path;
            Directory = System.IO.Path.GetDirectoryName(this.Path);
            SourceName = System.IO.Path.GetFileName(script_path);
            TextOriginal = (System.IO.File.ReadAllText(this.Path)).ToString();
            LineCount = TextOriginal.CountLines();
        }


        /// <summary>Recursive methode to read and format a script and the refered scripts defined in the #Include statement.</summary>
        /// <param name="par_script">Parent script used in recurtion call</param>
        /// <param name="par_line_number">Parent line number used in recurtion call</param>
        /// <param name="par_lien_of_code">Parent line of code used in recurtion call</param>
        /// <returns>A list of scripts</returns>
        private void FormatScript(Script par_script, int par_line_number) {

            this.TextFormated = this.TextOriginal;
            this.ParentScript = par_script;
            this.ParentLineNumber = par_line_number;

            //Handle includes
            Match match_inc = Regex.Match(this.TextFormated, @"^#Include ""([^""]+)""", RegexOptions.Multiline);
            while (match_inc.Success) {
                this.ChildenScripts = new List<Script>();
                var inc_path = match_inc.Groups[1].Value;
                var inc_line_number = this.TextFormated.GetLineNumber(match_inc);
                foreach (var child_script_path in Utils.ExpandFilePaths(new string[] { inc_path }, ".vbs", this.Directory)) {
                    this.ChildenScripts.Add(new Script(child_script_path, this, inc_line_number));
                }
                match_inc = match_inc.NextMatch();
            }
            this.TextFormated = this.TextFormated.RemoveAll(@"^#Include[^\r\n]*");

            //Handle Console prints
            this.TextFormated = Regex.Replace(this.TextFormated, @"Debug\.Print", @"Wscript.Echo", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (par_script == null) {
                //Format Arguments
                this.TextFormated = Regex.Replace(this.TextFormated, @"Wscript\.Arguments\(", @"Wscript.Arguments.Item(", RegexOptions.IgnoreCase);
                //Replace param
                if (this.Param != null)
                    this.TextFormated = Regex.Replace(this.TextFormated, @"\@\bparam\b", this.Param, RegexOptions.IgnoreCase);
                    //this.TextFormated = Regex.Replace(this.TextFormated, @"(\.Start.)""\w+""", "$1\"" + this.Param + "\"", RegexOptions.IgnoreCase);
                //Replace the wrapper instantiation if it's called using "New"
                this.TextFormated = Regex.Replace(this.TextFormated, @"Set ([\w_-]+) = New (SeleniumWrapper\.\w+)", @"Set $1 = CreateObject(""$2"")", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                this.TextFormated = Regex.Replace(this.TextFormated, @"Dim ([\w_-]+) As New (SeleniumWrapper\.\w+)", @"Set $1 = CreateObject(""$2"")", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                
                //Handles With
                var matches_with = Regex.Matches(this.TextFormated, (@"^\[With\((.*)\)\]\s+(Private |Public )?Sub ([\w_]+)"), RegexOptions.IgnoreCase | RegexOptions.Multiline);
                this.ProceduresParams = new Dictionary<string, ProcedureStringParams>(matches_with.Count);
                foreach (Match match in matches_with) {
                    ProceduresParams.Add( match.Groups[3].Value, new ProcedureStringParams{ 
                        Params = match.Groups[1].Value.Trim('\r', '\n', ' '),
                        Line = this.TextFormated.CountLines(0, match.Index)
                    });
                }
                this.TextFormated = this.TextFormated.ReplaceAll(@"^\[With\((.*)\)\]", "");
            }
        }


        public string GetCode() {
            return GetTextCode_recursive(null);
        }

        private string GetTextCode_recursive(Script script) {
            var content = new StringBuilder();
            if (script == null) script = this;
            if (script.ChildenScripts != null) {
                foreach(var child_script in script.ChildenScripts)
                    content.AppendLine(GetTextCode_recursive(child_script));
            }
            content.AppendLine(script.TextFormated);
            return content.ToString();
        }



        public TraceLine GetTraceLineAt(int line_number) {
            return GetTraceLineAt_recursive(line_number, this, new Integer(line_number));
        }

        private TraceLine GetTraceLineAt_recursive(int line_number, Script script, Integer line) {
            if (script.ChildenScripts != null) {
                foreach (var child_script in script.ChildenScripts) {
                    var trace_line = GetTraceLineAt_recursive(line_number, child_script, line);
                    if (trace_line != null)
                        return trace_line;
                }
            }
            if (line.Value - script.LineCount <= 0)
                return new TraceLine(script, line.Value);
            line.Value -= script.LineCount + 1;
            return null;
        }

    }

}
