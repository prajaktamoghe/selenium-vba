using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace vbsc {

    class Options : Dictionary<string, object> {

        private readonly List<string> _arguments;
        private readonly StringBuilder _sb_examples;
        private readonly StringBuilder _sb_options;

        public Options(string[] arguments) {
            _arguments = new List<string>(arguments);
            _sb_examples = new StringBuilder();
            _sb_options = new StringBuilder();
        }

        public void ParseOption(string id, string pattern, object default_value, string help, string info = null) {
            base.Add(id, ParseOption(pattern, default_value));
            if (info != null)
                _sb_options.AppendLine("\r\n " + help + "\r\n   " + info);
            else
                _sb_options.AppendLine(" " + help);
        }

        public void AddExample(string example) {
            _sb_examples.AppendLine(" " + example);
        }

        public string[] Files {
            get { return _arguments.ToArray(); }
        }

        public object ParseOption(string pattern, object default_Value) {
            var re = new Regex(pattern);
            for (int i = 0; i < _arguments.Count; i++) {
                if (re.IsMatch(_arguments[i])) {
                    var arg = _arguments[i];
                    _arguments.RemoveAt(i);
                    if (default_Value is bool)
                        return true;
                    else {
                        var value = arg.Substring(arg.IndexOf('=') + 1).Trim('"');
                        if (default_Value is string[])
                            return value.Split(',');
                        if (default_Value is int)
                            return int.Parse(value);
                        return value;
                    }
                }
            }
            return default_Value;
        }

        public override string ToString() {
            return string.Format(@"
Usage: sscript [OPTIONS] files...

files : Single file or list of files.

OPTIONS:

{0}

Exemples:

{1}
               ", _sb_options.ToString()
                , _sb_examples.ToString()
             );
        }


    }
}
