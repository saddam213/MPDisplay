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
using GUISkinFramework.Common;
using GUISkinFramework.Controls;
using MessageFramework.DataObjects;
using Microsoft.CSharp;
using MPDisplay.Common;

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

        static GUIVisibilityManager()
        {
           
        }

        private static Dictionary<int, Dictionary<int, bool>> _controlVisibilityMap = new Dictionary<int, Dictionary<int, bool>>();


        public static void RegisterControlVisibility(this IControlHost controlHost)
        {
            controlHost.DeregisterControlVisibility();
            _controlVisibilityMap.Add(controlHost.Id, controlHost.Controls.GetControls().DistinctBy(c => c.Id).ToDictionary(k => k.Id, v => v.IsWindowOpenVisible));
            controlHost.RefreshControlVisibility();
        }



        public static void DeregisterControlVisibility(this IControlHost controlHost)
        {
            if (_controlVisibilityMap.ContainsKey(controlHost.Id))
            {
                _controlVisibilityMap.Remove(controlHost.Id);
            }
        }


        public static void RefreshControlVisibility(this IControlHost controlHost)
        {
            UpdateControlVisibilityMap(controlHost);
            SetControlVisibility(controlHost);
        }


        private static void UpdateControlVisibilityMap(IControlHost controlHost)
        {
            if (_controlVisibilityMap.ContainsKey(controlHost.Id))
            {
                foreach (var control in controlHost.Controls.GetControls())
                {
                    if (_controlVisibilityMap[controlHost.Id].ContainsKey(control.Id))
                    {
                        bool current = _controlVisibilityMap[controlHost.Id][control.Id];
                        if (current != control.VisibleCondition.ShouldBeVisible(current))
                        {
                            _controlVisibilityMap[controlHost.Id][control.Id] = !_controlVisibilityMap[controlHost.Id][control.Id];
                            UpdateControlVisibilityMap(controlHost);
                            return;
                        }
                    }
                }
            }
        }

        private static void SetControlVisibility(IControlHost controlHost)
        {
            if (_controlVisibilityMap.ContainsKey(controlHost.Id))
            {
                // set controls based on map
                foreach (var control in controlHost.Controls.GetControls())
                {
                    if (_controlVisibilityMap[controlHost.Id].ContainsKey(control.Id))
                    {
                        control.SetControlVisibility(_controlVisibilityMap[controlHost.Id][control.Id]);
                    }
                }
            }
        }




        public static void ToggleControlVisibility(this IControlHost controlHost, int controlid)
        {
            if (_controlVisibilityMap.ContainsKey(controlHost.Id))
            {
                if (_controlVisibilityMap[controlHost.Id].ContainsKey(controlid))
                {
                    _controlVisibilityMap[controlHost.Id][controlid] = !_controlVisibilityMap[controlHost.Id][controlid];
                    controlHost.RefreshControlVisibility();
                }
            }
        }


        #region Condition Checks

        public static bool IsControlVisible(int windowId, int controlId)
        {
            if (_controlVisibilityMap.ContainsKey(windowId))
            {
                if (_controlVisibilityMap[windowId].ContainsKey(controlId))
                {
                    return _controlVisibilityMap[windowId][controlId];
                }
            }
            return false;
        }
             
        public static bool IsPluginEnabled(string pluginName)
        {
            return GUIDataRepository.InfoManager.EnabledPluginMap.Contains(pluginName.ToLower());
        }

        public static bool IsPlayer(int playerId)
        {
            return playerId.Equals((int)GUIDataRepository.InfoManager.PlayerType);
        }

        public static bool IsTvRecording()
        {
            return  GUIDataRepository.InfoManager.IsTvRecording;
        }

        public static bool IsFullscreenVideo(bool value)
        {
            return value == GUIDataRepository.InfoManager.IsFullscreenVideo;
        }

        public static bool IsMediaPortalWindow(int windowId)
        {
            return windowId == GUIDataRepository.InfoManager.WindowId;
        }

        public static bool IsMediaPortalDialog(int dialogId)
        {
            return dialogId == GUIDataRepository.InfoManager.DialogId;
        }

        public static bool IsMediaPortalPreviousWindow(int windowId)
        {
            return windowId == GUIDataRepository.InfoManager.PreviousWindowId;
        }

        public static bool IsMediaPortalControlFocused(int controlId)
        {
            return controlId == GUIDataRepository.InfoManager.FocusedWindowControlId;
        }

        public static bool IsMediaPortalConnected()
        {
            return GUIMessageService.Instance.IsMediaPortalConnected;
        }

        public static bool IsTVServerConnected()
        {
            return GUIMessageService.Instance.IsTVServerConnected;
        }

        public static bool IsMPDisplayConnected()
        {
            return GUIMessageService.Instance.IsMPDisplayConnected;
        }

        public static bool IsSkinOptionEnabled(string option)
        {
            return GUIDataRepository.InfoManager.IsSkinOptionEnabled(option);
        }

        public static bool IsMediaPortalListLayout(int layout)
        {
            return (int)GUIDataRepository.ListManager.GetMediaPortalListLayout() == layout;
        }

        #endregion

        #region Condition Parser/Compiler

        private static CompilerParameters _compilerParams;
        private static CSharpCodeProvider _codeProvider;
        private static bool _compilerLoaded = false;

        public static MethodInfo CreateVisibleCondition(int windowId, string xmlVisibleString)
        {
            xmlVisibleString = xmlVisibleString.Replace("++", "&&");
            xmlVisibleString = xmlVisibleString.Replace("IsControlVisible(", "GUIVisibilityManager.IsControlVisible("+windowId+",");

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
                // \(([^)]*)\)
                xmlVisibleString = Regex.Replace(xmlVisibleString, @"IsPluginEnabled\(([^)]*)\)", @"GUIVisibilityManager.IsPluginEnabled(""$1"")");
                //xmlVisibleString.Replace("IsPluginEnabled(", "GUIVisibilityManager.IsPluginEnabled(");
            }

            if (xmlVisibleString.Contains("IsSkinOptionEnabled("))
            {
                // \(([^)]*)\)
                xmlVisibleString = Regex.Replace(xmlVisibleString, @"IsSkinOptionEnabled\(([^)]*)\)", @"GUIVisibilityManager.IsSkinOptionEnabled(""$1"")");
                //xmlVisibleString.Replace("IsPluginEnabled(", "GUIVisibilityManager.IsPluginEnabled(");
            }


            xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalConnected", "GUIVisibilityManager.IsMediaPortalConnected()");
            xmlVisibleString = xmlVisibleString.Replace("IsTVServerConnected", "GUIVisibilityManager.IsTVServerConnected()");
            xmlVisibleString = xmlVisibleString.Replace("IsMPDisplayConnected", "GUIVisibilityManager.IsMPDisplayConnected()");
            xmlVisibleString = xmlVisibleString.Replace("IsTvRecording", "GUIVisibilityManager.IsTvRecording()");
            xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalWindow(", "GUIVisibilityManager.IsMediaPortalWindow(");
            xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalDialog(", "GUIVisibilityManager.IsMediaPortalDialog(");
            xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalPreviousWindow(", "GUIVisibilityManager.IsMediaPortalPreviousWindow(");
         
            xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalControlFocused(", "GUIVisibilityManager.IsMediaPortalControlFocused(");
          //  xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalControlVisible(", "GUIVisibilityManager.IsMediaPortalControlVisible(");

       

            //GUIWindowManager
            if (!string.IsNullOrWhiteSpace(xmlVisibleString))
            {
                return CompileCondition(xmlVisibleString);
            }
            return null;
        } 

        /// <summary>
        /// Creates a complex visible condition.
        /// </summary>
        /// <param name="xmlVisibleString">The XML visible string.</param>
        /// <returns></returns>
        private static MethodInfo CompileCondition(string xmlVisibleString)
        {
            if (!string.IsNullOrEmpty(xmlVisibleString))
            {
                LoadCompilerSettings();
                string code = _visibleClassString.Replace(_replacementString, xmlVisibleString);
                CompilerResults compileResults = _codeProvider.CompileAssemblyFromSource(_compilerParams, code);
                if (compileResults.Errors.HasErrors)
                {
                    // LOG
                    return null;
                }
                return compileResults.CompiledAssembly.GetModules()[0].GetType("Visibility.VisibleMethodClass").GetMethod("ShouldBeVisible");
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
                string[] references = { "System.dll", "GUIFramework.dll" };
                _compilerParams.ReferencedAssemblies.AddRange(references);
                _codeProvider = new CSharpCodeProvider();
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