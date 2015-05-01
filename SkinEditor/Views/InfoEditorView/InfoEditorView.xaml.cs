using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Windows.Input;
using MPDisplay.Common.Utils;
using SkinEditor.Helpers;
using SkinEditor.Themes;

namespace SkinEditor.Views
{
    /// <summary>
    /// Interaction logic for StyleEditorView.xaml
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public partial class InfoEditorView
    {

        public InfoEditorView(ConnectionHelper connectionHelper)
        {
            ConnectionHelper = connectionHelper;

            ConnectCommand = new RelayCommand(async () => await ConnectionHelper.InitializeServerConnection(), () => !ConnectionHelper.IsConnected);
            DisconnectCommand = new RelayCommand(async () => await ConnectionHelper.Disconnect(), () => ConnectionHelper.IsConnected);
            ClearPropertyCommand = new RelayCommand(ConnectionHelper.PropertyData.Clear);
            ClearListItemCommand = new RelayCommand(ConnectionHelper.ListItemData.Clear);
            PropertyEditCommand = new RelayCommand<SkinPropertyItem>(ConnectionHelper.OpenPropertyEditor);

            InitializeComponent();
        }


        public override string Title
        {
            get { return "Info Browser"; }
        }

        public override void Initialize()
        {
            ConnectionHelper.Settings = ConnectionSettings;
            ConnectionHelper.Baseclass = this;

            ConnectionHelper.StartSecondTimer();
        }

        public InfoEditorViewSettings ConnectionSettings
        {
            get { return ConnectSettings as InfoEditorViewSettings; }
        }


         public InfoEditorViewSettings Settings
        {
            get { return EditorSettings as InfoEditorViewSettings; }
        }

        public ICommand ConnectCommand { get; internal set; }
        public ICommand DisconnectCommand { get; internal set; }
        public ICommand ClearPropertyCommand { get; internal set; }
        public ICommand PropertyEditCommand { get; internal set; }
        public ICommand ClearListItemCommand { get; internal set; }

        public bool IsConnected
        {
            get { return ConnectionHelper.IsConnected; }
        }

        public bool IsMediaPortalConnected
        {
            get { return ConnectionHelper.IsMediaPortalConnected; }
        }

        public ObservableCollection<SkinPropertyItem> PropertyData
        {
            get { return ConnectionHelper.PropertyData; }
        }

        public ObservableCollection<SkinPropertyItem> ListItemData
        {
            get { return ConnectionHelper.ListItemData; }
        }

        public int WindowId
        {
            get { return ConnectionHelper.WindowId; }
        }

        public int DialogId
        {
            get { return ConnectionHelper.DialogId; }
        }

        public int FocusedControlId
        {
            get { return ConnectionHelper.FocusedControlId; }
        }

        public override void OnModelOpen()
        {
            NotifyPropertyChanged("Settings");
        }

        public override void OnModelClose()
        {

        }

    }
}
