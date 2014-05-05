using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kendo.DataViz.UI
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]
    public class ChartOptions
    {
        public bool AutoBind;
        public object AxisDefaults;
        public Axis CategoryAxis;
        public Area ChartArea;
        public object DataSource;
        public Legend Legend;
        public Area PlotArea;
        public String RenderAs;
        public Series[] Series;
        public String[] SeriesColors;
        public SeriesDefaults SeriesDefaults;
        public String Theme;
        public TitleLabel Title;
        public Label Tooltip;
        public bool Transition;
        public Axis ValueAxis;
        public Axis xAxis;
        public Axis yAxis;
    }
}
