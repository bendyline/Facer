using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using jQueryApi;
using Kendo.UI;

namespace kendo.mobile
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]
    public class MobileSwitch : Widget
    {
        public bool Check(params bool[] newCheckValue)
        {
            return false;
        }

        public bool Enable(params bool[] newEnableValue)
        {
            return false;
        }
    }
}
