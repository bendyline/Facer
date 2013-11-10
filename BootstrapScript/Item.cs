/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Html;
using jQueryApi;
using BL.UI;
using System.Runtime.CompilerServices;

namespace BL.BS
{

    public class Item : Control
    {
        private bool isActive;
        private bool isDisabled;
        private String text;
        private String href;

        [ScriptName("e_link")]
        private Element linkElement;

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

        [ScriptName("s_href")]
        public String Href
        {
            get
            {
                return this.href;
            }

            set
            {
                this.href = value;
                this.TrackInteractionEvents = true;

                this.Update();
            }
        }

        [ScriptName("b_active")]
        public bool Active
        {
            get
            {
                return this.isActive; 
            }

            set
            {
                this.isActive = value;

                this.UpdateClass();
            }
        }

        [ScriptName("b_disabled")]
        public bool IsDisabled
        {
            get
            {
                return this.isDisabled;
            }

            set
            {
                this.isDisabled = value;

                this.UpdateClass();
            }
        }

        public override string TagName
        {
            get
            {
                return "li";
            }
        }

        private void UpdateClass()
        {
            String className = "";

            if (this.isActive)
            {
                className += "active";
            }

            if (this.isDisabled)
            {
                if (className.Length > 0) 
                { 
                    className += " "; 
                }

                className += "disabled";
            }

            this.ClassName = className;
        }

        protected override void OnClick(ElementEvent e)
        {
            if (this.Href != null)
            {
                Window.Location.Assign(this.Href);
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.linkElement != null)
            {
                this.linkElement.InnerText = this.Text;
            }
        }
    }
}
