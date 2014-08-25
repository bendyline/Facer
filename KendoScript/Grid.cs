using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using BL.UI;
using jQueryApi;
using Kendo.UI;
using System.Runtime.CompilerServices;
using kendo.data;

namespace BL.UI.KendoControls
{

    public class Grid : Control
    {
        private Kendo.UI.Grid grid;
        private Kendo.UI.GridOptions gridOptions;

        public event ModelEventHandler Save;
        public event ModelEventHandler Edit;
        public event ModelEventHandler Remove;

        
        public ObservableObject DataItem
        {
            get
            {
                return this.grid.DataItem();
            }
        }


        public GridOptions Options
        {
            get
            {
                return this.gridOptions;
            }

            set
            {
                this.gridOptions = value;

                this.UpdateGrid();
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

        }

        public void AddRow()
        {
            this.grid.AddRow();
            
        }

        

        protected override void OnEnsureElements()
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
            this.grid.Bind("edit", this.HandleEdit);
            this.grid.Bind("remove", this.HandleRemove);
        }

        private void HandleEdit(object e)
        {
            Model model = null;

            Script.Literal("{0}=e.model", model);

            if (this.Edit != null)
            {
                ModelEventArgs mea = new ModelEventArgs();
                mea.Model = model;

                this.Edit(mea);
            }
        }

        private void HandleRemove(object e)
        {
            Model model = null;

            Script.Literal("{0}=e.model", model);

            if (this.Remove != null)
            {
                ModelEventArgs mea = new ModelEventArgs();
                mea.Model = model;

                this.Remove(mea);
            }
        }

        private void HandleSave(object e)
        {
            Model model = null;

            Script.Literal("{0}=e.model", model);

            if (this.Save != null)
            {
                ModelEventArgs mea = new ModelEventArgs();
                mea.Model = model;

                this.Save(mea);
            }
        }

        public override void Dispose()
        {
           this.grid.Destroy();
            
            base.Dispose();
        }
    }
}
