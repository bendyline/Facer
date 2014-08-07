using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace kendo.data
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]

    public class ModelOptions
    {
        public String Id;
        public Dictionary<String, ModelField> Fields;
    }
}
