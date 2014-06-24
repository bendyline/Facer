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
        private int rows = 4;
        public event EventHandler Changed;

/*
        This really should return TEXTAREA, but the editor doesn't really work that well here..
        public override string TagName
        {
            get
            {
                return "TEXTAREA";
            }
        }
*/
        [ScriptName("i_rows")]
        public int Rows
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
                    return;
                }
                this.editor.Value(value);
            }
        }

        public Editor()
        {
            this.editorOptions = new EditorOptions();
        }

        protected override void OnEnsureElements()
        {
            this.Element.Style.Height = "80px";
            this.Element.Style.Width = "100%";

            // note we are try catching to work around an exception issue in IE
            Script.Literal("var j={0};j.kendoEditor({2});{1}=j.data('kendoEditor')", this.J, this.editor, this.editorOptions);

            if (this.editor != null)
            {
                this.editor.Bind("change", this.HandleDateChange);
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
