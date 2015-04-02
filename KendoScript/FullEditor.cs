﻿using System;
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

    public class FullEditor : Control
    {
        [ScriptName("c_inlineEditor")]
        private Editor inlineEditor;

        private Editor popupEditor;
        private String title;

        [ScriptName("e_cta")]
        private Element cta;

        [ScriptName("e_contentDisplay")]
        private Element contentDisplay;

        private int rows = 4;
        public event EventHandler Changed;
        private String html;
        private bool popupMode = false;
        private EditorOptions editorOptions;

        private EventHandler popupChanged;

        private bool readOnly = false;

        public bool ReadOnly
        {
            get
            {
                return this.readOnly;
            }

            set
            {
                this.readOnly = value;

                this.ApplyReadOnly();
            }
        }

        public EditorOptions EditorOptions
        {
            get
            {
                if (this.editorOptions == null)
                {
                    this.editorOptions = new EditorOptions();
                }

                return this.editorOptions;
            }

            set
            {
                this.editorOptions = value;
            }
        }

        [ScriptName("s_title")]
        public String Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.title = value;
            }
        }

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
                return this.html;
            }

            set
            {
                if (this.html == value)
                {
                    return;
                }

                this.html = value;

                if (this.popupEditor != null)
                {
                    this.popupEditor.Value = this.html;
                }

                this.Update();
            }
        }

        public FullEditor()
        {
            this.TrackInteractionEvents = true;

            this.popupChanged = this.popupEditor_Changed;
        }

        protected override void OnClick(ElementEvent e)
        {
            if (this.readOnly)
            {
                return;
            }

            Dialog d = new Dialog();

            d.VerticalPadding = 0;
            d.HorizontalPadding = 0;
            d.Title = this.title;
            d.DisplayDoneButton = true;
            d.Show();

            if (this.popupEditor != null)
            {
                this.popupEditor.Changed -= this.popupChanged;
                this.popupEditor.Dispose();
            }

            this.popupEditor = new Editor();
            this.popupEditor.Options = this.EditorOptions;
            this.popupEditor.Rows = 0;
            this.popupEditor.EditorHeight = "100%";
            this.popupEditor.DisplayInlineToolbar = true;

            if (this.html != null)
            {
                this.popupEditor.Value = this.html;
            }

            this.popupEditor.Changed += this.popupChanged;

            d.Content = this.popupEditor;
        }

        private void ApplyReadOnly()
        {
            if (this.cta == null)
            {
                return;
            }

            if (this.readOnly)
            {
                ElementUtilities.SetText(this.cta, "");
            }
            else
            {
                ElementUtilities.SetText(this.cta, "(click to edit)");
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.ApplyReadOnly();
        }

        private void popupEditor_Changed(object sender, EventArgs e)
        {
            this.html = this.popupEditor.Value;

            this.Update();

            if (this.Changed != null)
            {
                this.Changed(this, e);
            }
        }

        protected override void OnUpdate()
        {
            if (this.contentDisplay != null && !this.popupMode)
            {
                if (this.html != null)
                {
                    this.contentDisplay.InnerHTML = ElementUtilities.ToStaticHTML(this.html);
                }
                else
                {
                    this.contentDisplay.InnerHTML = "";
                }
            }

            if (this.inlineEditor != null && this.popupMode)
            {
                this.inlineEditor.Value = this.html;
            }
        }

        public void Blur()
        {
            try
            {
                if (this.Element != null)
                {
                    this.Element.Blur();
                }
            }
            catch (Exception)
            {
                ;
            }
        }

        private void HandleDateChange(object e)
        {
            if (this.Changed != null)
            {
                this.Changed(null, EventArgs.Empty);
            }
        }
    }
}
