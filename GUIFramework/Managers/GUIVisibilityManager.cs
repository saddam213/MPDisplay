﻿using System;
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
using Common.Helpers;
using GUISkinFramework.Windows;
using GUISkinFramework;
using GUISkinFramework.Dialogs;
using Common.Logging;
using Common;
using Common.Settings;

namespace GUIFramework.Managers
{
    public enum VisibleMessageType
    {
        ControlVisibilityChanged,
        WindowVisibilityChanged,
        GlobalVisibilityChanged
    }

    /// <summary>
    /// Class to handle GUI element visibility
    /// </summary>
    public static class GUIVisibilityManager   
    {
        #region Fields

        private static Log Log = LoggingManager.GetLog(typeof(GUIVisibilityManager));
        private static Dictionary<int, Dictionary<int, bool>> _controlVisibilityMap = new Dictionary<int, Dictionary<int, bool>>();
        private static MessengerService<VisibleMessageType> _visibilityService = new MessengerService<VisibleMessageType>(); 

        #endregion

        #region Properties

        /// <summary>
        /// Gets the visibility service.
        /// </summary>
        public static MessengerService<VisibleMessageType> VisibilityService
        {
            get { return _visibilityService; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Registers a message callback.
        /// </summary>
        /// <param name="message">The message to listen for.</param>
        /// <param name="callback">The callback.</param>
        public static void RegisterMessage(VisibleMessageType message, Action callback)
        {
            VisibilityService.Register(message, callback);
        }

        /// <summary>
        /// Deregisters the message callback.
        /// </summary>
        /// <param name="message">The message registered.</param>
        /// <param name="owner">The callback owner.</param>
        public static void DeregisterMessage(VisibleMessageType message, object owner)
        {
            VisibilityService.Deregister(message, owner);
        }

        /// <summary>
        /// Notifiys all registered callbacks for the message type.
        /// </summary>
        /// <param name="message">The action.</param>
        public static void NotifyVisibilityChanged(VisibleMessageType message)
        {
            if (message == VisibleMessageType.GlobalVisibilityChanged)
            {
                VisibilityService.NotifyListeners(VisibleMessageType.GlobalVisibilityChanged);
                VisibilityService.NotifyListeners(VisibleMessageType.ControlVisibilityChanged);
                VisibilityService.NotifyListeners(VisibleMessageType.WindowVisibilityChanged);
            }
            else
            {
                VisibilityService.NotifyListeners(message);
            }
        }

        /// <summary>
        /// Registers the all controls into a visibility map.
        /// </summary>
        /// <param name="controlHost">The control host.</param>
        public static void RegisterControlVisibility(IControlHost controlHost)
        {
            DeregisterControlVisibility(controlHost);
            lock (_controlVisibilityMap)
            {
                _controlVisibilityMap.Add(controlHost.Id, controlHost.Controls.GetControls().DistinctBy(c => c.Id).ToDictionary(k => k.Id, v => v.IsWindowOpenVisible));
            }
        }

        /// <summary>
        /// Deregisters the controls from the visibility map.
        /// </summary>
        /// <param name="controlHost">The control host.</param>
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

        /// <summary>
        /// Updates the control visibility.
        /// </summary>
        /// <param name="control">The control.</param>
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

        #endregion

        #region Condition Checks

        /// <summary>
        /// Determines whether the control is visible .
        /// </summary>
        /// <param name="windowId">The window id.</param>
        /// <param name="controlId">The control id.</param>
        /// <returns></returns>
        public static bool IsControlVisible(int windowId, int controlId)
        {
            if (_controlVisibilityMap.ContainsKey(windowId) && _controlVisibilityMap[windowId].ContainsKey(controlId))
            {
                return _controlVisibilityMap[windowId][controlId];
            }
            return false;
        }

        /// <summary>
        /// Determines whether the current window id matches the supplied id.
        /// </summary>
        /// <param name="windowId">The window id.</param>
        /// <returns></returns>
        public static bool IsMediaPortalWindow(int windowId)
        {
            return windowId == InfoRepository.Instance.WindowId;
        }

        /// <summary>
        /// Determines whether the current dialog id matches the supplied id.
        /// </summary>
        /// <param name="dialogId">The dialog id.</param>
        /// <returns></returns>
        public static bool IsMediaPortalDialog(int dialogId)
        {
            return dialogId == InfoRepository.Instance.DialogId;
        }

        /// <summary>
        /// Determines whether the previous window id matches the supplied id.
        /// </summary>
        /// <param name="windowId">The window id.</param>
        /// <returns></returns>
        public static bool IsMediaPortalPreviousWindow(int windowId)
        {
            return windowId == InfoRepository.Instance.PreviousWindowId;
        }

        /// <summary>
        /// Determines whether [is media portal control focused] [the specified control id].
        /// </summary>
        /// <param name="controlId">The control id.</param>
        /// <returns></returns>
        public static bool IsMediaPortalControlFocused(int controlId)
        {
            return controlId == InfoRepository.Instance.FocusedWindowControlId;
        }

        /// <summary>
        /// Determines whether the specified plugin is enabled.
        /// </summary>
        /// <param name="pluginName">Name of the plugin.</param>
        /// <returns></returns>
        public static bool IsPluginEnabled(string pluginName)
        {
            return InfoRepository.Instance.EnabledPluginMap.Any(plugin => plugin.ToLower() == pluginName.ToLower());
        }

        /// <summary>
        /// Determines whether the specified player id is playing.
        /// </summary>
        /// <param name="playerId">The player id.</param>
        /// <returns></returns>
        public static bool IsPlayer(int playerId)
        {
            return playerId.Equals((int)InfoRepository.Instance.PlayerType);
        }

        /// <summary>
        /// Determines whether tv is recording.
        /// </summary>
        /// <returns></returns>
        public static bool IsTvRecording()
        {
            return InfoRepository.Instance.IsTvRecording;
        }

        /// <summary>
        /// Determines whether [is fullscreen video] [the specified value].
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns></returns>
        public static bool IsFullscreenVideo(bool value)
        {
            return value == InfoRepository.Instance.IsFullscreenVideo;
        }

        /// <summary>
        /// Determines whether [is media portal connected].
        /// </summary>
        /// <returns></returns>
        public static bool IsMediaPortalConnected()
        {
            return InfoRepository.Instance.IsMediaPortalConnected;
        }

        /// <summary>
        /// Determines whether [is tv server connected].
        /// </summary>
        /// <returns></returns>
        public static bool IsTVServerConnected()
        {
            return InfoRepository.Instance.IsTVServerConnected;
        }

        /// <summary>
        /// Determines whether [is mp display connected].
        /// </summary>
        /// <returns></returns>
        public static bool IsMPDisplayConnected()
        {
            return InfoRepository.Instance.IsMPDisplayConnected;
        }

        /// <summary>
        /// Determines whether [is skin option enabled] [the specified option].
        /// </summary>
        /// <param name="option">The option.</param>
        /// <returns></returns>
        public static bool IsSkinOptionEnabled(string option)
        {
            return InfoRepository.Instance.IsSkinOptionEnabled(option);
        }

        /// <summary>
        /// Determines whether [is media portal list layout] [the specified layout].
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <returns></returns>
        public static bool IsMediaPortalListLayout(int layout)
        {
            return (int)ListRepository.GetCurrentMediaPortalListLayout() == layout;
        }

        /// <summary>
        /// Determines whether [is multi seat install].
        /// </summary>
        /// <returns></returns>
        public static bool IsMultiSeatInstall()
        {
            return RegistrySettings.InstallType == MPDisplayInstallType.GUI;
        }

        #endregion

        #region Condition Parser/Compiler

        private static Module _visibleConditionModule;

        private static string _classTemplate =
          @"using System;
            using System.Collections.Generic;
            using GUIFramework.Managers;
            namespace Visibility
            {
               public class VisibleMethodClass
               {
                  *MethodPlaceHolder*
               }
            }";
        private static string _methodTemplate =
          @"public static bool ShouldBeVisible()
            { 
              if (*ConditionPlaceHolder*)
              {
                 return true;
              }  
              return false;
            }";


        /// <summary>
        /// Gets the visible condition.
        /// </summary>
        /// <param name="windowId">The window identifier.</param>
        /// <returns></returns>
        public static Func<bool> GetVisibleCondition(int windowId)
        {
            if (_visibleConditionModule != null)
            {
                var method = _visibleConditionModule.GetType("Visibility.VisibleMethodClass").GetMethod(string.Format("ShouldBeVisibleW{0}", windowId));
                if (method != null)
                {
                    return () => (bool)method.Invoke(_visibleConditionModule, null);
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the visible condition.
        /// </summary>
        /// <param name="windowId">The window identifier.</param>
        /// <param name="controlId">The control identifier.</param>
        /// <returns></returns>
        public static Func<bool> GetVisibleCondition(int windowId, int controlId)
        {
            if (_visibleConditionModule != null)
            {
                var method = _visibleConditionModule.GetType("Visibility.VisibleMethodClass").GetMethod(string.Format("ShouldBeVisibleW{0}C{1}", windowId, controlId));
                if (method != null)
                {
                    return () => (bool)method.Invoke(_visibleConditionModule, null);
                }
            }
            return null;
        }

        /// <summary>
        /// Creates the visible conditions.
        /// </summary>
        /// <param name="elements">The elements.</param>
        public static void CreateVisibleConditions(IEnumerable<IXmlControlHost> elements)
        {
            string methodTemplate = _methodTemplate;
            string visibleCode = _classTemplate;
            foreach (var controlHost in elements)
            {
                if (!string.IsNullOrEmpty(controlHost.VisibleCondition))
                {
                    visibleCode = visibleCode.Replace("*MethodPlaceHolder*", methodTemplate.Replace("ShouldBeVisible()", string.Format("ShouldBeVisibleW{0}()", controlHost.Id))
                        .Replace("*ConditionPlaceHolder*", CreateVisibleWindowConditionString(controlHost)) + Environment.NewLine + "*MethodPlaceHolder*");
                }

                foreach (var control in controlHost.Controls.GetControls())
                {
                    if (!string.IsNullOrEmpty(control.VisibleCondition))
                    {
                        visibleCode = visibleCode.Replace("*MethodPlaceHolder*", methodTemplate.Replace("ShouldBeVisible()", string.Format("ShouldBeVisibleW{0}C{1}()", controlHost.Id, control.Id))
                            .Replace("*ConditionPlaceHolder*", CreateVisibleControlConditionString(control, controlHost.Id)) + Environment.NewLine + "*MethodPlaceHolder*");
                    }
                }
            }
            visibleCode = visibleCode.Replace("*MethodPlaceHolder*", "");

            if (!visibleCode.Contains("*ConditionPlaceHolder*"))
            {
                CompileString(elements, visibleCode);
            }
        }

        private static void CompileString(IEnumerable<IXmlControlHost> elements, string visibleCode)
        {
            var _compilerParams = new CompilerParameters();
            _compilerParams.GenerateInMemory = true;
            _compilerParams.TreatWarningsAsErrors = false;
            _compilerParams.GenerateExecutable = false;
            _compilerParams.CompilerOptions = "/optimize";
            string[] references = { "System.dll", typeof(GUIFramework.GenericExtensions).Assembly.Location };
            _compilerParams.ReferencedAssemblies.AddRange(references);
            var _codeProvider = new CSharpCodeProvider();

            CompilerResults compileResults = _codeProvider.CompileAssemblyFromSource(_compilerParams, visibleCode);
            if (compileResults.Errors.HasErrors)
            {
                try
                {
                    CompilerError error = compileResults.Errors[0];
                    List<string> lines = visibleCode.Replace("\r", "").Split('\n').ToList();
                    if (lines.Any() && lines.Count > error.Line)
                    {
                        var errorLine = lines[error.Line - 3];
                        if (errorLine.Contains("public static bool ShouldBeVisible"))
                        {
                            errorLine = errorLine.Replace("public static bool ShouldBeVisible", "").Replace("()","").Trim();
                            if (errorLine.Contains('C'))
                            {
                                int[] details = errorLine.Split(new char[] { 'W', 'C' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                                var window = elements.FirstOrDefault(w => w.Id == details[0]);
                                var control = window.Controls.GetControls().FirstOrDefault(c => c.Id == details[1]);
                                Log.Message(LogLevel.Error, "An error occured creating control VisibleCondition, Error: {0}, Window: {1} ({2}), Control: {3} ({4})", error.ErrorText, window.Name, window.Id, control.Name, control.Id);
                                Log.Message(LogLevel.Error, "Visibility Code: <{0}>", visibleCode);
                            }
                            else
                            {
                                int details = int.Parse(errorLine.Replace("W", "").Trim());
                                var window = elements.FirstOrDefault(w => w.Id == details);
                                Log.Message(LogLevel.Error, "An error occured creating window VisibleCondition, Error: {0}, Window: {1} ({2})", error.ErrorText, window.Name, window.Id);
                                Log.Message(LogLevel.Error, "Visibility Code: <{0}>", visibleCode);
                            }

                            for (int i = 0; i < _methodTemplate.Replace("\r", "").Split('\n').Count(); i++)
                            {
                                lines.RemoveAt(error.Line - 3);
                            }

                            if (lines.Count > 15)
                            {
                                string newCode = string.Join(Environment.NewLine, lines.ToArray());
                                CompileString(elements, newCode);
                            }
                        }
                        else
                        {
                            Log.Message(LogLevel.Error, "An error occured loading VisibleConditions, Error: {0}", error.ErrorText);
                            Log.Message(LogLevel.Error, "Visibility Code: <{0}>", visibleCode);
                        }
                    }
                  
                }
                catch (Exception ex)
                {
                    Log.Exception("An error occured loading VisibleConditions", ex);
                    Log.Message(LogLevel.Error, "Visibility Code: <{0}>", visibleCode);
                }
            }
            else
            {
                var module = compileResults.CompiledAssembly.GetModules()[0];
                if (module != null)
                {
                    _visibleConditionModule = module;
                }
            }
        }

        /// <summary>
        /// Creates the visible control condition string.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="parentId">The parent identifier.</param>
        /// <returns></returns>
        private static string CreateVisibleControlConditionString(XmlControl control, int parentId)
        {
            string xmlVisibleString = control.VisibleCondition.Replace("++", "&&");

            if (xmlVisibleString.Contains("IsSkinOptionEnabled("))
            {
                // \(([^)]*)\)
                xmlVisibleString = Regex.Replace(xmlVisibleString, @"IsSkinOptionEnabled\(([^)]*)\)", @"GUIVisibilityManager.IsSkinOptionEnabled(""$1"")");
                //xmlVisibleString.Replace("IsPluginEnabled(", "GUIVisibilityManager.IsPluginEnabled(");
            }

            xmlVisibleString = xmlVisibleString.Replace("IsControlVisible(", "GUIVisibilityManager.IsControlVisible(" + parentId + ",");

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
            xmlVisibleString = xmlVisibleString.Replace("IsMultiSeatInstall", "GUIVisibilityManager.IsMultiSeatInstall()");
            //IsMultiSeatInstall
            return xmlVisibleString;
        }

        /// <summary>
        /// Creates the visible window condition string.
        /// </summary>
        /// <param name="controlHost">The control host.</param>
        /// <returns></returns>
        private static string CreateVisibleWindowConditionString(IXmlControlHost controlHost)
        {
            string  xmlVisibleString = controlHost.VisibleCondition.Replace("++", "&&");
            if (xmlVisibleString.Contains("IsSkinOptionEnabled("))
            {
                xmlVisibleString = Regex.Replace(xmlVisibleString, @"IsSkinOptionEnabled\(([^)]*)\)", @"GUIVisibilityManager.IsSkinOptionEnabled(""$1"")");
            }
            if (controlHost is XmlPlayerWindow)
            {
                if (xmlVisibleString.Contains("IsPlayer("))
                {
                    foreach (var playtype in Enum.GetValues(typeof(APIPlaybackType)))
                    {
                        xmlVisibleString = xmlVisibleString.Replace(string.Format("IsPlayer({0})", playtype), string.Format("GUIVisibilityManager.IsPlayer({0})", (int)playtype));
                    }
                }
            }
            if (controlHost is XmlMPWindow)
            {
                xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalWindow(", "GUIVisibilityManager.IsMediaPortalWindow(");
                xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalDialog(", "GUIVisibilityManager.IsMediaPortalDialog(");
                xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalPreviousWindow(", "GUIVisibilityManager.IsMediaPortalPreviousWindow(");
            }
            if (controlHost is XmlMPDialog)
            {
                xmlVisibleString = xmlVisibleString.Replace("IsMediaPortalDialog(", "GUIVisibilityManager.IsMediaPortalDialog(");
            }
            return xmlVisibleString;
        }
     
        #endregion
    }

  
}