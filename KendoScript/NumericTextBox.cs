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

    public class NumericTextBox : Control
    {
        public event EventHandler Changed;

        private NumericTextBoxOptions options;
        private Kendo.UI.NumericTextBox numericTextBox;
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

        [ScriptName("i_decimals")]
        public int Decimals
        {
            get
            {
                if (this.numericTextBox != null)
                {
                    return this.numericTextBox.Decimals();
                }

                return this.options.Decimals;
            }

            set
            {
                if (this.numericTextBox != null)
                {
                    this.numericTextBox.Decimals(value);
                }

                this.options.Decimals = value;
            }
        }

        [ScriptName("d_value")]
        public double Value
        {
            get
            {
                if (this.numericTextBox != null)
                {
                    return this.numericTextBox.Value();
                }

                return this.options.Value;
            }

            set
            {
                if (this.numericTextBox != null)
                {
                    this.numericTextBox.Value(value);
                }

                this.options.Value = value;
            }
        }

        [ScriptName("d_min")]
        public double Min
        {
            get
            {
                if (this.numericTextBox != null)
                {
                    return this.numericTextBox.Min();
                }

                return this.options.Min;
            }

            set
            {
                if (this.numericTextBox != null)
                {
                    this.numericTextBox.Min(value);
                }

                this.options.Min = value;
            }
        }

        [ScriptName("d_max")]
        public double Max
        {
            get
            {
                if (this.numericTextBox != null)
                {
                    return this.numericTextBox.Max();
                }

                return this.options.Max;
            }

            set
            {
                if (this.numericTextBox != null)
                {
                    this.numericTextBox.Max(value);
                }

                this.options.Max = value;
            }
        }

        public bool Spinners
        {
            get
            {
                return this.options.Spinners;
            }

            set
            {
                this.options.Spinners = value;
            }
        }
        public String Format
        {
            get
            {
                return this.options.Format;
            }

            set
            {
                this.options.Format = value;
            }
        }

        [ScriptName("d_step")]
        public double Step
        {
            get
            {
                if (this.numericTextBox != null)
                {
                    return this.numericTextBox.Step();
                }

                return this.options.Step;
            }

            set
            {
                if (this.numericTextBox != null)
                {
                    this.numericTextBox.Step(value);
                }

                this.options.Step = value;
            }
        }

        public NumericTextBox()
        {
            KendoUtilities.EnsureKendoBaseUx(this);

            this.EnsureScript("kendo.ui.NumericTextBox", "js/kendo/kendo.numerictextbox.min.js");

            this.options = new NumericTextBoxOptions();
        }

        private void OnApplyTemplate()
        {
            Script.Literal("var j = {0}; j.kendoNumericTextBox({2}); {1} = j.data('kendoNumericTextBox')", this.J, this.numericTextBox, this.options);

            numericTextBox.Bind("change", this.HandleDataChange);
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
            if (this.numericTextBox != null)
            {
                this.numericTextBox.Destroy();
            }

            base.Dispose();
        }
    }
}
