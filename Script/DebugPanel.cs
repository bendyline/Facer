
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BL.UI
{
    public class DebugPanel : Control
    {
        [ScriptName("c_logList")]
        private List logList;

        private int logItemId = 0;
        private ContentItemSet logItems;

        public DebugPanel()
        {
            this.logItems = new ContentItemSet();
            Log.ItemAdded += new LogItemEventHandler(Log_ItemAdded);
        }

        private void Log_ItemAdded(object sender, LogItemEventArgs e)
        {
            ContentItem ci = new ContentItem();

            ci.Id = logItemId.ToString();
            ci.Title = e.Item.Message;

            this.logItems.Items.Add(ci);

            logItemId++;

            if (this.logList != null)
            {
                this.logList.NotifyItemUpdated(ci);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.logList != null)
            {
                this.logList.ListItems = logItems;
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();


        }
    }
}
