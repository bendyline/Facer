/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BL.UI
{
    public class ContentItemSet
    {
        public List<ContentItem> Items;

        public ContentItemSet()
        {
            this.Items = new List<ContentItem>();
        }
    }
}
