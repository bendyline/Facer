// ContentControl.cs
//

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
