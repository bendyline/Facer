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
        private Nullable<int> dropdownWidth = null;
        private object pendingSelection = null;
        private object dropdownValue = null;

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
                if (this.dropDownList != null)
                {
                    return this.dropDownList.Value();
                }

                return dropdownValue;
            }

            set
            {
                if (this.dropDownList != null)
                {
                    this.dropDownList.Value(value);
                }

                if (this.dropDownListOptions != null)
                {
                    this.dropDownListOptions.Value = value;
                }
                
                this.dropdownValue = value;

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

        public String DropdownTemplate
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

                if (this.TemplateWasApplied)
                {
                    this.InitControl();
                }
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
            KendoUtilities.EnsureKendoBaseUx(this);
            KendoUtilities.EnsureKendoData(this);

            this.EnsureScript("kendo.mobile.ui.Scroller", "js/kendo/kendo.mobile.scroller.min.js");
            this.EnsureScript("kendo.ui.List", "js/kendo/kendo.list.min.js");
            this.EnsureScript("kendo.ui.DropDownList", "js/kendo/kendo.dropdownlist.min.js");

            this.dropDownListOptions = new DropDownListOptions();
        }

        protected override void OnApplyTemplate()
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

        public void Select(object selection)
        {
            this.pendingSelection = selection;

            if (this.dropDownList != null)
            {
                this.dropDownList.Select(selection);
            }
        }

        public void SetDropdownWidth(int width)
        {
            this.dropdownWidth = width;

            if (this.dropDownList != null)
            {
                this.dropDownList.List.Width((int)width);
            }
        }

        private void InitControl()
        {
            if (!this.isInitialized && this.DataSource != null)
            {
                ElementUtilities.ClearChildElements(this.Element);

                Element e = Document.CreateElement("DIV");

                e.Style.Width = "100%";

                jQueryObject jqueryObject = jQuery.FromObject(e);

                this.Element.AppendChild(e);

                Script.Literal("var j = {0}; j.kendoDropDownList({2}); {1} = j.data('kendoDropDownList')", jqueryObject, this.dropDownList, this.dropDownListOptions);

                if (this.dropdownValue != null)
                {
                    this.dropDownList.Value(this.dropdownValue);
                }

                if (this.pendingSelection != null)
                {
                    this.dropDownList.Select(this.pendingSelection);
                }

                this.dropDownList.Bind("change", this.HandleDataChange);

                if (this.dropdownWidth != null)
                {
                    this.dropDownList.List.Width((int)this.dropdownWidth);
                }

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
            if (this.dropDownList != null)
            {
                this.dropDownList.Destroy();
            }

            base.Dispose();
        }
    }
}
