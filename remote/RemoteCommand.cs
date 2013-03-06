
using System;
using System.Web;
using System.Text;
using System.Runtime.InteropServices;

namespace Selenium
{

	[ComVisible(false)]
	public class RemoteCommand
	{
		private static readonly string PARSE_ERROR_MESSAGE = "Command string must contain 4 pipe characters and should start with a '|'. Unable to parse command string";
		private readonly string[] args;
		private readonly string command;
		private string lErrorInfo;
		private Err lErrorType;

		public RemoteCommand(string command, string[] args)
		{
			this.command = command;
			this.args = args;
		}

		public string CommandString
		{
			get
			{
				StringBuilder sb = new StringBuilder("cmd=");
				sb.Append(HttpUtility.UrlEncode(command));
				if (args == null) return sb.ToString();
				for (int i = 0; i < args.Length; i++)
				{
					sb.Append('&').Append((i+1).ToString()).Append('=').Append(HttpUtility.UrlEncode(args[i]));
				}
				return sb.ToString();
			}
		}
		
		public string Command
		{
			get { return this.command; }
		}

		public string[] Args
		{
			get { return this.args; }
		}
		
		public void ReportError( Err errorLevel, Err errtype, string message)
		{
			this.lErrorType =errtype;
			this.lErrorInfo = message;
			if ( errtype <= errorLevel ) throw new Exception(message);
		}
			
		public string ErrorInfo
		{
			get { return this.lErrorInfo; }
			set{ this.lErrorInfo=value; }
		}
		
		public Err ErrorType
		{
			get { return this.lErrorType; }
		}

		public static RemoteCommand Parse(string commandString)
		{
			if (commandString == null || commandString.Trim().Length == 0 || !commandString.StartsWith("|"))
			{
				throw new ArgumentException(PARSE_ERROR_MESSAGE + "'" + commandString + "'.");
			}

			string[] commandArray = commandString.Split(new char[] { '|' });
			
			if (commandArray.Length != 5)
			{
				throw new ArgumentException(PARSE_ERROR_MESSAGE + "'" + commandString + "'.");
			}
			
			return new RemoteCommand(commandArray[1], new String[] {commandArray[2], commandArray[3]});
		}
	}
}
