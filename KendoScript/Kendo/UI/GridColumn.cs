using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kendo.UI
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]
    public class GridColumn
    {
        public String Title;
        public String Name;
        public String Field;
        public bool Encoded;
        public bool Filterable;

        public Action Editor;
        public String FooterTemplate;
        public String Format;
        public String GroupHeaderTemplate;
        public String GroupFooterTemplate;
        public object HeaderAttributes;
        public object HeaderTemplate;
        public object Hidden;
        public object Command;
        public bool Sortable;
        public String Template;
        public double Width;
        public List<DataValue> Values;

    }
}
