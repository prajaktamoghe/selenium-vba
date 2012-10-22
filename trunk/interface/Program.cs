using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;


namespace InterfaceGeneration
{
    class Program {
        static void Main(string[] args) {

            Parser parser = new Parser();

            parser.Parse();

        }

        public class Parser{

            StringBuilder sbClass = new StringBuilder();    
            StringBuilder sbInterface = new StringBuilder();
            private bool parseToDecimal=false;    
            
            public Parser(){
            
            
            }

            public void Parse(){

                StringBuilder sb = new StringBuilder();

                Assembly lAssembly = Assembly.LoadFrom("Selenium.WebDriverBackedSelenium.dll");
                String[] exclusionList = new String[] { "ChooseOkOnNextConfirmation", "ChooseCancelOnNextConfirmation", "Start", "get_Processor", "SetTimeout", "Open", "WindowMaximize" };
                String[] noWaitActionsList = new String[] { "Open", "SelectWindow", "ChooseCancelOnNextConfirmation", "AnswerOnNextPrompt", "Close", "SetContext", "SetTimeout", "SelectFrame", "Stop", "Wait", "Set", "Capture", "Delete", "WindowFocus" };

                System.Reflection.MethodInfo[] lMethods = lAssembly.GetType("Selenium.WebDriverBackedSelenium").GetMethods();
                string[] lRet = new string[lMethods.Length];
                for (int i = 0; i < lMethods.Length; i++)
                {
                    if (lMethods[i].DeclaringType.Name == "DefaultSelenium")
                    {
                        string intMethodsName = lMethods[i].Name;
                        if ( !Array.Exists(exclusionList, p => lMethods[i].Name.Equals(p)) )
                        {
                            XmlElement xmlElement = XMLFromMember(lMethods[i]);
                            string comment = Regex.Replace(xmlElement.InnerText.Replace("\\\"", "\"").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r\n", " "), @"[ ]{2,}", " ");
                            sbInterface.AppendLine("\t\t[Description(\"" + comment + "\")]");
                            sbClass.AppendLine("\t\t/// <summary>" + comment.Replace('<','"').Replace('>','"').Replace("&","and") + "</summary>");
                            string argsInt = string.Empty;
                            string argsMeth = string.Empty;
                            System.Reflection.ParameterInfo[] lParameters = lMethods[i].GetParameters();
                            for (int j = 0; j < lParameters.Length; j++)
                            {
                                if (argsInt != string.Empty) { argsInt += ", "; }
                                if (argsMeth != string.Empty) { argsMeth += ", "; }
                                argsInt += lParameters[j].ParameterType.Name + " " + lParameters[j].Name;
                                argsMeth += lParameters[j].Name;
                            }
                            string retType = lMethods[i].ReturnType.Name.Replace("Void", "void");
                            string expected = "expected";
                            this.parseToDecimal = false;
                            if(lMethods[i].ReturnType == typeof(Decimal)){
                                this.parseToDecimal=true;
                                expected = "Convert.ToDecimal(" + expected + ")";
                                retType = "Double";
                            }
                            if (retType == "void")
                            {
                                WriteLine( retType + " " + Char.ToLower(lMethods[i].Name[0]) + lMethods[i].Name.Substring(1) + "(" + argsInt + ")",
                                        "InvokeWd(()=>webDriverBacked." + lMethods[i].Name + "(" + argsMeth + "))");

                                if (!Array.Exists(noWaitActionsList, p => lMethods[i].Name.StartsWith(p)))
                                {
                                    WriteLine( retType + " " + Char.ToLower(lMethods[i].Name[0]) + lMethods[i].Name.Substring(1) + "AndWait(" + argsInt + ")",
                                            "InvokeWdAndWait(()=>webDriverBacked." + lMethods[i].Name + "(" + argsMeth + "))");
                                }

                            }
                            else
                            {
                                WriteLine( retType + " " + Char.ToLower(lMethods[i].Name[0]) + lMethods[i].Name.Substring(1) + "(" + argsInt + ")",
                                        " return (" + retType + ")InvokeWd(()=>this.result=webDriverBacked." + lMethods[i].Name + "(" + argsMeth + "))");

                                if (lMethods[i].Name.StartsWith("Get"))
                                {
                                    if (argsInt != string.Empty) argsInt += ", ";
                                    argsInt += retType + " expected";
                                    WriteLine( "void " + lMethods[i].Name.Replace("Get", "assert") + "(" + argsInt + ")",
                                            "InvokeWdAssert(()=>this.result=webDriverBacked." + lMethods[i].Name + "(" + argsMeth + ")," + expected + ",true)");

                                    WriteLine( "void " + lMethods[i].Name.Replace("Get", "assertNot") + "(" + argsInt + ")",
                                            "InvokeWdAssert(()=>this.result=webDriverBacked." + lMethods[i].Name + "(" + argsMeth + ")," + expected + ",false)");

                                    WriteLine( "String " + lMethods[i].Name.Replace("Get", "verify") + "(" + argsInt + ")",
                                            "return InvokeWdVerify(()=>this.result=webDriverBacked." + lMethods[i].Name + "(" + argsMeth + ")," + expected + ",true)");

                                    WriteLine( "String " + lMethods[i].Name.Replace("Get", "verifyNot") + "(" + argsInt + ")",
                                            "return InvokeWdVerify(()=>this.result=webDriverBacked." + lMethods[i].Name + "(" + argsMeth + ")," + expected + ",false)");

                                    WriteLine( "void " + lMethods[i].Name.Replace("Get", "waitFor") + "(" + argsInt + ")",
                                            "InvokeWdWaitFor(()=>this.result=webDriverBacked." + lMethods[i].Name + "(" + argsMeth + ")," + expected + ",true)");

                                    WriteLine( "void " + lMethods[i].Name.Replace("Get", "waitForNot") + "(" + argsInt + ")",
                                            "InvokeWdWaitFor(()=>this.result=webDriverBacked." + lMethods[i].Name + "(" + argsMeth + ")," + expected + ",false)");
                                }
                                else if (lMethods[i].Name.StartsWith("Is"))
                                {
                                    WriteLine( "void " + lMethods[i].Name.Replace("Is", "assert") + "(" + argsInt + ")",
                                            "InvokeWdAssert(()=>this.result=webDriverBacked." + lMethods[i].Name + "(" + argsMeth + "),true,true)");

                                    WriteLine( "String " + lMethods[i].Name.Replace("Is", "verify") + "(" + argsInt + ")",
                                            "return InvokeWdVerify(()=>this.result=webDriverBacked." + lMethods[i].Name + "(" + argsMeth + "),true,true)");

                                    WriteLine( "void " + lMethods[i].Name.Replace("Is", "waitFor") + "(" + argsInt + ")",
                                            "InvokeWdWaitFor(()=>this.result=webDriverBacked." + lMethods[i].Name + "(" + argsMeth + "),true,true)");

                                    if (lMethods[i].Name.EndsWith("Present"))
                                    {
                                        WriteLine( "void " + lMethods[i].Name.Replace("Is", "assert").Replace("Present", "NotPresent") + "(" + argsInt + ")",
                                                "InvokeWdAssert(()=>this.result=webDriverBacked." + lMethods[i].Name + "(" + argsMeth + "),false,true)");

                                        WriteLine( "String " + lMethods[i].Name.Replace("Is", "verify").Replace("Present", "NotPresent") + "(" + argsInt + ")",
                                                "return InvokeWdVerify(()=>this.result=webDriverBacked." + lMethods[i].Name + "(" + argsMeth + "),false,true)");

                                        WriteLine( "void " + lMethods[i].Name.Replace("Is", "waitFor").Replace("Present", "NotPresent") + "(" + argsInt + ")",
                                                "InvokeWdWaitFor(()=>this.result=webDriverBacked." + lMethods[i].Name + "(" + argsMeth + "),false,true)");
                                    }
                                    else
                                    {
                                        WriteLine( "void " + lMethods[i].Name.Replace("Is", "assertNot") + "(" + argsInt + ")",
                                                "InvokeWdAssert(()=>this.result=webDriverBacked." + lMethods[i].Name + "(" + argsMeth + "),false,true)");

                                        WriteLine( "String " + lMethods[i].Name.Replace("Is", "verifyNot") + "(" + argsInt + ")",
                                                "return InvokeWdVerify(()=>this.result=webDriverBacked." + lMethods[i].Name + "(" + argsMeth + "),false,true)");

                                        WriteLine( "void " + lMethods[i].Name.Replace("Is", "waitForNot") + "(" + argsInt + ")",
                                                "InvokeWdWaitFor(()=>this.result=webDriverBacked." + lMethods[i].Name + "(" + argsMeth + "),false,true)");
                                    }
                                }
                            }
                        }
                    }
                }

                sbClass.Replace("CSSCount(String", "CssCount(String");
                sbInterface.Replace("CSSCount(String", "CssCount(String");
            //    sbInterface.Replace("\tDecimal ", "\t[return: MarshalAs(UnmanagedType.Currency)]Decimal ");
            //    sbInterface.Replace("Decimal expected", "[MarshalAs(UnmanagedType.Currency)]Decimal expected");
                sbInterface.Replace("CSSCount(String", "CssCount(String");

                TextWriter fclasse = new StreamWriter("..\\..\\..\\wrapper\\WebDriverGen.cs");
                fclasse.WriteLine("using System;");
                fclasse.WriteLine("using System.Collections.Generic;");
                fclasse.WriteLine("using System.Runtime.InteropServices;");
                fclasse.WriteLine("using System.Text;");
                fclasse.WriteLine("");
                fclasse.WriteLine("namespace SeleniumWrapper");
                fclasse.WriteLine("{");
                fclasse.WriteLine("    partial class WebDriver");
                fclasse.WriteLine("    {");
                fclasse.WriteLine("        // Following funtion are automatically generated by reflexion");
                fclasse.WriteLine("        #region Auto-Generated Code");
                fclasse.Write(sbClass.ToString());
                fclasse.WriteLine("        #endregion");
                fclasse.WriteLine("    }");
                fclasse.WriteLine("}");
                fclasse.Close();


                TextWriter finterface = new StreamWriter("..\\..\\..\\wrapper\\IWebDriverGen.cs");
                finterface.WriteLine("using System;");
                finterface.WriteLine("using System.Collections.Generic;");
                finterface.WriteLine("using System.Runtime.InteropServices;");
                finterface.WriteLine("using System.Text;");
                finterface.WriteLine("using System.ComponentModel;");
                finterface.WriteLine("");
                finterface.WriteLine("namespace SeleniumWrapper");
                finterface.WriteLine("{");
                finterface.WriteLine("    public partial interface IWebDriver");
                finterface.WriteLine("    {");
                finterface.WriteLine("        // Following funtion are automatically generated by reflexion");
                finterface.WriteLine("        #region Auto-Generated Code");
                finterface.Write(sbInterface.ToString());
                finterface.WriteLine("        #endregion");
                finterface.WriteLine("    }");
                finterface.WriteLine("}");
                finterface.Close();

       /*         MatchCollection mc = Regex.Matches(sb.ToString(), "public ([^\\)]+\\))");
                if (mc.Count > 0)
                {
                    for (int i = 0; i < mc.Count; i++)
                    {
                        GroupCollection gc = mc[i].Groups;
                        finterface.WriteLine("\t\t" + gc[1].Value + ";");
                    }
                }
        */
            }

            public void WriteLine( string methodDeclaration, string methodContent){
                sbInterface.AppendLine("\t\t" + methodDeclaration + ";");
                if (this.parseToDecimal && methodContent.IndexOf("return")>0 )
                {
                    sbClass.AppendLine("\t\tpublic " + methodDeclaration + "{" + methodContent.Replace("return (Double)", "return Convert.ToDouble(") + ");}");
                }else{
                    sbClass.AppendLine("\t\tpublic " + methodDeclaration + "{" + methodContent + ";}");
                }
            }


        }



                /// <summary>
        /// Provides the documentation comments for a specific member
        /// </summary>
        /// <param name="memberInfo">The MemberInfo (reflection data) or the member to find documentation for</param>
        /// <returns>The XML fragment describing the member</returns>
        public static XmlElement XMLFromMember(MemberInfo memberInfo)
        {
            string parameters = null;
            if(memberInfo is MethodInfo){
                foreach (ParameterInfo pi in ((MethodInfo)memberInfo).GetParameters())
                {
                    if(parameters!=null) parameters += ",";
                    parameters += pi.ParameterType.ToString(); //, pi.Name);
                }
            }
            if (parameters != null) parameters = "(" + parameters + ")";
            return XMLFromName(memberInfo.DeclaringType, memberInfo.MemberType.ToString()[0], memberInfo.Name + parameters);
        }

        /// <summary>
        /// Provides the documentation comments for a specific type
        /// </summary>
        /// <param name="type">Type to find the documentation for</param>
        /// <returns>The XML fragment that describes the type</returns>
        public static XmlElement XMLFromType(Type type)
        {
            // Prefix in type names is T
            return XMLFromName(type, 'T', "");
        }

        /// <summary>
        /// Obtains the XML Element that describes a reflection element by searching the 
        /// members for a member that has a name that describes the element.
        /// </summary>
        /// <param name="type">The type or parent type, used to fetch the assembly</param>
        /// <param name="prefix">The prefix as seen in the name attribute in the documentation XML</param>
        /// <param name="name">Where relevant, the full name qualifier for the element</param>
        /// <returns>The member that has a name that describes the specified reflection element</returns>
        private static XmlElement XMLFromName(Type type, char prefix, string name)
        {
            string fullName;

            if (String.IsNullOrEmpty(name))
            {
                fullName = prefix + ":" + type.FullName;
            }
            else
            {
                fullName = prefix + ":" + type.FullName + "." + name;
            }

            XmlDocument xmlDocument = XMLFromAssembly(type.Assembly);
     //       ParameterInfo param = ((MethodInfo)type.).GetParameters();


            XmlElement matchedElement = null;

            foreach (XmlElement xmlElement in xmlDocument["doc"]["members"])
            {
                if (xmlElement.Attributes["name"].Value.Equals(fullName))
                {
                    if (matchedElement != null)
                    {
                        throw new Exception("Multiple matches to query");
                    }

                    matchedElement = xmlElement;
                }
            }

            if (matchedElement == null)
            {
                throw new Exception("Could not find documentation for specified element");
            }

            return matchedElement;
        }

        /// <summary>
        /// A cache used to remember Xml documentation for assemblies
        /// </summary>
        static Dictionary<Assembly, XmlDocument> cache = new Dictionary<Assembly, XmlDocument>();

        /// <summary>
        /// A cache used to store failure exceptions for assembly lookups
        /// </summary>
        static Dictionary<Assembly, Exception> failCache = new Dictionary<Assembly, Exception>();

        /// <summary>
        /// Obtains the documentation file for the specified assembly
        /// </summary>
        /// <param name="assembly">The assembly to find the XML document for</param>
        /// <returns>The XML document</returns>
        /// <remarks>This version uses a cache to preserve the assemblies, so that 
        /// the XML file is not loaded and parsed on every single lookup</remarks>
        public static XmlDocument XMLFromAssembly(Assembly assembly)
        {
            if (failCache.ContainsKey(assembly))
            {
                throw failCache[assembly];
            }

            try
            {

                if (!cache.ContainsKey(assembly))
                {
                    // load the docuemnt into the cache
                    cache[assembly] = XMLFromAssemblyNonCached(assembly);
                }

                return cache[assembly];
            }
            catch (Exception exception)
            {
                failCache[assembly] = exception;
                throw exception;
            }
        }

        /// <summary>
        /// Loads and parses the documentation file for the specified assembly
        /// </summary>
        /// <param name="assembly">The assembly to find the XML document for</param>
        /// <returns>The XML document</returns>
        private static XmlDocument XMLFromAssemblyNonCached(Assembly assembly)
        {
            string assemblyFilename = assembly.CodeBase;

            const string prefix = "file:///";

            if (assemblyFilename.StartsWith(prefix))
            {
                StreamReader streamReader;

                try
                {
                    streamReader = new StreamReader(Path.ChangeExtension(assemblyFilename.Substring(prefix.Length), ".xml"));
                }
                catch (FileNotFoundException exception)
                {
                    throw new Exception("XML documentation not present (make sure it is turned on in project properties when building)" + exception.Message);
                }

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(streamReader);
                return xmlDocument;
            }
            else
            {
                throw new Exception("Could not ascertain assembly filename", null);
            }
        }
    }
}
