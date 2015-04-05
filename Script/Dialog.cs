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
        [ScriptName("e_titleText")]
        public Element titleText;

        [ScriptName("e_leftCloseBin")]
        public Element leftCloseBin;

        [ScriptName("e_doneButton")]
        public Element doneButton;

        [ScriptName("e_topBar")]
        public Element topBar;

        [ScriptName("e_closeButtonR")]
        public Element closeButtonR;

        [ScriptName("e_closeButtonL")]
        public Element closeButtonL;

        [ScriptName("e_panel")]
        private Element panel;


        private bool displayCloseButton = true;
        private bool displayDoneButton = false;
        private bool isShowing = false;
        private int dialogWidth = 0;
        private int dialogHeight = 0;
        private String overflow = null;
        private String overflowX = null;
        private String overflowY = null;
        private String title = null;

        private int? maxWidth;
        private int? maxHeight;

        private int horizontalPadding = 30;
        private int verticalPadding = 30;

        public event EventHandler Closing;

        public bool DisplayDoneButton
        {
            get
            {
                return this.displayDoneButton;
            }

            set
            {
                this.displayDoneButton = value;

                this.UpdateDoneButton();
            }
        }

        public bool DisplayCloseButton
        {
            get
            {
                return this.displayCloseButton;
            }

            set
            {
                this.displayCloseButton = value;

                this.UpdateCloseButton();
            }
        }

        public String Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.title = value;

                this.UpdateTitle();
            }
        }

        public int HorizontalPadding
        {
            get
            {
                return this.horizontalPadding;
            }

            set
            {
                this.horizontalPadding = value;
            }
        }

        public int VerticalPadding
        {
            get
            {
                return this.verticalPadding;
            }

            set
            {
                this.verticalPadding = value;
            }
        }


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

        public bool IsShowing
        {
            get
            {
                return this.isShowing;
            }
        }

 
        public Dialog()
        {
            this.TrackResizeEvents = true;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.topBar.Style.PaddingTop = Context.Current.FullScreenTopBufferHeight + "px";

            this.UpdateCloseButton();
        }

        [ScriptName("v_onDoneButtonClick")]
        private void HandleDneButton(ElementEvent ee)
        {
            this.Hide();
        }

        [ScriptName("v_onCloseButtonLClick")]
        private void HandleCloseButtonL(ElementEvent ee)
        {
            this.Hide();
        }

        [ScriptName("v_onCloseButtonRClick")]
        private void HandleCloseButtonR(ElementEvent ee)
        {
            this.Hide();
        }

        private void UpdateCloseButton()
        {
            if (this.closeButtonR == null)
            {
                return;
            }

            if (this.displayCloseButton)
            {
                if (Context.Current.IsSmallFormFactor)
                {
                    this.closeButtonR.Style.Display = "none";
                    this.closeButtonL.Style.Display = "";
                    this.leftCloseBin.Style.Display = "";
                }
                else
                {
                    this.closeButtonR.Style.Display = "";
                    this.closeButtonL.Style.Display = "none";
                    this.leftCloseBin.Style.Display = "none";
                }
            }
            else
            {
                this.closeButtonL.Style.Display = "none";
                this.closeButtonR.Style.Display = "none";    
            }
        }

        private void UpdateDoneButton()
        {

            if (this.doneButton == null)
            {
                return;
            }

            if (this.displayDoneButton)
            {
                this.doneButton.Style.Display = String.Empty;
            }
            else
            {
                this.doneButton.Style.Display = "none";
            }
        }

        protected override void OnResize()
        {
            base.OnResize();

            this.ApplyResize();
        }

        private void ApplyResize()
        {
            if (this.Element != null)
            {
                Style elementStyle = this.Element.Style;

                elementStyle.Position = "fixed";
                elementStyle.ZIndex = 254;
                elementStyle.Top = "0px";
                elementStyle.Left = "0px";

                elementStyle.Width = Window.InnerWidth + "px";
                elementStyle.Height = Window.InnerHeight + "px";
            }


            int effectiveHorizontalPadding = this.horizontalPadding;
            int effectiveVerticalPadding = this.verticalPadding;

            if (Context.Current.IsSmallFormFactor)
            {
                effectiveHorizontalPadding = 0;
                effectiveVerticalPadding = 0;
            }

            if (this.panel != null)
            {
                double width = (Window.InnerWidth - (effectiveHorizontalPadding * 2));
                double height = (Window.InnerHeight - (effectiveVerticalPadding * 2));

                if (this.maxWidth != null)
                {
                    width = (int)this.maxWidth;

                    if (width > Window.InnerWidth)
                    {
                        width = Window.InnerWidth - 20;
                    }
                }

                if (this.maxHeight != null)
                {
                    height = (int)this.maxHeight;

                    if (height > Window.InnerHeight)
                    {
                        height = Window.InnerHeight - 20;
                    }
                }

                Style panelStyle = this.panel.Style;

                panelStyle.Left = ((Window.InnerWidth - width) / 2) + "px";
                panelStyle.Top = ((Window.InnerHeight - height) / 2) + "px";
                panelStyle.Width = width + "px";
                panelStyle.Height = height + "px";
            }
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

                if (this.Content is IDialogManager)
                {
                    ((IDialogManager)this.Content).ParentDialog = this;
                }
            }

            this.isShowing = true;

            this.overflow = Document.Body.Style.Overflow;
            this.overflowX = Document.Body.Style.OverflowX;
            this.overflowY = Document.Body.Style.OverflowY;

            Document.Body.Style.OverflowX = "hidden";
            Document.Body.Style.OverflowY = "hidden";

            Style panelStyle = this.panel.Style;

            panelStyle.Position = "fixed";
            panelStyle.ZIndex = 255;

            this.UpdateTitle();
            this.UpdateDoneButton();
            this.UpdateCloseButton();

            OpacityAnimator oa = new OpacityAnimator();
            oa.Element = this.Element;
            oa.From = 0;
            oa.To = 1;
            oa.Start(200, null, null);
            
            oa = new OpacityAnimator();
            oa.Element = this.panel;
            oa.From = 0;
            oa.To = 1;
            oa.StartAfter(200, 200, null, null);

            Document.Body.AppendChild(this.Element);

            this.ApplyResize();
        }

        private void UpdateTitle()
        {
            if (this.titleText == null)
            {
                return;
            }

            if (String.IsNullOrEmpty(this.title))
            {
                ElementUtilities.SetText(this.titleText, "");
            }
            else
            {
                ElementUtilities.SetText(this.titleText, this.title);
            }
        }

        public void Hide()
        {
            this.isShowing = false;

            if (this.Closing != null)
            {
                this.Closing(this, EventArgs.Empty);
            }

            Document.Body.Style.OverflowX = this.overflowX;
            Document.Body.Style.OverflowY = this.overflowY;
            Document.Body.Style.Overflow = this.overflow;

            try
            {
                if (Document.Body.Contains(this.Element))
                {
                    Document.Body.RemoveChild(this.Element);
                }
            }
            catch (Exception)
            {
                ;
            }
        }
    }
}
