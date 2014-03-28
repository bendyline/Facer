/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;

namespace BL.UI.App
{

    public class TemplateImageArea 
    {
        private String id;
        private bool isSelected;
        private bool isToggle;
        private String caption;
        private String hoverImageUrl;
        private String selectedImageUrl;

        public bool IsToggle
        {
            get
            {
                return this.isToggle;
            }

            set
            {
                this.isToggle = value;
            }
        }

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                this.isSelected = value;
            }
        }
        public String Caption
        {
            get
            {
                return this.caption;
            }

            set
            {
                this.caption = value;
            }
        }

        public String Id
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
            }
        }

        public String HoverImageUrl
        {
            get
            {
                return this.hoverImageUrl;
            }

            set
            {
                this.hoverImageUrl = value;
            }
        }

        public String SelectedImageUrl
        {
            get
            {
                return this.selectedImageUrl;
            }

            set
            {
                this.selectedImageUrl = value;
            }
        }
    }
}
