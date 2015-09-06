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
        private static Date lastScrollTime = new Date(2012, 1, 1);
        private static Date lastTimeUpdate = Date.Now;

        private static Element loadingElement;
        private static Dictionary<String, object> pendingOperations = new Dictionary<string, object>();

        public static void Init()
        {
            Window.SetInterval(UpdateTick, 50);
        }

        private static void UpdateTick()
        {
            Date now = Date.Now;

            // if the time interval is unexpectedly long, it could be because iOS
            // on versions < iOS8, and on Cordova and for web apps, pauses all
            // JS during scroll (and scroll events don't fire.). So treat long pauses as
            // essentially a  scroll
            if (now.GetTime() - lastTimeUpdate.GetTime() > 70)
            {
                lastScrollTime = now;
            }

            lastTimeUpdate = now;
        }

        public static void AddPendingOperation(String operationIdentifier)
        {
            pendingOperations[operationIdentifier] = true;


            ShowLoadingSpinner();
        }
        public static int GetPendingOperationCount()
        {
            int pendingOperationCount = 0;

            foreach (KeyValuePair<String, object> oper in pendingOperations)
            {
                if ((bool)oper.Value == true)
                {
                    pendingOperationCount++;
                }
            }

            return pendingOperationCount;
        }

        public static void RemovePendingOperation(String operationIdentifier)
        {
            pendingOperations[operationIdentifier] = false;

            if (GetPendingOperationCount() == 0)
            {
                HideLoadingSpinner();
            }
        }

        private static void ShowLoadingSpinner()
        {
            if (GetPendingOperationCount() == 0)
            {
                return;
            }

            if (loadingElement == null)
            {

                Element waitingElement = (Element)Document.CreateElement("div");
                Style outerStyle = waitingElement.Style;

                outerStyle.VerticalAlign = "middle";
                outerStyle.TextAlign = "center";
                outerStyle.Opacity = "0";
                outerStyle.Position = "fixed";
                outerStyle.Position = "fixed";

                ElementUtilities.SetBorderRadius(waitingElement, "4px");
                outerStyle.Border = "solid 1px #52bae4";
                outerStyle.BackgroundColor = "#52bae4";
                outerStyle.Width = "66px";
                outerStyle.Height = "66px";
                outerStyle.Padding = "10px";
            
                waitingElement.Style.Left = ((Context.Current.BrowserInnerWidth / 2) - 33) + "px";
                waitingElement.Style.Top = ((Context.Current.BrowserInnerHeight / 2) - 33) + "px";

                Element waitingInteriorElement = Document.CreateElement("I");
                waitingInteriorElement.ClassName = "fa fa-circle-o-notch fa-3x fa-spin";
                waitingInteriorElement.Style.Color = "white";

                waitingElement.AppendChild(waitingInteriorElement);

                Document.Body.AppendChild(waitingElement);

                loadingElement = waitingElement;
            }

            if (GetPendingOperationCount() == 0)
            {
                return;
            }

            loadingElement.Style.Display = "block";

            Window.SetTimeout(StartLoadingSpinnerAnimation, 100);
        }

        private static void HideLoadingSpinner()
        {
            if (loadingElement == null)
            {
                return;
            }

            loadingElement.Style.Display = "none";
            loadingElement.Style.Opacity = "0";
        } 

         private static void StartLoadingSpinnerAnimation()
        {
            if (loadingElement != null && GetPendingOperationCount() > 0)
            {
                loadingElement.Style.Opacity = ".8";
            }
        }

        public static bool HasScrolledRecently()
        {
            Date now = Date.Now;

            return (now.GetTime() - lastScrollTime.GetTime()) < 300;
        }

        public static void UpdateLastScrollTime()
        {
            lastScrollTime = Date.Now;
        }

        public static void RegisterTextInputBehaviorsEnterOnly(InputElement e)
        {
            e.AddEventListener("keyup", HandleInputTextKeyUp, true);
            e.AddEventListener("focus", HandleInputFocus, true);
        }

        public static void DeregisterTextInputBehaviorsEnterOnly(InputElement e)
        {
            e.RemoveEventListener("keyup", HandleInputTextKeyUp, true);
            e.RemoveEventListener("focus", HandleInputFocus, true);
        }

        public static void RegisterTextInputBehaviors(InputElement e)
        {
            e.AddEventListener("keyup", HandleInputTextKeyUp, true);
            e.AddEventListener("focus", HandleInputFocus, true);
        }

        public static void DeregisterTextInputBehaviors(InputElement e)
        {
            e.RemoveEventListener("keyup", HandleInputTextKeyUp, true);
            e.RemoveEventListener("focus", HandleInputFocus, true);
        }

        public static void RegisterTextAreaBehaviors(InputElement e)
        {
            e.AddEventListener("focus", HandleInputFocus, true);
        }

        public static void DeregisterTextAreaBehaviors(InputElement e)
        {
            e.RemoveEventListener("focus", HandleInputFocus, true);
        }

        private static void HandleInputFocus(ElementEvent e)
        {
            // if there is an onscreen keyboard, scroll the active text element up
            // so it remains visible (not hidden behind the on screen keyboard)
            if (Context.Current.IsOnscreenKeyboardDevice)
            {
                ClientRect elementRect = ElementUtilities.GetBoundingRect(e.Target);

                double offsetTop = elementRect.Top;

                double invisibleTop = Window.InnerHeight - (Context.Current.OnScreenKeyboardHeight + 40);

                if (offsetTop > invisibleTop)
                {
                    // find the first parent element that is scrollable.
                    Element scrollableParent = e.Target.ParentNode;

                    while (scrollableParent != null)
                    {
                        if (scrollableParent.Style != null && scrollableParent.Style.OverflowY != null && scrollableParent.Style.OverflowY.ToLowerCase() == "auto")
                        {
                            scrollableParent.ScrollTop += (int)(offsetTop - invisibleTop);

                            return;
                        }

                        scrollableParent = scrollableParent.ParentNode;
                    }
                }
            }
        }

        private static void HandleInputTextKeyUp(ElementEvent e)
        {
            // particularly on mobile devices, hitting enter seems like the way to dismiss the 
            // on screen keyboard, so force the text input to blur to cause the OSK to go
            // away
            if (e.KeyCode == 13)
            {
                e.Target.Blur();
            }
        }

        public static void ClearChildElements(Element e)
        {
            if (e == null)
            {
                return;
            }

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
            bool bval = Boolean.Parse(isChecked.ToString());
            Script.Literal("{0}.checked={1}", ie, bval);
        }

        public static void SetBackgroundSize(Element element, String value)
        {
            Script.Literal("{0}.style.backgroundSize={1}", element, value);
        }

        public static void SetBorderRadius(Element element, String value)
        {
            Script.Literal("{0}.style.borderRadius={1}", element, value);
        }

        public static void SetTransition(Element element, String value)
        {
            Script.Literal("{0}.style.transition={1}", element, value);
        }

        public static void SetTransform(Element element, String value)
        {
            Script.Literal("{0}.style.transform={1}", element, value);
        }

        public static void SetIsChecked(InputElement ie, bool isChecked)
        {
            Script.Literal("{0}.checked={1}", ie, isChecked);
        }

        public static String GetTouchStartEventName()
        {
            Script.Literal("if (window.navigator.pointerEnabled) { return \"pointerdown\"; } else if (window.navigator.msPointerEnabled) { return \"MSPointerDown\"; }");

            return "touchstart";
        }

        public static String GetTouchMoveEventName()
        {
            Script.Literal("if (window.navigator.pointerEnabled) { return \"pointermove\"; } else if (window.navigator.msPointerEnabled) { return \"MSPointerMove\"; }");

            return "touchmove";
        }

        public static String GetTouchEndEventName()
        {
            Script.Literal("if (window.navigator.pointerEnabled) { return \"pointerup\"; } else if (window.navigator.msPointerEnabled) { return \"MSPointerUp\"; }");

            return "touchend";
        }

        public static String GetTouchCancelEventName()
        {
            Script.Literal("if (window.navigator.pointerEnabled) { return \"pointercancel\"; } else if (window.navigator.msPointerEnabled) { return null; }");

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
        public static bool GetIsPointerEnabled()
        {
            bool isPointerEnabled = false;

            Script.Literal("{0}=(navigator.maxTouchPoints > 0 || navigator.msMaxTouchPoints > 0)", isPointerEnabled);

            return isPointerEnabled;
        }

        public static bool GetIsPrimary(ElementEvent e)
        {
            Script.Literal("if (window.navigator.pointerEnabled || window.navigator.msPointerEnabled) {{ if ({0}.isPrimary != false) {{ return true; }} return false; }} ", e);

            return true;
        }

        public static double GetPageX(ElementEvent e)
        {
            double pageX = 0;

            Script.Literal("if ({1}.touches != null) {{ {0}={1}.touches[0].pageX; }} else {{ {0}={1}.pageX;}}", pageX, e);

            return pageX;
        }

        public static double GetPageY(ElementEvent e)
        {
            double pageY = 0;

            Script.Literal("if({1}.touches != null) {{ {0}={1}.touches[0].pageY;}} else {{ {0}={1}.pageY;}}", pageY, e);

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

            if (text != null)
            {
                element.AppendChild(Document.CreateTextNode(text));
            }
        }

        public static void SetHtml(Element element, String html)
        {
            element.InnerHTML = ToStaticHTML(html);
        }

        public static void SetTouchAction(Element element, String value)
        {
            Script.Literal("{0}.touchAction={1};{0}.msTouchAction={1};", element.Style, value);
        }

        public static void SetEmptyContent(Element element)
        {
            element.InnerHTML = "&#160;";
        }

        public static string ToStaticHTML(String html)
        {
            Script.Literal("if (typeof window.toStaticHTML !== \"undefined\") {{ {0}=window.toStaticHTML({0}); }}", html);

            return html;
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

        public static Style GetComputedStyle(Element e)
        {
            Style style = e.Style;

            Script.Literal(@"
if ({0}.currentStyle)
{{
    {1} = {0}.currentStyle;
}}
else if (window.getComputedStyle)
{{
    {1} = document.defaultView.getComputedStyle({0}, null);
}}
        ", e, style);

            return style;
        }

        public static bool IsDefaultInputElement(ElementEvent e, bool includeYScrollEvents)
        {
            String targetTagName = e.Target.TagName.ToLowerCase();

            object contentEditable = e.Target.GetAttribute("contenteditable");

            if (targetTagName == "input" || targetTagName == "select" || targetTagName == "textarea" || (String)contentEditable == "true")
            {
                return true;
            }

            if (!Script.IsNullOrUndefined(e.SrcElement))
            {
                String targetClass = e.SrcElement.ClassName;

                if (!String.IsNullOrEmpty(targetClass))
                {
                    if (targetClass.IndexOf("switch-") > 0 || targetClass.IndexOf("grip") > 0 || targetClass.IndexOf("leaflet") > 0 || targetClass.IndexOf("handle") > 0)
                    {
                        return true;
                    }
                }

                Style style = GetComputedStyle(e.SrcElement);

                if (!Script.IsNullOrUndefined(style))
                {
                    if (style.Overflow == "auto" ||
                            style.Overflow == "scroll" ||
                            (includeYScrollEvents && style.OverflowY == "auto") ||
                            (includeYScrollEvents && style.OverflowY == "scroll") ||
                            style.OverflowX == "auto" ||
                            style.OverflowX == "scroll")
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }

}
