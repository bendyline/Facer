/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using BL.Extern;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Html;

namespace BL.UI
{
    public class OpacityAnimator
    {
        private double? from;
        private Element element;
        private List<Element> elements;

        private double? to;

        private Date start = Date.Empty;
        private double length;
        private int delay;
        private bool canceled = false;

        private Operation opacityOperation;
        public event EventHandler OpacityChangeComplete;

        public Element Element
        {
            get
            {
                return this.element;
            }

            set
            {
                this.element = value;
            }
        }

        public List<Element> Elements
        {
            get
            {
                if (this.elements == null)
                {
                    this.elements = new List<Element>();
                }

                return this.elements;
            }
        }

        public double? From
        {
            get
            {
                return this.from;
            }

            set
            {
                this.from = value;
            }
        }


        public double? To
        {
            get
            {
                return this.to;
            }

            set
            {
                this.to = value;
            }
        }

        public void Cancel(bool setToEndValues)
        {
            this.canceled = true;

            if (setToEndValues)
            {
                this.SetToEndValues();
            }
        }

        public void StartAfter(int delayInMilliseconds, double lengthInMilliseconds, AsyncCallback callback, object state)
        {
            this.length = lengthInMilliseconds;

            bool isRunning = false;

            // are we already in an animation?  If so, avoid using the delay, and just start the animation.
            if (this.start != Date.Empty)
            {
                delayInMilliseconds = 0;
                isRunning = true;
            }

            this.start = Date.Empty;
            this.delay = delayInMilliseconds;
            this.canceled = false;

            if (this.opacityOperation == null)
            {
                this.opacityOperation = new Operation();
            }

            if (callback != null)
            {
                this.opacityOperation.AddCallback(callback, state);
            }

            if (this.Element != null)
            {
                this.Element.Style.Opacity = this.from.ToString();
            }

            if (this.elements != null)
            {
                foreach (Element e in this.elements)
                {
                    e.Style.Opacity = this.from.ToString();
                }
            }

            if (!isRunning)
            {
                this.AnimateTick();
            }
        }

        public void Start(double lengthInMilliseconds, AsyncCallback callback, object state)
        {
            this.StartAfter(0, lengthInMilliseconds, callback, state);
        }

        private void AnimateTick()
        {
            Date now = Date.Now;

            if (this.start == Date.Empty)
            {
                this.start = now;
            }

            if (this.canceled)
            {
                return;
            }

            int ms = (now.GetTime() - this.start.GetTime());

            if (ms <  delay)
            {
                ElementUtilities.AnimateOnNextFrame(this.AnimateTick);
                return;
            }

            ms -= delay;

            double proportion = Easing.EaseInQuad(ms, 0, 1, length);

            if (proportion < 1)
            {
                if (this.from != null && this.to != null)
                {
                    if (this.canceled)
                    {
                        return;
                    } 
                    
                    if (this.Element != null)
                    {
                        this.Element.Style.Opacity = (this.from + ((this.to - this.from) * proportion)).ToString();
                    }

                    if (this.elements != null)
                    {
                        foreach (Element e in this.elements)
                        {
                            e.Style.Opacity = (this.from + ((this.to - this.from) * proportion)).ToString();                            
                        }
                    }
                }
                
                ElementUtilities.AnimateOnNextFrame(this.AnimateTick);
            }
            else
            {
                if (this.to != null)
                {
                    if (this.canceled)
                    {
                        return;
                    }

                    this.SetToEndValues();
                }

                if (this.opacityOperation != null)
                {
                    this.opacityOperation.CompleteAsAsyncDone(this);
                    this.opacityOperation = null;
                }

                this.start = Date.Empty;

                if (this.OpacityChangeComplete != null)
                {
                    this.OpacityChangeComplete(this, EventArgs.Empty);
                }
            }
        }

        private void SetToEndValues()
        {
            if (this.Element != null)
            {
                this.Element.Style.Opacity = this.to.ToString();
            }

            if (this.elements != null)
            {
                foreach (Element e in this.elements)
                {
                    e.Style.Opacity = this.to.ToString();
                }
            }

            this.start = Date.Empty;
        }
    }
}
