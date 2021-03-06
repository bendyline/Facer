﻿/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Html.Media.Graphics;

namespace BL.Gr
{
    public abstract class GraphicsElement
    {
        private double x;
        private double y;
        private Layer layer;

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

        public Layer Layer
        {
            get
            {
                return this.layer;
            }

            set
            {
                this.layer = value;
            }
        }

        public abstract void Draw(CanvasContext2D context);
    }
}
