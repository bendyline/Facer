/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */
 
using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;

namespace BL.UI.App
{
    public class SliderSwipePanel : ItemsControl
    {
        [ScriptName("e_itemsBin")]
        private Element itemsBin;

        [ScriptName("e_topAreaOuter")]
        private Element topAreaOuter;

        [ScriptName("e_containerLinkBin")]
        private Element containerLinkBin;

        [ScriptName("e_containerLinkLeft")]
        private Element containerLinkLeft;

        [ScriptName("e_containerLinkRight")]
        private Element containerLinkRight;

        public event ControlIntegerEventHandler ActiveControlChanged;

        private int activeIndex;
        private int previousIndex;

        private List<String> linkTitles;

        private bool isMouseDown;
        private bool isDragging;
        private bool isAnimatingToSlot;

        private double panelWidth;
        private Date animationStart;

        private ElementEvent downEvent;
        private int scrollAnimationTime = 200;
        private int gapBetweenSections = 30;

        private double initialScrollX;
        private ElementEventListener windowSizeChanged;

        private double fromX;
        private double toX;
        private bool isAnimating = false;

        private bool allowSwiping = true;

        private ElementEventListener draggingElementMouseMoveHandler = null;
        private ElementEventListener draggingElementMouseUpHandler = null;
        private ElementEventListener draggingElementMouseOutHandler = null;


        [ScriptName("q_linkTitles")]
        public List<String> LinkTitles
        {
            get
            {
                if (this.linkTitles == null)
                {
                    this.linkTitles = new List<string>();
                }

                return this.linkTitles;
            }

            set
            {
                this.linkTitles = value;
                this.UpdateLinkBin();
            }
        }

        [ScriptName("b_allowSwiping")]
        public bool AllowSwiping
        {
            get
            {
                return this.allowSwiping;
            }
            set
            {
                this.allowSwiping = value;
            }
        }


        public int ActiveIndex
        {
            get
            {
                return this.activeIndex;
            }

            set
            {
                if (this.activeIndex == value)
                {
                    this.SetFinalPosition();
                    return;
                }

                this.previousIndex = this.activeIndex;
                this.activeIndex = value;

                this.AnimateToIndexPosition();

                this.UpdateLinkHighlights();

                if (this.ActiveControlChanged != null)
                {
                    ControlIntegerEventArgs ciea = new ControlIntegerEventArgs(this.ItemControls[this.activeIndex], this.activeIndex);

                    this.ActiveControlChanged(this.activeIndex, ciea);
                }
            }
        }

        public SliderSwipePanel()
        {
            this.WrapItems = true;
            this.windowSizeChanged = this.UpdateSizings;

            Document.Body.AddEventListener("mouseout", this.HandleDragMouseOut, true);

            this.draggingElementMouseMoveHandler = this.HandleElementMouseMove;
            this.draggingElementMouseUpHandler = this.HandleElementMouseUp;
        }

        private void SetToX()
        {
            if (this.ActiveIndex == 0)
            {
                this.toX = 0;
            }
            else
            {
                this.toX = (ControlUtilities.GetBoundingRect(this.ItemControls[this.ActiveIndex].Element).Left + this.itemsBin.ScrollLeft) - 16;
            }
        }

        private void AnimateToIndexPosition()
        {
            this.SetToX();

            if (this.isAnimating)
            {
                return;
            }
            
            this.isAnimating = true;
            this.animationStart = Date.Now;

            this.fromX = this.itemsBin.ScrollLeft;

            Window.SetTimeout(this.AnimateTick, 10);
        }

        private void AnimateTick()
        {
            Date now = Date.Now;

            int ms = now.GetTime() - this.animationStart.GetTime();

            double proportion = ms / this.scrollAnimationTime;

            if (proportion < 1)
            {
                this.itemsBin.ScrollLeft = (int)(this.fromX + ((this.toX - this.fromX) * proportion));

                Window.SetTimeout(this.AnimateTick, 10);
            }
            else
            {
                this.SetFinalPosition();
                this.isAnimating = false;
            }
        }

        private void SetFinalPosition()
        {
            this.SetToX();
            this.itemsBin.ScrollLeft = (int)this.toX;
        }

        private void UpdateLinkBin()
        {
            if (this.containerLinkBin == null)
            {
                return;
            }

            while (this.containerLinkBin.ChildNodes.Length > 0)
            {
                this.containerLinkBin.RemoveChild(this.containerLinkBin.ChildNodes[0]);
            }

            if (this.linkTitles == null)
            {
                return;
            }

            if (Context.Current.IsSmallFormFactor && this.topAreaOuter != null)
            {
                this.topAreaOuter.Style.Display = "none";
                return;
            }

            if (this.linkTitles.Count == 0)
            {
                this.topAreaOuter.Style.Display = "none";
            }
            else
            {
                this.topAreaOuter.Style.Display = "block";
            }

            int i = 0;

            foreach (String linkTitle in this.linkTitles)
            {
                Element linkTitleElement = null;

                String cssBase = "swipeLink";

                if (i == this.activeIndex)
                {
                    cssBase += " selected";
                }

                if (i == this.LinkTitles.Count - 1)
                {
                    cssBase += " lastLink";
                }

                if (i == 0)
                {
                    cssBase += " firstLink";
                }

                linkTitleElement = this.CreateElement(cssBase);

                linkTitleElement.InnerText = linkTitle;

                linkTitleElement.SetAttribute("linkIndex", i);
                linkTitleElement.AddEventListener("click", this.HandleLinkClick, true);

                this.containerLinkBin.AppendChild(linkTitleElement);

                i++;
            }
        }


        private void UpdateLinkHighlights()
        {
            if (previousIndex >= 0)
            {
                Element secondaryTab = this.containerLinkBin.ChildNodes[this.previousIndex];

                if (secondaryTab != null)
                {
                    secondaryTab.ClassName = this.GetElementClass("swipeLink");
                }
            }

            Element primaryTab = this.containerLinkBin.ChildNodes[this.activeIndex];

            if (primaryTab != null)
            {
                primaryTab.ClassName = this.GetElementClass("swipeLink selected");
            }

        }

        private void HandleLinkClick(ElementEvent e)
        {
            object val = e.Target.GetAttribute("linkIndex");

            if (val != null && val is String)
            {
                this.ActiveIndex = Int32.Parse((String)val);
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Context.Current.IsSmallFormFactor)
            {
                this.gapBetweenSections = 10;
            }


            if (Context.Current.IsTouchOnly)
            {
                Document.Body.AddEventListener("touchstart", this.HandleElementMouseDown, true);
            }
            else
            {
                this.Element.AddEventListener("mousedown", this.HandleElementMouseDown, true);
                this.Element.AddEventListener("mousemove", this.HandleElementMouseMove, true);
                this.Element.AddEventListener("mouseup", this.HandleElementMouseUp, true);
                this.Element.AddEventListener("dragstart", this.HandleDragStartEvent, true);
            }

            this.UpdateLinkBin();

            this.UpdateSizings(null);

            Window.SetTimeout(new Action(this.UpdateSizingsAction), 1);
        }

        protected override void OnVisibilityChanged()
        {
            base.OnVisibilityChanged();

            if (this.Visible)
            {
                Window.AddEventListener("resize", this.windowSizeChanged);
            }
            else
            {
                Window.RemoveEventListener("resize", this.windowSizeChanged);
            }
        }

        protected override void OnItemControlAdded(Control c)
        {
            base.OnItemControlAdded(c);

            c.Visible = true;

            this.UpdateSizings(null);
        }


        private void HandleElementMouseDown(ElementEvent e)
        {
            if (this.allowSwiping)
            {
                String targetTagName = e.Target.TagName.ToLowerCase();

                object contentEditable = e.Target.GetAttribute("contenteditable");

                if (targetTagName == "input" || targetTagName == "select" || targetTagName == "textarea" || contentEditable == "true")
                {
                    return;
                }

                this.isMouseDown = true;

                Window.SetTimeout(this.ConsiderStartDragging, 200);

                this.downEvent = e;
                this.initialScrollX = this.itemsBin.ScrollLeft;
            }
        }

        private void HandleElementMouseMove(ElementEvent e)
        {
            if (this.isDragging)
            {
                this.itemsBin.ScrollLeft = (int)Math.Floor(this.initialScrollX + (ControlUtilities.GetPageX(this.downEvent) -  ControlUtilities.GetPageX(e)));

                e.PreventDefault();
                e.CancelBubble = true;
            }
        }

        private void ConsiderStartDragging()
        {
            if (!this.isMouseDown)
            {
                return;
            }

            if (Context.Current.IsTouchOnly)
            {
                Document.Body.AddEventListener("touchmove", this.draggingElementMouseMoveHandler, true);
                Document.Body.AddEventListener("touchend", this.draggingElementMouseUpHandler, true);
            }

            this.isDragging = true;
        }

        private void HandleDragMouseOut(ElementEvent e)
        {
            // has the mouse left the window?
            if (e.ToElement == null || e.ToElement.NodeName == "HTML")
            {
                this.HandleElementMouseUp(null);
            }

            e.CancelBubble = true;
        }

        private void HandleElementMouseUp(ElementEvent e)
        {
            this.isMouseDown = false;

            if (this.isDragging)
            {

                if (Context.Current.IsTouchOnly)
                {
                    Document.Body.RemoveEventListener("touchmove", this.draggingElementMouseMoveHandler, true);
                    Document.Body.RemoveEventListener("touchend", this.draggingElementMouseUpHandler, true);
                }

                this.isDragging = false;

                int newIndex = Math.Floor((this.itemsBin.ScrollLeft + panelWidth / 2) / panelWidth);

                if (newIndex != this.ActiveIndex)
                {
                    this.ActiveIndex = newIndex;
                }
                else
                {
                    if (this.itemsBin.ScrollLeft > (initialScrollX + (panelWidth / 4)) && this.ActiveIndex < this.ItemControls.Count - 1)
                    {
                        this.ActiveIndex++;
                    }
                    else if (this.itemsBin.ScrollLeft < (initialScrollX - (panelWidth / 4)) && this.ActiveIndex > 0)
                    {
                        this.ActiveIndex--;
                    }
                    else
                    {
                        this.AnimateToIndexPosition();
                    }
                }
            }
        }

        private void HandleDragStartEvent(ElementEvent e)
        {
            e.CancelBubble = true;

            e.PreventDefault();
            e.StopPropagation();
        }

        protected override void OnDimensionChanged()
        {
            base.OnDimensionChanged();

            this.UpdateSizings(null);
        }

        public void UpdateSizingsAction()
        {
            this.UpdateSizings(null);
        }

        public void UpdateSizings(ElementEvent e)
        {
            if (this.Element == null)
            {
                return;
            }

            ClientRect cr = ControlUtilities.GetBoundingRect(this.Element);

            double height = cr.Bottom - cr.Top;

            if (this.Height != null)
            {
                height = (double)this.Height;
            }

            this.panelWidth = Window.InnerWidth;

            foreach (Control c in this.ItemControls)
            {
                if (c.Element != null)
                {
                    Style style = c.Element.Style;

                    style.Width = ((cr.Right - cr.Left) - (this.gapBetweenSections + 4)).ToString() + "px";
                    style.MarginRight = this.gapBetweenSections + "px";
                    style.Height = (height-8).ToString() + "px";
                    c.Element.ParentNode.Style.Height = "100%";
                }
            }

            this.SetFinalPosition();
        }
    }
}
