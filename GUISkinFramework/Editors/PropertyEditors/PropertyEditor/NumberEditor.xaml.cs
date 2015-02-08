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
using GUISkinFramework.PropertyEditors;

namespace GUISkinFramework.Editor.PropertyEditors
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
            get { return SkinInfo.Properties.Where(p => p.PropertyType == Property.XmlPropertyType.Number).Select(x => x.SkinTag).ToList(); }
        }

        private XmlSkinInfo _skinInfo = new XmlSkinInfo();

        public XmlSkinInfo SkinInfo
        {
            get { return _skinInfo; }
            set { _skinInfo = value; NotifyPropertyChanged("SkinInfo"); NotifyPropertyChanged("NumberProperties"); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var propEditor = new PropertyEditor.PropertyEditor(SkinInfo);
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
