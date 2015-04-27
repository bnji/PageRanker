using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bendot
{
    public class SpiderData
    {
        public Uri StartUrl { get; set; }
        public UriKind UrlType { get; set; }
        public Boolean OnlySameDomain { get; set; }
        public String[] AllowedContentTypes { get; set; }
        public bool IsDebugMode { get; set; }

        public int MaxDepth { get; set; }
    }
}
