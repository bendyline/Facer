/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using BL.UI;
using System.Runtime.CompilerServices;

namespace BL
{
    public class ElementContentItemControl : Control
    {
        private String title;

        [ScriptName("s_title")]
        public String Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.title = value;

                this.Update();
            }
        }

        protected override void OnEnsureElements()
        {
            base.OnEnsureElements();

        }

        protected override void  OnUpdate()
        {
            if (this.title != null)
            {
                ControlUtilities.SetText(this.Element, this.title);
            }
        }
    }
}
