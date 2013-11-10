using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kendo.UI
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]
    public class Command
    {
        public String Name;
        public String Text;
        public String ClassName;
        public Action Click;
    }
}
