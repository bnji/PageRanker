using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageRankWpfGui.Classes
{
    class GraphContainerDisplayInfo : INotifyPropertyChanged
    {
        private String m_SourcePageTitle;
        private String m_TargetUrl;
        private String m_TargetPageTitle;
        private int m_TargetPageOutgoingLinkCount;
        private double m_PageRankScore;
        private String m_ContentType;

        public String SourcePageTitle
        {
            get { return m_SourcePageTitle; }
            set { m_SourcePageTitle = value; OnPropertyChanged("SourcePageTitle"); }
        }
        public String TargetUrl
        {
            get { return m_TargetUrl; }
            set { m_TargetUrl = value; OnPropertyChanged("TargetUrl"); }
        }
        public String TargetPageTitle
        {
            get { return m_TargetPageTitle; }
            set { m_TargetPageTitle = value; OnPropertyChanged("TargetPageTitle"); }
        }
        public Int32 TargetPageOutgoingLinkCount
        {
            get { return m_TargetPageOutgoingLinkCount; }
            set { m_TargetPageOutgoingLinkCount = value; OnPropertyChanged("TargetPageOutgoingLinkCount"); }
        }
        public Double PageRankScore
        {
            get { return m_PageRankScore; }
            set { m_PageRankScore = value; OnPropertyChanged("PageRankScore"); }
        }
        public String ContentType
        {
            get { return m_ContentType; }
            set
            {
                m_ContentType = value; 
                OnPropertyChanged("ContentType");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event 
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
