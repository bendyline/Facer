using jQueryApi;
using System;
using System.Collections.Generic;
using System.Html;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kendo.UI
{
    [Imported]
    public delegate Element ElementCloneFactory(Element[] e);

    [Imported]
    public delegate void jQueryEventHandler(jQueryEvent eventData);


    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]
    public class DraggableOptions
    {
        public String Axis;
        public jQueryObject Container;
        public object CursorOffset;
        public ElementCloneFactory Hint;

        public jQueryEventHandler Drag;

        [ScriptName("dragstart")]
        public jQueryEventHandler DragStart;

        [ScriptName("dragend")]
        public jQueryEventHandler DragEnd;

        [ScriptName("dragcancel")]
        public jQueryEventHandler DragCancel;

        public int Distance;
        public String Filter;
        public String Group;
        public object Function;
        public bool HoldToDrag;
        public String Ignore;
    }
}
