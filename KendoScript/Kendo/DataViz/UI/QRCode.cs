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
    public class QRCode : Widget
    {
        public QRCodeOptions Options;
        public jQueryObject Element;
        public jQueryObject Wrapper;

        public void SetOptions(QRCodeOptions options)
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

        public String Value(String value)
        {
            return null;
        }
    }
}
