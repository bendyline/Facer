using kendo.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kendo.UI
{
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Object")]
    public class ProtocolTransport
    {
        public String ContentType;
        public object Data;
        public String DataType;
        public String Type;
        public String Url;

        public object Read;
        public object Destroy;
        public object Create;
        public String ThumbnailUrl;
        public String UploadUrl;
        public String ImageUrl;
    }
}
