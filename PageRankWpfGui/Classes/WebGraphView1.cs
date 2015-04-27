using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using GraphSharp.Controls;
using PageRankWpfGui.Classes;
using bendot;
using QuickGraph;

namespace PageRankWpfGui.Classes
{
    public class WebGraphView1 : INotifyPropertyChanged
    {
        #region Private Members
        private string layoutAlgorithmType;
        private WebPageGraph m_Graph;
        private List<String> layoutAlgorithmTypes = new List<string>();
        #endregion

        #region Contstructor
        public WebGraphView1()
        {
            //Add Layout Algorithm Types
            layoutAlgorithmTypes.Add("BoundedFR");
            layoutAlgorithmTypes.Add("Circular");
            layoutAlgorithmTypes.Add("CompoundFDP");
            layoutAlgorithmTypes.Add("EfficientSugiyama");
            layoutAlgorithmTypes.Add("FR");
            layoutAlgorithmTypes.Add("ISOM");
            layoutAlgorithmTypes.Add("KK");
            layoutAlgorithmTypes.Add("LinLog");
            layoutAlgorithmTypes.Add("Tree");

            //Pick a default Layout Algorithm Type
            LayoutAlgorithmType = layoutAlgorithmTypes[1];
        }
        #endregion



        /// <summary>
        /// http://sachabarbs.wordpress.com/2010/08/31/pretty-cool-graphs-in-wpf/
        /// </summary>
        /// <param name="g">A graph object</param>
        public void DrawGraph(Graph g)
        {
            m_Graph = new WebPageGraph(true);
            List<WebPageInfoVertex> existingVertices = new List<WebPageInfoVertex>();

            foreach (WebPageInfo wpi in g.GraphData.Vertices)
            {
                existingVertices.Add(new WebPageInfoVertex(wpi.PageTitle, wpi));
            }
            foreach (WebPageInfo v in g.GraphData.Vertices)
            {
                Graph.AddVertex(new WebPageInfoVertex(v.PageTitle, v));
               // System.Windows.MessageBox.Show("Added Vertex: " + v.WebPageInfo.PageTitle);
            }

            /*foreach (Edge<WebPageInfo> e in g.GraphData.Edges)
            {
                try
                {
                    //graph.AddEdge(new WebPageInfoEdge(e.Source.PageTitle, new WebPageInfoVertex("", e.Source), new WebPageInfoVertex("", e.Target)));
                    AddNewGraphEdge(new WebPageInfoVertex(e.Source.PageTitle, e.Source), new WebPageInfoVertex(e.Target.PageTitle, e.Target));
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(e.Source.PageTitle + " - " + e.Target.PageTitle);
                    System.Windows.MessageBox.Show("" + ex);
                }
                //AddNewGraphEdge(new WebPageInfoVertex(e.Target.PageTitle, e.Target), new WebPageInfoVertex(e.Source.PageTitle, e.Source));
            }*/
            
            for (int i = 0; i < existingVertices.Count - 1; i++)
            {
                try
                {
                    AddNewGraphEdge(existingVertices[i], existingVertices[i+1]);
                }
                catch (Exception ex)
                {
                    //System.Windows.MessageBox.Show(""+ex);
                }
            }

            //Pick a default Layout Algorithm Type
            LayoutAlgorithmType = LayoutAlgorithmTypes[1];
        }

        #region Private Methods
        private WebPageInfoEdge AddNewGraphEdge(WebPageInfoVertex from, WebPageInfoVertex to)
        {
            string edgeString = string.Format("{0}-{1} Connected", from.ID, to.ID);
            WebPageInfoEdge newEdge = new WebPageInfoEdge(edgeString, from, to);
            Graph.AddEdge(newEdge);
            return newEdge;
        }

        #endregion

        #region Public Properties

        public List<String> LayoutAlgorithmTypes
        {
            get { return layoutAlgorithmTypes; }
        }

        public string LayoutAlgorithmType
        {
            get { return layoutAlgorithmType; }
            set
            {
                layoutAlgorithmType = value;
                NotifyPropertyChanged("LayoutAlgorithmType");
            }
        }

        public WebPageGraph Graph
        {
            get { return m_Graph; }
            set
            {
                m_Graph = value;
                NotifyPropertyChanged("Graph");
            }
        }
        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }
}
