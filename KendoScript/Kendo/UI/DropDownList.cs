using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using jQueryApi;
using kendo.data;

namespace Kendo.UI
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]    
    public class DropDownList : Widget
    {
        public DropDownListOptions Options;
        public jQueryObject List;
        public jQueryObject Ul;

        public void Close()
        {

        }

        public object DataItem()
        {
            return null;
        }

        public bool Enable(bool value)
        {
            return false;
        }


        public void Focus()
        {

        }
        public void Open()
        {

        }

        public void Refresh()
        {

        }

        public void Search(String search)
        {

        }
        public void Select(object selector)
        {

        }

        public void SetDataSource(DataSource dataSource)
        {

        }

        public String Text(params String[] text)
        {
            return null;
        }

        public void Toggle()
        {

        }

        public object Value(params object[] newValue)
        {
            return null;
        }


    }
}
