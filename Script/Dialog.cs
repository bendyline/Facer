/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Html;
using System.Runtime.CompilerServices;

namespace BL.UI
{
    public enum CloseButtonStyle
    {
        CloseX = 0,
        BackArrow = 1
    }
    public class Dialog : ContentControl
    {
        private CloseButtonStyle closeButtonStyle = CloseButtonStyle.CloseX;

        [ScriptName("e_titleText")]
        public Element titleText;

        [ScriptName("e_leftCloseBin")]
        public Element leftCloseBin;

        [ScriptName("e_doneButton")]
        public Element doneButton;

        [ScriptName("e_topBar")]
        public Element topBar;

        [ScriptName("e_topBarSizer")]
        public Element topBarSizer;

        [ScriptName("e_topInterior")]
        public Element topInterior;

        [ScriptName("e_bottomBar")]
        public Element bottomBar;

        [ScriptName("e_closeButtonR")]
        public Element closeButtonR;

        [ScriptName("e_contentContainer")]
        public Element contentContainer;

        [ScriptName("e_closeButtonL")]
        public Element closeButtonL;

        [ScriptName("e_panel")]
        private Element panel;


        private bool displayCloseButton = true;
        private bool displayDoneButton = false;
        private bool isShowing = false;
        private bool isPrimarilyDarkBackground = true;
        private int dialogWidth = 0;
        private int dialogHeight = 0;
        private int interiorBottomPadding = 0;
        private int interiorTopPadding = 0;
        private int dialogIndexAtTimeOfCreation = -1;
        private String overflow = null;
        private String overflowX = null;
        private String overflowY = null;
        private String title = null;

        private int? maxWidth;
        private int? maxHeight;

        private int horizontalPadding = 30;
        private int verticalPadding = 30;

        public event EventHandler Closing;

        private static int dialogsShown = 0;
        public static event EventHandler DialogShown;
        public static event EventHandler DialogHidden;

        private ElementEventListener keyboardEventHandler;

        public CloseButtonStyle CloseButtonStyle
        {
            get
            {
                return this.closeButtonStyle;
            }

            set
            {
                this.closeButtonStyle = value;

                this.ApplyCloseButtonStyle();
            }
        }

        public int InteriorBottomPadding
        {
            get
            {
                return this.interiorBottomPadding;
            }

            set
            {
                this.interiorBottomPadding = value;
            }
        }

        public int InteriorTopPadding
        {
            get
            {
                return this.interiorTopPadding;
            }

            set
            {
                this.interiorTopPadding = value;
            }
        }

        public bool IsPrimarilyDarkBackground
        {
            get
            {
                return this.isPrimarilyDarkBackground;
            }

            set
            {
                this.isPrimarilyDarkBackground = value;

                this.ApplyCloseButtonStyle();
            }
        }

        public static bool DialogIsShowing
        {
            get
            {
                return dialogsShown > 0;
            }
        }

        public static int DialogVisibleCount
        {
            get
            {
                return dialogsShown;
            }
        }

        private bool sizeContents = true;

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

        public bool SizeContents
        {
            get
            {
                return this.sizeContents;
            }

            set
            {
                this.sizeContents = value;

                this.ApplyResize();
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

            this.keyboardEventHandler = this.HandleKeyPress;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.topBarSizer.Style.MarginTop = Context.Current.FullScreenTopBufferHeight + "px";

            this.UpdateCloseButton();
            this.ApplyCloseButtonStyle();
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

        private void ApplyCloseButtonStyle()
        {
            if (this.closeButtonL == null)
            {
                return;
            }

            Element closeButton = this.closeButtonR;

            if (Context.Current.IsSmallFormFactor)
            {
                closeButton = this.closeButtonL;
            }

            if (this.closeButtonStyle == CloseButtonStyle.BackArrow)
            {
                ElementUtilities.SetText(closeButton, "");
                closeButton.Style.Position = "relative";
                closeButton.Style.Top = "-2px";
            }
            else
            {
                ElementUtilities.SetText(closeButton, "");
                closeButton.Style.Position = "";
                closeButton.Style.Top = "0px";
            }

            if (this.IsPrimarilyDarkBackground)
            {

                closeButton.Style.Color = "#FFFFFF";
            }
            else
            {

                closeButton.Style.Color = "#333333";
            }
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
                this.bottomBar.Style.Display = String.Empty;
            }
            else
            {
                this.bottomBar.Style.Display = "none";
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
                elementStyle.ZIndex = 4000;
                elementStyle.Top = "0px";
                elementStyle.Left = "0px";

                elementStyle.Width = Context.Current.BrowserInnerWidth + "px";
                elementStyle.Height = Context.Current.BrowserInnerHeight + "px";
            }


            int effectiveHorizontalPadding = this.horizontalPadding;
            int effectiveVerticalPadding = this.verticalPadding;

            if (this.topInterior != null)
            {
                this.topInterior.Style.Height = (40 + Context.Current.FullScreenTopBufferHeight).ToString() + "px";
            }

            if (Context.Current.IsSmallFormFactor)
            {
                effectiveHorizontalPadding = 0;
                effectiveVerticalPadding = 0;
            }

            if (this.panel != null)
            {
                double width = (Context.Current.BrowserInnerWidth - (effectiveHorizontalPadding * 2));
                double height = (Context.Current.BrowserInnerHeight - (effectiveVerticalPadding * 2));

                // if the max width and height are substantially less than the "whole window" general size, use that size instead
                if (this.maxWidth != null && this.maxHeight != null && this.maxWidth < (width - 40) && this.maxHeight < (height - 40))
                {
                    width = (int)this.maxWidth;
                    height = (int)this.maxHeight;

                    this.panel.Style.BorderWidth = "";
                }
                else
                {
                    if (effectiveHorizontalPadding == 0 && effectiveVerticalPadding == 0)
                    {
                        this.panel.Style.BorderWidth = "0px";
                    }
                    else
                    {
                        this.panel.Style.BorderWidth = "";
                    }
                }

                Style panelStyle = this.panel.Style;

                panelStyle.Left = ((Context.Current.BrowserInnerWidth - width) / 2) + "px";
                panelStyle.Top = ((Context.Current.BrowserInnerHeight - height) / 2) + "px";
                panelStyle.Width = width + "px";
                panelStyle.Height = height + "px";


                Style contentContainerStyle = this.contentContainer.Style;

                contentContainerStyle.Width = width + "px";

                if (this.sizeContents)
                {
                    this.contentContainer.Style.OverflowX = "hidden";

                    int offset = 0;

                    if (this.TemplateId == "bl-ui-dialogfullscreen")
                    {
                        offset = 0;
                        this.contentContainer.Style.OverflowY = "hidden";

                        if (Context.Current.DevicePlatform == DevicePlatform.iOS && !Context.Current.IsFullScreenWebApp && !Context.Current.IsHostedInApp)
                        {
                            // this is set in CSS, but due to iOS Safari's Bottom-Bar hide/show behavior we need to explicitly set it via BrowserInnerHeight,
                            // which takes into account the real interior size.
                            this.bottomBar.Style.Top = (Context.Current.BrowserInnerHeight - 69) + "px";
                        }
                    }
                    else
                    {
                        this.contentContainer.Style.OverflowY = "auto";

                        if (this.DisplayDoneButton)
                        {
                            offset += 64;
                        }
                    }

                    if (!String.IsNullOrEmpty(this.Title) || this.DisplayCloseButton)
                    {
                        offset += (45 + Context.Current.FullScreenTopBufferHeight);
                        this.topBar.Style.Display = "";
                    }
                    else
                    {
                        this.topBar.Style.Display = "none";
                    }

                    contentContainerStyle.PaddingTop = this.interiorTopPadding + "px";

                    contentContainerStyle.Height = (height - (offset + this.interiorBottomPadding)) + "px";

                    this.panel.Style.PaddingBottom = this.interiorBottomPadding + "px";
                }
                else
                {
                    this.contentContainer.Style.OverflowY = "hidden";
                    this.contentContainer.Style.OverflowX = "hidden";
                }
            }
        }

        private void HandleKeyPress(ElementEvent eventArgs)
        {
            if (eventArgs.KeyCode == 27 /* Escape */ && (this.dialogIndexAtTimeOfCreation + 1) == dialogsShown)
            {
                this.Hide();
            }
        }

        // NOTE NOTE NOTE: Some controls (e.g., Kendo) need Dialog.Content to be set AFTER AFTER you have called dialog.Show
        public void Show()
        {
            if (this.Element == null)
            {
                this.EnsureElements();
            }

            this.OnContentChanged(this.Content);

            Document.Body.AddEventListener("keydown", this.keyboardEventHandler, false);

            this.dialogIndexAtTimeOfCreation = dialogsShown;
            dialogsShown++;

            if (dialogsShown == 1)
            {
                if (DialogShown != null)
                {
                    DialogShown(this, EventArgs.Empty);
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
            panelStyle.ZIndex = 5000;

            this.UpdateTitle();
            this.UpdateDoneButton();
            this.UpdateCloseButton();

            this.Element.Style.Opacity = "0";

            if (Context.Current.DevicePlatform != DevicePlatform.Microsoft)
            {
                string transform = "scale(0.2,0.2)";
                Script.Literal("{0}.transform={1}", this.panel.Style, transform);
            }

            Window.SetTimeout(this.StartAnimation, 50);
            Document.Body.AppendChild(this.Element);

            this.ApplyResize();
        }

        private void StartAnimation()
        {
            String transition = "transform .35s ease-in-out, opacity .35s ease-in-out";

            // IE has a weird judder on scaling the dialog out that looks really bad, so don't scale the dialog on IE/Edge.
            if (Context.Current.DevicePlatform == DevicePlatform.Microsoft)
            {
                Script.Literal("{2}.transition={1};{2}.opacity=\"1\"", this.panel.Style, transition, this.Element.Style);
            }
            else
            {
                Script.Literal("{0}.transition={1};{2}.transition={1};{0}.transform=\"scale(1,1)\";{2}.opacity=\"1\"", this.panel.Style, transition, this.Element.Style);
            }
        }

        private void CloseAnimation()
        {
            Log.Message("Close animation for " + this.Element.ID);

            if (Context.Current.DevicePlatform == DevicePlatform.Microsoft)
            {
                Script.Literal("{1}.opacity=\"0\"", this.panel.Style, this.Element.Style);
            }
            else
            {
                Script.Literal("{0}.transform=\"scale(0.2,0.2)\";{1}.opacity=\"0\"", this.panel.Style, this.Element.Style);
            }
        }

        protected override void OnContentChanged(Control control)
        {
            base.OnContentChanged(control);

            if (this.Content != null)
            {
                this.Content.EnsureElements();

                if (this.Content is IDialogManager)
                {
                    ((IDialogManager)this.Content).ParentDialog = this;
                }
            }
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
            if (this.isShowing)
            {
                this.isShowing = false;

                if (Document.ActiveElement != null)
                {
                    Document.ActiveElement.Blur();
                }

                Document.Body.RemoveEventListener("keypress", this.keyboardEventHandler, false);

                this.CloseAnimation();
                Window.SetTimeout(this.HideContinue, 350);
            }
        }

        private void HideContinue()
        {
            dialogsShown--;

            if (dialogsShown == 0)
            {
                if (DialogHidden != null)
                {
                    DialogHidden(this, EventArgs.Empty);
                }
            }

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
