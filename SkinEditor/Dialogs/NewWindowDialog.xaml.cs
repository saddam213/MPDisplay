using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using GUISkinFramework.Skin;

namespace SkinEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for NewWindowDialog.xaml
    /// </summary>
    public partial class NewWindowDialog : INotifyPropertyChanged
    {
        private XmlSkinInfo _skinInfo;
        private IXmlControlHost _newElement;
        private readonly XmlStyleCollection _designerStyle;
        public enum WindowOption { None, MPDWindow, MPDDialog, MPWindow, MPDialog, PlayerWindow }
        private WindowOption _currentOption;
        private int _windowId;
        private string _windowName = "New...";


        public NewWindowDialog(XmlSkinInfo skinInfo, XmlStyleCollection designerStyle)
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            SkinInfo = skinInfo;
            _designerStyle = designerStyle;
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
                MessageBox.Show($"Window/Dialog Id '{_windowId}' is already in-use, Please select a new Id.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SkinInfo.Windows.Any(w => w.Name == _windowName) || SkinInfo.Dialogs.Any(w => w.Name == _windowName))
            {
                MessageBox.Show($"Window/Dialog Name '{_windowName}' is already in-use, Please select a new name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                case WindowOption.PlayerWindow:
                    NewWindow = CreateWindow<XmlPlayerWindow>();
                    break;
            }

            var xmlWindow = NewWindow as XmlWindow;
            if (xmlWindow != null)
            {
                var window = xmlWindow;
                window.BackgroundBrush = _designerStyle.GetDesignerBrushStyle("WindowBackground") ?? new XmlBrush();
                SkinInfo.AddWindow(window);
            }

            var xmlDialog = NewWindow as XmlDialog;
            if (xmlDialog != null)
            {
                var dialog = xmlDialog;
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
                Animations = new ObservableCollection<XmlAnimation>(),
                Controls = new ObservableCollection<XmlControl>()
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
                Animations = new ObservableCollection<XmlAnimation>(),
                Controls = new ObservableCollection<XmlControl>()
            };
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
      
      
        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion
    }
}
