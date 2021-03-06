﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GUISkinFramework.Skin;

namespace GUISkinFramework.Editors
{
    /// <summary>
    /// Interaction logic for PropertyEditor.xaml
    /// </summary>
    public partial class PropertyEditor : INotifyPropertyChanged
    {
 
        private XmlProperty _selectedProperty;
        private int _mediaPortalTagSelectedIndex;
        private FilterOptions _currentFilter;

        public PropertyEditor(XmlSkinInfo skinInfo)
        {
            InitializeComponent();
            SkinInfo = skinInfo;
            Properties = CollectionViewSource.GetDefaultView(skinInfo.Properties);
            Properties.GroupDescriptions.Clear();
            Properties.SortDescriptions.Clear();
            Properties.GroupDescriptions.Add(new PropertyGroupDescription("PropertyType"));
            Properties.SortDescriptions.Add(new SortDescription("SkinTag", ListSortDirection.Ascending));
            Properties.Filter = FilterPropertyList;
           // SelectedProperty = skinInfo.Properties.FirstOrDefault();
        }


        private XmlSkinInfo _skinInfo = new XmlSkinInfo();

        public XmlSkinInfo SkinInfo
        {
            get { return _skinInfo; }
            set { _skinInfo = value; NotifyPropertyChanged("SkinInfo"); }
        }

        public FilterOptions CurrentFilter
        {
            get { return _currentFilter; }
            set { _currentFilter = value; Properties.Refresh();  NotifyPropertyChanged("CurrentFilter"); }
        }

        private ICollectionView _properties;

        public ICollectionView Properties
        {
            get { return _properties; }
            set { _properties = value; NotifyPropertyChanged("Properties"); }
        }
          
        public XmlProperty SelectedProperty
        { 
            get { return _selectedProperty; }
            set
            { 
                _selectedProperty = value;
                MediaPortalTagSelectedIndex = 0;
                NotifyPropertyChanged("SelectedProperty");
            }
        }

        public int MediaPortalTagSelectedIndex
        {
            get { return _mediaPortalTagSelectedIndex; }
            set { _mediaPortalTagSelectedIndex = value; NotifyPropertyChanged("MediaPortalTagSelectedIndex"); }
        }
        

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void Button_PropertyAdd_Click(object sender, RoutedEventArgs e)
        {
            var prop = new XmlProperty
            {
                SkinTag = "New....",
                DesignerValue = "New....",
                MediaPortalTags = new ObservableCollection<XmlMediaPortalTag> 
                {
                    new XmlMediaPortalTag
                    { 
                        Tag = "New...."
                    }
                }
            };

            SkinInfo.Properties.Add(prop);
            SelectedProperty = prop;
        }

        #region UIEvents

        private void Button_PropertyRemove_Click(object sender, RoutedEventArgs e)
        {
            SkinInfo.Properties.Remove(SelectedProperty);
        }

        private void Button_MediaPortalTagAdd_Click(object sender, RoutedEventArgs e)
        {
            SelectedProperty?.MediaPortalTags.Add(new XmlMediaPortalTag { Tag = "New..." });
        }

        private void Button_MediaPortalTagRemove_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProperty == null) return;
            if (MediaPortalTagSelectedIndex >= 0 && SelectedProperty.MediaPortalTags.Count - 1 >= MediaPortalTagSelectedIndex)
            {
                SelectedProperty.MediaPortalTags.RemoveAt(MediaPortalTagSelectedIndex);
            }
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            Properties.Refresh();
        }

       // Filter the property list
        private bool FilterPropertyList(object item)
        {
            var value = true;

            var property = item as XmlProperty;

            switch (CurrentFilter)
            {
                case FilterOptions.ShowMP:
                    if (property != null && property.IsInternal) value = false;
                    break;

                case FilterOptions.ShowMPD:
                     if (property != null && !property.IsInternal) value = false;
                   break;
            }
            return value;
        } 

    #endregion
     }

  public class PropertyTagValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
          
            var tag = value as string;
            //check for empty/null file path:
            if (string.IsNullOrEmpty(tag) || string.IsNullOrWhiteSpace(tag))
            {
                return new ValidationResult(false, "The property tag may not be empty.");
            }

            return !tag.StartsWith("#") ? new ValidationResult(false, "The property tag must start with '#'") : new ValidationResult(true, null);
        }
    }

    // enum for the property list filter
    public enum FilterOptions { ShowAll, ShowMP, ShowMPD }

}
