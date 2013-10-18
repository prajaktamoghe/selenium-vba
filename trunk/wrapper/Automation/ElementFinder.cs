using System;
using System.Collections.Generic;
using System.Windows.Automation;

namespace SeleniumWrapper.Automation {

    /// <summary>
    /// Extension methods for AutomationElement to search elements
    /// </summary>
    public static class ElementFinder {

        private static TreeWalker _treewalker = new TreeWalker(
            new AndCondition(
                new PropertyCondition(AutomationElement.IsOffscreenProperty, false),
                new PropertyCondition(AutomationElement.IsControlElementProperty, true)
            )
        );

        #region Control Types

        private static readonly Dictionary<string, ControlType> _controlTypes = new Dictionary<string, ControlType> {
            { "button", ControlType.Button },
            { "calendar", ControlType.Calendar },
            { "checkbox", ControlType.CheckBox },
            { "combobox", ControlType.ComboBox },
            { "custom", ControlType.Custom },
            { "datagrid", ControlType.DataGrid },
            { "dataitem", ControlType.DataItem },
            { "document", ControlType.Document },
            { "edit", ControlType.Edit },
            { "group", ControlType.Group },
            { "header", ControlType.Header },
            { "headeritem", ControlType.HeaderItem },
            { "hyperlink", ControlType.Hyperlink },
            { "image", ControlType.Image },
            { "list", ControlType.List },
            { "listitem", ControlType.ListItem },
            { "menu", ControlType.Menu },
            { "menubar", ControlType.MenuBar },
            { "menuitem", ControlType.MenuItem },
            { "pane", ControlType.Pane },
            { "progressbar", ControlType.ProgressBar },
            { "radiobutton", ControlType.RadioButton },
            { "scrollbar", ControlType.ScrollBar },
            { "separator", ControlType.Separator },
            { "slider", ControlType.Slider },
            { "spinner", ControlType.Spinner },
            { "splitbutton", ControlType.SplitButton },
            { "statusbar", ControlType.StatusBar },
            { "tab", ControlType.Tab },
            { "tabitem", ControlType.TabItem },
            { "table", ControlType.Table },
            { "text", ControlType.Text },
            { "thumb", ControlType.Thumb },
            { "titlebar", ControlType.TitleBar },
            { "toolbar", ControlType.ToolBar },
            { "tooltip", ControlType.ToolTip },
            { "tree", ControlType.Tree },
            { "treeitem", ControlType.TreeItem },
            { "window", ControlType.Window }
        };

        #endregion

        private static ControlType GetControlType(string controlType) {
            try {
                return _controlTypes[controlType.Trim().Replace(" ", "").ToLower()];
            }catch{
                throw new Exception("Elements of type '" + controlType + "' are not available"); 
            }
        }

        public static AutomationElementCollection FindOtherTopElements(this AutomationElement element) {
            return AutomationElement.RootElement.FindAll(
                TreeScope.Children, 
                new AndCondition(
                    new PropertyCondition(AutomationElement.ProcessIdProperty, element.Current.ProcessId),
                    new NotCondition( new PropertyCondition(AutomationElement.NativeWindowHandleProperty, element.Current.NativeWindowHandle))
                )
            );
        }

        public static AutomationElement FindFirstByName(this AutomationElement element, string name, TreeScope scope) {
            if (name.IndexOf('*') == -1 && name.IndexOf('?') == -1)
                return element.FindFirst(
                    scope,
                    new AndCondition(
                        new PropertyCondition(AutomationElement.IsOffscreenProperty, false),
                        new PropertyCondition(AutomationElement.NameProperty, name)
                    )
                );
            else
                return element.FindFirst(scope, (ele) => NatHelper.WildcardMatch(name, ele.Current.Name));
        }

        // Debug->Exceptions/"Managed Debugging Assistants" -> Uncheck the NonComVisibleBaseClass Thrown option


        public static AutomationElement FindFirstByTypeAndName(this AutomationElement element, ControlType type, string name, TreeScope scope) {
            if (name.IndexOf('*') == -1 && name.IndexOf('?') == -1)
                return element.FindFirst(
                    scope,
                    new AndCondition(
                        new PropertyCondition(AutomationElement.IsOffscreenProperty, false),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, type),
                        new PropertyCondition(AutomationElement.NameProperty, name)
                    )
                );
            else
                return element.FindFirst(scope, (ele) => ele.Current.ControlType == type && NatHelper.WildcardMatch(name, ele.Current.Name));

        }

        public static AutomationElement FindChildByPosition(this AutomationElement element, ControlType type, int position) {
            var condition = new AndCondition(
                new PropertyCondition(AutomationElement.IsOffscreenProperty, false),
                new PropertyCondition(AutomationElement.ControlTypeProperty, type)
            );
            if (position == 0) {
                return element.FindFirst(TreeScope.Children, condition);
            } else {
                var childElements = (AutomationElementCollection)element.FindAll(TreeScope.Children, condition);
                if (position > childElements.Count)
                    throw new Exception("The provided position (" + position + ") is superior to number of elements (" + childElements.Count + ")");
                return childElements[position];
            }
        }

        public static AutomationElement FindChildByPosition(this AutomationElement element, int position) {
            var condition = new PropertyCondition(AutomationElement.IsOffscreenProperty, false);
            if (position == 0) {
                return element.FindFirst(TreeScope.Children, condition);
            } else {
                var childElements = (AutomationElementCollection)element.FindAll(TreeScope.Children, condition);
                if (position > childElements.Count)
                    throw new Exception("The provided position (" + position + ") is superior to number of elements (" + childElements.Count + ")");
                return childElements[position];
            }
        }

        public static AutomationElementCollection FindAllByType(this AutomationElement element, ControlType type, TreeScope scope) {
            return element.FindAll(
                scope,
                new AndCondition(
                    new PropertyCondition(AutomationElement.IsOffscreenProperty, false),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, type)
                )
            );
        }

        public static AutomationElement FindFirst(this AutomationElement element, TreeScope scope, Func<AutomationElement, bool> condition) {
            if (scope == TreeScope.Children)
                return element.FindFirstChild(condition);
            else if (scope == TreeScope.Descendants)
                return element.FindFirstDescendant(condition);
            else
                throw new Exception("This method doesn't support this scope: " + scope.ToString());
        }

        public static AutomationElement FindFirstDescendant(this AutomationElement element, Func<AutomationElement, bool> condition) {
            element = _treewalker.GetFirstChild(element);
            while (element != null) {
                if (condition(element))
                    return element;
                var subElement = FindFirstDescendant(element, condition);
                if (subElement != null)
                    return subElement;
                element = _treewalker.GetNextSibling(element);
            }
            return null;
        }

        public static AutomationElement FindFirstChild(this AutomationElement element, Func<AutomationElement, bool> condition) {
            element = _treewalker.GetFirstChild(element);
            while (element != null) {
                if (condition(element))
                    return element;
                element = _treewalker.GetNextSibling(element);
            }
            return null;
        }

        public static AutomationElement FindFirstByPath(this AutomationElement element, string path) {
            //Return the element if there's no more element to extract from the path
            if (element == null || path.Length == 0)
                return element;
            //Parse the first element in the path
            int start = 0, i = 0;
            while (start < path.Length && path[start] == '/')
                start++;
            string arg1 = string.Empty;
            string arg2 = string.Empty;
            bool inQuotes = false;
            bool inBracket = false;
            for (i = start; i < path.Length; i++) {
                var car = path[i];
                if (inBracket) {
                    if (inQuotes) {
                        if (car == '\'' && path[i - 1] != '\\')
                            inQuotes = false;
                        arg2 += car;
                    } else {
                        if (car == ']') {
                            inBracket = false;
                            continue;
                        }
                        if (car == '\'')
                            inQuotes = true;
                        arg2 += car;
                    }
                } else if (car == '[')
                    inBracket = true;
                else if (car == '/') 
                    break;
                else
                    arg1 += car;
            }

            //Search the element
            var nextPath = i < path.Length ? path.Substring(i) : string.Empty;

            if (arg2.Length == 0) {
                if (nextPath.Length == 0)
                    //Search on text
                    element = element.FindFirstByName(arg1, start == 1 ? TreeScope.Children : TreeScope.Descendants);
                else{
                    //Search in all types by recursion
                    var elements = element.FindAllByType(GetControlType(arg1), start == 2 ? TreeScope.Descendants : TreeScope.Children);
                    foreach (AutomationElement ele in elements) {
                        var subEle = ele.FindFirstByPath(nextPath);
                        if (subEle != null)
                            return subEle;
                    }
                }
            } else {
                int num;
                if (int.TryParse(arg2.Trim(), out num)) {
                    if (start == 2) throw new ArgumentException("Search on descendant by position is not available");
                    if(arg1.Length == 0)
                        //Search a child on position
                        element = element.FindChildByPosition(num);
                    else
                        //Search a child on type and position
                        element = element.FindChildByPosition(GetControlType(arg1), num);
                } else {
                    if(arg1.Length == 0)
                        //Search on name
                        element = element.FindFirstByName(arg2.Trim('\''), start == 1 ? TreeScope.Children : TreeScope.Descendants);
                    else
                        //Search on type and name
                        element = element.FindFirstByTypeAndName(GetControlType(arg1), arg2.Trim('\''), start == 2 ? TreeScope.Descendants : TreeScope.Children);
                }
            }
            //Search the next element by recursion
            return element.FindFirstByPath(nextPath);
        }

    }
}
