using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;

namespace BL.UI
{
    public static class ControlUtilities
    {

        public static void DisableElementTouchMove(Element e)
        {            
            e.AddEventListener("touchmove", CancelEvent, false);
        }

        public static void CancelEvent(ElementEvent eventDetails)
        {
            eventDetails.PreventDefault();
        }
    }

}
