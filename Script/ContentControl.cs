/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using BL.UI;

namespace BL
{
    public class ContentControl : Control
    {
        private String text;

        public String Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.text = value;
            }
        }
    }
}
