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
    public class TabStrip : Control
    {
        public event EventHandler TabChanged;

        private Kendo.UI.TabStrip tabStrip;

        public void Append(Tab tab)
        {
            this.tabStrip.Append(new Tab[] { tab });
        }

        protected override void OnEnsureElements()
        {
            Script.Literal("var j = {0}; j.kendoTabStrip(); {1} = j.data('kendoTabStrip')", this.J, this.tabStrip);

            this.tabStrip.Bind("activate", this.HandleTabChange);
        }

        private void HandleTabChange(object e)
        {
            if (this.TabChanged != null)
            {
                this.TabChanged(null, EventArgs.Empty);
            }
        }

        public override void Dispose()
        {
           this.tabStrip.Destroy();
            
            base.Dispose();
        }
    }
}
