using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace kendo.data
{
    [Imported]
    public delegate void ModelEventHandler(ModelEventArgs e);

    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]
    public class ModelEventArgs 
    {
        public Model Model;
    }
}
