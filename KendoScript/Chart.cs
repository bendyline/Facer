using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using BL.UI;
using jQueryApi;
using Kendo.UI;
using Kendo.DataViz.UI;
using System.Runtime.CompilerServices;

namespace BL.UI.KendoControls
{

    public class Chart : Control
    {
        public event EventHandler Changed;
        private Kendo.DataViz.UI.Chart chart;
        private ChartOptions chartOptions;

        public ChartOptions Options
        {
            get
            {
                return this.chartOptions;
            }

            set
            {
                if (this.chart != null)
                {
                    this.chart.SetOptions(value);
                }

                this.chartOptions = value;
            }
        }

        public Chart()
        {
            KendoUtilities.EnsureKendoBaseUx(this);
            KendoUtilities.EnsureKendoData(this);
            KendoUtilities.EnsureKendoDataViz(this);

            this.EnsureScript("kendo.dataviz.ui.Chart", "js/kendo/kendo.dataviz.chart.min.js");
        }

        protected override void OnApplyTemplate()
        {
            Script.Literal("var j = {0}; j.kendoChart({2}); {1} = j.data('kendoChart')", this.J, chart, this.chartOptions);

            chart.Bind("change", this.HandleDateChange);
        }

        public void Blur()
        {
            // done to get rid of random tooltips that stick around if you move the chart offscreen.
            // a better solution might be to use togglehighlight, but that implies tracking selection, etc.
            chart.Refresh();
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
            if (this.chart != null)
            {
                this.chart.Destroy();
            }

            base.Dispose();
        }
    }
}
