/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Diagnostics;

namespace BL.UI
{
    public static class ElementUtilities
    {
        public static void ClearChildElements(Element e)
        {
            while (e.ChildNodes.Length > 0)
            {
                e.RemoveChild(e.ChildNodes[0]);
            }
        }

        public static bool GetIsChecked(InputElement ie)
        {
            bool isChecked = false;

            Script.Literal("{0}={1}.checked", isChecked, ie);

            return isChecked;
        }

        public static void SetIsCheckedFromObject(InputElement ie, object isChecked)
        {
            Script.Literal("{0}.checked={1}", ie, isChecked.ToString());
        }

        public static void SetBackgroundSize(Element element, String value)
        {
            Script.Literal("{0}.style.backgroundSize={1}", element, value);
        }

        public static void SetIsChecked(InputElement ie, bool isChecked)
        {
            Script.Literal("{0}.checked={1}", ie, isChecked);
        }

        public static String GetTouchStartEventName()
        {
            Script.Literal("if (window.navigator.msPointerEnabled) { return \"MSPointerDown\"; }");

            return "touchstart";
        }

        public static String GetTouchMoveEventName()
        {
            Script.Literal("if (window.navigator.msPointerEnabled) { return \"MSPointerMove\"; }");

            return "touchmove";
        }

        public static String GetTouchEndEventName()
        {
            Script.Literal("if (window.navigator.msPointerEnabled) { return \"MSPointerUp\"; }");

            return "touchend";
        }

        public static String GetTouchCancelEventName()
        {
            Script.Literal("if (window.navigator.msPointerEnabled) { return null; }");

            return "touchcancel";
        }

        public static int? GetDimension(String dimension)
        {
            if (String.IsNullOrEmpty(dimension))
            {
                return null;
            }

            if (dimension.EndsWith("px"))
            {
                return Int32.Parse(dimension.Substring(0, dimension.Length - 2));
            }

            return null;
        }

        public static double GetPageX(ElementEvent e)
        {
            double pageX = 0;

            Script.Literal("if ({1}.touches != null) {{ {0}=e.touches[0].pageX; }} else {{ {0}={1}.pageX;}}", pageX, e);

            return pageX;
        }

        public static double GetPageY(ElementEvent e)
        {
            double pageY = 0;

            Script.Literal("if({1}.touches != null) {{ {0}=e.touches[0].pageY;}} else {{ {0}={1}.pageY;}}", pageY, e);

            return pageY;
        }

        public static ClientRect GetBoundingRect(Element e)
        {
            ClientRect cr = null;

            Script.Literal("{0}={1}.getBoundingClientRect()", cr, e);

            return cr;
        }

        public static void SetText(Element element, String text)
        {
            while (element.FirstChild != null)
            {
                element.RemoveChild(element.FirstChild);
            }

            element.AppendChild(Document.CreateTextNode(text));
        }


        public static void AnimateOnNextFrame(Action callback)
        {
            Script.Literal("if (window.requestAnimationFrame) {{window.requestAnimationFrame({0});}}else{{window.setTimeout({0}, 15);}}", callback);
        }
        public static Element GetEventTarget(ElementEvent e)
        {
            return e.Target;
        }

        public static bool RemoveIfChildOf(Element element, Element parentToLookFor)
        {
            if (!ElementIsChildOf(element, parentToLookFor))
            {
                return false;
            }

            parentToLookFor.RemoveChild(element);

            return true;
        }

        public static bool ElementIsChildOf(Element element, Element parentToLookFor)
        {
            for (int i = 0; i < parentToLookFor.Children.Length; i++)
            {
                if (parentToLookFor.Children[i]== element)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ElementIsDescendentOf(Element element, Element ancestorToLookFor)
        {
            if (element == ancestorToLookFor)
            {
                return true;
            }

            if (element.ParentNode != null)
            {
                return ElementIsDescendentOf(element.ParentNode, ancestorToLookFor);
            }

            return false;
        }

        public static double GetBoundingHeight(Element e)
        {
            double height = 0;

            Script.Literal("var cr={0}.getBoundingClientRect();return cr.bottom-cr.top", e);

            return height;
        }

        public static void DisableElementTouchMove(Element e)
        {            
            e.AddEventListener("touchmove", CancelEvent, false);
        }

        public static void CancelEvent(ElementEvent eventDetails)
        {

            Debug.WriteLine("ElementUtilities: Cancelling default pointer event action.");

            eventDetails.PreventDefault();
        }
    }

}
