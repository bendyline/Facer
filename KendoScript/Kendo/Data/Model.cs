using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace kendo.data
{
    [Imported]
    public class Model
    {
        public String Id;
        public Dictionary<String, ModelField> Fields;

        public static Model Define(ModelOptions mo) { return null;  }
    }
}
