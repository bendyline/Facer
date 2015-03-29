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
        public static void EnsureKendoBaseUx(Control c)
        {
            c.EnsurePrerequisite("kendo.effects.Expand", "js/kendo/kendo.fx.min.js");
            c.EnsurePrerequisite("kendo.UserEvents", "js/kendo/kendo.userevents.min.js");
            c.EnsurePrerequisite("kendo.TapCapture", "js/kendo/kendo.draganddrop.min.js");
            c.EnsurePrerequisite("kendo.Color", "js/kendo/kendo.color.min.js");
            c.EnsurePrerequisite("kendo.ui.Popup", "js/kendo/kendo.popup.min.js");
        }

        public static void EnsureKendoData(Control c)
        {
            c.EnsurePrerequisite("kendo.data.transports.odata", "js/kendo/kendo.data.odata.min.js");
            c.EnsurePrerequisite("kendo.data.XmlDataReader", "js/kendo/kendo.data.xml.min.js");
            c.EnsurePrerequisite("kendo.data.DataSource", "js/kendo/kendo.data.min.js");
        }

        public static void EnsureKendoEditable(Control c)
        {
            c.EnsurePrerequisite("kendo.ui.Calendar", "js/kendo/kendo.calendar.min.js");
            c.EnsurePrerequisite("kendo.ui.DatePicker", "js/kendo/kendo.datepicker.min.js");
            c.EnsurePrerequisite("kendo.ui.Validator", "js/kendo/kendo.validator.min.js");
            c.EnsurePrerequisite("kendo.ui.NumericTextBox", "js/kendo/kendo.numerictextbox.min.js");
            c.EnsurePrerequisite("kendo.ui.Editable", "js/kendo/kendo.editable.min.js");
        }

        public static void EnsureKendoDataViz(Control c)
        {
            c.EnsurePrerequisite("kendo.drawing.Surface", "js/kendo/kendo.drawing.min.js");
            c.EnsurePrerequisite("kendo.dataviz.Point2D", "js/kendo/kendo.dataviz.core.min.js");
            c.EnsurePrerequisite("kendo.dataviz.ui.themes.black", "js/kendo/kendo.dataviz.themes.min.js");
        }
    }
}
