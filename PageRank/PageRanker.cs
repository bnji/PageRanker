using QuickGraph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bendot
{
    /// <summary>
    /// Calculates the PageRank for the url's in a graph.
    /// </summary>
    public class PageRanker : IPageRanker
    {
        /// <summary>
        /// Holds a list of PageRank objects where the type of the Page object inside the PageRank object is WebPageInfo (information about a webpage).
        /// </summary>
        public List<PageRank> PageRankList { get; private set; }

        /// <summary>
        /// Graph object
        /// </summary>
        public IGraph Graph { get; private set; }

        /// <summary>
        /// The damper (set to same as Google's value?!)
        /// Prevents the PageRank calculation to go into an endless spiral!
        /// Also this takes into account the random surfer principle where a person gets bored and stops surfing...
        /// </summary>
        private double m_Damper = 0.85;

        /// <summary>
        /// Should be less than 1
        /// </summary>
        public double TotalScore { get; private set; }

        /// <summary>
        /// Holds information about how often the PageRank algorithm has run (after initial value has been set!)
        /// </summary>
        public int IterationCount { get; private set; }

        /// <summary>
        /// Constructor: Creates a new PageRanker with only required parameter being a Graph object.
        /// </summary>
        /// <param name="_graph">A directed graph</param>
        /// <param name="_damper">A value between 0 and 1</param>
        public PageRanker(IGraph _graph, double _damper = 0.85)
        {
            this.PageRankList = new List<PageRank>();
            this.Graph = _graph;
            this.m_Damper = _damper;
        }

        /// <summary>
        /// Perform the calculation (parameterless)
        /// </summary>
        public void Calculate()
        {
            Calculate(this.Graph, this.m_Damper);
        }

        /// <summary>
        /// Perform the calculation (with parameters)
        /// </summary>
        /// <param name="_graph">A graph with atleast 2 vertexes and 1 edge</param>
        /// <param name="_damping">Damping factor between 0 and 1</param>
        public void Calculate(IGraph _graph, double _damping = 0.85)
        {
            CalculatePageRank(_graph, _damping);
        }

        /// <summary>
        /// Do the iterative calculation of the graph's pages PageRank score.
        /// Hide this from the consumer of this class.
        /// </summary>
        /// <param name="_graph">A directed graph object</param>
        /// <param name="_damping">A value between 0 and 1</param>
        private void CalculatePageRank(IGraph _graph, double _damping = 0.85)
        {
            // Make sure it's within the bounds: 0 <= d <= 1
            if (_damping < 0 || _damping > 1)
            {
                _damping = m_Damper;
            }
            // Create initial page rankings
            foreach (IEdge<WebPageInfo> edge in _graph.Edges)
            {
                // We only need to process any URL once...
                // Therefore if only add a URL to the PageRankList if it's not in the list!
                if (!this.PageRankList.Any<PageRank>((PageRank prs) => Helper.IsUriSame(prs.Url, edge.Source.Url)))
                {
                    this.PageRankList.Add(edge.Source);
                }
                // Same with target URL's
                if (!this.PageRankList.Any<PageRank>((PageRank prt) => Helper.IsUriSame(prt.Url, edge.Target.Url)))
                {
                    this.PageRankList.Add(edge.Target);
                }
            }
            double tempTotal = 0.0;
            Boolean isDone = false;
            // Iterate until isDone is satisfied (TotalScore >= 1.0)
            while (!isDone)
            {
                for (int i = 0; i < this.PageRankList.Count; i++)
                {
                    PageRank pr = this.PageRankList[i];
                    Debug.Assert(!Double.IsNaN(pr.Score)); // Make sure it's a number
                    pr.Score = CalculatePageRankScore(pr); // Update the PageRank Score
                    tempTotal += pr.Score;// TotalScore;
                }
                isDone = tempTotal >= 1.0; // Done yet?
                if (isDone)
                { 
                    // If done, then save the temporary total score in the global instance variable
                    TotalScore = tempTotal;
                }
                tempTotal = 0; // Reset temporary total score for this iteration!
                IterationCount++; // Increment by 1 after each iteration
            }
        }

        /// <summary>
        /// Calculate a PageRank object's PageRank Score using the formula provided by Google's founders Larry Page and Sergey Brin.
        /// This algorithm corresponds with the algorithm originally created by Massimo Marchiori and also is defined as a Markov Chain.
        /// </summary>
        /// <param name="pr">PageRank object</param>
        /// <returns></returns>
        private double CalculatePageRankScore(PageRank pr)
        {
            double left = (1 - this.m_Damper);
            int outgoingLinkAmount = pr.OutgoingLinkCount;
            if (outgoingLinkAmount > 0)
                return left + this.m_Damper * (pr.Score / outgoingLinkAmount);
            return left;
        }
    }
}