using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace vbsc {

    static class Utils {

        public static string Join(this object[] array, char separator, int maxparamlen) {
            var sb = new StringBuilder();
            foreach (var ele in array) {
                if (sb.Length != 0)
                    sb.Append(separator);
                if(ele is string)
                    sb.Append('"' + (ele as string).Truncate(maxparamlen) + '"');
                else
                    sb.Append((string)ele.ToString().Truncate(maxparamlen));
            }
            return sb.ToString();
        }

        public static string Truncate(this string text, int maxlen) {
            if (text.Length > maxlen)
                return text.Substring(0, maxlen - 3) + "...";
            return text;
        }

        public static T GetLast<T>(this List<T> list) {
            if (list == null || list.Count == 0) return (T)(object)null;
            return list[list.Count - 1];
        }

        public static string CleanEnd(this string text) {
            return text.TrimEnd(' ', '\t', '\r', '\n');
        }

        public static string GetLineAt(this string text, int line) {
            StringBuilder linetext = new StringBuilder();
            int count = 1;
            foreach (char car in text) {
                if (car == '\n')
                    count++;
                if (count == line)
                    linetext.Append(car);
                else if (count >= line)
                    break;
            }
            return linetext.ToString().Trim(' ', '\t', '\r', '\n');
        }

        public static int CountLines(this string text, int start_index = 0, int end_index = -1) {
            if (end_index == -1)
                end_index += text.Length;
            int count = 1;
            for (int i = start_index; i <= end_index; i++) {
                if (text[i] == '\n')
                    count++;
            }
            return count;
        }

        public static int GetLineNumber(this string text, Match match) {
            return Regex.Matches(text.Substring(0, match.Index), "\n").Count + 1;
        }

        public static int GetLineNumber(this string text, string pattern, int group = 0, RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase) {
            var matches = Regex.Match(text, pattern, options);
            if (!matches.Success) return 0;
            return Regex.Matches(text.Substring(0, matches.Index), "\n").Count + 1;
        }

        public static string[] FindAll(this string text, string pattern, int group = 0, RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase) {
            var matches = Regex.Matches(text, pattern, options);
            var result = new List<string>(matches.Count);
            foreach (Match match in matches)
                result.Add(match.Groups[group].Value);
            return result.ToArray();
        }

        public static string ReplaceAll(this string text, string pattern, string replacement, RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase) {
            return Regex.Replace(text, pattern, replacement, options);
        }

        public static string RemoveAll(this string text, string pattern, RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase) {
            return Regex.Replace(text, pattern, "", options);
        }

        public static string[] ExpandFilePaths(IEnumerable<string> files, string extention, string workingdir = null) {
            var fileList = new List<string>();
            if (!extention.StartsWith("."))
                extention = '.' + extention;

            string working_dir_bak = null;
            if (workingdir != null) {
                working_dir_bak = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(workingdir);
            }

            foreach (var arg in files) {

                var expended_path = System.Environment.ExpandEnvironmentVariables(arg.Trim('"'));
                var directory = Path.GetDirectoryName(expended_path);
                if (directory.Length == 0)
                    directory = ".";
                var filename = Path.GetFileName(expended_path);
                bool hasone = false;
                foreach (var filepath in Directory.GetFiles(directory, filename)) {
                    if (filepath.EndsWith(extention)) {
                        fileList.Add(Path.GetFullPath(filepath));
                        hasone = true;
                    }
                }

                if (working_dir_bak != null)
                    Directory.SetCurrentDirectory(working_dir_bak);

                if (!hasone)
                    throw new FileNotFoundException("File not found", arg);
            }
            return fileList.ToArray();
        }
    }

}
