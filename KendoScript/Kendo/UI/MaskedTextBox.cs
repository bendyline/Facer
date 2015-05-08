using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using jQueryApi;

namespace Kendo.UI
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]    
    public class MaskedTextBox : Widget
    {
        public MaskedTextBoxOptions Options;
        public jQueryObject Element;
        public jQueryObject Wrapper;
     

        public String Value(params String[] strs)
        {
            return null;
        }
    }
}
