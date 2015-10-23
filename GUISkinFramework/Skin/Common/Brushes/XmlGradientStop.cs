using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Xml.Serialization;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "GradientStop")]
    public class XmlGradientStop : INotifyPropertyChanged
    {
        private string _color;
        private double _offset;

        public XmlGradientStop()
        {
        }

        public XmlGradientStop(GradientStop gradientStop)
        {
            _color = gradientStop.Color.ToString();
            _offset = gradientStop.Offset;
        }

        [XmlAttribute]
        public string Color
        {
            get { return _color; }
            set { _color = value; NotifyPropertyChanged("Color"); }
        }

        [XmlAttribute]
        public double Offset
        {
            get { return Math.Round( _offset,3); }
            set { _offset = value; NotifyPropertyChanged("Offset"); }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
