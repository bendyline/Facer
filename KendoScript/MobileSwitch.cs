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

        public override string TagName
        {
            get
            {
                return "INPUT";
            }
        }

        public bool Checked
        {
            get
            {
                return this.mobileSwitch.Check();
            }

            set
            {
                this.mobileSwitch.Check(value);
            }
        }

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
            this.options = new MobileSwitchOptions();
        }

        protected override void OnEnsureElements()
        {
            this.Complete();
        }

        private void Complete()
        {
            Script.Literal("var j = {0}; j.kendoMobileSwitch({2}); {1} = j.data('kendoMobileSwitch')", this.J, this.mobileSwitch, this.options);

            mobileSwitch.Bind("change", this.HandleDataChange);
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
           this.mobileSwitch.Destroy();
            
            base.Dispose();
        }
    }
}
