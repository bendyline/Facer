/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Html;
using System.Runtime.CompilerServices;

namespace BL.UI
{
    public class Dialog : ContentControl
    {
        private bool isShowing = false;
        private int dialogWidth = 0;
        private int dialogHeight = 0;
        private string overflow = null;
        private string overflowX = null;
        private string overflowY = null;

        private int? maxWidth;
        private int? maxHeight;

        public int? MaxWidth
        {
            get
            {
                return this.maxWidth;
            }

            set
            {
                this.maxWidth = value;
            }
        }

        public int? MaxHeight
        {
            get
            {
                return this.maxHeight;
            }

            set
            {
                this.maxHeight = value;
            }
        }

        [ScriptName("e_closeButton")]
        private Element closeButton;

        [ScriptName("e_panel")]
        private Element panel;

        public Dialog()
        {

        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.closeButton.AddEventListener("mouseup", this.HandleCloseButton, true);
        }

        private void HandleCloseButton(ElementEvent ee)
        {
            this.Hide();
        }

        public void Show()
        {
            if (this.Element == null)
            {
                this.EnsureElements();
            }

            if (this.Content != null)
            {
                this.Content.EnsureElements();
            }

            this.isShowing = true;

            this.overflow = Document.Body.Style.Overflow;
            this.overflowX = Document.Body.Style.OverflowX;
            this.overflowY = Document.Body.Style.OverflowY;

            Document.Body.Style.OverflowX = "hidden";
            Document.Body.Style.OverflowY = "hidden";


            Style elementStyle = this.Element.Style;

            elementStyle.Position = "absolute";
            elementStyle.ZIndex = 254;
            elementStyle.Top = "0px";
            elementStyle.Left = "0px";

            elementStyle.Width = Window.InnerWidth + "px";
            elementStyle.Height = Window.InnerHeight + "px";

            Style panelStyle = this.panel.Style;

            panelStyle.Position = "absolute";
            panelStyle.ZIndex = 255;

            double width = (Window.InnerWidth - 60);
            double height = (Window.InnerHeight - 60);

            if (this.maxWidth != null)
            {
                width = (int)this.maxWidth;
            }

            if (this.maxHeight != null)
            {
                height = (int)this.maxHeight;
            }

            panelStyle.Left = ((Window.InnerWidth - width) / 2) + "px";
            panelStyle.Top = ((Window.InnerHeight - height) / 2) + "px";
            panelStyle.Width =  width + "px";
            panelStyle.Height = height + "px";


            Document.Body.AppendChild(this.Element);
        }

        public void Hide()
        {
            this.isShowing = false;

            Document.Body.Style.OverflowX = this.overflowX;
            Document.Body.Style.OverflowY = this.overflowY;
            Document.Body.Style.Overflow = this.overflow;

            Document.Body.RemoveChild(this.Element);
        }
    }
}
