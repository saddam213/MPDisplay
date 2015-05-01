using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GUISkinFramework.Skin;

namespace GUISkinFramework.Editors
{
    /// <summary>
    /// Interaction logic for PropertyEditor.xaml
    /// </summary>
    public partial class LanguageEditor : UserControl, INotifyPropertyChanged
    {
        private XmlLanguageEntry _selectedLanguage;
     
        public LanguageEditor(XmlSkinInfo skinInfo)
        {
            InitializeComponent();
            SkinInfo = skinInfo;
        }


        private XmlSkinInfo _skinInfo = new XmlSkinInfo();

        public XmlSkinInfo SkinInfo
        {
            get { return _skinInfo; }
            set { _skinInfo = value; NotifyPropertyChanged("SkinInfo"); }
        }
        

       


        public XmlLanguageEntry SelectedEntry
        { 
            get { return _selectedLanguage; }
            set
            { 
                _selectedLanguage = value;

                NotifyPropertyChanged("SelectedEntry");
            }
        }

       

     

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private void Button_LanguageAdd_Click(object sender, RoutedEventArgs e)
        {
            var newEntry = new XmlLanguageEntry { SkinTag = "New....", Values = new ObservableCollection<XmlLanguageValue>() };
            if (SkinInfo.Language.LanguageEntries.Any())
            {
                var xmlLanguageEntry = SkinInfo.Language.LanguageEntries.FirstOrDefault();
                if (xmlLanguageEntry != null)
                    foreach (var item in xmlLanguageEntry.Values)
                    {
                        newEntry.Values.Add(new XmlLanguageValue { Language = item.Language });
                    }
            }


            SkinInfo.Language.LanguageEntries.Insert(0, newEntry);
            SelectedEntry = newEntry;
        }

        private void Button_LanguageRemove_Click(object sender, RoutedEventArgs e)
        {
            SkinInfo.Language.LanguageEntries.Remove(SelectedEntry);
        }
      
    }

    public class LanguageTagValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {

            var tag = value as string;
            //check for empty/null file path:
            if (string.IsNullOrEmpty(tag) || string.IsNullOrWhiteSpace(tag))
            {
                return new ValidationResult(false, "The language tag may not be empty.");
            }

            if (!tag.StartsWith("@"))
            {
                return new ValidationResult(false, "The language tag must start with '@'");
            }

            return new ValidationResult(true, null);
        }
    }

}
