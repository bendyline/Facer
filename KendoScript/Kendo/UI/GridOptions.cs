using kendo.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kendo.UI
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]
    public class GridOptions
    {
        public String AltRowTemplate;
        public bool AutoBind;
        public double ColumnResizeHandleWidth;
        public List<GridColumn> Columns;
        public object Editable;
        public GridColumnMenuOptions ColumnMenu;
        public object DataSource;
        public object DetailTemplate;
        public object Filterable;
        public GridGroupableOptions Groupable;
        public double Height;
        public bool Navigatable;
        public bool Scrollable;
        public object Pageable;
        public GridSortableOptions Sortable;
        public object Toolbar;
    }
}
