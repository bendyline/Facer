/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using BL;
using BL.Extern;
using jQueryApi;
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


        private bool isMouseDown;
        private bool isDragging;

        private bool displayPaddles = true;
        private bool elementInternallyScrolled = false;

        private Date animationStart;

        private int scrollAnimationTime = 200;
        private int gapBetweenSections = 4;

        private double initialScrollX;
        private ElementEventListener windowSizeChanged;
        private Date lastScrollTime;
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
        private ElementEventListener elementScrolledHandler = null;

        public event EventHandler Scrolling;

        public int GapBetweenSections
        {
            get
            {
                return this.gapBetweenSections;
            }

            set
            {
                this.gapBetweenSections = value;
            }
        }

        public Date LastScrollTime
        {
            get
            {
                return this.lastScrollTime;
            }
        }

        public bool DisplayPaddles
        {
            get
            {
                return      this.displayPaddles && 
                                (this.ItemControls == null || (this.ItemControls != null && this.visibleItemCount < this.ItemControls.Count)) && 
                                !Context.Current.IsTouchOnly;
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

            this.elementScrolledHandler = this.HandleElementScroll;
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

            this.ApplyHorizontalBinVisibility();
        }

        private void ApplyPaddleVisibility()
        {
            if (this.DisplayPaddles)
            {
                this.rightPaddle.Style.Display = String.Empty;
                this.leftPaddle.Style.Display = String.Empty;

                if (this.DisplayPaddles && this.StartIndex > 0)
                {
                    this.leftPaddle.Style.Visibility  = String.Empty;
                }
                else
                {
                    this.leftPaddle.Style.Visibility = "hidden";
                }

                if (this.DisplayPaddles && this.ItemControls != null && (this.StartIndex + 1) < this.ItemControls.Count - (visibleItemCount + 1))
                {
                    this.rightPaddle.Style.Visibility = String.Empty;
                }
                else
                {
                    this.rightPaddle.Style.Visibility = "hidden";
                }
            }
            else
            {
                this.rightPaddle.Style.Display = "none";
                this.leftPaddle.Style.Display = "none";
            }
        }

        private void ApplyHorizontalBinVisibility()
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
                    ClientRect cr = ElementUtilities.GetBoundingRect(this.ItemControls[i].Element);
                    this.toX += (cr.Right - cr.Left);
                    Log.Message("Setting sizing for item " + i + " to " + cr.Right + "|" + cr.Left + " TOX:" + this.toX + " SI: " +this.StartIndex);
                }
            }
        }

        private void InferStartIndex()
        {
            this.startIndex = 0;
            double left = 0;

            for (int i = 0; i < this.ItemControls.Count; i++)
            {
                ClientRect cr = ElementUtilities.GetBoundingRect(this.ItemControls[i].Element);

                left += (cr.Right - cr.Left);

                if (left > this.itemsBin.ScrollLeft)
                {
                    this.startIndex = i;
                    return;
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

            Window.SetTimeout(this.AnimateTick, 15);
        }

        private void AnimateTick()
        {
            Date now = Date.Now;

            int ms = now.GetTime() - this.animationStart.GetTime();

            double proportion = Easing.EaseInQuad(ms, 0, 1, this.scrollAnimationTime);

            if (proportion < 1)
            {
                int valu = (int)(this.fromX + ((this.toX - this.fromX) * proportion));

                this.elementInternallyScrolled = true;
                jQuery.FromObject(this.itemsBin).ScrollLeft(valu);

                this.lastScrollTime = Date.Now;
                // Log.Message("Setting bin left to " + this.itemsBin.ScrollLeft + " TOX:" + this.toX + " FROMX:" + this.fromX + " PROP" + proportion + " VAL" + valu);
                Window.SetTimeout(this.AnimateTick, 15);
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

            if (this.Scrolling != null)
            {
                this.Scrolling(this, EventArgs.Empty);
            }

            if (this.itemsBin == null)
            {
                return;
            }

            this.elementInternallyScrolled = true;
            this.itemsBin.ScrollLeft = (int)this.toX;
            this.lastScrollTime = Date.Now;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
           
            if (Context.Current.IsTouchOnly)
            {
                Debug.WriteLine("(HorziontalBin::OnApplyTemplate) - Registering touch events " + ElementUtilities.GetTouchStartEventName());

                this.itemsBin.AddEventListener(ElementUtilities.GetTouchStartEventName(), this.HandleElementMouseDown, true);
            }
            else
            {
                Debug.WriteLine("(HorziontalBin::OnApplyTemplate) - Registering mouse events ");

                this.itemsBin.AddEventListener("mousedown", this.HandleElementMouseDown, true);
                this.itemsBin.AddEventListener("mousemove", this.HandleElementMouseMove, true);
                this.itemsBin.AddEventListener("scroll", this.elementScrolledHandler, true);
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

        private void HandleElementMouseDown(ElementEvent e)
        {
            if (ElementUtilities.IsDefaultInputElement(e, false))
            {
                return;
            }
            Debug.WriteLine("HorizontalBin: Mouse Down");

     //       e.PreventDefault();  

            if (this.allowSwiping && !this.isDragging)
            {        
                this.isMouseDown = true;

                this.initialScrollX = this.itemsBin.ScrollLeft + ElementUtilities.GetPageX(e);

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
        private void HandleElementScroll(ElementEvent e)
        {
            if (!this.elementInternallyScrolled)
            {
                if (this.isDragging)
                {
                    this.isDragging = false;
                }

                this.InferStartIndex();
                this.ApplyPaddleVisibility();
            }

            this.elementInternallyScrolled = false;
        }

        private void HandleElementMouseMove(ElementEvent e)
        {
            if (ElementUtilities.IsDefaultInputElement(e, false))
            {
                e.CancelBubble = false;
                return;
            }


            if (this.isDragging)
            {
                this.lastDragEventTime = Date.Now.GetTime();
                Window.SetTimeout(this.HandleDragMoveDeadTimeout, 100);

                double eventX = ElementUtilities.GetPageX(e);
                int newLeft = (int)Math.Floor(this.initialScrollX - eventX);

                if (Math.Abs((newLeft + eventX) - this.initialScrollX) > 10)
                {
                    e.PreventDefault();
                }

                Debug.WriteLine("(HorizontalBin::HandleElementMouseMove) - Mouse Move drag: " + newLeft);

                this.elementInternallyScrolled = true;

                if (this.itemsBin != null)
                {
                    this.itemsBin.ScrollLeft = newLeft;
                }

                this.lastScrollTime = Date.Now;

                if (this.Scrolling != null)
                {
                    this.Scrolling(this, EventArgs.Empty);
                }

                this.ApplyPaddleVisibility();
          //      e.CancelBubble = true;
            }
            else
            {
                Debug.WriteLine("(HorizontalBin::HandleElementMouseMove) - Mouse Move NO DRAG");
            }
        }

        private void ConsiderStartDragging()
        {
            if (!this.isMouseDown || this.isDragging)
            {
                return;
            }

            this.isDragging = true;

            Debug.WriteLine("(HorizontalBin::ConsiderStartDragging)");

            if (Context.Current.IsTouchOnly)
            {
                Document.Body.AddEventListener(ElementUtilities.GetTouchMoveEventName(), this.draggingElementMouseMoveHandler, true);
                Document.Body.AddEventListener(ElementUtilities.GetTouchEndEventName(), this.draggingElementMouseUpHandler, true);

                if (ElementUtilities.GetTouchCancelEventName() != null)
                {
                    Document.Body.AddEventListener(ElementUtilities.GetTouchCancelEventName(), this.draggingElementMouseUpHandler, true);
                }
            }
        }

        private void HandleDragMouseOut(ElementEvent e)
        {
            if (!this.AllowSwiping || !this.isDragging)
            {
                return;
            }

            e.PreventDefault();

            Debug.WriteLine("(HorizontalBin::HandleDragMouseOut)");

            // has the mouse left the window?
            if ((e.ToElement == null && !Context.Current.IsTouchOnly) || (e.ToElement != null && e.ToElement.NodeName == "HTML"))
            {
                this.HandleElementMouseUp(e);
            }

            e.CancelBubble = true;
        }

        private void HandleElementMouseUp(ElementEvent e)
        {
            this.isMouseDown = false;
            Debug.WriteLine("(HorizontalBin::HandleElementMouseUp)");

            if (this.isDragging)
            {
                Debug.WriteLine("(HorizontalBin::HandleElementMouseUp) - Is Dragging ");
                if (e != null)
                {
                    e.PreventDefault();
                }

                if (Context.Current.IsTouchOnly)
                {
                   Document.Body.RemoveEventListener(ElementUtilities.GetTouchMoveEventName(), this.draggingElementMouseMoveHandler, true);
                   Document.Body.RemoveEventListener(ElementUtilities.GetTouchEndEventName(), this.draggingElementMouseUpHandler, true);
                }

                this.isDragging = false;

                int newIndex =  0;
                bool isGoingRight = this.itemsBin.ScrollLeft > this.initialScrollX;

                for (int i=0; i<this.ItemControls.Count; i++)
                {
                    ClientRect cr = ElementUtilities.GetBoundingRect(this.ItemControls[i].Element);

                    if (cr.Left < this.itemsBin.ScrollLeft)
                    {
                        if (this.itemsBin.ScrollLeft > cr.Left + ((cr.Right - cr.Left) / 2) && isGoingRight)
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

            int adjust = 8;

            if (this.DisplayPaddles)
            {
                adjust = 78;
            }

            int width = 0; 

            if (this.Width != null)
            {
                width = ((int)this.Width - adjust);
            }
            else if (this.Element.ParentNode != null)
            {
                ClientRect cr = ElementUtilities.GetBoundingRect(this.Element.ParentNode);

                width = (int)((cr.Right - cr.Left) - adjust);
            }

            int itemWidth= 0;
            int itemCount = 0;

            foreach (Control c in this.ItemControls)
            {
                if (c.Element != null)
                {
                    Style style = c.Element.Style;

                    style.MarginRight = this.gapBetweenSections + "px";
                    style.Height = (height-8).ToString() + "px";

                    ClientRect cr = ElementUtilities.GetBoundingRect(c.Element);

                    itemWidth += (int)(cr.Right - cr.Left);

                    c.Element.ParentNode.Style.Height = "100%";
                    itemCount++;
                }
            }

            itemWidth /= itemCount;
                
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
