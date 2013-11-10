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
    public class NumericTextBox : Widget
    {
        public NumericTextBoxOptions Options;
        public jQueryObject Element;
        public jQueryObject Wrapper;

        public Int32 Decimals(params int[] decimals)
        {
            return -1;
        }
        
        public double Max(params double[] max)
        {
            return -1;
        }

        public double Min(params double[] min)
        {
            return -1;
        }

        public double Value(params double[] val)
        {
            return -1;
        }
        
        public double Step(params double[] step)
        {
            return -1;
        }
    }
}
