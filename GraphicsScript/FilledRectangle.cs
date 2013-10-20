using System;
using System.Collections.Generic;
using System.Linq;
using System.Html.Media.Graphics;

namespace BL.Gr
{
    public class FilledRectangle : GraphicsElement
    {
        public override void Draw(CanvasContext2D context)
        {
            context.FillStyle = "green";
            context.FillRect(10, 10, 10, 10);
        }
    }
}
