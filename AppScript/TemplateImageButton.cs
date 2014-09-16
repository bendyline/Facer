/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;

namespace BL.UI.App
{
    public enum TemplateImageTemplate
    {
        FourSliceBottomAlignedHalfCircle = 0
    }


    public class TemplateImageButton : Control
    {
        public event StringEventHandler Clicked;
        public event EventHandler OffspaceClicked;

        private String baseImageUrl;
        private List<TemplateImageArea> templateImageAreas;
        private List<Element> labels;

        private bool isRadioButtonToggle = false;
        private TemplateImageTemplate interfaceTemplate = TemplateImageTemplate.FourSliceBottomAlignedHalfCircle;

        [ScriptName("e_image")]
        private ImageElement imageElement;

        private int? imageWidth;
        private int? imageHeight;

        private int selectedZone = -1;

        public bool IsRadioButtonToggle
        {
            get
            {
                return this.isRadioButtonToggle;
            }

            set
            {
                this.isRadioButtonToggle = value;
            }
        }

        public int? ImageWidth
        {
            get
            {
                return this.imageWidth;
            }

            set
            {
                this.imageWidth = value;
            }
        }

        public int? ImageHeight
        {
            get
            {
                return this.imageHeight;
            }

            set
            {
                this.imageHeight = value;
            }
        }

        public String BaseImageUrl
        {
            get
            {
                return this.baseImageUrl;
            }

            set
            {
                this.baseImageUrl = value;

                this.Update();
            }
        }
        public TemplateImageTemplate InterfaceTemplate
        {
            get
            {
                return this.interfaceTemplate;
            }

            set
            {
                this.interfaceTemplate = value;
            }
        }

        public TemplateImageButton()
        {
            this.templateImageAreas = new List<TemplateImageArea>();
            this.labels = new List<Element>();
        }

        public TemplateImageArea GetArea(int index)
        {
            return this.templateImageAreas[index];
        }

        public void SetArea(int index, TemplateImageArea area)
        {
            while (this.templateImageAreas.Count <= index)
            {
                this.templateImageAreas.Add(new TemplateImageArea());
            }

            while (this.labels.Count <= index)
            {
                Element e = Document.CreateElement("DIV");
                if (this.Element != null)
                {
                    this.Element.AppendChild(e);
                }
                e.Style.Position = "absolute";
                e.ClassName = this.TypeId + "-label";
                this.labels.Add(e);
            }

            ElementUtilities.SetText(this.labels[index], area.Caption);

            this.templateImageAreas[index] = area;

            this.UpdateLabelPositions();
        }
        private void UpdateLabelPositions()
        {
            if (this.ImageWidth == null || this.ImageHeight == null)
            {
                return;
            }

            int i = 0;

            foreach (Element e in this.labels)
            {
                if (i == 0)
                {
                    e.Style.Left = Math.Floor((int)this.ImageWidth * .07) + "px";
                    e.Style.Top = Math.Floor((int)this.ImageHeight * .7) + "px";
                }
                else if (i == 1)
                {
                    e.Style.Left = Math.Floor((int)this.ImageWidth * .26) + "px";
                    e.Style.Top = Math.Floor((int)this.ImageHeight * .24) + "px";
                }
                else if (i == 2)
                {
                    e.Style.Left = Math.Floor((int)this.ImageWidth * .55) + "px";
                    e.Style.Top = Math.Floor((int)this.ImageHeight * .24) + "px";
                }
                else if (i == 3)
                {
                    e.Style.Left = Math.Floor((int)this.ImageWidth * .75) + "px";
                    e.Style.Top = Math.Floor((int)this.ImageHeight * .7) + "px";
                }

                e.Style.Width = Math.Floor((int)this.ImageWidth * .2) + "px";                
                e.Style.Height = Math.Floor((int)this.ImageHeight* .2) + "px";
                e.Style.VerticalAlign = "middle";
                e.Style.TextAlign = "center";

                i++;
            }

        }
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            this.UpdateSelectedZone();
        }

        protected override void OnInit()
        {
            base.OnInit();

            if (this.labels != null)
            {
                foreach (Element e in this.labels)
                {
                    this.Element.AppendChild(e);
                }
            }
            this.TrackInteractionEvents = true;
        }

        private int GetSelectedZone(ElementEvent e)
        {
            int i = 0;
            Element elt = ElementUtilities.GetEventTarget(e);

            foreach (Element labelElement in this.labels)
            {
                if (labelElement == elt)
                {
                    return i;
                }

                i++;
            }

            int selectedNumber = -1;

            if (this.InterfaceTemplate == TemplateImageTemplate.FourSliceBottomAlignedHalfCircle)
            {
                // is the mouse within the "deadzone" circle at the bottom?
                double dist = Math.Sqrt( Math.Pow((this.Element.OffsetWidth / 2) - e.OffsetX, 2) + Math.Pow(this.Element.OffsetHeight - e.OffsetY, 2));

                if (dist < 90)
                {
                    selectedNumber = -1;
                }
                else if (e.OffsetX < this.Element.OffsetWidth / 2)
                {
                    if (e.OffsetY > e.OffsetX)
                    {
                        selectedNumber = 0;
                    }
                    else
                    {
                        selectedNumber = 1;
                    }
                }
                else
                {
                    if ((this.Element.OffsetHeight - e.OffsetY) > (e.OffsetX - (this.Element.OffsetWidth / 2)))
                    {
                        selectedNumber = 2;
                    }
                    else
                    {
                        selectedNumber = 3;
                    }
                }
            }

            return selectedNumber;
        }

        protected override void OnMouseOver(ElementEvent e)
        {
            base.OnMouseOver(e);
        
            this.selectedZone = GetSelectedZone(e);

            this.UpdateSelectedZone();
        }

        protected override void OnMouseOut(ElementEvent e)
        {
            base.OnMouseOut(e);

            this.selectedZone = GetSelectedZone(e);

            this.UpdateSelectedZone();
        }

        protected override void OnMouseMove(ElementEvent e)
        {
            base.OnMouseMove(e);

            this.selectedZone = GetSelectedZone(e);

            this.UpdateSelectedZone();
        }

        private void UpdateSelectedZone()
        {
            if (this.selectedZone < 0)
            {
                this.imageElement.Src = this.BaseImageUrl;
            }
            else if (this.selectedZone >= 0)
            {
                if (this.templateImageAreas.Count >= this.selectedZone)
                {
                    this.imageElement.Src = this.templateImageAreas[this.selectedZone].HoverImageUrl;
                }
            }
        }

        protected override void OnClick(System.Html.ElementEvent e)
        {
            base.OnClick(e);

            int selectedNumber = GetSelectedZone(e); 

            if (this.Clicked != null && selectedNumber >= 0)
            {
                TemplateImageArea tia = this.templateImageAreas[selectedNumber];

                if (tia.IsToggle)
                {
                    tia.IsSelected = !tia.IsSelected;
                }

                StringEventArgs iea = new StringEventArgs(tia.Id);

                this.Clicked(this, iea);
            }
            else if (selectedNumber < 0)
            {
                this.OffspaceClicked(this, EventArgs.Empty);
            }
        }
    }
}
