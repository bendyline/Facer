// Class1.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using jQueryApi;

namespace BL.BS
{

    public class NavbarButton : ElementContentControl
    {
        public override string DefaultClass
        {
            get
            {
                return "btn btn-default navbar-btn";
            }
        }

        public override string TagName
        {
            get
            {
                return "button";
            }
        }

    }
}
