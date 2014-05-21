/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;

namespace BL.UI
{
    public static class ControlUtilities
    {

        public static ClientRect GetBodyBoundingRect(Element e)
        {
            ClientRect cr = null;

            Script.Literal("{0}={1}.getBoundingClientRect(); var bcr=document.body.getBoundingClientRect();{0}.top-=bcr.top;", cr, e);

            return cr;
        }

        public static double GetPageX(ElementEvent e)
        {
            double pageX = 0;

            Script.Literal("{0}={1}.pageX", pageX, e);

            return pageX;
        }

        public static double GetPageY(ElementEvent e)
        {
            double pageY = 0;

            Script.Literal("{0}={1}.pageY", pageY, e);

            return pageY;
        }

        public static ClientRect GetBoundingRect(Element e)
        {
            ClientRect cr = null;

            Script.Literal("{0}={1}.getBoundingClientRect()", cr, e);

            return cr;
        }

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
