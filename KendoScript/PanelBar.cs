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
    public class PanelBar : Control
    {
        public event EventHandler TabChanged;

        private Kendo.UI.PanelBar panelBar;

        public void Append(PanelBarItem panelBarItem)
        {
            this.panelBar.Append(new PanelBarItem[] { panelBarItem }, null);
        }

        protected override void OnEnsureElements()
        {
            Script.Literal("var j = {0}; j.kendoPanelBar(); {1} = j.data('kendoPanelBar')", this.J, this.panelBar);
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
           this.panelBar.Destroy();
            
            base.Dispose();
        }
    }
}
