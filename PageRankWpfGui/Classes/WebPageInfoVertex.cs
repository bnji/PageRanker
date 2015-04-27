using bendot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PageRankWpfGui.Classes
{
    /// <summary>
    /// A simple identifiable vertex.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{ID}-{IsMale}")]

    public class WebPageInfoVertex
    {
        public string ID { get; private set; }
        public WebPageInfo WebPageInfo { get; private set; }

        public WebPageInfoVertex(string id, WebPageInfo _wpi)
        {
            ID = id;
            WebPageInfo = _wpi;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", ID, WebPageInfo.Url);
        }
    }
}
