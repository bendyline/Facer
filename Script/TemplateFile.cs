/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using jQueryApi;
using System.Html;

namespace BL.UI
{
    public class TemplateFile
    {
        private Operation operation;
        private TemplateManager tm;
        private String fileName;
        private bool isLoaded = false;

        public String FileName
        {
            get
            {
                return this.fileName;
            }

            set
            {
                this.fileName = value;
            }
        }

        public bool IsLoaded
        {
            get
            {
                return this.isLoaded;
            }

            set
            {
                this.isLoaded = value;
            }
        }

        public static String rootTemplatePath = "gs/t/";
        public static String rootCssPath = "gs/t/";

        public Operation Operation
        {
            get
            {
                return operation;
            }
        }


        public TemplateFile(TemplateManager templateManager)
        {
            this.tm = templateManager;
        }

        public void EnsureRetrieved(String id, AsyncCallback callback, object state)
        {
            bool isNew = false;

            if (this.operation == null)
            {
                this.operation = new Operation();
                isNew = true;
            }

            this.operation.CallbackStates.Add(CallbackState.WrapWithTag(callback, state, id));

            if (isNew)
            {
                String cssPath = UrlUtilities.EnsurePathEndsWithSlash(Context.Current.ResourceBasePath) + rootCssPath + this.fileName + ".t.css?v=" + Context.Current.VersionToken;
                ControlManager.Current.EnsureStylesheet(cssPath);

                /*
                ElementCollection ec = Document.GetElementsByTagName("LINK");
                 
                bool foundCss = false;
                
                for (int i = 0; i < ec.Length; i++ )
                {
                    Element e = ec[i];

                    String href = (String)e.GetAttribute("href");

                    if (href == cssPath)
                    {
                        foundCss = true;
                    }
                }

                if (!foundCss)
                {
                    Element e = Document.CreateElement("LINK");
                    e.SetAttribute("rel", "stylesheet");
                    e.SetAttribute("type", "text/css");
                    e.SetAttribute("href", cssPath);

                    Document.GetElementsByTagName("head")[0].AppendChild(e);
                }*/

                jQuery.GetJson(UrlUtilities.EnsurePathEndsWithSlash(Context.Current.ResourceBasePath) + rootTemplatePath + this.fileName + ".t.json?v=" + Context.Current.VersionToken, new AjaxCallback<object>(this.TemplatesRetrieved));
            }
        }

        private void TemplatesRetrieved(object data)
        {
            if (data == null)
            {
                return;
            }
            else
            {
                List<Template> templates = (List<Template>)data;
                this.tm.AddTemplates(templates);
                this.IsLoaded = true;
            }

            Operation o = this.operation;

            if (o != null)
            {
                this.operation = null;

                foreach (CallbackState cs in o.CallbackStates)
                {
                    if (cs.Callback != null)
                    {
                        CallbackResult cr = new CallbackResult();

                        cr.Data = this.tm.Templates[(String)cs.Tag];
                        cr.IsCompleted = true;

                        cs.Callback(cr);
                    }
                }
            }
        }
    }
}
