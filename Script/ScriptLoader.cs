/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using BL.UI;
using jQueryApi;
using System.Diagnostics;
using System.Collections.Generic;

namespace BL.UI
{
    public class ScriptLoader
    {
        private Operation operation;
        private String scriptPath;
        private String scriptToken;
        private bool isLoaded = false;

        private static List<ScriptLoader> pendingScriptLoaders = new List<ScriptLoader>();

        public String ScriptPath
        {
            get
            {
                return this.scriptPath;
            }

            set
            {
                this.scriptPath = value;
            }
        }

        public Operation Operation
        {
            get
            {
                return this.operation;
            }
        }

        public ScriptLoader(String scriptPath, String scriptToken)
        {
            this.scriptPath = scriptPath;
            this.operation = null;
            this.scriptToken = scriptToken;
        }       

        public void Load()
        {
            // ensure scripts load in order, so only load one at a time, in the order that Load is requested.
            int currentPendingCount = pendingScriptLoaders.Count;

            pendingScriptLoaders.Add(this);

            if (currentPendingCount == 0)
            {
                pendingScriptLoaders[0].LoadInternal();
            }
        }

        internal void LoadInternal()
        {
            jQuery.GetScript(this.scriptPath, this.ScriptLoadedContinue);
        }

        private void ScriptLoadedContinue(object o)
        {
            this.isLoaded = true;

            if (this.operation != null)
            {
                Operation op = this.operation;
                this.operation = null;

                Debug.Assert(ControlManager.Current.IsLoadedScriptItem(this.scriptToken), "JavaScript object '" + this.scriptToken + "' was not found in target file '" + this.scriptPath + "'");
                op.CompleteAsAsyncDone(scriptPath);
            }

            pendingScriptLoaders.RemoveAt(0);

            if (pendingScriptLoaders.Count > 0)
            {
                pendingScriptLoaders[0].LoadInternal();
            }
        }

        public void EnsureLoaded(AsyncCallback callback, object state)
        {
            if (this.isLoaded)
            {
                if (callback != null)
                {
                    CallbackResult cr = new CallbackResult();
                    cr.IsCompleted = true;
                    cr.AsyncState = state;

                    callback(cr);
                }

                return;
            }

            bool isNew = false;

            if (this.operation == null)
            {
                this.operation = new Operation();
                isNew = true;
            }

            this.operation.AddCallback(callback, state);

            if (isNew)
            {
//                Debug.Assert(!this.isLoaded);
                if (this.isLoaded)
                {
                    if (callback != null)
                    {
                        CallbackResult cr = new CallbackResult();
                        cr.IsCompleted = true;
                        cr.AsyncState = state;

                        callback(cr);
                    }
                }
                else
                {
                    this.Load();
                }
            }
        }
    }
}
