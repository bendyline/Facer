/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using BL.Extern;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Html;

namespace BL.UI
{
    public class HeightAnimator
    {
        private double? from;
        private Element element;
        private List<Element> elements;

        private double? to;

        private Date start = Date.Empty;
        private double length;
        private int delay;
        private bool canceled = false;

        private Operation heightOperation;
        public event EventHandler HeightChangeComplete;

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
                isRunning = true;
            }

            this.start = Date.Empty;
            this.delay = delayInMilliseconds;
            this.canceled = false;

            if (this.heightOperation == null)
            {
                this.heightOperation = new Operation();
            }

            if (callback != null)
            {
                this.heightOperation.AddCallback(callback, state);
            }

            this.SetToStartValues();

            if (!isRunning)
            {
                this.AnimateTick();
            }
        }

        private void SetToStartValues()
        {
            if (this.Element != null)
            {
                this.Element.Style.Height = Math.Floor(this.from) + "px";
                this.Element.Style.MinHeight = this.Element.Style.Height;
                this.Element.Style.MaxHeight = this.Element.Style.Height;
            }

            if (this.elements != null)
            {
                foreach (Element e in this.elements)
                {
                    e.Style.Height = Math.Floor(this.from) + "px";
                    e.Style.MinHeight = this.Element.Style.Height;
                    e.Style.MaxHeight = this.Element.Style.Height;
                }
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
                this.SetToStartValues();
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
                        this.Element.Style.Height = Math.Floor(this.from + ((this.to - this.from) * proportion)) + "px";
                        this.Element.Style.MinHeight = this.Element.Style.Height;
                        this.Element.Style.MaxHeight = this.Element.Style.Height;
                    }

                    if (this.elements != null)
                    {
                        foreach (Element e in this.elements)
                        {
                            e.Style.Height = Math.Floor(this.from + ((this.to - this.from) * proportion)) + "px";
                            e.Style.MinHeight = e.Style.Height;
                            e.Style.MaxHeight = e.Style.Height;
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

                if (this.heightOperation != null)
                {
                    this.heightOperation.CompleteAsAsyncDone(this);
                    this.heightOperation = null;
                }

                this.start = Date.Empty;

                if (this.HeightChangeComplete != null)
                {
                    this.HeightChangeComplete(this, EventArgs.Empty);
                }
            }
        }

        private void SetToEndValues()
        {
            if (this.Element != null)
            {
                this.Element.Style.Height = Math.Floor(this.to) + "px";
            }

            if (this.elements != null)
            {
                foreach (Element e in this.elements)
                {
                    e.Style.Height = Math.Floor(this.to) + "px";
                }
            }

            this.start = Date.Empty;
        }
    }
}
