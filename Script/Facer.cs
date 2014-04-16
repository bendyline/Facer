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
    [IgnoreNamespace]
    public static class Facer
    {
        public static void AttachToElements()
        {
            ElementCollection elements = Document.Body.QuerySelectorAll("div[data-bl-type]");

            Facer.ProcessElements(elements);
        }

        private static void ProcessElements(ElementCollection ec)
        {
            for (int i = 0; i < ec.Length; i++ )
            {
                Element e = ec[i];

                ElementAttribute ea = e.Attributes.GetNamedItem("data-bl-type");

                if (ea != null)
                {

                    if (!String.IsNullOrEmpty(ea.Value))
                    {
                        Control c = ControlManager.Current.Create(ea.Value);

                        if (c != null)
                        {
                            c.AttachToElement(e);
                        }
                    }
                }
            }
        }
    }
}