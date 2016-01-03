using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using GUISkinFramework.Skin;
using Microsoft.CSharp;
using MPDisplay.Common.Controls;
// this is added due to the enum PlaybackType:
// ReSharper disable UnusedMember.Local

namespace GUISkinFramework.Editors
{
    /// <summary>
    /// Interaction logic for VisibleConditionEditorDialog.xaml
    /// </summary>
    public partial class VisibleConditionEditorDialog : INotifyPropertyChanged
    {
        private object _instance;
        private string _currentCondition = string.Empty;
        private bool _ischecking;
        private string _errorToolTip;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisibleConditionEditorDialog"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="skininfo">The skin XML</param>
        public VisibleConditionEditorDialog(object instance, XmlSkinInfo skininfo)
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            SkinInfo = skininfo;
            Instance = instance;
            PopulateAutoCompleteList();
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
        public string CurrentCondition
        {
            get { return _currentCondition; }
            set { _currentCondition = value; NotifyPropertyChanged("CurrentCondition"); NotifyPropertyChanged("IsConditionValid"); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is condition valid.
        /// </summary>
        public bool IsConditionValid => AllConditionsValid();

        /// <summary>
        /// Gets or sets the auto complete list.
        /// </summary>
        public ObservableCollection<AutoCompleteEntry> AutoCompleteList { get; set; } = new ObservableCollection<AutoCompleteEntry>();

        public string ErrorToolTip
        {
            get { return _errorToolTip; }
            set { _errorToolTip = value; NotifyPropertyChanged("ErrorToolTip"); }
        }

        private XmlSkinInfo _skinInfo = new XmlSkinInfo();

        public XmlSkinInfo SkinInfo
        {
            get { return _skinInfo; }
            set { _skinInfo = value; NotifyPropertyChanged("SkinInfo"); }
        }
      

        private bool AllConditionsValid()
        {
            if (string.IsNullOrEmpty(_currentCondition))
            {
                ErrorToolTip = string.Empty;
                return false;
            }

            if (_ischecking) return ErrorToolTip == string.Empty;

            _ischecking = true;
            ErrorToolTip = CreateVisibleCondition(_currentCondition);
            _ischecking = false;
            return ErrorToolTip == string.Empty;

         
        }

        /// <summary>
        /// Populates the auto complete list.
        /// </summary>
        private void PopulateAutoCompleteList()
        {
            AutoCompleteList.Add(new AutoCompleteEntry("&&", "&&", "&&", "&", " "));
            AutoCompleteList.Add(new AutoCompleteEntry("||", "||", "||", "|", " "));

            foreach (var option in SkinInfo.SkinOptions)
            {
                AutoCompleteList.Add(new AutoCompleteEntry($"IsSkinOptionEnabled({option.Name})",
                    $"IsSkinOptionEnabled({option.Name})", "skin", "iss", "option"));
                AutoCompleteList.Add(new AutoCompleteEntry($"!IsSkinOptionEnabled({option.Name})",
                    $"!IsSkinOptionEnabled({option.Name})", "!", "!skin", "!iss", "!option"));
            }


            if (_instance is XmlMPWindow)
            {
                AutoCompleteList.Add(new AutoCompleteEntry("IsMediaPortalPreviousWindow(windowId)", "IsMediaPortalPreviousWindow(windowId)", "MP", "ism", "media", "window", "win", "prev"));
                AutoCompleteList.Add(new AutoCompleteEntry("!IsMediaPortalPreviousWindow(windowId)", "!IsMediaPortalPreviousWindow(windowId)", "!", "!MP", "!ism", "!media", "!window", "!win", "!prev"));
                AutoCompleteList.Add(new AutoCompleteEntry("IsMediaPortalWindow(windowId)", "IsMediaPortalWindow(windowId)", "MP", "ism", "media", "window", "win"));
                AutoCompleteList.Add(new AutoCompleteEntry("!IsMediaPortalWindow(windowId)", "!IsMediaPortalWindow(windowId)", "!", "!MP", "!ism", "!media", "!window", "!win"));
            }

            if (_instance is XmlMPDialog)
            {
                AutoCompleteList.Add(new AutoCompleteEntry("IsMediaPortalDialog(windowId)", "IsMediaPortalDialog(windowId)", "MP", "ism", "media", "dialog", "dia"));
                AutoCompleteList.Add(new AutoCompleteEntry("!IsMediaPortalDialog(windowId)", "!IsMediaPortalDialog(windowId)", "!", "!MP", "!ism", "!media", "!dialog", "!dia"));
            }

            if (_instance is XmlPlayerWindow)
            {
                foreach (var playType in Enum.GetNames(typeof(PlaybackType)))
                {
                    AutoCompleteList.Add(new AutoCompleteEntry($"IsPlayer({playType})", $"IsPlayer({playType})", "play", "isp", "player"));
                    AutoCompleteList.Add(new AutoCompleteEntry($"!IsPlayer({playType})", $"!IsPlayer({playType})", "!", "!play", "!isp", "!player"));
                }
            }


            if (!(_instance is XmlControl)) return;

            foreach (var playType in Enum.GetNames(typeof(PlaybackType)))
            {
                AutoCompleteList.Add(new AutoCompleteEntry($"IsPlayer({playType})", $"IsPlayer({playType})", "play", "isp", "player"));
                AutoCompleteList.Add(new AutoCompleteEntry($"!IsPlayer({playType})", $"!IsPlayer({playType})", "!", "!play", "!isp", "!player"));
            }

            foreach (var layout in Enum.GetNames(typeof(XmlListLayout)).Where(x => !x.Equals("Auto")))
            {
                AutoCompleteList.Add(new AutoCompleteEntry($"IsMediaPortalListLayout({layout})",
                    $"IsMediaPortalListLayout({layout})", "MP", "ism", "lay", "list"));
                AutoCompleteList.Add(new AutoCompleteEntry($"!IsMediaPortalListLayout({layout})",
                    $"!IsMediaPortalListLayout({layout})", "!", "!MP", "!ism", "!lay", "!list"));
            }

            AutoCompleteList.Add(new AutoCompleteEntry("IsControlVisible(controlId)", "IsControlVisible(controlId)", "control", "isc", "visible", "vis"));
            AutoCompleteList.Add(new AutoCompleteEntry("!IsControlVisible(controlId)", "!IsControlVisible(controlId)", "!", "!control", "!isc", "!visible", "!vis"));
            AutoCompleteList.Add(new AutoCompleteEntry("IsMediaPortalControlFocused(controlId)", "IsMediaPortalControlFocused(controlId)", "MP", "media", "control", "ism", "focused", "foc"));
            AutoCompleteList.Add(new AutoCompleteEntry("!IsMediaPortalControlFocused(controlId)", "!IsMediaPortalControlFocused(controlId)", "!", "!MP", "!media", "!control", "!ism", "!focused", "!foc"));
            AutoCompleteList.Add(new AutoCompleteEntry("IsPluginEnabled(pluginName)", "IsPluginEnabled(pluginName)", "MP", "media", "control", "ism", "focused", "foc"));
            AutoCompleteList.Add(new AutoCompleteEntry("!IsPluginEnabled(pluginName)", "!IsPluginEnabled(pluginName)", "!", "!MP", "!media", "!control", "!ism", "!focused", "!foc"));
            AutoCompleteList.Add(new AutoCompleteEntry("IsMediaPortalPreviousWindow(windowId)", "IsMediaPortalPreviousWindow(windowId)", "MP", "ism", "media", "window", "win", "prev"));
            AutoCompleteList.Add(new AutoCompleteEntry("!IsMediaPortalPreviousWindow(windowId)", "!IsMediaPortalPreviousWindow(windowId)", "!", "!MP", "!ism", "!media", "!window", "!win", "!prev"));
            AutoCompleteList.Add(new AutoCompleteEntry("IsMediaPortalWindow(windowId)", "IsMediaPortalWindow(windowId)", "MP", "ism", "media", "window", "win"));
            AutoCompleteList.Add(new AutoCompleteEntry("!IsMediaPortalWindow(windowId)", "!IsMediaPortalWindow(windowId)", "!", "!MP", "!ism", "!media", "!window", "!win"));
            AutoCompleteList.Add(new AutoCompleteEntry("IsTvRecording", "IsTvRecording", "tv", "ist", "record", "rec"));
            AutoCompleteList.Add(new AutoCompleteEntry("!IsTvRecording", "!IsTvRecording", "!", "!tv", "!ist", "!record", "!rec"));
            AutoCompleteList.Add(new AutoCompleteEntry("IsMediaPortalConnected", "IsMediaPortalConnected", "MP", "ism", "connect", "con"));
            AutoCompleteList.Add(new AutoCompleteEntry("!IsMediaPortalConnected", "!IsMediaPortalConnected", "!", "!MP", "!ism", "!connect", "!con"));
            AutoCompleteList.Add(new AutoCompleteEntry("IsTVServerConnected", "IsTVServerConnected", "MP", "ist", "connect", "tv"));
            AutoCompleteList.Add(new AutoCompleteEntry("!IsTVServerConnected", "!IsTVServerConnected", "!", "!MP", "!ist", "!connect", "!tv"));
            AutoCompleteList.Add(new AutoCompleteEntry("IsMPDisplayConnected", "IsMPDisplayConnected", "MP", "ist", "connect", "tv"));
            AutoCompleteList.Add(new AutoCompleteEntry("!IsMPDisplayConnected", "!IsMPDisplayConnected", "!", "!MP", "!ist", "!connect", "!tv"));

            AutoCompleteList.Add(new AutoCompleteEntry("IsMultiSeatInstall", "IsMultiSeatInstall", "Is", "ism", "inst", "mul"));
            AutoCompleteList.Add(new AutoCompleteEntry("!IsMultiSeatInstall", "!IsMultiSeatInstall", "!", "!Is", "!ism", "!inst", "!mul"));
        }


        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
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

        private CompilerParameters _compilerParams;
        private CSharpCodeProvider _codeProvider;
        private bool _compilerLoaded;

        public string ValidateCondition(string condition)
        {
            if (string.IsNullOrEmpty(condition)) return string.Empty;

            LoadCompilerSettings();
            var code = VisibleClassString.Replace(ReplacementString, condition);
            var compileResults = _codeProvider.CompileAssemblyFromSource(_compilerParams, code);
            return compileResults.Errors.HasErrors ? compileResults.Errors.Cast<CompilerError>().Aggregate(string.Empty, (current, item) => current + item.ErrorText + Environment.NewLine) : string.Empty;
        }


        public static string FullReference(string relativeReference)
        {
            // First, get the path for this executing assembly.
            var a = Assembly.GetExecutingAssembly();
            var path = Path.GetDirectoryName(a.Location);

            // if the file exists in this Path - prepend the path
            if (path == null) return string.Empty;

            var fullReference = Path.Combine(path, relativeReference);
            if (File.Exists(fullReference)) return fullReference;

            // Strip off any trailing ".dll" if present.
            fullReference = String.Compare(relativeReference.Substring(relativeReference.Length - 4), ".dll", StringComparison.OrdinalIgnoreCase) == 0 ? relativeReference.Substring(0, relativeReference.Length - 4) : relativeReference;

            // See if the required assembly is already present in our current AppDomain
            foreach (var currAssembly in AppDomain.CurrentDomain.GetAssemblies().Where(currAssembly => String.Compare(currAssembly.GetName().Name, fullReference, StringComparison.OrdinalIgnoreCase) == 0))
            {
                // Found it, return the location as the full reference.
                return currAssembly.Location;
            }

            // The assembly isn't present in our current application, so attempt to
            // load it from the GAC, using the partial name.
            try
            {
                var tempAssembly = Assembly.Load(fullReference);
                return tempAssembly.Location;
            }
            catch
            {
                // If we cannot load or otherwise access the assembly from the GAC then just
                // return the relative reference and hope for the best.
                return relativeReference;
            }
        }

        private void LoadCompilerSettings()
        {
            if (_compilerLoaded) return;

            _compilerParams = new CompilerParameters
            {
                GenerateInMemory = true,
                TreatWarningsAsErrors = false,
                GenerateExecutable = false,
                CompilerOptions = "/optimize"
            };
            string[] references = { "System.dll", FullReference("GUIFramework.dll") };
            _compilerParams.ReferencedAssemblies.AddRange(references);
            _codeProvider = new CSharpCodeProvider();
            _compilerLoaded = true;
        }

        private const string ReplacementString = "8AC9ED14-B51F-401D-8CFF-87469ED062ED";

        /// <summary>
        /// The class we will generate to handle the controls visibility
        /// </summary>
        private const string VisibleClassString = @"using System;
          using System.Collections.Generic;
          using GUIFramework.Managers;
          namespace Visibility
          {
             public class VisibleMethodClass
             {
                public static bool ShouldBeVisible()
                {
                   if (8AC9ED14-B51F-401D-8CFF-87469ED062ED)
                   {
                       return true;
                   }
                   return false;
                }
             }
          }";

        // ReSharper disable InconsistentNaming
        // attention: this enum must match APIPlaybackType in MessageFramework exactly!
        private enum PlaybackType
        {
           None = 0,
            IsTV,
            IsCDA,
            IsDVD,
            IsVideo,
            IsMusic,
            IsRadio,
            IsTVRecording,
            IsPlugin,
            MyFilms,
            MovingPictures,
            MPTVSeries,
            mvCentral,
            YoutubeFm,
            OnlineVideos,
            MyAnime,
            Rockstar,
            PandoraMusicBox,
            RadioTime,
            Streamradio,
            TuneIn
        }


        public  string CreateVisibleCondition(string xmlVisibleString)
        {
            xmlVisibleString = xmlVisibleString.Replace("++", "&&");
            xmlVisibleString = xmlVisibleString.Replace("IsControlVisible(", "GUIVisibilityManager.IsControlVisible(" + 0 + ",");

            if (xmlVisibleString.Contains("IsPlayer("))
            {
                xmlVisibleString = Enum.GetValues(typeof (PlaybackType)).Cast<object>().Aggregate(xmlVisibleString, (current, playtype) => current.Replace(
                    $"IsPlayer({playtype})", $"GUIVisibilityManager.IsPlayer({(int) playtype})"));
            }

            if (xmlVisibleString.Contains("IsMediaPortalListLayout("))
            {
                xmlVisibleString = Enum.GetValues(typeof (XmlListLayout)).Cast<object>().Aggregate(xmlVisibleString, (current, layout) => current.Replace(
                    $"IsMediaPortalListLayout({layout})",
                    $"GUIVisibilityManager.IsMediaPortalListLayout({(int) layout})"));
            }

            if (xmlVisibleString.Contains("IsPluginEnabled("))
            {
                // \(([^)]*)\)
                xmlVisibleString = Regex.Replace(xmlVisibleString, @"IsPluginEnabled\(([^)]*)\)", @"GUIVisibilityManager.IsPluginEnabled(""$1"")");
                //xmlVisibleString.Replace("IsPluginEnabled(", "GUIVisibilityManager.IsPluginEnabled(");
            }

            if (xmlVisibleString.Contains("IsSkinOptionEnabled("))
            {
                // \(([^)]*)\)
                xmlVisibleString = Regex.Replace(xmlVisibleString, @"IsSkinOptionEnabled\(([^)]*)\)", @"GUIVisibilityManager.IsSkinOptionEnabled(""$1"")");
            }


            xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalConnected", "GUIVisibilityManager.IsMediaPortalConnected()");
            xmlVisibleString = xmlVisibleString.Replace("IsTVServerConnected", "GUIVisibilityManager.IsTVServerConnected()");
            xmlVisibleString = xmlVisibleString.Replace("IsMPDisplayConnected", "GUIVisibilityManager.IsMPDisplayConnected()");
            xmlVisibleString = xmlVisibleString.Replace("IsTvRecording", "GUIVisibilityManager.IsTvRecording()");
            xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalWindow(", "GUIVisibilityManager.IsMediaPortalWindow(");
            xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalDialog(", "GUIVisibilityManager.IsMediaPortalDialog(");
            xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalPreviousWindow(", "GUIVisibilityManager.IsMediaPortalPreviousWindow(");

            xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalControlFocused(", "GUIVisibilityManager.IsMediaPortalControlFocused(");
            xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalControlVisible(", "GUIVisibilityManager.IsMediaPortalControlVisible(");
            xmlVisibleString = xmlVisibleString.Replace("IsMultiSeatInstall", "GUIVisibilityManager.IsMultiSeatInstall()");
 
            //GUIWindowManager
            return !string.IsNullOrWhiteSpace(xmlVisibleString) ? ValidateCondition(xmlVisibleString) : string.Empty;
        }

    }

}
