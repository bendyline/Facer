/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

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
