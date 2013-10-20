// IControlFactory.cs
//

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
