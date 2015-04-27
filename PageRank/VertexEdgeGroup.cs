using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bendot
{
    public class VertexEdgeGroup
    {
        public WebPageInfo Vertex { get; set; }
        private IEdge<WebPageInfo> m_Edge = null;
        public IEdge<WebPageInfo> Edge { get { return m_Edge; } set { m_Edge = value; } }

        public VertexEdgeGroup(WebPageInfo _v)
        {
            Vertex = _v;
        }

        public VertexEdgeGroup(WebPageInfo _v, IEdge<WebPageInfo> _e)
            : this(_v)
        {
            Edge = _e;
        }
    }
}
