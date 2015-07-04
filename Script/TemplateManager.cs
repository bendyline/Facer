/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;

namespace BL.UI
{
    public class TemplateManager
    {
        private static TemplateManager current = null;
        private Dictionary<String, Template> templatesById;
        private Dictionary<String, TemplateFile> templateFilesById;
        private Dictionary<String, String> templateOverrides;

        public Dictionary<String, Template> Templates
        {
            get
            {
                return this.templatesById;
            }
        }

        public Dictionary<String, TemplateFile> TemplateFiles
        {
            get
            {
                return this.templateFilesById;
            }
        }

        public static TemplateManager Current
        {
            get
            {
                if (current == null)
                {
                    current = new TemplateManager();
                }

                return current;
            }
        }

        public TemplateManager()
        {
            this.templatesById = new Dictionary<string, Template>();
            this.templateFilesById = new Dictionary<string, TemplateFile>();
            this.templateOverrides = new Dictionary<String, String>();
        }

        public void AddTemplateOverride(String from, String to)
        {
            this.templateOverrides[from.ToLowerCase()] = to.ToLowerCase();
        }

        public void AddTemplateFile(String fileName, List<Template> templates)
        {
            TemplateFile tf = null;

            if (this.templateFilesById[fileName] == null)
            {
                tf = new TemplateFile(this);
                fileName = fileName.ToLowerCase();

                tf.FileName = fileName;
                tf.IsLoaded = true;

                this.templateFilesById[fileName] = tf;
            }

            foreach (Template t in templates)
            {
                templatesById[t.Id.ToLowerCase()] = t;
            }
        }

        public void AddTemplates(List<Template> templates)
        {
            foreach (Template t in templates)
            {
                templatesById[t.Id.ToLowerCase()] = t;
            }
        }

        public bool GetTemplateAsync(String id, AsyncCallback ac, object state)
        {
            id = id.ToLowerCase();

            String overrideId = this.templateOverrides[id];

            if (overrideId != null)
            {
                id = overrideId;
            }

            if (this.templatesById[id] != null)
            {
                CallbackResult cr = new CallbackResult();
                cr.AsyncState = state;
                cr.Data = this.templatesById[id];

                cr.CompletedSynchronously = true;
                cr.IsCompleted = true;
              
                ac(cr);

                return true;
            }

            id = id.Replace(".", "-");

            int lastDash = id.LastIndexOf("-");

            if (lastDash >= 0)
            {
                String fileName = id.Substring(0, lastDash).Replace("-", ".").ToLowerCase();

                if (this.templateFilesById.ContainsKey(fileName))
                {
                    TemplateFile tf = this.templateFilesById[fileName];

                    if (tf.IsLoaded)
                    {
                        if (ac != null)
                        {
                            CallbackResult cr = new CallbackResult();
                            cr.AsyncState = state;
                            cr.Data = this.templatesById[id];
                            cr.CompletedSynchronously = true;
                            cr.IsCompleted = true;
                            ac(cr);
                        }
                        return true;
                    }
                    else
                    {
                        tf.EnsureRetrieved(id, ac, state);
                        return false;
                    }
                }
                else
                {
                    TemplateFile newTemplateFile = new TemplateFile(this);
                    newTemplateFile.FileName = fileName;

                    this.templateFilesById[fileName] = newTemplateFile;

                    newTemplateFile.EnsureRetrieved(id, ac, state);
                    return false;
                }
            }

            CallbackResult notfoundcr = new CallbackResult();
            notfoundcr.AsyncState = null;

            notfoundcr.CompletedSynchronously = true;
            notfoundcr.IsCompleted = false;
            ac(notfoundcr);
            return true;
        }
    }
}
