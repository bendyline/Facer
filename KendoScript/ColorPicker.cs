using System;
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

        private String colorValue;

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

        [ScriptName("i_tileSizeWidth")]
        public double? TileSizeWidth
        {
            get
            {
                if (this.options == null)
                {
                    return null;
                }

                return this.options.TileSize.Width;
            }

            set
            {
                if (value == null)
                {
                    this.options.TileSize = null;
                }

                if (this.options.TileSize == null)
                {
                    this.options.TileSize = new Size();
                }

                this.options.TileSize.Width = (double)value;
            }
        }

        [ScriptName("i_tileSizeHeight")]
        public double? TileSizeHeight
        {
            get
            {
                if (this.options == null)
                {
                    return null;
                }

                return this.options.TileSize.Height;
            }

            set
            {
                if (value == null)
                {
                    this.options.TileSize = null;
                }

                if (this.options.TileSize == null)
                {
                    this.options.TileSize = new Size();
                }

                this.options.TileSize.Height = (double)value;
            }
        }

        [ScriptName("s_palette")]
        public object Palette
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
                if (this.colorPicker != null)
                {
                    return this.colorPicker.Value();
                }

                return this.colorValue;
            }

            set
            {
                if (this.colorPicker != null)
                {
                    this.colorPicker.Value(value);
                }

                this.colorValue = value;
            }
        }

        public ColorPicker()
        {
            KendoUtilities.EnsureKendoBaseUx(this);

            this.EnsureScript("kendo.ui.Slider", "js/kendo/kendo.slider.min.js");
            this.EnsureScript("kendo.ui.ColorPicker", "js/kendo/kendo.colorpicker.min.js");

            this.options = new ColorPickerOptions();
        }

        protected override void OnApplyTemplate()
        {
            ElementUtilities.ClearChildElements(this.Element);

            Element e = Document.CreateElement("DIV");

            e.Style.Width = "100%";

            jQueryObject jqueryObject = jQuery.FromObject(e);

            this.Element.AppendChild(e);

            Script.Literal("var j = {0}; j.kendoColorPicker({2}); {1} = j.data('kendoColorPicker')", jqueryObject, colorPicker, this.options);

            if (this.colorValue != null)
            {
                this.colorPicker.Value(this.colorValue);
            }

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
            if (this.colorPicker != null)
            {
                this.colorPicker.Destroy();
            }

            base.Dispose();
        }
    }
}
