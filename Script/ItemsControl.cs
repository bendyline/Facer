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
        private bool wrapItems = false;

        public virtual Element ItemsContainerElement
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
                if (this.itemsContainerElement == value)
                {
                    return;
                }

                this.ClearItemContainerElement();

                this.itemsContainerElement = value;

                if (this.itemsContainerElement != null && this.ItemControls != null)
                {
                    foreach (Control c in this.ItemControls)
                    {
                        this.AppendControl(c);
                    }
                }
            }
        }

        public ICollection<Control> ItemControls
        {
            get
            {
                return this.itemControls;
            }
        }

        protected bool WrapItems
        {
            get
            {
                return this.wrapItems;
            }

            set
            {
                this.wrapItems = value;
            }
        }

        public ItemsControl()
        {
        }

        public Element GetWrapper(int index)
        {
            if (this.Element != null)
            {
                if (this.Element.ChildNodes.Length > index)
                {
                    return this.Element.ChildNodes[index];
                }
            }
            return null;
        }

        public Control GetById(String id)
        {
            return this.itemControlsById[id];
        }


        internal override void ClearTemplate()
        {
            base.ClearTemplate();

            if (this.itemControls != null)
            {
                foreach (Control c in this.ItemControls)
                {
                    c.ClearTemplate();
                }
            }
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

        private void ClearItemContainerElement()
        {
            if (this.ItemsContainerElement != null && this.ItemControls != null)
            {
                foreach (Control c in this.ItemControls)
                {
                    if (c.Element != null && this.ItemsContainerElement.Contains(c.Element))
                    {
                        try
                        {
                            this.ItemsContainerElement.RemoveChild(c.Element);
                        }
                        catch (Exception)
                        {
                            ;
                        }
                    }
                }
            }
        }
        public void ClearItemControls()
        {
            this.ClearItemContainerElement();

            this.itemControls = null;
            this.itemControlsById = null;
        }


        public override void Dispose()
        {
            foreach (Control c in this.itemControls)
            {
                c.Dispose();
            }

            base.Dispose();
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
            Element itemsContainerElement = this.ItemsContainerElement;

            if (this.itemControls == null || itemsContainerElement == null)
            {
                return;
            }

            Element elt = null;
            Element eltWrapper = null;

            bool isNew = false;

            if (c.Element == null)
            {
                String tagName = c.TagName;

                if (tagName == null)
                {
                    tagName = "DIV";
                }

                 elt = Document.CreateElement(tagName);

                 if (this.wrapItems)
                 {
                     eltWrapper = this.CreateElement("itemWrapper");
                     eltWrapper.AppendChild(elt);
                 }

                 isNew = true;
            }
            else
            {
                c.EnsureElements();

                elt = c.Element;

                if (this.wrapItems)
                {
                    if (elt.ParentNode != null)
                    {
                        eltWrapper = elt.ParentNode;
                    }
                    else
                    {
                        eltWrapper = this.CreateElement("itemWrapper");
                        eltWrapper.AppendChild(elt);
                    }
                }
            }

            if ((eltWrapper != null && eltWrapper.ParentNode != itemsContainerElement) || (eltWrapper == null && elt.ParentNode != itemsContainerElement))
            {
                if (index < 0 || index >= itemsContainerElement.Children.Length)
                {
                    if (eltWrapper != null)
                    {
                        itemsContainerElement.AppendChild(eltWrapper);
                    }
                    else
                    {
                        itemsContainerElement.AppendChild(elt);
                    }
                }
                else
                {
                    if (eltWrapper != null)
                    {
                        itemsContainerElement.InsertBefore(eltWrapper, itemsContainerElement.Children[index]);
                    }
                    else
                    {
                        itemsContainerElement.InsertBefore(elt, itemsContainerElement.Children[index]);
                    }
                }
            }

            if (isNew)
            {
                c.Element = elt;
            }
        }
    }
}
