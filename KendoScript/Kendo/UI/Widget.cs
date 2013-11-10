using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kendo.UI
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]    
    public class Widget
    {

        [ScriptName("readonly")]
        public bool ReadOnly(params bool[] readOnly)
        {
            return false;
        }

        public void Destroy()
        {
            ;
        }

        public void Bind(String eventName, KendoEventHandler eventHandler)
        {

        }
    }
}
