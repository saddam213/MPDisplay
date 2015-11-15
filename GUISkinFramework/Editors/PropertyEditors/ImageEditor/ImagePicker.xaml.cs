using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Common.Helpers;
using GUISkinFramework.Skin;
using Microsoft.VisualBasic.FileIO;
using SearchOption = System.IO.SearchOption;

namespace GUISkinFramework.Editors
{
    /// <summary>
    /// Interaction logic for ImagePicker.xaml
    /// </summary>
    public partial class ImagePicker : INotifyPropertyChanged
    {
        private readonly List<string> _allowedsExtensions = new List<string>(new[] { ".BMP", ".JPG", ".GIF", ".PNG" });
        private const string ImageFilter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG";
        private FileSystemWatcher _imageWatcher;
        private ObservableCollection<ImageFile> _imageFiles = new ObservableCollection<ImageFile>();
        private ImageFile _selectedImage;

        public ImagePicker()
        {
            InitializeComponent();
            CreateContextMenu();
        }


        public XmlSkinInfo SkinInfo
        {
            get { return (XmlSkinInfo)GetValue(SkinInfoProperty); }
            set { SetValue(SkinInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SkinInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SkinInfoProperty =
            DependencyProperty.Register("SkinInfo", typeof(XmlSkinInfo), typeof(ImagePicker), new PropertyMetadata(new XmlSkinInfo(), OnSkinInfoChanged));

        private static void OnSkinInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var _this = d as ImagePicker;
            if (_this == null) return;

            var xmlSkinInfo = e.NewValue as XmlSkinInfo;
            if (xmlSkinInfo != null) _this.GetFolderImages(xmlSkinInfo.SkinImageFolder);
            _this.CreateFileWatcher();
        }


        public XmlImageFile SelectedXmlImage
        {
            get { return (XmlImageFile)GetValue(SelectedXmlImageProperty); }
            set { SetValue(SelectedXmlImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedXmlImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedXmlImageProperty =
            DependencyProperty.Register("SelectedXmlImage", typeof(XmlImageFile), typeof(ImagePicker)
            , new PropertyMetadata(new XmlImageFile(), OnSelectedXmlImageChanged));

        private static void OnSelectedXmlImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;

            var xmlImage = e.NewValue as XmlImageFile;
            var _this = d as ImagePicker;

            if (xmlImage != null)
            {
                _this?.GetFolderImages(Path.GetDirectoryName(xmlImage.FileName));
            }

            var image = _this?.ImageFiles.Where(f => !f.IsFolder).FirstOrDefault(f => xmlImage != null && f.File.FullName == xmlImage.FileName);

            if (image == null) return;

            _this.GetFolderImages(image.Directory.FullName);
            _this.SelectedImage = _this.ImageFiles.FirstOrDefault(f => f.FileName == image.FileName);
        }



        public ObservableCollection<ImageFile> ImageFiles
        {
            get { return _imageFiles; }
            set { _imageFiles = value; }
        }

        public ImageFile SelectedImage
        {
            get { return _selectedImage; }
            set
            {
                _selectedImage = value;
                SetXmlImage(_selectedImage);
                ListBox.ScrollIntoView(_selectedImage);
                NotifyPropertyChanged("SelectedImage");
            }
        }

        private void SetXmlImage(ImageFile image)
        {
            if (image == null || image.IsFolder) return;

            if (SelectedXmlImage == null || SelectedImage.FileName != SelectedXmlImage.FileName)
            {
                SelectedXmlImage = SkinInfo.Images.FirstOrDefault(f => f.FileName == SelectedImage.FileName);
            }
        }


        public ObservableCollection<object> FolderMenu { get; set; } = new ObservableCollection<object>();

        public ObservableCollection<object> ImageMenu { get; set; } = new ObservableCollection<object>();

        public string ImageRootPath => GetDirectoryPath(SkinInfo.SkinImageFolder);

        private string _currentFolder = string.Empty;

        public string CurrentFolder
        {
            get { return _currentFolder; }
            set { _currentFolder = value; NotifyPropertyChanged("CurrentFolder"); }
        }
        
        private bool IsParentFolder(string folder)
        {
            return  GetDirectoryPath(folder).Equals(GetDirectoryPath(ImageRootPath));
        }

        public string GetDirectoryPath(string dir)
        {
            return Directory.Exists(dir) ? new DirectoryInfo(dir).FullName.Trim('\\') : string.Empty;
        }

        private void GetFolderImages(string path)
        {
            _imageFiles.Clear();
            if (!Directory.Exists(path)) return;

            if (!IsParentFolder(path))
            {
                _imageFiles.Add(new ImageFile { IsBack = true, Directory = new DirectoryInfo(GetDirectoryPath(path)), IsFolder = true });
            }
            foreach (var subFolder in Directory.GetDirectories(path))
            {
                _imageFiles.Add(new ImageFile { Directory = new DirectoryInfo(subFolder), IsFolder = true });
            }
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var imageFile in Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly))
            {
                var extension = Path.GetExtension(imageFile);
                if (extension != null && _allowedsExtensions.Contains(extension.ToUpper()))
                {
                    _imageFiles.Add(new ImageFile { Directory = new DirectoryInfo(path), File = new FileInfo(imageFile) });
                }
            }
            _currentFolder = GetDirectoryPath( path);
        }

        private void ListBox_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            if (SelectedImage == null || !SelectedImage.IsFolder) return;
            if (SelectedImage.IsBack && !IsParentFolder(SelectedImage.Directory.FullName))
            {
                if (SelectedImage.Directory.Parent != null) GetFolderImages(SelectedImage.Directory.Parent.FullName);
                return;
            }

            GetFolderImages(SelectedImage.Directory.FullName);
        }

        public void CreateFileWatcher()
        {
            _imageWatcher?.Dispose();

            if (!Directory.Exists(SkinInfo.SkinImageFolder)) return;
            _imageWatcher = new FileSystemWatcher(SkinInfo.SkinImageFolder)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true
            };
            _imageWatcher.Created += (s, e) => ReloadCurrentFolder();
            _imageWatcher.Deleted += (s, e) => ReloadCurrentFolder();
            _imageWatcher.Renamed += (s, e) => ReloadCurrentFolder();
        }

        private void ReloadCurrentFolder()
        {
            _imageWatcher.EnableRaisingEvents = false;
            Dispatcher.Invoke(DispatcherPriority.Background, (Action)delegate
            {
                GetFolderImages(_currentFolder);
                SkinInfo.ReloadImages();
            });
            _imageWatcher.EnableRaisingEvents = true;
        }

        #region ContextMenu

        public void CreateContextMenu()
        {
            ImageMenu.Clear();
            ImageMenu.Add(CreateContextMenuItem("Add Image", "Add", () => AddImage(false)));
            ImageMenu.Add(CreateContextMenuItem("Add Folder", "New", () => AddImage(true)));
            ImageMenu.Add(new Separator());
            ImageMenu.Add(CreateContextMenuItem("Open", "XmlImage", () => OpenImage(false)));
            ImageMenu.Add(CreateContextMenuItem("Open Location", "Open", () => OpenImage(true)));
            ImageMenu.Add(CreateContextMenuItem("Delete", "Delete", () => DeleteImage(false)));

            FolderMenu.Clear();
            FolderMenu.Add(CreateContextMenuItem("Open", "Open", () => OpenImage(true)));
            FolderMenu.Add(CreateContextMenuItem("Delete", "Delete", () => DeleteImage(true)));
        }

        private static MenuItem CreateContextMenuItem(string header, string icon, Action handler)
        {
            var menuItem = new MenuItem
            {
                Header = header,
                Icon =
                    new Image
                    {
                        Margin = new Thickness(2),
                        Stretch = Stretch.Uniform,
                        Width = 16,
                        Height = 16,
                        Source =
                            new BitmapImage(new Uri($@"/GUISkinFramework;component/Images/{icon}.png",
                                UriKind.RelativeOrAbsolute))
                    }
            };
            if (handler != null)
            {
                menuItem.Click += (s, e) => handler();
            }
            return menuItem;
        }

        private void OpenImage(bool isfolder)
        {
            if (SelectedImage == null) return;
            if (isfolder)
            {
                Process.Start("explorer.exe", SelectedImage.Directory.FullName);
                return;
            }
            Process.Start(SelectedImage.File.FullName);
        }

        private void AddImage(bool isfolder)
        {
            if (isfolder)
            {
                var folder = DirectoryHelpers.FolderBrowserDialog();
                if (!string.IsNullOrEmpty(folder))
                {
                    FileSystem.CopyDirectory(folder, Path.Combine(_currentFolder, new DirectoryInfo(folder).Name), UIOption.AllDialogs, UICancelOption.DoNothing);
                }
                return;
            }
            var file = FileHelpers.OpenFileDialog(SkinInfo.SkinImageFolder, ImageFilter);
            if (!string.IsNullOrEmpty(file))
            {
                FileHelpers.CopyFile(file, Path.Combine(_currentFolder, Path.GetFileName(file)));
            }
        }

        private void DeleteImage(bool isfolder)
        {
            if (SelectedImage == null) return;
            if (MessageBox.Show(
                $"Are you sure you want to permanently delete this {(isfolder ? "folder" : "image")}?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) !=
                MessageBoxResult.Yes) return;
            if (isfolder)
            {
                DirectoryHelpers.TryDelete(SelectedImage.Directory.FullName);
                return;
            }
            FileHelpers.TryDelete(SelectedImage.File.FullName);
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
     
        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion
    }

    public class ImageFile : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is back.
        /// </summary>
        public bool IsBack { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is folder.
        /// </summary>
        public bool IsFolder { get; set; }

        /// <summary>
        /// Gets or sets the directory.
        /// </summary>
        public DirectoryInfo Directory { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        public FileInfo File { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName
        {
            get { return IsBack ? ".." : IsFolder ? Directory.Name : File.Name; }
            // ReSharper disable once ValueParameterNotUsed
            set { NotifyPropertyChanged("DisplayName"); }
        }

        public string ToolTip => IsBack ? "Previous Folder" : IsFolder ?
            string.Format("Folder:{1}{0}", Directory.FullName, Environment.NewLine)
            : string.Format("Image Type: {1} Image{0}Size: {2:0.##} KB{0}FileName: {3}",
                Environment.NewLine, File.Extension.Replace(".", "").ToUpper(), (File.Length / 1024.0), File.FullName);

        public string FileName
        {
            get
            {
                if (!IsFolder && File != null)
                {
                    return File.FullName;
                }
                return string.Empty;
            }
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
