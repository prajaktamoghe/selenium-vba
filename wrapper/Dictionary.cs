using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SeleniumWrapper {

    [Guid("2305D4BC-6D57-4272-AD42-1456C48D8E5C")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IDictionary {

        int Count();

        System.Collections.IEnumerator GetEnumerator();

        object Item(object Index);

        object[] Keys { get; }

        object[] Values { get; }
    }


    [Description("Waiting functions to keep the visual basic editor from not responding")]
    [Guid("8B7667DB-3D5E-4724-B0B6-CEA7D8FD6954")]
    [ComVisible(true), ComDefaultInterface(typeof(IDictionary)), ClassInterface(ClassInterfaceType.None)]
    public class Dictionary : Dictionary<object, object>, IDictionary {

        public Dictionary(int size) : base(size) {

        }

        public int Count() {
            return base.Count;
        }

        public System.Collections.IEnumerator GetEnumerator() {
            return base.Values.GetEnumerator();
        }

        public object Item(object Index) {
            return base[Index];
        }

        public object[] Keys {
            get { 
                var ret = new object[base.Count];
                base.Keys.CopyTo(ret, 0);
                return ret;
            }
        }

        public object[] Values {
            get {
                var ret = new object[base.Count];
                base.Values.CopyTo(ret, 0);
                return ret;
            }
        }

    }
}
