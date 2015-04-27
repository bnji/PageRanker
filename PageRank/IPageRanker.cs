using System;
using System.Collections.Generic;

namespace bendot
{
    /// <summary>
    /// Interface for the Google PageRanker
    /// More about Google PageRank: http://infolab.stanford.edu/pub/papers/google.pdf
    /// </summary>
    public interface IPageRanker
    {
        /// <summary>
        /// Calculates the Graph values based on Google PageRank algorithm
        /// </summary>
        void Calculate();
    }
}
