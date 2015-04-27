using QuickGraph;
using System;
using System.Collections.Generic;

namespace bendot
{
    /// <summary>
    /// Graph Wrapper Interface
    /// </summary>
    public interface IGraph
    {
        bool AddEdge(IEdge<WebPageInfo> e);
        bool AddVertex(WebPageInfo v);
        Uri StartUrl { get; }
        IEnumerable<IEdge<WebPageInfo>> Edges { get; }
        int GetEdgeCount();
        int GetVertexCount();
    }
}
