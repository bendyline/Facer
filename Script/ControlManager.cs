// ControlManager.cs
//

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
