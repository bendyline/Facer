/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BL.UI.App
{
    public class PaneSettings
    {
        private bool fitToHeight = true;
        private bool visible = true;
        private bool fitToWidth = true;

        public bool Visible
        {
            get
            {
                return this.visible;
            }

            set
            {
                this.visible = value;
            }
        }

        public bool FitToHeight
        {
            get
            {
                return this.fitToHeight;
            }

            set
            {
                this.fitToHeight = value;
            }
        }

        public bool FitToWidth
        {
            get
            {
                return this.fitToWidth;
            }

            set
            {
                this.fitToWidth = value;
            }
        }
    }
}
