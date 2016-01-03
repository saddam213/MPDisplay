using System.ComponentModel;
using System.Windows;
using Common.Helpers;
using GUISkinFramework.Skin;

namespace GUISkinFramework.Editors
{
   

    /// <summary>
    /// Interaction logic for BackgroundEditorDialog.xaml
    /// </summary>
    public partial class BrushEditorDialog : INotifyPropertyChanged
    {
      
        public XmlSkinInfo SkinInfo
        {
            get { return (XmlSkinInfo)GetValue(SkinInfoProperty); }
            set { SetValue(SkinInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SkinInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SkinInfoProperty =
            DependencyProperty.Register("SkinInfo", typeof(XmlSkinInfo), typeof(BrushEditorDialog), new PropertyMetadata(new XmlSkinInfo()));

        private XmlBrush _value;
        private XmlBrush _newValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrushEditorDialog"/> class.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="skinInfo">SkinInfo XML</param>
        public BrushEditorDialog(XmlBrush brush, XmlSkinInfo skinInfo)
        {
            InitializeComponent();
            SkinInfo = skinInfo;
            Value = brush;
            NewValue = brush;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public XmlBrush Value
        {
            get { return _value; }
            set { _value = value; NotifyPropertyChanged("Value"); }
        }

        /// <summary>
        /// Gets or sets the new value.
        /// </summary>
        public XmlBrush NewValue
        {
            get { return _newValue; }
            set { _newValue = value; NotifyPropertyChanged("NewValue"); }
        }

        /// <summary>
        /// Brushes the type editor_ on brush changed.
        /// </summary>
        /// <param name="brush">The brush.</param>
        private void BrushTypeEditor_OnBrushChanged(XmlBrush brush)
        {
            NewValue = brush.CreateCopy();
        }

        /// <summary>
        /// Handles the Click event of the Button_Ok control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// Handles the Click event of the Button_Cancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Handles the Click event of the Button_SaveStyle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_SaveStyle_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new StyleSaveDialog(NewValue, SkinInfo);
            if (saveDialog.ShowDialog() != true) return;

            NotifyPropertyChanged("Styles");
            NewValue = saveDialog.NewStyle as XmlBrush;
            Value = saveDialog.NewStyle as XmlBrush;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="property">The property.</param>
        public void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
