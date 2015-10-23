using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Common.Helpers;
using GUISkinFramework.Skin;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Editors
{
    /// <summary>
    /// Interaction logic for BrushEditor.xaml
    /// </summary>
    public partial class StyleEditor : ITypeEditor, INotifyPropertyChanged
    {
    
        private PropertyItem _item;

        public StyleEditor()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(XmlControlStyle), typeof(StyleEditor),
                                                                                      new FrameworkPropertyMetadata(new XmlControlStyle(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public XmlControlStyle Value
        {
            get { return (XmlControlStyle)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public List<string> Styles 
        {
            get { return new List<string>(new[] { "None" }.Concat(SkinInfo.Style.ControlStyles.Where(s => Value != null && s.GetType() == Value.GetType()).Select(x => x.StyleId))); }
        }

        private XmlSkinInfo _skinInfo = new XmlSkinInfo();

        public XmlSkinInfo SkinInfo
        {
            get { return _skinInfo; }
            set { _skinInfo = value; NotifyPropertyChanged("SkinInfo"); NotifyPropertyChanged("Styles"); }
        }
        


        private string _selectedStyle;
        public string SelectedStyle
        {
            get { return _selectedStyle; }
            set
            {
                _selectedStyle = value;
                SaveStyle(_selectedStyle);
                NotifyPropertyChanged("SelectedStyle");
            }
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStyle != "None") return;

            var saveDialog = new StyleSaveDialog(Value, _item.PropertyGrid.Tag as XmlSkinInfo);
            if (saveDialog.ShowDialog() != true) return;

            NotifyPropertyChanged("Styles");
            SelectedStyle = saveDialog.StyleId;
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            _item = propertyItem;
            SkinInfo = propertyItem.PropertyGrid.Tag as XmlSkinInfo;
            if (_item == null || _item.Value == null) return this;

            var style = _item.Value as XmlControlStyle;
            if (style == null) return this;

            Value = style;
            NotifyPropertyChanged("Styles");
            SelectedStyle = !string.IsNullOrEmpty(style.StyleId) ? style.StyleId : "None";
            return this;
        }

        private void SaveStyle(string style)
        {
            if (string.IsNullOrEmpty(style)) return;

            if (style == "None")
            {
                if (Value != null && !string.IsNullOrEmpty(Value.StyleId))
                {
                    Value = Value != null ? Value.CreateCopy() : (XmlControlStyle)Activator.CreateInstance(Value.GetType());
                    Value.StyleId = string.Empty;
                }
                _item.HasChildProperties = true;
                _item.IsExpanded = true;
            }
            else
            {
                if (SkinInfo.Style.ControlStyles.Any(s => s.StyleId == _selectedStyle))
                {
                    Value = SkinInfo.Style.ControlStyles.FirstOrDefault(s => s.StyleId == _selectedStyle);
                    _item.IsExpanded = false;
                    _item.HasChildProperties = false;
                }
                else
                {
                    SelectedStyle = "None";
                    return;
                }
            }
            _item.Value = Value;

            // Value.PropertyChanged += (s, e) => { (_Item.Instance as XmlControl).NotifyPropertyChanged(_Item.PropertyDescriptor.Name); };

            var styleProperty = _item.Instance.GetType().GetProperty(_item.PropertyDescriptor.Name);
            if (Value != null && (styleProperty != null && styleProperty.PropertyType == Value.GetType()))
            {
                styleProperty.SetValue(_item.Instance, Value);
            }
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
