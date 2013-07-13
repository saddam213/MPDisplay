using System;
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
using System.Windows.Shapes;
using GUISkinFramework;
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Dialogs;
using GUISkinFramework.Styles;
using GUISkinFramework.Windows;

namespace SkinEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for NewWindowDialog.xaml
    /// </summary>
    public partial class NewWindowDialog : Window, INotifyPropertyChanged
    {
        private XmlSkinInfo _skinInfo;
        private IXmlControlHost _newElement;
        private XmlStyleCollection _designerStyle;
        public enum WindowOption { None, MPDWindow, MPDDialog, MPWindow, MPDialog }
        private WindowOption _currentOption;
        private int _windowId;
        private string _windowName = "New...";


        public NewWindowDialog(XmlSkinInfo skinInfo, XmlStyleCollection designerStyle)
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            this.SkinInfo = skinInfo;
            this._designerStyle = designerStyle;
        }

        public XmlSkinInfo SkinInfo
        {
            get { return _skinInfo; }
            set { _skinInfo = value; NotifyPropertyChanged("SkinInfo"); }
        }

        public IXmlControlHost NewWindow
        {
            get { return _newElement; }
            set { _newElement = value; NotifyPropertyChanged("NewElement"); }
        }

        public WindowOption CurrentOption
        {
            get { return _currentOption; }
            set { _currentOption = value; NotifyPropertyChanged("CurrentOption"); }
        }

        public int WindowId
        {
            get { return _windowId; }
            set { _windowId = value; NotifyPropertyChanged("WindowId"); }
        }

        public string WindowName
        {
            get { return _windowName; }
            set { _windowName = value; NotifyPropertyChanged("WindowName");}
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            if (SkinInfo.Windows.Any(w => w.Id == _windowId) || SkinInfo.Dialogs.Any(w => w.Id == _windowId))
            {
                MessageBox.Show(string.Format("Window/Dialog Id '{0}' is already in-use, Please select a new Id.", _windowId), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SkinInfo.Windows.Any(w => w.Name == _windowName) || SkinInfo.Dialogs.Any(w => w.Name == _windowName))
            {
                MessageBox.Show(string.Format("Window/Dialog Name '{0}' is already in-use, Please select a new name.", _windowName), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            switch (CurrentOption)
            {
                case WindowOption.MPDWindow:
                    NewWindow = CreateWindow<XmlMPDWindow>();
                    break;
                case WindowOption.MPDDialog:
                    NewWindow = CreateDialog<XmlMPDDialog>();
                    break;
                case WindowOption.MPWindow:
                    NewWindow = CreateWindow<XmlMPWindow>();
                    break;
                case WindowOption.MPDialog:
                    NewWindow = CreateDialog<XmlMPDialog>();
                    break;
                default:
                    break;
            }

            if (NewWindow is XmlWindow)
            {
                var window = NewWindow as XmlWindow;
                window.BackgroundBrush = _designerStyle.GetDesignerBrushStyle("WindowBackground") ?? new XmlBrush();
                SkinInfo.AddWindow(window);
            }

            if (NewWindow is XmlDialog)
            {
                var dialog = NewWindow as XmlDialog;
                dialog.BackgroundBrush = _designerStyle.GetDesignerBrushStyle("WindowBackground") ?? new XmlBrush();
                dialog.BorderBrush = _designerStyle.GetDesignerBrushStyle("DialogBorder") ?? new XmlBrush();
                dialog.CornerRadius = "5";
                dialog.BorderThickness = "2";
                SkinInfo.AddDialog(dialog);
            }

            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }


        /// <summary>
        /// Creates the window.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T CreateWindow<T>() where T : XmlWindow, new()
        {
            return new T
            {
                Id = _windowId,
                Name = _windowName,
                Height = SkinInfo.SkinHeight ,
                Width = SkinInfo.SkinWidth ,
                Animations = new ObservableCollection<GUISkinFramework.Animations.XmlAnimation>(),
                Controls = new System.Collections.ObjectModel.ObservableCollection<GUISkinFramework.Controls.XmlControl>(),
            };
        }

        /// <summary>
        /// Creates the dialog.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T CreateDialog<T>() where T : XmlDialog, new()
        {
            return new T
            {
                Id = _windowId,
                Name = _windowName,
                Height = 300,
                Width = 500,
                Animations = new ObservableCollection<GUISkinFramework.Animations.XmlAnimation>(),
                Controls = new System.Collections.ObjectModel.ObservableCollection<GUISkinFramework.Controls.XmlControl>(),
            };
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
