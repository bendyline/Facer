/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

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

            ElementUtilities.DisableElementTouchMove(this.Element);
        }
    }
}
