using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using jQueryApi;
using Kendo.UI;
using kendo.data;

namespace Kendo.DataViz.UI
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]    
    public class Chart : Widget
    {
        public ChartOptions Options;
        public jQueryObject Element;
        public jQueryObject Wrapper;

        public DataSource DataSource;

        public void SetDataSource(DataSource dataSource)
        {
            ;
        }

        public void SetOptions(ChartOptions options)
        {
            ;
        }

        public void Refresh()
        {
            ;
        }

        public void Redraw()
        {
            ;
        }

        public object Svg()
        {
            return null;
        }

        [ScriptName("ImageDataURL")]
        public String GetImageDataUrl()
        {
            return null;
        }
    }
}
