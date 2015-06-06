using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Common.Settings;
using MPDisplay.Common.Utils;

namespace GUIConfig.ViewModels
{
    /// <summary>
    /// Interaction logic for BasicSettingsView.xaml
    /// </summary>
    public partial class AddImageSettingsView
    {
        #region Fields

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
            set { _selectedAddImageProperty = value; NotifyPropertyChanged(); }
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
        /// Launches path picker.
        /// </summary>
        private void LaunchPathPicker()
        {
            var dlg = new FolderBrowserDialog();
            var result = dlg.ShowDialog();
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
            if (SelectedAddImageProperty == null) return;
            var addImageSettings = DataObject as AddImageSettings;
            if (addImageSettings != null)
            {
                var items = addImageSettings.AddImagePropertySettings;
                items.Remove(SelectedAddImageProperty);
                SelectedAddImageProperty = items.FirstOrDefault();
            }
            HasPendingChanges = true;
        }

        /// <summary>
        /// Creates a new empty property item.
        /// </summary>
        private void NewItem()
        {
            var addImageSettings = DataObject as AddImageSettings;
            if (addImageSettings != null)
            {
                var items = addImageSettings.AddImagePropertySettings;
                var newitem = new AddImagePropertySettings {Path = "#MPThumbsPath#"};
                items.Add( newitem  );
                SelectedAddImageProperty = newitem;
            }
            HasPendingChanges = true;
        }

        #endregion
    }
}
