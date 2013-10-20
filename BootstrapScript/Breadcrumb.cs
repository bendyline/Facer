/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

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
