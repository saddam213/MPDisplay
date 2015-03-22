using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Common.Helpers;
using Common.Logging;
using Common.Settings;
using MPDisplay.Common.Utils;

namespace GUIConfig.ViewModels
{
    /// <summary>
    /// Interaction logic for BasicSettingsView.xaml
    /// </summary>
    public partial class AddImageSettingsView : ViewModelBase
    {
        #region Fields

        private Log Log = LoggingManager.GetLog(typeof(AddImageSettingsView));
        private AddImagePropertySettings _selectedAddImageProperty;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SkinSettingsView"/> class.
        /// </summary>
        public AddImageSettingsView()
        {
            SelectPathCommand = new RelayCommand(LaunchPathPicker);
            DeleteCommand = new RelayCommand(DeleteItem);
            NewCommand = new RelayCommand(NewItem);
            InitializeComponent();
            HasPendingChanges = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the select path command.
        /// </summary>
        public ICommand SelectPathCommand { get; set; }

        /// <summary>
        /// Gets or sets the delete command.
        /// </summary>
        public ICommand DeleteCommand { get; set; }

        /// <summary>
        /// Gets or sets the New command.
        /// </summary>
        public ICommand NewCommand { get; set; }

         /// <summary>
        /// Gets or sets the selected skin.
        /// </summary>
        public AddImagePropertySettings SelectedAddImageProperty
        {
            get { return _selectedAddImageProperty; }
            set { _selectedAddImageProperty = value; NotifyPropertyChanged("SelectedAddImageProperty"); }
        }

        /// <summary>
        /// Gets the tabs title.
        /// </summary>
        public override string Title
        {
            get { return "AddImage Settings"; }
        }

        #endregion 

        #region Methods

        /// <summary>
        /// Called when model tab opens.
        /// </summary>
        public override void OnModelOpen()
        {
            base.OnModelOpen();
        }

        /// <summary>
        /// Called when model tab closes.
        /// </summary>
        public override void OnModelClose()
        {
            base.OnModelClose();
        }

        /// <summary>
        /// Saves the changes.
        /// </summary>
        public override void SaveChanges()
        {
            base.SaveChanges();
        }

        /// <summary>
        /// Launches path picker.
        /// </summary>
        private void LaunchPathPicker()
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            DialogResult result = dlg.ShowDialog();
            if( result == DialogResult.OK )
            {
                SelectedAddImageProperty.Path = dlg.SelectedPath;
            }
        }

        /// <summary>
        /// Deletes the current property item.
        /// </summary>
        private void DeleteItem()
        {
            if (SelectedAddImageProperty != null)
            {
                var items = (DataObject as AddImageSettings).AddImagePropertySettings;
                items.Remove(SelectedAddImageProperty);
                SelectedAddImageProperty = items.FirstOrDefault();
                HasPendingChanges = true;
            }
        }

        /// <summary>
        /// Creates a new empty property item.
        /// </summary>
        private void NewItem()
        {
            var items = (DataObject as AddImageSettings).AddImagePropertySettings;
            var newitem = new AddImagePropertySettings();
            newitem.Path = "#MPThumbsPath#";
            items.Add( newitem  );
            SelectedAddImageProperty = newitem;
            HasPendingChanges = true;
        }

        #endregion
    }
}
