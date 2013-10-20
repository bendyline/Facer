using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using BL.UI;
using jQueryApi;
using System.Runtime.CompilerServices;

namespace BL.JQM
{
    public class Button : ElementContentControl
    {
        private bool? inline;
        private bool? mini;
        private jQueryUtilityFunction jq;
        private String icon;

        public override string TagName
        {
	        get 
	        { 
		         return "button";
	        }
        }

        [ScriptName("s_icon")]
        public String Icon
        {
            get
            {
                return "star";
            }

            set
            {
                this.icon = value;
            }
        }

        [ScriptName("s_iconpos")]
        public String IconPos
        {
            get
            {
                return "right";
            }

            set
            {
                this.icon = value;
            }
        }

        [ScriptName("b_inline")]
        public bool? Inline
        {
            get
            {
                return this.inline;
            }

            set
            {
                this.inline = value;

                this.Update();
            }
        }

        [ScriptName("b_mini")]
        public bool? Mini
        {
            get
            {
                return this.mini;
            }

            set
            {
                this.mini = value;

                this.Update();
            }
        }

        protected override void OnEnsureElements()
        {
            Element elt = this.Element;
            
            Script.Literal("var j = {0};j.buttonMarkup({2})", this.J, this.jq, this.GetObject());
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
        }

        private void HandleClick(jQueryEvent eventData)
        {
       /*     if (this.Clicked != null)
            {
                EventArgs ea = EventArgs.Empty;

                this.Clicked(this, ea);
            }*/
        }
    }
}
