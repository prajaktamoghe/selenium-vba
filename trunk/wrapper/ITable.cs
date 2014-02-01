using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SeleniumWrapper {
    /// <summary>Commands for the Table web element</summary>

    [Description("Commands for the Table web element")]
    [Guid("61208894-B0CE-46C5-915A-BE15C121F199")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface Table {

        [Description("Return an array filled with data from a table element")]
        object[,] getData(int firstRowsToSkip = 0, int lastRowsToSkip = 0);
    }

}
