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

        public override string TagName
        {
            get
            {
                if (this.displayInlineToolbar)
                {
                    return "TEXTAREA";
                }
                else
                {
                    return base.TagName;
                }
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

                if (this.editorHeight != null && this.Element != null)
                {
                    this.Element.Style.Height = this.editorHeight;
                }
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

                if (this.Element != null)
                {
                    this.Element.SetAttribute("rows", this.rows);
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
                if (this.editor ==  null)
                {
                    this.tempValue = value;
                    return;
                }

                this.editor.Value(value);
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
            }
        }

        public Editor()
        {
        }

        protected override void OnEnsureElements()
        {
            if (this.editorOptions != null && this.editorOptions.ImageBrowser != null && this.editorOptions.ImageBrowser.BeforeLaunch != null && !this.calledImageBrowserCallback)
            {
                this.editorOptions.ImageBrowser.BeforeLaunch(this.PostLaunchContinue, this.editorOptions);
            }
            else
            {
                this.EnsureElementsContinue();
            }
        }

        private void PostLaunchContinue(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                this.calledImageBrowserCallback = true;
                this.EnsureElementsContinue();
            }
        }

        private void EnsureElementsContinue()
        {
            if (this.editorHeight != null)
            {
                this.Element.Style.Height = this.editorHeight;
            }

            if (this.rows != null)
            {
                this.Element.SetAttribute("rows", this.rows);
            }

            // kendo would hide our TEXTAREA, so we should set our visibility to false.
            this.Visible = false;
            
            this.Element.Style.Width = "100%";

            if (this.editorOptions == null)
            {
                this.editorOptions = new EditorOptions();
            }

            if (this.editorOptions.Tools == null)
            {
                String[] toolsToUse = new String[] 
                {
                "formatting",
                "fontName",
                "fontSize",
                "bold",
                "italic",
                "underline",
                "strikethrough",
                "foreColor",
                "backColor",
                "cleanFormatting",
                "subscript",
                "superscript",
                "createLink",
                "unlink",
                "insertImage",
                "insertFile",
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

                this.editorOptions.Tools = toolsToUse;
            }

            if (this.editorOptions.Stylesheets == null)
            {
                this.editorOptions.Stylesheets = new String[] { Context.Current.ResourceBasePath + "qla/css/qla.css?v=" + Context.Current.VersionToken };
            }

            // note we are try catching to work around an exception issue in IE
            Script.Literal("var j={0};j.kendoEditor({2});{1}=j.data('kendoEditor')", this.J, this.editor, this.editorOptions);

            this.Element.Style.BorderWidth = "0px";

            if (this.editor != null)
            {
                this.editor.Bind("change", this.HandleDateChange);

                if (this.tempValue != null)
                {
                    this.editor.Value(this.tempValue);
                }
            }
        }

        public void Blur()
        {
            this.Element.Blur();
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
           this.editor.Destroy();
            
            base.Dispose();
        }
    }
}
