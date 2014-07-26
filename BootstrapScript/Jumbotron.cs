/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Html;
using BL.UI;

namespace BL.BS
{
    public class Jumbotron : ItemsControl
    {
        [ScriptName("e_title")]
        protected Element titleElement;

        [ScriptName("e_lead")]
        protected Element leadElement;

        [ScriptName("e_image")]
        protected ImageElement imageElement;

        [ScriptName("e_imageCell")]
        protected ImageElement imageCellElement;

        [ScriptName("e_callToAction")]
        protected Element callToActionElement;

        private String title;
        private String lead;
        private String imageUrl;

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

        [ScriptName("s_lead")]
        public String Lead
        {
            get
            {
                return this.lead;
            }

            set
            {
                this.lead = value;

                this.Update();
            }
        }

        [ScriptName("s_imageUrl")]
        public String ImageUrl
        {
            get
            {
                return this.imageUrl;
            }

            set
            {
                this.imageUrl = value;

                this.Update();
            }
        }

        protected override void OnApplyTemplate()
        {
            if (this.titleElement != null)
            {
                ControlUtilities.SetText(this.titleElement, this.Title);
            }

            if (this.leadElement != null)
            {
                ControlUtilities.SetText(this.leadElement, this.Lead);
            }

            if (this.imageElement != null && this.imageCellElement != null)
            {
                if (String.IsNullOrEmpty(this.ImageUrl))
                {
                    this.imageCellElement.Style.Display = "none";
                }
                else
                {
                    this.imageElement.Src = this.ImageUrl;
                    this.imageCellElement.Style.Display = "block";
                }
            }
        }
    }
}
