/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;

namespace BL.UI
{
    public interface IControl
    {
        [ScriptName("b_visible")]
        bool Visible { get; set; }
  
        [ScriptName("s_templateId")] 
        String TemplateId { get; set; }
        Element Element { get; set; }

        void EnsureElements();
    }
}
