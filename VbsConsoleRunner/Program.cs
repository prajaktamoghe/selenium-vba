using System;
using System.IO;

namespace vbsc {

    class Program {

        [MTAThread]
        static void Main(string[] args) {

            Environment.ExitCode = 1;

            if (args.Length == 0) {
                Debugger.Debug();
                return;
            }

            var options = new Options(args);
            options.ParseOption("help", @"^[-/]*(help|\?)$", false, "help :  Provide help");
            options.ParseOption("debug", @"^debug$", false, "debug :  Breaks on errors");
            options.ParseOption("noexit", @"^noexit$", false, "noexit :  The console remains open at the end");
            options.ParseOption("noinfo", @"^noinfo$", false, "noinfo : Do not display the text from WScript.Echo");
            options.ParseOption("args", @"^args=.*|a=.*$", new string[0], "args,t=value1,value2,... :", "Lists of arguments to send to the script (Comma to separate values).");
            options.ParseOption("out", @"^out=.*|o=.*$", (string)null, "out,o=filepath : ", "Log file. To add the current datetime : {yyyyMMdd-HHmmss}");
            options.ParseOption("filter", @"^filter=.*|f=.*$", ".*", "filter,f=value : ", "Pattern to filter procedures");
            options.ParseOption("params", @"^params=.*|p=.*$", new string[0], "params,p=value1,value2,... : ", "Lists of params to run each script with (Tag = \"@param\").");
            options.ParseOption("threads", @"^threads=.*|t=.*$", 1, "threads,t=n : ", "Number of script to execute in parallel.");

            options.AddExample(@"sscript noexit args=firefox,chrome ""c:\scripts\*.vbs\"" ""c:\scripts\*.vbs\""");
            options.AddExample(@"sscript o=""c:\scripts\result-{time}.log"" ""c:\scripts\*.vbs""");
            options.AddExample(@"sscript t=4 ""c:\scripts\*.vbs""");

            if (options.Files.Length > 0) {
                var dir = Path.GetDirectoryName(options.Files[0]);
                if(!string.IsNullOrEmpty(dir))
                    Directory.SetCurrentDirectory(dir);
            }

            if ((bool)options["help"]) {
                //Display help
                Console.WriteLine(options.ToString());
                return;
            }

            //Run scripts
            if (RunScripts(options, args))
                Environment.ExitCode = 0;

            //Wait for a key pressed if noexit is present
            if ((bool)options["noexit"])
                Console.ReadKey();
        }

        static bool RunScripts(Options options, string[] args) {

            try {
                StdInOut.HideInfo = (bool)options["noinfo"];
                var starttime = DateTime.Now;
                StdInOut.LogStart(starttime);

                //Check log path
                var logpath = (string)options["out"];
                if (logpath != null) {
                    logpath = logpath.Replace("{time}", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
                    if (logpath.IndexOfAny(Path.GetInvalidPathChars()) != -1) {
                        StdInOut.LogError("Invalide log file path.", "Argument: " + (string)options["out"]);
                        return false;
                    }
                }

                string[] list_scripts_path;
                try {
                    list_scripts_path = Utils.ExpandFilePaths(options.Files, "vbs");
                } catch (FileNotFoundException ex) {
                    StdInOut.LogError(ex.Message, "Argument: " + ex.FileName);
                    return false;
                }

                var runner = new MultiScriptRunner((int)options["threads"]);
                var results = runner.Run(list_scripts_path, (string[])options["args"], (string[])options["params"], (string)options["filter"], (bool)options["debug"]);

                //Print final result
                StdInOut.LogResults(results, starttime, DateTime.Now);

                //Save log file                
                if (!string.IsNullOrEmpty(logpath))
                    StdInOut.SaveTo(logpath);
                return results.Exists((r) => !r.Succeed) == false;

            } catch (Exception ex) {
                StdInOut.LogException(ex, args);
            }
            return false;

        }

    }

}
