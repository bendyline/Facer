// Header.cs
//

using System;
using System.Collections.Generic;
using BL.UI;

namespace BL.JQM
{
    public class FooterArea : jQueryControl
    {
        public override string Role
        {
            get { return "footer"; }
        }

        public override String TypeId
        {
            get
            {
                return "bl-jqm-fa";
            }
        }

        protected override void OnEnsureElements()
        {
            base.OnEnsureElements();


            ControlUtilities.DisableElementTouchMove(this.Element);
        }
    }
}
