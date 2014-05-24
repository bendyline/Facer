/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Html;

namespace BL.UI
{
    public abstract class ContentControl : Control
    {
        private Element contentContainerElement;
        private Control content;

        public Element ContentContainerElement
        {
            get
            {
                if (this.contentContainerElement == null)
                {
                    return this.Element;
                }

                return this.contentContainerElement;
            }

            set
            {
                if (this.contentContainerElement == value)
                {
                    return;
                }
                
                this.contentContainerElement = value;

                this.SetControl(this.content);
            }
        }

        public Control Content
        {
            get
            {
                return this.content;
            }

            set
            {
                this.content = value;

                this.SetControl(this.content);
;            }
        }

        public ContentControl()
        {
        }

        private void SetControl(Control c)
        {
            this.InsertControl(-1, c);
        }

        private void InsertControl(int index, Control c)
        {
            Element e = this.ContentContainerElement;

            if (this.content  == null || e == null)
            {
                return;
            }

            bool isNew = false;
            Element elt = null;

            if (c.Element == null)
            {
                String tagName = c.TagName;

                if (tagName == null)
                {
                    tagName = "DIV";
                }

                elt = Document.CreateElement(tagName);
                isNew = true;
            }
            else
            {
                c.EnsureElements();
                elt = c.Element;
            }

            if (elt.ParentNode != e)
            {
                if (index < 0 || index >= e.Children.Length)
                {
                    e.AppendChild(elt);
                }
                else
                {
                    e.InsertBefore(elt, e.Children[index]);
                }
            }

            if (isNew)
            {
                c.Element = elt;
            }
        }
    }
}
