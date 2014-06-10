/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private List<bool> visibilities;

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
        private double initialScrollY;
        private ElementEventListener windowSizeChanged;

        private double fromX;
        private double toX;
        private bool isAnimating = false;

        private bool allowSwiping = true;

        private ElementEventListener draggingElementMouseMoveHandler = null;
        private ElementEventListener draggingElementMouseUpHandler = null;
        private ElementEventListener draggingElementMouseOutHandler = null;

        public event IntegerEventHandler VerticalScrollChanged;


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

        public double InitialScrollY
        {
            get
            {
                return this.initialScrollY;
            }
            
            set
            {
                this.initialScrollY = value;
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

                if (this.containerLinkBin != null && this.containerLinkBin.ChildNodes.Length > this.activeIndex)
                {
                    Element activeTab = this.containerLinkBin.ChildNodes[this.activeIndex];
                    activeTab.Focus();
                }

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

            this.visibilities = new List<bool>();

            if (!Context.Current.IsTouchOnly)
            {
                Document.Body.AddEventListener("mouseout", this.HandleDragMouseOut, true);
            }

            this.draggingElementMouseMoveHandler = this.HandleElementMouseMove;
            this.draggingElementMouseUpHandler = this.HandleElementMouseUp;
        }

        public void SetVisible(int index, bool isVisible)
        {
            while (this.visibilities.Count < index)
            {
                this.visibilities.Add(true);
            }

            this.visibilities[index] = isVisible;

            this.ApplyVisibility();
        }

        private void ApplyVisibility()
        {
            int index = 0;

            foreach (bool vis in this.visibilities)
            {
                if (this.containerLinkBin.ChildNodes.Length > index)
                {
                    Element elt = this.containerLinkBin.ChildNodes[index];

                    if (vis)
                    {
                        elt.Style.Display = String.Empty;
                    }
                    else
                    {
                        elt.Style.Display = "none";
                    }
                }

                if (this.ItemControls.Count > index)
                {
                    this.ItemControls[index].Visible = vis;
                }

                index++;
            }
        }

        private void SetToX()
        {
            if (this.ActiveIndex == 0)
            {
                this.toX = 0;
            }
            else
            {
                double baseLeft = ControlUtilities.GetBoundingRect(this.Element).Left;

                double elementLeft = ControlUtilities.GetBoundingRect(this.ItemControls[this.ActiveIndex].Element).Left;

                this.toX = ((elementLeft + this.itemsBin.ScrollLeft) - baseLeft) - 4;
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

                linkTitleElement.TabIndex = 1;
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
                Debug.WriteLine("SliderSwipePanel: Registering touch events " + ControlUtilities.GetTouchStartEventName());
                this.Element.AddEventListener(ControlUtilities.GetTouchStartEventName(), this.HandleElementMouseDown, true);
            }
            else
            {
                Debug.WriteLine("SliderSwipePanel: Registering mouse events ");

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

        private bool IsDefaultInputElement(ElementEvent e)
        {
            String targetTagName = e.Target.TagName.ToLowerCase();

            object contentEditable = e.Target.GetAttribute("contenteditable");

            if (targetTagName == "input" || targetTagName == "select" || targetTagName == "textarea" || contentEditable == "true")
            {
                return true;
            }

            return false;
        }

        private void HandleElementMouseDown(ElementEvent e)
        {
            if (IsDefaultInputElement(e))
            {
                return;
            }

            e.PreventDefault();  

            if (this.allowSwiping && !this.isDragging)
            {        
                this.isMouseDown = true;

                this.downEvent = e;
                this.initialScrollX = this.itemsBin.ScrollLeft;

                if (Context.Current.IsTouchOnly)
                {
                    this.ConsiderStartDragging();
                }
                else
                {
                    Window.SetTimeout(this.ConsiderStartDragging, 200);
                }
            }
        }

        private void HandleElementMouseMove(ElementEvent e)
        {
            if (IsDefaultInputElement(e))
            {
                return;
            }

            e.PreventDefault();

            if (this.isDragging)
            {
                int newLeft = (int)Math.Floor(this.initialScrollX + (ControlUtilities.GetPageX(this.downEvent) - ControlUtilities.GetPageX(e)));
                Debug.WriteLine("SliderSwipePanel: Mouse Move drag: " + newLeft);
                this.itemsBin.ScrollLeft = newLeft;

                e.CancelBubble = true;

                if (this.VerticalScrollChanged != null)
                {
                    int newTop = (int)Math.Floor(this.initialScrollX+ (ControlUtilities.GetPageX(this.downEvent) - ControlUtilities.GetPageX(e)));

                    IntegerEventArgs iea = new IntegerEventArgs(newTop);

                    this.VerticalScrollChanged(this, iea);
                }
            }
            else
            {
                Debug.WriteLine("SliderSwipePanel: Mouse Move NO DRAG");
            }
        }

        private void ConsiderStartDragging()
        {
            if (!this.isMouseDown || this.isDragging)
            {
                return;
            }

            this.isDragging = true;
            
            Debug.WriteLine("SliderSwipePanel: Mouse CSD");

            if (Context.Current.IsTouchOnly)
            {
                Document.Body.AddEventListener(ControlUtilities.GetTouchMoveEventName(), this.draggingElementMouseMoveHandler, true);
                Document.Body.AddEventListener(ControlUtilities.GetTouchEndEventName(), this.draggingElementMouseUpHandler, true);

                if (ControlUtilities.GetTouchCancelEventName() != null)
                {
                    Document.Body.AddEventListener(ControlUtilities.GetTouchCancelEventName(), this.draggingElementMouseUpHandler, true);
                }
            }

        }

        private void HandleDragMouseOut(ElementEvent e)
        {
            e.PreventDefault();

            if (!this.AllowSwiping)
            {
                return;
            }

            Debug.WriteLine("SliderSwipePanel: MouseOut");
            // has the mouse left the window?
            if ((e.ToElement == null && !Context.Current.IsTouchOnly) || (e.ToElement != null && e.ToElement.NodeName == "HTML"))
            {
                this.HandleElementMouseUp(e);
            }

            e.CancelBubble = true;
        }

        private void HandleElementMouseUp(ElementEvent e)
        {
            e.PreventDefault();

            this.isMouseDown = false;
            Debug.WriteLine("SliderSwipePanel: MouseUp");

            if (this.isDragging)
            {
                Debug.WriteLine("SliderSwipePanel: MouseUp is dragging");
                if (Context.Current.IsTouchOnly)
                {
                   Document.Body.RemoveEventListener(ControlUtilities.GetTouchMoveEventName(), this.draggingElementMouseMoveHandler, true);
                   Document.Body.RemoveEventListener(ControlUtilities.GetTouchEndEventName(), this.draggingElementMouseUpHandler, true);
                }
                Debug.WriteLine("MouseUp");
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
            double height = 0;
            
            if (this.Height != null)
            {
                height = (double)this.Height;
            }
            else
            {
                ClientRect topAreaRect = ControlUtilities.GetBoundingRect(this.topAreaOuter);

                height = (cr.Bottom - cr.Top) -  (topAreaRect.Bottom - topAreaRect.Top);
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
