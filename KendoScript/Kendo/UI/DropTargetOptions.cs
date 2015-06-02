using jQueryApi;
using System;
using System.Collections.Generic;
using System.Html;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kendo.UI
{
    public delegate void DropEventHandler(DropEventArgs e);

    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]
    public class DropTargetOptions
    {
        public String Group;

        [ScriptName("dragenter")]
        public DropEventHandler DragEnter;

        [ScriptName("dragleave")]
        public DropEventHandler DragLeave;

        [ScriptName("drop")]
        public DropEventHandler Drop;

    }
}
