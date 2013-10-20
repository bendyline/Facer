// Class1.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using jQueryApi;

namespace BL.BS
{

    public class Button : ElementContentControl
    {
        public override string DefaultClass
        {
            get
            {
                return "btn btn-default";
            }
        }

        public override string TagName
        {
            get
            {
                return "button";
            }
        }
        protected override void OnEnsureElements()
        {
//            Script.Literal("var j = {0}; j.kendoDropDownList(); {1} = j.data('kendoDropDownlist')", this.J, this.dropDownList);

   //         this.dropDownList.Bind("change", this.HandleDateChange);
        }

    }
}
