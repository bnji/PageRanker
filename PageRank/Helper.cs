
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bendot
{
    /// <summary>
    /// 
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// Check if two URL's are identical. Order doesn't matter.
        /// </summary>
        /// <param name="url1">First URL</param>
        /// <param name="url2">Second URL</param>
        /// <returns>TRUE if they are identical</returns>
        public static Boolean IsUriSame(Uri url1, Uri url2)
        {
            if (url1.IsAbsoluteUri)
                return url1.MakeRelativeUri(url2).ToString().Length == 0;
            if (url2.IsAbsoluteUri)
                return url2.MakeRelativeUri(url1).ToString().Length == 0;
            return false;
        }
    }
}
