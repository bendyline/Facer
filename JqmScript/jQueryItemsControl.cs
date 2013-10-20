/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

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
