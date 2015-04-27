using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageRankWpfGui.Classes
{
    class PageRankContainerDisplayInfo : INotifyPropertyChanged
    {
        private Double m_Score;

        public Double Score
        {
            get { return m_Score; }
            set { m_Score = value; OnPropertyChanged("Score"); }
        }

        private String m_Url;

        public String Url
        {
            get { return m_Url; }
            set { m_Url = value; OnPropertyChanged("Url"); }
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