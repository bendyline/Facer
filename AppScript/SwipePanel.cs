/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */
 
using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;

namespace BL.UI.App
{
    public class SwipePanel : ItemsControl
    {
        private const int animationLength = 400;
        private int activeIndex = 0;
        private int previousIndex = 0;

        [ScriptName("e_topAreaOuter")]
        private Element topAreaOuter;

        private int leftIndex = 0;
        private int rightIndex = 0;

        private bool allowSwiping = true;
        private bool isAnimating;
        private bool isAnimatingInFromRight;

        private int animationStepCount;

        private List<String> linkTitles;

        [ScriptName("e_containerLinkBin")]
        private Element containerLinkBin;

        [ScriptName("e_containerLinkLeft")]
        private Element containerLinkLeft;

        [ScriptName("e_containerLinkRight")]
        private Element containerLinkRight;

        private ElementEffects previousElementAnimator;
        private ElementEffects activeElementAnimator;
        private ElementEffects leftElementAnimator;
        private ElementEffects rightElementAnimator;

        public event ControlIntegerEventHandler ActiveControlChanged;

        public Element ContainerLinkLeft
        {
            get
            {
                return this.containerLinkLeft;
            }
        }

        public Element ContainerLinkRight
        {
            get
            {
                return this.containerLinkRight;
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

                ElementAnimationType secondaryAnimationType = ElementAnimationType.PosOutToLeft;
                ElementAnimationType primaryAnimationType = ElementAnimationType.RightInToPos;

                this.previousIndex = this.activeIndex;

                this.activeIndex = value;

                if (this.activeIndex < this.previousIndex)
                {
                    secondaryAnimationType = ElementAnimationType.PosOutToRight;
                    primaryAnimationType = ElementAnimationType.LeftInToPos;
                }

                this.previousElementAnimator = this.ItemControls[this.previousIndex].Effects;
                this.previousElementAnimator.AnimationComplete += secondaryElementAnimator_AnimationComplete;
                this.previousElementAnimator.Animate(secondaryAnimationType, ElementAnimationEndBehavior.HideAndResetToPosition, animationLength);

                this.activeElementAnimator = this.ItemControls[this.activeIndex].Effects;

                this.activeElementAnimator.OverrideHeight = this.previousElementAnimator.Height;
                this.activeElementAnimator.OverrideWidth = this.previousElementAnimator.Width;

                this.activeElementAnimator.Animate(primaryAnimationType, ElementAnimationEndBehavior.ResetToPosition, animationLength);

                this.ApplyVisibility();

                if (this.ActiveControlChanged != null)
                {
                    ControlIntegerEventArgs ciea = new ControlIntegerEventArgs(this.ItemControls[this.activeIndex], this.activeIndex);

                    this.ActiveControlChanged(this, ciea);
                }

                this.UpdateLinkHighlights();
            }
        }

        private void secondaryElementAnimator_AnimationComplete(object sender, EventArgs e)
        {
            this.activeElementAnimator.EndMove();
            this.previousIndex = -1;
            this.ApplyVisibility();
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

            int i=0;

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
                this.previousIndex = -1;
                this.previousElementAnimator.EndMove();
                this.activeElementAnimator.EndMove();
                this.ApplyVisibility();
            }

            this.previousElementAnimator.Left -= 20;

            this.activeElementAnimator.Left -= 20;
        }

        protected override void OnItemControlAdded(Control c)
        {
            base.OnItemControlAdded(c);

            this.HandleControlEvents(c);

            this.ApplyVisibility();
        }

        private void Effects_DragMoved(object sender, PointAndStartElementEventArgs e)
        {
            if (this.leftElementAnimator != null)
            {
                this.leftElementAnimator.Left = this.activeElementAnimator.Left - (double)this.leftElementAnimator.EffectiveWidth;
            }
            if (this.rightElementAnimator != null)
            {
                this.rightElementAnimator.Left = this.activeElementAnimator.Left + (double)this.rightElementAnimator.EffectiveWidth;
            }
        }

        private void Effects_DragStart(object sender, PointAndStartElementEventArgs e)
        {
            this.activeElementAnimator = this.ItemControls[this.activeIndex].Effects;

            if (this.activeIndex > 0)
            {
                this.leftIndex = this.activeIndex - 1;
                this.ItemControls[this.leftIndex].Visible = true;
                this.leftElementAnimator = this.ItemControls[this.leftIndex].Effects;
                this.leftElementAnimator.OverrideHeight = this.activeElementAnimator.Height;
                this.leftElementAnimator.OverrideWidth = this.activeElementAnimator.Width;
                this.leftElementAnimator.Left = this.activeElementAnimator.Left - (double)this.leftElementAnimator.EffectiveWidth;

                this.leftElementAnimator.StartMove();

            }
            else
            {
                this.leftIndex = -1;
                this.leftElementAnimator = null;
            }

            if (this.activeIndex < this.ItemControls.Count - 1)
            {
                this.rightIndex = this.activeIndex + 1;
                this.ItemControls[this.rightIndex].Visible = true;
                this.rightElementAnimator = this.ItemControls[this.rightIndex].Effects;
                this.rightElementAnimator.OverrideHeight = this.activeElementAnimator.Height;
                this.rightElementAnimator.OverrideWidth = this.activeElementAnimator.Width;
                this.rightElementAnimator.Left = this.activeElementAnimator.Left + (double)this.rightElementAnimator.EffectiveWidth;

                this.rightElementAnimator.StartMove();

            }
            else
            {
                this.rightIndex = -1;
                this.rightElementAnimator = null;
            }
        }

        private void Effects_DragComplete(object sender, PointAndStartElementEventArgs e)
        {

            ElementEffects eff =  this.ItemControls[this.activeIndex].Effects;
           
            if (e.MovedRight && this.activeIndex < this.ItemControls.Count - 1)
            {
                this.previousIndex = this.activeIndex;

                if (this.leftElementAnimator != null)
                {
                    this.leftElementAnimator.EndMove();
                    this.ItemControls[this.leftIndex].Visible = false;
                }

                this.activeIndex++;

                this.previousElementAnimator = this.ItemControls[this.previousIndex].Effects;
                this.previousElementAnimator.AnimationComplete += secondaryElementAnimator_AnimationComplete;
                this.previousElementAnimator.Animate(ElementAnimationType.PosOutToLeft, ElementAnimationEndBehavior.HideAndResetToPosition, animationLength);

                this.activeElementAnimator = this.ItemControls[this.activeIndex].Effects;

                this.activeElementAnimator.OverrideHeight = this.previousElementAnimator.Height;
                this.activeElementAnimator.OverrideWidth = this.previousElementAnimator.Width;
                this.activeElementAnimator.Behavior = ElementBehavior.DragHorizontal;

                this.activeElementAnimator.Animate(ElementAnimationType.RightInToPos, ElementAnimationEndBehavior.ResetToPosition, animationLength);

                this.ApplyVisibility();

                if (this.ActiveControlChanged != null)
                {
                    ControlIntegerEventArgs ciea = new ControlIntegerEventArgs(this.ItemControls[this.activeIndex], this.activeIndex);

                    this.ActiveControlChanged(this, ciea);
                }

                this.UpdateLinkHighlights();

            }
            else if (e.MovedLeft && this.activeIndex > 0)
            {
                this.previousIndex = this.activeIndex;


                if (this.rightElementAnimator != null)
                {
                    this.rightElementAnimator.EndMove();
                    this.ItemControls[this.rightIndex].Visible = false;
                }

                this.activeIndex--;

                this.previousElementAnimator = this.ItemControls[this.previousIndex].Effects;
                this.previousElementAnimator.AnimationComplete += secondaryElementAnimator_AnimationComplete;
                this.previousElementAnimator.Animate(ElementAnimationType.PosOutToRight, ElementAnimationEndBehavior.HideAndResetToPosition, animationLength);

                this.activeElementAnimator = this.ItemControls[this.activeIndex].Effects;

                this.activeElementAnimator.OverrideHeight = this.previousElementAnimator.Height;
                this.activeElementAnimator.OverrideWidth = this.previousElementAnimator.Width;
                this.activeElementAnimator.Behavior = ElementBehavior.DragHorizontal;

                this.activeElementAnimator.Animate(ElementAnimationType.LeftInToPos, ElementAnimationEndBehavior.ResetToPosition, animationLength);

                this.ApplyVisibility();

                if (this.ActiveControlChanged != null)
                {
                    ControlIntegerEventArgs ciea = new ControlIntegerEventArgs(this.ItemControls[this.activeIndex], this.activeIndex);

                    this.ActiveControlChanged(this, ciea);

                    this.UpdateLinkHighlights();
                }
            }
            else
            {
                eff.EndMove();

                if (this.leftElementAnimator != null)
                {
                    this.leftElementAnimator.EndMove();
                }

                if (this.rightElementAnimator != null)
                {
                    this.rightElementAnimator.EndMove();
                }
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            foreach (Control c in this.ItemControls)
            {
                this.HandleControlEvents(c);
            }

            this.UpdateLinkBin();
        }

        private void HandleControlEvents(Control c)
        {
             if (c.Effects.Behavior != ElementBehavior.DragHorizontal && this.allowSwiping)
            {
                c.Effects.Behavior = ElementBehavior.DragHorizontal;
                c.Effects.DragMoved += Effects_DragMoved;
                c.Effects.DragStart += Effects_DragStart;
                c.Effects.DragComplete += Effects_DragComplete;
            }
             else if (!this.allowSwiping && c.Effects.Behavior == ElementBehavior.DragHorizontal)
             {
                 c.Effects.Behavior = ElementBehavior.None;
                 c.Effects.DragMoved -= Effects_DragMoved;
                 c.Effects.DragStart -= Effects_DragStart;
                 c.Effects.DragComplete -= Effects_DragComplete;
             }
        }

        private void ApplyVisibility()
        {
            int index = 0;

            foreach (Control c in this.ItemControls)
            {
                if (index == activeIndex || index == previousIndex)
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
