/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using BL.UI;

namespace BL.UI
{
    public delegate void ControlEventHandler(object sender, ControlEventArgs e);

    public class ControlEventArgs : EventArgs
    {
        private Control c;
        private String id;
        private String idWithinParent;


        public String IdWithinParentControl
        {
            get
            {
                if (this.idWithinParent == null && c != null)
                {
                    return c.IdWithinParentControl;
                }

                return this.idWithinParent;
            }

            set
            {
                this.idWithinParent = value;
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

        public ControlEventArgs(Control c)
        {
            this.c = c;
        }       
    }
}
