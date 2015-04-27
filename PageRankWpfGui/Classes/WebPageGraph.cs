using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;

namespace PageRankWpfGui.Classes
{
    public class WebPageGraph : BidirectionalGraph<WebPageInfoVertex, WebPageInfoEdge>
    {
        public WebPageGraph() { }

        public WebPageGraph(bool allowParallelEdges)
            : base(allowParallelEdges) { }

        public WebPageGraph(bool allowParallelEdges, int vertexCapacity)
            : base(allowParallelEdges, vertexCapacity) { }
    }
}