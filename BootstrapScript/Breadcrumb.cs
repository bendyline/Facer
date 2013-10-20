// NavbarSection.cs
//

using System;
using System.Collections.Generic;
using BL.UI;

namespace BL.BS
{
    public class Breadcrumb : ItemsControl
    {
        public override string DefaultClass
        {
            get
            {
                return "breadcrumb";
            }
        }

        public override string TagName
        {
            get
            {
                return "ol";
            }
        }
    }
}
