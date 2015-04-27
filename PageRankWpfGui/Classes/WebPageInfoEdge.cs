using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PageRankWpfGui.Classes
{
    /// <summary>
    /// A simple identifiable edge.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Source.ID} -> {Target.ID}")]
    public class WebPageInfoEdge : Edge<WebPageInfoVertex>
    {
        public string ID
        {
            get;
            private set;
        }

        public WebPageInfoEdge(string id, WebPageInfoVertex source, WebPageInfoVertex target)
            : base(source, target)
        {
            ID = id;
        }
    }
}
