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

    public enum SliderSwipeNavigationPositioning
    {
        BottomCenter = 2,
        BottomLeft = 1
    }

    public class SliderSwipePanel : ItemsControl
    {
        [ScriptName("e_itemsBin")]
        private Element itemsBin;

        [ScriptName("e_topAreaOuter")]
        private Element topAreaOuter;

        [ScriptName("e_containerLinkBin")]
        private Element containerLinkBin;

        [ScriptName("e_swipeGuideRight")]
        private Element swipeGuideRight;

        [ScriptName("e_swipeNavigation")]
        private Element swipeNavigation;

        [ScriptName("e_swipeNavigationTitle")]
        private Element swipeNavigationTitle;

        [ScriptName("e_swipeNavigationDotBin")]
        private Element swipeNavigationDotBin;

        private SliderSwipeMode mode = SliderSwipeMode.WholePage;

        private List<bool> visibilities;

        public event ControlIntegerEventHandler ActiveControlChanged;

        private int activeIndex;
        private int previousIndex;
        private bool useFullWindow = false;

        private PaneSettingsCollection paneSettingsCollection;
        private List<String> linkTitles;

        private bool isConsideringDrag;
        private bool isDragging;

        private int lastFlashIndex = -1;
        private double panelWidth;

        private double lastDragEventTime;
        private Date animationStart;

        private double downEventPageX;
        private int swipeGuideCount = 0;
        private bool displayingSwipeNavigation = false;
        private bool displayLinkBar = true;
        private double downEventPageY;
        private bool alwaysDisplaySwipeNavigation = false;

        private OpacityAnimator swipeNavigationOpacityAnimator;

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
        private bool displaySwipeNavigationTitle = true;

        private bool allowSwiping = true;
        private String interiorItemHeight;

        private ElementEventListener draggingElementMouseMoveHandler = null;
        private ElementEventListener draggingElementMouseUpHandler = null;
        private ElementEventListener draggingElementMouseOutHandler = null;

        public event IntegerEventHandler VerticalScrollChanged;
        public event EventHandler IndexChangeAnimationCompleted;

        private int swipeNavigationOffsetY= 124;
        private int swipeNavigationOffsetX = 0;
        private SliderSwipeNavigationPositioning swipeNavigationPositioning = SliderSwipeNavigationPositioning.BottomCenter;

        private String accentColor;

        [ScriptName("b_displaySwipeNavigationTitle")]
        public bool DisplaySwipeNavigationTitle
        {
            get
            {
                return this.displaySwipeNavigationTitle;
            }

            set
            {
                this.displaySwipeNavigationTitle = value;
            }
        }

        [ScriptName("i_swipeNavigationOffsetX")]
        public int SwipeNavigationOffsetX
        {
            get
            {
                return this.swipeNavigationOffsetX;
            }

            set
            {
                this.swipeNavigationOffsetX = value;

                this.UpdateSizingsOverTime();
            }
        }

        [ScriptName("i_swipeNavigationOffsetY")]
        public int SwipeNavigationOffsetY
        {
            get
            {
                return this.swipeNavigationOffsetY;
            }

            set
            {
                this.swipeNavigationOffsetY = value;

                this.UpdateSizingsOverTime();
            }
        }

        [ScriptName("i_swipeNavigationPositioning")]
        public SliderSwipeNavigationPositioning SwipeNavigationPositioning
        {
            get
            {
                return this.swipeNavigationPositioning;
            }

            set
            {
                this.swipeNavigationPositioning = value;

                this.UpdateSizingsOverTime();
            }
        }

        [ScriptName("b_alwaysDisplaySwipeNavigation")]
        public bool AlwaysDisplaySwipeNavigation
        {
            get
            {
                return this.alwaysDisplaySwipeNavigation;
            }

            set
            {
                if (this.alwaysDisplaySwipeNavigation == value)
                {
                    return;
                }

                this.alwaysDisplaySwipeNavigation = value;

                this.FlashSwipeNavigation();
            }
        }

        [ScriptName("b_useFullWindow")]
        public bool UseFullWindow
        {
            get
            {
                return this.useFullWindow;
            }

            set
            {
                this.useFullWindow = value;
            }
        }

        public String AccentColor
        {
            get
            {
                return this.accentColor;
            }

            set
            {
                if (this.accentColor == value)
                {
                    return;
                }

                this.accentColor = value;

                this.UpdateAccentColor();
            }
        }

        [ScriptName("b_displayLinkBar")]
        public bool DisplayLinkBar
        {
            get
            {
                return this.displayLinkBar;
            }

            set
            {
                this.displayLinkBar = value;
            }
        }

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

                if (this.mode == SliderSwipeMode.RightSlide && value == SliderSwipeMode.WholePage)
                {
                    this.RevertToPreviousWholePage();
                }
                else
                {
                    this.mode = value;

                    this.ApplyVisibility();
                    this.UpdateSizingsOverTime();
                    this.SetToX();
                }
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
                if (this.allowSwiping == value)
                {
                    return;
                }

                this.allowSwiping = value;

                this.UpdateSwiping();
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
                Log.DebugMessage("Setting swipe panel active index to " + value + " from " + this.activeIndex);

                if (this.activeIndex == value)
                {
                    this.SetFinalPosition();
                    return;
                }

                this.previousIndex = this.activeIndex;
                this.activeIndex = value;

                this.FlashSwipeNavigation();

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

                this.UpdateSizingsOverTime();
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
            this.draggingElementMouseUpHandler = this.HandlePointerUp;
        }

        public void RevertToPreviousWholePage()
        {
            this.mode = SliderSwipeMode.WholePage;

            this.ActiveIndex = this.PreviousIndex;

            this.previousIndex = -1;
            this.ApplyVisibility();
            this.UpdateSizingsOverTime();
            this.SetToX();
        }

        public void FocusOnRightSlidePanel(int index)
        {
            if (this.mode == SliderSwipeMode.RightSlide)
            {
                this.RevertToPreviousWholePage();
            }

            this.mode = SliderSwipeMode.RightSlide;

            this.ActiveIndex = index;
        }

        public void ConsiderShowingSwipeGuidelines()
        {
            if (  ( this.allowSwiping && 
                    Context.Current.IsTouchOnly && 
                    swipeGuideCount < 3 )
                  && this.swipeGuideRight != null 
                  && this.swipeGuideRight.Style.Display != "block")
            {
                this.swipeGuideRight.Style.Display = "block";

                OpacityAnimator oa = new OpacityAnimator();
                oa.Element = this.swipeGuideRight;
                oa.From = 1;
                oa.To = 0;
                oa.StartAfter(3000, 500, this.HideSwipeGuide, null);

                this.UpdateSizingsOverTime();

                this.swipeGuideCount++;
            }
        }

        private void HideSwipeGuide(IAsyncResult result)
        {
            if (this.swipeGuideRight != null)
            {
                this.swipeGuideRight.Style.Display = "none";
            }
        }

        public void HideSwipePanels()
        {
            this.HideSwipeNavigation(null);
            this.HideSwipeGuide(null);
        }

        private void FlashSwipeNavigation()
        {
            if (    (  this.alwaysDisplaySwipeNavigation || 
                        (   this.allowSwiping &&
                            Context.Current.IsTouchOnly &&
                            this.lastFlashIndex != this.ActiveIndex
                        )
                    ) &&
                    this.swipeNavigation != null && 
                    this.swipeNavigationTitle != null)
            {
                this.swipeNavigation.Style.Display = "block";

                this.UpdateSwipeNavigationLinkState();

                if (this.swipeNavigationOpacityAnimator == null)
                {
                    this.swipeNavigationOpacityAnimator = new OpacityAnimator();
                }

                this.lastFlashIndex = this.ActiveIndex;

                if (!this.alwaysDisplaySwipeNavigation)
                {
                    this.swipeNavigationOpacityAnimator.Element = this.swipeNavigation;
                    this.swipeNavigationOpacityAnimator.From = 1;
                    this.swipeNavigationOpacityAnimator.To = 0;
                    this.swipeNavigationOpacityAnimator.StartAfter(3000, 500, this.HideSwipeNavigation, null);
                }

                this.UpdateSizingsOverTime();

                this.displayingSwipeNavigation = true;
            }
        }
        private void HideSwipeNavigation(IAsyncResult result)
        {

            if (this.swipeNavigation != null)
            {
                this.swipeNavigation.Style.Display = "none";
            }
        }

        private void UpdateSwipeNavigationLinkState()
        {
            if (this.displaySwipeNavigationTitle)
            {
                this.swipeNavigationTitle.InnerText = this.LinkTitles[this.ActiveIndex];
                this.swipeNavigationTitle.Style.Display = "";
            }
            else
            {
                this.swipeNavigationTitle.Style.Display = "none";
            }

            ElementUtilities.ClearChildElements(this.swipeNavigationDotBin);

            for (int i=0; i<this.ItemControls.Count; i++)
            {
                if (this.GetVisible(i))
                {
                    Element dot = null;

                    if (i == this.activeIndex)
                    {
                        dot = this.CreateElementWithType("swipeDotBase swipeDotBaseActive", "SPAN");
                    }
                    else
                    {
                        dot = this.CreateElementWithType("swipeDotBase swipeDotBaseInactive", "SPAN");
                    }

                    dot.InnerHTML = "&#160;";

                    this.swipeNavigationDotBin.AppendChild(dot);
                }
            }
        }


        public void SetActiveIndexImmediate(int newActiveIndex)
        {
            Log.DebugMessage("Active index " + newActiveIndex + " from " + this.activeIndex);
            this.FlashSwipeNavigation();

            if (this.activeIndex == newActiveIndex)
            {
                this.ApplyVisibility();
                this.UpdateSizings();
                this.SetFinalPosition();
                return;
            }

            this.previousIndex = this.activeIndex;
            this.activeIndex = newActiveIndex;

            this.ApplyVisibility();
           
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

            this.UpdateSizingsOverTime();
        }


        public bool GetVisible(int index)
        {
            if (index >= this.visibilities.Count)
            {
                return true;
            }

            return this.visibilities[index];
        }

        public void SetVisible(int index, bool isVisible)
        {
            while (this.visibilities.Count < index)
            {
                this.visibilities.Add(true);
            }

            this.visibilities[index] = isVisible;

            this.ApplyVisibility();
            this.UpdateSizingsOverTime();

            this.FlashSwipeNavigation();
        }

        private void UpdateSwiping()
        {
            if (this.Element == null)
            {
                return;
            }

            if (this.allowSwiping)
            {
                // block the default touch action, which would cause the whole page to swipe
                ElementUtilities.SetTouchAction(this.Element, "none");
            }
            else
            {
                ElementUtilities.SetTouchAction(this.Element, "pan-y");
            }
        }

        private int GetNextPage(int index)
        {
            for (int i = index + 1; i < this.ItemControls.Count; i++)
            {
                if (this.visibilities.Count <= i || this.visibilities[i])
                {
                    return i;
                }
            }

            return -1;
        }
        private int GetPreviousPage(int index)
        {
            for (int i = index -1; i >= 0; i--)
            {
                if (this.visibilities.Count <= i || this.visibilities[i])
                {
                    return i;
                }
            }

            return -1;
        }

        private void ApplyVisibility()
        {
            if (this.containerLinkBin == null)
            {
                return;
            }

            int maxIndexToShow = this.activeIndex;

            if (this.allowSwiping)
            {
                if (this.GetNextPage(maxIndexToShow) >= 0)
                {
                    maxIndexToShow++;
                }
            }

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

                if (this.paneSettingsCollection[i].Visible && 
                    (
                        (this.mode == SliderSwipeMode.WholePage && i <= maxIndexToShow) || 
                        (i <= Math.Max(this.activeIndex, this.previousIndex) && (i <= this.previousIndex || this.activeIndex == i))
                    )
                  )
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

                Element activeElement = this.ItemControls[this.ActiveIndex].Element;
                double elementLeft = 0;

                if (activeElement != null)
                {
                    elementLeft = ElementUtilities.GetBoundingRect(activeElement).Left;
                }

                if (this.mode == SliderSwipeMode.WholePage)
                {
                    double left = 0;

                    for (int i = 0; i < this.ActiveIndex; i++)
                    {
                        Element previousElement = this.ItemControls[i].Element;
                        if (previousElement != null)
                        {
                            ClientRect previousRect = ElementUtilities.GetBoundingRect(previousElement);

                            if ((this.visibilities.Count <= i || this.visibilities[i]) && this.ItemControls[i].Visible)
                            {
                                left += ((previousRect.Right - previousRect.Left) + (this.gapBetweenSections));
                            }
                        }
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

                        if ((this.visibilities.Count <= i || this.visibilities[i]) && this.ItemControls[i].Visible)
                        {
                            left += (previousRect.Right - previousRect.Left);
                        }
                    }

                    this.toX = left + GetSlideoutWidth();
                }
            }
        }

        private void UpdateAccentColor()
        {
            if (this.topAreaOuter == null)
            {
                return;
            }

            this.topAreaOuter.Style.BackgroundColor = this.accentColor;
        }


        public int GetSlideoutWidth()
        {
            if (Window.InnerWidth < 768)
            {
                return Window.InnerWidth;
            }

            return 300;
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

            // Log.DebugMessage("SliderSwipePanel Animation Frame @ " + ms + " pos: " + proportion);
            
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

            // Log.DebugMessage("SetFinalPosition: Setting swipe panel position to " + this.toX + "|" + this.activeIndex);

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

            if (this.linkTitles == null || this.linkTitles.Count == 0 || !this.displayLinkBar)
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
            this.UpdateSizingsOverTime();
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

            cssBase += " link" + this.activeIndex;

            if (index == this.activeIndex)
            {
                cssBase += " link" + this.activeIndex + "Selected";
            }
            
            return cssBase;
        }

        private void UpdateLinkHighlights()
        {
            if (this.containerLinkBin == null)
            {
                return;
            }

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

            this.UpdateSwipeNavigationLinkState();
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


            if (Context.Current.IsTouchOnly || ElementUtilities.GetIsPointerEnabled())
            {
                // Debug.WriteLine("(SliderSwipePanel::OnApplyTemplate) - Registering touch events " + ElementUtilities.GetTouchStartEventName());
                Document.Body.AddEventListener(ElementUtilities.GetTouchStartEventName(), this.HandleElementMouseDown, true);

                Document.Body.AddEventListener(ElementUtilities.GetTouchMoveEventName(), this.draggingElementMouseMoveHandler, true);
                this.Element.AddEventListener(ElementUtilities.GetTouchEndEventName(), this.draggingElementMouseUpHandler, true);
                
                if (ElementUtilities.GetTouchCancelEventName() != null)
                {
                    this.Element.AddEventListener(ElementUtilities.GetTouchCancelEventName(), this.draggingElementMouseUpHandler, true);
                }

                // bug: Chrome on desktop kind-of implements pointer events, but doesn't seem to pass mouse events via pointers.
                if (Context.Current.DevicePlatform == DevicePlatform.Chrome && !Context.Current.IsTouchOnly)
                {
                    this.Element.AddEventListener("mousedown", this.HandleElementMouseDown, true);
                    this.Element.AddEventListener("mousemove", this.HandleElementMouseMove, true);
                    this.Element.AddEventListener("mouseup", this.HandlePointerUp, true);
                    this.Element.AddEventListener("dragstart", this.HandleDragStartEvent, true);
                }
            }
            else
            {
                this.Element.AddEventListener("mousedown", this.HandleElementMouseDown, true);
                this.Element.AddEventListener("mousemove", this.HandleElementMouseMove, true);
                this.Element.AddEventListener("mouseup", this.HandlePointerUp, true);
                this.Element.AddEventListener("dragstart", this.HandleDragStartEvent, true);
            }
            
            this.UpdateLinkBin();
            this.UpdateAccentColor();
            this.ApplyVisibility();

            this.FlashSwipeNavigation();

            this.UpdateSizingsOverTime();
            this.ConsiderShowingSwipeGuidelines();
            this.UpdateSwiping();
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

            this.UpdateSizingsOverTime();
        }

        protected override void OnItemControlAdded(Control c)
        {
            base.OnItemControlAdded(c);

            this.ApplyVisibility();
            this.UpdateSizingsOverTime();
        }

        private void HandleElementMouseDown(ElementEvent e)
        {
            if (ElementUtilities.IsDefaultInputElement(e, false) || !ElementUtilities.GetIsPrimary(e) || !this.allowSwiping)
            {
                return;
            }

            if (this.allowSwiping && !this.isDragging && !this.isConsideringDrag)
            {
                // Debug.WriteLine("(SliderSwipePanel::HandleElementMouseDown) - Starting drag");

           //     e.PreventDefault();

                this.isConsideringDrag = true;

                this.downEventPageX = ElementUtilities.GetPageX(e);
                this.downEventPageY = ElementUtilities.GetPageY(e);

                this.initialScrollX = this.itemsBin.ScrollLeft;
            }
            else
            {
                // Debug.WriteLine("(SliderSwipePanel::HandleElementMouseDown) - Not starting drag");
            }
        }

        private void HandleDragMoveDeadTimeout()
        {
            int now = Date.Now.GetTime();

            if (now - this.lastDragEventTime > 100 && this.isDragging)
            {
                this.HandlePointerUp(null);
            }
        }

        private void HandleElementMouseMove(ElementEvent e)
        {
            if (ElementUtilities.IsDefaultInputElement(e, false) || !ElementUtilities.GetIsPrimary(e) || !this.allowSwiping)
            { 
                return;
            }

            this.lastMoveEvent = e;

            if (this.isDragging)
            {
                e.PreventDefault();

                ElementUtilities.UpdateLastScrollTime();

                this.lastDragEventTime = Date.Now.GetTime();

                Window.SetTimeout(this.HandleDragMoveDeadTimeout, 100);

                int newLeft = (int)Math.Floor(this.initialScrollX + (this.downEventPageX - ElementUtilities.GetPageX(e)));
                // Debug.WriteLine("(SliderSwipePanel::HandleElementMouseMove) - Mouse Move drag: " + newLeft);

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

                // Debug.WriteLine("(SliderSwipePanel::HandleElementMouseMove) - Not dragging");
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

                // Debug.WriteLine("(SliderSwipePanel::ConsiderStartDragging)");
            }
            else
            {
                this.isConsideringDrag = false;

                // Debug.WriteLine("(SliderSwipePanel::ConsiderStartDragging) - Failed CSD check" + diffX + diffY);
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
                this.HandlePointerUp(e);
            }

            e.CancelBubble = true;
        }

        private void HandlePointerUp(ElementEvent e)
        {
            if (!ElementUtilities.GetIsPrimary(e) || !this.allowSwiping)
            {
                return;
            }

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

                int nextPage = this.GetNextPage(this.ActiveIndex);
                int previousPage = this.GetPreviousPage(this.ActiveIndex);

                if (diffX > 8 && this.ActiveIndex < this.ItemControls.Count - 1 && nextPage >= 0) //this.itemsBin.ScrollLeft > (initialScrollX + (panelWidth / 4)) && this.ActiveIndex < this.ItemControls.Count - 1)
                {                    
                    this.ActiveIndex = nextPage;
                }
                else if (diffX < -8 && previousPage >= 0) //this.itemsBin.ScrollLeft < (initialScrollX - (panelWidth / 4)) && this.ActiveIndex > 0)
                {
                    this.ActiveIndex = previousPage;
                }
                else
                {
                    this.AnimateToIndexPosition();
                }
            }
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

            this.UpdateSizingsOverTime();

            if (!this.isAnimating)
            {
                this.SetFinalPosition();
            }
        }

        public void UpdateSizingsEvent(ElementEvent e)
        {
            this.UpdateSizingsOverTime();

            if (!this.isAnimating)
            {
                this.SetFinalPosition();
            }
        }

        private void UpdateSizingsOverTime()
        {
            this.UpdateSizings();

            Window.SetTimeout(new Action(this.UpdateSizings), 1);
            Window.SetTimeout(new Action(this.UpdateSizings), 400);
            Window.SetTimeout(new Action(this.UpdateSizings), 800);
        }

        public void UpdateSizings()
        {
            if (this.Element == null)
            {
                return;
            }

            ClientRect cr = ElementUtilities.GetBoundingRect(this.Element);

            double elementVisibleRight = cr.Right;
            double elementVisibleLeft = cr.Left;
            double elementVisibleBottom = cr.Bottom;

            if (elementVisibleLeft < 0)
            {
                elementVisibleLeft = 0;
            }

            double elementWidth = elementVisibleRight - elementVisibleLeft;

            if (elementVisibleRight > Window.InnerWidth)
            {
                elementVisibleRight = Window.InnerWidth;
            }

            if (elementVisibleBottom > Window.InnerHeight)
            {
                elementVisibleBottom = Window.InnerHeight;
            }

            double height = 0;

            if (this.Height != null)
            {
                height = (double)this.Height;
            }
            else
            {
                height = (cr.Bottom - cr.Top);
            }

            if (this.topAreaOuter != null)
            {
                ClientRect topAreaRect = ElementUtilities.GetBoundingRect(this.topAreaOuter);

                height = height - (topAreaRect.Bottom - topAreaRect.Top);
            }

            this.panelWidth = Window.InnerWidth;

            if (this.swipeGuideRight != null && elementVisibleRight > 0)
            {
                this.swipeGuideRight.Style.MaxWidth = "70px";

                this.swipeGuideRight.Style.Width = "70px";
                this.swipeGuideRight.Style.Left = (elementVisibleRight - 70).ToString() + "px";
                this.swipeGuideRight.Style.Top = (((elementVisibleBottom - cr.Top) / 2) + cr.Top - 70).ToString() + "px";
            }

            if (this.swipeNavigation != null && elementVisibleRight > 0)
            {
                if (this.swipeNavigationPositioning == SliderSwipeNavigationPositioning.BottomCenter)
                {
                    this.swipeNavigation.Style.Left = ( ((elementWidth - 200) + swipeNavigationOffsetX) / 2).ToString() + "px";
                    this.swipeNavigation.Style.Top = (elementVisibleBottom - swipeNavigationOffsetY).ToString() + "px";
                }
                else
                {
                    this.swipeNavigation.Style.Left = (swipeNavigationOffsetX).ToString() + "px";
                    this.swipeNavigation.Style.Top = (elementVisibleBottom - swipeNavigationOffsetY).ToString() + "px";
                }
            }

            int index = 0;

            foreach (Control c in this.ItemControls)
            {
                PaneSettings ps = this.Settings[index];

                if (c.Element != null)
                {
                    Style style = c.Element.Style;

                    double width = (elementWidth - this.gapBetweenSections);// +3;

                    if (this.useFullWindow)
                    {
                        style.MaxWidth = Context.Current.BrowserInnerWidth + "px";
                        style.MinWidth = Context.Current.BrowserInnerWidth + "px";
                        style.Width = Context.Current.BrowserInnerWidth + "px";
                    }
                    else if (ps.FitToWidth && width > 100)
                    {
                        style.MinWidth = width.ToString() + "px";
                        style.Width = width.ToString() + "px";
                    }
                    else
                    {
                        style.MinWidth = null;
                        style.Width = null;
                    }
                      
                    style.MarginRight = this.gapBetweenSections + "px";

                    if (this.useFullWindow)
                    {
                        style.MaxHeight = Context.Current.BrowserInnerHeight + "px";
                        style.MinHeight = Context.Current.BrowserInnerHeight + "px";
                        style.Height = Context.Current.BrowserInnerHeight + "px";
                    }
                    else if (this.InteriorItemHeight != null)
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
                        style.Height = (height).ToString() + "px";
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
