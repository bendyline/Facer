using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kendo.UI
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]
    public class ExportFileOptions
    {
        public String FileName;
        public bool ForceProxy;

        [ScriptName("proxyURL")]
        public String ProxyUrl;
    }
}
