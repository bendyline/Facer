using jQueryApi;
using System;
using System.Collections.Generic;
using System.Html;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kendo.UI
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]
    public class DropEventArgs
    {
        public Draggable Draggable;
        public jQueryObject DropTarget;
        public Element Target;
    }
}
