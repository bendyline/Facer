/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;

namespace BL.UI
{
    public class List : ItemsControl
    {
        private ContentItemSet items;
        private Dictionary<String, ListItem> existingItems;

        public ContentItemSet ListItems
        {
            get
            {
                return items;
            }

            set
            {
                items = value;

                this.Update();
            }
        }

        public List()
        {
            this.existingItems = new Dictionary<string, ListItem>();
        }

        protected override void OnUpdate()
        {
            if (this.ListItems != null)
            {
                foreach (ContentItem ci in this.ListItems.Items)
                {
                    this.NotifyItemUpdated(ci);
                }
            }
        }

        public void NotifyItemUpdated(ContentItem ci)
        {
            ListItem s = existingItems[ci.Id];

            if (s == null)
            {
                s = new ListItem();

                if (ci.TemplateId != null)
                {
                    s.TemplateId = ci.TemplateId;
                }

                s.Item = ci;

                existingItems[ci.Id] = s;

                this.AddItemControl(s);
            }
        }
    }
}
