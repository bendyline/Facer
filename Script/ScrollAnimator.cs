/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Html;

namespace BL.UI
{
    public class ScrollAnimator
    {
        private double fromY;
        private double toY;
        private Date start;
        private double length;

        public double FromY
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

        public double ToY
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

        public void Start(double length)
        {
            this.length = length;
            this.start = Date.Now;

            Window.SetTimeout(this.AnimateTick, 10);
        }

        private void AnimateTick()
        {
            Date now = Date.Now;

            int ms = now.GetTime() - this.start.GetTime();

            double proportion = ms / length;

            if (proportion < 1)
            {
                Window.Scroll(Window.PageXOffset, (int)  (this.fromY + ((this.toY - this.fromY) * proportion)));

                Window.SetTimeout(this.AnimateTick, 10);
            }
            else
            {
                Window.Scroll(Window.PageXOffset, (int)this.toY);
            }
        }
    }
}
