// NavbarSection.cs
//

using System;
using System.Collections.Generic;
using BL.UI;

namespace BL.BS
{
    public class NavbarLinkSection : NavbarSectionBase
    {
        public override string DefaultClass
        {
            get
            {
                return "nav navbar-nav navbar-left";
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
