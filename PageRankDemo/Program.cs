using bendot;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace demo.bendot
{
    class Program
    {
        private PageRanker PageRank { get; set; }
        private static Program m_Instance = new Program();
        public static Program Instance { get { return m_Instance; } }

        static void Main(string[] args)
        {
            Console.Write("Enter a url to crawl: ");
            String urlString = Console.ReadLine();
            if (urlString.StartsWith("www.") || !urlString.StartsWith("http://"))
            {
                urlString = "http://" + urlString;
            }
            //Console.WriteLine(WebPageInfo.GetPageTitleFromURL(urlString));
            //Console.ReadLine();

            Uri url;
            if (!Uri.TryCreate(urlString, UriKind.Absolute, out url))
            {
                Console.WriteLine("Invalid URL!");
                goto EndProgram;
            }
            var spider = new Spider(new SpiderData()
            {
                StartUrl = url,
                UrlType = UriKind.RelativeOrAbsolute,
                OnlySameDomain = true,
                MaxDepth = 3,
                AllowedContentTypes = new string[] { MediaTypeNames.Text.Html, MediaTypeNames.Text.Plain, MediaTypeNames.Text.Xml, MediaTypeNames.Application.Pdf },
                IsDebugMode = false
            });
            //spider.OnWebResponseEvent += (o, e) =>
            //{
            //    if (e.Response == null)
            //    {
            //        Console.WriteLine("0 bytes received from " + e.Url.AbsoluteUri);
            //    }
            //    else
            //    {
            //        Console.WriteLine(e.Response.ContentLength + " bytes received from " + e.Url.AbsoluteUri);
            //    }
            //};
            //spider.OnAddEdgeEvent += (o, e) =>
            //{
            //    Console.WriteLine("Added " + e.Edge.Target.Url.AbsoluteUri);
            //};
            Graph graph = spider.Crawl();
            Instance.PageRank = new PageRanker(graph);
            if (Instance.PageRank.Graph.Edges.Count() == 0)
            {
                Console.WriteLine("No Graph data!");
            }
            else
            {
                IList<IEdge<WebPageInfo>> edges = Instance.PageRank.Graph.Edges.ToList();
                StringBuilder sitemapData = new StringBuilder();
                sitemapData.AppendLine(url + " - " + edges.Count() + " links.");
                for (int i = 0; i < edges.Count(); i++)
                {
                    var edge = edges[i];
                    sitemapData.AppendLine((" " + (i + 1)).PadLeft(3) + ": " + edge.Target.Url.AbsoluteUri);
                }
                sitemapData.AppendLine("");
                foreach (Edge<WebPageInfo> edge in edges)
                {
                    //sitemapData.AppendLine("Source:");
                    //sitemapData.AppendLine(" > URL:\t\t" + edge.Source.Url.AbsoluteUri);
                    //sitemapData.AppendLine(" > Title:\t" + edge.Source.PageTitle);
                    //sitemapData.AppendLine("Target:");
                    //sitemapData.AppendLine(" > URL:\t\t" + edge.Target.Url.AbsoluteUri);
                    //sitemapData.AppendLine(" > Title:\t" + edge.Target.PageTitle);
                    //sitemapData.AppendLine(" > Outgoing links:" + edge.Target.OutgoingLinks.Count);
                    sitemapData.AppendLine(edge.Target.Url.AbsoluteUri + " - " + edge.Target.OutgoingLinks.Count + " links.");

                    for (int i = 0; i < edge.Target.OutgoingLinks.Count; i++)
                    {
                        var item = edge.Target.OutgoingLinks[i];
                        sitemapData.AppendLine((" " + (i + 1)).PadLeft(3) + ": " + item.AbsoluteUri);
                    }
                    sitemapData.AppendLine("");
                }
                Instance.PageRank.Calculate(); //Calculate PageRank
                var pageRankData = new StringBuilder();
                foreach (WebPageInfo pr in Instance.PageRank.PageRankList)
                {
                    pageRankData.AppendLine(pr.Url.AbsoluteUri + ". PageRank: " + pr.Score);
                    //PageRanker.TotalScore += pr.Score;
                }
                pageRankData.AppendLine("PageRank Total: " + Instance.PageRank.TotalScore);

                Console.WriteLine(sitemapData.ToString());
                Console.WriteLine(pageRankData.ToString());
                File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "sitemap_" + DateTime.Now.Ticks + ".txt"), sitemapData.ToString());
                File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "pagerank_" + DateTime.Now.Ticks + ".txt"), pageRankData.ToString());

            }
        EndProgram:
            Console.WriteLine("Press <any> key to exit...");
            Console.ReadLine();
        }
    }
}