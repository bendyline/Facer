/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using jQueryApi;
using System;
using System.Collections.Generic;

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

        public void LoadScript(String scriptPath, String scriptToken, AsyncCallback callback, object state)
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

            object o = null;

            try
            {
                o = Script.Eval(scriptItem);
            }
            catch 
            {
                o = null;
            }

            if (Script.IsNullOrUndefined(o))
            {
                return false;
            }
            else
            {
                this.scriptItemStatuses[scriptItem] = true;
                return true;
            }
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

        public Control CreateControlFromFullName(String fullName)
        {
            int lastPeriod = fullName.LastIndexOf(".");

            if (lastPeriod < 0)
            {
                return null;
            }

            String namespaceStr = fullName.Substring(0, lastPeriod);
            String typeNameStr = fullName.Substring(lastPeriod + 1, fullName.Length);

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
