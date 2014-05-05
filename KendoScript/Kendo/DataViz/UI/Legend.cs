using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kendo.DataViz.UI
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]
    public class Legend
    {
        public String Background;
        public Line Border;
        public ItemArea InactiveItems;
        public BaseLabel Labels;
        public Thickness Margin;
        public Number OffsetX;
        public Number OffsetY;
        public String Position;
        public bool Visible;
    }
}
