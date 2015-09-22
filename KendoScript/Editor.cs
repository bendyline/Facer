using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using BL.UI;
using jQueryApi;
using Kendo.UI;
using System.Runtime.CompilerServices;
using kendo.data;

namespace BL.UI.KendoControls
{

    public class Editor : Control
    {
        private Kendo.UI.Editor editor;
        private Kendo.UI.EditorOptions editorOptions;
        private int? rows = null;
        public event EventHandler Changed;
        private bool displayInlineToolbar;
        private String editorHeight = null;
        private String tempValue = null;
        private bool calledImageBrowserCallback = false;
        private bool grabFocusOnLoad = false;
        private Element editorElement = null;

        public bool GrabFocusOnLoad
        {
            get
            {
                return this.grabFocusOnLoad;
            }

            set
            {
                this.grabFocusOnLoad = value;
            }
        }

        [ScriptName("b_displayInlineToolbar")]
        public bool DisplayInlineToolbar
        {
            get
            {
                return this.displayInlineToolbar;
            }

            set
            {
                this.displayInlineToolbar=  value;
            }
        }

        [ScriptName("s_editorHeight")]
        public String EditorHeight
        {
            get
            {
                return this.editorHeight;
            }

            set
            {
                this.editorHeight = value;

                this.ApplyEditorHeight();
            }
        }

        [ScriptName("i_rows")]
        public int? Rows
        {
            get
            {
                return this.rows;
            }

            set
            {
                this.rows = value;

                if (this.editorElement != null)
                {
                    this.editorElement.SetAttribute("rows", this.rows);
                }
            }
        }

        [ScriptName("s_value")]
        public String Value
        {
            get
            {
                return this.editor.Value();
            }

            set
            {
                this.tempValue = value;

                if (this.editor ==  null)
                {
                    return;
                }

                // an exception in "getSelection" is thrown in Chrome if the RTE contained an image previously
                try
                {
                    this.editor.Value(value);
                }
                catch (Exception)
                {
                }
            }
        }

        public EditorOptions Options
        {
            get
            {
                return this.editorOptions;
            }

            set
            {
                this.editorOptions = value;

                this.DelayApplyTemplate = false;
            }
        }

        public Editor()
        {
            this.DelayApplyTemplate = true;

            KendoUtilities.EnsureKendoBaseUx(this);
            KendoUtilities.EnsureKendoData(this);

            this.EnsureScript("kendo.mobile.ui.Scroller", "js/kendo/kendo.mobile.scroller.min.js");
            this.EnsureScript("kendo.ui.List", "js/kendo/kendo.list.min.js");
            this.EnsureScript("kendo.ui.Selectable", "js/kendo/kendo.selectable.min.js");

            this.EnsureScript("kendo.observable", "js/kendo/kendo.binder.min.js");

            KendoUtilities.EnsureKendoEditable(this);

            this.EnsureScript("kendo.ui.Slider", "js/kendo/kendo.slider.min.js");
            this.EnsureScript("kendo.ui.Resizable", "js/kendo/kendo.resizable.min.js");
            this.EnsureScript("kendo.ui.Upload", "js/kendo/kendo.upload.min.js");
            this.EnsureScript("kendo.ui.Window", "js/kendo/kendo.window.min.js");
            this.EnsureScript("kendo.ui.DropDownList", "js/kendo/kendo.dropdownlist.min.js");
            this.EnsureScript("kendo.ui.ComboBox", "js/kendo/kendo.combobox.min.js");
            this.EnsureScript("kendo.ui.ColorPicker", "js/kendo/kendo.colorpicker.min.js");
            this.EnsureScript("kendo.ui.ListView", "js/kendo/kendo.listview.min.js");
            this.EnsureScript("kendo.ui.FileBrowser", "js/kendo/kendo.filebrowser.min.js");
            this.EnsureScript("kendo.ui.ImageBrowser", "js/kendo/kendo.imagebrowser.min.js");
            this.EnsureScript("kendo.ui.Editor", "js/kendo/kendo.editor.min.js");
        }

        private void ApplyEditorHeight()
        {
            if (this.editorHeight != null && this.Element != null && this.editorElement != null)
            {
                this.Element.Style.Height = this.editorHeight;
                this.editorElement.Style.Height = this.editorHeight;

                Element tableElement = this.Element.ChildNodes[0];
                if (tableElement.TagName.ToUpperCase() == "TABLE" && this.editorHeight.EndsWith("px"))
                {
                    //                 TBODY           Second TR      TD                  IFRAME
                    tableElement.ChildNodes[0].ChildNodes[1].ChildNodes[0].ChildNodes[0].Style.Height = Int32.Parse(this.editorHeight.Substring(0, this.editorHeight.Length - 2)) - 80 + "px";
                }
            }
            else if (this.Element != null && this.editorHeight == null)
            {
                this.Element.Style.Height = "100%";
            }
        }

        protected override void OnApplyTemplate()
        {
            if (this.editorOptions != null && this.editorOptions.ImageBrowser != null && this.editorOptions.ImageBrowser.BeforeLaunch != null && !this.calledImageBrowserCallback)
            {
                this.editorOptions.ImageBrowser.BeforeLaunch(this.PostLaunchContinue, this.editorOptions);
            }
            else
            {
                Window.SetTimeout(this.ApplyTemplateContinue, 10);
            }
        }

        private void PostLaunchContinue(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                this.calledImageBrowserCallback = true;

                Window.SetTimeout(this.ApplyTemplateContinue, 10);
            }
        }

        private void ApplyTemplateContinue()
        {
            ElementUtilities.ClearChildElements(this.Element);

            if (this.displayInlineToolbar)
            {
                this.editorElement = Document.CreateElement("TEXTAREA");
            }
            else
            {
                this.editorElement = Document.CreateElement("DIV");
            }

            this.Element.AppendChild(this.editorElement);

            if (this.rows != null)
            {
                this.editorElement.SetAttribute("rows", this.rows);
            }

            this.ApplyEditorHeight();
            
            this.Element.Style.Width = "100%";
            
            this.editorElement.Style.Width = "100%";

            if (this.editorOptions == null)
            {
                this.editorOptions = new EditorOptions();
            }

            if (this.editorOptions.Tools == null)
            {
                String[] toolsToUse = null;
                
                if (Context.Current.IsSmallFormFactor)
                {
                    toolsToUse = new String[] 
                    {
                        "bold",
                        "italic",
                        "underline",
                        "foreColor",
                        "cleanFormatting",
                        "createLink",
                        "unlink",
                        "insertImage",
                        "insertUnorderedList",
                        "insertOrderedList",
                        "indent",
                        "outdent"
                    };
                }
                else
                {
                     toolsToUse = new String[] 
                    {
                        "fontName",
                        "fontSize",
                        "bold",
                        "italic",
                        "underline",
                        "strikethrough",
                        "foreColor",
                        "backColor",
                        "cleanFormatting",
                        "createLink",
                        "unlink",
                        "insertImage",
                        "insertUnorderedList",
                        "insertOrderedList",
                        "indent",
                        "outdent",
                        "justifyLeft",
                        "justifyCenter",
                        "justifyRight",
                        "justifyFull",
                        "createTable",
                        "addRowAbove",
                        "addRowBelow",
                        "addColumnLeft",
                        "addColumnRight",
                        "deleteRow",
                        "deleteColumn"
                    };
                }

                this.editorOptions.Tools = toolsToUse;
            }

            if (this.editorOptions.Serialization == null)
            {
                this.editorOptions.Serialization = new EditorSerializationOptions();
                // use presentational <b>/<i>/<font> tags because server side santization doesn't like <span style="">
                this.editorOptions.Serialization.Semantic = false;
            }

            if (this.editorOptions.Stylesheets == null)
            {
                this.editorOptions.Stylesheets = new String[] { Context.Current.ResourceBasePath + "qla/css/qla.css?v=" + Context.Current.VersionToken };
            }

            jQueryObject jqo = jQuery.FromObject(this.editorElement);

            Script.Literal("{0}.kendoEditor({2});{1}={0}.data('kendoEditor')", jqo, this.editor, this.editorOptions);

            this.editorElement.Style.BorderWidth = "0px";

            this.ApplyEditorHeight();

            if (this.editor != null)
            {
                this.editor.Bind("change", this.HandleDateChange);

                if (this.tempValue != null)
                {
                    this.editor.Value(this.tempValue);
                }

                if (this.grabFocusOnLoad)
                {
                    this.editor.Focus();
                }
            }
        }

        public void Blur()
        {
            this.Element.Blur();

            if (this.editorElement != null)
            {
                this.editorElement.Blur();
            }
        }

        private void HandleDateChange(object e)
        {
            if (this.Changed != null)
            {
                this.Changed(null, EventArgs.Empty);
            }
        }


        public override void Dispose()
        {
            if (this.editor != null)
            {
                this.editor.Destroy();
            }

            base.Dispose();
        }
    }
}
