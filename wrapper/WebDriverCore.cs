using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Remote;

namespace SeleniumWrapper {

    public abstract class WebDriverCore : MarshalByRefObject, IDisposable {

        public delegate Object ActionResult();
        public delegate void ActionVoid();

        protected static WebDriverCore _webDriverCoreStatic;

        protected OpenQA.Selenium.IWebDriver _webDriver;
        protected Selenium.WebDriverBackedSelenium _webDriverBacked;
        internal int _timeout;
        internal bool _canceled;

        protected String _baseUrl;
        protected Dictionary<string, object> _capabilities;
        protected Dictionary<string, object> _preferences;
        protected List<string> _extensions;
        protected List<string> _arguments;
        protected string _profile;
        protected Proxy _proxy { get; set; }
        protected bool _isStartedRemotely;
        protected System.Timers.Timer _timerhotkey;

        String _error;
        Thread _thread;
        System.Action delegate_function;

        public WebDriverCore() {
            _timeout = 30000;
            _timerhotkey = new System.Timers.Timer(200);
            _timerhotkey.Elapsed += new System.Timers.ElapsedEventHandler(TimerCheckHotKey);
            _capabilities = new Dictionary<string, object>();
            if (this.delegate_function != null) this.delegate_function();
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppDomain_UnhandledException);
            //AppDomain.CurrentDomain.ProcessExit += (sender, e) => Utils.runShellCommand(@"FOR /D %A IN (%TEMP%\anonymous*) DO RD /S /Q ""%A"" & FOR /D %A IN (%TEMP%\scoped_dir*) DO RD /S /Q ""%A"" & DEL /q /f %TEMP%\IE*.tmp");
        }

        ~WebDriverCore() {
            Dispose(true);
        }

        public void Dispose() {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing) {
            _timerhotkey.Stop();
            if (_thread != null) _thread.Abort();
        }

        public bool CopyStaticDriver(string baseUrl = null) {
            if (_webDriverCoreStatic == null)
                return false;
            _webDriver = _webDriverCoreStatic._webDriver;
            _webDriverBacked = _webDriverCoreStatic._webDriverBacked;
            _timeout = _webDriverCoreStatic._timeout;
            _canceled = false;
            _baseUrl = string.IsNullOrEmpty(baseUrl) ? _webDriverCoreStatic._baseUrl : baseUrl;
            _capabilities = _webDriverCoreStatic._capabilities;
            _preferences = _webDriverCoreStatic._preferences;
            _extensions = _webDriverCoreStatic._extensions;
            _arguments = _webDriverCoreStatic._arguments;
            _profile = _webDriverCoreStatic._profile;
            _proxy = _webDriverCoreStatic._proxy;
            _isStartedRemotely = _webDriverCoreStatic._isStartedRemotely;
            return true;
        }

        public OpenQA.Selenium.IWebDriver WebDriver {
            get {
                if (_webDriver == null && !CopyStaticDriver())
                    throw new ApplicationException("Browser not started. Use the command start or startRemotely.");
                return _webDriver; 
            }
            set {
                _webDriverCoreStatic = this;
                _webDriver = value;
            }
        }

        public Selenium.WebDriverBackedSelenium WebDriverBacked {
            get {
                if (_webDriverBacked == null) {
                    if (_webDriver == null && !CopyStaticDriver(_baseUrl))
                        throw new ApplicationException("Browser not started. Use the command start or startRemotely.");
                    if (_webDriverBacked == null) {
                        _webDriverBacked = new Selenium.WebDriverBackedSelenium(_webDriver, _baseUrl);
                        _webDriverBacked.Start();
                    }
                }
                return _webDriverBacked; 
            }
        }

        private void AppDomain_UnhandledException(Object sender, UnhandledExceptionEventArgs e) {
            Exception exception = (Exception)e.ExceptionObject;
            if (!(exception is ThreadAbortException)) {
                _error = exception.GetType().Name + ": " + exception.Message;
                _thread.Abort();
            }
        }

        private void TimerCheckHotKey(object source, ElapsedEventArgs e) {
            if (Utils.isEscapeKeyPressed()) {
                _canceled = true;
                _thread.Abort();
            }
        }

        public void RegisterFunction([MarshalAs(UnmanagedType.FunctionPtr)] System.Action doevents_function) {
            this.delegate_function = doevents_function;
        }

        private string GetErrorPrefix(string methodeName) {
            string lMethodname = Regex.Match(methodeName, "<([^>]+)>").Groups[0].Value;
            return "Method " + lMethodname + " failed !";
        }


        protected void InvokeWd(ActionVoid action) {
            _error = null;
            _thread = new System.Threading.Thread((System.Threading.ThreadStart)delegate {
                try {
                    action();
                } catch (System.Exception ex) {
                    if (!(ex is ThreadAbortException)) _error = ex.GetType().ToString() + ": " + ex.Message;
                }
            });
            _thread.Start();
            bool succeed = _thread.Join(_timeout + 1000);
            this.CheckCanceled();
            if (!succeed) throw new ApplicationException(GetErrorPrefix(action.Method.Name) + "\nTimed out running command after " + _timeout + " milliseconds");
            if (_error != null) throw new ApplicationException(GetErrorPrefix(action.Method.Name) + "\n" + _error);
        }

        protected Object InvokeWd(ActionResult action) {
            object result = null;
            _error = null;
            _thread = new System.Threading.Thread((System.Threading.ThreadStart)delegate {
                try {
                    result = action();
                } catch (System.Exception ex) {
                    if (!(ex is ThreadAbortException)) _error = ex.Message;
                }
            });
            _thread.Start();
            bool succeed = _thread.Join(_timeout + 1000);
            this.CheckCanceled();
            if (!succeed) throw new ApplicationException(GetErrorPrefix(action.Method.Name) + "\nTimed out running command after " + _timeout + " milliseconds");
            if (_error != null) throw new ApplicationException(GetErrorPrefix(action.Method.Name) + "\n" + _error);
            //if (EndOfCommand != null) EndOfCommand();
            return result;
        }

        protected void InvokeWdWaitFor(ActionResult action, Object expected, bool match) {
            object result = null;
            _error = null;
            _thread = new System.Threading.Thread((System.Threading.ThreadStart)delegate {
                while (!_canceled && (match ^ Utils.ObjectEquals(result, expected))) {
                    try {
                        result = action();
                    } catch (Exception ex) {
                        if (ex is ThreadAbortException) break;
                        _error = ex.Message;
                    }
                    Thread.Sleep(100);
                }
            });
            _thread.Start();
            bool succed = _thread.Join(_timeout + 1000);
            this.CheckCanceled();
            if (!succed || _error != null) {
                var sb = new StringBuilder();
                sb.Append(GetErrorPrefix(action.Method.Name));
                if (expected != null)
                    sb.Append(" Expected" + (match ? "=" : "!=") + "<" + expected.ToString() + "> result=<" + (result ?? "null").ToString() + ">.");
                if (!succed)
                    sb.Append(" Timed out after " + _timeout + " ms.");
                if (_error != null)
                    sb.Append(_error);
                throw new ApplicationException(sb.ToString());
            }
        }

        protected void InvokeWdAssert(ActionResult action, Object expected, bool match) {
            Object result = InvokeWd(action);
            if (match ^ Utils.ObjectEquals(result, expected)) throw new ApplicationException(GetErrorPrefix(action.Method.Name) + "\nexpected" + (match ? "=" : "!=") + "<" + expected.ToString() + ">\nresult=<" + result.ToString() + "> ");
        }

        protected String InvokeWdVerify(ActionResult action, Object expected, bool match) {
            Object result = InvokeWd(action);
            if (match ^ Utils.ObjectEquals(result, expected)) {
                return "KO, " + GetErrorPrefix(action.Method.Name) + " expected" + (match ? "=" : "!=") + "<" + expected.ToString() + "> result=<" + result.ToString() + "> ";
            } else {
                return "OK";
            }
        }

        protected void InvokeWdAndWait(ActionVoid action) {
            InvokeWd(action);
            WebDriverBacked.WaitForPageToLoad(_timeout.ToString());
        }

        /// <summary>Repeatedly applies this instance's input value to the given function until one of the following</summary>
        /// <param name="function"></param>
        /// <param name="timeoutms"></param>
        /// <returns></returns>
        internal object WaitUntilObject(ActionResult function, int timeoutms) {
            var endTime = DateTime.Now.AddMilliseconds(timeoutms);
            string errorMsg = String.Empty;
            while (true) {
                try {
                    errorMsg = String.Empty;
                    var result = function();
                    if (result != null) return result;
                } catch (Exception ex) {
                    errorMsg = ex.Message;
                }
                this.CheckCanceled();
                if (DateTime.Now > endTime)
                    throw new TimeoutException("The operation has timed out! " + errorMsg);
                Thread.Sleep(30);
            }
        }

        internal void CheckCanceled() {
            if (_canceled) {
                _canceled = false;
                throw new ApplicationException("Code execution has been interrupted");
            }
        }

        // http://code.google.com/p/selenium/wiki/DesiredCapabilities#Proxy_JSON_Object

        protected FirefoxProfile getFirefoxOptions() {
            FirefoxProfile firefoxProfile;
            if (_profile != null) {
                if (System.IO.Directory.Exists(_profile)) {
                    firefoxProfile = new FirefoxProfile(_profile);
                } else {
                    firefoxProfile = new FirefoxProfileManager().GetProfile(_profile);
                    if (firefoxProfile == null)
                        Process.Start("firefox.exe", "-CreateProfile " + _profile).WaitForExit();
                    firefoxProfile = new FirefoxProfileManager().GetProfile(_profile);
                }
            } else {
                firefoxProfile = new FirefoxProfile();
            }
            if (_preferences != null) {
                foreach (KeyValuePair<string, object> pref in _preferences) {
                    if (pref.Value is string)
                        firefoxProfile.SetPreference(pref.Key, (string)pref.Value);
                    else if (pref.Value is short)
                        firefoxProfile.SetPreference(pref.Key, Convert.ToInt32(pref.Value));
                    else if (pref.Value is bool)
                        firefoxProfile.SetPreference(pref.Key, (bool)pref.Value);
                }
            }
            if (_extensions != null) {
                foreach (string ext in _extensions)
                    firefoxProfile.AddExtension(ext);
            }
            if (_proxy != null)
                firefoxProfile.SetProxyPreferences(_proxy);
            firefoxProfile.EnableNativeEvents = false;
            firefoxProfile.AcceptUntrustedCertificates = true;
            firefoxProfile.Port = 9055;
            return firefoxProfile;
        }

        protected ChromeOptions getChromeOptions() {
            ChromeOptions chromeOptions = new ChromeOptions();
            if (_arguments != null)
                chromeOptions.AddArguments(_arguments);
            if (_profile != null)
                chromeOptions.AddArgument("user-data-dir=" + _profile);
            if (_preferences != null) {
                foreach (KeyValuePair<string, object> pair in _preferences)
                    chromeOptions.AddUserProfilePreference(pair.Key, pair.Value);
            }
            if (_extensions != null)
                chromeOptions.AddExtensions(_extensions);
            if (_proxy != null)
                chromeOptions.AddArgument("--proxy-server=" + _proxy.HttpProxy);
            foreach (KeyValuePair<string, object> capability in _capabilities) {
                switch (capability.Key) {
                    case "chrome.binary":
                        chromeOptions.BinaryLocation = (string)capability.Value;
                        break;
                    default:
                        chromeOptions.AddAdditionalCapability(capability.Key, capability.Value);
                        break;
                }
            }
            return chromeOptions;
        }

        protected InternetExplorerOptions getInternetExplorerOptions() {
            InternetExplorerOptions ieoptions = new InternetExplorerOptions();
            if (_preferences != null) throw new Exception("Preference configuration is not available for InternetExplorerDriver!");
            if (_extensions != null) throw new Exception("Preference configuration is not available for InternetExplorerDriver!");
            if (_proxy != null) throw new Exception("Proxy configuration is not available for InternetExplorerDriver!");
            foreach (KeyValuePair<string, object> capability in _capabilities) {
                switch (capability.Key) {
                    case "ignoreProtectedModeSettings": ieoptions.IntroduceInstabilityByIgnoringProtectedModeSettings = (bool)capability.Value; break;
                    case "ignoreZoomSetting": ieoptions.IgnoreZoomLevel = (bool)capability.Value; break;
                    case "initialBrowserUrl": ieoptions.InitialBrowserUrl = (string)capability.Value; break;
                    case "elementScrollBehavior": ieoptions.ElementScrollBehavior = (InternetExplorerElementScrollBehavior)capability.Value; break;
                    case "unexpectedAlertBehaviour": ieoptions.UnexpectedAlertBehavior = (InternetExplorerUnexpectedAlertBehavior)capability.Value; break;
                    case "nativeEvents": ieoptions.EnableNativeEvents = (bool)capability.Value; break;
                    default: ieoptions.AddAdditionalCapability(capability.Key, capability.Value); break;
                }
            }
            return ieoptions;
        }

        protected PhantomJSOptions getPhantomJSOptions() {
            var phantomjsOptions = new PhantomJSOptions();
            if (_profile != null) throw new Exception("Profile configuration is not available for PhantomJS driver!");
            if (_preferences != null) throw new Exception("Preference configuration is not available for PhantomJS!");
            if (_extensions != null) throw new Exception("Extension configuration is not available for PhantomJS!");
            if (_proxy != null) throw new Exception("Proxy configuration is not available for InternetExplorerDriver!");
            foreach (var capability in _capabilities)
                phantomjsOptions.AddAdditionalCapability(capability.Key, capability.Value);
            return phantomjsOptions;
        }

        protected SafariOptions getSafariOptions() {
            var safariOptions = new SafariOptions();
            //if (!String.IsNullOrEmpty(directory))
            //    safariOptions.SafariLocation = directory;
            if (_profile != null) throw new Exception("Profile configuration is not available for Safari driver!");
            if (_preferences != null) throw new Exception("Preference configuration is not available for Safari!");
            if (_extensions != null && _extensions.Count != 0)
                safariOptions.CustomExtensionPath = _extensions[0];
            if (_proxy != null) throw new Exception("Proxy configuration is not available for Safari!");
            foreach (var capability in _capabilities)
                safariOptions.AddAdditionalCapability(capability.Key, capability.Value);
            return safariOptions;
        }
    }

}
