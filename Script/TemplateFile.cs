// TemplateFile.cs
//

using System;
using System.Collections.Generic;
using jQueryApi;

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

        public static String rootTemplatePath = "/gs/t/";

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

        public void EnsureRetrieving(String id, AsyncCallback callback, object state)
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
                jQuery.GetJson(rootTemplatePath + this.fileName + ".t.json", new AjaxCallback<object>(this.TemplatesRetrieved));
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
                    CallbackResult cr = new CallbackResult();

                    cr.Data = this.tm.Templates[(String)cs.Tag];
                    cr.IsCompleted = true;

                    cs.Callback(cr);
                }
            }
        }
    }
}
