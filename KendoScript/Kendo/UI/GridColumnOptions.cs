using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kendo.UI
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]
    public class GridColumnOptions
    {
     //   public List<GridColumn> Columns;
        public List<String> Aggregates;
        public String GroupFooterTemplate;
        public Dictionary<String, String> Attributes;
        public String Command;

    }
}
