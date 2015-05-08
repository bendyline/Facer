using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kendo.UI
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]
    public class MaskedTextBoxOptions
    {
        [ScriptName("ARIATemplate")]
        public String AriaTemplate;

        public String Culture;
        public String Mask;
        public String Value;
    }
}
