using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace bendot
{
    /// <summary>
    /// WebPageInfo class
    /// Inherits/Extends the abstract PageRank class.
    /// Contains information about a Webpage.
    /// </summary>
    public class WebPageInfo : PageRank
    {
        public string Content { get; private set; }
        public String ContentType { get; private set; }
        private String m_baseUrl = null;
        private UriKind m_UrlType = UriKind.Absolute;
        private String m_PageTitle = "-1";
        public String PageTitle { get { return m_PageTitle; } private set { m_PageTitle = value; } }
        private HtmlDocument m_HtmlDoc = new HtmlDocument();
        public HtmlDocument HtmlDoc { get { return m_HtmlDoc; } private set { m_HtmlDoc = value; } }
        public List<Uri> OutgoingLinks { get; private set; }
        private HtmlWeb htmlWeb;
        public HttpStatusCode StatusCode { get; private set; }
        private Boolean m_IsLocalFile;

        private readonly byte retryDownloadMax = 3;
        private byte retryDownloadCount = 0;

        public Boolean IsLocalFile
        {
            get { return m_IsLocalFile; }
            set { m_IsLocalFile = value; }
        }

        public static string GetPageTitleFromURL(string _url)
        {
            return new HtmlWeb().Load(_url).DocumentNode.SelectSingleNode("//head//title").InnerHtml;
        }

        public WebPageInfo(Uri _url, UriKind _urlType, String _content, string _contentType)
        {
            this.PageTitle = "-1";
            this.Score = 0;
            this.Url = _url;
            this.m_UrlType = _urlType;
            this.m_baseUrl = this.Url.GetLeftPart(UriPartial.Path);
            this.htmlWeb = new HtmlWeb();
            this.OutgoingLinks = new List<Uri>();
            this.ContentType = _contentType;
            if (this.ContentType == MediaTypeNames.Text.Html)
            {
                DownloadHtmlContent3(_content);
                PopulateOutgoingLinks();
            }
            else
            {
                try
                {
                    this.PageTitle = Path.GetFileName(this.Url.AbsoluteUri);
                }
                catch { }
            }
        }

        private void DownloadHtmlContent3(String _content)
        {
            HtmlDoc.LoadHtml(_content);
            HtmlNode node = HtmlDoc.DocumentNode.SelectSingleNode("//head//title");
            PageTitle = (node != null ? node.InnerText : HtmlDoc.DocumentNode.InnerText).Trim();
        }

        private void DownloadHtmlContent2(String _url)
        {
            HtmlDoc = htmlWeb.Load(_url);
            if (htmlWeb.StatusCode == HttpStatusCode.OK)
            {
                HtmlNode node = HtmlDoc.DocumentNode.SelectSingleNode("//head//title");
                PageTitle = (node != null ? node.InnerText : HtmlDoc.DocumentNode.InnerText).Trim();
            }
            if (retryDownloadCount < retryDownloadMax)
            {
                if (StatusCode == HttpStatusCode.MovedPermanently)
                {
                    DownloadHtmlContent2(htmlWeb.ResponseUri.AbsoluteUri);
                }
            }
            else
            {
                retryDownloadCount = 0;
            }
            retryDownloadCount++;            
        }

        private Boolean DownloadHtmlContent(String _url)
        {
            Boolean isSuccess = false;
            try
            {

                String fileName = "";
                String fileUri = "file:///";
                if (_url.StartsWith(fileUri))
                {
                    fileName = _url.Substring(fileUri.Length, _url.Length - fileUri.Length);
                }
                if (System.IO.File.Exists(fileName))
                {
                    m_IsLocalFile = true;
                    m_HtmlDoc.OptionFixNestedTags = true;
                    m_HtmlDoc.Load(fileName);
                    StatusCode = HttpStatusCode.OK; // Fake a successfull HTTP Status Code :) - we could use goto statement also..
                }
                else
                {
                    m_IsLocalFile = false;
                    m_HtmlDoc = htmlWeb.Load(_url);
                    StatusCode = htmlWeb.StatusCode;
                }
                if (StatusCode == HttpStatusCode.OK)
                {
                    isSuccess = true;
                    try
                    {
                        HtmlNode node = HtmlDoc.DocumentNode.SelectSingleNode("//head//title");
                        m_PageTitle = node != null ? node.InnerText : HtmlDoc.DocumentNode.InnerText;
                        m_PageTitle = m_PageTitle.Trim();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                if (StatusCode == HttpStatusCode.MovedPermanently)
                {
                    DownloadHtmlContent(htmlWeb.ResponseUri.AbsoluteUri);
                }
            }
            catch (UriFormatException ex)
            {

            }
            catch (WebException ex)
            {

            }
            catch (HtmlWebException ex)
            {

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return isSuccess;
        }

        private Boolean PopulateOutgoingLinks()
        {
            Boolean isSuccess = false;
            if (m_HtmlDoc == null || m_HtmlDoc.DocumentNode == null)
                return isSuccess;

            HtmlNodeCollection nodes = m_HtmlDoc.DocumentNode.SelectNodes("//a[@href]");
            if (nodes != null)
            {
                foreach (HtmlNode link in nodes)
                {
                    if (link == null)
                        break;
                    isSuccess = true;
                    Uri newUrl = null;

                    Uri.TryCreate(link.Attributes["href"].Value, m_UrlType, out newUrl);
                    if (newUrl != null)
                    {
                        if (!newUrl.IsAbsoluteUri)
                        {
                            Uri.TryCreate(new Uri(this.m_baseUrl), newUrl.ToString(), out newUrl);
                        }
                        //System.Windows.Forms.MessageBox.Show(""+newUrl);
                        this.OutgoingLinks.Add(newUrl);
                    }
                }
                this.OutgoingLinkCount = this.OutgoingLinks.Count;
            }
            return isSuccess;
        }
    }
}