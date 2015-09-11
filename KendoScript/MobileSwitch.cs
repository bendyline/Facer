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
    public class MobileSwitch : Control
    {
        public event EventHandler Changed;

        private MobileSwitchOptions options;
        private kendo.mobile.MobileSwitch mobileSwitch;
        private bool? isChecked;

        [ScriptName("b_checked")]
        public bool Checked
        {
            get
            {
                if (this.mobileSwitch == null)
                {
                    return (bool)this.isChecked;
                }

                return this.mobileSwitch.Check();
            }

            set
            {
                this.isChecked = value;

                if (this.mobileSwitch != null)
                {
                    this.mobileSwitch.Check(value);

                }
            }
        }

        [ScriptName("b_enabled")]
        public bool Enabled
        {
            get
            {
                return this.mobileSwitch.Enable();
            }

            set
            {
                this.mobileSwitch.Enable(value);
            }
        }


        [ScriptName("s_offLabel")]
        public String OffLabel
        {
            get
            {
                return this.options.OffLabel;
            }

            set
            {
                this.options.OffLabel = value;
            }
        }

        [ScriptName("s_onLabel")]
        public String OnLabel
        {
            get
            {
                return this.options.OnLabel;
            }

            set
            {
                this.options.OnLabel = value;
            }
        }


        public MobileSwitch()
        {
            KendoUtilities.EnsureKendoBaseUx(this);

            this.EnsureScript("kendo.mobile.Application", "js/kendo/kendo.mobile.application.min.js");
            this.EnsureScript("kendo.mobile.ui.Switch", "js/kendo/kendo.mobile.switch.min.js");

            this.options = new MobileSwitchOptions();
        }

        protected override void OnApplyTemplate()
        {
            KendoUtilities.EnsureMobileApplication();

            // this is a bit of a hack, but if we don't wait, the switch doesn't "slide".
            Window.SetTimeout(this.ContinueAddSwitch, 10);
        }

        private void ContinueAddSwitch()
        {
            Script.Literal("var j = {0}; j.kendoMobileSwitch({2}); {1} = j.data('kendoMobileSwitch')", this.J, this.mobileSwitch, this.options);

            mobileSwitch.Bind("change", this.HandleDataChange);

            if (this.isChecked != null)
            {
                this.mobileSwitch.Check((bool)this.isChecked);
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
            if (this.mobileSwitch != null)
            {
                this.mobileSwitch.Destroy();
            }

            base.Dispose();
        }
    }
}
