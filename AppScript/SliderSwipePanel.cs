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
        BottomLeft = 1,
        BottomCenter = 2,
        InlineSmallFormFactorOnly=3
    }

    public class SliderSwipePanel : ItemsControl
    {
        [ScriptName("e_itemsBin")]
        private Element itemsBin;

        [ScriptName("e_smallSwipeArea")]
        private Element smallSwipeArea;

        [ScriptName("e_itemsTable")]
        private Element itemsTable;

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
        private ElementEventListener sizeChanged;
        
        private double toX;
        private bool isAnimating = false;
        private bool displaySwipeNavigationTitle = true;

        private bool allowSwiping = true;
        private String interiorItemHeight;

        private ElementEventListener draggingElementMouseMoveHandler = null;
        private ElementEventListener draggingElementMouseUpHandler = null;
        private ElementEventListener draggingElementMouseOutHandler = null;
        
        public event EventHandler IndexChangeAnimationCompleted;

        private bool isWaitingForDragEnd = false;
        private bool isImmediateAnimating = false;
        private bool isWindowSizeEventRegistered = false;
        private bool reparentedSwipeGuideRight = false;

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
                    this.UpdateInteriorHorizontalPosition();
                    return;
                }

                this.previousIndex = this.activeIndex;
                this.activeIndex = value;

                this.FlashSwipeNavigation();

                this.UpdateSwipeGuidelines();
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
            this.sizeChanged = this.UpdateSizingsEvent;

            this.paneSettingsCollection = new PaneSettingsCollection();

            this.visibilities = new List<bool>();

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
            if ((this.allowSwiping &&
                    Context.Current.IsTouchOnly &&
                    ElementUtilities.IsVisible(this.Element) && 
                    this.ActiveIndex < this.ItemControls.Count - 1 && 
                     this.GetNextPage(this.ActiveIndex) >= 0 &&
                    swipeGuideCount < 4 )
                  && this.swipeGuideRight != null 
                  && this.Visible
                  && this.swipeGuideRight.Style.Display != "block")
            {
                if (!this.reparentedSwipeGuideRight)
                {
                    this.reparentedSwipeGuideRight = true;
                    this.swipeGuideRight.ParentNode.RemoveChild(this.swipeGuideRight);

                    Document.Body.AppendChild(this.swipeGuideRight);
                }

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

        private void UpdateSwipeGuidelines()
        {
            if (this.ActiveIndex >= this.ItemControls.Count - 1 || this.GetNextPage(this.ActiveIndex) < 0)
            {
                this.HideSwipeGuide(null);
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
                    this.swipeNavigationDotBin != null && 
                    this.swipeNavigationTitle != null)
            {

                this.UpdateSwipeNavigationLinkState();

                if (this.swipeNavigationOpacityAnimator == null)
                {
                    this.swipeNavigationOpacityAnimator = new OpacityAnimator();
                }

                this.lastFlashIndex = this.ActiveIndex;

                if (this.swipeNavigation != null)
                {
                    this.swipeNavigation.Style.Display = "block";

                    if (!this.alwaysDisplaySwipeNavigation)
                    {
                        this.swipeNavigationOpacityAnimator.Element = this.swipeNavigation;
                        this.swipeNavigationOpacityAnimator.From = 1;
                        this.swipeNavigationOpacityAnimator.To = 0;
                        this.swipeNavigationOpacityAnimator.StartAfter(3000, 500, this.HideSwipeNavigation, null);
                    }
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
            if (this.swipeNavigationTitle != null)
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
            }

            if (this.swipeNavigationDotBin != null)
            {
                ElementUtilities.ClearChildElements(this.swipeNavigationDotBin);

                for (int i = 0; i < this.ItemControls.Count; i++)
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
        }


        public void SetActiveIndexImmediate(int newActiveIndex)
        {
            // Log.DebugMessage("Active index " + newActiveIndex + " from " + this.activeIndex);
            this.FlashSwipeNavigation();

            if (this.activeIndex == newActiveIndex)
            {
                this.ApplyVisibility();
                this.UpdateSizings();
                this.SetAnimationImmediate();
                this.UpdateInteriorHorizontalPosition();
                return;
            }

            this.previousIndex = this.activeIndex;
            this.activeIndex = newActiveIndex;

            this.ApplyVisibility();
           
            this.UpdateLinkHighlights();
            this.UpdateSwipeGuidelines();

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

                    this.toX = left;
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

        private void SetAnimationImmediate()
        {
            if (!this.isImmediateAnimating && this.itemsTable != null)
            {
                ElementUtilities.SetTransition(this.itemsTable, "none");
                this.isImmediateAnimating = true;
            }
        }

        private void SetAnimationLong()
        {
            if (this.isImmediateAnimating && this.itemsTable != null)
            {
                if (Context.Current.DevicePlatform == DevicePlatform.iOS || Context.Current.DevicePlatform == DevicePlatform.MacSafari)
                {
                    ElementUtilities.SetTransition(this.itemsTable, "left 0.4s ease-in-out");
                }
                else
                {
                    ElementUtilities.SetTransition(this.itemsTable, "transform 0.4s ease-in-out");
                }

                this.isImmediateAnimating = false;
            }
        }

        private void AnimateToIndexPosition()
        {
            if (this.isAnimating)
            {
                return;
            }

            this.isAnimating = true;

            this.SetAnimationLong();

            ElementUtilities.AnimateOnNextFrame(new Action(this.UpdateInteriorHorizontalPosition));

            Window.SetTimeout(this.AnimationComplete, 420);
        }

        private void AnimationComplete()
        {
            this.isAnimating = false;
            this.SetAnimationImmediate();
        }
       
        
        private void UpdateInteriorHorizontalPosition()
        {
            this.SetToX();

            // Log.DebugMessage("SetFinalPosition: Setting swipe panel position to " + this.toX + "|" + this.activeIndex);

            this.SetPanelLeft(this.toX);
        }

        private void SetPanelLeft(double left)
        {
            if (this.itemsBin == null)
            {
                return;
            }

            // iOS doesn't really animate translations very well.
            if (Context.Current.DevicePlatform == DevicePlatform.iOS || Context.Current.DevicePlatform == DevicePlatform.MacSafari)
            {
                this.itemsTable.Style.Left = "-" + left + "px";
            }
            else
            {
                ElementUtilities.SetTransform(this.itemsTable, "translateX(-" + left + "px)");
            }

            this.itemsBin.ScrollLeft = 0;
        }

        private void UpdateLinkBin()
        {
            if (this.swipeNavigationPositioning == SliderSwipeNavigationPositioning.InlineSmallFormFactorOnly)
            {
                if (this.swipeNavigationDotBin != null)
                {
                    if (Context.Current.IsSmallFormFactor)
                    {
                        this.swipeNavigationDotBin.Style.Display = "";
                    }
                    else
                    {
                        this.swipeNavigationDotBin.Style.Display = "none";
                    }
                }

                if (this.swipeNavigationTitle != null)
                {
                    if (Context.Current.IsSmallFormFactor)
                    {
                        this.swipeNavigationTitle.Style.Display = "";
                    }
                    else
                    {
                        this.swipeNavigationTitle.Style.Display = "none";
                    }
                }
            }

            if (this.containerLinkBin == null)
            {
                return;
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

            if (Context.Current.IsSmallFormFactor)
            {
                return;
            }

            while (this.containerLinkBin.ChildNodes.Length > 0)
            {
                this.containerLinkBin.RemoveChild(this.containerLinkBin.ChildNodes[0]);
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

            if (!Context.Current.IsSmallFormFactor && this.smallSwipeArea != null)
            {
                this.smallSwipeArea.Style.Display = "none";
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

            this.ConsiderSizeChangedRegistration();

            this.UpdateSizingsOverTime();
            this.ConsiderShowingSwipeGuidelines();
            this.UpdateSwiping();
        }

        protected override void OnVisibilityChanged()
        {
            base.OnVisibilityChanged();

            this.ConsiderSizeChangedRegistration();

            this.UpdateSizingsOverTime();

            if (!this.Visible)
            {
                this.HideSwipeGuide(null);
            }
            else
            {
                this.ConsiderShowingSwipeGuidelines();
            }
        }

        private void ConsiderSizeChangedRegistration()
        { 
            if (this.Element != null)
            {
                if (this.Visible && !this.isWindowSizeEventRegistered)
                {
                    this.isWindowSizeEventRegistered = true;

                    Window.AddEventListener("resize", this.sizeChanged);
                }
                else if (this.isWindowSizeEventRegistered)
                {
                    this.isWindowSizeEventRegistered = false;

                    Window.RemoveEventListener("resize", this.sizeChanged);
                }
            }
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

                this.initialScrollX = this.toX;

                Window.SetTimeout(this.ExpireConsiderStartDragging, 250);
            }
            else
            {
                // Debug.WriteLine("(SliderSwipePanel::HandleElementMouseDown) - Not starting drag");
            }
        }

        private void HandleDragMoveDeadTimeout()
        {
            if (!this.isDragging)
            {
                return;
            }

            int now = Date.Now.GetTime();

            if (now - this.lastDragEventTime > 100 && this.isDragging)
            {
                this.isWaitingForDragEnd = false;
                this.HandlePointerUp(null);
            }
            else
            {
                Window.SetTimeout(this.HandleDragMoveDeadTimeout, 50);
            }
        }

        private void HandleElementMouseMove(ElementEvent e)
        {
            if (!ElementUtilities.GetIsPrimary(e) || !this.allowSwiping)
            {
                return;
            }

            if (!this.isDragging)
            {
                if (ElementUtilities.IsDefaultInputElement(e, false))
                {
                    return;
                }
            }

            this.lastMoveEvent = e;

            if (this.isDragging)
            {
                e.PreventDefault();

                ElementUtilities.UpdateLastScrollTime();

                this.lastDragEventTime = Date.Now.GetTime();

                if (!this.isWaitingForDragEnd)
                {
                    Window.SetTimeout(this.HandleDragMoveDeadTimeout, 110);
                }

                double newLeft = this.initialScrollX + (this.downEventPageX - ElementUtilities.GetPageX(e));

                this.SetAnimationImmediate();

                if (newLeft != this.toX)
                {
                    this.SetPanelLeft(newLeft);
                    this.toX = newLeft;
                }
            }
            else
            {                
                if (this.isConsideringDrag)
                {
                    double diffX = Math.Abs((this.downEventPageX - ElementUtilities.GetPageX(this.lastMoveEvent)));
                    double diffY = Math.Abs((this.downEventPageY - ElementUtilities.GetPageY(this.lastMoveEvent)));

                    if (diffX > 4 || diffY > 4)
                    {
                        this.ConsiderStartDragging();
                    }
                }

                // Debug.WriteLine("(SliderSwipePanel::HandleElementMouseMove) - Not dragging");
            }
        }

        private void ExpireConsiderStartDragging()
        {
            this.isConsideringDrag = false;
        }

        private void ConsiderStartDragging()
        {
            if (!this.isConsideringDrag || this.isDragging)
            {
                return;
            }

            double diffX = Math.Abs((this.downEventPageX - ElementUtilities.GetPageX(this.lastMoveEvent)));
            double diffY = Math.Abs((this.downEventPageY - ElementUtilities.GetPageY(this.lastMoveEvent)));

            if (diffX > diffY && diffX > 4 && !this.isAnimating)
            {
                this.isDragging = true;

                // Debug.WriteLine("(SliderSwipePanel::ConsiderStartDragging)");
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
            if ((e != null && !ElementUtilities.GetIsPrimary(e)) || !this.allowSwiping)
            {
                return;
            }

            this.isConsideringDrag = false;
            Debug.WriteLine("(SliderSwipePanel::PointerUp)");

            if (this.isDragging)
            {
                if (e != null)
                {
                    e.PreventDefault();
                }


                this.isWaitingForDragEnd = false;
                this.isDragging = false;

                double diffX = this.downEventPageX - ElementUtilities.GetPageX(this.lastMoveEvent);

                int nextPage = this.GetNextPage(this.ActiveIndex);
                int previousPage = this.GetPreviousPage(this.ActiveIndex);

                if (diffX > 8 && this.ActiveIndex < this.ItemControls.Count - 1 && nextPage >= 0) 
                {                    
                    this.ActiveIndex = nextPage;
                }
                else if (diffX < -8 && previousPage >= 0) 
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
        }

        public void UpdateSizingsEvent(ElementEvent e)
        {
            this.UpdateSizingsOverTime();
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

            // Log.DebugMessage("Sizing Event: " + elementVisibleLeft + "|" + elementVisibleBottom + "|" + this.Height);

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



            if (Context.Current.DevicePlatform == DevicePlatform.iOS && !Context.Current.IsFullScreenWebApp && !Context.Current.IsHostedInApp && this.swipeNavigation != null)
            {
                // this is set in CSS, but due to iOS Safari's Bottom-Bar hide/show behavior we need to explicitly set it via BrowserInnerHeight,
                // which takes into account the real interior size.
                this.swipeNavigation.Style.Top = (Context.Current.BrowserInnerHeight - 69) + "px";
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
                        this.itemsBin.Style.Width = "100vw";
                        this.itemsBin.Style.MaxWidth = "100vw";
                        this.itemsBin.Style.MinWidth = "100vw";

                        String newHeight = Context.Current.BrowserInnerHeight + "px";

                        this.itemsBin.Style.Height = newHeight;
                        this.itemsBin.Style.MaxHeight = newHeight;
                        this.itemsBin.Style.MinHeight = newHeight;
                    }

                    if (this.useFullWindow || (ps.FitToWidth && width > 100))
                    {
                        style.MaxWidth = "100vw";
                        style.MinWidth = "100vw";
                        style.Width = "100vw";
                    }
                    else
                    {
                        style.MinWidth = null;
                        style.Width = null;
                    }
                      
                    style.MarginRight = this.gapBetweenSections + "px";

                    if (this.useFullWindow)
                    {
                        String newHeight = Context.Current.BrowserInnerHeight + "px";

                        style.Height = newHeight;
                        style.MaxHeight = newHeight;
                        style.MinHeight = newHeight;
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

            this.UpdateInteriorHorizontalPosition();
        }
    }
}
