using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SeleniumWrapper {
    /// <summary>Commands for the Select web element</summary>

    [Description("Commands for the Select web element")]
    [Guid("D1A1CC9D-5CE0-41CE-B16B-4CE442A16502")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface Select {
        [Description("")]
        bool IsMultiple { get; }

        [Description("Returns a list of all options belonging to this select tag")]
        WebElementCollection Options { get; }

        [Description("The first selected option in this select tag (or the currently selected option in a normal select)")]
        IWebElement SelectedOption { get; }

        [Description("Gets all of the selected options within the select element.")]
        WebElementCollection AllSelectedOptions { get; }

        [Description("Select the option at the given index. This is done by examing the “index” attribute of an element, and not merely by counting.")]
        void selectByIndex(int index);

        [Description("Select all options that display text matching the argument.")]
        void selectByText(string text);

        [Description("Select all options that have a value matching the argument.")]
        void selectByValue(string value);

        [Description("Clear all selected entries. This is only valid when the SELECT supports multiple selections. throws NotImplementedError If the SELECT does not support multiple selections")]
        void deselectAll();

        [Description("Deselect the option at the given index. This is done by examing the “index” attribute of an element, and not merely by counting.")]
        void deselectByIndex(int index);

        [Description("Deselect all options that display text matching the argument.")]
        void deselectByText(string text);

        [Description("Deselect all options that have a value matching the argument. That is, when given “foo” this would deselect an option like:")]
        void deselectByValue(string value);

    }

}
