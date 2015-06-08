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

    public class Menu : Control
    {
        public event EventHandler Selected;

        private List<object> appendedItems;

        private Kendo.UI.Menu menu;
        private Kendo.UI.MenuOptions menuOptions;

        private bool isInitialized = false;

        public override string TagName
        {
            get
            {
                return "UL";
            }
            set
            {
                base.TagName = value;
            }
        }

        [ScriptName("b_closeOnClick")]
        public bool CloseOnClick
        {
            get
            {
                return this.Options.CloseOnClick;
            }

            set
            {
                this.Options.CloseOnClick = value;
            }
        }

        [ScriptName("b_openOnClick")]
        public bool OpenOnClick
        {
            get
            {
                return this.Options.OpenOnClick;
            }

            set
            {
                this.Options.OpenOnClick = value;
            }
        }

        [ScriptName("s_popupCollision")]
        public String  PopupCollision
        {
            get
            {
                return this.Options.PopupCollision;
            }

            set
            {
                this.Options.PopupCollision = value;
            }
        }

        [ScriptName("s_orientation")]
        public String Orientation
        {
            get
            {
                return this.Options.Orientation;
            }

            set
            {
                this.Options.Orientation = value;
            }
        }

        [ScriptName("s_direction")]
        public String Direction
        {
            get
            {
                return this.Options.Direction;
            }

            set
            {
                this.Options.Direction = value;
            }
        }

        public MenuOptions Options
        {
            get
            {
                return this.menuOptions;
            }

            set
            {
                this.menuOptions = value;
            }
        }

        public Menu()
        {
            KendoUtilities.EnsureKendoBaseUx(this);

            this.EnsurePrerequisite("kendo.ui.Menu", "js/kendo/kendo.menu.min.js");

            this.menuOptions = new MenuOptions();
        }

        protected override void OnApplyTemplate()
        {
            if (!this.isInitialized && this.ElementsEnsured)
            {
                ElementUtilities.ClearChildElements(this.Element);

                Element e = Document.CreateElement("DIV");

                e.Style.Width = "100%";

                jQueryObject jqueryObject = jQuery.FromObject(e);

                this.Element.AppendChild(e);

                Script.Literal("var j = {0}; j.kendoMenu({2}); {1} = j.data('kendoMenu')", jqueryObject, this.menu, this.menuOptions);

                this.menu.Bind("select", this.HandleSelect);

                if (this.appendedItems != null)
                {
                    foreach (object o in this.appendedItems)
                    {
                        this.menu.Append(o);
                    }
                }

                this.isInitialized = true;
            }

            this.ApplyVisibility();
        }

        private void HandleSelect(object e)
        {
            Script.Literal("{0}={0}.item", e);
            jQueryObject o = jQuery.FromObject(e);

            string text = o.Children(".k-link").GetText();


            object result = this.GetSelectedItem(this.appendedItems, text);
            

            if (this.Selected != null)
            {
                this.Selected(result, EventArgs.Empty);
            }
        }

        public object GetSelectedItem(object o, String text)
        {
            if (o is IEnumerable<object> || o is Array)
            {
                IEnumerable<object> results = (IEnumerable<object>)o;

                foreach (object oChild in results)
                {
                    object result = GetSelectedItem(oChild, text);

                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            else if (o is TextImageItems)
            {
                TextImageItems tii = (TextImageItems)o;

                if (tii.Text == text)
                {
                    return tii;
                }

                if (tii.Items != null)
                {
                    foreach (TextImageItems tiiChild in tii.Items)
                    {
                        object result = GetSelectedItem(tiiChild,text);

                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
            }

            return null;
        }

        public void Append(object o)
        {
            if (this.appendedItems == null)
            {
                this.appendedItems = new List<object>();
            }

            this.appendedItems.Add(o);

            if (this.menu != null)
            {
                this.menu.Append(o);
            }
        }

        public override void Dispose()
        {
            if (this.menu != null)
            {
                this.menu.Destroy();
            }

            base.Dispose();
        }
    }
}
