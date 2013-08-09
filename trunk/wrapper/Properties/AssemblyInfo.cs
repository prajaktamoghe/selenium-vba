using System.Runtime.CompilerServices;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ComponentModel;
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("Selenium Wrapper")]
[assembly: AssemblyDescription("SeleniumWrapper Type Library")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Florent BREHERET")]
[assembly: AssemblyProduct("SeleniumWrapper")]
[assembly: AssemblyCopyright("Copyright © Florent Breheret 2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: System.Runtime.InteropServices.ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: System.Runtime.InteropServices.Guid("e57e03de-c7fe-4c12-85c8-ec8b32dffb86")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("1.0.14.1")]

namespace SeleniumWrapper
{
    [Guid("94EE4CF6-6968-4B30-83D6-1C6CC915103B"), ComVisible(true)]
    public interface IAssembly
    {
        [Description("Get the assembly version")]
        string GetVersion();
    }

    /// <summary>
    /// Class to get informations about the regitered assembly.
    /// </summary>

    [Description("Class to get informations about the regitered assembly"), ProgId("SeleniumWrapper.Assembly")]
    [Guid("5BDDC122-7092-453F-8486-DBC455180DE3"), ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class Assembly : IAssembly
    {
        public string GetVersion()
        {
            try
            {
                //return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                return System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion.ToString();
            }
            catch (System.Exception)
            {
                return null;
            }
        }
    }
}
