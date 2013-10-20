/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using BL.UI;
using System.Html;

namespace BL.JQM
{
    public class Page : ItemsControl
    {
        private String name;

        public String Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (this.name == value)
                {
                    return;
                }

                if (this.name != null)
                {
                    PageManager.Current.ClearPage(this.name);
                }

                this.name = value;

                PageManager.Current.RegisterPage(this);
            }
        }

        public override string TypeId
        {
            get
            {
                return "bl-jqm-page";
            }
        }

        public Page()
        {
            this.TrackResizeEvents = true;
        }

        protected override void OnEnsureElements()
        {            
            Element e = this.CreateTemplateChild("content");            
            this.Element.AppendChild(e);
            this.ContentElement = e;

        }

        protected override void OnResize()
        {
            int innerHeight = Window.InnerHeight + 5;

            this.ContentElement.Style.Height = innerHeight + "px";
            
            ContentArea contentArea = null;
            int height = 0;

            foreach (Control c in this.TemplateControls)
            {
                if (c is ContentArea)
                {
                    contentArea = (ContentArea)c;
                }
                else
                {
                    height += c.EffectiveHeight;
                }
            }

            if (contentArea != null)
            {
                Element elt = contentArea.BinElement;

                int effectiveHeight = innerHeight - height;

                if (elt != null)
                {
                    elt.Style.Height = effectiveHeight + "px";
                }

            }
        }
    }
}
