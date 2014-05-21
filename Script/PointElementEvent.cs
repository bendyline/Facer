/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using BL.UI;
using System.Html;

namespace BL
{
    public delegate void PointElementEventHandler(object sender, PointElementEventArgs e);

    public class PointElementEventArgs : EventArgs
    {
        private double x;
        private double y;
        private Element element;

        public double X
        {
            get
            {
                return this.x;
            }

            set
            {
                this.x = value;
            }
        }
        public double Y
        {
            get
            {
                return this.y;
            }

            set
            {
                this.y = value;
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
            }
        }

        public PointElementEventArgs(double x, double y, Element element)
        {
            this.x = x;
            this.y = y;

            this.element = element;
        }       
    }
}
