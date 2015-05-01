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

namespace GUISkinFramework.Editors
{
    /// <summary>
    /// Interaction logic for VisibleConditionEditorDialog.xaml
    /// </summary>
    public partial class VisibleConditionEditorDialog : Window, INotifyPropertyChanged
    {
        private object _instance;
        private string _currentCondition = string.Empty;
        private ObservableCollection<AutoCompleteEntry> _autoCompleteList = new ObservableCollection<AutoCompleteEntry>();
        private bool _ischecking = false;
        private string _errorToolTip;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisibleConditionEditorDialog"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
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
        public bool IsConditionValid
        {
            get { return AllConditionsValid(); }
        }

        /// <summary>
        /// Gets or sets the auto complete list.
        /// </summary>
        public ObservableCollection<AutoCompleteEntry> AutoCompleteList
        {
            get { return _autoCompleteList; }
            set { _autoCompleteList = value; }
        }

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

            if (!_ischecking)
            {
                _ischecking = true;
                ErrorToolTip = CreateVisibleCondition(_currentCondition);
                _ischecking = false;
            }
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
                AutoCompleteList.Add(new AutoCompleteEntry(string.Format("IsSkinOptionEnabled({0})", option.Name), string.Format("IsSkinOptionEnabled({0})", option.Name), "skin", "iss", "option"));
                AutoCompleteList.Add(new AutoCompleteEntry(string.Format("!IsSkinOptionEnabled({0})", option.Name), string.Format("!IsSkinOptionEnabled({0})", option.Name), "!", "!skin", "!iss", "!option"));
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
                    AutoCompleteList.Add(new AutoCompleteEntry(string.Format("IsPlayer({0})", playType), string.Format("IsPlayer({0})", playType), "play", "isp", "player"));
                    AutoCompleteList.Add(new AutoCompleteEntry(string.Format("!IsPlayer({0})", playType), string.Format("!IsPlayer({0})", playType), "!", "!play", "!isp", "!player"));
                }
            }


            if (_instance is XmlControl)
            {

                foreach (var playType in Enum.GetNames(typeof(PlaybackType)))
                {
                    AutoCompleteList.Add(new AutoCompleteEntry(string.Format("IsPlayer({0})", playType), string.Format("IsPlayer({0})", playType), "play", "isp", "player"));
                    AutoCompleteList.Add(new AutoCompleteEntry(string.Format("!IsPlayer({0})", playType), string.Format("!IsPlayer({0})", playType), "!", "!play", "!isp", "!player"));
                }

                foreach (var layout in Enum.GetNames(typeof(XmlListLayout)).Where(x => !x.Equals("Auto")))
                {
                    AutoCompleteList.Add(new AutoCompleteEntry(string.Format("IsMediaPortalListLayout({0})", layout), string.Format("IsMediaPortalListLayout({0})", layout), "MP", "ism", "lay", "list"));
                    AutoCompleteList.Add(new AutoCompleteEntry(string.Format("!IsMediaPortalListLayout({0})", layout), string.Format("!IsMediaPortalListLayout({0})", layout), "!", "!MP", "!ism", "!lay", "!list"));
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
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion




        private CompilerParameters _compilerParams;
        private CSharpCodeProvider _codeProvider;
        private bool _compilerLoaded = false;




        public string ValidateCondition(string condition)
        {
            if (!string.IsNullOrEmpty(condition))
            {
                LoadCompilerSettings();
                string code = _visibleClassString.Replace(_replacementString, condition);
                CompilerResults compileResults = _codeProvider.CompileAssemblyFromSource(_compilerParams, code);
                if (compileResults.Errors.HasErrors)
                {
                    string error = string.Empty;
                    foreach (CompilerError item in compileResults.Errors)
                    {
                        error += item.ErrorText + Environment.NewLine;
                    }
                    return error;
                }
            }
            return string.Empty;
        }


        public static string FullReference(string relativeReference)
        {
            // First, get the path for this executing assembly.
            Assembly a = Assembly.GetExecutingAssembly();
            string path = Path.GetDirectoryName(a.Location);

            // if the file exists in this Path - prepend the path
            string fullReference = Path.Combine(path, relativeReference);
            if (File.Exists(fullReference))
                return fullReference;
            else
            {
                // Strip off any trailing ".dll" if present.
                if (string.Compare(relativeReference.Substring(relativeReference.Length - 4),".dll", true) == 0)
                    fullReference = relativeReference.Substring(0, relativeReference.Length - 4);
                else
                    fullReference = relativeReference;

                // See if the required assembly is already present in our current AppDomain
                foreach (Assembly currAssembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (string.Compare(currAssembly.GetName().Name, fullReference, true) == 0)
                    {
                        // Found it, return the location as the full reference.
                        return currAssembly.Location;
                    }
                }

                // The assembly isn't present in our current application, so attempt to
                // load it from the GAC, using the partial name.
                try
                {
                    Assembly tempAssembly = Assembly.Load(fullReference);
                    return tempAssembly.Location;
                }
                catch
                {
                    // If we cannot load or otherwise access the assembly from the GAC then just
                    // return the relative reference and hope for the best.
                    return relativeReference;
                }
            }
        }

        private void LoadCompilerSettings()
        {
            if (!_compilerLoaded)
            {
                _compilerParams = new CompilerParameters();
                _compilerParams.GenerateInMemory = true;
                _compilerParams.TreatWarningsAsErrors = false;
                _compilerParams.GenerateExecutable = false;
                _compilerParams.CompilerOptions = "/optimize";
                string[] references = { "System.dll", FullReference("GUIFramework.dll") };
                _compilerParams.ReferencedAssemblies.AddRange(references);
                _codeProvider = new CSharpCodeProvider();
                _compilerLoaded = true;
            }
        }

        private string _replacementString = "8AC9ED14-B51F-401D-8CFF-87469ED062ED";

        /// <summary>
        /// The class we will generate to handle the controls visibility
        /// </summary>
        private string _visibleClassString =
        @"using System;
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
            TuneIn,
        }


        public  string CreateVisibleCondition(string xmlVisibleString)
        {
            xmlVisibleString = xmlVisibleString.Replace("++", "&&");
            xmlVisibleString = xmlVisibleString.Replace("IsControlVisible(", "GUIVisibilityManager.IsControlVisible(" + 0 + ",");

            if (xmlVisibleString.Contains("IsPlayer("))
            {
                foreach (var playtype in Enum.GetValues(typeof(PlaybackType)))
                {
                    xmlVisibleString = xmlVisibleString.Replace(string.Format("IsPlayer({0})", playtype), string.Format("GUIVisibilityManager.IsPlayer({0})", (int)playtype));
                }
            }

            if (xmlVisibleString.Contains("IsMediaPortalListLayout("))
            {
                foreach (var layout in Enum.GetValues(typeof(XmlListLayout)))
                {
                    xmlVisibleString = xmlVisibleString.Replace(string.Format("IsMediaPortalListLayout({0})", layout), string.Format("GUIVisibilityManager.IsMediaPortalListLayout({0})", (int)layout));
                }
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
            if (!string.IsNullOrWhiteSpace(xmlVisibleString))
            {
                return ValidateCondition(xmlVisibleString);
            }
            return string.Empty;
        }

     


    }

  
}
