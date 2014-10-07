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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using GUISkinFramework;
using GUISkinFramework.ExtensionMethods;
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Controls;
using GUISkinFramework.Styles;
using MPDisplay.Common.Controls.PropertyGrid;
using MPDisplay.Common;
using SkinEditor.Dialogs;
using MPDisplay.Common.Controls;
using Common.Helpers;
using MPDisplay.Common.ExtensionMethods;
using GUIFramework;
using MessageFramework.DataObjects;
using System.ServiceModel;
using MPDisplay.Common.Utils;
using SkinEditor.ConnectionHelpers;

namespace SkinEditor.Views
{
    /// <summary>
    /// Interaction logic for StyleEditorView.xaml
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public partial class InfoEditorView : EditorViewModel
    {

        public InfoEditorView(ConnectionHelper _connectionHelper)
        {
            ConnectionHelper = _connectionHelper;

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
            ConnectionHelper.settings = ConnectionSettings;
            ConnectionHelper.baseclass = this;

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
