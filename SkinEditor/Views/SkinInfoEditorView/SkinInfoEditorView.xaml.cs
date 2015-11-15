using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using GUISkinFramework.ExtensionMethods;
using GUISkinFramework.Skin;
using MPDisplay.Common.Utils;

namespace SkinEditor.Views
{
    /// <summary>
    /// Interaction logic for StyleEditorView.xaml
    /// </summary>
    public partial class SkinInfoEditorView
    {
        public SkinInfoEditorView()
        {
            SkinOptionItemMoveUp = new RelayCommand<int>(SkinInfo.SkinOptions.MoveItemUp, SkinInfo.SkinOptions.CanItemMoveUp);
            SkinOptionItemMoveDown = new RelayCommand<int>(SkinInfo.SkinOptions.MoveItemDown, SkinInfo.SkinOptions.CanItemMoveDown);
            SkinOptionItemAdd = new RelayCommand(() => SkinInfo.SkinOptions.Add(new XmlSkinOption {  Name = "New..." }));
            SkinOptionItemRemove = new RelayCommand(() => SkinInfo.SkinOptions.Remove(SelectedSkinOption), () => SelectedSkinOption != null);
            InitializeComponent();
        }

        public override string Title => "Skin Settings";

        public override void Initialize()
        {
            base.Initialize();
            if (SkinInfo == null) return;

            SkinInfo.PropertyChanged += SkinInfo_PropertyChanged;
            SkinInfo.SkinOptions.CollectionChanged += SkinOptions_CollectionChanged;
        }

        void SkinOptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            HasPendingChanges = true;
        }

        void SkinInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            HasPendingChanges = true;
        }

        public SkinInfoEditorViewSettings Settings => EditorSettings as SkinInfoEditorViewSettings;

        public ICommand SkinOptionItemMoveUp { get; internal set; }
        public ICommand SkinOptionItemMoveDown { get; internal set; }
        public ICommand SkinOptionItemAdd { get; internal set; }
        public ICommand SkinOptionItemRemove { get; internal set; }

        private XmlSkinOption _selectedSkinOption;

        public XmlSkinOption SelectedSkinOption
        {
            get { return _selectedSkinOption; }
            set { _selectedSkinOption = value; NotifyPropertyChanged("SelectedSkinOption"); }
        }
        
    }


    public class SkinOptionNameValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var filePath = value as string;
            //check for empty/null file path:
            if (string.IsNullOrEmpty(filePath) || filePath.Any(char.IsWhiteSpace))
            {
                return new ValidationResult(false, "SkinOption name cannot contain empty space");
            }

            return !Regex.IsMatch(filePath, @"^[a-zA-Z0-9\\_]+$") ? new ValidationResult(false, "SkinOption name can only contain letters, numbers and underscore") : new ValidationResult(true, null);
        }
    }
}
