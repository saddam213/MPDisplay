using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
using GUISkinFramework.Animations;
using GUISkinFramework.Common;
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Controls;
using MPDisplay.Common.Controls.PropertyGrid;
using MPDisplay.Common.Controls.PropertyGrid.Editors;

namespace GUISkinFramework.Editor.PropertyEditors
{
    /// <summary>
    /// Interaction logic for BrushEditor.xaml
    /// </summary>
    public partial class AnimationEditor : UserControl, ITypeEditor
    {
    
        private PropertyItem _Item;

        public AnimationEditor()
        {
            InitializeComponent();
          
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(ObservableCollection<XmlAnimation>), typeof(AnimationEditor),
                                                                                      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ObservableCollection<XmlAnimation> Value
        {
            get { return (ObservableCollection<XmlAnimation>)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public string ActionInfo
        {
            get { return (string)GetValue(ActionInfoProperty); }
            set { SetValue(ActionInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BrushTypeInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActionInfoProperty =
            DependencyProperty.Register("ActionInfo", typeof(string), typeof(AnimationEditor), new PropertyMetadata("(Empty)"));

        public string ActionToolTip

        {
            get { return (string)GetValue(ActionToolTipProperty); }
            set { SetValue(ActionToolTipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BrushTypeInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActionToolTipProperty =
            DependencyProperty.Register("ActionToolTip", typeof(string), typeof(AnimationEditor), new PropertyMetadata("(Empty)"));

      

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AnimationEditorDialog editor = new AnimationEditorDialog(_Item.Instance);
            editor.SetItems((_Item.Value as ObservableCollection<XmlAnimation>) ?? new ObservableCollection<XmlAnimation>());
            if (editor.ShowDialog() == true)
            {
                _Item.Value = editor.GetItems();
            }
            ActionInfo = GetText();
            ActionToolTip = GetToolTipText();
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            _Item = propertyItem;
            ActionInfo = GetText();
            ActionToolTip = GetToolTipText();
            return this;
        }

        private string GetText()
        {
            //if (_Item != null && _Item.Value != null)
            //{
            //    if (_Item.Value is IList)
            //    {
            //        return (_Item.Value as IList).Count > 0 ? "(Collection)" : "(Empty)";
            //    }
            //}
            return "(Empty)";
        }

        private string GetToolTipText()
        {
           
            //if (_Item != null && _Item.Value is IList && (_Item.Value as IList).Count > 0)
            //{
            //    string returnValue = "Actions:" + Environment.NewLine;
            //    foreach (var item in (_Item.Value as IList))
            //    {
            //        returnValue += (item as XmlAction).DisplayName + Environment.NewLine;
            //    }
            //    return returnValue;
            //}
            return "(Empty)";
        }
    }


}
