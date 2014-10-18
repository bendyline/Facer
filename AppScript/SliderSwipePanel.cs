/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using BL.Extern;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Html;
using System.Runtime.CompilerServices;

namespace BL.UI.App
{
    public enum SliderSwipeMode
    {
        WholePage = 0,
        LeftSlide = 1,
        RightSlide = 2
    }

    public class SliderSwipePanel : ItemsControl
    {
        [ScriptName("e_itemsBin")]
        private Element itemsBin;

        [ScriptName("e_topAreaOuter")]
        private Element topAreaOuter;

        [ScriptName("e_containerLinkBin")]
        private Element containerLinkBin;

        [ScriptName("e_containerLinkLeft")]
        private Element containerLinkLeft;

        [ScriptName("e_containerLinkRight")]
        private Element containerLinkRight;

        private SliderSwipeMode mode = SliderSwipeMode.WholePage;

        private List<bool> visibilities;

        public event ControlIntegerEventHandler ActiveControlChanged;

        private int activeIndex;
        private int previousIndex;

        private PaneSettingsCollection paneSettingsCollection;
        private List<String> linkTitles;

        private bool isConsideringDrag;
        private bool isDragging;
        private bool isAnimatingToSlot;

        private double panelWidth;

        private double lastDragEventTime;
        private Date animationStart;

        private double downEventPageX;
        private double downEventPageY;

        private ElementEvent lastMoveEvent;

        private int scrollAnimationTime = 500;
        private int gapBetweenSections = 0;

        private double initialScrollX;
        private double initialScrollY;
        private Action animationTickHandler;
        private ElementEventListener windowSizeChanged;

        private double fromX;
        private double toX;
        private bool isAnimating = false;

        private bool allowSwiping = true;
        private String interiorItemHeight;

        private ElementEventListener draggingElementMouseMoveHandler = null;
        private ElementEventListener draggingElementMouseUpHandler = null;
        private ElementEventListener draggingElementMouseOutHandler = null;

        public event IntegerEventHandler VerticalScrollChanged;
        public event EventHandler IndexChangeAnimationCompleted;

        public int PreviousIndex
        {
            get
            {
                return this.previousIndex;
            }
        }

        public PaneSettingsCollection Settings
        {
            get
            {
                return this.paneSettingsCollection;
            }
        }

        public SliderSwipeMode Mode
        {
            get
            {
                return this.mode;
            }

            set
            {
                if (this.mode == value)
                {
                    return;
                }

                this.mode = value;

                this.ApplyVisibility();
            }
        }

        [ScriptName("q_linkTitles")]
        public List<String> LinkTitles
        {
            get
            {
                if (this.linkTitles == null)
                {
                    this.linkTitles = new List<string>();
                }

                return this.linkTitles;
            }

            set
            {
                this.linkTitles = value;
                this.UpdateLinkBin();
            }
        }

        [ScriptName("b_allowSwiping")]
        public bool AllowSwiping
        {
            get
            {
                return this.allowSwiping;
            }
            set
            {
                this.allowSwiping = value;
            }
        }

        public bool IsAnimating
        {
            get
            {
                return this.isAnimating;
            }
        }

        public double InitialScrollY
        {
            get
            {
                return this.initialScrollY;
            }
            
            set
            {
                this.initialScrollY = value;
            }
        }

        public int ActiveIndex
        {
            get
            {
                return this.activeIndex;
            }

            set
            {
                if (this.activeIndex == value)
                {
                    this.SetFinalPosition();
                    return;
                }

                this.previousIndex = this.activeIndex;
                this.activeIndex = value;

                this.ApplyVisibility();
                this.AnimateToIndexPosition();

                this.UpdateLinkHighlights();

                if (this.containerLinkBin != null && this.containerLinkBin.ChildNodes.Length > this.activeIndex)
                {
                    Element activeTab = this.containerLinkBin.ChildNodes[this.activeIndex];
                    activeTab.Focus();
                }

                if (this.ActiveControlChanged != null)
                {
                    ControlIntegerEventArgs ciea = new ControlIntegerEventArgs(this.ItemControls[this.activeIndex], this.activeIndex);

                    this.ActiveControlChanged(this.activeIndex, ciea);
                }
            }
        }

        public String InteriorItemHeight
        {
            get
            {
                return this.interiorItemHeight;
            }

            set
            {
                this.interiorItemHeight = value;

                this.UpdateSizings();
                Window.SetTimeout(new Action(this.UpdateSizings), 1);
                Window.SetTimeout(new Action(this.UpdateSizings), 100);

            }
        }

        public SliderSwipePanel()
        {
            this.WrapItems = true;
            this.windowSizeChanged = this.UpdateSizingsEvent;

            this.paneSettingsCollection = new PaneSettingsCollection();

            this.visibilities = new List<bool>();
            this.animationTickHandler = new Action(this.AnimateTick);

            if (!Context.Current.IsTouchOnly)
            {
                Document.Body.AddEventListener("mouseout", this.HandleDragMouseOut, true);
            }

            this.draggingElementMouseMoveHandler = this.HandleElementMouseMove;
            this.draggingElementMouseUpHandler = this.HandleElementMouseUp;
        }

        public void SetActiveIndexImmediate(int newActiveIndex)
        {
            if (this.activeIndex == newActiveIndex)
            {
                this.SetFinalPosition();
                return;
            }

            this.previousIndex = this.activeIndex;
            this.activeIndex = newActiveIndex;

            this.ApplyVisibility();
            this.SetFinalPosition();

            this.UpdateLinkHighlights();

            if (this.containerLinkBin != null && this.containerLinkBin.ChildNodes.Length > this.activeIndex)
            {
                Element activeTab = this.containerLinkBin.ChildNodes[this.activeIndex];
                activeTab.Focus();
            }

            if (this.ActiveControlChanged != null)
            {
                ControlIntegerEventArgs ciea = new ControlIntegerEventArgs(this.ItemControls[this.activeIndex], this.activeIndex);

                this.ActiveControlChanged(this.activeIndex, ciea);
            }
        }

        public void SetVisible(int index, bool isVisible)
        {
            while (this.visibilities.Count < index)
            {
                this.visibilities.Add(true);
            }

            this.visibilities[index] = isVisible;

            this.ApplyVisibility();
        }

        private void ApplyVisibility()
        {
            for (int i = 0; i < this.ItemControls.Count; i++ )
            {
                bool vis = true;

                if (this.visibilities.Count > i)
                {
                    vis = this.visibilities[i];
                }

                if (this.containerLinkBin.ChildNodes.Length > i)
                {
                    Element elt = this.containerLinkBin.ChildNodes[i];

                    if (vis)
                    {
                        elt.Style.Display = String.Empty;
                    }
                    else
                    {
                        elt.Style.Display = "none";
                    }
                }
                if ((this.mode == SliderSwipeMode.WholePage && i <= this.activeIndex + 1) || (i <= this.activeIndex && (i <= this.previousIndex || this.activeIndex == i)))
                {
                    this.ItemControls[i].Visible = vis;
                }
                else
                {
                    this.ItemControls[i].Visible = false;
                }
            }
        }

        private void SetToX()
        {
            if (this.ActiveIndex == 0)
            {
                this.toX = 0;
            }
            else
            {
                double baseLeft = ElementUtilities.GetBoundingRect(this.Element).Left;

                double elementLeft = ElementUtilities.GetBoundingRect(this.ItemControls[this.ActiveIndex].Element).Left;

                if (this.mode == SliderSwipeMode.WholePage)
                {
                    double left = 0;

                    for (int i = 0; i < this.ActiveIndex; i++)
                    {
                        Element previousElement = this.ItemControls[i].Element;
                        ClientRect previousRect = ElementUtilities.GetBoundingRect(previousElement);

                        left += ((previousRect.Right - previousRect.Left) + (this.gapBetweenSections) + 1);
                    }

                    this.toX = left;// ((elementLeft + this.itemsBin.ScrollLeft) - baseLeft);
                }
                else
                {
                    double left = 0;

                    for (int i=0; i<this.PreviousIndex; i++)
                    {
                        Element previousElement = this.ItemControls[this.PreviousIndex].Element;
                        ClientRect previousRect = ElementUtilities.GetBoundingRect(previousElement);

                        left += (previousRect.Right - previousRect.Left);
                    }

                    this.toX = left + 300;
                }
            }
        }


        public void SetLinkTitle(int index, String title)
        {
            if (this.linkTitles == null || index >= this.linkTitles.Count)
            {
                return;
            }

            this.linkTitles[index] = title;

            this.UpdateLinkBin();
        }

        private void AnimateToIndexPosition()
        {
            this.SetToX();

            if (this.isAnimating)
            {
                return;
            }
            
            this.isAnimating = true;

            this.fromX = this.itemsBin.ScrollLeft;

            this.animationStart = Date.Empty;

            ElementUtilities.AnimateOnNextFrame(this.animationTickHandler);
        }

        private void AnimateTick()
        {
            Date now = Date.Now;

            if (this.animationStart == Date.Empty)
            {
                this.animationStart = now;
            }

            int ms = now.GetTime() - this.animationStart.GetTime();
            double proportion = Easing.EaseInOutQuart(ms, 0, 1, this.scrollAnimationTime );
            
            Debug.WriteLine("SliderSwipePanel Animation Frame @ " + ms + " pos: " + proportion);
            
            if (proportion < 1 && proportion > -1 && ms <= this.scrollAnimationTime)
            {
                this.SetPanelLeft(this.fromX + ((this.toX - this.fromX) * proportion));

                ElementUtilities.AnimateOnNextFrame(this.animationTickHandler);
            }
            else
            {
                this.SetFinalPosition();
                this.isAnimating = false;

                if (this.IndexChangeAnimationCompleted != null)
                {
                    this.IndexChangeAnimationCompleted(this, EventArgs.Empty);
                }
            }
        }

        private void SetFinalPosition()
        {
            this.SetToX();

            this.SetPanelLeft(this.toX);
        }

        private void SetPanelLeft(double left)
        {
      //      Script.Literal("{0}.style.transform=\"translate(-\" + {1} + \"px,0px)\"", this.itemsBin, left);

            //this.itemsBin.Style.Transform = "translate(" + left + "px,0px)";

      //      transform: translate(3em,0);

            if (this.itemsBin == null)
            {
                return;
            }

            this.itemsBin.ScrollLeft = (int)left;
        }

        private void UpdateLinkBin()
        {
            if (this.containerLinkBin == null)
            {
                return;
            }

            while (this.containerLinkBin.ChildNodes.Length > 0)
            {
                this.containerLinkBin.RemoveChild(this.containerLinkBin.ChildNodes[0]);
            }

            if (this.linkTitles == null || this.linkTitles.Count == 0)
            {
                this.topAreaOuter.Style.Display = "none";
                return;
            }
            else
            {
                this.topAreaOuter.Style.Display = "block";
            }

            if (Context.Current.IsSmallFormFactor && this.topAreaOuter != null)
            {
                this.topAreaOuter.Style.Display = "none";
                return;
            }

            int i = 0;

            foreach (String linkTitle in this.linkTitles)
            {
                Element linkTitleElement = null;


                linkTitleElement = this.CreateElement(this.GetClassNameForElement(i));

                linkTitleElement.TabIndex = 1;
                ElementUtilities.SetText(linkTitleElement, linkTitle);

                linkTitleElement.SetAttribute("linkIndex", i);
                linkTitleElement.AddEventListener("click", this.HandleLinkClick, true);

                this.containerLinkBin.AppendChild(linkTitleElement);

                i++;
            }

            this.ApplyVisibility();
        }

        private String GetClassNameForElement(int index)
        {

            String cssBase = "swipeLink";

            if (index == this.activeIndex)
            {
                cssBase += " selected";
            }

            if (index == this.LinkTitles.Count - 1)
            {
                cssBase += " lastLink";

                if (index == this.activeIndex)
                {
                    cssBase += " lastLinkSelected";
                }
            }
            else if (index == 0)
            {
                cssBase += " firstLink";

                if (index == this.activeIndex)
                {
                    cssBase += " firstLinkSelected";
                }
            }
            else if (index == 1)
            {
                cssBase += " secondLink";

                if (index == this.activeIndex)
                {
                    cssBase += " secondLinkSelected";
                }
            }
            else
            {
                cssBase += " innerLink";

                if (index == this.activeIndex)
                {
                    cssBase += " innerLinkSelected";
                }
            }

            return cssBase;
        }

        private void UpdateLinkHighlights()
        {
            if (previousIndex >= 0)
            {
                Element secondaryTab = this.containerLinkBin.ChildNodes[this.previousIndex];

                if (secondaryTab != null)
                {
                    secondaryTab.ClassName = this.GetElementClass(this.GetClassNameForElement(previousIndex));
                }
            }

            Element primaryTab = this.containerLinkBin.ChildNodes[this.activeIndex];

            if (primaryTab != null)
            {
                primaryTab.ClassName = this.GetElementClass(this.GetClassNameForElement(this.activeIndex));
            }
        }

        private void HandleLinkClick(ElementEvent e)
        {
            object val = e.Target.GetAttribute("linkIndex");

            if (val != null && val is String)
            {
                this.ActiveIndex = Int32.Parse((String)val);
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Context.Current.IsSmallFormFactor)
            {
//                this.gapBetweenSections = 10;
            }


            if (Context.Current.IsTouchOnly)
            {
                Debug.WriteLine("(SliderSwipePanel::OnApplyTemplate) - Registering touch events " + ElementUtilities.GetTouchStartEventName());
                this.Element.AddEventListener(ElementUtilities.GetTouchStartEventName(), this.HandleElementMouseDown, true);

                Document.Body.AddEventListener(ElementUtilities.GetTouchMoveEventName(), this.draggingElementMouseMoveHandler, true);
                Document.Body.AddEventListener(ElementUtilities.GetTouchEndEventName(), this.draggingElementMouseUpHandler, true);

                if (ElementUtilities.GetTouchCancelEventName() != null)
                {
                    Document.Body.AddEventListener(ElementUtilities.GetTouchCancelEventName(), this.draggingElementMouseUpHandler, true);
                }
            }
            else
            {
                Debug.WriteLine("(SliderSwipePanel::OnApplyTemplate) - Registering mouse events ");

                this.Element.AddEventListener("mousedown", this.HandleElementMouseDown, true);
                this.Element.AddEventListener("mousemove", this.HandleElementMouseMove, true);
                this.Element.AddEventListener("mouseup", this.HandleElementMouseUp, true);
                this.Element.AddEventListener("dragstart", this.HandleDragStartEvent, true);
            }
            
            this.UpdateLinkBin();

            this.UpdateSizings();

            Window.SetTimeout(new Action(this.UpdateSizings), 1);
            Window.SetTimeout(new Action(this.UpdateSizings), 100);

        }

        protected override void OnVisibilityChanged()
        {
            base.OnVisibilityChanged();

            if (this.Visible)
            {
                Window.AddEventListener("resize", this.windowSizeChanged);
            }
            else
            {
                Window.RemoveEventListener("resize", this.windowSizeChanged);
            }
        }

        protected override void OnItemControlAdded(Control c)
        {
            base.OnItemControlAdded(c);

            c.Visible = true;

            this.UpdateSizings();

            Window.SetTimeout(new Action(this.UpdateSizings), 1);
            Window.SetTimeout(new Action(this.UpdateSizings), 100);
        }

        private bool IsDefaultInputElement(ElementEvent e)
        {
            String targetTagName = e.Target.TagName.ToLowerCase();

            object contentEditable = e.Target.GetAttribute("contenteditable");

            if (targetTagName == "input" || targetTagName == "select" || targetTagName == "textarea" || contentEditable == "true")
            {
                return true;
            }

            return false;
        }

        private void HandleElementMouseDown(ElementEvent e)
        {
            if (IsDefaultInputElement(e))
            {
                return;
            }

            if (this.allowSwiping && !this.isDragging && !this.isConsideringDrag)
            {
                Debug.WriteLine("(SliderSwipePanel::HandleElementMouseDown) - Starting drag");

           //     e.PreventDefault();

                this.isConsideringDrag = true;

                this.downEventPageX = ElementUtilities.GetPageX(e);
                this.downEventPageY = ElementUtilities.GetPageY(e);

                this.initialScrollX = this.itemsBin.ScrollLeft;
            }
            else
            {
                Debug.WriteLine("(SliderSwipePanel::HandleElementMouseDown) - Not starting drag");
            }
        }

        private void HandleDragMoveDeadTimeout()
        {
            int now = Date.Now.GetTime();

            if (now - this.lastDragEventTime > 100 && this.isDragging)
            {
                this.HandleElementMouseUp(null);
            }
        }

        private void HandleElementMouseMove(ElementEvent e)
        {
            if (IsDefaultInputElement(e))
            {
                return;
            }
            this.lastMoveEvent = e;


            if (this.isDragging)
            {
                e.PreventDefault();

                this.lastDragEventTime = Date.Now.GetTime();

                Window.SetTimeout(this.HandleDragMoveDeadTimeout, 100);

                int newLeft = (int)Math.Floor(this.initialScrollX + (this.downEventPageX - ElementUtilities.GetPageX(e)));
                Debug.WriteLine("(SliderSwipePanel::HandleElementMouseMove) - Mouse Move drag: " + newLeft);

                this.SetPanelLeft(newLeft);

                if (this.VerticalScrollChanged != null)
                {
                    int newTop = (int)Math.Floor(this.initialScrollY+ (this.downEventPageY - ElementUtilities.GetPageY(e)));

                    IntegerEventArgs iea = new IntegerEventArgs(newTop);

                    this.VerticalScrollChanged(this, iea);
                }
            }
            else
            {
                if (this.isConsideringDrag)
                {
                    double diffX = Math.Abs((this.downEventPageX - ElementUtilities.GetPageX(this.lastMoveEvent)));
                    double diffY = Math.Abs((this.downEventPageY - ElementUtilities.GetPageY(this.lastMoveEvent)));

                    if (diffX != 0 || diffY != 0)
                    {
                        this.ConsiderStartDragging();
                    }
                }

                Debug.WriteLine("(SliderSwipePanel::HandleElementMouseMove) - Not dragging");
            }
        }

        private void ConsiderStartDragging()
        {
            if (!this.isConsideringDrag || this.isDragging)
            {
                return;
            }

            double diffX = Math.Abs((this.downEventPageX - ElementUtilities.GetPageX(this.lastMoveEvent)));
            double diffY = Math.Abs((this.downEventPageY - ElementUtilities.GetPageY(this.lastMoveEvent)));

            if (diffX > diffY)
            {
                this.isDragging = true;

                Debug.WriteLine("(SliderSwipePanel::ConsiderStartDragging)");
            }
            else
            {
                this.isConsideringDrag = false;

                Debug.WriteLine("(SliderSwipePanel::ConsiderStartDragging) - Failed CSD check" + diffX + diffY);
            }
        }

        private void HandleDragMouseOut(ElementEvent e)
        {
            if (this.isDragging)
            {
                e.PreventDefault();
            }

            if (!this.AllowSwiping || !this.isDragging)
            {
                return;
            }

            Debug.WriteLine("(SliderSwipePanel::HandleDragMouseOut)");
            // has the mouse left the window?
            if ((e.ToElement == null && !Context.Current.IsTouchOnly) || (e.ToElement != null && e.ToElement.NodeName == "HTML"))
            {
                this.HandleElementMouseUp(e);
            }

            e.CancelBubble = true;
        }

        private void HandleElementMouseUp(ElementEvent e)
        {
            this.isConsideringDrag = false;
            Debug.WriteLine("(SliderSwipePanel::MouseUp)");

            if (this.isDragging)
            {
                if (e != null)
                {
                    e.PreventDefault();
                }

                Debug.WriteLine("(SliderSwipePanel::ConsiderStartDragging) - Dragging");
         /*       if (Context.Current.IsTouchOnly)
                {
                   Document.Body.RemoveEventListener(ElementUtilities.GetTouchMoveEventName(), this.draggingElementMouseMoveHandler, true);
                   Document.Body.RemoveEventListener(ElementUtilities.GetTouchEndEventName(), this.draggingElementMouseUpHandler, true);
                }*/

                this.isDragging = false;
                double diffX = this.downEventPageX - ElementUtilities.GetPageX(this.lastMoveEvent);


                if (diffX > 8 && this.ActiveIndex < this.ItemControls.Count - 1) //this.itemsBin.ScrollLeft > (initialScrollX + (panelWidth / 4)) && this.ActiveIndex < this.ItemControls.Count - 1)
                {
                    this.ActiveIndex++;
                }
                else if (diffX < -8 && this.ActiveIndex > 0) //this.itemsBin.ScrollLeft < (initialScrollX - (panelWidth / 4)) && this.ActiveIndex > 0)
                {
                    this.ActiveIndex--;
                }
                else
                {
                    this.AnimateToIndexPosition();
                }
            }
/*
                int newIndex = Math.Floor((this.itemsBin.ScrollLeft + panelWidth / 2) / panelWidth);

                if (newIndex != this.ActiveIndex)
                {
                    this.ActiveIndex = newIndex;
                }
                else
                {
                         }*/
        }

        private void HandleDragStartEvent(ElementEvent e)
        {
            e.CancelBubble = true;

            e.PreventDefault();
            e.StopPropagation();
        }

        protected override void OnDimensionChanged()
        {
            base.OnDimensionChanged();

            this.UpdateSizings();

            Window.SetTimeout(new Action(this.UpdateSizings), 1);
            Window.SetTimeout(new Action(this.UpdateSizings), 100);
        }

        public void UpdateSizingsEvent(ElementEvent e)
        {
            this.UpdateSizings();

            Window.SetTimeout(new Action(this.UpdateSizings), 1);
            Window.SetTimeout(new Action(this.UpdateSizings), 100);
        }

        public void UpdateSizings()
        {
            if (this.Element == null)
            {
                return;
            }

            ClientRect cr = ElementUtilities.GetBoundingRect(this.Element);
            double height = 0;
            
            if (this.Height != null)
            {
                height = (double)this.Height;
            }
            else if (this.topAreaOuter != null)
            {
                ClientRect topAreaRect = ElementUtilities.GetBoundingRect(this.topAreaOuter);

                height = (cr.Bottom - cr.Top) -  (topAreaRect.Bottom - topAreaRect.Top);
            }
            else
            {
                height = (cr.Bottom - cr.Top);
            }

            this.panelWidth = Window.InnerWidth;
            int index = 0;

            foreach (Control c in this.ItemControls)
            {
                PaneSettings ps = this.Settings[index];

                if (c.Element != null)
                {
                    Style style = c.Element.Style;

                    double width = ((cr.Right - cr.Left) - this.gapBetweenSections) + 2;

                    style.MinWidth = width.ToString() + "px";
                    style.Width = width.ToString() + "px";
                    style.MarginRight = this.gapBetweenSections + "px";


                    if (this.InteriorItemHeight != null)
                    {
                        style.MaxHeight = this.InteriorItemHeight;
                        style.MinHeight = this.InteriorItemHeight;
                        style.Height = this.InteriorItemHeight;
                    }
                    else if (ps.FitToHeight == false)
                    {
                        style.Height = null;
                    }
                    else
                    {
                        style.Height = (height - 4).ToString() + "px";
                    }

                    c.Element.ParentNode.Style.Height = "100%";
                }

                index++;
            }

            if (!this.isAnimating)
            {
                this.SetFinalPosition();
            }
        }
    }
}
