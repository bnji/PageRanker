using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bendot
{
    /// <summary>
    /// Holds a score for some type T.
    /// This acts like a Wrapper class, because it's Page of type T is not a PageRank, but can hava a score and possibly some extra info, 
    /// which does not relate directly to the Page of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageRankOld<T>
    {
        /// <summary>
        /// Holds information about some object, could be a website for example by using the WebPageInfo class as type T.
        /// It should only be possible to set the value of the page when created.
        /// </summary>
        public T Page { get; private set; }
        /// <summary>
        /// The score for the Page of type T.
        /// This value change be changed whenever the user of the class wants to.
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Create a new PageRank
        /// The initial score should not be 0 or 1. A value of 1 - d (damper) seems to be fair, and makes the iterations go down.
        /// </summary>
        /// <param name="_page">Some page which has links pointing to other page/s</param>
        /// <param name="_score">A value between 0 and 1</param>
        public PageRankOld(T _page, double _score = 0.15)
        {
            this.Page = _page;
            this.Score = _score;
        }
    }
}