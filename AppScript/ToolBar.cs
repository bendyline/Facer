/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */
 
using System;
using System.Collections.Generic;

namespace BL.UI.App
{
    public class ToolBar : ItemsControl
    {
        public event ControlEventHandler ItemClicked;
        public event ControlValueEventHandler ValueChanged;

        protected override void OnItemControlAdded(Control c)
        {
            if (c is ToolBarButton)
            {
                ((ToolBarButton)c).Clicked += new EventHandler(ToolBar_Clicked);
            }

            if (c is ToolBarPopoutButton)
            {
                ((ToolBarPopoutButton)c).Clicked += new StringEventHandler(ToolBarPopoutButton_Clicked);
                ((ToolBarPopoutButton)c).ValueChanged += new ControlValueEventHandler(ToolBarPopoutButton_ValueChanged);
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

        private void ToolBarPopoutButton_ValueChanged(object sender, ControlValueEventArgs e)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, e);
            }
        }


        private void ToolBarPopoutButton_Clicked(object sender, StringEventArgs e)
        {
            if (this.ItemClicked != null)
            {
                ControlEventArgs cea = new ControlEventArgs((Control)sender);

                cea.Id = e.Value;

                this.ItemClicked(this, cea);
            }
        }
    }
}
