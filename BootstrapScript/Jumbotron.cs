using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Html;
using BL.UI;

namespace BL.BS
{
    public class Jumbotron : ItemsControl
    {
        [ScriptName("e_title")]
        protected Element titleElement;

        [ScriptName("e_lead")]
        protected Element leadElement;

        [ScriptName("e_callToAction")]
        protected Element callToActionElement;

        private String title;
        private String lead;

        [ScriptName("s_title")]
        public String Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.title = value;
            }
        }

        [ScriptName("s_lead")]
        public String Lead
        {
            get
            {
                return this.lead;
            }

            set
            {
                this.lead = value;
            }
        }

        public override void OnApplyTemplate()
        {
            if (this.titleElement != null)
            {
                this.titleElement.InnerText = this.Title;
            }

            if (this.leadElement != null)
            {
                this.leadElement.InnerText = this.Lead;
            }
        }
    }
}
