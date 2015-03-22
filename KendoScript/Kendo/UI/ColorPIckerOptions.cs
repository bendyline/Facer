using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kendo.UI
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]
    public class ColorPickerOptions
    {
        [ScriptName("ARIATemplate")]
        public String AriaTemplate;

        public bool Buttons;
        public int Columns;
        public Size TileSize;
        public bool Opacity;
        public object Palette;
        public bool Preview;

        public String Value;
    }
}
