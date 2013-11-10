using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using GUISkinFramework.Common;
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Controls;
using GUISkinFramework.Skin;
using GUISkinFramework.Styles;
using Common.Helpers;

namespace GUISkinFramework.Editor.PropertyEditors
{
    /// <summary>
    /// Interaction logic for StyleSaveDialog.xaml
    /// </summary>
    public partial class StyleSaveDialog : Window, INotifyPropertyChanged 
    {
        private string _styleId;
        private XmlStyle _style;

        public StyleSaveDialog(XmlStyle style, XmlSkinInfo skinInfo)
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            SkinInfo = skinInfo;
            NewStyle = style;
            StyleId = "New " + style.GetType().Name.Replace("Xml", "");
        }

        private XmlSkinInfo _skinInfo = new XmlSkinInfo();

        public XmlSkinInfo SkinInfo
        {
            get { return _skinInfo; }
            set { _skinInfo = value; NotifyPropertyChanged("SkinInfo"); }
        }

        public string StyleId
        {
            get { return _styleId; }
            set { _styleId = value; NotifyPropertyChanged("StyleId"); }
        }

        public XmlStyle NewStyle
        {
            get { return _style; }
            set { _style = value; NotifyPropertyChanged("Style"); }
        }
        
        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            var brushStyle = NewStyle.CreateCopy() as XmlStyle;
            if (brushStyle != null)
            {
                brushStyle.StyleId = StyleId;
                if (!SkinInfo.Style.StyleExists(brushStyle) || Overwrite())
                {
                    foreach (var style in SkinInfo.Styles.Values)
                    {
                        style.SaveStyle(brushStyle.CreateCopy());
                    }
                }
                NewStyle = brushStyle;
            }
            DialogResult = true;
        }

        private bool Overwrite()
        {
            return MessageBox.Show(string.Format("Style '{0}' already exists{1}{1}Would you like to overwrite?",StyleId,Environment.NewLine)
                , "Overwrite?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }


        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}
