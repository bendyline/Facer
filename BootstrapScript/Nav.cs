// Class1.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using jQueryApi;
using BL.UI;

namespace BL.BS
{

    public class Nav : ItemsControl
    {
        public override string DefaultClass
        {
            get
            {
                return "nav nav-pills";
            }
        }

        public override string TagName
        {
            get
            {
                return "ul";
            }
        }
    }
}
