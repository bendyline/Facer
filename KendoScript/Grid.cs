using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using BL.UI;
using jQueryApi;
using Kendo.UI;
using System.Runtime.CompilerServices;
using kendo.data;
using kendo.ui;

namespace BL.UI.KendoControls
{

    public class Grid : Control
    {
        private Kendo.UI.Grid grid;
        private Kendo.UI.GridOptions gridOptions;

        public event ModelEventHandler Save;
        public event ModelEventHandler Edit;
        public event ModelEventHandler Cancel;
        public event SelectionEventHandler Change;
        public event ModelEventHandler Remove;

        
        public ObservableObject DataItem
        {
            get
            {
                return this.grid.DataItem(null);
            }
        }


        public GridOptions Options
        {
            get
            {
                if (this.gridOptions == null)
                {
                    this.gridOptions = new GridOptions();
                }

                return this.gridOptions;
            }

            set
            {
                this.gridOptions = value;

                if (this.TemplateWasApplied)
                {
                    this.UpdateGrid();
                }
            }
        }

        public override int? Height
        {
            get
            {
                return (int?)this.Options.Height;
            }

            set
            {
                this.Options.Height = value;

                if (this.TemplateWasApplied)
                {
                    this.Element.Style.Height = value + "px";
                    this.Element.Style.MinHeight = value + "px";

                    if (this.Element.ChildNodes.Length >= 2)
                    {
                        Element contentContainer = this.Element.ChildNodes[1];

                        contentContainer.Style.Height = (value -35) + "px";
                    }
                }
            }
        }

        public object Data
        {
            get
            {
                return this.gridOptions.DataSource;
            }
            set
            {
                if (this.grid != null)
                {
                    DataSourceOptions dso = new DataSourceOptions();
                    dso.Data = value;
                    DataSource ds = new kendo.data.DataSource(dso);
                    grid.SetDataSource(ds);
                }

                this.gridOptions.DataSource = value;
            }
        }


        public DataSource DataSource
        {
            get
            {
                return (DataSource)this.gridOptions.DataSource;
            }
            set
            {
                if (this.grid != null)
                {
                    grid.SetDataSource(value);
                }

                this.gridOptions.DataSource = value;
            }
        }

        public Grid()
        {
            KendoUtilities.EnsureKendoBaseUx(this);
            KendoUtilities.EnsureKendoData(this);

            this.EnsureScript("kendo.ui.ColumnSorter", "js/kendo/kendo.columnsorter.min.js");

            KendoUtilities.EnsureKendoEditable(this);
            this.EnsureScript("kendo.observable", "js/kendo/kendo.binder.min.js");

            this.EnsureScript("kendo.ui.Window", "js/kendo/kendo.window.min.js");

            this.EnsureScript("kendo.mobile.ui.Scroller", "js/kendo/kendo.mobile.scroller.min.js");
            this.EnsureScript("kendo.ui.List", "js/kendo/kendo.list.min.js");
            this.EnsureScript("kendo.View", "js/kendo/kendo.view.min.js");
            this.EnsureScript("kendo.ui.DropDownList", "js/kendo/kendo.dropdownlist.min.js");
            this.EnsureScript("kendo.mobile.ui.Scroller", "js/kendo/kendo.mobile.scroller.min.js");
            this.EnsureScript("kendo.drawing.Surface", "js/kendo/kendo.drawing.min.js");

            this.EnsureScript("kendo.ui.Selectable", "js/kendo/kendo.selectable.min.js");
            this.EnsureScript("kendo.ui.Menu", "js/kendo/kendo.menu.min.js");
            this.EnsureScript("kendo.ui.FilterMenu", "js/kendo/kendo.filtermenu.min.js");
            this.EnsureScript("kendo.ui.ColumnMenu", "js/kendo/kendo.columnmenu.min.js");
            this.EnsureScript("kendo.ui.Groupable", "js/kendo/kendo.groupable.min.js");
            this.EnsureScript("kendo.ui.Pager", "js/kendo/kendo.pager.min.js");
            this.EnsureScript("kendo.ui.Sortable", "js/kendo/kendo.sortable.min.js");
            this.EnsureScript("kendo.ui.Reorderable", "js/kendo/kendo.reorderable.min.js");
            this.EnsureScript("kendo.ui.Resizable", "js/kendo/kendo.resizable.min.js");

            this.EnsureScript("kendo.mobile.ui.Loader", "js/kendo/kendo.mobile.loader.min.js");
            this.EnsureScript("kendo.mobile.ui.View", "js/kendo/kendo.mobile.view.min.js");
            this.EnsureScript("kendo.mobile.ui.Pane", "js/kendo/kendo.mobile.pane.min.js");
            this.EnsureScript("kendo.mobile.ui.Shim", "js/kendo/kendo.mobile.shim.min.js");
            this.EnsureScript("kendo.mobile.ui.PopOver", "js/kendo/kendo.mobile.popover.min.js");
            this.EnsureScript("kendo.mobile.ui.ActionSheet", "js/kendo/kendo.mobile.actionsheet.min.js");

            this.EnsureScript("kendo.ooxml.Worksheet", "js/kendo/kendo.ooxml.min.js");
            this.EnsureScript("kendo.pdf.Document", "js/kendo/kendo.pdf.min.js");
            this.EnsureScript("kendo.ExcelMixin", "js/kendo/kendo.excel.min.js");

            this.EnsureScript("kendo.ui.Grid", "js/kendo/kendo.grid.min.js");

        }

        public Element GetRowById(String id)
        {
            Element tbody = this.grid.Tbody.GetElement(0);

            for (int i=0; i<tbody.ChildNodes.Length; i++)
            {
                Element row = tbody.ChildNodes[i];

                ObservableObject model = this.grid.DataItem(row);

                if (model != null)
                {
                    String idCandidate = null;

                    Script.Literal("{0}={1}[\"id\"]", idCandidate, model);

                    if (id == idCandidate)
                    {
                        return row;
                    }
                }
            }

            return null;
        }

        public Element GetRow(int index)
        {
            //                          content node  .table.     .tbody     .tr
            return this.grid.Tbody.GetElement(0).ChildNodes[index];
        }

        public void AddRow()
        {
            this.grid.AddRow();
            
        }

        protected override void OnApplyTemplate()
        { 
            if (this.gridOptions != null)
            {
                this.UpdateGrid();
            }
        }

        private void UpdateGrid()
        {
            Script.Literal("var j = {0}; j.kendoGrid({2}); {1} = j.data('kendoGrid')", this.J, this.grid, this.gridOptions);

            this.grid.Bind("save", this.HandleSave);
            this.grid.Bind("change", this.HandleChange);
            this.grid.Bind("edit", this.HandleEdit);
            this.grid.Bind("cancel", this.HandleCancel);
            this.grid.Bind("remove", this.HandleRemove);
        }

        private void HandleEdit(object e)
        {
            Model model = null;

            Script.Literal("{0}={1}.model", model, e);

            if (this.Edit != null)
            {
                ModelEventArgs mea = new ModelEventArgs();
                mea.Model = model;

                this.Edit(mea);
            }
        }

        private void HandleCancel(object e)
        {
            Model model = null;

            Script.Literal("{0}={1}.model", model, e);

            if (this.Cancel != null)
            {
                ModelEventArgs mea = new ModelEventArgs();

                mea.Model = model;

                this.Cancel(mea);
            }
        }

        private void HandleChange(object e)
        {   
            if (Window.Event != null && Window.Event.Target != null)
            {
                if (ElementUtilities.IsDefaultInputElement(Window.Event, false))
                {
                    return;
                }
            }

            Script.Literal("{0}={0}.sender", e);

            if (this.Change != null)
            {
                SelectionEventArgs sea = (SelectionEventArgs)e;

                this.Change(sea);
            }
        }

        private void HandleRemove(object e)
        {
            Model model = null;

            Script.Literal("{0}={1}.model", model, e);

            if (this.Remove != null)
            {
                ModelEventArgs mea = new ModelEventArgs();
                mea.Model = model;

                this.Remove(mea);
            }
        }

        public void SaveAsExcel()
        {
            ControlManager.Current.LoadScript("JSZip", UrlUtilities.EnsurePathEndsWithSlash(Context.Current.ResourceBasePath) + "js/jszip.min.js", this.ContinueSaveAsExcel, null);
        }

        private void ContinueSaveAsExcel(IAsyncResult result)
        {
            this.grid.SaveAsExcel();
        }

        public void SaveAsPdf()
        {
            ControlManager.Current.LoadScript("kendo.ui.ProgressBar", UrlUtilities.EnsurePathEndsWithSlash(Context.Current.ResourceBasePath) + "js/kendo/kendo.progressbar.min.js", this.ContinueSaveAsPdf, null);
        }

        private void ContinueSaveAsPdf(IAsyncResult result)
        {
            this.grid.SaveAsPdf();
        }

        public void CancelRow()
        {
            this.grid.CancelRow();
        }

        public void SaveChanges()
        {
            this.grid.SaveChanges();
        }
        public void ClearSelection()
        {
            this.grid.ClearSelection();
        }

        public object Select(object newSelection)
        {
            return this.grid.Select(newSelection);
        }

        public void SaveRow()
        {
            this.grid.SaveRow();
        }

        public void RemoveRow(jQueryObject jQueryItemToBeRemoved)
        {
            this.grid.RemoveRow(jQueryItemToBeRemoved);
        }

        public void EditRow(jQueryObject row)
        {
            this.grid.EditRow(row);
        }

        private void HandleSave(object e)
        {
            Model model = null;

            Script.Literal("{0}={1}.model", model, e);

            if (this.Save != null)
            {
                ModelEventArgs mea = new ModelEventArgs();
                mea.Model = model;

                this.Save(mea);
            }
        }

        public override void Dispose()
        {
            if (this.grid != null)
            {
                this.grid.Destroy();
            }

            base.Dispose();
        }
    }
}
