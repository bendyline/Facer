using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kendo.UI
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]
    public class DateTimePickerOptions
    {
        [ScriptName("ARIATemplate")]
        public String AriaTemplate;

        public String Culture;
        public Date[] Dates;
        public String Depth;
        public String Footer;
        public String Format;
        public Date Max;
        public Date Min;
        public object Month;
        public String[] ParseFormats;
        public String Start;
        public Date Value;
    }
}
