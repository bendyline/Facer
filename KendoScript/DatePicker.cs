﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using BL.UI;
using jQueryApi;
using Kendo.UI;
using System.Runtime.CompilerServices;

namespace BL.UI.KendoControls
{

    public class DatePicker : Control
    {
        public event EventHandler Changed;

        private Date pendingValue;
        private Kendo.UI.DatePicker datePicker;

        public override string TagName
        {
            get
            {
                return "INPUT";
            }
        }

        [ScriptName("d_value")]
        public Date Value
        {
            get
            {
                if (this.datePicker == null)
                {
                    return this.pendingValue;
                }

                return this.datePicker.Value();
            }

            set
            {
                if (this.datePicker == null)
                {
                    pendingValue = value;
                    return;
                }

                this.datePicker.Value(value);
            }
        }

        [ScriptName("d_min")]
        public Date Min
        {
            get
            {
                return this.datePicker.Min();
            }

            set
            {
                this.datePicker.Min(value);
            }
        }

        [ScriptName("d_max")]
        public Date Max
        {
            get
            {
                return this.datePicker.Max();
            }

            set
            {
                this.datePicker.Max(value);
            }
        }

        public DatePicker()
        {
            KendoUtilities.EnsureKendoBaseUx(this);
            KendoUtilities.EnsureKendoEditable(this);
        }

        protected override void OnApplyTemplate()
        {
            Script.Literal("var j = {0}; j.kendoDatePicker(); {1} = j.data('kendoDatePicker')", this.J, datePicker);

            if (this.pendingValue != null)
            {
                this.datePicker.Value(this.pendingValue);
            }

            datePicker.Bind("change", this.HandleDateChange);
        }

        private void HandleDateChange(object e)
        {
            if (this.Changed != null)
            {
                this.Changed(null, EventArgs.Empty);
            }
        }

        public override void Dispose()
        {
            if (this.datePicker != null)
            {
                this.datePicker.Destroy();
            }

            base.Dispose();
        }
    }
}
