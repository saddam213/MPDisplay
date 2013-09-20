using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using GUIFramework.GUI;
using GUIFramework.GUI.Windows;
using GUISkinFramework.Common;
using GUISkinFramework.Controls;
using MessageFramework.DataObjects;
using Microsoft.CSharp;
using MPDisplay.Common;
using MPDisplay.Common.Log;

namespace GUIFramework.Managers
{
    public enum VisibleMessageType
    {
        ControlVisibilityChanged,
        WindowVisibilityChanged,
        GlobalVisibilityChanged
    }

    public static class GUIVisibilityManager   
    {
        private static Log Log = LoggingManager.GetLog(typeof(GUIVisibilityManager));
        private static Dictionary<int, Dictionary<int, bool>> _controlVisibilityMap = new Dictionary<int, Dictionary<int, bool>>();
        private static MessengerService<VisibleMessageType> _visibilityService = new MessengerService<VisibleMessageType>();
        public static MessengerService<VisibleMessageType> VisibilityService
        {
            get { return _visibilityService; }
        }

        public static void RegisterMessage(VisibleMessageType action, Action callback)
        {
            VisibilityService.Register(action, callback);
        }

        public static void DeregisterMessage(VisibleMessageType action, object owner)
        {
            VisibilityService.Deregister(action, owner);
        }

        public static void NotifyVisibilityChanged(VisibleMessageType action)
        {
            if (action == VisibleMessageType.GlobalVisibilityChanged)
            {
                VisibilityService.NotifyListeners(VisibleMessageType.GlobalVisibilityChanged);
                VisibilityService.NotifyListeners(VisibleMessageType.ControlVisibilityChanged);
                VisibilityService.NotifyListeners(VisibleMessageType.WindowVisibilityChanged);
            }
            else
            {
                VisibilityService.NotifyListeners(action);
            }
        }

        public static void RegisterControlVisibility(IControlHost controlHost)
        {
            DeregisterControlVisibility(controlHost);
            lock (_controlVisibilityMap)
            {
                _controlVisibilityMap.Add(controlHost.Id, controlHost.Controls.GetControls().DistinctBy(c => c.Id).ToDictionary(k => k.Id, v => v.IsWindowOpenVisible));
            }
        }

        public static void DeregisterControlVisibility(IControlHost controlHost)
        {
            lock (_controlVisibilityMap)
            {
                if (_controlVisibilityMap.ContainsKey(controlHost.Id))
                {
                    _controlVisibilityMap.Remove(controlHost.Id);
                }
            }
        }

        public static void UpdateControlVisibility(GUIControl control)
        {
            if (control != null)
            {
                if (_controlVisibilityMap.ContainsKey(control.ParentId))
                {
                    if (_controlVisibilityMap[control.ParentId].ContainsKey(control.Id))
                    {
                        if (_controlVisibilityMap[control.ParentId][control.Id] != control.IsControlVisible)
                        {
                            lock (_controlVisibilityMap)
                            {
                                _controlVisibilityMap[control.ParentId][control.Id] = control.IsControlVisible;
                            }
                            NotifyVisibilityChanged(VisibleMessageType.ControlVisibilityChanged);
                        }
                    }
                }
            }
        }

        #region Condition Checks

     
        public static bool IsControlVisible(int windowId, int controlId)
        {
            if (_controlVisibilityMap.ContainsKey(windowId) && _controlVisibilityMap[windowId].ContainsKey(controlId))
            {
                return _controlVisibilityMap[windowId][controlId];
            }
            return false;
        }

        public static bool IsMediaPortalWindow(int windowId)
        {
            return windowId == InfoRepository.Instance.WindowId;
        }

        public static bool IsMediaPortalDialog(int dialogId)
        {
            return dialogId == InfoRepository.Instance.DialogId;
        }

        public static bool IsMediaPortalPreviousWindow(int windowId)
        {
            return windowId == InfoRepository.Instance.PreviousWindowId;
        }

        public static bool IsMediaPortalControlFocused(int controlId)
        {
            return controlId == InfoRepository.Instance.FocusedWindowControlId;
        }

        public static bool IsPluginEnabled(string pluginName)
        {
            return InfoRepository.Instance.EnabledPluginMap.Any(plugin => plugin.ToLower() == pluginName.ToLower());
        }

        public static bool IsPlayer(int playerId)
        {
            return playerId.Equals((int)InfoRepository.Instance.PlayerType);
        }

        public static bool IsTvRecording()
        {
            return InfoRepository.Instance.IsTvRecording;
        }

        public static bool IsFullscreenVideo(bool value)
        {
            return value == InfoRepository.Instance.IsFullscreenVideo;
        }

        public static bool IsMediaPortalConnected()
        {
            return InfoRepository.Instance.IsMediaPortalConnected;
        }

        public static bool IsTVServerConnected()
        {
            return InfoRepository.Instance.IsTVServerConnected;
        }

        public static bool IsMPDisplayConnected()
        {
            return InfoRepository.Instance.IsMPDisplayConnected;
        }

        public static bool IsSkinOptionEnabled(string option)
        {
            return InfoRepository.Instance.IsSkinOptionEnabled(option);
        }

        public static bool IsMediaPortalListLayout(int layout)
        {
            return (int)ListRepository.GetCurrentMediaPortalListLayout() == layout;
        }

        #endregion

        #region Condition Parser/Compiler

        private static CompilerParameters _compilerParams;
        private static CSharpCodeProvider _codeProvider;
        private static bool _compilerLoaded = false;

        public static Func<bool> CreateVisibleCondition(GUIControl control, string xmlVisibleString)
        {
            xmlVisibleString = xmlVisibleString.Replace("++", "&&");

            if (xmlVisibleString.Contains("IsSkinOptionEnabled("))
            {
                // \(([^)]*)\)
                xmlVisibleString = Regex.Replace(xmlVisibleString, @"IsSkinOptionEnabled\(([^)]*)\)", @"GUIVisibilityManager.IsSkinOptionEnabled(""$1"")");
                //xmlVisibleString.Replace("IsPluginEnabled(", "GUIVisibilityManager.IsPluginEnabled(");
            }

            xmlVisibleString = xmlVisibleString.Replace("IsControlVisible(", "GUIVisibilityManager.IsControlVisible(" + control.ParentId + ",");

            if (xmlVisibleString.Contains("IsPlayer("))
            {
                foreach (var playtype in Enum.GetValues(typeof(APIPlaybackType)))
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
                xmlVisibleString = Regex.Replace(xmlVisibleString, @"IsPluginEnabled\(([^)]*)\)", @"GUIVisibilityManager.IsPluginEnabled(""$1"")");
            }

            xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalConnected", "GUIVisibilityManager.IsMediaPortalConnected()");
            xmlVisibleString = xmlVisibleString.Replace("IsTVServerConnected", "GUIVisibilityManager.IsTVServerConnected()");
            xmlVisibleString = xmlVisibleString.Replace("IsMPDisplayConnected", "GUIVisibilityManager.IsMPDisplayConnected()");
            xmlVisibleString = xmlVisibleString.Replace("IsTvRecording", "GUIVisibilityManager.IsTvRecording()");
            xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalControlFocused(", "GUIVisibilityManager.IsMediaPortalControlFocused(");
            xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalWindow(", "GUIVisibilityManager.IsMediaPortalWindow(");
            xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalDialog(", "GUIVisibilityManager.IsMediaPortalDialog(");
            xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalPreviousWindow(", "GUIVisibilityManager.IsMediaPortalPreviousWindow(");


            //GUIWindowManager
            if (!string.IsNullOrWhiteSpace(xmlVisibleString))
            {
                return CompileCondition(control.Id, control.ParentId, xmlVisibleString);
            }
            return null;
        }


        public static Func<bool> CreateVisibleCondition(GUIWindow window, string xmlVisibleString)
        {
            xmlVisibleString = xmlVisibleString.Replace("++", "&&");
            if (xmlVisibleString.Contains("IsSkinOptionEnabled("))
            {
                xmlVisibleString = Regex.Replace(xmlVisibleString, @"IsSkinOptionEnabled\(([^)]*)\)", @"GUIVisibilityManager.IsSkinOptionEnabled(""$1"")");

            }

            if (window is GUIPlayerWindow)
            {
                if (xmlVisibleString.Contains("IsPlayer("))
                {
                    foreach (var playtype in Enum.GetValues(typeof(APIPlaybackType)))
                    {
                        xmlVisibleString = xmlVisibleString.Replace(string.Format("IsPlayer({0})", playtype), string.Format("GUIVisibilityManager.IsPlayer({0})", (int)playtype));
                    }
                }
            }

            if (window is GUIMPWindow)
            {
                xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalWindow(", "GUIVisibilityManager.IsMediaPortalWindow(");
                xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalDialog(", "GUIVisibilityManager.IsMediaPortalDialog(");
                xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalPreviousWindow(", "GUIVisibilityManager.IsMediaPortalPreviousWindow(");
            }

            //GUIWindowManager
            if (!string.IsNullOrWhiteSpace(xmlVisibleString))
            {
                return CompileCondition(-1, window.Id, xmlVisibleString);
            }
            return null;
        }

        public static Func<bool> CreateVisibleCondition(GUIDialog dialog, string xmlVisibleString)
        {
            xmlVisibleString = xmlVisibleString.Replace("++", "&&");
            if (xmlVisibleString.Contains("IsSkinOptionEnabled("))
            {
                xmlVisibleString = Regex.Replace(xmlVisibleString, @"IsSkinOptionEnabled\(([^)]*)\)", @"GUIVisibilityManager.IsSkinOptionEnabled(""$1"")");

            }

            xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalDialog(", "GUIVisibilityManager.IsMediaPortalDialog(");

            //GUIWindowManager
            if (!string.IsNullOrWhiteSpace(xmlVisibleString))
            {
                return CompileCondition(-1, dialog.Id, xmlVisibleString);
            }
            return null;
        }

        /// <summary>
        /// Creates a complex visible condition.
        /// </summary>
        /// <param name="xmlVisibleString">The XML visible string.</param>
        /// <returns></returns>
        private static Func<bool> CompileCondition(int controlId, int windowId,string xmlVisibleString)
        {
            if (!string.IsNullOrEmpty(xmlVisibleString))
            {
                LoadCompilerSettings();
                string code = _visibleClassString.Replace(_replacementString, xmlVisibleString);
                CompilerResults compileResults = _codeProvider.CompileAssemblyFromSource(_compilerParams, code);
                if (compileResults.Errors.HasErrors)
                {
                    if (controlId == -1)
                    {
                        Log.Message(LogLevel.Error, "Invalid Window VisibleCondition Detected, WindowId: {0}, VisibleString: {1}, Error: {2}"
                            , windowId, xmlVisibleString, compileResults.Errors[0].ErrorText);
                    }
                    else
                    {
                        Log.Message(LogLevel.Error, "Invalid Control VisibleCondition Detected, WindowId: {0}, ControlId: {1}, VisibleString: {2}, Error: {3}"
                            , windowId, controlId, xmlVisibleString,  compileResults.Errors[0].ErrorText);
                    }
                    return null;
                }
                var method = compileResults.CompiledAssembly.GetModules()[0].GetType("Visibility.VisibleMethodClass").GetMethod("ShouldBeVisible");

                return (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), method);
            }
            return null;
        }

    
  

        /// <summary>
        /// Loads the compiler settings.
        /// </summary>
        private static void LoadCompilerSettings()
        {
            if (!_compilerLoaded)
            {
                _compilerParams = new CompilerParameters();
                _compilerParams.GenerateInMemory = true;
                _compilerParams.TreatWarningsAsErrors = false;
                _compilerParams.GenerateExecutable = false;
                _compilerParams.CompilerOptions = "/optimize";
                string[] references = { "System.dll", typeof(GUIFramework.GenericExtensions).Assembly.Location };
                _compilerParams.ReferencedAssemblies.AddRange(references);
                _codeProvider = new CSharpCodeProvider( new Dictionary<String, String>{{ "CompilerVersion","v3.5" }});
                _compilerLoaded = true;
            }
        }

        /// <summary>
        /// The unique string to replace in the dynamic code to add the xml condition string
        /// </summary>
        private const string _replacementString = "8AC9ED14-B51F-401D-8CFF-87469ED062ED";

        /// <summary>
        /// The class we will generate to handle the controls visibility
        /// </summary>
        private static string _visibleClassString =
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
      

        #endregion



    
    }

  
}