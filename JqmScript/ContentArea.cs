/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Html;

namespace BL.JQM
{
    public class ContentArea : jQueryItemsControl
    {
        private Element binElement;

        public Element BinElement
        {
            get
            {
                return this.binElement;
            }
        }

        public override string Role
        {
            get { return "content"; }
        }

        public override String TypeId
        {
            get
            {
                return "bl-jqm-ca";
            }
        }

        protected override void OnEnsureElements()
        {
            this.binElement = this.CreateElement("bin");

            this.Element.AppendChild(this.binElement);

            Element content = this.CreateElement("contentArea");

            this.ContentElement = content;

            this.binElement.AppendChild(content);

            Element container = this.CreateElement("items");

            this.ItemsContainerElement = container;
           
            this.BinElement.AppendChild(container);
        }
    }
}
