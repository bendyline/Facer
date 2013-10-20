// Checkboxradio.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Html;
using BL.UI;

namespace BL.JQM
{
    public class Checkboxradio : Control
    {
        private bool? mini;
        private jQueryUtilityFunction jq;

        [ScriptName("e_label")]
        private Element label;

        [ScriptName("e_input")]
        private InputElement input;

        private String text;

        public override string TagName
        {
	        get 
	        { 
		         return "button";
	        }
        }

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
            
            Script.Literal("var j = {0};j.checkboxradio({2})", this.J, this.jq, this.GetObject());
        }


        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.label != null)
            {
                this.label.InnerText = this.text;
            }
        }

    }
}
