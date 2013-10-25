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
        private String title;

        [ScriptName("e_title")]
        private Element titleElement;

        [ScriptName("s_title")]
        public String Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.title = value
;
                this.OnUpdate();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.titleElement != null)
            {
                this.titleElement.InnerText = this.title;
            }
        }
    }
}
