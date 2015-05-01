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


        public override string Title
        {
            get { return "Test Area"; }
        }

        public TestEditorViewSettings Settings 
        {
            get { return EditorSettings as TestEditorViewSettings; }
        }

        private async void Button_Click_LoadSkin(object sender, RoutedEventArgs e)
        {
            var skin = SkinInfo.CreateCopy();
            skin.SkinFolderPath = SkinInfo.SkinFolderPath;
            skin.LoadXmlSkin();

          await  Surface.LoadSkin(skin, new GUISettings
            {
                ConnectionSettings = new ConnectionSettings
                {
                    ConnectionName = "MPDisplay1",
                    Port = 44444,
                    IpAddress = "localhost"
                }
            });

            NotifyPropertyChanged("LabelProperties");
        }

        private void Button_Click_SendProperty(object sender, RoutedEventArgs e)
        {
          //  GUIInfoManager.OnPropertyStringReceived(SelectedProperty, (sender as Button).Tag.ToString());
        }

        private void Button_Click_OpenMpWindow(object sender, RoutedEventArgs e)
        {

        }

        //public ObservableCollection<string> LabelProperties
        //{
        //    get { return GUIInfoManager.RegisteredProperties; }
        //}

        private string _selectedProperty;

        public string SelectedProperty
        {
            get { return _selectedProperty; }
            set { _selectedProperty = value; NotifyPropertyChanged("SelectedProperty"); }
        }
 

        private void Button_Click_SendWindowId(object sender, RoutedEventArgs e)
        {

        }











       
    }
}
