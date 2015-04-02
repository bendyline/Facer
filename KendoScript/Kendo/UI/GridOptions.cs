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
        public ExportFileOptions Excel;
        public object DetailTemplate;
        public object Filterable;
        public GridGroupableOptions Groupable;
        public object Height;
        public bool Navigatable;
        public bool Scrollable;
        public ExportFileOptions Pdf;
        public object Pageable;
        public object Selectable;
        public object Resizable;
        public GridSortableOptions Sortable;
        public object Toolbar;
    }
}
