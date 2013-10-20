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
    public abstract class Control : SerializableObject, IDisposable
    {
        private Element element;
        private Element contentElement;
        private List<Control> templateControls;
        private List<Element> templateElements;
        private String templateId;
        private String template;
        private bool initted = false;
        private bool childControlsCreated = false;
        private bool elementsEnsured = false;
        private String innerHtml;
        private String typeId = null;
        private String className;
        private jQueryObject jqueryObject;
        private String id;
        private bool visible = true;
        protected bool trackResizeEvents = false;
        private bool hookedResize = false;
        private String oldDisplayMode;
        private int? height;
        private int? width;
        private bool templateWasApplied = false;
        private bool trackInteractionEvents = false;
        private bool interactionEventsRegistered = false;

        [ScriptName("s_templateId")]
        public String TemplateId
        {
            get
            {
                if (this.templateId == null)
                {
                    this.templateId = this.GetType().FullName.Replace(".", "-").ToLowerCase();
                }

                return this.templateId;
            }

            set
            {
                this.templateId = value;
                this.templateWasApplied = false;
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

        public int? Width
        {
            get
            {
                return this.width;
            }

            set
            {
                this.width = value;
            }
        }

        public virtual String TagName
        {
            get
            {
                return null;
            }
        }

        public int? Height
        {
            get
            {
                return this.height;
            }

            set
            {
                this.height = value;

                Element thisElement = this.Element;

                if (thisElement != null)
                {
                    thisElement.Style.PosHeight = (int)this.height;
                }
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

        public bool Visible
        {
            get
            {
                return this.visible;
            }

            set
            {
                this.visible = value;

                Element thisElement = this.Element;

                if (thisElement != null)
                {
                    if (this.visible)
                    {
                        if (this.oldDisplayMode != null)
                        {
                          //  thisElement.Style.Display = this.oldDisplayMode;
                            this.oldDisplayMode = null;
                        }
                        else
                        {
                    //        thisElement.Style.Display = "inherit";
                        }

                        this.DoResize();
                    }
                    else 
                    {
                        String displayMode = thisElement.Style.Display;

                        if (displayMode != "none" && !String.IsNullOrEmpty(displayMode))
                        {
                            this.oldDisplayMode = displayMode;
                        }
                        else
                        {
                            this.oldDisplayMode = "inherit";
                        }
                    }

                    this.HandleResizeEventing();
                }
            }
        }

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
                    this.ContentElement.InnerHTML = value;
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
        }


        private void HandleClick(ElementEvent e)
        {
            this.OnClick(e);
        }

        protected virtual void OnClick(ElementEvent e)
        {

        }

        private void HandleInteractionEventing()
        {
            if (!this.trackInteractionEvents)
            {
                if (this.interactionEventsRegistered)
                {
                    this.element.RemoveEventListener("click", this.HandleClick, true);
                    this.interactionEventsRegistered = false;
                }

                return;
            }

            if (!interactionEventsRegistered && this.element != null)
            {
                this.element.AddEventListener("click", this.HandleClick, true);
                this.interactionEventsRegistered = true;
            }
        }

        private void ApplyClass()
        {
            if (this.Element != null)
            {
                if (this.className != null)
                {
                    this.Element.ClassName = this.TypeId + " " + this.className;
                }
                else if (this.DefaultClass != null)
                {
                    this.Element.ClassName = this.TypeId + " " + this.DefaultClass;
                }
                else
                {
                    this.Element.ClassName = this.TypeId;
                }
            }
        }


        public void EnsureId()
        {
            if (this.id != null)
            {
                return;
            }

            if (this.Element != null)
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

        public void AttachTo(object elementOrId, object options)
        {
            Script.Literal(@"if (elementOrId != null) {{ if (elementOrId.tagName != null) {{ {0} = elementOrId; }} if (typeof(elementOrId) == ""string"") {{ {0} = document.getElementById(elementOrId); }}}}", this.element);

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

        public virtual void OnApplyTemplate()
        {

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

        private void ApplyTemplate()
        {
            if (this.templateWasApplied)
            {
                return;
            }

            if (this.Template != null)
            {
                this.templateWasApplied = true;
                this.ApplyTemplateContinue();
                return;
            }

            String templateId = this.TemplateId;

            if (templateId != null)
            {
                this.templateWasApplied = true; 
                TemplateManager.Current.GetTemplateAsync(templateId, this.HandleApplyTemplateContinue, null);
            }
        }

        private void HandleApplyTemplateContinue(IAsyncResult result)
        {
            Template t = (Template)result.Data;

            if (t != null)
            {
                this.template = t.Content;
                this.ApplyTemplateContinue();
            }
            else
            {
                this.OnBaseControlsElementsPostInit();
            }
        }

        private void ApplyTemplateContinue()
        {
            String template = this.Template;

            this.templateControls = new List<Control>();
            this.templateElements = new List<Element>();

            if (template != null)
            {
                TemplateParser tp = new TemplateParser();

                TemplateParserResult tpr = tp.Parse(this.Id, this.TemplateId, template);

                this.Element.InnerHTML = tpr.Markup;

                if (tpr.ItemsContainer != null)
                {
                    Element e = this.GetElementFromPath(this.Element, tpr.ItemsContainer);

                    Script.Literal("var fn = {0}[\"set_itemsContainerElement\"];  if (fn != null) {{ fn.apply(this, new Array({1})); }}", this, e);
                }

                for (int i = 0; i < tpr.Controls.Count; i++)
                {
                    List<int> path = tpr.ControlElementPaths[i];
                    Control c = tpr.Controls[i];
                    String controlId = tpr.ControlIds[i];

                    Element element = this.GetElementFromPath(this.Element, path);

                    this.templateControls.Add(c);
                    this.templateElements.Add(element);

                    if (controlId != null)
                    {
                        Script.Literal("{0}['c_' + {1}]={2}", this, controlId, c);
                        Script.Literal("{0}['e_' + {1}]={2}", this, controlId, element);
                    }
                }

                for (int i = 0; i < tpr.ElementIds.Count; i++)
                {
                    List<int> path = tpr.ElementElementPaths[i];
                    String elementId = tpr.ElementIds[i];

                    Element element = this.GetElementFromPath(this.Element, path);

                 
                    if (elementId != null)
                    {
                        Script.Literal("{0}['e_' + {1}]={2}", this, elementId, element);
                    }
                }
            }

            for (int i = 0; i < this.templateControls.Count; i++ )
            {
                Control c = this.templateControls[i];
                Element e = this.templateElements[i];

                c.AttachTo(e, null);
            }

            this.OnApplyTemplate();

            this.OnBaseControlsElementsPostInit();
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
            Element thisElement = this.Element;

            if (thisElement == null)
            {
                this.element = Document.CreateElement(this.TagName);
                this.templateWasApplied = false;
                this.elementsEnsured = false;

                this.EnsureId();

                thisElement = this.Element;
            }

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
                    this.ContentElement.InnerHTML = this.innerHtml;
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

                this.OnUpdate();
            }

            if (this.trackResizeEvents)
            {
                this.DoResize();
            }
        }

        public virtual void Dispose()
        {

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

            this.templateControls.Add(child);

            if (this.ElementsEnsured)
            {
                this.EnsureElementForBaseControl(child);
            }
        }

        /// <summary>
        /// Creates a new DHTML element.
        /// </summary>
        /// <param name="elementClass">Modifier class.</param>
        /// <returns></returns>
        protected Element CreateElementWithType(String elementClass, String type)
        {
            Element e = Document.CreateElement(type);

            e.ClassName = this.TypeId + "-" + elementClass;

            return e;
        }
        /// <summary>
        /// Creates a new DHTML element.
        /// </summary>
        /// <param name="elementClass">Modifier class.</param>
        /// <returns></returns>
        protected Element CreateElement(String elementClass)
        {
            Element e = Document.CreateElement("DIV");

            e.ClassName = this.TypeId + "-" + elementClass;

            return e;
        }
        

        protected Element CreateTemplateChild(String elementName)
        {
            Element e = Document.CreateElement("DIV");

            e.ClassName = this.TypeId + "-" + elementName;

            return e;
        }

        public void Update()
        {
            if (!this.elementsEnsured)
            {
                return;
            }

            this.OnUpdate();
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

        private void HandleResizeEventing()
        {
            if (!this.hookedResize && (this.trackResizeEvents && this.Visible))
            {
                Window.AddEventListener("resize", this.HandleResize, true);
                this.hookedResize = true;
            }
            else if (this.hookedResize)
            {
                Window.RemoveEventListener("resize", this.HandleResize, true);
                this.hookedResize = false;
            }
        }

        private void HandleResize(ElementEvent e)
        {
            this.DoResize();
        }

        protected virtual void DoResize()
        {
            Window.SetTimeout(this.OnResize, 1);
        }

        protected virtual void OnResize()
        {

        }
    }
}
