using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bendot
{
    /// <summary>
    /// Graph (Wrapper) class, which contains some extra relevant information about the graph.
    /// 
    /// It should not be possible to directly manipulate or access the graph (m_Data, but if functionality is needed for the consumer
    /// of this class, then it's properties or/and methods should be provided by this class/interface!
    /// </summary>
    public class Graph : IGraph
    {
        /// <summary>
        /// The starting point
        /// </summary>
        private Uri m_StartUrl = null;
        public Uri StartUrl { get { return m_StartUrl; } private set { m_StartUrl = value; } }
        /// <summary>
        /// A mutable directed graph, which will hold information about WebPages (website information)
        /// </summary>
        private BidirectionalGraph<WebPageInfo, IEdge<WebPageInfo>> m_GraphData;
        //Enable this if you want to manipulat the Path directly
        public BidirectionalGraph<WebPageInfo, IEdge<WebPageInfo>> GraphData { get { return m_GraphData; } private set { m_GraphData = value; } }
        

        /// <summary>
        /// Graph constructor.
        /// </summary>
        public Graph()
        {
            m_GraphData = new BidirectionalGraph<WebPageInfo, IEdge<WebPageInfo>>();
        }

        /// <summary>
        /// Graph constructor which intitally sets the starting URL.
        /// </summary>
        /// <param name="_startUrl"></param>
        public Graph(Uri _startUrl)
            : this()
        {
            this.m_StartUrl = _startUrl;
        }

        /// <summary>
        /// Get the graph's edge count.
        /// </summary>
        /// <returns>The graph's edge count</returns>
        public int GetEdgeCount()
        {
            if (this.m_GraphData != null)
            {
                return this.m_GraphData.Edges.Count();
            }
            return 0;
        }

        /// <summary>
        /// Get the graph's vertex count.
        /// </summary>
        /// <returns>The graph's vertex count</returns>
        public int GetVertexCount()
        {
            if (this.m_GraphData != null)
            {
                return this.m_GraphData.VertexCount;
            }
            return 0;
        }

        /// <summary>
        /// Gets the edges (wrapper for the graph)
        /// </summary>
        public IEnumerable<IEdge<WebPageInfo>> Edges
        {
            get
            {
                IEnumerable<IEdge<WebPageInfo>> edges = null;
                if (this.m_GraphData != null)
                {
                    edges = this.m_GraphData.Edges;
                }
                return edges;
            }
        }

        /// <summary>
        /// Add a vertex (node) to the graph.
        /// </summary>
        /// <param name="wpi">WebPageInfo object</param>
        /// <returns></returns>
        public bool AddVertex(WebPageInfo v)
        {
            if (this.m_GraphData != null && v != null)
            {
                return this.m_GraphData.AddVertex(v);
            }
            return false;
        }

        /// <summary>
        /// Add a edge (line) to the graph.
        /// </summary>
        /// <param name="wpi">Edge<WebPageInfo> object</param>
        /// <returns></returns>
        public bool AddEdge(IEdge<WebPageInfo> e)
        {
            if (this.m_GraphData != null && e != null)
            {
                return this.m_GraphData.AddEdge(e);
            }
            return false;
        }
    }
}
