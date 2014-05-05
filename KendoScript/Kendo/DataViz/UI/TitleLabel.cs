using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kendo.DataViz.UI
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]
    public class TitleLabel : BaseLabel 
    {
        public Thickness Margin;
        public String Position;
        public Number Rotation;
        public String Text;
        public bool Visible;
    }
}
