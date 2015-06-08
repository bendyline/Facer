/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using Kendo.UI;
using BL.UI.KendoControls;

namespace BL.UI.KendoControls
{
    public class DropZoneTarget : Control
    {
        [ScriptName("e_dropArea")]
        private Element dropArea;

        [ScriptName("e_dropTop")]
        private Element dropTop;

        private Control nextControl;
        private Control currentControl;
        private Control previousControl;

        private DropTarget dropTarget;
        private int expandedHeight = 120;

        private bool isActive = false;
        private bool isHoveredOver = false;

        private HeightAnimator activeHeightAnimator;

        public event EventHandler DroppedOn;

        public int ExpandedHeight
        {
            get
            {
                return this.expandedHeight;
            }

            set
            {
                this.expandedHeight = value;
            }
        }

        public Control CurrentControl
        {
            get
            {
                return this.currentControl;
            }

            set
            {
                this.currentControl = value;
            }
        }

        public Control NextControl
        {
            get
            {
                return this.nextControl;
            }

            set
            {
                this.nextControl = value;
            }
        }

        public Control PreviousControl
        {
            get
            {
                return this.previousControl;
            }

            set
            {
                this.previousControl = value;
            }
        }

        public bool IsActive
        {
            get
            {
                return this.isActive;
            }

            set
            {
                if (this.isActive == value)
                {
                    return;
                }

                this.isActive = value;

                OpacityAnimator oa = new OpacityAnimator();
                oa.Element = this.Element;

                if (this.isActive)
                {
                    oa.From = 0;
                    oa.To = 1;
                }
                else
                {
                    oa.From = 1;
                    oa.To = 0;

                    if (this.isHoveredOver)
                    {
                        this.UnHover();
                    }
                }

                oa.Start(400, null, null);

                this.Update();
            }
        }
        public bool IsHoveredOver
        {
            get
            {
                return this.isHoveredOver;
            }

            set
            {
                if (this.isHoveredOver== value)
                {
                    return;
                }

                this.isHoveredOver = value;

                this.Update();
            }
        }

        public DropZoneTarget()
        {
            KendoUtilities.EnsureKendoBaseUx(this);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            DropTargetOptions dto = new DropTargetOptions();

            dto.Drop = this.HandleDrop;
            dto.DragEnter = this.HandleDragEnter;
            dto.DragLeave = this.HandleDragLeave;


            this.dropTarget = KendoUtilities.CreateDropTarget(this, dto);
        }

        private void HandleDragEnter(DropEventArgs eventArgs)
        {
            if (!this.IsActive)
            {
                return;
            }

            this.IsHoveredOver = true;

            if (this.activeHeightAnimator != null)
            {
                this.activeHeightAnimator.Cancel(true);
            }

            this.activeHeightAnimator = new HeightAnimator();
            this.activeHeightAnimator.From = 20;
            this.activeHeightAnimator.To  = this.expandedHeight;

            this.activeHeightAnimator.Element = this.Element;

            this.activeHeightAnimator.Start(300, null, null);
        }

        private void HandleDragLeave(DropEventArgs eventArgs)
        {
            if (!this.IsActive)
            {
                return;
            }

            this.UnHover();
        }

        private void UnHover()
        {

            this.IsHoveredOver = false;

            this.activeHeightAnimator = new HeightAnimator();
            this.activeHeightAnimator.From = this.expandedHeight;
            this.activeHeightAnimator.To = 20;

            this.activeHeightAnimator.Element = this.Element;

            this.activeHeightAnimator.Start(300, null, null);
        }

        private void HandleDrop(DropEventArgs eventArgs)
        {
            if (this.DroppedOn != null)
            {
                this.DroppedOn(this, EventArgs.Empty);
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.isActive)
            {

            }
        }
    }
}
