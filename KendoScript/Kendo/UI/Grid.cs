using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using jQueryApi;
using System.Html;
using kendo.data;

namespace Kendo.UI
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]    
    public class Grid: Widget
    {
        public GridOptions Options;
        public jQueryObject Table;
        public jQueryObject Tbody;
        public jQueryObject Thead;

        public List<GridColumn> Columns;
        public DataSource DataSource;

        public ObservableObject DataItem()
        {
            return null;
        }

        public void AddRow()
        {

        }

        public void CancelChanges()
        {
        }


        public void CancelRow()
        {

        }

        public int CellIndex(object cell)
        {
            return -1;
        }

        public void ClearSelection()
        {

        }
        public void CloseCell()
        {

        }

        public void CollapseGroup(object row)
        {

        }

        public void CollapseRow(object row)
        {

        }

        public ObservableObject DataItem(object selector)
        {
            return null;
        }

        public void EditCell(jQueryObject cell)
        {

        }
        public void EditRow(jQueryObject row)
        {

        }

        public void ExpandGroup(object row)
        {

        }

        public void ExpandRow(object row)
        {

        }

        public void HideColumn(int column)
        {

        }

        public void Refresh()
        {
        }


        public void RemoveRow(object row)
        {

        }

        public void ReorderColumn(int destIndex, GridColumn column)
        {

        }

        public void SaveChanges()
        {

        }
        public void SaveRow()
        {

        }
        public void Select(object rows)
        {

        }

        public void SetDataSource(DataSource dataSource)
        {

        }

        public void ShowColumn(int index)
        {

        }


    }
}
