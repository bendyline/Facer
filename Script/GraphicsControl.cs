using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using BL.Gr;

namespace BL.UI
{
    public class GraphicsControl : Control
    {
        private DrawingContext drawingContext;

        public DrawingContext DrawingContext
        {
            get
            {
                if (this.drawingContext == null)
                {
                    this.EnsureElements();
                }

                return this.drawingContext;
            }
        }

        public override String TypeId
        {
            get
            {
                return "ui-gr";
            }
        }

        public GraphicsControl()
        {
        }

        protected override void OnEnsureElements()
        {            
            this.drawingContext = new DrawingContext(this.Element);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            this.drawingContext.Update();
        }
    }
}
