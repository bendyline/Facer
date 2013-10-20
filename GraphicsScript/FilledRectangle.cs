/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

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
