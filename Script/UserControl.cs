/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Html;
using System.Diagnostics;
using jQueryApi;
using BL.UI;
using System.Runtime.CompilerServices;
using BL;
using System.Net;
using Bendyline.Base;
using System.Serialization;

namespace BL.UI
{
    public class UserControl : Control
    {        
        private User user;
        private UserReference userReference;

        public User User
        {
            get
            {
                return this.user;
            }

            set
            {
                if (this.user == value)
                {
                    return;
                }

                this.user = value;

                this.Update();
            }
        }

        public UserReference UserReference
        {
            get
            {
                return this.userReference;
            }

            set
            {
                if (this.userReference == value)
                {
                    return;
                }

                this.userReference = value;

                this.Update();
            }
        }

        public String EffectiveId
        {
            get
            {
                if (this.user != null)
                {
                    return this.user.Id;
                }

                if (this.userReference != null)
                {
                    return this.userReference.Id;
                }

                return null;
            }
        }
        public String EffectiveNickName
        {
            get
            {
                if (this.user != null)
                {
                    return this.user.NickName;
                }

                if (this.userReference != null)
                {
                    return this.userReference.NickName;
                }

                return null;
            }
        }

        public UserControl()
        {

        }
    }
}
