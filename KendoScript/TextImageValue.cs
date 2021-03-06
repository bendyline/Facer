﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using BL.UI;
using jQueryApi;
using Kendo.UI;
using Kendo.DataViz.UI;
using System.Runtime.CompilerServices;

namespace BL.UI.KendoControls
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]    
    public class TextImageItems
    {
        public String Text;
        public String ImageUrl;
        public object Value;
        public IEnumerable<TextImageItems> Items;
    }
}
