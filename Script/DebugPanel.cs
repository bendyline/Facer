
using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;

namespace BL.UI
{
    public class DebugPanel : Control
    {
        [ScriptName("c_logList")]
        private List logList;

        private int logItemId = 0;
        private ContentItemSet logItems;

        [ScriptName("e_collapse")]
        private Element collapse;

        private bool isExpanded = true;

        public event EventHandler Toggled;

        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }

            set
            {
                this.isExpanded = value;

                if (this.isExpanded == value)
                {
                    this.Element.Style.PosWidth = 280;
                }
                else
                {
                    this.Element.Style.PosWidth = 0;
                }

                if (this.Toggled != null)
                {
                    this.Toggled(this, EventArgs.Empty);
                }
            }
        }

        public DebugPanel()
        {
            this.logItems = new ContentItemSet();

            Log.ItemAdded += new LogItemEventHandler(Log_ItemAdded);

            this.TrackResizeEvents = true;

        }

        protected override void OnInit()
        {
            base.OnInit();
            
            Window.AddEventListener("resize", this.HandleSizeChanged, true);
        }


        private void HandleSizeChanged(ElementEvent e)
        {
            this.OnResize();
        }

        protected override void OnResize()
        {
            base.OnResize();

            this.logList.Element.Style.Height = Window.InnerHeight + "px";
            this.collapse.Style.PosLeft = Window.InnerWidth - 30;
        }

        private void Log_ItemAdded(object sender, LogItemEventArgs e)
        {
            ContentItem ci = new ContentItem();

            ci.Id = logItemId.ToString();
            ci.Title = e.Item.Message;

            this.logItems.Items.Insert(0, ci);

            logItemId++;

            if (this.logList != null)
            {
                this.logList.NotifyItemUpdated(ci);
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.logList != null)
            {
                this.logList.ListItems = logItems;
            }

            if (this.collapse != null)
            {
                this.collapse.AddEventListener("click", this.CollapseClick, true);
            }
        }

        private void CollapseClick(ElementEvent ee)
        {
            this.IsExpanded = !this.IsExpanded;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();


        }
    }
}
