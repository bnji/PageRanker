using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bendot
{
    /// <summary>
    /// PageRank Class
    /// The class is abstract, since it should not be able to be instanciated.
    /// Nodes/Vertices in a graph, which represents a network of pages where the edges are hyperlinks, should extend this class.
    /// </summary>
    public abstract class PageRank
    {
        /// <summary>
        /// The Webpage URL
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// The score for the Page of type T.
        /// This value change be changed whenever the user of the class wants to.
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// The amount of outgoing links from the site
        /// This should only be set from within the class which extends this class
        /// </summary>
        public int OutgoingLinkCount { get; protected set; }
    }
}
