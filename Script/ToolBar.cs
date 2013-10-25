/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;

namespace BL.UI
{
    public class ToolBar : ItemsControl
    {
        public event ControlEventHandler ItemClicked;

        protected override void OnItemControlAdded(Control c)
        {
            if (c is ToolBarButton)
            {
                ((ToolBarButton)c).Clicked += new EventHandler(ToolBar_Clicked);
            }
        }

        private void ToolBar_Clicked(object sender, EventArgs e)
        {
            if (this.ItemClicked != null)
            {
                ControlEventArgs cea = new ControlEventArgs((Control)sender);

                this.ItemClicked(this, cea);
            }
        }
    }
}
