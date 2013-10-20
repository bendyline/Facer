/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;

namespace BL.UI
{
    public class BaseControlFactory : IControlFactory
    {
        public String Namespace 
        {
            get
            {
                return "BL.UI";
            }
        }

        public Control CreateControl(String controlName)
        {
            switch (controlName)
            {
            //    case "graphicscontrol":
           //         return new GraphicsControl();

                case "literalcontrol":
                    return new LiteralControl();
             }

            return null;
        }
    }
}
