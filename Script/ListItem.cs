/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Linq;
using BL.UI;
using System.Runtime.CompilerServices;
using System.Html;

namespace BL.UI
{
    public class ListItem : Control
    {
        private ContentItem item;
        
        [ScriptName("e_title")]
        protected Element titleElement;

        [ScriptName("e_content")]
        protected Element contentElement;

        [ScriptName("e_image")]
        protected ImageElement image;

        public override string DefaultClass
        {
            get
            {
                return "row";
            }
        }

        public ContentItem Item
        {
            get
            {
                return this.item; 
            }

            set
            {
                this.item = value;

                if (this.item.TemplateId != null)
                {
                    this.TemplateId = this.item.TemplateId;
                }

                this.Update();
            }
        }


        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.item == null)
            {
                return;
            }

            if (titleElement != null)
            {
                ControlUtilities.SetText(titleElement, this.item.Title);
            }

            if (contentElement != null)
            {
                ControlUtilities.SetText(contentElement, this.item.Content);
            }

            if (image != null)
            {
                if (this.item.MainImage == null)
                {
                    this.image.Style.Display = "none";
                }
                else
                {
                    this.image.Style.Display = "block";
                    image.Src = this.item.MainImage;
                }
            }
        }
    }
}
