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
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Controls;
using GUISkinFramework.Styles;
using MPDisplay.Common.Controls.PropertyGrid;
using MPDisplay.Common.ExtensionMethods;
using SkinEditor.Dialogs;
using GUISkinFramework.Windows;
using GUIFramework.Managers;
using MPDisplay.Common.Settings;
using MPDisplay.Common.Utils;
using GUISkinFramework.Common;
using GUISkinFramework.ExtensionMethods;
using System.Text.RegularExpressions;

namespace SkinEditor.Views
{
    /// <summary>
    /// Interaction logic for StyleEditorView.xaml
    /// </summary>
    public partial class SkinInfoEditorView : EditorViewModel
    {
        public SkinInfoEditorView()
        {
            SkinOptionItemMoveUp = new RelayCommand(param => SkinInfo.SkinOptions.MoveItemUp((int)param), param => param == null ? false : SkinInfo.SkinOptions.CanItemMoveUp((int)param));
            SkinOptionItemMoveDown = new RelayCommand(param => SkinInfo.SkinOptions.MoveItemDown((int)param), param => param == null ? false : SkinInfo.SkinOptions.CanItemMoveDown((int)param));
            SkinOptionItemAdd = new RelayCommand(param => SkinInfo.SkinOptions.Add(new XmlSkinOption {  Name = "New..." }));
            SkinOptionItemRemove = new RelayCommand(param => SkinInfo.SkinOptions.Remove(SelectedSkinOption), param => SelectedSkinOption != null);
            InitializeComponent();
        }

        public override string Title
        {
            get { return "Skin Settings"; }
        }

        public override void Initialize()
        {
            base.Initialize();
            if (SkinInfo != null)
            {
                SkinInfo.PropertyChanged += SkinInfo_PropertyChanged;
                SkinInfo.SkinOptions.CollectionChanged += SkinOptions_CollectionChanged;
            }
        }

        void SkinOptions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            HasPendingChanges = true;
        }

        void SkinInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            HasPendingChanges = true;
        }

        public SkinInfoEditorViewSettings Settings 
        {
            get { return EditorSettings as SkinInfoEditorViewSettings; }
        }


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
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            var filePath = value as string;
            //check for empty/null file path:
            if (string.IsNullOrEmpty(filePath) || filePath.Any(char.IsWhiteSpace))
            {
                return new ValidationResult(false, "SkinOption name cannot contain empty space");
            }

            if (!Regex.IsMatch(filePath, @"^[a-zA-Z0-9\\_]+$"))
            {
                return new ValidationResult(false, "SkinOption name can only contain letters, numbers and underscore");
            }

            return new ValidationResult(true, null);
        }
    }
}
