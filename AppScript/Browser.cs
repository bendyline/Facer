/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */
 
using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;

namespace BL.UI.App
{
    public class Browser : Control
    {
        [ScriptName("e_browser")]
        private IFrameElement browser;

        private String url;

        public String Url
        {
            get
            {
                return this.url;
            }

            set
            {
                if (this.url == value)
                {
                    return;
                }

                this.url = value;

                this.Update();
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.browser != null && this.url != null)
            {
                this.browser.Src = this.url;
            }
        }
    }
}
