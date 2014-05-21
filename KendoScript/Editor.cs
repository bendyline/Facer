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


        public Editor()
        {
            this.editorOptions = new EditorOptions();
        }

        protected override void OnEnsureElements()
        { 
            // note we are try catching to work around an exception issue in IE
            Script.Literal("var j={0};j.kendoEditor({2});{1}=j.data('kendoEditor')", this.J, this.editor, this.editorOptions);
        }

        public override void Dispose()
        {
           this.editor.Destroy();
            
            base.Dispose();
        }
    }
}
