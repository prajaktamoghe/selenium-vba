using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace SeleniumWrapper.Automation
{
    //Doc : http://msdn.microsoft.com/en-us/library/system.windows.forms.sendkeys.aspx
    //      http://msdn.microsoft.com/en-us/library/windows/desktop/ms646267(v=vs.85).aspx
    //      http://www.autoitscript.com/autoit3/docs/appendix/SendKeys.htm

    [Guid("8FD1A805-C1FD-4FA6-9136-2BD5BD8F57D6")]
    [ComVisible(true)]
    public interface IKeyboard
    {
        [Description("Send keys to the foreground window")]
        void SendKeys(String keysOrModifiers, [Optional][DefaultParameterValue("")]String keys);

        [Description("Send characters to the foreground window")]
        void SendChars(String characters);

        [Description("Delay in millisecond after SendKeys or SendChars is executed")]
        int Delay { get; set; }

        [Description("Delay in millisecond between each character")]
        int DelayBetweenCharacter { get; set; }

        [Description("Allow the use of short modifiers (^=LCTRL, #=LWIN, +=SHIFT, %=LMENU) and grouping with (...) . Place the character in {.} to escape the command if this option is activated")]
        bool UseShortModifiers { get; set; }
	}

    /// <summary>Defines the interface through which the user can send keys and characters</summary>
    /// <example>
    /// <code lang="vb
    /// This example opens a webpage and save it">
    /// Public Sub TC002()
    ///   Dim keyBoard As New SeleniumWrapper.KeyBoard
    ///   selenium.Start "firefox", "http://www.mywebsite.com/"   'Launch Firefox
    ///   selenium.Open "/"
    ///   keyBoard.SendKeys "^s"  'Send Ctrl+s which is the shortcut to save the page
    ///   driver.Wait 1000    'Waits 1sec for the popup to show
    ///   keyBoard.SendKeys "pagename.htm{ENTER}"    'Send a name for the page and press ENTER
    /// End Sub
    /// </code>
    /// </example>

    [Guid("DB5860AF-05A9-45E1-908A-4D78A784007E")]
    [ComVisible(true), ComDefaultInterface(typeof(IKeyboard)), ClassInterface(ClassInterfaceType.None)]
    public class Keyboard : IKeyboard
    {
        #region VkCodesModifiers
        private static readonly Dictionary<char, ushort> VkCodesModifiers = new Dictionary<char, ushort>(){
            { '%', '\xA4' },  //LMENU
            { '^', '\xA2' },  //LCONTROL
            { '+', '\xA0' },  //LSHIFT
            { '#', '\x5B' }   //LWIN
        };
        #endregion

        #region VkCodesNumPad
        private static readonly Dictionary<char, ushort> VkCodesNumPad = new Dictionary<char, ushort>(){
            { '0', '\x60' },  //
            { '1', '\x61' },  //
            { '2', '\x62' },  //
            { '3', '\x63' },  //
            { '4', '\x64' },  //
            { '5', '\x65' },  //
            { '6', '\x66' },  //
            { '7', '\x67' },  //
            { '8', '\x68' },  //
            { '9', '\x69' },  //
            { '*', '\x6A' },  // Multiply key
            { '+', '\x6B' },  // Add key
            { '-', '\x6D' },  // Subtract key
            { '/', '\x6F' },  // Divide key
            { '.', '\x6E' }  // Decimal key
        };
        #endregion

        #region VkCodesCommands
        private static readonly Dictionary<string, ushort> VkCodesCommands = new Dictionary<string, ushort>(){
            { "WAIT", '\x00' },     // Wait millisecond
            { "LBUTTON", '\x01' },  // Left mouse button 
            { "RBUTTON", '\x02' },  // Right mouse button 
            { "CANCEL", '\x03' },  // Control-break processing 
            { "MBUTTON", '\x04' },  // Middle mouse button (three-button mouse) 
            { "XBUTTON1", '\x05' },  // X1 mouse button 
            { "XBUTTON2", '\x06' },  // X2 mouse button -", '\x07' },  // Undefined 
            { "BACK", '\x08' },  // BACKSPACE key
            { "BACKSPACE", '\x08' },  // BACKSPACE key
            { "BS", '\x08' },  // BACKSPACE key
            { "TAB", '\x09' },  // TAB key -", '\x0A-0B' },  // Reserved 
            { "CLEAR", '\x0C' },  // CLEAR key 
            { "RETURN", '\x0D' },  // ENTER key -", '\x0E-0F' },  // Undefined 
            { "ENTER", '\x0D' },  // ENTER key -", '\x0E-0F' },  // Undefined 
            { "SHIFT", '\x10' },  // SHIFT key 
            { "CTRL", '\x11' },  // CTRL key
            { "CONTROL", '\x11' },  // CTRL key
            { "MENU", '\x12' },  // ALT key 
            { "PAUSE", '\x13' },  // PAUSE key 
            { "BREAK", '\x13' },  // PAUSE key 
            { "CAPITAL", '\x14' },  // CAPS LOCK key 
            { "CAPSLOCK", '\x14' },  // CAPS LOCK key 
            { "KANA", '\x15' },  // IME Kana mode 
            { "HANGUEL", '\x15' },  // IME Hanguel mode (maintained for compatibility; use VK_HANGUL) 
            { "HANGUL", '\x15' },  // IME Hangul mode -", '\x16' },  // Undefined 
            { "JUNJA", '\x17' },  // IME Junja mode 
            { "FINAL", '\x18' },  // IME final mode 
            { "HANJA", '\x19' },  // IME Hanja mode 
            { "KANJI", '\x19' },  // IME Kanji mode -", '\x1A' },  // Undefined 
            { "ESCAPE", '\x1B' },  // ESC key 
            { "ESC", '\x1B' },  // ESC key 
            { "CONVERT", '\x1C' },  // IME convert 
            { "NONCONVERT", '\x1D' },  // IME nonconvert 
            { "ACCEPT", '\x1E' },  // IME accept 
            { "MODECHANGE", '\x1F' },  // IME mode change request 
            { "SPACE", '\x20' },  // SPACEBAR 
            { "PRIOR", '\x21' },  // PAGE UP key 
            { "PGUP", '\x21' },  // PAGE UP key 
            { "NEXT", '\x22' },  // PAGE DOWN key 
            { "PGDN", '\x22' },  // PAGE DOWN key 
            { "END", '\x23' },  // END key 
            { "HOME", '\x24' },  // HOME key 
            { "LEFT", '\x25' },  // LEFT ARROW key 
            { "UP", '\x26' },  // UP ARROW key 
            { "RIGHT", '\x27' },  // RIGHT ARROW key 
            { "DOWN", '\x28' },  // DOWN ARROW key 
            { "SELECT", '\x29' },  // SELECT key 
            { "PRINT", '\x2A' },  // PRINT key 
            { "EXECUTE", '\x2B' },  // EXECUTE key 
            { "SNAPSHOT", '\x2C' },  // PRINT SCREEN key
            { "INSERT", '\x2D' },  // INS key
            { "INS", '\x2D' },  // INS key
            { "DELETE", '\x2E' },  // DEL key 
            { "DEL", '\x2E' },  // DEL key 
            { "HELP", '\x2F' },  // HELP key", '\x30' },  // 0 key", '\x31' },  // 1 key", '\x32' },  // 2 key", '\x33' },  // 3 key", '\x34' },  // 4 key", '\x35' },  // 5 key", '\x36' },  // 6 key", '\x37' },  // 7 key", '\x38' },  // 8 key", '\x39' },  // 9 key -", '\x3A-40' },  // Undefined", '\x41' },  // A key", '\x42' },  // B key", '\x43' },  // C key", '\x44' },  // D key", '\x45' },  // E key", '\x46' },  // F key", '\x47' },  // G key", '\x48' },  // H key", '\x49' },  // I key", '\x4A' },  // J key", '\x4B' },  // K key", '\x4C' },  // L key", '\x4D' },  // M key", '\x4E' },  // N key", '\x4F' },  // O key", '\x50' },  // P key", '\x51' },  // Q key", '\x52' },  // R key", '\x53' },  // S key", '\x54' },  // T key", '\x55' },  // U key", '\x56' },  // V key", '\x57' },  // W key", '\x58' },  // X key", '\x59' },  // Y key", '\x5A' },  // Z key 
            { "WIN", '\x5B' },  // Left Windows key (Natural keyboard) 
            { "LWIN", '\x5B' },  // Left Windows key (Natural keyboard) 
            { "RWIN", '\x5C' },  // Right Windows key (Natural keyboard) 
            { "APPS", '\x5D' },  // Applications key (Natural keyboard) -", '\x5E' },  // Reserved 
            { "SLEEP", '\x5F' },  // Computer Sleep key 
            { "NUMPAD0", '\x60' },  // Numeric keypad 0 key 
            { "NUMPAD1", '\x61' },  // Numeric keypad 1 key 
            { "NUMPAD2", '\x62' },  // Numeric keypad 2 key 
            { "NUMPAD3", '\x63' },  // Numeric keypad 3 key 
            { "NUMPAD4", '\x64' },  // Numeric keypad 4 key 
            { "NUMPAD5", '\x65' },  // Numeric keypad 5 key 
            { "NUMPAD6", '\x66' },  // Numeric keypad 6 key 
            { "NUMPAD7", '\x67' },  // Numeric keypad 7 key 
            { "NUMPAD8", '\x68' },  // Numeric keypad 8 key 
            { "NUMPAD9", '\x69' },  // Numeric keypad 9 key 
            { "MULTIPLY", '\x6A' },  // Multiply key 
            { "ADD", '\x6B' },  // Add key 
            { "SEPARATOR", '\x6C' },  // Separator key 
            { "SUBTRACT", '\x6D' },  // Subtract key 
            { "DECIMAL", '\x6E' },  // Decimal key 
            { "DIVIDE", '\x6F' },  // Divide key 
            { "F1", '\x70' },  // F1 key 
            { "F2", '\x71' },  // F2 key 
            { "F3", '\x72' },  // F3 key 
            { "F4", '\x73' },  // F4 key 
            { "F5", '\x74' },  // F5 key 
            { "F6", '\x75' },  // F6 key 
            { "F7", '\x76' },  // F7 key 
            { "F8", '\x77' },  // F8 key 
            { "F9", '\x78' },  // F9 key 
            { "F10", '\x79' },  // F10 key 
            { "F11", '\x7A' },  // F11 key 
            { "F12", '\x7B' },  // F12 key 
            { "F13", '\x7C' },  // F13 key 
            { "F14", '\x7D' },  // F14 key 
            { "F15", '\x7E' },  // F15 key 
            { "F16", '\x7F' },  // F16 key 
            { "F17", '\x80' },  // F17 key 
            { "F18", '\x81' },  // F18 key 
            { "F19", '\x82' },  // F19 key 
            { "F20", '\x83' },  // F20 key 
            { "F21", '\x84' },  // F21 key 
            { "F22", '\x85' },  // F22 key 
            { "F23", '\x86' },  // F23 key 
            { "F24", '\x87' },  // F24 key -", '\x88-8F' },  // Unassigned 
            { "NUMLOCK", '\x90' },  // NUM LOCK key 
            { "SCROLL", '\x91' },  // SCROLL LOCK key", '\x92-96' },  // OEM specific -", '\x97-9F' },  // Unassigned 
            { "SCROLLLOCK", '\x91' },  // SCROLL LOCK key", '\x92-96' },  // OEM specific -", '\x97-9F' },  // Unassigned 
            { "LSHIFT", '\xA0' },  // Left SHIFT key 
            { "RSHIFT", '\xA1' },  // Right SHIFT key 
            { "LCONTROL", '\xA2' },  // Left CONTROL key 
            { "LCTRL", '\xA2' },  // Left CONTROL key 
            { "RCONTROL", '\xA3' },  // Right CONTROL key
            { "RCTRL", '\xA3' },  // Right CONTROL key
            { "LMENU", '\xA4' },  // Left MENU key 
            { "RMENU", '\xA5' },  // Right MENU key 
            { "ALT", '\xA4' },  // Left MENU key 
            { "ALTGR", '\xA5' },  // Right MENU key 
            { "BROWSER_BACK", '\xA6' },  // Browser Back key 
            { "BROWSER_FORWARD", '\xA7' },  // Browser Forward key 
            { "BROWSER_REFRESH", '\xA8' },  // Browser Refresh key 
            { "BROWSER_STOP", '\xA9' },  // Browser Stop key 
            { "BROWSER_SEARCH", '\xAA' },  // Browser Search key 
            { "BROWSER_FAVORITES", '\xAB' },  // Browser Favorites key 
            { "BROWSER_HOME", '\xAC' },  // Browser Start and Home key 
            { "VOLUME_MUTE", '\xAD' },  // Volume Mute key 
            { "VOLUME_DOWN", '\xAE' },  // Volume Down key 
            { "VOLUME_UP", '\xAF' },  // Volume Up key 
            { "MEDIA_NEXT_TRACK", '\xB0' },  // Next Track key 
            { "MEDIA_PREV_TRACK", '\xB1' },  // Previous Track key 
            { "MEDIA_STOP", '\xB2' },  // Stop Media key 
            { "MEDIA_PLAY_PAUSE", '\xB3' },  // Play/Pause Media key 
            { "LAUNCH_MAIL", '\xB4' },  // Start Mail key 
            { "LAUNCH_MEDIA_SELECT", '\xB5' },  // Select Media key 
            { "LAUNCH_APP1", '\xB6' },  // Start Application 1 key 
            { "LAUNCH_APP2", '\xB7' },  // Start Application 2 key -", '\xB8-B9' },  // Reserved 
            { "OEM_1", '\xBA' },  // Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ';:' key 
            { "OEM_PLUS", '\xBB' },  // For any country/region, the '+' key 
            { "OEM_COMMA", '\xBC' },  // For any country/region, the ',' key 
            { "OEM_MINUS", '\xBD' },  // For any country/region, the '-' key 
            { "OEM_PERIOD", '\xBE' },  // For any country/region, the '.' key 
            { "OEM_2", '\xBF' },  // Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '/?' key 
            { "OEM_3", '\xC0' },  // Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '`~' key -", '\xC1-D7' },  // Reserved -", '\xD8-DA' },  // Unassigned 
            { "OEM_4", '\xDB' },  // Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '[{' key 
            { "OEM_5", '\xDC' },  // Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '\|' key 
            { "OEM_6", '\xDD' },  // Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ']}' key 
            { "OEM_7", '\xDE' },  // Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the 'single-quote/double-quote' key 
            { "OEM_8", '\xDF' },  // Used for miscellaneous characters; it can vary by keyboard. -", '\xE0' },  // Reserved", '\xE1' },  // OEM specific 
            { "OEM_102", '\xE2' },  // Either the angle bracket key or the backslash key on the RT 102-key keyboard", '\xE3-E4' },  // OEM specific 
            { "PROCESSKEY", '\xE5' },  // IME PROCESS key", '\xE6' },  // OEM specific 
            { "PACKET", '\xE7' },  // Used to pass Unicode characters as if they were keystrokes. The 
            { "PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP -", '\xE8' },  // Unassigned", '\xE9-F5' },  // OEM specific 
            { "ATTN", '\xF6' },  // Attn key 
            { "CRSEL", '\xF7' },  // CrSel key 
            { "EXSEL", '\xF8' },  // ExSel key 
            { "EREOF", '\xF9' },  // Erase EOF key 
            { "PLAY", '\xFA' },  // Play key 
            { "ZOOM", '\xFB' },  // Zoom key 
            { "NONAME", '\xFC' },  // Reserved 
            { "PA1", '\xFD' },  // PA1 key 
            { "OEM_CLEAR", '\xFE' }  // Clear key Requirements
        };
        #endregion

        private static Dictionary<char, ushort> VkCodesRaw = new Dictionary<char, ushort>();

        public int Delay { get; set; }
        public int DelayBetweenCharacter { get; set; }
        public bool UseShortModifiers { get; set; }

        /// <summary>Keybork classs</summary>
        public Keyboard() : this(80, 6, true)
        {
        }

        /// <summary>Keybork classs</summary>
        /// <param name="delayMs">Delay to wait after a methode has been executed</param>
        /// <param name="delayBetweenCharsMs">Delay in millisecond to wait before sending the next character</param>
        /// <param name="UseShortModifiers">Allow the use of short modifiers (^=LCTRL, #=LWIN, +=SHIFT, %=LMENU) and grouping with (...) . Place the character in {.} to escape the command if this option is activated</param>
        public Keyboard(int delayMs, int delayBetweenCharsMs, bool UseShortModifiers)
        {
            //Initialize delays
            this.Delay = delayMs;
            this.DelayBetweenCharacter = delayBetweenCharsMs;
            this.UseShortModifiers = true;

            //Read all the characters from the keyboard
            for (ushort vkCode = 0; vkCode < 0XFF; vkCode++){
                try{
                    var car = (char)Win32.MapVirtualKey((uint)vkCode, Win32.MAPVK.MAPVK_VK_TO_CHAR);
                    if(car == '\0') continue;
                    if(car>='A' && car<='Z')
                        VkCodesRaw.Add( (char)(car + 32) , vkCode); //Convert to lower case
                    else
                        VkCodesRaw.Add(car, vkCode);
                }catch { }
            }

            //Initilize CAPSLOCK, NUMLOCK and SCROLL
            foreach(ushort vkCode in new ushort[]{ 0x14, 0x90, 0x91 }){
                if ((Win32.GetKeyState(vkCode) & 0x0F) != 0)
                    SendKey(vkCode, Action.DownUP);
            }
        }
        
        /// <summary>Send multiple characters without commands</summary>
        /// <param name="characters">Characters to send</param>
        public void SendChars(String characters)
        {
            try {
                Win32.BlockInput(true);
                foreach (char car in ConvertLiteralCharacters(characters))
                    SendChar(car);
            } finally {
                Win32.BlockInput(false);
            }
            WaitForIdle();
        }

        private enum Hold{
            None,       //No holding
            SingleKey,  //Holds down modifiers keys for one character
            GroupeKeys  //Holds down modifiers keys for a group of characters
        }

        /// <summary>Send multiple keys with commands</summary>
        public void SendKeys(String keysOrmodifiers, [Optional][DefaultParameterValue("")]String keys)
        {
            var downKeys = new List<ushort>();
            try {
                Win32.BlockInput(true);
                if(string.IsNullOrEmpty(keys))
                    keys = keysOrmodifiers;
                else{
                    //Send modifiers if any
                    foreach(char car in keysOrmodifiers){
                        ushort vkCode = (ushort)(car & 0xFF);
                        SendKey(vkCode, Action.Down);
                        downKeys.Add(vkCode);
                    }
                }
                //replace literal characters (\r, \t, \n ...)
                keys = ConvertLiteralCharacters(keys);
                var hold = Hold.None;
                //Loop through each character
                for (int pos = 0; pos < keys.Length; pos++) {
                    char car = keys[pos];
                    ushort vkCode = 0;
                    //Handle commands
                    if(car == '{'){
                        var end = keys.IndexOf('}', pos + 2);
                        if (end == -1) throw new ArgumentException("Closing bracket missing !");
                        string command = keys.Substring(pos + 1, end - pos - 1);
                        pos = end;
                        var indexspace = command.IndexOf(' ');
                        var cmdname = (indexspace == -1) ? command : command.Substring(0, indexspace);
                        if (cmdname.Length == 1)
                            car = cmdname[0];
                        else if (VkCodesCommands.ContainsKey(cmdname))
                            vkCode = VkCodesCommands[cmdname];
                        else
                            throw new Exception("Command {" + command + "} is not available");
                        int repeat = 1;
                        if (indexspace != -1){
                            String cmdext = command.Substring(indexspace + 1).Trim();
                            if (cmdext == "UP"){
                                SendKey(vkCode, Action.Up);
                                downKeys.Remove(vkCode);
                                continue;
                            }else if (cmdext == "DOWN" || cmdext == "DN"){
                                SendKey(vkCode, Action.Down);
                                downKeys.Add(vkCode);
                                continue;
                            }else if(!int.TryParse(cmdext, out repeat))
                                throw new ArgumentException("Command extention {... " + cmdext + "} is invalide");
                        }
                        if(cmdname == "WAIT"){
                            System.Threading.Thread.Sleep(repeat);
                        }else if(vkCode != 0)
                            SendKey(vkCode, Action.DownUP, repeat);
                        else
                            SendChar(car, repeat);
                        if (hold == Hold.SingleKey){
                            ReleaseKeys(downKeys);
                            hold = Hold.None;
                        }
                    //Handle key modifiers
                    }else if(this.UseShortModifiers && VkCodesModifiers.ContainsKey(car) && hold != Hold.GroupeKeys){
                        vkCode = VkCodesModifiers[car];
                        SendKey(vkCode, Action.Down);
                        downKeys.Add(vkCode);
                        hold = Hold.SingleKey;
                    }else if(this.UseShortModifiers && car == '(' && hold != Hold.GroupeKeys){
                        hold = Hold.GroupeKeys;
                    }else if(this.UseShortModifiers && car == ')' && hold == Hold.GroupeKeys){
                        ReleaseKeys(downKeys);
                        hold = Hold.None;
                    //Handle other characters
                    }else{
                        SendChar(car);
                        if (hold == Hold.SingleKey){
                            ReleaseKeys(downKeys);
                            hold = Hold.None;
                        }
                    }
                }
            }finally{
                ReleaseKeys(downKeys);
                Win32.BlockInput(false);
            }
            WaitForIdle();
        }

        /// <summary>Wait for input idle</summary>
        private void WaitForIdle(){
            //Wait for input IDLE
            System.Threading.Thread.Sleep(this.Delay);
            int pid;
            Win32.GetWindowThreadProcessId(Win32.GetForegroundWindow(), out pid);
            IntPtr process = IntPtr.Zero;
            try{
                process = Win32.OpenProcess(Win32.PROCESS.PROCESS_VM_READ, false, pid);
                Win32.WaitForInputIdle(process, 20000);
            }finally{
                if(process != IntPtr.Zero) 
                    Win32.CloseHandle(process);
            }
        }

        /// <summary>Convert literal character such as line feed or tabulation</summary>
        /// <param name="text">Text to convert</param>
        /// <returns>Converted text</returns>
        private string ConvertLiteralCharacters(string text){
            return Regex.Replace(
                text.Replace("\\r\\n", "\r")
                    .Replace("\\n", "\r")
                    .Replace("\\t", "\t")
                    .Replace("\\r", "\r")
                    .Replace("\\f", "\f")
                    .Replace("\\v", "\v")
                    .Replace("\\0", "\0")
                    .Replace("\\b", "\b"),
                @"\\u([0-9A-Fa-f]{2,4})", m => ((char)int.Parse(m.Groups[1].Value, System.Globalization.NumberStyles.HexNumber)).ToString()
            );
        }

        /// <summary>Release all the down keys</summary>
        /// <param name="downKeys">List of virtual key codes of the donw keys</param>
        private void ReleaseKeys(List<ushort> downKeys){
            for(int i=downKeys.Count-1; i>-1; i--)
                SendKey(downKeys[i], Action.Up);
            downKeys.Clear();
        }
        
        /// <summary>Send a character</summary>
        /// <param name="character">Character</param>
        /// <param name="nbTime">Nb times to send the key</param>
        public void SendChar(char character, int nbTime = 1){
            if(VkCodesRaw.ContainsKey(character)){
                //Send a keyboard key code
                SendKey(VkCodesRaw[character], Action.DownUP, nbTime);
            }else{
                //Send unicode
                while (nbTime-- > 0){
                    SendKeyboardInput(0, (ushort)character, Win32.KEYEVENTF.KEYEVENTF_UNICODE);
                    SendKeyboardInput(0, (ushort)character, Win32.KEYEVENTF.KEYEVENTF_UNICODE | Win32.KEYEVENTF.KEYEVENTF_KEYUP);
                }
            }
            /*
            ushort vkCode = 0;
            if (this.UseNumPad && VkCodesNumPad.ContainsKey(character)){
                SendKey(VkCodesNumPad[character], Action.DownUP, nbTime);
                return;
            }
            if(character < 0xFF)
                vkCode = (ushort)NativeMethods.VkKeyScan(character);
            if (vkCode > 0 && vkCode < 0xFF){
                SendKey((ushort)vkCode, Action.DownUP, nbTime);
            }else{
                while (nbTime-- > 0){
                    SendKeyboardInput(0, (ushort)character, NativeMethods.KEYEVENTF.KEYEVENTF_UNICODE);
                    SendKeyboardInput(0, (ushort)character, NativeMethods.KEYEVENTF.KEYEVENTF_UNICODE | NativeMethods.KEYEVENTF.KEYEVENTF_KEYUP);
                }
            }*/
        }


        public enum Action : uint {
            Down = 1,
            Up = 2,
            DownUP = 3
        }

        /// <summary>Send a virtual key code</summary>
        /// <param name="vkCode">Virtual key code</param>
        /// <param name="action">DownUp, Down, Up</param>
        /// <param name="nbTime">Nb times to send the key</param>
        public void SendKey(ushort vkCode, Action action, int nbTime = 1){
            var scanCode = (ushort)Win32.MapVirtualKey((uint)(vkCode & 0xFF), Win32.MAPVK.MAPVK_VK_TO_VSC);
            while(nbTime-- > 0){
                if( (action & Action.Down) != 0)
                    SendKeyboardInput(vkCode, scanCode, 0);
                if( (action & Action.Up) != 0)
                    SendKeyboardInput(vkCode, scanCode, Win32.KEYEVENTF.KEYEVENTF_KEYUP );
            }
        }

        /// <summary>Send the data</summary>
        /// <param name="wVk">Virtual ey</param>
        /// <param name="wScan">Scan code</param>
        /// <param name="dwFlags">KEYEVENTF flag</param>
        private void SendKeyboardInput(ushort wVk, ushort wScan, Win32.KEYEVENTF dwFlags){
            //compose data
            var input = new Win32.INPUT{
                type = Win32.INPUTT.INPUTT_Keyboard,
                ki = new Win32.KeyboardInputData{
                    wVk = wVk,
                    wScan = wScan,
                    dwFlags = dwFlags,
                    time = 0,
                    dwExtraInfo = IntPtr.Zero
                }
            };
            
            //Send the data
            Win32.SendInput(1, ref input, 0x1C);  //var size = Marshal.SizeOf(input);
            //Debug.Print("vkCode=" + wVk.ToString("x4") + " scanCode=" + wScan.ToString("x4") + " flag=" + dwFlags.ToString("x4"));

            //Wait for the message to be consume
            var endtime = DateTime.Now.AddMilliseconds(5000);
            System.Threading.Thread.Sleep(this.DelayBetweenCharacter);
            while(true){
                if(!Win32.GetInputState()) break;
                if (DateTime.Now > endtime) throw new TimeoutException("Failed to consume the char " + ((wVk != 0) ? wVk : wScan));
                System.Threading.Thread.Sleep(5);
            }
        }


    }

}
