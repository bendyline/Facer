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

    public class MaskedTextBox : Control
    {
        public event EventHandler Changed;

        private String pendingValue;
        private Kendo.UI.MaskedTextBox maskedTextbox;
        private MaskedTextBoxOptions options;

        public override string TagName
        {
            get
            {
                return "INPUT";
            }
        }

        [ScriptName("d_value")]
        public String Value
        {
            get
            {
                if (this.maskedTextbox == null)
                {
                    return this.pendingValue;
                }

                return this.maskedTextbox.Value();
            }

            set
            {
                if (this.maskedTextbox == null)
                {
                    pendingValue = value;
                    return;
                }

                this.maskedTextbox.Value(value);
            }
        }

        [ScriptName("s_mask")]
        public String Mask
        {
            get
            {
                return this.options.Mask;
            }

            set
            {
                this.options.Mask = value;
            }
        }


        public MaskedTextBox()
        {
            this.EnsurePrerequisite("kendo.ui.MaskedTextBox", "js/kendo/kendo.maskedtextbox.min.js");

            this.options = new MaskedTextBoxOptions();
        }

        private void OnApplyTemplate()
        {
            Script.Literal("var j = {0}; j.kendoMaskedTextBox({2}); {1} = j.data('kendoMaskedTextBox')", this.J, this.maskedTextbox, this.options);

            maskedTextbox.Bind("change", this.HandleDataChange);
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
            if (this.maskedTextbox != null)
            {
                this.maskedTextbox.Destroy();
            }

            base.Dispose();
        }
    }
}
