/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */
 
using System;
using System.Collections.Generic;
using System.Html;

namespace BL.UI.App
{
    public class SwipePanel : ItemsControl
    {
        private int activeIndex = 0;
        private int secondaryIndex = 0;

        private bool isAnimating;
        private bool isAnimatingInFromRight;

        private int animationStepCount;

        private ElementEffects secondaryElementAnimator;
        private ElementEffects primaryElementAnimator;

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
                    return;
                }

                this.secondaryIndex = this.activeIndex;

                this.activeIndex = value;

                if (this.activeIndex > this.secondaryIndex)
                {
                    isAnimatingInFromRight = true;
                }
                else
                {
                    isAnimatingInFromRight = false;
                }

                this.secondaryElementAnimator = this.ItemControls[this.secondaryIndex].Effects;
                this.secondaryElementAnimator.AnimationComplete += secondaryElementAnimator_AnimationComplete;
                this.secondaryElementAnimator.Animate(ElementAnimationType.PosOutToLeft, ElementAnimationEndBehavior.HideAndResetToPosition, 700);

                this.primaryElementAnimator = this.ItemControls[this.activeIndex].Effects;

                this.primaryElementAnimator.OverrideHeight = this.secondaryElementAnimator.Height;
                this.primaryElementAnimator.OverrideWidth = this.secondaryElementAnimator.Width;
                this.primaryElementAnimator.Behavior = ElementBehavior.Drag2D;

                this.primaryElementAnimator.Animate(ElementAnimationType.RightInToPos, ElementAnimationEndBehavior.ResetToPosition, 700);

                this.ApplyVisibility();
            }
        }

        private void secondaryElementAnimator_AnimationComplete(object sender, EventArgs e)
        {
            this.primaryElementAnimator.EndMove();
            this.secondaryIndex = -1;
            this.ApplyVisibility();
        }

        private void Animate()
        {
            int curStep = animationStepCount;
            animationStepCount--;

            if (animationStepCount > 0)
            {
                Window.SetTimeout(this.Animate, 20);
            }
            else
            {
                this.secondaryIndex = -1;
                this.secondaryElementAnimator.EndMove();
                this.primaryElementAnimator.EndMove();
                this.ApplyVisibility();
            }

            this.secondaryElementAnimator.Left -= 20;

            this.primaryElementAnimator.Left -= 20;
        }

        protected override void OnItemControlAdded(Control c)
        {
            base.OnItemControlAdded(c);

            this.HandleControlEvents(c);

            this.ApplyVisibility();
        }

        private void Effects_DragMoved(object sender, PointElementEventArgs e)
        {
        }

        private void Effects_DragStart(object sender, PointElementEventArgs e)
        {
        }

        private void Effects_DragComplete(object sender, PointElementEventArgs e)
        {
        }

        protected override void OnEnsureElements()
        {
            base.OnEnsureElements();

            foreach (Control c in this.ItemControls)
            {
                this.HandleControlEvents(c);
            }
        }

        private void HandleControlEvents(Control c)
        {
            if (c.Element == null)
            {
                return;
            }

            if (this.ElementsEnsured)
            {
                c.Effects.Behavior = ElementBehavior.DragHorizontal;
                c.Effects.DragMoved += Effects_DragMoved;
                c.Effects.DragStart += Effects_DragStart;
                c.Effects.DragComplete += Effects_DragComplete;
            }
        }

        private void ApplyVisibility()
        {
            int index = 0;

            foreach (Control c in this.ItemControls)
            {
                if (index == activeIndex || index == secondaryIndex)
                {
                    c.Visible = true;
                }
                else
                {
                    c.Visible = false;
                }

                index++;
            }
        }
    }
}
