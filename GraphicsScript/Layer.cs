/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Html.Media.Graphics;
using System.Html;

namespace BL.Gr
{
    public class Layer
    {
        private CanvasContext2D bufferContext;
        private CanvasElement buffer;

        private CanvasContext2D visualContext;
        private CanvasElement visual;

        private bool needsUpdate = true;

        private DrawingContext context;

        private double x;
        private double y;
        private double width;
        private double height;

        private List<GraphicsElement> elements;

        public bool NeedsUpdate
        {
            get
            {
                return this.needsUpdate;
            }

            set
            {
                this.needsUpdate = value;
            }
        }

        public DrawingContext Context
        {
            get
            {
                return this.context;
            }

            set
            {
                this.context = value;
            }
        }

        internal CanvasContext2D BufferContext
        {
            get
            {
                return this.bufferContext;
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

        public double Width 
        {
            get
            {
                return this.width;
            }

            set
            {
                this.width = value;
            }
        }
        
        public double Height 
        {
            get
            {
                return this.height;
            }

            set
            {
                this.height = value;
            }
        }

        public Layer(DrawingContext context)
        {
            this.context = context;

            CanvasElement ce = (CanvasElement)Document.CreateElement("CANVAS");
            this.buffer = ce;
            this.Context.CanvasContainer.AppendChild(ce);

            this.bufferContext = (CanvasContext2D)this.buffer.GetContext("2d");

            this.elements = new List<GraphicsElement>();
        }

        public FilledRectangle CreateFilledRectangle()
        {
            FilledRectangle fr = new FilledRectangle();
            fr.Layer = this;

            this.elements.Add(fr);
            
            return fr;
        }

        public void Update()
        {
            CanvasContext2D canvasContext = this.BufferContext;

            foreach (GraphicsElement ge in this.elements)
            {
                ge.Draw(canvasContext);
            }
        }
    }
}
