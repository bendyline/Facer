// Header.cs
//

using System;
using System.Collections.Generic;
using BL.UI;

namespace BL.JQM
{
    public class HeaderArea : jQueryControl
    {
        public override string Role
        {
            get { return "header"; }
        }

        public override String TypeId
        {
            get
            {
                return "bl-jqm-ha";
            }
        }

        protected override void OnEnsureElements()
        {
            base.OnEnsureElements();

            ControlUtilities.DisableElementTouchMove(this.Element);
        }
    }
}
