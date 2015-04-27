using QuickGraph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bendot
{
    public delegate void OnAddEdgeDelegate(object sender, EdgeEventArgs e);
    public delegate void OnWebResponseDelegate(object sender, WebResponseEventArgs e);

    public class EdgeEventArgs : EventArgs
    {
        public IEdge<WebPageInfo> Edge { get; set; }
    }

    public class WebResponseEventArgs : EventArgs
    {
        public Uri Url { get; set; }

        public HttpWebResponse Response { get; set; }
    }

    /// <summary>
    /// A very simple and buggy website crawler
    /// </summary>
    public class Spider
    {
        public event OnAddEdgeDelegate OnAddEdgeEvent;
        public event OnWebResponseDelegate OnWebResponseEvent;

        public SpiderData Data { get; private set; }
        public List<string> HostsProcessed { get; set; }

        public Spider(SpiderData data)
        {
            HostsProcessed = new List<string>();
            Data = data;
        }

        /// <summary>
        /// Crawl a website. Give it an initial startingpoint (it's center) and let it do its magic.
        /// This implementation is called Depth-First Traversal, which means it goes all the way down
        /// the link chain (so to speak) before it visits it's neighbours.
        /// Page. 768 Datastructures
        /// </summary>
        /// <param name="Data.StartUrl">URL (link/website) starting point</param>
        /// <param name="Data.UrlType">Crawl only absolute URI, relative URI or both?</param>
        /// <param name="Data.OnlySameDomain">If not, then crawl the entire web or crash the computer... hmmm</param>
        /// <returns></returns>
        public Graph Crawl()
        {
            String startDomain = Data.StartUrl.Host; // e.g. www.example.com
            Queue<VertexEdgeGroup> traversalOrder = new Queue<VertexEdgeGroup>();
            Stack<WebPageInfo> vertexStack = new Stack<WebPageInfo>();
            WebPageInfo originVertex = GetWebPageInfo(Data.StartUrl);
            if (originVertex == null)
            {
                return null;
            }
            traversalOrder.Enqueue(new VertexEdgeGroup(originVertex));
            vertexStack.Push(originVertex);
            Stack<Uri> tempUrlList;
            List<string> processedUrls = new List<string>();

            if (Data.AllowedContentTypes == null)
            {
                Data.AllowedContentTypes = new String[] { MediaTypeNames.Text.Html, MediaTypeNames.Text.Plain, MediaTypeNames.Text.Xml };
            }

            while (vertexStack.Count > 0)
            {
                WebPageInfo topVertex = vertexStack.Pop();
                tempUrlList = new Stack<Uri>(topVertex.OutgoingLinks);
                if (Data.IsDebugMode)
                {
                    Console.WriteLine("Stack size: " + vertexStack.Count);
                    Console.WriteLine("topVertex URL: " + topVertex.Url);
                }
                while (tempUrlList.Count > 0) // has neighbour
                {
                    Uri url = tempUrlList.Peek();

                    Uri urlRelative = topVertex.Url.MakeRelativeUri(url);
                    var currentHost = url.Host;
                    Boolean isSameDomain = currentHost.Equals(startDomain);
                    var pq = urlRelative.OriginalString;
                    //if (pq.StartsWith("#"))
                    //{
                    //    processedUrls.Add(url);
                    //}

                    WebPageInfo nextNeighbor = null;
                    // Process if it's not a file
                    if (!topVertex.IsLocalFile)
                    {
                        nextNeighbor = GetWebPageInfo(url);
                        if (nextNeighbor == null)
                        {
                            goto PopTempUrl;
                        }
                    }


                    if (!Data.AllowedContentTypes.Any(w => w.Contains(nextNeighbor.ContentType)))
                    {
                        goto PopTempUrl;
                    }

                    var isUrlProcessed = processedUrls.Contains(url.AbsoluteUri.Replace("#", ""));// processedUrls.Find(u => Uri.Compare(u, new Uri(url.AbsoluteUri.Replace("#", "")), UriComponents.AbsoluteUri, UriFormat.UriEscaped, StringComparison.InvariantCulture) == 0) != null;
                    if (!isUrlProcessed)// Helper.IsUriSame(u, url))) //next neighbour not visited before
                    //if (!processedUrls.Contains(urlRelative)) //next neighbour not visited before

                    {
                        // Make sure that we only process url's from the same domain when Data.OnlySameDomain = true
                        if (Data.OnlySameDomain && !isSameDomain)
                        {
                            goto PopTempUrl;
                        }
                        if (!HostsProcessed.Contains(currentHost))
                        {
                            HostsProcessed.Add(currentHost);
                        }
                        if (HostsProcessed.Count >= Data.MaxDepth)
                        {
                            goto PopTempUrl;
                        }
                        if (Data.IsDebugMode)
                        {
                            //Console.WriteLine("" + topVertex.PageTitle + " --> " + url);
                            Console.WriteLine(vertexStack.Count + ", " + tempUrlList.Count + ", " + topVertex.Url + " > " + url);
                        }
                        var edge = new Edge<WebPageInfo>(topVertex, nextNeighbor);
                        if (OnAddEdgeEvent != null)
                        {
                            OnAddEdgeEvent(this, new EdgeEventArgs()
                            {
                                Edge = edge
                            });
                        }
                        processedUrls.Add(url.AbsoluteUri.Replace("#", ""));
                        //processedUrls.Add(urlRelative);
                        traversalOrder.Enqueue(new VertexEdgeGroup(nextNeighbor, edge));
                        vertexStack.Push(nextNeighbor);
                    }
                    goto PopTempUrl;
                //}
                //else
                //{
                //    Console.Write("Don't process url " + url + ", because it's not html or plain text document, but a " + ContentType);
                //    goto PopTempUrl;
                //}
                PopTempUrl:
                    tempUrlList.Pop();
                }
                //vertexStack.Pop();
            }
            //Create and return the graph
            Graph newGraph = new Graph(Data.StartUrl);
            while (traversalOrder.Count > 0)
            {
                VertexEdgeGroup group = traversalOrder.Dequeue();
                WebPageInfo v = group.Vertex;
                IEdge<WebPageInfo> e = group.Edge;
                newGraph.AddVertex(v);
                if (e != null)
                {
                    newGraph.AddEdge(e);
                }
            }
            return newGraph;
        }

        WebPageInfo GetWebPageInfo(Uri url, bool loadContent = true)
        {
            WebPageInfo result = null;
            using (var response = GetWebResponse(url, loadContent))
            {
                OnWebResponseEvent(this, new WebResponseEventArgs()
                {
                    Url = url,
                    Response = response
                });
                if (response != null && response.ContentLength > 0)
                {
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        result = new WebPageInfo(url, Data.UrlType, sr.ReadToEnd(), response.ContentType);
                        sr.Close();
                        response.Close();
                    }
                }
            }
            return result;
        }

        private static HttpWebResponse GetWebResponse(Uri _url, bool loadContent = true)
        {
            HttpWebResponse response = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(_url);
                request.UserAgent = "PageRankDemo/0.1 (Windows NT 6.2; WOW64) AppleKit .NET CLR 4; .NET CLR 4.0)";
                //Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/24.0.1312.60 Safari/537.17
                //request.AllowAutoRedirect = false;
                //request.MaximumAutomaticRedirections = 1;
                request.ReadWriteTimeout = 3000;
                request.Timeout = 3000;
                request.Method = WebRequestMethods.Http.Head;
                if (loadContent)
                {
                    request.Method = WebRequestMethods.Http.Get;
                }
                //request.Proxy = null;
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {

            }
            return response;
        }
    }
}