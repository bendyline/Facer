﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using BL.UI;
using jQueryApi;
using Kendo.UI;
using System.Runtime.CompilerServices;

namespace BL.UI.KendoControls
{

    public class ColorPicker : Control
    {
        public event EventHandler Changed;

        private Kendo.UI.ColorPicker colorPicker;
        private Kendo.UI.ColorPickerOptions options;

        [ScriptName("b_preview")]
        public Boolean Preview
        {
            get
            {
                return this.options.Preview;
            }

            set
            {
                this.options.Preview = value;
            }
        }

        [ScriptName("s_palette")]
        public String Palette
        {
            get
            {
                return this.options.Palette;
            }

            set
            {
                this.options.Palette = value;
            }
        }

        [ScriptName("b_buttons")]
        public Boolean Buttons
        {
            get
            {
                return this.options.Buttons;
            }

            set
            {
                this.options.Buttons = value;
            }
        }


        [ScriptName("s_value")]
        public String Value
        {
            get
            {
                return this.colorPicker.Value();
            }

            set
            {
                this.colorPicker.Value(value);
            }
        }

        public ColorPicker()
        {
            this.options = new ColorPickerOptions();
        }

        protected override void OnEnsureElements()
        {
            Script.Literal("var j = {0}; j.kendoColorPicker({2}); {1} = j.data('kendoColorPicker')", this.J, colorPicker, this.options);

            colorPicker.Bind("change", this.HandleColorChange);
        }

        private void HandleColorChange(object e)
        {
            if (this.Changed != null)
            {
                this.Changed(null, EventArgs.Empty);
            }
        }

        public override void Dispose()
        {
           this.colorPicker.Destroy();
            
            base.Dispose();
        }
    }
}
