// Class1.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using System.Html.Media.Graphics;

namespace BL.Gr
{
    public class DrawingContext
    {
        private Element canvasContainer;
        private int width;
        private int height;
        private Layer baseLayer;

        public Layer Base
        {
            get
            {
                return this.baseLayer;
            }

            set
            {
                this.baseLayer = value;
            }
        }

        public int Width
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

        public int Height
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


        public List<Layer> layers;

        public Element CanvasContainer
        {
            get { return this.canvasContainer; }
            set { this.canvasContainer = value; }
        }

        public DrawingContext(Element canvasContainer)
        {
            this.canvasContainer = canvasContainer;

            this.layers = new List<Layer>();

            this.baseLayer = new Layer(this);
            this.layers.Add(this.baseLayer);
        }

        public void Update()
        {
            foreach (Layer layer in this.layers)
            {
                if (layer.NeedsUpdate)
                {
                    layer.Update();
                }
            }
          
        }
    }
}
