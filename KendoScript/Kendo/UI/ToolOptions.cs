using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kendo.UI
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]
    public class ToolOptions
    {

        public String Name;
        public String Tooltip;
        public Function Exec;
        public List<object> Items;
        public String Template;
    }
}
