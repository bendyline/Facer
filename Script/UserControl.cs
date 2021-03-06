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
using System.Serialization;

namespace BL.UI
{
    public class UserControl : Control
    {        
        private User user;
        private UserReference userReference;
        private bool connectToCurrentUser = false;
        private PropertyChangedEventHandler userPropertyChangedEventHandler;
        private PropertyChangedEventHandler userChangedEventHandler;

        protected bool ConnectToCurrentUser
        {
            get
            {
                return this.connectToCurrentUser;
            }

            set
            {
                if (this.connectToCurrentUser == value)
                {
                    return;
                }

                this.connectToCurrentUser = value;

                if (this.connectToCurrentUser)
                {
                    this.User = Context.Current.User;

                    Context.Current.UserChanged += Current_UserChanged;
                }
            }
        }

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

                this.user.PropertyChanged += user_PropertyChanged;

                this.OnUserUpdated();

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

                this.OnUserReferenceUpdated();

                this.Update();
            }
        }

        public Nullable<long> UserId
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
            set
            {
                UserReference ur = new UserReference();

                ur.Id = value;
                ur.NickName = String.Empty;

                this.UserReference = ur;
            }
        }
        public String UserUniqueKey
        {
            get
            {
                if (this.user != null)
                {
                    return this.user.UniqueKey;
                }

                if (this.userReference != null)
                {
                    return this.userReference.UniqueKey;
                }

                return null;
            }
            set
            {
                UserReference ur = new UserReference();

                ur.UniqueKey = value;
                ur.NickName = String.Empty;

                this.UserReference = ur;
            }
        }

        public String EffectiveNickName
        {
            get
            {
                if (this.user != null)
                {
                    return this.user.Summary;
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
            this.userChangedEventHandler = Current_UserChanged;
            this.userPropertyChangedEventHandler = user_PropertyChanged;
        }

        private void Current_UserChanged(object sender, PropertyChangedEventArgs e)
        {
            this.User = Context.Current.User;
        }

        protected void OnUserReferenceUpdated()
        {
            if (this.userReference != null && this.userReference.UniqueKey != null)
            {
                User user = UserManager.Current.EnsureUserByUniqueKey(this.userReference.UniqueKey);

                user.LoadUser(this.UserLoaded, null);
            }
            else if (this.userReference != null && this.userReference.Id != null)
            {
                User user = UserManager.Current.EnsureUserById((long)this.userReference.Id);

                user.LoadUser(this.UserLoaded, null);
            }
        }

        private void UserLoaded(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                this.User = (User)result.Data;
            }
        }

        
        private void user_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnUserUpdated();
        }

        protected virtual void OnUserUpdated()
        {

        }
    }
}
