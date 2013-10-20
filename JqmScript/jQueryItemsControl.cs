// JQueryControl.cs
//

using System;
using System.Collections.Generic;
using BL.UI;

namespace BL.JQM
{
    public abstract class jQueryItemsControl : ItemsControl
    {

        public abstract String Role
        {
            get;
        }

        protected override void OnEnsureElements()
        {
            this.Element.SetAttribute("data-role", this.Role);
        }
    }
}
