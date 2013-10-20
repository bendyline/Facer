/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;

namespace BL.JQM
{
    public class Link : jQueryControl
    {
        private String relation;
        private bool reverse = false;

        public override string Role
        {
            get { return "link"; }
        }

        public String Relation
        {
            get
            {
                return this.relation;
            }

            set
            {
                this.relation = value;

                this.Update();
            }
        }

        public bool Reverse
        {
            get
            {
                return this.reverse;
            }

            set
            {
                this.reverse = value;
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.relation != null)
            {
                this.Element.SetAttribute("data-rel", this.relation);
            }

            if (this.reverse)
            {
                this.Element.SetAttribute("data-direction", "reverse");
            }
        }

    }
}
