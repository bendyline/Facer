/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using BL.UI;
using System.Html;

namespace BL
{
    public delegate void PointAndStartElementEventHandler(object sender, PointAndStartElementEventArgs e);

    public class PointAndStartElementEventArgs : EventArgs
    {
        private double startX;
        private double startY;
        private double x;
        private double y;
        private Element element;

        public double StartX
        {
            get
            {
                return this.startX;
            }

            set
            {
                this.startX = value;
            }
        }
        public double StartY
        {
            get
            {
                return this.startY;
            }

            set
            {
                this.startY = value;
            }
        }

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

        public bool MovedLeft
        {
            get
            {
                return this.x > this.startX;
            }
        }
        public bool MovedUp
        {
            get
            {
                return this.y > this.startY;
            }
        }

        public bool MovedRight
        {
            get
            {
                return this.x < this.startX;
            }
        }

        public bool MovedDown
        {
            get
            {
                return this.y < this.startY;
            }
        }

        public PointAndStartElementEventArgs(double startX, double startY, double x, double y, Element element)
        {
            this.x = x;
            this.y = y;

            this.element = element;
        }       
    }
}
