// IControlFactory.cs
//

using System;
using System.Collections.Generic;

namespace BL.UI.KendoControls
{
    public class KendoControlFactory : IControlFactory
    {
        public String Namespace 
        {
            get
            {
                return "BL.UI.KendoControls";
            }
        }

        public Control CreateControl(String controlName)
        {
            switch (controlName)
            {
                case "datepicker":
                    return new DatePicker();
             }

            return null;
        }
    }
}
