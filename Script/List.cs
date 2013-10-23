// Class1.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using jQueryApi;

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
