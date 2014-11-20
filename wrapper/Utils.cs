using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

namespace SeleniumWrapper {
    [Guid("7CC07D9C-3BBF-450C-B7E6-01D514FE0B1A")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IUtils {
        [Description("Indicates whether the regular expression finds a match in the input string using the regular expression specified in the pattern parameter.")]
        bool isMatch(string input, string pattern);

        [Description("Searches the specified input string for an occurrence of the regular expression supplied in the pattern parameter.")]
        object match(string input, string pattern);

        [Description("Searches the specified input string for an occurrence of the regular expression supplied in the pattern parameter and replace it.")]
        string replace(string input, string pattern, string replacement);

        [Description("Get a screenshot")]
        Image getScreenShot();
    }

    /// <summary>
    /// 
    /// </summary>

    [Description("")]
    [Guid("C9A3B3ED-EE5F-43BD-A47B-A34FCBA29598")]
    [ComVisible(true), ComDefaultInterface(typeof(IUtils)), ClassInterface(ClassInterfaceType.None)]
    public class Utils : IUtils {

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern short GetKeyState(int virtualKeyCode);

        [DllImport("User32.dll")]
        private static extern int SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern int GetForegroundWindow();

        internal static void maximizeForegroundWindow() {
            ShowWindow(GetForegroundWindow(), 3 /*SW_SHOWMAXIMIZED*/);
        }

        internal static bool isEscapeKeyPressed() {
            return (GetKeyState(0x1b) & 0x8000) != 0;
        }

        internal static bool runShellCommand(string command) {
            System.Diagnostics.Process p = new System.Diagnostics.Process {
                StartInfo = new System.Diagnostics.ProcessStartInfo {
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = (@"CMD /C " + command)
                }
            };
            p.Start();
            p.WaitForExit(5000);
            if (p.HasExited == false) { //Check to see if the process is still running.
                if (p.Responding) {//Test to see if the process is hung up.
                    p.CloseMainWindow();//Process was responding; close the main window.
                    return false;
                } else {
                    p.Kill(); //Process was not responding; force the process to close.
                    return false;
                }
            }
            return true;
        }

        public Image getScreenShot() {
            var bounds = System.Windows.Forms.Screen.GetBounds(System.Drawing.Point.Empty);
            var bitmap = new System.Drawing.Bitmap(bounds.Width, bounds.Height);
            using (var g = System.Drawing.Graphics.FromImage(bitmap))
                g.CopyFromScreen(System.Drawing.Point.Empty, System.Drawing.Point.Empty, bounds.Size);
            return new Image(bitmap);
        }

        /// <summary>Indicates whether the regular expression finds a match in the input string using the regular expression specified in the pattern parameter.</summary>
        /// <param name="input">The string to search for a match.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>true if the regular expression finds a match; otherwise, false.</returns>
        public bool isMatch(string input, string pattern) {
            return Regex.IsMatch(input, pattern);
        }

        /// <summary>Searches the specified input string for an occurrence of the regular expression supplied in the pattern parameter.</summary>
        /// <param name="input">The string to search for a match.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>Matching strings</returns>
        public object match(string input, string pattern) {
            Match match = Regex.Match(input, pattern);
            if (match.Groups != null) {
                string[] lst = new string[match.Groups.Count];
                for (int i = 0; i < match.Groups.Count; i++)
                    lst[i] = match.Groups[i].Value;
                return lst;
            } else {
                return match.Value;
            }
        }

        /// <summary>Within a specified input string, replaces all strings that match a specified regular expression with a specified replacement string.</summary>
        /// <param name="input">The string to search for a match.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="replacement">The replacement string.</param>
        /// <returns>A new string that is identical to the input string, except that a replacement string takes the place of each matched string.</returns>
        public string replace(string input, string pattern, string replacement) {
            return Regex.Replace(input, pattern, replacement);
        }

        public static bool ObjectEquals(Object a, Object b) {
            if(a is IEnumerable && b is IEnumerable){
                var enum_b = (b as IEnumerable).GetEnumerator();
                foreach (var va in a as IEnumerable) {
                    if (!enum_b.MoveNext() || !object.Equals(va, enum_b.Current))
                        return false;
                }
                if (enum_b.MoveNext())
                    return false;
                return true;
            }
            return object.Equals(a, b);
        }

        internal static string ToStrings(Object value) {
            if(value == null)
                return null;
            if (value is IEnumerable) {
                var sb = new StringBuilder();
                sb.Append("[");
                foreach (var item in value as IEnumerable) {
                    if (sb.Length != 1)
                        sb.Append(",");
                    sb.Append(item);
                }
                sb.Append("]");
                return sb.ToString();
            }
            return value.ToString();
        }

        /// <summary>Get a substring of the first N characters.</summary>
        public static string Truncate(string source) {
            return Utils.Truncate(source, 100);
        }

        /// <summary>Get a substring of the first N characters.</summary>
        public static string Truncate(string source, int length) {
            if (source.Length > length)
                source = source.Substring(0, length) + "...";
            return source;
        }
    }
}
