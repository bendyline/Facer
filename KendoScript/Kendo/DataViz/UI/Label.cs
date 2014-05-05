using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kendo.DataViz.UI
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]
    public class Label : BaseLabel 
    {
        public String Culture;
        public DateFormats DateFormats;
        public Thickness Margin;
        public bool Mirror;
        public Number Rotation;
        public Number Skip;
        public Number Step;

    }
}
