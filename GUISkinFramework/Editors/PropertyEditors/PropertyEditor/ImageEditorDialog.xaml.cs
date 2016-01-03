using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using GUISkinFramework.Skin;

namespace GUISkinFramework.Editors
{
    /// <summary>
    /// Interaction logic for VisibleConditionEditorDialog.xaml
    /// </summary>
    public partial class ImageEditorDialog : INotifyPropertyChanged
    {
        private object _instance;
        private string _currentLabel = string.Empty;
 
        /// <summary>
        /// Initializes a new instance of the <see cref="VisibleConditionEditorDialog"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="skinInfo">The current skin XML</param>
        public ImageEditorDialog(object instance, XmlSkinInfo skinInfo)
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            Instance = instance;
            SkinInfo = skinInfo;
        }

        private XmlSkinInfo _skinInfo = new XmlSkinInfo();

        public XmlSkinInfo SkinInfo
        {
            get { return _skinInfo; }
            set { _skinInfo = value; NotifyPropertyChanged("SkinInfo"); }
        }
        

        private void SetLabel(string label)
        {
            if (label.Contains("+"))
            {
                foreach (var item in label.Split('+'))
                {
                    LabelItems.Add(item);
                    LabelItems.Add("+");
                }
                LabelItems.RemoveAt(LabelItems.Count -1);
            }
            else
            {
                if (!string.IsNullOrEmpty(label))
                {
                    LabelItems.Add(label);
                }
            }
            SelectedLabelItem = null;
            NotifyPropertyChanged("DisplayLabel");
        }

        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        public object Instance
        {
            get { return _instance; }
            set { _instance = value; NotifyPropertyChanged("Instance"); }
        }

        /// <summary>
        /// Gets or sets the current condition.
        /// </summary>
        public string CurrentLabel
        {
            get { return _currentLabel; }
            set { _currentLabel = value; SetLabel(_currentLabel); NotifyPropertyChanged("CurrentLabel"); }
        }





        private XmlImageFile _selectedImage;
        private string _selectedTextEntry = "";
        private XmlProperty _selectedPropertyEntry;

        public XmlImageFile SelectedImage
        {
            get { return _selectedImage; }
            set { _selectedImage = value; NotifyPropertyChanged("SelectedImage"); }
        }

        public string SelectedTextEntry
        {
            get { return _selectedTextEntry; }
            set { _selectedTextEntry = value; NotifyPropertyChanged("SelectedTextEntry"); }
        }

        public XmlProperty SelectedPropertyEntry
        {
            get { return _selectedPropertyEntry; }
            set { _selectedPropertyEntry = value; NotifyPropertyChanged("SelectedPropertyEntry"); }
        }


        public ObservableCollection<string> LabelItems { get; set; } = new ObservableCollection<string>();


        private string _selectedLabelItem;

        public string SelectedLabelItem
        {
            get { return _selectedLabelItem; }
            set
            {
                _selectedLabelItem = value;
                SelectedTextEntry = string.Empty;
                SelectedImage = null;
                SelectedPropertyEntry = null;
                if (!string.IsNullOrEmpty(_selectedLabelItem) && !_selectedLabelItem.Contains('+'))
                {
                    var imageFile = SkinInfo.Images.FirstOrDefault(i => i.XmlName.Equals(_selectedLabelItem));
                    if (imageFile != null)
                    {
                        SelectedImage = imageFile;
                    }
                    else if (_selectedLabelItem.StartsWith("#"))
                    {
                        SelectedPropertyEntry = SkinInfo.Properties.FirstOrDefault(p => p.SkinTag == _selectedLabelItem);
                    }
                    else
                    {
                        SelectedTextEntry = _selectedLabelItem;
                    }
                }
                NotifyPropertyChanged("SelectedLabelItem");
            }
        }

        private int _selectedLabelItemIndex;

        public int SelectedLabelItemIndex
        {
            get { return _selectedLabelItemIndex; }
            set { _selectedLabelItemIndex = value;  NotifyPropertyChanged("SelectedLabelItemIndex"); }
        }






    

        public string DisplayLabel
        {
            get
            {
                var displayLabel = string.Empty;
                foreach (var item in LabelItems.Where(l => !l.Equals("+")))
                {
                    var imageFile = SkinInfo.Images.FirstOrDefault(i => i.XmlName.Equals(item));
                    if (imageFile != null)
                    {
                        displayLabel += imageFile.FileName;
                        continue;
                    }

                    if (item.StartsWith("#"))
                    {
                        var prop = SkinInfo.Properties.FirstOrDefault(x => x.SkinTag == item);
                        if (prop != null)
                        {
                            displayLabel += prop.DesignerValue;
                            continue;
                        }
                    }

                    displayLabel += item;
                }
                return displayLabel;
            }
        }


        private string GetLabel()
        {
            return string.Concat(LabelItems);
        }




    

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            CurrentLabel = GetLabel();
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion

        private void Button_ImageAdd_Click(object sender, RoutedEventArgs e)
        {
            if (LabelItems.Count != 0)
            {
                LabelItems.Add("+");
            }
            LabelItems.Add(SelectedImage.XmlName);
            NotifyPropertyChanged("DisplayLabel");
        }

        private void Button_TextAdd_Click(object sender, RoutedEventArgs e)
        {
            if (LabelItems.Count != 0)
            {
                LabelItems.Add("+");
            }
            LabelItems.Add(SelectedTextEntry);
            NotifyPropertyChanged("DisplayLabel");
        }

        private void Button_PropertyAdd_Click(object sender, RoutedEventArgs e)
        {
            if (LabelItems.Count != 0)
            {
                LabelItems.Add("+");
            }
            LabelItems.Add(SelectedPropertyEntry.SkinTag);
            NotifyPropertyChanged("DisplayLabel");
        }

        private void Button_LabelItemRemove_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedLabelItemIndex >= 0 && SelectedLabelItemIndex <= LabelItems.Count - 1)
            {
                var index = SelectedLabelItemIndex;
                if (LabelItems.ElementAtOrDefault(index) == "+")
                {
                    return;
                }

                if (LabelItems.ElementAtOrDefault(index + 1) == "+")
                {
                    LabelItems.RemoveAt(index);
                    LabelItems.RemoveAt(index);
                }
                else
                {
                    LabelItems.RemoveAt(index);
                    if (LabelItems.LastOrDefault() == "+")
                    {
                        LabelItems.RemoveAt(LabelItems.Count - 1);
                    }
                }
            }
            NotifyPropertyChanged("DisplayLabel");
        }

        private void Button_ImageEdit_Click(object sender, RoutedEventArgs e)
        {
            new EditorDialog(new ImagePicker { Width = 700, Height = 600 , SkinInfo = SkinInfo}, false).ShowDialog();
        }

        private void Button_PropertyEdit_Click(object sender, RoutedEventArgs e)
        {
            var editor = new PropertyEditor(SkinInfo);
            if (SelectedPropertyEntry != null)
            {
                editor.SelectedProperty = SelectedPropertyEntry;
            }
            new EditorDialog(editor, false).ShowDialog();
        }
    }
}
