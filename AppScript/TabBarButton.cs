/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BL.UI.App
{
    public class TabBarButton : ContentItemControl
    {
        public event EventHandler Clicked;
        private bool toggled;

        [ScriptName("b_toggled")]
        public bool Toggled
        {
            get
            {
                return this.toggled;
            }

            set
            {
                this.toggled = value;

                if (this.toggled)
                {
                    this.ClassName = this.TypeId + "-toggled";
                }
                else
                {
                    this.ClassName = null;
                }
            }
        }

        protected override void OnInit()
        {
            base.OnInit();

            this.TrackInteractionEvents = true;
        }

        protected override void OnClick(System.Html.ElementEvent e)
        {
            this.Toggled = !this.Toggled;

            base.OnClick(e);

            if (this.Clicked != null)
            {
                this.Clicked(this, EventArgs.Empty);
            }
        }
    }
}
