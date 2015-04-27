using bendot;
using Microsoft.Win32;
using PageRankWpfGui.Classes;
using PageRankWpfGui.Properties;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PageRankWpfGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties
        private Spider spider = null;
        private ObservableCollection<GraphContainerDisplayInfo> gdList = new ObservableCollection<GraphContainerDisplayInfo>();
        private List<PageRankContainerDisplayInfo> prList = new List<PageRankContainerDisplayInfo>();
        private Graph graph = new Graph();
        private WebGraphView1 webgraphView = new WebGraphView1();
        private PageRanker PageRanker;
        private String newContentType = null;
        private String[] allowedContentTypes = new String[Settings.Default.AllowedContentTypes.Count];
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
        }

        #region Methods
        private void LoadSettings()
        {
            Settings.Default.Reload();
            this.chkOnlySameDomain.IsChecked = Settings.Default.OnlySameDomain;
            this.ChkOnlyAbsolutePath.IsChecked = Settings.Default.OnlyAbsolutePath;
            this.TxtDampingFacor.Text = Settings.Default.DampingFacor.ToString();
            this.CmbAllowedContentTypes.ItemsSource = Settings.Default.AllowedContentTypes;
            if (Settings.Default.AllowedContentTypes != null && Settings.Default.AllowedContentTypes.Count > 0)
            {
                this.CmbAllowedContentTypes.SelectedIndex = 0;
                allowedContentTypes = new String[Settings.Default.AllowedContentTypes.Count];
                Settings.Default.AllowedContentTypes.CopyTo(allowedContentTypes, 0);
            }
            if (CheckUri(Settings.Default.StartUrl))
            {
                this.TxtStartUrl.Text = Settings.Default.StartUrl;
            }
        }

        private Boolean CheckUri(String _urlOrFile)
        {
            Boolean isValid = false;
            Uri u;
            if (!Uri.TryCreate(_urlOrFile, UriKind.Absolute, out u) && !File.Exists(_urlOrFile))
            {
                String file = Path.Combine(Environment.CurrentDirectory, @"Webdata\a\index.html");
                if (File.Exists(file))
                {
                    _urlOrFile = file;
                }
                else
                {
                    _urlOrFile = "http://www.hammerbenjamin.com/pageranktest/1/a.html";
                }
                isValid = false;
            }
            else
            {
                isValid = true;
            }
            return isValid;
        }

        private void CalcPageRank()
        {
            TabControl1.SelectedItem = PageRanking;
            this.prList.Clear();
            this.PageRankOutput.ItemsSource = this.prList;
            //this.PageRankOutput.DataContext = this.prList;
            Clear();
            //BuildGraph();
            PageRanker = new PageRanker(this.graph, Settings.Default.DampingFacor); // Create a new page ranker
            PageRanker.Calculate(); //Calculate PageRank
            foreach (WebPageInfo pr in PageRanker.PageRankList)
            {
                prList.Add(new PageRankContainerDisplayInfo()
                {
                    Url = pr.Url.ToString(),
                    Score = pr.Score
                });
            }
            gdList.Clear();
            int c = 0;
            //update the score for the graph
            foreach (Edge<WebPageInfo> edge in this.graph.Edges)
            {
                edge.Source.Score = PageRanker.PageRankList[c].Score;
                gdList.Add(new GraphContainerDisplayInfo()
                {
                    SourcePageTitle = edge.Source.PageTitle,
                    TargetPageTitle = edge.Target.PageTitle,
                    PageRankScore = edge.Source.Score,
                    TargetPageOutgoingLinkCount = edge.Target.OutgoingLinks.Count,
                    TargetUrl = edge.Target.Url.ToString()
                });
                c++;
            }
            WriteLine("PageRank Total: " + PageRanker.TotalScore);
            WriteLine("Iteration Count: " + PageRanker.IterationCount);
        }

        private void CreateGraph()
        {
            TabControl1.SelectedItem = Webgraph;
            this.gdList.Clear();
            this.GraphOutput.DataContext = this.gdList;
            Clear();
            if (this.graph == null)
            {
                WriteLine("No Graph data!");
            }
            else
            {
                WriteLine("The web page " + this.graph.StartUrl + " has " + this.graph.GetEdgeCount() + " links.");
                WriteLine("Total webpages (vertices/nodes): " + this.graph.GetVertexCount() + " & Links (edges): " + this.graph.GetEdgeCount());
                foreach (Edge<WebPageInfo> edge in this.graph.Edges)
                {
                    gdList.Add(new GraphContainerDisplayInfo()
                    {
                        SourcePageTitle = edge.Source.PageTitle,
                        TargetPageTitle = edge.Target.PageTitle,
                        PageRankScore = edge.Target.Score,
                        TargetPageOutgoingLinkCount = edge.Target.OutgoingLinks.Count,
                        TargetUrl = edge.Target.Url.ToString()
                    });
                }

                //PageRanker.Calculate(); //Calculate PageRank
                //foreach (WebPageInfo pr in PageRanker.PageRankList)
                //{
                //    gdList.
                //    pr.
                //    //Console.WriteLine(pr.Url.AbsoluteUri + " has a PageRank score of " + pr.Score);
                //    //PageRanker.TotalScore += pr.Score;
                //}

                webgraphView.DrawGraph(this.graph);
                Graph1.LayoutAlgorithmType = webgraphView.LayoutAlgorithmType;
                Graph1.OverlapRemovalAlgorithmType = "FSA";
                Graph1.HighlightAlgorithmType = "Simple";
                Graph1.Graph = webgraphView.Graph;
            }
        }

        private void BuildGraph()
        {
            TabControl1.SelectedItem = Webgraph;
            this.gdList.Clear();
            this.GraphOutput.DataContext = this.gdList;
            Clear();

            Uri url = null;
            if (!Uri.TryCreate(this.TxtStartUrl.Text, UriKind.Absolute, out url))
            {
                Clear();
                WriteLine("Invalid URL!");
                return;
            }
            //Settings.Default.AllowedContentTypes.CopyTo(allowedContentTypes, 0);
            spider = new Spider(new SpiderData()
            {
                StartUrl = url,
                UrlType = Settings.Default.OnlyAbsolutePath ? UriKind.Absolute : UriKind.RelativeOrAbsolute,
                OnlySameDomain = Settings.Default.OnlySameDomain,
                MaxDepth = 3,
                AllowedContentTypes = allowedContentTypes, //new string[] { MediaTypeNames.Text.Html, MediaTypeNames.Text.Plain, MediaTypeNames.Text.Xml, MediaTypeNames.Application.Pdf },
                IsDebugMode = false
            });
            spider.OnWebResponseEvent += (o, e) =>
            {
                if (e.Response == null)
                {
                    Console.WriteLine("0 bytes received from " + e.Url.AbsoluteUri);
                }
                else
                {
                    Console.WriteLine(e.Response.ContentLength + " bytes received from " + e.Url.AbsoluteUri);
                }
            };
            spider.OnAddEdgeEvent += (o, e) =>
            {
                Console.WriteLine("Added " + e.Edge.Target.Url.AbsoluteUri);
                var edge = e.Edge;
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    this.gdList.Add(new GraphContainerDisplayInfo()
                    {
                        SourcePageTitle = edge.Source.PageTitle,
                        TargetPageTitle = edge.Target.PageTitle,
                        PageRankScore = edge.Target.Score,
                        TargetPageOutgoingLinkCount = edge.Target.OutgoingLinks.Count,
                        TargetUrl = edge.Target.Url.ToString(),
                        ContentType = edge.Target.ContentType
                    });
                });
                //GraphOutput.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(delegate
                //{
                //    DataContext = this.gdList;
                //}));
            };
            var t = new Thread(new ThreadStart(Crawl));
            t.IsBackground = false;
            t.Start();
        }

        void Crawl()
        {
            this.graph = spider.Crawl();
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                if (this.graph != null)
                {
                    WriteLine("The web page " + this.graph.StartUrl + " has " + this.graph.GetEdgeCount() + " links.");
                    WriteLine("Total webpages (vertices/nodes): " + this.graph.GetVertexCount() + " & Links (edges): " + this.graph.GetEdgeCount());

                    webgraphView.DrawGraph(this.graph);
                    Graph1.LayoutAlgorithmType = webgraphView.LayoutAlgorithmType;
                    Graph1.OverlapRemovalAlgorithmType = "FSA";
                    Graph1.HighlightAlgorithmType = "Simple";
                    Graph1.Graph = webgraphView.Graph;
                }
                else
                {
                    WriteLine("Something went wrong. Graph is null!");
                }
            });
        }

        private void Clear()
        {
            TxtOutput.Text = "";
        }

        private void Write(object data)
        {
            TxtOutput.AppendText("" + data);
        }

        private void WriteLine(object data)
        {
            TxtOutput.AppendText(data + Environment.NewLine);
        }
        #endregion

        #region EventHandlers
        private void TxtStartUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                BuildGraph();
                CreateGraph();
            }
        }

        private void BtnCrawl_Click(object sender, RoutedEventArgs e)
        {
            BuildGraph();
            //CreateGraph();
        }

        private void BtnPageRank_Click(object sender, RoutedEventArgs e)
        {
            CalcPageRank();
        }

        private void TxtStartUrl_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void DampingFacor_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            Double d = 0.85;
            String dStr = this.TxtDampingFacor.Text;
            if (Double.TryParse(dStr, out d))
            {
                if (d < 0 || d > 1)
                {
                    d = 0.85;
                }
                Settings.Default.DampingFacor = d;
            }
        }

        private void SaveSettings_Click_1(object sender, RoutedEventArgs e)
        {
            Settings.Default.OnlySameDomain = this.chkOnlySameDomain.IsChecked.Value;
            Settings.Default.OnlyAbsolutePath = this.ChkOnlyAbsolutePath.IsChecked.Value;
            Settings.Default.Save();
            LoadSettings();
        }

        private void BtnAddAllowedContentType_Click_1(object sender, RoutedEventArgs e)
        {
            this.TxtNewAllowedContentType.Visibility = System.Windows.Visibility.Visible;
            this.CmbAllowedContentTypes.Visibility = System.Windows.Visibility.Hidden;
            this.BtnCreateAllowedContentType.IsEnabled = true;
            this.BtnAddAllowedContentType.IsEnabled = false;
            this.BtnRemoveAllowedContentType.IsEnabled = false;
        }

        private void BtnRemoveAllowedContentType_Click_1(object sender, RoutedEventArgs e)
        {
            String contentTypeToRemove = this.CmbAllowedContentTypes.SelectedValue as String;
            Settings.Default.AllowedContentTypes.Remove(contentTypeToRemove);
            Settings.Default.Save();
            LoadSettings();
        }

        private void TxtNewAllowedContentType_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {
                newContentType = this.TxtNewAllowedContentType.Text;
            }
            catch (Exception ex)
            {

            }
        }

        private void BtnCreateAllowedContentType_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.newContentType != null && this.newContentType.Length > 0 && !Settings.Default.AllowedContentTypes.Contains(this.newContentType))
            {
                Settings.Default.AllowedContentTypes.Add(this.newContentType);
                Settings.Default.Save();
                this.TxtNewAllowedContentType.Text = "";
                this.TxtNewAllowedContentType.Visibility = System.Windows.Visibility.Hidden;
                this.CmbAllowedContentTypes.Visibility = System.Windows.Visibility.Visible;
                this.BtnCreateAllowedContentType.IsEnabled = false;
                this.BtnAddAllowedContentType.IsEnabled = true;
                this.BtnRemoveAllowedContentType.IsEnabled = true;
                LoadSettings();
            }
        }
        #endregion

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.CurrentDirectory;
            ofd.Filter = "Web documents(*.htm;*.html)|*.htm;*html|All files (*.*)|*.*";
            ofd.RestoreDirectory = true;
            ofd.Multiselect = false;
            ofd.ShowDialog();
            this.TxtStartUrl.Text = ofd.FileName;
        }

        private void BtnSetStartUrl_Click_1(object sender, RoutedEventArgs e)
        {
            String newStartUrl = this.TxtCurrentStartUrl.Text;
            if (CheckUri(newStartUrl))
            {
                this.TxtCurrentStartUrl.Text = newStartUrl;
                Settings.Default.StartUrl = newStartUrl;
                Settings.Default.Save();
            }
            else
            {
                this.TxtCurrentStartUrl.Text = Settings.Default.StartUrl;
            }
        }
    }
}