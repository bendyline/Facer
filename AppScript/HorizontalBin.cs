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

        [ScriptName("e_leftPaddle")]
        private Element leftPaddle;

        [ScriptName("e_rightPaddle")]
        private Element rightPaddle;

        private List<bool> visibilities;

        public event ControlIntegerEventHandler ActiveControlChanged;

        private bool isMouseDown;
        private bool isDragging;
        private bool isAnimatingToSlot;

        private bool displayPaddles = true;

        private double panelWidth;
        private Date animationStart;

        private int scrollAnimationTime = 200;
        private int gapBetweenSections = 10;

        private double initialScrollX;
        private double initialScrollY;
        private ElementEventListener windowSizeChanged;

        private int visibleItemCount = 1;
        private double fromX;
        private double toX;
        private bool isAnimating = false;
        private int startIndex;
        private bool allowSwiping = true;
        private int lastDragEventTime;

        private ElementEventListener draggingElementMouseMoveHandler = null;
        private ElementEventListener draggingElementMouseUpHandler = null;
        private ElementEventListener draggingElementMouseOutHandler = null;

        public bool DisplayPaddles
        {
            get
            {
                return this.displayPaddles;
            }
        }

        public int StartIndex
        {
            get
            {
                return this.startIndex;
            }

            set
            {
                this.startIndex = value;

                this.SetToX();
                this.AnimateToIndexPosition();
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

        public double InitialScrollX
        {
            get
            {
                return this.initialScrollX;
            }
        }

        public HorizontalBin()
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

        private void ApplyPaddleVisibility()
        {
            if (this.displayPaddles && this.ItemControls != null  && this.StartIndex > 0 && this.visibleItemCount < this.ItemControls.Count)
            {
                this.leftPaddle.Style.Display = String.Empty;
            }
            else
            {
                this.leftPaddle.Style.Display = "none";
            }

            if (this.displayPaddles && this.ItemControls != null && this.StartIndex < this.ItemControls.Count - visibleItemCount && this.visibleItemCount < this.ItemControls.Count)
            {
                this.rightPaddle.Style.Display = String.Empty;
            }
            else
            {
                this.rightPaddle.Style.Display = "none";
            }
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
            if (this.StartIndex == 0)
            {
                this.toX = 0;
            }
            else
            {
                this.toX = 0;

                for (int i = 0; i < this.ItemControls.Count && i <= this.StartIndex; i++)
                {
                    ClientRect cr = ControlUtilities.GetBoundingRect(this.ItemControls[i].Element);

                    this.toX += (cr.Right - cr.Left);
                }
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
            
            if (Context.Current.IsTouchOnly)
            {
                Debug.WriteLine("HorziontalBin: Registering touch events " + ControlUtilities.GetTouchStartEventName());

                this.itemsBin.AddEventListener(ControlUtilities.GetTouchStartEventName(), this.HandleElementMouseDown, true);
            }
            else
            {
                Debug.WriteLine("HorizontalBin: Registering mouse events ");

                this.itemsBin.AddEventListener("mousedown", this.HandleElementMouseDown, true);
                this.itemsBin.AddEventListener("mousemove", this.HandleElementMouseMove, true);
                this.itemsBin.AddEventListener("mouseup", this.HandleElementMouseUp, true);
                this.itemsBin.AddEventListener("dragstart", this.HandleDragStartEvent, true);
            }

            this.rightPaddle.AddEventListener("mouseup", this.HandleRightPaddle, true);

            this.leftPaddle.AddEventListener("mouseup", this.HandleLeftPaddle, true);

            this.UpdateSizings(null);

            Window.SetTimeout(new Action(this.UpdateSizingsAction), 1);
        }

        private void HandleLeftPaddle(ElementEvent e)
        {
            this.StartIndex -= visibleItemCount;

            this.ApplyPaddleVisibility();
        }

        private void HandleRightPaddle(ElementEvent e)
        {
            this.StartIndex += visibleItemCount;

            this.ApplyPaddleVisibility();
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
            Debug.WriteLine("HorizontalBin: Mouse Down");

            e.PreventDefault();  

            if (this.allowSwiping && !this.isDragging)
            {        
                this.isMouseDown = true;

                this.initialScrollX = this.itemsBin.ScrollLeft + ControlUtilities.GetPageX(e);

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

        private void HandleDragMoveDeadTimeout()
        {
            int now = Date.Now.GetTime();

            if (now - this.lastDragEventTime > 100 && this.isDragging)
            {
                this.HandleElementMouseUp(null);
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
                this.lastDragEventTime = Date.Now.GetTime();
                Window.SetTimeout(this.HandleDragMoveDeadTimeout, 100);

                int newLeft = (int)Math.Floor(this.initialScrollX - ControlUtilities.GetPageX(e));
                Debug.WriteLine("HorizontalBin: Mouse Move drag: " + newLeft);

                this.itemsBin.ScrollLeft = newLeft;

                this.ApplyPaddleVisibility();
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
            if (e != null)
            {
                e.PreventDefault();
            }

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

                int left = 0;
                int newIndex =  0;
                for (int i=0; i<this.ItemControls.Count; i++)
                {
                    ClientRect cr = ControlUtilities.GetBoundingRect(this.ItemControls[i].Element);

                    if (cr.Left < this.itemsBin.ScrollLeft)
                    {
                        if (this.itemsBin.ScrollLeft > cr.Left + ((cr.Right - cr.Left) / 2))
                        {
                            newIndex = i + 1;
                        }
                        else
                        {
                            newIndex = i;
                        }
                    }
                }

                if (newIndex != this.StartIndex)
                {
                    this.StartIndex = newIndex;
                }

                this.ApplyPaddleVisibility();
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

            int width = 0; 

            if (this.Width != null)
            {
                width = ((int)this.Width - 48);
            }
            else if (this.Element.ParentNode != null)
            {
                ClientRect cr = ControlUtilities.GetBoundingRect(this.Element.ParentNode);

                width = (int)((cr.Right - cr.Left) - 48);
            }

            int itemWidth= 0;

            foreach (Control c in this.ItemControls)
            {
                if (c.Element != null)
                {
                    Style style = c.Element.Style;

                    style.MarginRight = this.gapBetweenSections + "px";
                    style.Height = (height-8).ToString() + "px";

                    ClientRect cr = ControlUtilities.GetBoundingRect(c.Element);

                    itemWidth = (int)(cr.Right - cr.Left);

                    c.Element.ParentNode.Style.Height = "100%";
                }
            }

            if (width > 0)
            {
                this.itemsBin.Style.MaxWidth = width + "px";

                this.visibleItemCount = Math.Max(1, Math.Floor(width / itemWidth));
            }

            this.SetFinalPosition();
            this.ApplyPaddleVisibility();
        }
    }
}
