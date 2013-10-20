// ContentControl.cs
//

using System;
using System.Collections.Generic;
using BL.UI;
using System.Runtime.CompilerServices;

namespace BL
{
    public class ElementContentControl : Control
    {
        private String text;

        [ScriptName("s_text")]
        public String Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.text = value;

                this.Update();
            }
        }

        protected override void OnEnsureElements()
        {
            base.OnEnsureElements();

        }

        protected override void  OnUpdate()
        {
            if (this.text != null)
            {
                this.Element.InnerText = this.text;
            }
        }
    }
}
