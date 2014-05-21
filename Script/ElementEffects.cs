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

        private double startLeft;
        private double startTop;
        private double endLeft;
        private double endTop;

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
        public event PointElementEventHandler DragComplete;
        public event PointElementEventHandler DragStart;
        public event PointElementEventHandler DragMoved;


        private ElementEventListener draggingElementMouseMoveHandler = null;
        private ElementEventListener draggingElementMouseUpHandler = null;

        private ElementAnimationEndBehavior endBehavior = ElementAnimationEndBehavior.ResetToPosition;

        private ElementBehavior behavior;

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

        public ElementEffects(Element element)
        {
            this.element = element;
        }

        public void ApplyBehavior()
        {
            if (this.behavior == ElementBehavior.DragHorizontal || this.behavior == ElementBehavior.DragVertical || this.behavior == ElementBehavior.Drag2D)
            {
                this.element.AddEventListener("mousedown", this.HandleElementMouseDown, true);
                this.element.AddEventListener("mouseup", this.HandleElementMouseUp, true);
            }
        }

        private void HandleElementMouseDown(ElementEvent e)
        {
            this.isMouseDown = true;

            this.initialMouseDownLeft = ControlUtilities.GetPageX(e);
            this.initialMouseDownTop = ControlUtilities.GetPageY(e);

            Window.SetTimeout(this.ConsiderStartDragging, 200);
        }

        private void HandleElementMouseUp(ElementEvent e)
        {
            this.isMouseDown = false;

            this.HandleDragMouseUpBehavior();
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

            if (this.DragMoved != null)
            {
                PointElementEventArgs ee = new PointElementEventArgs(this.Left, this.Top, this.element);

                this.DragMoved(this, ee);
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
            this.isDragging = false;


            if (this.DragComplete != null)
            {
                PointElementEventArgs ee = new PointElementEventArgs(this.Left, this.Top, this.element);

                this.DragComplete(this, ee);
            }

            Document.Body.RemoveEventListener("mousemove", this.draggingElementMouseMoveHandler, true);
            Document.Body.RemoveEventListener("mouseup", this.draggingElementMouseUpHandler, true);
        }

        private void ConsiderStartDragging()
        {
            if (!this.isMouseDown)
            {
                return;
            }

            this.StartMove();

            this.dragStartElementLeft = this.Left;
            this.dragStartElementTop = this.Top;

            if (this.DragStart != null)
            {
                PointElementEventArgs ee = new PointElementEventArgs(this.Left, this.Top, this.element);

                this.DragStart(this, ee);
            }

            this.isDragging = true;

            this.draggingElementMouseMoveHandler = this.HandleElementDragging;
            this.draggingElementMouseUpHandler = this.HandleDragMouseUp;

            Document.Body.AddEventListener("mousemove", this.draggingElementMouseMoveHandler, true);
            Document.Body.AddEventListener("mouseup", this.draggingElementMouseUpHandler, true);
        }

        public void StartMove()
        {
            this.isMoving = true;
            Style elementStyle = this.element.Style;

            this.originalHeight = elementStyle.Height;
            this.originalLeft = elementStyle.Left;
            this.originalTop = elementStyle.Top;
            this.originalWidth = elementStyle.Width;
            this.originalPosition = elementStyle.Position;

            ClientRect rect = ControlUtilities.GetBodyBoundingRect(this.element);

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

            ClientRect parentRect = ControlUtilities.GetBodyBoundingRect(this.parentElement);

            this.left = parentRect.Left;
            this.top = parentRect.Top;

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
        }

        public void Animate(ElementAnimationType animationType, ElementAnimationEndBehavior endBehavior, int millisecondsLength)
        {
            this.endBehavior = endBehavior;
            isAnimating = true;

            this.animationStart = Date.Now;
            this.animationLength = millisecondsLength;

            this.StartMove();

            switch (animationType)
            {
                case ElementAnimationType.LeftInToPos:
                    startTop = this.Top;
                    startLeft = -this.width;

                    endTop = this.Top;
                    endLeft = this.Left;
                    break;

                case ElementAnimationType.RightInToPos:
                    startTop = this.Top;
                    startLeft = Document.Body.OffsetWidth;

                    endTop = this.Top;
                    endLeft = this.Left;
                    break;

                case ElementAnimationType.PosOutToLeft:
                    startTop = this.Top;
                    startLeft = this.Left;

                    endTop = this.Top;
                    endLeft = -this.width;
                    break;

                case ElementAnimationType.PosOutToRight:
                    startTop = this.Top;
                    startLeft = this.Left;

                    endTop = this.Top;
                    endLeft = Document.Body.OffsetWidth;
                    break;
            }

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
                double newLeft = startLeft + ((endLeft - startLeft) * proportion);
                double newTop = startTop + ((endTop - startTop) * proportion);

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
