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

namespace SkinEditor.Views
{
    /// <summary>
    /// Interaction logic for StyleEditorView.xaml
    /// </summary>
    public partial class TestEditorView : EditorViewModel
    {
      
        public TestEditorView()
        {
            InitializeComponent();
        }


        public override string Title
        {
            get { return "Test Area"; }
        }

        public override void Initialize()
        {
            base.Initialize();
         
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

          await  surface.LoadSkin(skin, new GUISettings
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
