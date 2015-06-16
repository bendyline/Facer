/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using BL.UI;
using System.Runtime.CompilerServices;
using System.Html;

namespace BL
{
    public class ContentItemControl : Control
    {
        private String text;

        [ScriptName("e_text")]
        private Element textElement;

        [ScriptName("s_text")]
        public String Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.text = value
;
                this.OnUpdate();
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.textElement != null)
            {
                ElementUtilities.SetText(this.textElement, this.text);
            }
        }
    }
}
