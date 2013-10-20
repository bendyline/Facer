/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

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
