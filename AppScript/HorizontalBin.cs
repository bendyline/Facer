/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Html;
using System.Runtime.CompilerServices;

namespace BL.UI.App
{
    public class HorizontalBin : ItemsControl
    {
        [ScriptName("e_itemsBin")]
        private Element itemsBin;

        private List<bool> visibilities;

        public event ControlIntegerEventHandler ActiveControlChanged;

        private bool isMouseDown;
        private bool isDragging;
        private bool isAnimatingToSlot;

        private double panelWidth;
        private Date animationStart;

        private ElementEvent downEvent;
        private int scrollAnimationTime = 200;
        private int gapBetweenSections = 10;

        private double initialScrollX;
        private double initialScrollY;
        private ElementEventListener windowSizeChanged;

        private double fromX;
        private double toX;
        private bool isAnimating = false;
        private int activeIndex;
        private bool allowSwiping = true;

        private ElementEventListener draggingElementMouseMoveHandler = null;
        private ElementEventListener draggingElementMouseUpHandler = null;
        private ElementEventListener draggingElementMouseOutHandler = null;

        public int ActiveIndex
        {
            get
            {
                return this.activeIndex;
            }

            set
            {
                this.activeIndex = value;
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

        public HorizontalBin()
        {
            this.WrapItems = true;
            this.windowSizeChanged = this.UpdateSizings;

            this.visibilities = new List<bool>();
            /*
            if (!Context.Current.IsTouchOnly)
            {
                Document.Body.AddEventListener("mouseout", this.HandleDragMouseOut, true);
            }*/

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
            for (int i = 0; i < this.ItemControls.Count; i++ )
            {
                bool vis = true;

                if (this.visibilities.Count > i)
                {
                    vis = this.visibilities[i];
                }

                this.ItemControls[i].Visible = vis;
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

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Context.Current.IsSmallFormFactor)
            {
                this.gapBetweenSections = 6;
            }

            /*
            if (Context.Current.IsTouchOnly)
            {
                Debug.WriteLine("HorziontalBin: Registering touch events " + ControlUtilities.GetTouchStartEventName());
                this.Element.AddEventListener(ControlUtilities.GetTouchStartEventName(), this.HandleElementMouseDown, true);
            }
            else
            {
                Debug.WriteLine("HorizontalBin: Registering mouse events ");

                this.Element.AddEventListener("mousedown", this.HandleElementMouseDown, true);
                this.Element.AddEventListener("mousemove", this.HandleElementMouseMove, true);
                this.Element.AddEventListener("mouseup", this.HandleElementMouseUp, true);
                this.Element.AddEventListener("dragstart", this.HandleDragStartEvent, true);
            }
            */

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
                Debug.WriteLine("HorizontalBin: Mouse Move drag: " + newLeft);

                this.itemsBin.ScrollLeft = newLeft;

                e.CancelBubble = true;
            }
            else
            {
                Debug.WriteLine("HorizontalBin: Mouse Move NO DRAG");
            }
        }

        private void ConsiderStartDragging()
        {
            if (!this.isMouseDown || this.isDragging)
            {
                return;
            }

            this.isDragging = true;
            
            Debug.WriteLine("HorizontalBin: Mouse CSD");

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

            Debug.WriteLine("HorizontalBin: MouseOut");

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
            Debug.WriteLine("HorizontalBin: MouseUp");

            if (this.isDragging)
            {
                Debug.WriteLine("HorizontalBin: MouseUp is dragging");
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
            if (this.Element == null || this.ItemControls == null)
            {
                return;
            }

            double height = 0;
            
            if (this.Height != null)
            {
                height = (double)this.Height;
            }

            if (this.Element.ParentNode != null)
            {
                ClientRect cr = ControlUtilities.GetBoundingRect(this.Element.ParentNode);

                this.itemsBin.Style.Width = ((cr.Right - cr.Left) - 8) + "px";
            }

            foreach (Control c in this.ItemControls)
            {
                if (c.Element != null)
                {
                    Style style = c.Element.Style;

                    style.MarginRight = this.gapBetweenSections + "px";
                    style.Height = (height-8).ToString() + "px";
                    c.Element.ParentNode.Style.Height = "100%";
                }
            }

            this.SetFinalPosition();
        }
    }
}
