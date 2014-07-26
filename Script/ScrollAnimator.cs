/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using BL.Extern;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Html;

namespace BL.UI
{
    public class ScrollAnimator
    {
        private double? fromY;
        private double? toY;
        private double? fromX;
        private double? toX;

        private Date start;
        private double length;

        private Operation scrollingOperation;
        public event EventHandler ScrollingComplete;

        public double? FromY
        {
            get
            {
                return this.fromY;
            }

            set
            {
                this.fromY = value;
            }
        }


        public double? ToY
        {
            get
            {
                return this.toY;
            }

            set
            {
                this.toY = value;
            }
        }

        public double? FromX
        {
            get
            {
                return this.fromX;
            }

            set
            {
                this.fromX = value;
            }
        }

        public double? ToX
        {
            get
            {
                return this.toX;
            }

            set
            {
                this.toX = value;
            }
        }

        public void Start(double length, AsyncCallback callback, object state)
        {
            this.length = length;
            this.start = Date.Empty;

            this.scrollingOperation = new Operation();

            if (callback != null)
            {
                this.scrollingOperation.AddCallback(callback, state);
            }

            ControlUtilities.AnimateOnNextFrame(this.AnimateTick);
        }

        private void AnimateTick()
        {
            Date now = Date.Now;

            if (this.start == Date.Empty)
            {
                this.start = now;
            }

            int ms = now.GetTime() - this.start.GetTime();

            double proportion = Easing.EaseInQuad(ms, 0, 1, length);

            if (proportion < 1)
            {
                if (this.fromY != null && this.toY != null)
                {
                    Window.Scroll(Window.PageXOffset, (int)(this.fromY + ((this.toY - this.fromY) * proportion)));
                }

                ControlUtilities.AnimateOnNextFrame(this.AnimateTick);
            }
            else
            {
                if (this.toY != null)
                {
                    Window.Scroll(Window.PageXOffset, (int)this.toY);
                }

                this.scrollingOperation.CompleteAsAsyncDone(this);
                this.scrollingOperation = null;

                if (this.ScrollingComplete != null)
                {
                    this.ScrollingComplete(this, EventArgs.Empty);
                }
            }
        }
    }
}
