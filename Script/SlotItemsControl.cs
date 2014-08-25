/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;

namespace BL.UI
{
    public class SlotItemsControl : ItemsControl
    {
        [ScriptName("e_item0")]
        private Element item0;

        [ScriptName("e_item1")]
        private Element item1;

        [ScriptName("e_item2")]
        private Element item2;

        [ScriptName("e_item3")]
        private Element item3;

        [ScriptName("e_item4")]
        private Element item4;

        [ScriptName("e_item5")]
        private Element item5;

        [ScriptName("e_item6")]
        private Element item6;

        [ScriptName("e_item7")]
        private Element item7;

        [ScriptName("e_item8")]
        private Element item8;

        [ScriptName("e_item9")]
        private Element item9;

        [ScriptName("e_item10")]
        private Element item10;

        [ScriptName("e_item11")]
        private Element item11;

        [ScriptName("e_item12")]
        private Element item12;

        [ScriptName("e_item13")]
        private Element item13;

        [ScriptName("e_item14")]
        private Element item14;

        [ScriptName("e_item15")]
        private Element item15;

        [ScriptName("e_item16")]
        private Element item16;

        [ScriptName("e_item17")]
        private Element item17;

        [ScriptName("e_item18")]
        private Element item18;

        [ScriptName("e_item19")]
        private Element item19;

        [ScriptName("e_item20")]
        private Element item20;


        public override Element ItemsContainerElement
        {
            get
            {
                return null;
            }

            set
            {

            }
        }

        public SlotItemsControl()
        {
        }

        protected override void OnUpdate()
        {
       
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.UpdateItems();
        }

        private void UpdateItems()
        {
            this.UpdateItem(this.item0, 0);
            this.UpdateItem(this.item1, 1);
            this.UpdateItem(this.item2, 2);
            this.UpdateItem(this.item3, 3);
            this.UpdateItem(this.item4, 4);
            this.UpdateItem(this.item5, 5);
            this.UpdateItem(this.item6, 6);
            this.UpdateItem(this.item7, 7);
            this.UpdateItem(this.item8, 8);
            this.UpdateItem(this.item9, 9);
        }

        private void UpdateItem(Element e, int index)
        {
            if (e != null && this.ItemControls.Count > index)
            {
                Control c = this.ItemControls[index];

                c.EnsureElements();

                if (c.Element.ParentNode != e)
                {
                    if (c.Element.ParentNode != null)
                    {
                        c.Element.ParentNode.RemoveChild(c.Element);
                    }

                    e.AppendChild(c.Element);
                }
            }
        }
    }
}
