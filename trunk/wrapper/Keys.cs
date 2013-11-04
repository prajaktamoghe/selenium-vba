using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace SeleniumWrapper
{
    [Guid("3349C16B-9DB7-4A54-BAA4-637431245D48")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IKeys
    {
        string Add { get; }
        string Alt { get; }
        string ArrowDown { get; }
        string ArrowLeft { get; }
        string ArrowRight { get; }
        string ArrowUp { get; }
        string Backspace { get; }
        string Cancel { get; }
        string Clear { get; }
        string Command { get; }
        string Control { get; }
        string Decimal { get; }
        string Delete { get; }
        string Divide { get; }
        string Down { get; }
        string End { get; }
        string Enter { get; }
        string Equal { get; }
        string Escape { get; }
        string F1 { get; }
        string F10 { get; }
        string F11 { get; }
        string F12 { get; }
        string F2 { get; }
        string F3 { get; }
        string F4 { get; }
        string F5 { get; }
        string F6 { get; }
        string F7 { get; }
        string F8 { get; }
        string F9 { get; }
        string Help { get; }
        string Home { get; }
        string Insert { get; }
        string Left { get; }
        string LeftAlt { get; }
        string LeftControl { get; }
        string LeftShift { get; }
        string Meta { get; }
        string Multiply { get; }
        string Null { get; }
        string NumberPad0 { get; }
        string NumberPad1 { get; }
        string NumberPad2 { get; }
        string NumberPad3 { get; }
        string NumberPad4 { get; }
        string NumberPad5 { get; }
        string NumberPad6 { get; }
        string NumberPad7 { get; }
        string NumberPad8 { get; }
        string NumberPad9 { get; }
        string PageDown { get; }
        string PageUp { get; }
        string Pause { get; }
        string Return { get; }
        string Right { get; }
        string Semicolon { get; }
        string Separator { get; }
        string Shift { get; }
        string Space { get; }
        string Subtract { get; }
        string Tab { get; }
        string Up { get; }
    }

    /// <summary>
    /// Representations of pressable keys that are not text keys for sending to the browser.
    /// </summary>

    [Description("Representations of pressable keys that are not text keys for sending to the browser.")]
    [Guid("02BAE541-F3BD-4E95-8349-BE75D14E7B41")]
    [ComVisible(true), ComDefaultInterface(typeof(IKeys)), ClassInterface(ClassInterfaceType.None)]
    public class Keys : IKeys
    {

        private string conv(int code){
            return Convert.ToString(Convert.ToChar(code, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Represents the NUL keystroke.
        /// </summary>
        public string Null { get{ return conv(0xE000);} }

        /// <summary>
        /// Represents the Cancel keystroke.
        /// </summary>
        public string Cancel { get{ return conv(0xE001);} }

        /// <summary>
        /// Represents the Help keystroke.
        /// </summary>
        public string Help { get{ return conv(0xE002);} }

        /// <summary>
        /// Represents the Backspace key.
        /// </summary>
        public string Backspace { get{ return conv(0xE003);} }

        /// <summary>
        /// Represents the Tab key.
        /// </summary>
        public string Tab { get{ return conv(0xE004);} }

        /// <summary>
        /// Represents the Clear keystroke.
        /// </summary>
        public string Clear { get{ return conv(0xE005);} }

        /// <summary>
        /// Represents the Return key.
        /// </summary>
        public string Return { get{ return conv(0xE006);} }

        /// <summary>
        /// Represents the Enter key.
        /// </summary>
        public string Enter { get{ return conv(0xE007);} }

        /// <summary>
        /// Represents the Shift key.
        /// </summary>
        public string Shift { get{ return conv(0xE008);} }

        /// <summary>
        /// Represents the Shift key.
        /// </summary>
        public string LeftShift { get{ return conv(0xE008);} } // alias

        /// <summary>
        /// Represents the Control key.
        /// </summary>
        public string Control { get{ return conv(0xE009);} }

        /// <summary>
        /// Represents the Control key.
        /// </summary>
        public string LeftControl { get{ return conv(0xE009);} } // alias

        /// <summary>
        /// Represents the Alt key.
        /// </summary>
        public string Alt { get{ return conv(0xE00A);} }

        /// <summary>
        /// Represents the Alt key.
        /// </summary>
        public string LeftAlt { get{ return conv(0xE00A);} } // alias

        /// <summary>
        /// Represents the Pause key.
        /// </summary>
        public string Pause { get{ return conv(0xE00B);} }

        /// <summary>
        /// Represents the Escape key.
        /// </summary>
        public string Escape { get{ return conv(0xE00C);} }

        /// <summary>
        /// Represents the Spacebar key.
        /// </summary>
        public string Space { get{ return conv(0xE00D);} }

        /// <summary>
        /// Represents the Page Up key.
        /// </summary>
        public string PageUp { get{ return conv(0xE00E);} }

        /// <summary>
        /// Represents the Page Down key.
        /// </summary>
        public string PageDown { get{ return conv(0xE00F);} }

        /// <summary>
        /// Represents the End key.
        /// </summary>
        public string End { get{ return conv(0xE010);} }

        /// <summary>
        /// Represents the Home key.
        /// </summary>
        public string Home { get{ return conv(0xE011);} }

        /// <summary>
        /// Represents the left arrow key.
        /// </summary>
        public string Left { get{ return conv(0xE012);} }

        /// <summary>
        /// Represents the left arrow key.
        /// </summary>
        public string ArrowLeft { get{ return conv(0xE012);} } // alias

        /// <summary>
        /// Represents the up arrow key.
        /// </summary>
        public string Up { get{ return conv(0xE013);} }

        /// <summary>
        /// Represents the up arrow key.
        /// </summary>
        public string ArrowUp { get{ return conv(0xE013);} } // alias

        /// <summary>
        /// Represents the right arrow key.
        /// </summary>
        public string Right { get{ return conv(0xE014);} }

        /// <summary>
        /// Represents the right arrow key.
        /// </summary>
        public string ArrowRight { get{ return conv(0xE014);} } // alias

        /// <summary>
        /// Represents the Left arrow key.
        /// </summary>
        public string Down { get{ return conv(0xE015);} }

        /// <summary>
        /// Represents the Left arrow key.
        /// </summary>
        public string ArrowDown { get{ return conv(0xE015);} } // alias

        /// <summary>
        /// Represents the Insert key.
        /// </summary>
        public string Insert { get{ return conv(0xE016);} }

        /// <summary>
        /// Represents the Delete key.
        /// </summary>
        public string Delete { get{ return conv(0xE017);} }

        /// <summary>
        /// Represents the semi-colon key.
        /// </summary>
        public string Semicolon { get{ return conv(0xE018);} }

        /// <summary>
        /// Represents the equal sign key.
        /// </summary>
        public string Equal { get{ return conv(0xE019);} }

        // Number pad keys

        /// <summary>
        /// Represents the number pad 0 key.
        /// </summary>
        public string NumberPad0 { get{ return conv(0xE01A);} }

        /// <summary>
        /// Represents the number pad 1 key.
        /// </summary>
        public string NumberPad1 { get{ return conv(0xE01B);} }

        /// <summary>
        /// Represents the number pad 2 key.
        /// </summary>
        public string NumberPad2 { get{ return conv(0xE01C);} }

        /// <summary>
        /// Represents the number pad 3 key.
        /// </summary>
        public string NumberPad3 { get{ return conv(0xE01D);} }

        /// <summary>
        /// Represents the number pad 4 key.
        /// </summary>
        public string NumberPad4 { get{ return conv(0xE01E);} }

        /// <summary>
        /// Represents the number pad 5 key.
        /// </summary>
        public string NumberPad5 { get{ return conv(0xE01F);} }

        /// <summary>
        /// Represents the number pad 6 key.
        /// </summary>
        public string NumberPad6 { get{ return conv(0xE020);} }

        /// <summary>
        /// Represents the number pad 7 key.
        /// </summary>
        public string NumberPad7 { get{ return conv(0xE021);} }

        /// <summary>
        /// Represents the number pad 8 key.
        /// </summary>
        public string NumberPad8 { get{ return conv(0xE022);} }

        /// <summary>
        /// Represents the number pad 9 key.
        /// </summary>
        public string NumberPad9 { get{ return conv(0xE023);} }

        /// <summary>
        /// Represents the number pad multiplication key.
        /// </summary>
        public string Multiply { get{ return conv(0xE024);} }

        /// <summary>
        /// Represents the number pad addition key.
        /// </summary>
        public string Add { get{ return conv(0xE025);} }

        /// <summary>
        /// Represents the number pad thousands separator key.
        /// </summary>
        public string Separator { get{ return conv(0xE026);} }

        /// <summary>
        /// Represents the number pad subtraction key.
        /// </summary>
        public string Subtract { get{ return conv(0xE027);} }

        /// <summary>
        /// Represents the number pad decimal separator key.
        /// </summary>
        public string Decimal { get{ return conv(0xE028);} }

        /// <summary>
        /// Represents the number pad division key.
        /// </summary>
        public string Divide { get{ return conv(0xE029);} }

        // Function keys

        /// <summary>
        /// Represents the function key F1.
        /// </summary>
        public string F1 { get{ return conv(0xE031);} }

        /// <summary>
        /// Represents the function key F2.
        /// </summary>
        public string F2 { get{ return conv(0xE032);} }

        /// <summary>
        /// Represents the function key F3.
        /// </summary>
        public string F3 { get{ return conv(0xE033);} }

        /// <summary>
        /// Represents the function key F4.
        /// </summary>
        public string F4 { get{ return conv(0xE034);} }

        /// <summary>
        /// Represents the function key F5.
        /// </summary>
        public string F5 { get{ return conv(0xE035);} }

        /// <summary>
        /// Represents the function key F6.
        /// </summary>
        public string F6 { get{ return conv(0xE036);} }

        /// <summary>
        /// Represents the function key F7.
        /// </summary>
        public string F7 { get{ return conv(0xE037);} }

        /// <summary>
        /// Represents the function key F8.
        /// </summary>
        public string F8 { get{ return conv(0xE038);} }

        /// <summary>
        /// Represents the function key F9.
        /// </summary>
        public string F9 { get{ return conv(0xE039);} }

        /// <summary>
        /// Represents the function key F10.
        /// </summary>
        public string F10 { get{ return conv(0xE03A);} }

        /// <summary>
        /// Represents the function key F11.
        /// </summary>
        public string F11 { get{ return conv(0xE03B);} }

        /// <summary>
        /// Represents the function key F12.
        /// </summary>
        public string F12 { get{ return conv(0xE03C);} }

        /// <summary>
        /// Represents the function key META.
        /// </summary>
        public string Meta { get{ return conv(0xE03D);} }

        /// <summary>
        /// Represents the function key COMMAND.
        /// </summary>
        public string Command { get{ return conv(0xE03D);} }
    }
}
