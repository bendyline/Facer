/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using jQueryApi;
using System;
using System.Collections.Generic;
using System.Html;

namespace BL.UI
{
    public class ControlManager
    {
        private Dictionary<String, IControlFactory> factoriesByShortNamespace;
        private Dictionary<string, bool> scriptItemStatuses;
        private Dictionary<String, ScriptLoader> loadersByScript;

        private static ControlManager current = new ControlManager();

        public static ControlManager Current
        {
            get
            {
                return current;
            }
        }

        public ControlManager()
        {
            this.loadersByScript = new Dictionary<string, ScriptLoader>();
            this.factoriesByShortNamespace = new Dictionary<string, IControlFactory>();
            this.scriptItemStatuses = new Dictionary<string, bool>();
        }

        public bool HasStylesheet(String stylesheetPath)
        {
            ElementCollection ec = Document.GetElementsByTagName("LINK");
            String cssPath = stylesheetPath.ToLowerCase().Trim();

            for (int i = 0; i < ec.Length; i++ )
            {
                Element e = ec[i];

                String href = (String)e.GetAttribute("href");

                if (href != null)
                {
                    href = href.ToLowerCase().Trim();

                    if (href == cssPath)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }

        public void EnsureStylesheet(String stylesheetPath)
        {
            if (!HasStylesheet(stylesheetPath))
            {
                Element e = Document.CreateElement("LINK");
                e.SetAttribute("rel", "stylesheet");
                e.SetAttribute("type", "text/css");
                e.SetAttribute("href", stylesheetPath);

                Document.GetElementsByTagName("head")[0].AppendChild(e);
            }
        }

        public void CreateControlFromFullTypeAsync(String fullType, AsyncCallback callback, object state)
        {
            int lastPeriod = fullType.LastIndexOf(".");

            if (lastPeriod <= 0)
            {
                if (callback != null)
                {
                    CallbackResult.NotifySynchronousFailure(callback, state, "invalidTypeName");
                    return;
                }
            }

            this.CreateControlAsync(fullType.Substring(0, lastPeriod), fullType.Substring(lastPeriod + 1, fullType.Length), callback, state);
        }

        public void CreateControlAsync(String namespaceName, String typeName, AsyncCallback callback, object state)
        {
            String adjust = String.Format(Context.Current.ScriptLibraryTemplate, namespaceName.ToLowerCase());

            adjust += "?v=" + Context.Current.VersionToken;

            CallbackState cs = CallbackState.Wrap(callback, state);

            cs.Tag = namespaceName + "." + typeName;

            String path = UrlUtilities.EnsurePathEndsWithSlash(Context.Current.ResourceBasePath) + adjust;

            if (IsLoadedScriptItem(namespaceName + "." + typeName))
            {
                CallbackResult.NotifySynchronousSuccess(this.CreateControlAsyncContinue, cs, null);
            }
            else
            {
                this.LoadScript(namespaceName, path, this.CreateControlAsyncContinue, cs);
            }
        }

        private void CreateControlAsyncContinue(IAsyncResult result)
        {
            CallbackState cs = (CallbackState)result.AsyncState;

            String fullTypeName = (String)cs.Tag;

            object o = Script.Eval("new " + fullTypeName + "()");

            if (cs.Callback != null)
            {
                CallbackResult cr = new CallbackResult();
                cr.Data = o;
                cr.AsyncState = cs.State;

                cs.Callback(cr);
            }
        }

        public void LoadScript(String scriptToken, String scriptPath, AsyncCallback callback, object state)
        {
            ScriptLoader sl = null;

            if (this.loadersByScript.ContainsKey(scriptPath))
            {
                sl = this.loadersByScript[scriptPath];
            }
            else
            {
                this.loadersByScript[scriptPath] = new ScriptLoader(scriptPath, scriptToken);
                sl = this.loadersByScript[scriptPath];
            }

            sl.EnsureLoaded(callback, state);
        }

        public bool IsLoadedScriptItem(String scriptItem)
        {
            if (this.scriptItemStatuses.ContainsKey(scriptItem))
            {                
                if (this.scriptItemStatuses[scriptItem] == true)
                {
                    return true;
                }
            }

            object previousObject = null;

            String currentType = scriptItem;

            int firstPeriod = scriptItem.IndexOf(".");
            bool isEndType = false;

            while (!isEndType)
            {
                String type = null;

                if (firstPeriod < 0)
                {
                    firstPeriod = currentType.Length;
                    isEndType = true;

                    type = currentType;
                }
                else
                {
                    type = currentType.Substring(0, firstPeriod);
                    currentType = currentType.Substring(firstPeriod + 1, currentType.Length);
                    firstPeriod = currentType.IndexOf(".");
                }

                if (previousObject == null)
                {
                    Script.Literal("{0}=window[{1}]", previousObject, type);
                }
                else
                {
                    Script.Literal("{0}={0}[{1}]", previousObject, type);
                }

                if (Script.IsNullOrUndefined(previousObject))
                {
                    this.scriptItemStatuses[scriptItem] = false;
                    return false;
                }
                else if (isEndType)
                {
                    this.scriptItemStatuses[scriptItem] = true;
                    return true;
                }
            }

            this.scriptItemStatuses[scriptItem] = false;
            return false;
        }

        public Control Create(String fullControlName)
        {
            object c = null;

            for (int i = 0; i < fullControlName.Length; i++ )
            {
                char ch = fullControlName.CharAt(i);

                if (!((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9') || ch == '.'))
                {
                    throw new Exception("Not a valid control name");
                }
            }

            int lastPeriod = fullControlName.LastIndexOf(".");

            if (lastPeriod < 0)
            {
                throw new Exception("Not a valid control name");
            }

            if (c == null)
            {
                c = Script.Eval("new " + fullControlName + "()");
            }

            return (Control)c;
        }

        public Control CreateControlFromFullType(String fullType)
        {
            int lastPeriod = fullType.LastIndexOf(".");

            if (lastPeriod < 0)
            {
                return null;
            }

            String namespaceStr = fullType.Substring(0, lastPeriod);
            String typeNameStr = fullType.Substring(lastPeriod + 1, fullType.Length);

            return CreateControl(namespaceStr, typeNameStr);
        }

        public Control CreateControl(string controlNamespace, String controlName)
        {
            controlNamespace = controlNamespace.ToLowerCase();
            controlName = controlName.ToLowerCase();

            if (this.factoriesByShortNamespace.ContainsKey(controlNamespace))
            {
                IControlFactory cf = this.factoriesByShortNamespace[controlNamespace];

                return cf.CreateControl(controlName);
            }

            return null;
        }

        public void Register(IControlFactory factory)
        {
            this.factoriesByShortNamespace[factory.Namespace] = factory;
        }
    }
}
