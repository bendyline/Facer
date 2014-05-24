/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using BL.UI;

namespace BL
{
    public delegate void ControlIntegerEventHandler(object sender, ControlIntegerEventArgs e);

    public class ControlIntegerEventArgs : EventArgs
    {
        private Control c;
        private String id;
        private int selectedInt;

        public int SelectedInt
        {
            get
            {
                return this.selectedInt;
            }

            set
            {
                this.selectedInt = value;
            }
        }

        public String Id
        {
            get
            {
                if (this.id == null && c != null)
                {
                    return c.Id;
                }

                return this.id;
            }

            set
            {
                this.id = value;
            }
        }

        public Control Control
        {
            get { return this.c; }
            set { this.c= value; }
        }

        public ControlIntegerEventArgs(Control c, int selectedInt)
        {
            this.selectedInt = selectedInt;
            this.c = c;
        }       
    }
}
