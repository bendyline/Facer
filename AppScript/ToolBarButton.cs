/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;

namespace BL.UI.App
{
    public class ToolBarButton : ContentItemControl
    {
        public event EventHandler Clicked;

        protected override void OnInit()
        {
            base.OnInit();

            this.TrackInteractionEvents = true;
        }

        protected override void OnClick(System.Html.ElementEvent e)
        {
            base.OnClick(e);

            if (this.Clicked != null)
            {
                this.Clicked(this, EventArgs.Empty);
            }
        }
    }
}
