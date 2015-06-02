using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using jQueryApi;
using System.Html;
using kendo.data;

namespace Kendo.UI
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]    
    public class Editor : Widget
    {
        public EditorOptions Options;
        public Element Body;

        public String Value(params string[] newValue)
        {
            return null;
        }
    }
}
