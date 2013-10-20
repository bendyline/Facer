/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
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
