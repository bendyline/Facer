using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using BL.UI;
using jQueryApi;
using Kendo.UI;
using System.Runtime.CompilerServices;

namespace BL.UI.KendoControls
{

    public class DateTimePicker : Control
    {
        public event EventHandler Changed;

        private Date pendingValue;
        private Kendo.UI.DateTimePicker dateTimePicker;

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
                if (this.dateTimePicker == null)
                {
                    return this.pendingValue;
                }

                return this.dateTimePicker.Value();
            }

            set
            {
                if (this.dateTimePicker == null)
                {
                    pendingValue = value;
                    return;
                }

                this.dateTimePicker.Value(value);
            }
        }

        [ScriptName("d_min")]
        public Date Min
        {
            get
            {
                return this.dateTimePicker.Min();
            }

            set
            {
                this.dateTimePicker.Min(value);
            }
        }

        [ScriptName("d_max")]
        public Date Max
        {
            get
            {
                return this.dateTimePicker.Max();
            }

            set
            {
                this.dateTimePicker.Max(value);
            }
        }

        public DateTimePicker()
        {
            KendoUtilities.EnsureKendoBaseUx(this);
            KendoUtilities.EnsureKendoEditable(this);

            this.EnsurePrerequisite("kendo.ui.TimePicker", "js/kendo/kendo.timepicker.min.js");
            this.EnsurePrerequisite("kendo.ui.DateTimePicker", "js/kendo/kendo.datetimepicker.min.js");
        }

        protected override void OnApplyTemplate()
        {
            Script.Literal("var j = {0}; j.kendoDateTimePicker(); {1} = j.data('kendoDateTimePicker')", this.J, dateTimePicker);

            if (this.pendingValue != null)
            {
                this.dateTimePicker.Value(this.pendingValue);
            }

            dateTimePicker.Bind("change", this.HandleDateChange);
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
            if (this.dateTimePicker != null)
            {
                this.dateTimePicker.Destroy();
            }

            base.Dispose();
        }
    }
}
