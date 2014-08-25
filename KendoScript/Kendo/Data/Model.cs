using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace kendo.data
{
    [Imported]
    public class Model : ObservableObject
    {
        public bool IsNew()
        {
            return false;
        }

        public static Model Define(ModelOptions mo) { return null;  }
    }
}
