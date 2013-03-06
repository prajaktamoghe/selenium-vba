
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Collections;
using System.Text;
using Selenium;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Selenium
{
	/// <summary>
	/// Sends commands and retrieves results via HTTP.
	/// </summary>
	[ComVisible(false)]
	public class HttpCommandProcessor
	{
		[ComVisible(false)]
		public delegate void EventInfoHandler(Evt vEventLevel, Evt evtype, string message);
        public event EventInfoHandler SendEventInfo;
		
		//public SeleniumCommand ex;
		public RemoteCommand remoteCommand;
		//public Err EventLevel;
		//public Evt EventLevel;
		private string url;
		private string sessionId;
		private string browserStartCommand;
		private string browserURL;
        private string extensionJs;
        private static RegistryKey RegKey;
		private Err vErrorLevel;
		private Evt vEventLevel;

		/// <summary>
		/// The server URL, to whom we send command requests
		/// </summary>
		public string Url
		{
			get { return url; }
		}
		public Err ExceptionLevel
		{
			set { this.vErrorLevel=value ; }
		}
		public Evt EventLevel
		{
			set { this.vEventLevel=value ; }
			get { return this.vEventLevel; }
		}
		
		/// <summary>
		/// Specifies a server host/port, a command to launch the browser, and a starting URL for the browser.
		/// </summary>
		/// <param name="serverHost">the host name on which the Selenium Server resides</param>
		/// <param name="serverPort">the port on which the Selenium Server is listening</param>
		/// <param name="browserStartCommand">the command string used to launch the browser, e.g. "*firefox" or "c:\\program files\\internet explorer\\iexplore.exe"</param>
		/// <param name="browserURL">the starting URL including just a domain name.  We'll start the browser pointing at the Selenium resources on this URL,
		/// e.g. "http://www.google.com" would send the browser to "http://www.google.com/selenium-server/RemoteRunner.html"</param>
		
		public HttpCommandProcessor() 
		{
            RegKey = Registry.CurrentUser.OpenSubKey(@"Software\Selenium", true);
            if (RegKey == null){ RegKey = Registry.CurrentUser.OpenSubKey(@"Software", true).CreateSubKey("Selenium");}
			this.url = (string)RegKey.GetValue("url");
			this.sessionId = (string)RegKey.GetValue("sessionId");
			this.browserStartCommand = (string)RegKey.GetValue("browserStartCommand");
			this.browserURL = (string)RegKey.GetValue("browserURL");
			this.extensionJs = (string)RegKey.GetValue("extensionJs");
		}
		
		/// <summary>
		/// Send the specified remote command to the browser to be performed
		/// </summary>
		/// <param name="command">the remote command verb</param>
		/// <param name="args">the arguments to the remote command (depends on the verb)</param>
		/// <returns>the command result, defined by the remote JavaScript.  "getX" style
		///		commands may return data from the browser</returns>
		public string DoCommand(string command, string[] args)
		{
			string resultBody = SendCommand(command, args);
			if (!resultBody.StartsWith("OK"))
			{
				//this.remoteCommand.setInfo(resultBody);
				this.SendEventInfo(this.vEventLevel,Evt.e1errorcmd, resultBody );	
				this.remoteCommand.ReportError( this.vErrorLevel, Err.e2command, resultBody );
				//throw new Exception(response.StatusDescription);
				//throw new SeleniumCommand(resultBody);
			}
			return resultBody;
		}

		private string SendCommand(string command, string[] args)
		{
			this.remoteCommand = new RemoteCommand(command, args);			
			this.SendEventInfo(this.vEventLevel,Evt.e4cmdrunned, "" );
			using (HttpWebResponse response = (HttpWebResponse) CreateWebRequest(this.remoteCommand).GetResponse())
			{
				if (response.StatusCode != HttpStatusCode.OK)
				{
					string msg = response.StatusDescription;
					this.SendEventInfo(this.vEventLevel,Evt.e1errorcmd, msg );						
					this.remoteCommand.ReportError( this.vErrorLevel, Err.e1server, msg );				
				}
				return ReadResponse(response);
			}
		
		}
		
		/// <summary>
		/// Runs the specified remote accessor (getter) command and returns the retrieved result
		/// </summary>
		/// <param name="commandName">the remote Command verb</param>
		/// <param name="args">the arguments to the remote Command (depends on the verb)</param>
		/// <returns>the result of running the accessor on the browser</returns>
		public String GetString(String commandName, String[] args) 
		{
			return DoCommand(commandName, args).Substring(3); // skip "OK,"
		}


		/// <summary>
		/// Retrieves the body of the HTTP response
		/// </summary>
		/// <param name="response">the response object to read</param>
		/// <returns>the body of the HTTP response</returns>
		public virtual string ReadResponse(HttpWebResponse response)
		{
			using (StreamReader reader = new StreamReader(response.GetResponseStream()))
			{
				return reader.ReadToEnd();
			}
		}

        /// <summary>
        /// Builds an HTTP request based on the specified remote Command
        /// </summary>
        /// <param name="command">the command we'll send to the server</param>
        /// <returns>an HTTP request, which will perform this command</returns>
        public virtual WebRequest CreateWebRequest(RemoteCommand command)
        {
            byte[] data = BuildCommandPostData(command.CommandString);
            
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            request.Timeout = Timeout.Infinite;
            
            Stream rs = request.GetRequestStream();
            rs.Write(data, 0, data.Length);
            rs.Close();
            
            return request;
        }

        private byte[] BuildCommandPostData(string commandString)
        {
            string data = commandString;
            if (sessionId != null)
            {
                data += "&sessionId=" + sessionId;
            }
            return (new UTF8Encoding()).GetBytes(data);
        }

        /// <summary>
        /// Sets the extension Javascript to be used in the created session
        /// </summary>
        public void SetExtensionJs(string extensionJs) 
        {
            this.extensionJs = extensionJs;
			RegKey.SetValue("extensionJs", extensionJs );
        }

		/// <summary>
		/// Creates a new browser session
		/// </summary>
		public void Start() 
		{
			string result = GetString("getNewBrowserSession", new String[] {browserStartCommand, browserURL, extensionJs});
			sessionId = result;	
		}

        /// <summary>
        /// Take any extra options that may be needed when creating a browser session
        /// </summary>
        /// <param name="optionString">Browser Options</param>
        public void Start(string serverHost, int serverPort, string browserStartCommand, string browserURL)
        {	
			this.url = "http://" + serverHost + ":"+ serverPort + "/selenium-server/driver/";
			this.browserStartCommand = browserStartCommand;
			this.browserURL = browserURL;
			this.extensionJs = "";
		
            string result = GetString("getNewBrowserSession", new String[] { browserStartCommand, browserURL, extensionJs, });
            this.sessionId = result;
			
			RegKey.SetValue("url", this.url );
			RegKey.SetValue("sessionId", this.sessionId );
			RegKey.SetValue("browserStartCommand", this.browserStartCommand );
			RegKey.SetValue("browserURL", this.browserURL );
			RegKey.SetValue("extensionJs", this.extensionJs );
        }

        /// <summary>
        /// Wraps the version of start() that takes a string parameter, sending it the result
        /// of calling ToString() on optionsObject, which will likey be a BrowserConfigurationOptions instan
        /// </summary>
        /// <param name="optionsObject">Contains BrowserConfigurationOptions </param>
        public void Start(Object optionsObject)
        {
            Start(optionsObject.ToString());
        }

		/// <summary>
		/// Stops the previous browser session, killing the browser
		/// </summary>
		public void Stop() 
		{
			DoCommand("testComplete", null);
			sessionId = null;
			RegKey.SetValue("url", "" );
			RegKey.SetValue("sessionId", "" );
			RegKey.SetValue("browserStartCommand", "" );
			RegKey.SetValue("browserURL", "" );
			RegKey.SetValue("extensionJs", "" );
		}


		/// <summary>
		/// Runs the specified remote accessor (getter) command and returns the retrieved result
		/// </summary>
		/// <param name="commandName">the remote Command verb</param>
		/// <param name="args">the arguments to the remote Command (depends on the verb)</param>
		/// <returns>the result of running the accessor on the browser</returns>
		public String[] GetStringArray(String commandName, String[] args)
		{
			String result = GetString(commandName, args);
			return parseCSV(result);
		}

		/// <summary>
		/// Parse Selenium comma separated values.
		/// </summary>
		/// <param name="input">the comma delimited string to parse</param>
		/// <returns>the parsed comma-separated entries</returns>
		public static String[] parseCSV(String input) 
		{
			ArrayList output = new ArrayList();
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < input.Length; i++) 
			{
				char c = input.ToCharArray()[i];
				switch (c) 
				{
					case ',':
						output.Add(sb.ToString());
						sb = new StringBuilder();
						continue;
					case '\\':
						i++;
						c = input.ToCharArray()[i];
						sb.Append(c);
						continue;
					default:
						sb.Append(c);
						break;
				}  
			}
			output.Add(sb.ToString());
			return (String[]) output.ToArray(typeof(String));
		}

		/// <summary>
		/// Runs the specified remote accessor (getter) command and returns the retrieved result
		/// </summary>
		/// <param name="commandName">the remote Command verb</param>
		/// <param name="args">the arguments to the remote Command (depends on the verb)</param>
		/// <returns>the result of running the accessor on the browser</returns>
		public Decimal GetNumber(String commandName, String[] args)
		{
			String result = GetString(commandName, args);
			Decimal d = Decimal.Parse(result);
			return d;
		}

		/// <summary>
		/// Runs the specified remote accessor (getter) command and returns the retrieved result
		/// </summary>
		/// <param name="commandName">the remote Command verb</param>
		/// <param name="args">the arguments to the remote Command (depends on the verb)</param>
		/// <returns>the result of running the accessor on the browser</returns>
		public Decimal[] GetNumberArray(String commandName, String[] args)
		{
			String[] result = GetStringArray(commandName, args);
			Decimal[] d = new Decimal[result.Length];
			for (int i = 0; i < result.Length; i++)
			{
				d[i] = Decimal.Parse(result[i]);
			}
			return d;
		}

		/// <summary>
		/// Runs the specified remote accessor (getter) command and returns the retrieved result
		/// </summary>
		/// <param name="commandName">the remote Command verb</param>
		/// <param name="args">the arguments to the remote Command (depends on the verb)</param>
		/// <returns>the result of running the accessor on the browser</returns>
		public bool GetBoolean(String commandName, String[] args)
		{
			String result = GetString(commandName, args);
			bool b;
			if ("true".Equals(result)) 
			{
				b = true;
				return b;
			}
			if ("false".Equals(result)) 
			{
				b = false;
				return b;
			}
			throw new Exception("result was neither 'true' nor 'false': " + result);
		}
		
		public void AssertTrue(String commandName, String[] args)
		{	string resultBody = SendCommand(commandName, args);
			if (!resultBody.StartsWith("OK"))
			{
				string msg = "Assert failed: " + resultBody.Substring(7);
				this.SendEventInfo(this.vEventLevel,Evt.e2assertfailed, msg );
				this.remoteCommand.ReportError( this.vErrorLevel,Err.e3assert, msg);
			}
		}
		
		public void VerifyTrue(String commandName, String[] args)
		{
			String resultBody = SendCommand(commandName, args);
			if (!resultBody.StartsWith("OK"))
			{
				string msg = "Verify failed: " + resultBody;
				this.SendEventInfo(this.vEventLevel,Evt.e3verifyfailed, msg );			
				this.remoteCommand.ReportError( this.vErrorLevel, Err.e4verify, msg );
			}
		}
		
		/// <summary>
		/// Runs the specified remote accessor (getter) command and returns the retrieved result
		/// </summary>
		/// <param name="commandName">the remote Command verb</param>
		/// <param name="args">the arguments to the remote Command (depends on the verb)</param>
		/// <returns>the result of running the accessor on the browser</returns>
		public bool[] GetBooleanArray(String commandName, String[] args)
		{
			String[] result = GetStringArray(commandName, args);
			bool[] b = new bool[result.Length];
			for (int i = 0; i < result.Length; i++)
			{
				if ("true".Equals(result)) 
				{
					b[i] = true;
					continue;
				}
				if ("false".Equals(result)) 
				{
					b[i] = false;
					continue;
				}
				throw new Exception("result was neither 'true' nor 'false': " + result);
			}
			return b;
		}

	}
}
