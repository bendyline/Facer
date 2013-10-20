// IControlFactory.cs
//

using System;
using System.Collections.Generic;

namespace BL.UI
{
    public interface IControlFactory
    {
        String Namespace { get; }
        Control CreateControl(String controlName);
    }
}
