using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using Common.Helpers;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "StyleCollection")]
    public class XmlStyleCollection : INotifyPropertyChanged
    {
        private ObservableCollection<XmlBrush> _brushStyles = new ObservableCollection<XmlBrush>();
        private ObservableCollection<XmlControlStyle> _controlStyles = new ObservableCollection<XmlControlStyle>();

        [XmlIgnore]
        public string Name { get; set; }

        [XmlArray(ElementName = "BrushStyles")]
        public ObservableCollection<XmlBrush> BrushStyles
        {
            get { return _brushStyles; }
            set { _brushStyles = value; }
        }

        [XmlArray(ElementName = "ControlStyles")]
        public ObservableCollection<XmlControlStyle> ControlStyles
        {
            get { return _controlStyles; }
            set { _controlStyles = value; }
        }

        public void InitializeStyleCollection()
        {
            foreach (var controlStyle in ControlStyles)
            {
                controlStyle.LoadSubStyles(this);
            }
        }

        public void SaveStyle<T>(T style) where T : XmlStyle
        {
            if (style != null)
            {
                if (style is XmlBrush)
                {
                    BrushStyles.Remove(BrushStyles.FirstOrDefault(s => s.StyleId.Equals(style.StyleId)));
                    BrushStyles.Add(style as XmlBrush);
                }
                else if (style is XmlControlStyle)
                {
                    ControlStyles.Remove(ControlStyles.FirstOrDefault(s => s.StyleId.Equals(style.StyleId)));
                    ControlStyles.Add(style as XmlControlStyle);
                }
            }
        }

        public T GetStyle<T>(T style) where T : XmlStyle
        {
            if (style != null)
            {
                if (!string.IsNullOrEmpty(style.StyleId))
                {
                    if (style is XmlBrush)
                    {
                        return (T)(object)BrushStyles.FirstOrDefault(s => s.StyleId.Equals(style.StyleId));
                    }
                    else if (style is XmlControlStyle)
                    {
                        return (T)(object)ControlStyles.FirstOrDefault(s => s.StyleId.Equals(style.StyleId));
                    }
                }
                return style;
            }
            return default(T);
        }


        public T GetStyle<T>(string styleId) where T : XmlStyle
        {
            if (!string.IsNullOrEmpty(styleId))
            {
                if (typeof(T) == typeof(XmlBrush))
                {
                    return (T)(object)BrushStyles.FirstOrDefault(s => s.StyleId.Equals(styleId));
                }
                else if (typeof(T) == typeof(XmlControlStyle))
                {
                    return (T)(object)ControlStyles.FirstOrDefault(s => s.StyleId.Equals(styleId));
                }
            }
            return default(T);
        }

        public XmlControlStyle GetDesignerStyle(Type controlType)
        {
            if (ControlStyles.Any(s => s.StyleId == controlType.Name))
            {
                var style = ControlStyles.FirstOrDefault(s => s.StyleId == controlType.Name).CreateCopy();
                style.StyleId = string.Empty;
                return style;
            }
            return null;
        }

        public XmlControlStyle GetDesignerStyle(string name)
        {
            if (ControlStyles.Any(s => s.StyleId == name))
            {
                var style = ControlStyles.FirstOrDefault(s => s.StyleId == name).CreateCopy();
                style.StyleId = string.Empty;
                return style;
            }
            return null;
        }

        public XmlBrush GetDesignerBrushStyle(string styleId)
        {
            if (BrushStyles.Any(s => s.StyleId == styleId))
            {
                var brush = BrushStyles.FirstOrDefault(s => s.StyleId == styleId).CreateCopy();
                brush.StyleId = string.Empty;
                return brush;
            }
            return null;
        }


        public bool StyleExists(XmlStyle style)
        {
            if (style != null)
            {
                if (style is XmlBrush)
                {
                    return BrushStyles.Any(s => s.StyleId.Equals(style.StyleId));
                }
                else if (style is XmlControlStyle)
                {
                    return ControlStyles.Any(s => s.StyleId.Equals(style.StyleId));
                }
            }
            return false;
        }


        public T GetControlStyle<T>(T controlStyle) where T : XmlControlStyle
        {
            if (controlStyle != null)
            {
                if (!string.IsNullOrEmpty(controlStyle.StyleId))
                {
                    return (T)(object)ControlStyles.FirstOrDefault(c => c.StyleId.Equals(controlStyle.StyleId))
                                   ?? Activator.CreateInstance<T>();
                }
                controlStyle.LoadSubStyles(this);
            }
            return controlStyle ?? Activator.CreateInstance<T>();
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
