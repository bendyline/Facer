/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using jQueryApi;

namespace BL.UI.App
{
    public class ToolBarPopoutButton : ContentItemControl
    {
        public event StringEventHandler Clicked;
        public event ControlValueEventHandler ValueChanged;

        private Element popoutElement;
        private TemplateImageButton tib;

        private String stringValue;

        private bool popoutIsVisible = false;

        
        public String Value
        {
            get
            {
                return this.stringValue;
            }

            set
            {
                this.stringValue = value;
            }
        }

        public TemplateImageButton Popout
        {
            get
            {
                return this.tib;
            }
        }

        public ToolBarPopoutButton()
        {
            this.tib = new TemplateImageButton();
            this.tib.Clicked += tib_Clicked;
            this.tib.OffspaceClicked += tib_OffspaceClicked;
        }

        private void tib_OffspaceClicked(object sender, EventArgs e)
        {
            this.Hide() ;
        }

        private void tib_Clicked(object sender, StringEventArgs e)
        {
            if (popoutIsVisible)
            {
                this.Hide();
            }

            if (this.tib.IsRadioButtonToggle)
            {
                if (this.ValueChanged != null)
                {
                    ControlValueEventArgs cvea = new ControlValueEventArgs(this);

                    cvea.Value = e.Value;

                    this.ValueChanged(this, cvea);
                }
            }
            else
            {
                if (this.Clicked != null)
                {
                    this.Clicked(this, e);
                }
            }
        }

        private void Show()
        {
            if (this.popoutElement == null)
            {
                this.popoutElement = Document.CreateElement("DIV");
                this.popoutElement.Style.Position = "absolute";

                jQueryPosition jqp = this.J.GetOffset();

                int left = jqp.Left;
                int top = jqp.Top;

                if (this.Popout.ImageWidth != null)
                {
                    left -= (((int)this.Popout.ImageWidth) / 2) - (this.Element.OffsetWidth / 2);
                }
                
                if (this.Popout.ImageHeight != null)
                {
                    top -= (((int)this.Popout.ImageHeight) - this.Element.OffsetHeight);
                }

                this.popoutElement.Style.Left = left + "px";
                this.popoutElement.Style.Top = top + "px";

                this.tib.AttachTo(this.popoutElement, null);
            }

            Document.Body.AppendChild(this.popoutElement);
            this.popoutIsVisible = true;
        }

        private void Hide()
        {
            if (this.popoutElement != null)
            {
                Document.Body.RemoveChild(this.popoutElement);
            }
        
            this.popoutIsVisible = false;
        }

        protected override void OnInit()
        {
            base.OnInit();

            this.TrackInteractionEvents = true;
        }

        protected override void OnClick(System.Html.ElementEvent e)
        {
            if (this.popoutIsVisible)
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }
        }
    }
}
