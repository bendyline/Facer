// Class1.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using jQueryApi;

namespace BL.BS
{

    public class NavbarText : ElementContentControl
    {
        public override string DefaultClass
        {
            get
            {
                return "navbar-text";
            }
        }

        public override string TagName
        {
            get
            {
                return "p";
            }
        }

    }
}
