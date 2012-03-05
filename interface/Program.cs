using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;


namespace InterfaceGeneration
{
    class Program {
        static void Main(string[] args) {

            StringBuilder sb = new StringBuilder();

            Assembly lAssembly = Assembly.LoadFrom("Selenium.WebDriverBackedSelenium.dll");
            String[] exclusionList = new String[] {"Start", "get_Processor", "SetTimeout", "Open" };
            String[] noWaitActionsList = new String[] {"Open","SelectWindow","ChooseCancelOnNextConfirmation","AnswerOnNextPrompt","Close","SetContext","SetTimeout","SelectFrame","Stop","Wait","Set","Capture","Delete"};
			
            System.Reflection.MethodInfo[] lMethods = lAssembly.GetType("Selenium.WebDriverBackedSelenium").GetMethods();
            string[] lRet = new string[lMethods.Length];
            for (int i = 0; i < lMethods.Length; i++) {
                if (lMethods[i].DeclaringType.Name == "DefaultSelenium") {
                    string intMethodsName = lMethods[i].Name;
                    if( !Array.Exists(noWaitActionsList, p => lMethods[i].Name.Equals(p)) )
                    {
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
                        
                        if (lMethods[i].ReturnType.Name == "Void"){
                            sb.AppendLine("\t\tpublic " + lMethods[i].ReturnType.Name.Replace("Void", "void") + " " + Char.ToLower(lMethods[i].Name[0]) + lMethods[i].Name.Substring(1) + "(" + argsInt + "){"
                                    + "Invoke(()=>webDriver." + lMethods[i].Name + "(" + argsMeth + "));}");

                            if ( !Array.Exists(noWaitActionsList, p => lMethods[i].Name.StartsWith(p)) ) {
                                sb.AppendLine("\t\tpublic " + lMethods[i].ReturnType.Name.Replace("Void", "void") + " " + Char.ToLower(lMethods[i].Name[0]) + lMethods[i].Name.Substring(1) + "AndWait(" + argsInt + "){"
                                        + "InvokeAndWait(()=>webDriver." + lMethods[i].Name + "(" + argsMeth + "));}");
                            }

                        }else{
                            sb.AppendLine("\t\tpublic " + lMethods[i].ReturnType.Name + " " + Char.ToLower(lMethods[i].Name[0]) + lMethods[i].Name.Substring(1) + "(" + argsInt + "){"
                                    + " return (" + lMethods[i].ReturnType.Name + ")Invoke(()=>this.result=webDriver." + lMethods[i].Name + "(" + argsMeth + ")); }");

                            if (lMethods[i].Name.StartsWith("Get")){
                                if (argsInt != string.Empty)  argsInt += ", ";
                                argsInt += lMethods[i].ReturnType.Name + " expected";
                                sb.AppendLine("\t\tpublic void " + lMethods[i].Name.Replace("Get", "assert") + "(" + argsInt + "){"
                                        + "InvokeAssert(()=>this.result=webDriver." + lMethods[i].Name + "(" + argsMeth + "),expected,true);}");

                                sb.AppendLine("\t\tpublic void " + lMethods[i].Name.Replace("Get", "assertNot") + "(" + argsInt + "){"
                                        + "InvokeAssert(()=>this.result=webDriver." + lMethods[i].Name + "(" + argsMeth + "),expected,false);}");

                                sb.AppendLine("\t\tpublic String " + lMethods[i].Name.Replace("Get", "verify") + "(" + argsInt + "){"
                                        + "return InvokeVerify(()=>this.result=webDriver." + lMethods[i].Name + "(" + argsMeth + "),expected,true);}");

                                sb.AppendLine("\t\tpublic String " + lMethods[i].Name.Replace("Get", "verifyNot") + "(" + argsInt + "){"
                                        + "return InvokeVerify(()=>this.result=webDriver." + lMethods[i].Name + "(" + argsMeth + "),expected,false);}");

                                sb.AppendLine("\t\tpublic void " + lMethods[i].Name.Replace("Get", "waitFor") + "(" + argsInt + "){"
                                        + "InvokeWaitFor(()=>this.result=webDriver." + lMethods[i].Name + "(" + argsMeth + "),expected,true);}");

                                sb.AppendLine("\t\tpublic void " + lMethods[i].Name.Replace("Get", "waitForNot") + "(" + argsInt + "){"
                                        + "InvokeWaitFor(()=>this.result=webDriver." + lMethods[i].Name + "(" + argsMeth + "),expected,false);}");
                            }else if(lMethods[i].Name.StartsWith("Is")){
                                sb.AppendLine("\t\tpublic void " + lMethods[i].Name.Replace("Is", "assert") + "(" + argsInt + "){"
                                        + "InvokeAssert(()=>this.result=webDriver." + lMethods[i].Name + "(" + argsMeth + "),true,true);}");

                                sb.AppendLine("\t\tpublic String " + lMethods[i].Name.Replace("Is", "verify") + "(" + argsInt + "){"
                                        + "return InvokeVerify(()=>this.result=webDriver." + lMethods[i].Name + "(" + argsMeth + "),true,true);}");

                                sb.AppendLine("\t\tpublic void " + lMethods[i].Name.Replace("Is", "waitFor") + "(" + argsInt + "){"
                                        + "InvokeWaitFor(()=>this.result=webDriver." + lMethods[i].Name + "(" + argsMeth + "),true,true);}");

                                if(lMethods[i].Name.EndsWith("Present")){
                                    sb.AppendLine("\t\tpublic void " + lMethods[i].Name.Replace("Is", "assert").Replace("Present", "NotPresent") + "(" + argsInt + "){"
                                            + "InvokeAssert(()=>this.result=webDriver." + lMethods[i].Name + "(" + argsMeth + "),false,true);}");

                                    sb.AppendLine("\t\tpublic String " + lMethods[i].Name.Replace("Is", "verify").Replace("Present", "NotPresent") + "(" + argsInt + "){"
                                            + "return InvokeVerify(()=>this.result=webDriver." + lMethods[i].Name + "(" + argsMeth + "),false,true);}");

                                    sb.AppendLine("\t\tpublic void " + lMethods[i].Name.Replace("Is", "waitFor").Replace("Present", "NotPresent") + "(" + argsInt + "){"
                                            + "InvokeWaitFor(()=>this.result=webDriver." + lMethods[i].Name + "(" + argsMeth + "),false,true);}");
                                }else{
                                    sb.AppendLine("\t\tpublic void " + lMethods[i].Name.Replace("Is", "assertNot") + "(" + argsInt + "){"
                                            + "InvokeAssert(()=>this.result=webDriver." + lMethods[i].Name + "(" + argsMeth + "),false,true);}");

                                    sb.AppendLine("\t\tpublic String " + lMethods[i].Name.Replace("Is", "verifyNot") + "(" + argsInt + "){"
                                            + "return InvokeVerify(()=>this.result=webDriver." + lMethods[i].Name + "(" + argsMeth + "),false,true);}");

                                    sb.AppendLine("\t\tpublic void " + lMethods[i].Name.Replace("Is", "waitForNot") + "(" + argsInt + "){"
                                            + "InvokeWaitFor(()=>this.result=webDriver." + lMethods[i].Name + "(" + argsMeth + "),false,true);}");
                                }
                            }
                        }
                    }
                }
            }

            sb.Replace("CSSCount(String", "CssCount(String");

            TextWriter fclasse = new StreamWriter("..\\..\\class.txt");
            fclasse.WriteLine("\t\t#region Auto-Generated Code");
            fclasse.Write(sb.ToString());
            fclasse.WriteLine("\t\t#endregion");
            fclasse.Close();

            TextWriter finterface = new StreamWriter("..\\..\\interface.txt");
            finterface.WriteLine("\t\t#region Auto-Generated Code");
            MatchCollection mc = Regex.Matches(sb.ToString(), "public ([^\\)]+\\))");
            if (mc.Count > 0){
                for (int i = 0; i < mc.Count; i++){
                    GroupCollection gc = mc[i].Groups;
                    finterface.WriteLine("\t\t" + gc[1].Value + ";");
                }
            }
            finterface.WriteLine("\t\t#endregion");
            finterface.Close();

        }
    }
}
