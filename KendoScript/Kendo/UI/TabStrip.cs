using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using jQueryApi;
using System.Html;

namespace Kendo.UI
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]    
    public class TabStrip : Widget
    {
        public TabStripOptions Options;
        public jQueryObject TabGroup;

        public TabStrip Append(Tab[] tab)
        {
            return null;
        }
        
        public Element ContentElement(int index)
        {
            return null;
        }
        public Element ContentHolder(int index)
        {
            return null;
        }

        public void DeactivateTab(jQueryObject tab)
        {
            ;
        }

        public TabStrip Disable(Element selector)
        {
            return null;
        }
        public TabStrip Enable(Element selector, bool enable)
        {
            return null;
        }

        public TabStrip InsertAfter(Tab[] tab, jQueryObject selector)
        {
            return null;
        }

        public TabStrip InsertBefore(Tab[] tab, jQueryObject selector)
        {
            return null;
        }

        public Element[] Items()
        {
            return null;
        }

        public TabStrip Reload(Element selector)
        {
            return null;
        }

        public object Select(object selector)
        {
            return null;
        }
    }
}
