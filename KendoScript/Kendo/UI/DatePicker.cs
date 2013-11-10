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
    public class DatePicker : Widget
    {
        public DatePickerOptions Options;
        public jQueryObject Element;
        public jQueryObject Wrapper;
        public object DateView;

        public void Close()
        {

        }
    
        public void Open()
        {

        }

        public Date Max(params Date[] dateTime)
        {
            return new Date(1900,1,1);
        }

        public Date Min(params Date[] dateTime)
        {
            return new Date(1900, 1, 1);
        }

        public Date Value(params Date[] dateTime)
        {
            return new Date(1900, 1, 1);
        }
    }
}
