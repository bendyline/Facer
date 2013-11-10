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

    public class DropDownList : Control
    {
        public event EventHandler Changed;

        private Kendo.UI.DropDownList dropDownList;
        private Kendo.UI.DropDownListOptions dropDownListOptions;

        private bool isInitialized = false;
        private bool delayLoad = false;

        public bool DelayLoad
        {
            get
            {
                return this.delayLoad;
            }

            set
            {
                this.delayLoad = value;
            }
        }

        public override string TagName
        {
            get
            {
                return "INPUT";
            }
        }

        [ScriptName("o_value")]
        public object Value
        {
            get
            {
                return this.dropDownList.Value();
            }

            set
            {
                this.dropDownList.Value(value);
            }
        }

        public String DataTextField
        {
            get
            {
                return this.dropDownListOptions.DataTextField;
            }

            set
            {
                this.dropDownListOptions.DataTextField = value;
            }
        }

        public String DataValueField
        {
            get
            {
                return this.dropDownListOptions.DataValueField;
            }

            set
            {
                this.dropDownListOptions.DataValueField = value;
            }
        }

        public object Data
        {
            get
            {
                return this.dropDownListOptions.DataSource;
            }
            set
            {
                object ds = value;

                if (this.dropDownList != null)
                {
                    DataSourceOptions dso = new DataSourceOptions();
                    dso.Data = value;
                    ds = new DataSource(dso);
                    dropDownList.SetDataSource((DataSource)ds);
                }

                this.dropDownListOptions.DataSource = ds;

                this.InitControl();
            }
        }

        public DataSource DataSource
        {
            get
            {
                return (DataSource)this.dropDownListOptions.DataSource;
            }
            set
            {
                if (this.dropDownList != null)
                {
                    dropDownList.SetDataSource(value);
                }

                this.dropDownListOptions.DataSource = value;
            }
        }


        public DropDownList()
        {
            this.dropDownListOptions = new DropDownListOptions();
        }

        protected override void OnEnsureElements()
        {
            if (this.DelayLoad)
            {
                Window.SetTimeout(this.InitControl, 1);
            }
            else
            {
                this.InitControl();
            }
        }

        public void InitControl()
        {
            if (!this.isInitialized && this.DataSource != null && this.ElementsEnsured)
            {
                Script.Literal("var j = {0}; j.kendoDropDownList({2}); {1} = j.data('kendoDropDownList')", this.J, this.dropDownList, this.dropDownListOptions);

                this.dropDownList.Bind("change", this.HandleDataChange);

                this.isInitialized = true;
            }
        }

        private void HandleDataChange(object e)
        {
            if (this.Changed != null)
            {
                this.Changed(null, EventArgs.Empty);
            }
        }

        public override void Dispose()
        {
           this.dropDownList.Destroy();
            
            base.Dispose();
        }
    }
}
