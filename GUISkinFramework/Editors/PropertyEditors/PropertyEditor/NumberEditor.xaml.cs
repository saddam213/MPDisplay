using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GUISkinFramework.Skin;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Editors
{
    /// <summary>
    /// Interaction logic for BrushEditor.xaml
    /// </summary>
    public partial class NumberEditor : UserControl, ITypeEditor, INotifyPropertyChanged
    {
    
        private PropertyItem _Item;

        public NumberEditor()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(NumberEditor),
                                                                                      new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public List<string> NumberProperties
        {
            get { return SkinInfo.Properties.Where(p => p.PropertyType == XmlPropertyType.Number).Select(x => x.SkinTag).ToList(); }
        }

        private XmlSkinInfo _skinInfo = new XmlSkinInfo();

        public XmlSkinInfo SkinInfo
        {
            get { return _skinInfo; }
            set { _skinInfo = value; NotifyPropertyChanged("SkinInfo"); NotifyPropertyChanged("NumberProperties"); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var propEditor = new PropertyEditor(SkinInfo);
            propEditor.SelectedProperty = SkinInfo.Properties.FirstOrDefault(p => p.SkinTag == Value);
            if (new EditorDialog(propEditor, true).ShowDialog() == true && propEditor.SelectedProperty != null)
            {
                NotifyPropertyChanged("NumberProperties");
                Value = propEditor.SelectedProperty.SkinTag;
            }
          
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            _Item = propertyItem;
            SkinInfo = propertyItem.PropertyGrid.Tag as XmlSkinInfo;
            Binding binding = new Binding("Value");
            binding.Source = propertyItem;
            binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            BindingOperations.SetBinding(this, ValueProperty, binding);
            NotifyPropertyChanged("NumberProperties");
            return this;
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
