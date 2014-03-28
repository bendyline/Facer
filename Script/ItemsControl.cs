/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Html;

namespace BL.UI
{
    public abstract class ItemsControl : Control
    {
        private Element itemsContainerElement;
        private List<Control> itemControls;
        private Dictionary<String, Control> itemControlsById;

        public Element ItemsContainerElement
        {
            get
            {
                if (this.itemsContainerElement == null)
                {
                    return this.Element;
                }

                return this.itemsContainerElement;
            }

            set
            {
                this.itemsContainerElement = value;
            }
        }

        public ICollection<Control> ItemControls
        {
            get
            {
                return this.itemControls;
            }
        }

        public ItemsControl()
        {
        }

        public Control GetById(String id)
        {
            return this.itemControlsById[id];
        }

        public void InsertItemControl(int index, Control c)
        {
            if (this.itemControls == null)
            {
                this.itemControls = new List<Control>();
                this.itemControlsById = new Dictionary<string, Control>();
            }

            Debug.Assert(!this.itemControls.Contains(c));

            this.itemControls.Insert(index, c);
            this.itemControlsById[c.Id] = c;

            if (this.ElementsEnsured)
            {
                this.InsertControl(index, c);
            }

            this.OnItemControlAdded(c);
        }

        public void AddItemControl(Control c)
        {
            if (this.itemControls == null)
            {
                this.itemControls = new List<Control>();
                this.itemControlsById = new Dictionary<string, Control>();
            }

            Debug.Assert(!this.itemControls.Contains(c));

            this.itemControls.Add(c);
            this.itemControlsById[c.Id] = c;

            if (this.ElementsEnsured)
            {
                this.AppendControl(c);
            }

            this.OnItemControlAdded(c);
        }

        protected virtual void OnItemControlAdded(Control c)
        {

        }

        public void ClearItemControls()
        {
            if (this.itemsContainerElement != null)
            {
                foreach (Control c in this.ItemControls)
                {
                    this.itemsContainerElement.RemoveChild(c.Element);
                }
            }

            this.itemControls = null;
            this.itemControlsById = null;
        }

        protected override void OnBaseControlsElementsPostInit()
        {
            if (this.itemControls == null || this.ItemsContainerElement == null)
            {
                return;
            }

            foreach (Control c in this.itemControls)
            {
                this.AppendControl(c);
            }
        }

        private void AppendControl(Control c)
        {
            this.InsertControl(-1, c);
        }

        private void InsertControl(int index, Control c)
        {
            Element e = this.ItemsContainerElement;

            if (this.itemControls == null || e == null)
            {
                return;
            }

            if (c.Element == null)
            {
                String tagName = c.TagName;

                if (tagName == null)
                {
                    tagName = "DIV";
                }

                Element elt = Document.CreateElement(tagName);

                c.Element = elt;
            }
            else
            {
                c.EnsureElements();
            }

            if (c.Element.ParentNode != e)
            {
                if (index < 0 || index >= e.Children.Length)
                {
                    e.AppendChild(c.Element);
                }
                else
                {
                    e.InsertBefore(c.Element, e.Children[index]);
                }
            }
        }
    }
}
