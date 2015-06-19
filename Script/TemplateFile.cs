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
            if (this.isLoaded)
            {
                if (callback != null)
                {
                    CallbackResult.NotifySynchronousSuccess(callback, state, this);
                }

                return;
            }

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
                this.isLoaded = true;
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
