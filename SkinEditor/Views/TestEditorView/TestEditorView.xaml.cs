using System.Windows;
using Common.Helpers;
using Common.Settings;

namespace SkinEditor.Views
{
    /// <summary>
    /// Interaction logic for StyleEditorView.xaml
    /// </summary>
    public partial class TestEditorView
    {
      
        public TestEditorView()
        {
            InitializeComponent();
        }

        public override string Title => "Test Area";

        public TestEditorViewSettings Settings => EditorSettings as TestEditorViewSettings;

        private async void Button_Click_LoadSkin(object sender, RoutedEventArgs e)
        {
            var skin = SkinInfo.CreateCopy();
            skin.SkinFolderPath = SkinInfo.SkinFolderPath;
            skin.LoadXmlSkin();

          await  Surface.LoadSkin(skin, new GUISettings
            {
                ConnectionSettings = new ConnectionSettings
                {
                    Port = 44444,
                    IpAddress = "localhost"
                }
            });

            NotifyPropertyChanged("LabelProperties");
        }

        private string _selectedProperty;

        public string SelectedProperty
        {
            get { return _selectedProperty; }
            set { _selectedProperty = value; NotifyPropertyChanged("SelectedProperty"); }
        }
    }
}
