/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */
 
using System;
using System.Collections.Generic;

namespace BL.UI.App
{
    public class TabBar : ItemsControl
    {
        public event ControlEventHandler ItemClicked;
        private TabBarButton lastControl;

        protected override void OnItemControlAdded(Control c)
        {
            if (c is TabBarButton)
            {
                ((ToolBarButton)c).Clicked += new EventHandler(TabBar_Clicked);
            }

        }

        private void TabBar_Clicked(object sender, EventArgs e)
        {
            TabBarButton newControl = (TabBarButton)sender;

            if (this.lastControl != null)
            {
                this.lastControl.Toggled = false;
            }

            this.lastControl = newControl;

            if (this.ItemClicked != null)
            {
                ControlEventArgs cea = new ControlEventArgs(newControl);

                this.ItemClicked(this, cea);
            }
        }
    }
}
