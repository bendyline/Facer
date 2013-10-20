/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;

namespace BL.UI
{
    public class ControlManager
    {
        private Dictionary<String, IControlFactory> factoriesByShortNamespace;

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
            this.factoriesByShortNamespace = new Dictionary<string, IControlFactory>();
        }

        public Control Create(String fullControlName)
        {
            object c = null;
    
            if (c == null)
            {
                c = Script.Eval("new " + fullControlName + "()");
            }

            return (Control)c;
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
