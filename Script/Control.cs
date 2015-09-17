/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Html;
using System.Diagnostics;
using jQueryApi;
using System.Runtime.CompilerServices;

namespace BL.UI
{

    /// <summary>
    /// The base class of user interface-related controls.
    /// 
    /// Terms:
    ///     A child control is a control that a control uses to implement itself, and that are owned by a parent control.
    ///     An items control is a type of control that can contain arbitrary child controls.
    /// 
    /// Control Lifecycle:
    ///     Construction - Initial construction of the control.
    ///     Init - Any sort of initialization that control must do.
    ///     ApplyTemplate - templates are applied if specified.
    ///     CreateChildControls - Creates any implementation 
    ///     EnsureElements - Creates DHTML elements and binds them to child controls as necessary.
    ///     Update -- Updates the state of DHTML elements based on changes to a control.
    /// </summary>
    public abstract class Control : SerializableObject, IDisposable, IControl
    {
        private Element element;
        private Element contentElement;
        private Control parentControl;
        private List<Control> templateControls;
        private List<Control> templateDescendentControls;
        private List<Element> templateElements;
        private List<String> setElements;
        private Dictionary<String, Element> templateStandaloneElements;
        private List<ElementAndEvent> clickEventDelegates;
        private String templateId;
        private String template;
        private String tagName;
        private bool hasShownSpinner;
        private bool initted = false;
        private bool childControlsCreated = false;
        private bool elementsEnsured = false;
        private String innerHtml;
        private String typeId = null;
        private String className;
        private jQueryObject jqueryObject;
        private String id;
        private object tag;
        private bool visible = true;
        protected bool trackResizeEvents = false;
        private bool hookedResize = false;
        private String oldDisplayMode;
        private int? height;
        private int? width;
        private bool delayApplyTemplate = false;
        private bool ensureElementsRequested = false;
        private bool templateWasApplied = false;
        private bool trackInteractionEvents = false;
        private bool interactionEventsRegistered = false;
        private bool fireUpdateOnTemplateComplete = true;
        private ElementEffects effects;
        private long touchStartTime = 0;
        private long lastClickTime = 0;
        private TemplateParser controlLoader = null;

        private Dictionary<String, String> prerequisiteScripts = null;
        private List<String> prerequisiteStylesheets = null;
        private int prerequisiteScriptsRequested = 0;
        private bool hasRequestedPrerequisiteScripts = false;
        private bool hasRequestedPrerequisiteStyleSheets = false;
        private bool templateApplyRequested = false;
        private bool enqueueUpdates = false;
        private bool updatePending = false;

        private Action applyVisibleOnFrameAction;

        private bool isDelayHidden = false;
        private bool isRetrievingTemplate = false;

        private ElementEventListener resizeHandler;
        private ElementEventListener mouseOverHandler;
        private ElementEventListener mouseOutHandler;
        private ElementEventListener mouseEnterHandler;
        private ElementEventListener mouseLeaveHandler;
        private ElementEventListener mouseMoveHandler;
        private ElementEventListener touchStartHandler;
        private ElementEventListener touchEndHandler;
        private ElementEventListener clickHandler;
        private ElementEventListener windowScrollHandler;

        public event EventHandler TemplateApplied;

        public Control ParentControl
        {
            get
            {
                return this.parentControl;
            }

            set
            {
                this.parentControl = value;
            }
        }

        protected bool EnqueueUpdates
        {
            get
            {
                return this.enqueueUpdates;
            }

            set
            {
                this.enqueueUpdates = value;
            }
        }

        protected bool DelayApplyTemplate
        {
            get
            {
                return this.delayApplyTemplate;
            }

            set
            {
                if (this.delayApplyTemplate == value)
                {
                    return;
                }

                this.delayApplyTemplate = value;

                if (!this.delayApplyTemplate && this.element != null && !this.templateWasApplied)
                {
                    this.EnsureElements();
                }
            }
        }

        [ScriptName("s_templateId")]
        public String TemplateId
        {
            get
            {
                if (this.templateId == null)
                {
                    this.templateId = this.GetDefaultTemplateId();
                }

                return this.templateId;
            }

            set
            { 
                if (this.templateId == value || (value == null && (this.templateId == this.GetDefaultTemplateId() || this.templateId == null)))
                {
                    return;
                }

                this.templateId = value;

                bool templateWasAppliedBefore = this.templateWasApplied;

                this.ClearTemplate();

                if (templateWasAppliedBefore)
                {
                    this.isRetrievingTemplate = false;
                    this.ApplyTemplate();
                }

                this.ApplyClass();
            }
        }

        public ElementEffects Effects
        {
            get
            {
                if (this.effects == null)
                {
                    this.effects = new ElementEffects();
                    this.effects.Element = this.element;
                    this.effects.Control = this;
                }

                return this.effects;
            }
        }

        public String Template
        {
            get
            {
                return this.template;
            }

            set
            {
                this.template = value;
                this.templateWasApplied = false;
            }
        }

        [ScriptName("i_width")]
        public int? Width
        {
            get
            {
                return this.width;
            }

            set
            {
                if (this.width == value)
                {
                    return;
                }

                this.width = value;

                Element thisElement = this.Element;

                if (thisElement != null && this.width != null)
                {
                    thisElement.Style.Width = ((int)this.width).ToString() + "px";
                }

                this.OnDimensionChanged();
            }
        }

        public object Tag
        {
            get
            {
                return this.tag;
            }

            set
            {
                this.tag = value;
            }
        }

        public virtual String TagName
        {
            get
            {
                return tagName;
            }

            set
            {
                this.tagName = value;
            }
        }

        [ScriptName("i_height")]
        public virtual int? Height
        {
            get
            {
                return this.height;
            }

            set
            {
                if (this.height == value)
                {
                    this.OnDimensionChanged();

                    return;
                }

                this.height = value;

                Element thisElement = this.Element;

                if (thisElement != null && this.height != null)
                {
                    thisElement.Style.Height = ((int)this.height).ToString() + "px";
                }

                this.OnDimensionChanged();
            }
        }

        public jQueryObject J
        {
            get
            {
                if (this.jqueryObject == null)
                {
                    this.jqueryObject = jQuery.FromObject(this.element);
                }

                return this.jqueryObject;
            }
        }

        public bool TemplateWasApplied
        {
            get
            {
                return this.templateWasApplied;
            }
        }

        public int EffectiveHeight
        {
            get
            {
                if (this.height != null)
                {
                    return (int)this.height;
                }

                Element thisElement = this.Element;

                if (thisElement != null)
                {
                    return thisElement.ScrollHeight;
                }

                return 0;
            }
        }

        protected bool TrackInteractionEvents
        {
            get
            {
                return this.trackInteractionEvents;
            }

            set
            {
                this.trackInteractionEvents = value;

                this.HandleInteractionEventing();
            }
        }

        protected bool TrackResizeEvents
        {
            get
            {
                return this.trackResizeEvents;
            }

            set
            {
                this.trackResizeEvents = value;

                this.HandleResizeEventing();
            }
        }

        [ScriptName("b_visible")]
        public bool Visible
        {
            get
            {
                return this.visible;
            }

            set
            {
                if (this.visible == value)
                {
                    return;
                }

                this.visible = value;

                if (this.visible)
                {
                    if (!this.ElementsEnsured && this.ensureElementsRequested)
                    {
                        this.EnsureElementsForce(false);
                    } 
                }

                this.ApplyVisibility();
            }
        }

        public String IdWithinParentControl
        {
            get
            {
                String leafId = this.Id;

                int lastDash = leafId.LastIndexOf("-");

                if (lastDash > 0)
                {
                    leafId = leafId.Substring(lastDash + 1, leafId.Length);
                }

                return leafId;
            }
        }

        [ScriptName("s_id")]
        public String Id
        {
            get
            {
                if (this.id == null)
                {
                    this.EnsureId();
                }

                return this.id;
            }

            set
            {
                this.id = value;
            }
        }

        public virtual String DefaultClass
        {
            get
            {
                return null;
            }
        }

        public String ClassName
        {
            get
            {
                return this.className;
            }

            set
            {
                this.className = value;

                this.ApplyClass();
            }
        }

        public virtual String TypeId
        {
            get
            {
                if (this.typeId == null)
                {
                    this.typeId = this.GetType().FullName.Replace(".", "-").ToLowerCase();
                }

                return this.typeId;
            }
        }

        public String InnerHtml
        {
            get
            {
                return this.innerHtml;
            }

            set
            {
                this.innerHtml = value;

                if (this.ElementsEnsured)
                {
                    HtmlUtilities.SetInnerHtml(this.ContentElement, value);
                }
            }
        }

        public List<Control> TemplateControls
        {
            get
            {
                if (this.templateControls == null)
                {
                    this.templateControls = new List<Control>();
                }

                return this.templateControls;
            }
        }

        public List<Control> TemplateDescendentControls
        {
            get
            {
                if (this.templateDescendentControls == null)
                {
                    this.templateDescendentControls = new List<Control>();
                }

                return this.templateDescendentControls;
            }
        }

        public List<Element> TemplateElements
        {
            get
            {
                if (this.templateElements == null)
                {
                    this.templateElements = new List<Element>();
                }

                return this.templateElements;
            }
        }

        public bool Initted
        {
            get
            {
                return this.initted;
            }
        }

        public bool ElementsEnsured
        {
            get
            {
                return this.elementsEnsured;
            }
        }

        public bool ChildControlsCreated
        {
            get
            {
                return this.childControlsCreated;
            }
        }

        /// <summary>
        /// Root element of this control.  Setting this to a parent element will kick off the full control lifecycle.
        /// </summary>
        public Element Element
        {
            get
            {
                return this.element;
            }

            set
            {
                if (this.element == value)
                {
                    return;
                }

                this.interactionEventsRegistered = false;

                this.element = value;

                if (this.element == null)
                {
                    this.elementsEnsured = false;
                }

                if (this.effects != null && this.element != null)
                {
                    this.effects.Element = this.element;
                }

                this.jqueryObject = null;
                this.HandleInteractionEventing();

                this.EnsureElements();
            }
        }

        /// <summary>
        /// Default element where child control elements will be added to if they are not otherwise located.
        /// </summary>
        public Element ContentElement
        {
            get
            {
                if (this.contentElement == null)
                {
                    return this.element;
                }

                return this.contentElement;
            }

            set
            {
                if (this.contentElement == value)
                {
                    return;
                }

                this.contentElement = value;    
            }
        }


        public Control() : base()
        {
            this.applyVisibleOnFrameAction = new Action(this.ApplyVisibleOnAnimationFrame);
        }


        internal virtual void ClearTemplate()
        {
            if (this.templateWasApplied)
            {
                foreach (Control c in this.TemplateControls)
                {
                    c.ClearTemplate();
                    c.Element = null;
                    c.Dispose();
                }

                this.OnTemplateDisposed();

                if (this.setElements != null)
                {
                    foreach (String memberId in this.setElements)
                    {
                        Script.Literal("{0}[{1}]=null", this, memberId);
                    }
                }

                this.template = null;
                this.templateWasApplied = false;
                this.fireUpdateOnTemplateComplete = true;
            }
        }

        protected virtual void OnTemplateDisposed()
        {

        }

        public void EnsurePrepared()
        {
            if (!this.ElementsEnsured && this.ensureElementsRequested)
            {
                this.EnsureElementsForce(true);
           } 
        }

        protected virtual void OnDimensionChanged()
        {

        }
        private void HandleTouchStart(ElementEvent e)
        {
            this.touchStartTime = Date.Now.GetTime();
        }

        private void HandleTouchEnd(ElementEvent e)
        {
            Date now = Date.Now;

            if (now.GetTime() - this.touchStartTime < 200 && 
                now.GetTime() - this.lastClickTime > 200 && 
                !ElementUtilities.HasScrolledRecently())
            {
                this.lastClickTime = now.GetTime();
                this.OnClick(e);
            }
        }

        private void HandleClick(ElementEvent e)
        {
            Date now = Date.Now;

            if (now.GetTime() - this.lastClickTime > 200 && 
                !ElementUtilities.HasScrolledRecently())
            {
                this.lastClickTime = now.GetTime();
                this.OnClick(e);
            }
        }

        protected virtual void OnClick(ElementEvent e)
        {

        }

        private void HandleMouseEnter(ElementEvent e)
        {
            this.OnMouseEnter(e);
        }

        protected virtual void OnMouseEnter(ElementEvent e)
        {

        }

        private void HandleMouseLeave(ElementEvent e)
        {
            this.OnMouseLeave(e);
        }

        protected virtual void OnMouseLeave(ElementEvent e)
        {

        }

        private void HandleMouseOver(ElementEvent e)
        {
            this.OnMouseOver(e);
        }

        protected virtual void OnMouseOver(ElementEvent e)
        {

        }

        private void HandleMouseOut(ElementEvent e)
        {
            this.OnMouseOut(e);
        }

        protected virtual void OnMouseOut(ElementEvent e)
        {

        }

        private void HandleMouseMove(ElementEvent e)
        {
            this.OnMouseMove(e);
        }

        protected virtual void OnMouseMove(ElementEvent e)
        {

        }

        private String GetDefaultTemplateId()
        {
            return this.GetType().FullName.Replace(".", "-").ToLowerCase();
        }

        private void DisposeInteractionEventing()
        {
            if (this.interactionEventsRegistered)
            {
                this.element.RemoveEventListener("click", this.clickHandler, false);

                if (!ElementUtilities.GetIsPointerEnabled())
                {
                    this.element.RemoveEventListener("touchstart", this.touchStartHandler, false);
                    this.element.RemoveEventListener("touchend", this.touchEndHandler, false);
                }
                this.element.RemoveEventListener("mouseover", this.mouseOverHandler, true);
                this.element.RemoveEventListener("mouseout", this.mouseOutHandler, true);
                this.element.RemoveEventListener("mouseenter", this.mouseEnterHandler, true);
                this.element.RemoveEventListener("mouseleave", this.mouseLeaveHandler, true);
                this.element.RemoveEventListener("mousemove", this.mouseMoveHandler, false);
                Window.RemoveEventListener("scroll", this.windowScrollHandler, true);

                this.interactionEventsRegistered = false;
            }
        }

        private void HandleInteractionEventing()
        {
            if (!this.trackInteractionEvents)
            {
                this.DisposeInteractionEventing();

                return;
            }

            if (!interactionEventsRegistered && this.element != null)
            {
                if (this.mouseOutHandler == null)
                {
                    this.mouseOverHandler = this.HandleMouseOver;
                    this.mouseOutHandler = this.HandleMouseOut;
                    this.mouseEnterHandler = this.HandleMouseEnter;
                    this.mouseLeaveHandler = this.HandleMouseLeave;
                    this.mouseMoveHandler = this.HandleMouseMove;

                    this.touchStartHandler = this.HandleTouchStart;
                    this.touchEndHandler = this.HandleTouchEnd;
                    this.windowScrollHandler = this.HandleWindowScroll;

                    this.clickHandler = this.HandleClick;
                }

                this.element.AddEventListener("click", this.clickHandler, false);

                if (!ElementUtilities.GetIsPointerEnabled())
                {
                    this.element.AddEventListener("touchstart", this.touchStartHandler, false);
                    this.element.AddEventListener("touchend", this.touchEndHandler, false);
                }

                this.element.AddEventListener("mouseover", this.mouseOverHandler, false);
                this.element.AddEventListener("mouseout", this.mouseOutHandler, false);
                this.element.AddEventListener("mouseenter", this.mouseEnterHandler, false);
                this.element.AddEventListener("mouseleave", this.mouseLeaveHandler, false);
                this.element.AddEventListener("mousemove", this.mouseMoveHandler, false);
                Window.AddEventListener("scroll", this.windowScrollHandler, false);

                this.interactionEventsRegistered = true;
            }
        }

        private void ApplyClass()
        {
            if (this.Element != null)
            {
                if (this.className != null)
                {
                    this.Element.ClassName = this.TemplateId + " " + this.className;
                }
                else if (this.DefaultClass != null)
                {
                    this.Element.ClassName = this.TemplateId + " " + this.DefaultClass;
                }
                else
                {
                    this.Element.ClassName = this.TemplateId;
                }
            }
        }

        public String GetElementClass(String componentName)
        {
            String[] components = componentName.Split(' ');

            String summ = "";

            foreach (String component in components)
            {
                if (summ.Length > 0)
                {
                    summ += " ";
                }

                summ += this.TemplateId + "-" + component;
            }

            return summ;
        }

        public void EnsureId()
        {
            if (!String.IsNullOrEmpty(this.id))
            {
                return;
            }

            if (this.Element != null && !String.IsNullOrEmpty(this.Element.ID))
            {
                this.id = this.Element.ID;
            }

            if (this.id == null)
            {
                this.id = Utilities.CreateRandomId();

                if (this.Element != null)
                {
                    this.Element.ID = this.id;
                }
            }
        }

        public void AttachToElement(Element e)
        {
            this.element = e;

            if (this.effects != null)
            {
                this.effects.Element = e;
            }

            this.LoadFromElement();
            this.HandleInteractionEventing();

            if (this.Element != null)
            {
                this.EnsureElements();
            }
        }

        private void LoadFromElement()
        {
            ElementAttributeCollection eac = this.element.Attributes;
            
            for (int i = 0; i < eac.Length; i++)
            {
                ElementAttribute ea = null;

                Script.Literal("{0}={1}[{2}]", ea, eac, i);

                String attrName = ea.Name;

                if (attrName.StartsWith("data-bl-"))
                {
                    String propName = attrName.Substring(8, attrName.Length);

                    if (propName != "type")
                    {
                        this.SetProperty(propName, ea.Value);
                    }
                }
            }
        }

        public void AttachTo(object elementOrId, object options)
        {
            Script.Literal(@"if ({1} != null) {{ if ({1}.tagName != null) {{ {0} = {1}; }} if (typeof({1}) == ""string"") {{ {0} = document.getElementById({1}); }}}}", this.element, elementOrId);

            this.HandleInteractionEventing();

            if (this.Element != null)
            {
                this.EnsureElements();
            }
        }

        public void Init()
        {
            if (this.initted || this.element == null)
            {
                return;
            }

            this.OnInit();

            this.initted = true;
        }

        protected virtual void OnApplyTemplate()
        {

        }
        
        public void EnsureScript(String scriptItem, String scriptPath)
        {
            if (this.prerequisiteScripts == null)
            {
                this.prerequisiteScripts = new Dictionary<string, string>();
            }

            this.prerequisiteScripts[scriptItem] = scriptPath;
        }

        public void EnsureStylesheet(String stylesheetPath)
        {
            if (this.prerequisiteStylesheets == null)
            {
                this.prerequisiteStylesheets = new List<string>();
            }

            this.prerequisiteStylesheets.Add(stylesheetPath);
        }

        public void CreateChildControls()
        {
            if (!this.initted)
            {
                this.Init();
            }

            if (!this.childControlsCreated)
            {
                this.OnCreateChildControls();
            }

            this.childControlsCreated = true;
        }

        public bool LoadPrerequisites()
        {
            if (this.prerequisiteStylesheets != null && !this.hasRequestedPrerequisiteStyleSheets)
            {
                this.hasRequestedPrerequisiteStyleSheets = true;
                
                foreach (String sheet in this.prerequisiteStylesheets)
                {
                    ControlManager.Current.EnsureStylesheet(sheet);
                }
            }

            if (this.prerequisiteScripts != null && !this.hasRequestedPrerequisiteScripts)
            {
                this.hasRequestedPrerequisiteScripts = true;
                bool foundAllScripts = true;

                this.prerequisiteScriptsRequested = 0;
                Dictionary<String, String> scriptsToLoad = new Dictionary<String, String>();

                foreach (KeyValuePair<String, String> script in this.prerequisiteScripts)
                {
                    if (!ControlManager.Current.IsLoadedScriptItem(script.Key) && !scriptsToLoad.ContainsKey(script.Key))
                    {
                        this.prerequisiteScriptsRequested++;

                        foundAllScripts = false;

                        scriptsToLoad[script.Key] = script.Value;
                    }
                }

                foreach (KeyValuePair<String, String> script in scriptsToLoad)
                {
                    String adjust = script.Value.ToLowerCase();

                    adjust += "?v=" + Context.Current.VersionToken;

                    String path = adjust;

                    if (!UrlUtilities.IsPathAbsolute(path))
                    {
                        path = UrlUtilities.EnsurePathEndsWithSlash(Context.Current.ResourceBasePath) + adjust;
                    }

                    ControlManager.Current.LoadScript(script.Key, path, this.ScriptLoadedContinue, path);
                }

                if (!foundAllScripts)
                {
                    return false;
                }
            }

            return true;
        }

        private void ApplyTemplate()
        {
            if (this.templateWasApplied)
            {
                return;
            }

            if (this.hasRequestedPrerequisiteScripts && this.prerequisiteScriptsRequested > 0)
            {
                return;
            }

            this.templateApplyRequested = true;

            if (!this.LoadPrerequisites())
            {
                this.EnsureLoadingSpinner();

                return;
            }

            if (this.Template != null)
            {
                //this.templateWasApplied = true;
                this.ApplyTemplateContinue();
                return;
            }

            String templateId = this.TemplateId;

            if (templateId != null)
            {
                if (!this.isRetrievingTemplate)
                {
                    isRetrievingTemplate = true;

                    if (!TemplateManager.Current.GetTemplateAsync(templateId, this.HandleApplyTemplateContinue, null))
                    {
                        this.EnsureLoadingSpinner();
                    }
                }
                return;
            }

            // if no template is defined for this, then just call OnApplyTemplate and call it good.
            this.CompleteApplyTemplate();
        }

        private void EnsureLoadingSpinner()
        {
            if (!this.hasShownSpinner)
            {
                this.hasShownSpinner = true;
                ElementUtilities.AddPendingOperation("ctl" + this.Id);
            }
        }
        
        private void HandleApplyTemplateContinue(IAsyncResult result)
        {
            this.isRetrievingTemplate = false;
            Template t = (Template)result.Data;

            if (t != null)
            {
                this.templateId = t.Id;
                this.template = t.Content;

                this.controlLoader =  new TemplateParser();
                this.controlLoader.ScriptsLoaded += controlLoader_ScriptsLoaded;

                if (this.controlLoader.EnsureScriptsLoaded(this.template))
                {
                    this.ApplyTemplateContinue();
                }
                else
                {
                    this.EnsureLoadingSpinner();
                }
            }
            else
            {
                Log.DebugMessage(String.Format("Expected template '{0}' was not found.", templateId));
               
                this.CompleteApplyTemplate();
            }
        }

        private void ScriptLoadedContinue(IAsyncResult result)
        {
            // Log.DebugMessage("Control with template '" + this.TemplateId + "' has " + this.prerequisiteScriptsRequested + " scripts left. Path: " + result.AsyncState.ToString());

            this.prerequisiteScriptsRequested--;

            this.ConsiderScriptsAreLoaded();
        }

        /*
        private void MonitorScripts()
        {
            if (!this.ConsiderScriptsAreLoaded())
            {
                Window.SetTimeout(this.MonitorScripts, 1000);
                Log.DebugMessage("Monitoring " + this.prerequisiteScriptsRequested + " scripts for " + this.TemplateId);
                return;
            }

            Debug.Fail("Initialized scripts from monitoring");
        }*/

        private bool ConsiderScriptsAreLoaded()
        {
            foreach (KeyValuePair<String, String> script in this.prerequisiteScripts)
            {
                if (!ControlManager.Current.IsLoadedScriptItem(script.Key))
                {
                    return false;
                }
            }

            if (this.prerequisiteStylesheets != null)
            {
                foreach (String s in this.prerequisiteStylesheets)
                {
                    if (!ControlManager.Current.HasStylesheet(s))
                    {
                        Debug.Fail("Stylesheet not loaded");
                        
                        return false;
                    }
                }
            }

            if (!this.templateWasApplied && this.templateApplyRequested)
            {
                this.ApplyTemplate();
            }

            return true;
        }

        private void controlLoader_ScriptsLoaded(object sender, EventArgs e)
        {
            this.ApplyTemplateContinue();
        }

        private void ApplyTemplateContinue()
        {
            String template = this.Template;

            this.templateControls = new List<Control>();
            this.templateDescendentControls = new List<Control>();
            this.templateElements = new List<Element>();
            this.templateStandaloneElements = new Dictionary<String, Element>();

            if (template != null)
            {
                TemplateParser tp = new TemplateParser();

                TemplateParserResult tpr = tp.Parse(this.Id, this.TemplateId, template);

                this.hasShownSpinner = false;
                HtmlUtilities.SetInnerHtml(this.Element, tpr.Markup);

                if (tpr.ItemsContainer != null)
                {
                    Element e = this.GetElementFromPath(this.Element, tpr.ItemsContainer);

                    Script.Literal("var fn = {0}[\"set_itemsContainerElement\"];  if (fn != null) {{ fn.apply(this, new Array({1})); }}", this, e);
                }

                if (tpr.ContentContainer != null)
                {
                    Element e = this.GetElementFromPath(this.Element, tpr.ContentContainer);

                    Script.Literal("var fn = {0}[\"set_contentContainerElement\"];  if (fn != null) {{ fn.apply(this, new Array({1})); }}", this, e);
                }

                /*
                if (this.setElements != null)
                {
                    foreach (String memberId in this.setElements)
                    {
                        Script.Literal("{0}[{1}]=null", this, memberId);
                    }
                }*/

                this.setElements = new List<string>();

                for (int i = 0; i < tpr.Controls.Count; i++)
                {
                    List<int> path = tpr.ControlElementPaths[i];
                    Control c = tpr.Controls[i];
                    String controlId = tpr.ControlIds[i];

                    // note Path will be null in cases where you have a ContentControl or ItemControl child with an ID
                    if (path != null)
                    {
                        c.ParentControl = this;
                        this.templateControls.Add(c);

                        Element element = this.GetElementFromPath(this.Element, path);

                        this.templateElements.Add(element);

                        if (controlId != null)
                        {
                            Script.Literal("{0}['e_' + {1}]={2}", this, controlId, element);
                            this.setElements.Add("e_" + controlId);
                        }
                    }
                    else
                    {
                        this.templateDescendentControls.Add(c);
                    }

                    if (controlId != null)
                    {
                        Script.Literal("{0}['c_' + {1}]={2}", this, controlId, c);
                        this.setElements.Add("c_" + controlId);
                    }
                }

                for (int i = 0; i < tpr.ElementIds.Count; i++)
                {
                    List<int> path = tpr.ElementElementPaths[i];
                    String elementId = tpr.ElementIds[i];

                    Element element = this.GetElementFromPath(this.Element, path);
                 
                    if (elementId != null)
                    {
                        this.templateStandaloneElements[elementId] = element;
                        Script.Literal("{0}['e_' + {1}]={2}", this, elementId, element);

                        // hook click functions if they exist in the form of functions called v_on<ElementId>Click
                        String clickFunctionName = "v_on" + elementId.Substring(0, 1).ToUpperCase() + elementId.Substring(1, elementId.Length) + "Click";

                        Script.Literal("if ({0}[{1}] != null) {{ if ({3} == null) {{ {3} = {{}}; }} var del = ss.Delegate.create({0}, {0}[{1}]); {3}[{3}.length-1]={{\"element\":{2},\"event\":del}}; {2}.addEventListener('click', del, true); {2}.addEventListener('touchEnd', del, true); }}", this, clickFunctionName, element, this.clickEventDelegates);

                        this.setElements.Add("e_" + elementId);
                    }
                }
            }
            else
            {
                this.RemoveSpinner();
            }

            for (int i = 0; i < this.templateControls.Count; i++ )
            {
                Control c = this.templateControls[i];
                Element e = this.templateElements[i];
                
                Debug.Assert(e != null);

                c.AttachTo(e, null);
            }

            this.CompleteApplyTemplate();
        }

        private void RemoveSpinner()
        {
            ElementUtilities.RemovePendingOperation("ctl" + this.Id);

            this.hasShownSpinner = false;
        }

        private void CompleteApplyTemplate()
        {
            this.templateWasApplied = true;

            this.RemoveSpinner();

            if (this.Visible && this.isDelayHidden)
            {
                this.isDelayHidden = false;
                this.MakeElementVisible();
            }

            this.OnApplyTemplate();

            this.OnBaseControlsElementsPostInit();

            if (this.fireUpdateOnTemplateComplete)
            {
                this.fireUpdateOnTemplateComplete = false;

                this.ApplyOnUpdate();
            }

            if (this.TemplateApplied != null)
            {
                this.TemplateApplied(this, EventArgs.Empty);
            }
        }

        private void ApplyOnUpdate()
        {
            if (!this.enqueueUpdates)
            {
                this.OnUpdate();
                return;
            }

            if (this.updatePending)
            {
                return;
            }

            this.updatePending = true;
            Window.SetTimeout(this.ApplyOnUpdateContinue, 5);
        }

        private void ApplyOnUpdateContinue()
        {
            this.updatePending = false;
            this.OnUpdate();
        }

        private Element GetElementFromPath(Element element, List<int> path)
        {           
            for (int j = 0; j < path.Count; j++)
            {
                element = element.Children[path[j]];
            }

            return element;
        }

        public void EnsureElements()
        {
            this.EnsureElementsForce(false);
        }

        private void EnsureElementsForce(bool force)
        {
            Element thisElement = this.Element;

            if (thisElement == null)
            {
                String tagName = this.TagName;

                if (tagName == null) { tagName = "div"; }

                this.element = Document.CreateElement(tagName);

                if (this.effects != null)
                {
                    this.effects.Element = this.element;
                }

                this.jqueryObject = null;

                this.HandleInteractionEventing();

                this.templateWasApplied = false;
                this.elementsEnsured = false;

                this.EnsureId();

                thisElement = this.Element;

                if (!this.Visible || this.delayApplyTemplate)
                {
                    thisElement.Style.Display = "none";
                }
                else
                {
                    thisElement.Style.Display = "";
                }
            }

            if (force || (this.Visible && !this.delayApplyTemplate))
            {
                this.ApplyTemplate();

                if (!this.childControlsCreated)
                {
                    this.CreateChildControls();
                }

                if (!this.ElementsEnsured)
                {
                    this.elementsEnsured = true;

                    this.ApplyClass();

                    this.OnEnsureElements();

                    if (this.innerHtml != null)
                    {
                        HtmlUtilities.SetInnerHtml(this.ContentElement, this.innerHtml);
                        this.hasShownSpinner = false;
                    }

                    if (this.templateControls != null)
                    {
                        foreach (Control c in this.templateControls)
                        {
                            this.EnsureElementForBaseControl(c);
                        }
                    }

                    this.HandleResizeEventing();

                    if (this.height != null)
                    {
                        thisElement.Style.PosHeight = (int)this.Height;
                    }

                    if (this.width != null)
                    {
                        thisElement.Style.PosWidth = (int)this.Width;
                    }

                    this.ApplyOnUpdate();
                }

                if (this.trackResizeEvents)
                {
                    this.DoResize();
                }
            }
            else
            {
                this.ensureElementsRequested = true;
            }

            this.ApplyVisibility();
        }        

        public Element GetTemplateElementById(String id)
        {
            if (this.templateStandaloneElements.ContainsKey(id))
            {
                return this.templateStandaloneElements[id];
            }

            return null;
        }

        public Control GetTemplateControlById(String id)
        {
            if (this.templateControls == null)
            {
                return null;
            }

            foreach (Control c in this.templateControls)
            {
                if (c.IdWithinParentControl == id)
                {
                    return c;
                }
            }

            if (this.templateDescendentControls == null)
            {
                return null;
            }

            foreach (Control c in this.templateDescendentControls)
            {
                if (c.IdWithinParentControl == id)
                {
                    return c;
                }
            }

            return null;
        }

        protected void ApplyVisibility()
        {
            Script.Literal("if (window.requestAnimationFrame) {{window.requestAnimationFrame({0});}}else{{window.setTimeout({0}, 1);}}", this.applyVisibleOnFrameAction);
        }

        private void MakeElementVisible()
        {
            if (!this.isDelayHidden)
            {
                if (this.oldDisplayMode != null)
                {
                    this.Element.Style.Display = this.oldDisplayMode;
                    this.oldDisplayMode = null;
                }
                else
                {
                    this.Element.Style.Display = "";
                }
            }
        }

        private void MakeElementHidden()
        {
            String displayMode = this.Element.Style.Display;

            if (displayMode != "none" && !String.IsNullOrEmpty(displayMode))
            {
                this.oldDisplayMode = displayMode;
            }
            else
            {
                this.oldDisplayMode = null;
            }

            this.Element.Style.Display = "none";
        }

        private void ApplyVisibleOnAnimationFrame()
        {
            if (this.Element != null)
            {
                if (this.visible)
                {
                    this.MakeElementVisible();

                    this.OnVisibilityChanged();

                    if (this.trackResizeEvents)
                    {
                        this.OnResize();
                        Window.SetTimeout(this.OnResize, 1);
                    }
                }
                else
                {
                    this.MakeElementHidden();

                    this.OnVisibilityChanged();
                }
            }
        }

        protected virtual void OnVisibilityChanged()
        {

        }

        public virtual void Dispose()
        {
            if (this.hasShownSpinner)
            {
                ElementUtilities.RemovePendingOperation("ctl" + this.Id);
            }

            this.DisposeInteractionEventing();
            this.DisposeResizeEventing();

            this.parentControl = null;

            if (this.clickEventDelegates != null)
            {
                foreach (ElementAndEvent del in this.clickEventDelegates)
                {
                    del.Element.RemoveEventListener("click", del.Event, true);
                    del.Element.RemoveEventListener("touchend", del.Event, true);
                }
            }

            this.ClearTemplate();

            if (this.templateDescendentControls != null)
            {
                foreach (Control c in this.templateDescendentControls)
                {
                    if (c is IDisposable)
                    {
                        ((IDisposable)c).Dispose();
                    }
                }
            }

            Debug.WriteLine("(Control::Dispose) - Disposed " + this.TypeId + " " + this.Id);
        }

        /// <summary>
        /// Internal function that helps set up a child control and creates an element for it if it doesn't exist.
        /// </summary>
        /// <param name="child"></param>
        private void EnsureElementForBaseControl(Control child)
        {
            if (child.Element == null)
            {
                Element controlDiv = Document.CreateElement("DIV");
                child.Element = controlDiv;

                this.ContentElement.AppendChild(controlDiv);
            }
            else
            {
                bool foundElt = false;

                Element elt = child.Element;

                Element parentNode = elt;

                do
                {
                    parentNode = parentNode.ParentNode;
                    if (parentNode == this.ContentElement)
                    {
                        foundElt = true;
                    }
                } while (!foundElt && parentNode != null);

                if (!foundElt)
                {
                    this.ContentElement.AppendChild(elt);
                }
                this.OnBaseControlsElementsPostInit();

                child.EnsureElements();
            }
        }

        /// <summary>
        /// Adds a child control to this control.
        /// </summary>
        /// <param name="child">Child control to add.</param>
        protected void AddBaseControl(Control child)
        {
            if (this.templateControls == null)
            {
                this.templateControls = new List<Control>();
            }

            Debug.Assert(!this.templateControls.Contains(child));
            child.ParentControl = this;
            this.templateControls.Add(child);

            if (this.ElementsEnsured)
            {
                this.EnsureElementForBaseControl(child);
            }
        }

        /// <summary>
        /// Creates a new DOM element.
        /// </summary>
        /// <param name="elementClass">Modifier class.</param>
        /// <returns></returns>
        protected Element CreateElementWithTypeAndAdditionalClasses(String elementClass, String type, String additionalClasses)
        {
            Element e = Document.CreateElement(type);

            e.ClassName = this.GetElementClass(elementClass) + " " + additionalClasses;

            return e;
        }
        /// <summary>
        /// Creates a new DOM element.
        /// </summary>
        /// <param name="elementClass">Modifier class.</param>
        /// <returns></returns>
        protected Element CreateElementWithType(String elementClass, String type)
        {
            Element e = Document.CreateElement(type);

            e.ClassName = this.GetElementClass(elementClass);

            return e;
        }
        /// <summary>
        /// Creates a new DOM element.
        /// </summary>
        /// <param name="elementClass">Modifier class.</param>
        /// <returns></returns>
        protected Element CreateElement(String elementClass)
        {
            Element e = Document.CreateElement("DIV");

            e.ClassName = this.GetElementClass(elementClass);

            return e;
        }

        /// <summary>
        /// Creates a new DOM element.
        /// </summary>
        /// <param name="elementClass">Modifier class.</param>
        /// <returns></returns>
        protected Element CreateElementWithAdditionalClasses(String elementClass, String additionalClasses)
        {
            Element e = Document.CreateElement("DIV");

            e.ClassName = this.GetElementClass(elementClass) + " " + additionalClasses;

            return e;
        }
        

        protected Element CreateTemplateChild(String elementName)
        {
            Element e = Document.CreateElement("DIV");

            e.ClassName = this.GetElementClass(elementName);

            return e;
        }

        public void Update()
        {
            if (!this.elementsEnsured)
            {
                return;
            }

            this.ApplyOnUpdate();
        }

        protected virtual void OnInit()
        {
        }

        protected virtual void OnCreateChildControls()
        {
        }

        protected virtual void OnEnsureElements()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnBaseControlsElementsPostInit()
        {
        }

        private void DisposeResizeEventing()
        {
            if (this.hookedResize)
            {
                Window.RemoveEventListener("resize", this.resizeHandler, true);
                this.hookedResize = false;
            }
        }

        private void HandleResizeEventing()
        {
            if (this.resizeHandler == null)
            {
                this.resizeHandler = this.HandleResize;
            }

            if (!this.hookedResize && (this.trackResizeEvents && this.Visible))
            {
                Window.AddEventListener("resize", this.resizeHandler, true);
                this.hookedResize = true;
            }
            else if (this.hookedResize && (!this.trackResizeEvents || !this.Visible))
            {
                this.DisposeResizeEventing();
                this.hookedResize = false;
            }
        }
        
        
        private void HandleWindowScroll(ElementEvent e)
        {
            ElementUtilities.UpdateLastScrollTime();
        }


        private void HandleResize(ElementEvent e)
        {
            this.DoResize();
        }

        private void DoResize()
        {
            Window.SetTimeout(this.OnResize, 1);
        }

        protected virtual void OnResize()
        {

        }
    }
}
