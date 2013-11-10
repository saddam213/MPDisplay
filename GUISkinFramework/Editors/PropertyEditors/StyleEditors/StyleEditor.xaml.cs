using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GUISkinFramework.Common;
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Controls;
using MPDisplay.Common.Controls.PropertyGrid;
using MPDisplay.Common.Controls.PropertyGrid.Attributes;
using MPDisplay.Common.Controls.PropertyGrid.Editors;
using GUISkinFramework.Skin;
using GUISkinFramework.Styles;
using Common.Helpers;

namespace GUISkinFramework.Editor.PropertyEditors
{
    /// <summary>
    /// Interaction logic for BrushEditor.xaml
    /// </summary>
    public partial class StyleEditor : UserControl, ITypeEditor, INotifyPropertyChanged
    {
    
        private PropertyItem _Item;

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
            get { return new List<string>(new string[] { "None" }.Concat(SkinInfo.Style.ControlStyles.Where(s => Value != null && s.GetType() == Value.GetType()).Select(x => x.StyleId))); }
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
            if (SelectedStyle == "None")
            {
                var saveDialog = new StyleSaveDialog(Value, _Item.PropertyGrid.Tag as XmlSkinInfo);
                if (saveDialog.ShowDialog() == true)
                {
                    NotifyPropertyChanged("Styles");
                    SelectedStyle = saveDialog.StyleId;
                }
            }
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            _Item = propertyItem;
            SkinInfo = propertyItem.PropertyGrid.Tag as XmlSkinInfo;
            if (_Item != null && _Item.Value != null)
            {
                var style = _Item.Value as XmlControlStyle;
                if (style != null)
                {
                    Value = style;
                    NotifyPropertyChanged("Styles");
                    SelectedStyle = !string.IsNullOrEmpty(style.StyleId) ? style.StyleId : "None";
                }
            }
            return this;
        }

        private void SaveStyle(string style)
        {
            if (!string.IsNullOrEmpty(style))
            {
                if (style == "None")
                {
                    if (Value != null && !string.IsNullOrEmpty(Value.StyleId))
                    {
                       Value = Value != null ? Value.CreateCopy() : (XmlControlStyle)Activator.CreateInstance(Value.GetType());
                        Value.StyleId = string.Empty;
                    }
                    _Item.HasChildProperties = true;
                    _Item.IsExpanded = true;
                }
                else
                {
                    if (SkinInfo.Style.ControlStyles.Any(s => s.StyleId == _selectedStyle))
                    {
                        Value = SkinInfo.Style.ControlStyles.FirstOrDefault(s => s.StyleId == _selectedStyle);
                        _Item.IsExpanded = false;
                        _Item.HasChildProperties = false;
                    }
                    else
                    {
                        SelectedStyle = "None";
                        return;
                    }
                }
                _Item.Value = Value;

               // Value.PropertyChanged += (s, e) => { (_Item.Instance as XmlControl).NotifyPropertyChanged(_Item.PropertyDescriptor.Name); };

                var styleProperty = _Item.Instance.GetType().GetProperty(_Item.PropertyDescriptor.Name);
                if (styleProperty != null && styleProperty.PropertyType == Value.GetType())
                {
                    styleProperty.SetValue(_Item.Instance, Value);
                }
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
