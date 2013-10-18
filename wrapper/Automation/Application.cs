using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SeleniumWrapper.Automation {

    [Guid("866E162D-4522-4AE3-9D13-0A685FB8ACB0")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IApplication {

        [Description("Start a new application with the provided path")]
        Window start(String exepath, [Optional][DefaultParameterValue("")]string arguments);

        [Description("Get the active foreground window")]
        Window getActive();

        [Description("Get the desktop window")]
        Window getDesktop();

        [Description("Get a top window by title")]
        Window fromTitle(string title, [Optional][DefaultParameterValue(0)]int timeoutms);

        [Description("Get a window using it's handle code")]
        Window fromWindowHandle(IntPtr handle);

        [Description("Get the window from the program name")]
        Window fromName(string program);
    }

    [Guid("1D9E7B36-3522-47C8-A26E-7E9A1FD5D450")]
    [ComVisible(true), ComDefaultInterface(typeof(Automation.IApplication)), ClassInterface(ClassInterfaceType.None)]
    public class Application : IApplication {

        /// <summary>Start a new application with the provided path</summary>
        /// <param name="exepath">File path to run</param>
        /// <param name="arguments">Optional - Execution arguments</param>
        /// <returns>Window object (Main application window)</returns>
        public static Window Start(String exepath, string arguments = "") {
            var file = new System.IO.FileInfo(exepath);
            if (!file.Exists) throw new ApplicationException("File not found : " + exepath);
            var process = System.Diagnostics.Process.Start(file.FullName, arguments);
            System.Threading.Thread.Sleep(500);
            IntPtr hwnd;
            try {
                hwnd = NatHelper.WaitNot(() => process.MainWindowHandle, IntPtr.Zero, 30000);
                process.WaitForInputIdle(30000);
            } catch (TimeoutException ex) {
                throw new Exception("Failed to start the process", ex);
            } finally {
                process.Dispose();
            }
            NatHelper.Wait(() => Win32.GetForegroundWindow(), hwnd);
            return new Window(hwnd);
        }

        /// <summary>Start a new application with the provided path</summary>
        /// <param name="exepath">File path to run</param>
        /// <param name="arguments">Optional - Execution arguments</param>
        /// <returns>Window object (Main application window)</returns>
        public Window start(String exepath, [Optional][DefaultParameterValue("")]string arguments) {
            return Application.Start(exepath, arguments);
        }

        /// <summary>Get the foreground window</summary>
        /// <returns>Window object</returns>
        public Window getActive() {
            return new Window(Win32.GetForegroundWindow());
        }

        /// <summary>Get the desktop window</summary>
        /// <returns>Window object</returns>
        public Window getDesktop() {
            return new Window(Win32.GetDesktopWindow());
        }

        /// <summary>Get a top window by title</summary>
        /// <param name="title">Windown title. Wildcard character can be used</param>
        /// <param name="timeoutms">Timeout in millisecond</param>
        /// <returns>Window object</returns>
        /// <example>
        /// GetFromTitle("* - Notepad")
        /// </example>
        public Window fromTitle(string title, [Optional][DefaultParameterValue(1000)]int timeoutms) {
            try {
                return new Window(NatHelper.WaitNot(() => NatHelper.GetWindowByTitle(title), IntPtr.Zero, timeoutms));
            } catch (TimeoutException ex) {
                throw new Exception("Failed to fin the window named '" + title + "'", ex);
            }
        }

        /// <summary>Get a window using it's handle code</summary>
        /// <param name="handle">Handle</param>
        /// <returns>Window object</returns>
        public Window fromWindowHandle(IntPtr handle) {
            return new Window(handle);
        }

        /// <summary>Get the window from the program name</summary>
        /// <param name="program">Program name</param>
        /// <returns>Window object</returns>
        /// <example>
        /// FromProgram("notepad").Activate();
        /// </example>
        public Window fromName(string program) {
            var processes = System.Diagnostics.Process.GetProcessesByName(program.Replace(".exe", ""));
            if (processes == null || processes.Length == 0)
                throw new ApplicationException("Failed to find the process named \"" + program + "\"");
            return new Window(processes[0].MainWindowHandle);
        }

    }
}
