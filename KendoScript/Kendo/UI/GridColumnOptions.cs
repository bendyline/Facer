using jQueryApi;
using System;
using System.Collections.Generic;
using System.Html;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kendo.UI
{
    [Imported]
    [IgnoreNamespace]
    public delegate void GridColumnUserInterfaceFactory(jQueryObject container, GridColumnUserInterfaceFactoryOptions options);

    [Imported]
    [IgnoreNamespace]
    public delegate String ItemTemplateFactory(object item);

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
        public GridColumnUserInterfaceFactory Editor;
    }
}
