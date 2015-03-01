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

                this.ApplyVisibility();
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

                this.ApplyVisibility();
            }
        }

        public double DropdownHeight
        {
            get
            {
                return this.dropDownListOptions.Height;
            }

            set
            {
                this.dropDownListOptions.Height = value;
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

                this.ApplyVisibility();
            }
        }

        public String HeaderTemplate
        {
            get
            {
                return this.dropDownListOptions.HeaderTemplate;
            }

            set
            {
                this.dropDownListOptions.HeaderTemplate = value;

                this.ApplyVisibility();
            }
        }

        public String Template
        {
            get
            {
                return this.dropDownListOptions.Template;
            }

            set
            {
                this.dropDownListOptions.Template = value;

                this.ApplyVisibility();
            }
        }
        public String ValueTemplate
        {
            get
            {
                return this.dropDownListOptions.ValueTemplate;
            }

            set
            {
                this.dropDownListOptions.ValueTemplate = value;

                this.ApplyVisibility();
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

                this.ApplyVisibility();
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

        private void InitControl()
        {
            if (!this.isInitialized && this.DataSource != null && this.ElementsEnsured)
            {
                Element e = Document.CreateElement("DIV");

                e.Style.Width = "100%";

                jQueryObject jqueryObject = jQuery.FromObject(e);

                this.Element.AppendChild(e);

                Script.Literal("var j = {0}; j.kendoDropDownList({2}); {1} = j.data('kendoDropDownList')", jqueryObject, this.dropDownList, this.dropDownListOptions);

                this.dropDownList.Bind("change", this.HandleDataChange);

                this.isInitialized = true;
            }

            this.ApplyVisibility();
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
