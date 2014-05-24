/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Html;
using System.Linq;

namespace BL.UI
{
    public enum ElementAnimationType
    {
        PosOutToLeft = 0,
        LeftInToPos = 1,
        PosOutToRight = 2,
        RightInToPos = 3
    }

    public enum ElementAnimationEndBehavior
    {
        ResetToPosition = 0,
        HideAndResetToPosition = 1,
        LeaveInPlace = 2
    }

    public enum ElementBehavior
    {
        None=0,
        DragHorizontal = 1,
        DragVertical = 2,
        Drag2D = 3
    }

    public class ElementEffects
    {
        public Element parentElement;
        public Element element;
        private Element placeHolderElement;
        private Element beforeInParentElement;
        private double left;
        private double top;
        private double width;
        private double height;

        private String originalLeft;
        private String originalTop;
        private String originalWidth;
        private String originalHeight;
        private String originalPosition;

        private Date animationStart;
        private int animationLength;

        private double originLeft;
        private double originTop;

        private double animationStartLeft;
        private double animationStartTop;
        private double animationEndLeft;
        private double animationEndTop;

        private double initialMouseDownLeft;
        private double initialMouseDownTop;

        private double? overrideWidth;
        private double? overrideHeight;

        private bool isMoving = false;
        private bool isDragging = false;
        private bool isAnimating = false;
        private bool isMouseDown = false;

        private double dragStartElementLeft;
        private double dragStartElementTop;

        private static List<ElementEffects> animationList = null;
        private static bool isAnimatingAtLeastOne = false;

        public event EventHandler AnimationComplete;
        public event PointAndStartElementEventHandler DragComplete;
        public event PointAndStartElementEventHandler DragStart;
        public event PointAndStartElementEventHandler DragMoved;


        private ElementEventListener draggingElementMouseMoveHandler = null;
        private ElementEventListener draggingElementMouseUpHandler = null;
        private ElementEventListener draggingElementMouseOutHandler = null;

        private ElementAnimationEndBehavior endBehavior = ElementAnimationEndBehavior.ResetToPosition;

        private ElementBehavior behavior = ElementBehavior.None;

        public ElementBehavior Behavior
        {
            get
            {
                return this.behavior;
            }

            set
            {
                if (this.behavior == value)
                {
                    return;
                }

                this.behavior = value;

                this.ApplyBehavior();
            }
        }

        public double OriginLeft
        {
            get
            {
                return this.originLeft;
            }

            set
            {
                this.originLeft = value;
            }
        }

        public double OriginTop
        {
            get
            {
                return this.originTop;
            }

            set
            {
                this.originTop = value;
            }
        }

        public double Left
        {
            get
            {
                return this.left;
            }

            set
            {
                this.left = value;

                this.element.Style.Left= this.left + "px";
            }
        }

        public double Top
        {
            get
            {
                return this.top;
            }

            set
            {
                this.top = value;

                this.element.Style.Top = this.top + "px";
            }
        }

        public double Height
        {
            get
            {
                return this.height;
            }
        }

        public double Width
        {
            get
            {
                return this.width;
            }
        }

        public double EffectiveWidth
        {
            get
            {
                if (this.overrideWidth != null)
                {
                    return (double)this.overrideWidth;
                }

                return this.width;
            }
        }

        public double? OverrideWidth
        {
            get
            {
                return this.overrideWidth;
            }

            set
            {
                this.overrideWidth = value;
            }
        }

        public double? OverrideHeight
        {
            get
            {
                return this.overrideHeight;
            }

            set
            {
                this.overrideHeight = value;
            }
        }

        public Element Element
        {
            get
            {
                return this.element;
            }

            set
            {
                this.element = value;

                this.ApplyBehavior();
            }
        }

        public ElementEffects()
        {
            this.element = element;
        }

        public void ApplyBehavior()
        {
            if (this.element == null)
            {
                return;
            }
            if (this.behavior == ElementBehavior.DragHorizontal || this.behavior == ElementBehavior.DragVertical || this.behavior == ElementBehavior.Drag2D)
            {
                this.element.SetAttribute("unselectable", "on");
                Script.Literal("{0}.userSelect=\"none\"", this.element.Style);


                if (Context.Current.IsTouchOnly)
                {
                    this.element.AddEventListener("touchstart", this.HandleElementMouseDown, true);
                    this.element.AddEventListener("touchend", this.HandleElementMouseUp, true);
                }
                else
                {
                    this.element.AddEventListener("mousedown", this.HandleElementMouseDown, true);
                    this.element.AddEventListener("mouseup", this.HandleElementMouseUp, true);
                    this.element.AddEventListener("dragstart", this.HandleDragStartEvent, true);
                }
            }
        }

        private void HandleElementMouseDown(ElementEvent e)
        {
            this.isMouseDown = true;

            this.initialMouseDownLeft = ControlUtilities.GetPageX(e);
            this.initialMouseDownTop = ControlUtilities.GetPageY(e);


            Script.Literal(@"
 if (window.getSelection().empty) { window.getSelection().empty(); }
");

            Window.SetTimeout(this.ConsiderStartDragging, 200);
        }

        private void HandleElementMouseUp(ElementEvent e)
        {
            this.isMouseDown = false;

            this.HandleDragMouseUpBehavior();
        }

        private void HandleDragStartEvent(ElementEvent e)
        {
            e.CancelBubble = true;

            e.PreventDefault();
            e.StopPropagation();
        }

        private void HandleElementDragging(ElementEvent e)
        {
            if (this.behavior == ElementBehavior.DragHorizontal || this.behavior == ElementBehavior.Drag2D)
            {
                this.Left = this.dragStartElementLeft + (ControlUtilities.GetPageX(e) - this.initialMouseDownLeft);
            }

            if (this.behavior == ElementBehavior.DragVertical || this.behavior == ElementBehavior.Drag2D)
            {
                this.Top = this.dragStartElementTop + (ControlUtilities.GetPageY(e) - this.initialMouseDownTop);
            }
/*
            Script.Literal(@"
 if (window.getSelection().empty) { window.getSelection().empty(); }
");
*/

            if (this.DragMoved != null)
            {
                PointAndStartElementEventArgs ee = new PointAndStartElementEventArgs(this.animationStartLeft, this.animationStartTop, this.Left, this.Top, this.element);

                this.DragMoved(this, ee);
            }

            e.PreventDefault();
            e.CancelBubble = true;
 //           e.StopImmediatePropagation();
    //        e.StopPropagation();
        }
        private void HandleDragMouseOut(ElementEvent e)
        {
            // has the mouse left the window?
            if (e.ToElement == null || e.ToElement.NodeName == "HTML")
            {
                this.HandleDragMouseUpBehavior();
            }

            e.CancelBubble = true;
        }

        private void HandleDragMouseUp(ElementEvent e)
        {
            this.HandleDragMouseUpBehavior();
            e.CancelBubble = true;
        }

        private void HandleDragMouseUpBehavior()
        {
            if (!this.isDragging)
            {
                return;
            }

            this.isDragging = false;


            Script.Literal(@"
  document.onselectstart = null;
  document.onmousedown = null");


            if (this.DragComplete != null)
            {
                PointAndStartElementEventArgs ee = new PointAndStartElementEventArgs(this.animationStartLeft, this.animationStartTop, this.Left, this.Top, this.element);

                this.DragComplete(this, ee);
            }

            Document.Body.RemoveEventListener("mousemove", this.draggingElementMouseMoveHandler, true);
            Document.Body.RemoveEventListener("mouseup", this.draggingElementMouseUpHandler, true);
            Document.Body.RemoveEventListener("mouseout", this.draggingElementMouseOutHandler, true);
        }

        private void ConsiderStartDragging()
        {
            if (!this.isMouseDown || this.behavior == ElementBehavior.None)
            {
                return;
            }

            this.StartMove();
            
            this.dragStartElementLeft = this.Left;
            this.dragStartElementTop = this.Top;

            Script.Literal(@"
 if (window.getSelection().empty) { window.getSelection().empty(); }

  document.onselectstart = function() {return false;}
  document.onmousedown = function() {return false;}");


            if (this.DragStart != null)
            {
                PointAndStartElementEventArgs ee = new PointAndStartElementEventArgs(this.animationStartLeft, this.animationStartTop, this.Left, this.Top, this.element);

                this.DragStart(this, ee);
            }

            this.isDragging = true;

            this.draggingElementMouseMoveHandler = this.HandleElementDragging;
            this.draggingElementMouseUpHandler = this.HandleDragMouseUp;
            this.draggingElementMouseOutHandler = this.HandleDragMouseOut;

            if (Context.Current.IsTouchOnly)
            {
                Document.Body.AddEventListener("touchmove", this.draggingElementMouseMoveHandler, true);
                Document.Body.AddEventListener("touchend", this.draggingElementMouseUpHandler, true);
            }
            else
            {
                Document.Body.AddEventListener("mousemove", this.draggingElementMouseMoveHandler, true);
                Document.Body.AddEventListener("mouseup", this.draggingElementMouseUpHandler, true);
                Document.Body.AddEventListener("mouseout", this.HandleDragMouseOut, true);
            }
        }

        public void StartMove()
        {
            if (this.isMoving)
            {
                return;
            }

            this.isMoving = true;
            Style elementStyle = this.element.Style;

            this.originalHeight = elementStyle.Height;
            this.originalLeft = elementStyle.Left;
            this.originalTop = elementStyle.Top;
            this.originalWidth = elementStyle.Width;
            this.originalPosition = elementStyle.Position;

            ClientRect rect = ControlUtilities.GetBoundingRect(this.element);

            this.parentElement = this.element.ParentNode;

            this.beforeInParentElement = null;

            int childNodeCount = this.parentElement.ChildNodes.Length;

            for (int i = 0; i < childNodeCount; i++ )
            {
                Element e = this.parentElement.ChildNodes[i];

                if (e == this.element && i < childNodeCount-1)
                {
                    this.beforeInParentElement = this.parentElement.ChildNodes[i + 1];
                }
            }

            this.placeHolderElement = Document.CreateElement("DIV");
            this.placeHolderElement.InnerHTML = "&#160;";
            this.placeHolderElement.Style.Width = (rect.Right - rect.Left) + "px";
            this.placeHolderElement.Style.Height = ((rect.Bottom - rect.Top) + 4) + "px";

            this.parentElement.RemoveChild(this.element);
            this.parentElement.InsertBefore(this.placeHolderElement, beforeInParentElement);

            this.element.Style.Position = "absolute";

            ClientRect parentRect = ControlUtilities.GetBoundingRect(this.parentElement);

            this.left = parentRect.Left + Window.PageXOffset;
            this.top = parentRect.Top + Window.PageYOffset;

            elementStyle.Left = left + "px";
            elementStyle.Top = top + "px";

            this.width = (rect.Right - rect.Left);
            this.height = (rect.Bottom - rect.Top);

            if (this.overrideWidth != null)
            {
                elementStyle.Width = ((double)this.overrideWidth) + "px";
            }
            else
            {
                elementStyle.Width = width + "px";
            }

            if (this.overrideHeight != null)
            {
                elementStyle.Height = ((double)this.overrideHeight) + "px";
            }
            else
            {
                elementStyle.Height = height + "px";
            }

            elementStyle.ZIndex = 255;

            Document.Body.AppendChild(this.element);

            this.originLeft = this.Left;
            this.originTop = this.Top;
        }

        public void Animate(ElementAnimationType animationType, ElementAnimationEndBehavior endBehavior, int millisecondsLength)
        {
            this.endBehavior = endBehavior;
            isAnimating = true;

            bool wasMoving = this.isMoving;

            this.StartMove();

            if (wasMoving)
            {
                animationStartTop = this.Top;
                animationStartLeft = this.Left;
            }

            switch (animationType)
            {
                case ElementAnimationType.LeftInToPos:
                    animationStartTop = this.Top;
                    if (!wasMoving)
                    {
                        animationStartLeft = -this.EffectiveWidth;
                    }

                    animationEndTop = this.OriginTop;
                    animationEndLeft = this.OriginLeft;
                    break;

                case ElementAnimationType.RightInToPos:
                    if (!wasMoving)
                    {
                        animationStartTop = this.Top;
                        animationStartLeft = Document.Body.OffsetWidth;
                    }

                    animationEndTop = this.OriginTop;
                    animationEndLeft = this.OriginLeft;
                    break;

                case ElementAnimationType.PosOutToLeft:

                    if (!wasMoving)
                    {
                        animationStartTop = this.Top;
                        animationStartLeft = this.Left;
                    }

                    animationEndTop = this.OriginTop;
                    animationEndLeft = -this.EffectiveWidth;
                    break;

                case ElementAnimationType.PosOutToRight:
                    if (!wasMoving)
                    {
                        animationStartTop = this.Top;
                        animationStartLeft = this.Left;
                    }

                    animationEndTop = this.OriginTop;
                    animationEndLeft = Document.Body.OffsetWidth;
                    break;
            }


            this.animationStart = Date.Now;
            this.animationLength = millisecondsLength;

            AddToAnimationList(this);
        }

        private void EndAnimation()
        {
            isAnimating = false;

            RemoveAnimationList(this);

            if (this.endBehavior == ElementAnimationEndBehavior.ResetToPosition)
            {
                this.EndMove();
            }
            else if (this.endBehavior == ElementAnimationEndBehavior.HideAndResetToPosition)
            {
                this.element.Style.Display = "none";
                this.EndMove();
            }

            if (this.AnimationComplete != null)
            {
                this.AnimationComplete(this, EventArgs.Empty);
            }
        }

        public void UpdateAnimation()
        {
            if (!this.isAnimating)
            {
                return;
            }

            Date time = Date.Now;

            double curPosition = time.GetTime() - this.animationStart.GetTime();

            double proportion = curPosition  / this.animationLength;

            if (proportion >= 1)
            {
                this.EndAnimation();
            }
            else
            {
                double newLeft = animationStartLeft + ((animationEndLeft - animationStartLeft) * proportion);
                double newTop = animationStartTop + ((animationEndTop - animationStartTop) * proportion);

                if (!this.isAnimating)
                {
                    return;
                }

                this.Left = newLeft;
                this.Top = newTop;
            }
        }

        public static void RemoveAnimationList(ElementEffects mover)
        {
            animationList.Remove(mover);

            if (animationList.Count == 0)
            {
                animationList = null;
            }
        }
        public static void AddToAnimationList(ElementEffects mover)
        {
            bool startAnimating = false;

            if (animationList == null)
            {
                animationList = new List<ElementEffects>();
                isAnimatingAtLeastOne = false;
                startAnimating = true;
            }

            animationList.Add(mover);

            if (startAnimating)
            {
                AnimationTick();
            }
        }

        public static void AnimationTick()
        {           
            if (animationList == null)
            {
                return;
            }

            if (!isAnimatingAtLeastOne)
            {
                isAnimatingAtLeastOne = true;

                foreach (ElementEffects mover in animationList)
                {
                    mover.UpdateAnimation();
                }

                isAnimatingAtLeastOne = false;
            }

            Script.Literal("if (window.requestAnimationFrame) {{window.requestAnimationFrame(BL.UI.ElementEffects.animationTick);}}else{{window.setTimeout(BL.UI.ElementEffects.animationTick, 20);}}");
        }

        public void EndMove()
        {
            if (!this.isMoving)
            {
                return;
            }

            this.isMoving = false;

            if (this.isAnimating)
            {
                this.EndAnimation();
            }

           Style elementStyle = this.element.Style;

           elementStyle.Left = this.originalLeft;
           elementStyle.Top = this.originalTop;
           elementStyle.Width = this.originalWidth;
           elementStyle.Height = this.originalHeight;
           elementStyle.Position = this.originalPosition;

           Document.Body.RemoveChild(this.element);

           this.parentElement.InsertBefore(this.element, this.placeHolderElement);
           this.parentElement.RemoveChild(this.placeHolderElement);
        }
    }
}
