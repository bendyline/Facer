using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using BL.UI;


namespace BL.JQM
{
    public class ListView : Control
    {

        public ListView()
        {

        }

        protected override void OnEnsureElements()
        {
            Element ul = Document.CreateElement("ul");
            ul.SetAttribute("data-role", "listview");
            ul.SetAttribute("data-inset", "true");
            this.Element.AppendChild(ul);

            Element li = Document.CreateElement("li");
            li.InnerHTML = "<a href='#'>Acura</a>";
            ul.AppendChild(li);
            
        }
    }
}
