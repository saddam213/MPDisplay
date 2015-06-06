using System.IO;
using System.Linq;
using System.Windows;

namespace SkinEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static string _startupSkinInfoFilename = string.Empty;
        public static string StartupSkinInfoFilename
        {
            get { return _startupSkinInfoFilename; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
         
            base.OnStartup(e);
            if (!e.Args.Any()) return;

            var skinInfoFile = e.Args[0];
            if (File.Exists(skinInfoFile))
            {
                _startupSkinInfoFilename = skinInfoFile;
            }
        }
    }
}
