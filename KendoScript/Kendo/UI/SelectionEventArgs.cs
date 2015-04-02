using System;
using System.Collections.Generic;
using System.Html;
using System.Linq;
using System.Runtime.CompilerServices;

namespace kendo.ui
{
    [Imported]
    public delegate void SelectionEventHandler(SelectionEventArgs e);

    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]
    public class SelectionEventArgs 
    {
        [ScriptName("element")]
        public ICollection<Element> Elements;
    }
}
