// IControlFactory.cs
//

using jQueryApi;
using Kendo.UI;
using System;
using System.Collections.Generic;
using System.Html;

namespace BL.UI.KendoControls
{
    public static class KendoUtilities 
    {
        private static object mobileApp;

        public static void EnsureMobileApplication()
        {
            if (mobileApp == null)
            {
        //       Script.Literal("{0}=new kendo.mobile.Application(document.body);", mobileApp);
            }
        }


        public static Kendo.UI.Draggable CreateDraggable(Element e, DraggableOptions options)
        {
            Draggable draggable = null;

            jQueryObject jqueryObject = jQuery.FromObject(e);

            Script.Literal("{2}={0}.kendoDraggable({1})", jqueryObject, options, draggable);

            return draggable;
        }

        public static Kendo.UI.DropTarget CreateDropTarget(Control c, DropTargetOptions options)
        {
            DropTarget dropTarget = null;

            Script.Literal("{2}={0}.kendoDropTarget({1})", c.J, options, dropTarget);

            return dropTarget;
        }

        public static String SafeifyFileName(String fileName)
        {
            fileName = fileName.Replace("/", ".");
            fileName = fileName.Replace("\\", ".");

            return fileName;
        }
   
        public static void EnsureKendoBaseUx(Control c)
        {
            c.EnsureScript("kendo.effects.Expand", "js/kendo/kendo.fx.min.js");
            c.EnsureScript("kendo.UserEvents", "js/kendo/kendo.userevents.min.js");
            c.EnsureScript("kendo.TapCapture", "js/kendo/kendo.draganddrop.min.js");
            c.EnsureScript("kendo.Color", "js/kendo/kendo.color.min.js");
            c.EnsureScript("kendo.ui.Popup", "js/kendo/kendo.popup.min.js");
        }

        public static void EnsureKendoData(Control c)
        {
            c.EnsureScript("kendo.data.transports.odata", "js/kendo/kendo.data.odata.min.js");
            c.EnsureScript("kendo.data.XmlDataReader", "js/kendo/kendo.data.xml.min.js");
            c.EnsureScript("kendo.data.DataSource", "js/kendo/kendo.data.min.js");
        }

        public static void EnsureKendoEditable(Control c)
        {
            c.EnsureScript("kendo.ui.Calendar", "js/kendo/kendo.calendar.min.js");
            c.EnsureScript("kendo.ui.DatePicker", "js/kendo/kendo.datepicker.min.js");
            c.EnsureScript("kendo.ui.Validator", "js/kendo/kendo.validator.min.js");
            c.EnsureScript("kendo.ui.NumericTextBox", "js/kendo/kendo.numerictextbox.min.js");
            c.EnsureScript("kendo.ui.Editable", "js/kendo/kendo.editable.min.js");
        }

        public static void EnsureKendoDataViz(Control c)
        {
            c.EnsureScript("kendo.drawing.Surface", "js/kendo/kendo.drawing.min.js");
            c.EnsureScript("kendo.dataviz.Point2D", "js/kendo/kendo.dataviz.core.min.js");
            c.EnsureScript("kendo.dataviz.ui.themes.black", "js/kendo/kendo.dataviz.themes.min.js");
        }
    }
}
