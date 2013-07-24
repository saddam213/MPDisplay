using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GUIFramework.GUI;
using GUIFramework.GUI.Controls;
using GUISkinFramework;
using GUISkinFramework.Controls;
using GUISkinFramework.Property;
using GUISkinFramework.Skin;
using MessageFramework.DataObjects;
using MPDisplay.Common;
using MPDisplay.Common.Settings;

namespace GUIFramework.Managers
{
    public interface IRepository
    {
        GUISettings Settings { get; set; }
        XmlSkinInfo SkinInfo { get; set; }
        void Initialize(GUISettings settings, XmlSkinInfo skininfo);
        void ClearRepository();
        void ResetRepository();
    }


    public class GUIDataRepository : IRepository
    {
        #region Singleton Implementation

        private GUIDataRepository() { }
        private static GUIDataRepository _instance;
        public static GUIDataRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GUIDataRepository();
                }
                return _instance;
            }
        }

        public static async void RegisterWindowData(GUIWindow window)
        {
            if (window != null)
            {
                await GUIMessageService.Instance.SendMediaPortalMessage(Instance.GetDataRegistrationMessage(window, APIMediaPortalMessageType.WindowInfoMessage));
            }
        }

        public static async void RegisterDialogData(GUIDialog dialog)
        {
            if (dialog != null)
            {
                await GUIMessageService.Instance.SendMediaPortalMessage(Instance.GetDataRegistrationMessage(dialog, APIMediaPortalMessageType.DialogInfoMessage));
            }
        }

  
      
        public static T GetManager<T>() where T : IRepository
        {
            return Instance.GetDataRepository<T>();
        }

        public static PropertyRepository PropertyManager
        {
            get { return Instance.GetDataRepository<PropertyRepository>(); }
        }

        public static ListRepository ListManager
        {
            get { return Instance.GetDataRepository<ListRepository>(); }
        }

        public static InfoRepository InfoManager
        {
            get { return Instance.GetDataRepository<InfoRepository>(); }
        }

        #endregion

        private Dictionary<int, APIMediaPortalMessage> _windowInfoMessageCache = new Dictionary<int, APIMediaPortalMessage>();
        private List<IRepository> _repositories = new List<IRepository>();
        public GUISettings Settings { get; set; }
        public XmlSkinInfo SkinInfo { get; set; }

        public void Initialize(GUISettings settings, XmlSkinInfo skininfo)
        {
            Settings = settings;
            SkinInfo = skininfo;
            _windowInfoMessageCache = new Dictionary<int, APIMediaPortalMessage>();
            _repositories.Add(PropertyRepository.Instance);
            _repositories.Add(ListRepository.Instance);
            _repositories.Add(InfoRepository.Instance);

            foreach (var repository in _repositories)
            {
                repository.Initialize(settings, skininfo);
            }
        }

        public void ClearRepository()
        {
            foreach (var repository in _repositories)
            {
                repository.ClearRepository();
            }
        }

        public void ResetRepository()
        {
            Settings = null;
            SkinInfo = null;
            foreach (var repository in _repositories)
            {
                repository.ResetRepository();
            }
        }

        public T GetDataRepository<T>() where T : IRepository
        {
            return _repositories.OfType<T>().FirstOrDefault();
        }

     
        private APIMediaPortalMessage GetDataRegistrationMessage(IControlHost controlHost, APIMediaPortalMessageType messageType)
        {
            if (controlHost != null)
            {
                if (!_windowInfoMessageCache.ContainsKey(controlHost.Id))
                {
                    var msg = new APIMediaPortalMessage
                    {
                        MessageType = messageType,
                        WindowMessage = new APIWindowInfoMessage
                        {
                            EQData = GenericDataRepository.GetEQDataLength(controlHost),
                            Lists = controlHost.Controls.GetControls().OfType<GUIList>().Select(l => l.ListType.ToAPIType()).ToList(),
                            Properties = PropertyRepository.GetRegisteredProperties(controlHost)
                        }
                    };
                    _windowInfoMessageCache.Add(controlHost.Id, msg);
                }
                return _windowInfoMessageCache[controlHost.Id];
            }
            return null;
        }
    }

    public class PropertyRepository : IRepository
    {
        #region Singleton Implementation

        private PropertyRepository() { }
        private static PropertyRepository _instance;
        public static PropertyRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PropertyRepository();
                }
                return _instance;
            }
        }

        public static void RegisterPropertyMessage(string property, Action callback)
        {
            Instance.RegisterProperty(property, callback);
        }

        public static void DeregisterPropertyMessage( IPropertyControl owner,string property)
        {
            Instance.DeregisterProperty(owner, property);
        }

        public static async Task<T> GetProperty<T>(string property)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)await Instance.GetControlLabelValue(property);
            }
            if (typeof(T) == typeof(int))
            {
                return (T)(object)await Instance.GetControlNumberValue(property);
            }
            if (typeof(T) == typeof(byte[]))
            {
                return (T)(object)await Instance.GetControlImageValue(property);
            }
            return default(T);
        }

        public static List<XmlProperty> GetRegisteredProperties(IPropertyControl control, string xmlstring)
        {
            return Instance.GetControlProperties(control, xmlstring);
        }

        public static List<APIPropertyMessage> GetRegisteredProperties(IControlHost controlHost)
        {
            return Instance.GetControlHostProperties(controlHost);
        }

        #endregion

   

       
        private List<APIPropertyMessage> _skinProperties = new List<APIPropertyMessage>();
        private MessengerService<string> _propertyService = new MessengerService<string>();
        private DataRepository<string, APIPropertyMessage> _propertyRepository;
        private DataRepository<string, string> _propertyDefaults;

        public GUISettings Settings { get; set; }
        public XmlSkinInfo SkinInfo { get; set; }

        public void Initialize(GUISettings settings, XmlSkinInfo skininfo)
        {
            Settings = settings;
            SkinInfo = skininfo;
            _propertyRepository = new DataRepository<string, APIPropertyMessage>();
            _propertyDefaults = new DataRepository<string, string>();
            _skinProperties = GetSkinProperties();
            if (SkinInfo != null)
            {
                foreach (var property in SkinInfo.Properties)
                {
                    _propertyDefaults.AddOrUpdate(property.SkinTag, property.DefaultValue);
                }
            }
        }

        public void ClearRepository()
        {
            _propertyRepository.ClearRepository();
        }

        public void ResetRepository()
        {
            ClearRepository();
            Settings = null;
            SkinInfo = null;
        }
      
        
        /// <summary>
        /// Adds an APIProperty property.
        /// </summary>
        /// <param name="tag">The property tag.</param>
        /// <param name="tagValue">The property value.</param>
        public async Task AddProperty(APIPropertyMessage propertyMessage)
        {
            await Task.Factory.StartNew(() =>
            {
                if (propertyMessage != null)
                {
                    if (_propertyRepository.AddOrUpdate(propertyMessage.SkinTag, propertyMessage))
                    {
                        NotifyPropertyChanged(propertyMessage.SkinTag);
                    }
                }
            });
        }

        public void DeregisterProperty(IPropertyControl control, string propertyString)
        {
            if (HasProperties(propertyString))
            {
                foreach (var item in GetPropertiesFromXmlString(propertyString))
                {
                    _propertyService.Deregister(item, control);
                }
            }
        }

        public void RegisterProperty(string propertyString, Action callback)
        {
            if (HasProperties(propertyString))
            {
                foreach (var property in GetPropertiesFromXmlString(propertyString))
                {
                    _propertyService.Register(property, callback);
                }
            }
        }

        public async Task<string> GetControlLabelValue(string xmlstring)
        {
            return await Task.Factory.StartNew<string>(() =>
            {
                string returnValue = string.Empty;
                var parts = xmlstring.Contains("+") ? xmlstring.Split('+').ToList() : new List<string> { xmlstring };
                foreach (var part in parts)
                {
                    if (part.StartsWith("@"))
                    {
                        returnValue += SkinInfo.GetLanguageValue(part);
                        continue;
                    }
                    if (part.StartsWith("#"))
                    {
                        returnValue += GetPropertyLabelValueOrDefault(part);
                        continue;
                    }
                    returnValue += part;
                }
                return returnValue;
            });
        }

        public async Task<byte[]> GetControlImageValue(string xmlstring)
        {
            if (xmlstring.StartsWith("#") && !xmlstring.Contains("+"))
            {
                return await Task.Factory.StartNew<byte[]>(() =>
                {
                    return GetPropertyImageValueOrDefault(xmlstring);
                });
            }
            else
            {
                string filename = await GetControlLabelValue(xmlstring);
                return await Task.Factory.StartNew<byte[]>(() =>
                {
                    return SkinInfo.GetImageValue(filename);
                });
            }
        }

        public async Task<int> GetControlNumberValue(string xmlstring)
        {
            return await Task.Factory.StartNew<int>(() =>
            {
                return GetPropertyNumberValueOrDefault(xmlstring);
            });
        }

        public List<XmlProperty> GetControlProperties(IPropertyControl control, string xmlstring)
        {
            var returnValue = new List<XmlProperty>();
            if (HasProperties(xmlstring))
            {
                foreach (var property in GetPropertiesFromXmlString(xmlstring))
                {
                    returnValue.Add(SkinInfo.Properties.FirstOrDefault(x => x.SkinTag == property));
                }
            }
            return returnValue;
        }

        public List<APIPropertyMessage> GetControlHostProperties(IControlHost controlHost)
        {
            if (controlHost != null)
            {
                var skinTags = new List<string>();
                foreach (var control in controlHost.Controls.GetControls().OfType<IPropertyControl>())
                {
                    if (control.RegisteredProperties != null && control.RegisteredProperties.Any())
                    {
                        foreach (var property in control.RegisteredProperties)
                        {
                            if (property != null && !string.IsNullOrEmpty(property.SkinTag) && !skinTags.Contains(property.SkinTag))
                            {
                                skinTags.Add(property.SkinTag);
                            }
                        }
                    }
                }
                return _skinProperties.Where(p => p != null && skinTags.Contains(p.SkinTag)).ToList();
            }
            return new List<APIPropertyMessage>();
        }

        private string GetPropertyLabelValueOrDefault(string tag)
        {
            var result = _propertyRepository.GetValueOrDefault(tag, null);
            if (result != null && !string.IsNullOrEmpty(result.Label))
            {
                return result.Label;
            }
            return _propertyDefaults.GetValueOrDefault(tag,string.Empty);
        }

        private byte[] GetPropertyImageValueOrDefault(string tag)
        {
            var result = _propertyRepository.GetValueOrDefault(tag, null);
            if (result != null && result.Image != null)
            {
                var img = result.Image.ToImageBytes();
                if (img != null)
                {
                    return img;
                }
            }
            return SkinInfo.GetImageValue(_propertyDefaults.GetValueOrDefault(tag, string.Empty));
        }

        private int GetPropertyNumberValueOrDefault(string tag)
        {
            int returnValue = 0;
            var result = _propertyRepository.GetValueOrDefault(tag, null);
            if (result != null)
            {
                return result.Number;
            }

            if (int.TryParse(_propertyDefaults.GetValueOrDefault(tag, "0"), out returnValue))
            {
                return returnValue;
            }
            return 0;
        }

        private bool HasProperties(string xmlString)
        {
            return !string.IsNullOrEmpty(xmlString) && xmlString.Contains("#");
        }

        private IEnumerable<string> GetPropertiesFromXmlString(string xmlString)
        {
            if (xmlString.Contains("+"))
            {
                foreach (var property in xmlString.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => x.Trim().StartsWith("#"))
                    .Select(x => x.Trim()))
                {
                    yield return property;
                }
            }
            else
            {
                yield return xmlString.Trim();
            }
        }

        private List<APIPropertyMessage> GetSkinProperties()
        {
            var properties = new List<APIPropertyMessage>();
            if (SkinInfo != null)
            {
                foreach (var xmlProperty in SkinInfo.Properties)
                {
                    if (!properties.Any(x => x.SkinTag == xmlProperty.SkinTag))
                    {
                        properties.Add(new APIPropertyMessage
                        {
                            SkinTag = xmlProperty.SkinTag,
                            Tags = xmlProperty.MediaPortalTags.Select(x => x.Tag).ToList(),
                            DefaultValue = xmlProperty.DefaultValue,
                            PropertyType = xmlProperty.PropertyType.ToAPIType()
                        });
                    }
                }
            }
            return properties;
        }

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="tag">The tag.</param>
        private void NotifyPropertyChanged(string tag)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                _propertyService.NotifyListeners(tag);
            });
        }


       
    }

    public class ListRepository : IRepository
    {
        #region Singleton Implementation

        private ListRepository() { }
        private static ListRepository _instance;
        public static ListRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ListRepository();
                }
                return _instance;
            }
        }

        public static void RegisterListMessage(XmlListType listType, Action listItemCallback, Action listActionCallback, Action listLayoutCallback)
        {
            Instance.RegisterList(listType, listItemCallback, listActionCallback, listLayoutCallback);
        }

        public static void DeregisterListMessage(XmlListType listType, GUIList owner)
        {
            Instance.DeregisterList(owner, listType);
        }

        //public static void NotifyListeners<T>(InfoMessageType message, T value)
        //{
        //    Instance.NotifiyValueChanged<T>(message, value);
        //}

        #endregion

        private MessengerService<ListServiceMessage> _listService = new MessengerService<ListServiceMessage>();
        private DataRepository<XmlListType, List<APIListItem>> _listRepository;
        private DataRepository<XmlListType, APIListAction> _listSelectionRepository;
        private APIListLayout _mediaPortalListLayout;

        public GUISettings Settings { get; set; }
        public XmlSkinInfo SkinInfo { get; set; }


        public void Initialize(GUISettings settings, XmlSkinInfo skininfo)
        {
            Settings = settings;
            SkinInfo = skininfo;
            _listRepository = new DataRepository<XmlListType, List<APIListItem>>((v1, v2) => v1.IsEqual(v2));
            _listSelectionRepository = new DataRepository<XmlListType, APIListAction>((v1, v2) => v1.IsEqual(v2));
        }

        public void ClearRepository()
        {
            _listRepository.ClearRepository();
            _listSelectionRepository.ClearRepository();
        }

        public void ResetRepository()
        {
            Settings = null;
            SkinInfo = null;
        }

        public async Task AddListData(APIListMessage message)
        {
            await Task.Factory.StartNew(() =>
            {
                if (message != null)
                {
                    if (message.MessageType == APIListMessageType.List && message.List != null)
                    {
                        switch (message.List.ListType)
                        {
                            case APIListType.List:
                                AddList(XmlListType.MediaPortalListControl, message.List.ListItems);
                                _mediaPortalListLayout = message.List.ListLayout;
                                NotifyListLayoutChanged();
                                break;
                            case APIListType.Menu:
                                AddList(XmlListType.MediaPortalMenuControl, message.List.ListItems);
                                break;
                            case APIListType.GroupMenu:
                                AddList(XmlListType.MediaPortalButtonGroup, message.List.ListItems);
                                break;
                            case APIListType.DialogList:
                                break;
                            default:
                                break;
                        }
                    }
                    else if (message.MessageType == APIListMessageType.Action && message.Action != null)
                    {
                        if (message.Action.ActionType == APIListActionType.SelectedItem)
                        {
                            switch (message.Action.ItemListType)
                            {
                                case APIListType.List:
                                    AddListSelection(XmlListType.MediaPortalListControl, message.Action);
                                    break;
                                case APIListType.Menu:
                                    AddListSelection(XmlListType.MediaPortalMenuControl, message.Action);
                                    break;
                                case APIListType.GroupMenu:
                                    AddListSelection(XmlListType.MediaPortalButtonGroup, message.Action);
                                    break;
                                case APIListType.DialogList:
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (message.Action.ActionType == APIListActionType.Layout)
                        {
                            _mediaPortalListLayout = message.Action.ItemLayout;
                            NotifyListLayoutChanged();
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Adds an APIProperty property.
        /// </summary>
        /// <param name="listType">Type of the list.</param>
        /// <param name="list">The list.</param>
        public void AddList(XmlListType listType, List<APIListItem> list)
        {
            if (_listRepository.AddOrUpdate(listType, list))
            {
                NotifyListChanged(listType);
            }
        }

        public void AddListSelection(XmlListType listType, APIListAction action)
        {
            if (_listSelectionRepository.AddOrUpdate(listType, action))
            {
                NotifyListSelectionChanged(listType);
            }
        }

        public async Task<List<APIListItem>> GetListItems(XmlListType listType)
        {
            return await Task.Factory.StartNew<List<APIListItem>>(() =>
            {
                   return _listRepository.GetValue(listType);
            });
        }

        public async Task<APIListAction> GetSelectedListItem(XmlListType listType)
        {
            return await Task.Factory.StartNew<APIListAction>(() =>
            {
                return _listSelectionRepository.GetValue(listType);
            });
        }

        public XmlListLayout GetMediaPortalListLayout()
        {
            switch (_mediaPortalListLayout)
            {
                case APIListLayout.Vertical:
                    return XmlListLayout.Vertical;
                case APIListLayout.Horizontal:
                    return XmlListLayout.Horizontal;
                case APIListLayout.CoverFlow:
                    return XmlListLayout.CoverFlow;
                default:
                    break;
            }
            return XmlListLayout.Vertical;
        }

        public void RegisterList(XmlListType listType, Action listItemCallback, Action listActionCallback, Action listLayoutCallback)
        {
            switch (listType)
            {
                case XmlListType.MediaPortalListControl:
                    _listService.Register(ListServiceMessage.ListItems, listItemCallback);
                    _listService.Register(ListServiceMessage.ListItemSelect, listActionCallback);
                    _listService.Register(ListServiceMessage.ListItemLayout, listLayoutCallback);
                    break;
                case XmlListType.MediaPortalButtonGroup:
                    _listService.Register(ListServiceMessage.GroupItems, listItemCallback);
                    _listService.Register(ListServiceMessage.GroupItemSelect, listActionCallback);
                    break;
                case XmlListType.MediaPortalMenuControl:
                    _listService.Register(ListServiceMessage.MenuItems, listItemCallback);
                    _listService.Register(ListServiceMessage.MenuItemSelect, listActionCallback);
                    break;
                case XmlListType.MPDisplaySkins:
                    _listService.Register(ListServiceMessage.SkinItems, listItemCallback);
                    _listService.Register(ListServiceMessage.SkinItemSelect, listActionCallback);
                    break;
                case XmlListType.MPDisplayStyles:
                    _listService.Register(ListServiceMessage.StyleItems, listItemCallback);
                    _listService.Register(ListServiceMessage.StyleItemSelect, listActionCallback);
                    break;
                case XmlListType.MPDisplayLanguages:
                    _listService.Register(ListServiceMessage.LanguageItems, listItemCallback);
                    _listService.Register(ListServiceMessage.LanguageItemSelect, listActionCallback);
                    break;
                default:
                    break;
            }
        }

        public void DeregisterList(GUIList control, XmlListType listType)
        {
            switch (listType)
            {
                case XmlListType.MediaPortalListControl:
                    _listService.Deregister(ListServiceMessage.ListItems, control);
                    _listService.Deregister(ListServiceMessage.ListItemSelect, control);
                    _listService.Deregister(ListServiceMessage.ListItemLayout, control);
                    break;
                case XmlListType.MediaPortalButtonGroup:
                    _listService.Deregister(ListServiceMessage.GroupItems, control);
                    _listService.Deregister(ListServiceMessage.GroupItemSelect, control);
                    break;
                case XmlListType.MediaPortalMenuControl:
                    _listService.Deregister(ListServiceMessage.MenuItems, control);
                    _listService.Deregister(ListServiceMessage.MenuItemSelect, control);
                    break;
                case XmlListType.MPDisplaySkins:
                    _listService.Deregister(ListServiceMessage.SkinItems, control);
                    _listService.Deregister(ListServiceMessage.SkinItemSelect, control);
                    break;
                case XmlListType.MPDisplayStyles:
                    _listService.Deregister(ListServiceMessage.StyleItems, control);
                    _listService.Deregister(ListServiceMessage.StyleItemSelect, control);
                    break;
                case XmlListType.MPDisplayLanguages:
                    _listService.Deregister(ListServiceMessage.LanguageItems, control);
                    _listService.Deregister(ListServiceMessage.LanguageItemSelect, control);
                    break;
                default:
                    break;
            }
        }


        public void FocusListControlItem(GUIList listcontrol, APIListItem item)
        {
            if (listcontrol != null && item != null)
            {
                switch (listcontrol.ListType)
                {
                    case XmlListType.MediaPortalListControl:
                        SendListAction(APIListActionType.FocusedItem, APIListType.List, item);
                        break;
                    case XmlListType.MediaPortalButtonGroup:
                        SendListAction(APIListActionType.FocusedItem, APIListType.GroupMenu, item);
                        break;
                    case XmlListType.MediaPortalMenuControl:
                        SendListAction(APIListActionType.FocusedItem, APIListType.Menu, item);
                        break;
                    default:
                        break;
                }
            }
        }

        public void SelectListControlItem(GUIList listcontrol, APIListItem item)
        {
            if (listcontrol != null && item != null)
            {
                switch (listcontrol.ListType)
                {
                    case XmlListType.None:
                        break;
                    case XmlListType.MediaPortalListControl:
                        SendListAction(APIListActionType.SelectedItem, APIListType.List, item);
                        break;
                    case XmlListType.MediaPortalButtonGroup:
                        SendListAction(APIListActionType.SelectedItem, APIListType.GroupMenu, item);
                        break;
                    case XmlListType.MediaPortalMenuControl:
                        SendListAction(APIListActionType.SelectedItem, APIListType.Menu, item);
                        break;
                    case XmlListType.MPDisplaySkins:
                        break;
                    case XmlListType.MPDisplayStyles:
                        SkinInfo.SetStyle(item.Label);
                        break;
                    case XmlListType.MPDisplayLanguages:
                        SkinInfo.SetLanguage(item.Label);
                        break;
                    default:
                        break;
                }
            }
        }

        private void SendListAction(APIListActionType actionType, APIListType listType, APIListItem item)
        {
            GUIMessageService.Instance.SendMediaPortalMessage(new APIMediaPortalMessage
            {
                MessageType = APIMediaPortalMessageType.ActionMessage,
                ActionMessage = new APIActionMessage
                {
                    ActionType = APIActionMessageType.ListAction,
                    ListAction = new APIListAction
                    {
                        ActionType = actionType,
                        ItemListType = listType,
                        ItemText = item.Label,
                        ItemIndex = item.Index,
                    }
                }
            });
        }



        private void NotifyListChanged(XmlListType listType)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                switch (listType)
                {
                    case XmlListType.None:
                        break;
                    case XmlListType.MediaPortalListControl:
                        _listService.NotifyListeners(ListServiceMessage.ListItems);
                        break;
                    case XmlListType.MediaPortalButtonGroup:
                        _listService.NotifyListeners(ListServiceMessage.GroupItems);
                        break;
                    case XmlListType.MediaPortalMenuControl:
                        _listService.NotifyListeners(ListServiceMessage.MenuItems);
                        break;
                    case XmlListType.MPDisplaySkins:
                        _listService.NotifyListeners(ListServiceMessage.SkinItems);
                        break;
                    case XmlListType.MPDisplayStyles:
                        _listService.NotifyListeners(ListServiceMessage.StyleItems);
                        break;
                    case XmlListType.MPDisplayLanguages:
                        _listService.NotifyListeners(ListServiceMessage.LanguageItems);
                        break;
                    default:
                        break;
                }
            });
        }

        private void NotifyListSelectionChanged(XmlListType listType)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                switch (listType)
                {
                    case XmlListType.None:
                        break;
                    case XmlListType.MediaPortalListControl:
                        _listService.NotifyListeners(ListServiceMessage.ListItemSelect);
                        break;
                    case XmlListType.MediaPortalButtonGroup:
                        _listService.NotifyListeners(ListServiceMessage.GroupItemSelect);
                        break;
                    case XmlListType.MediaPortalMenuControl:
                        _listService.NotifyListeners(ListServiceMessage.MenuItemSelect);
                        break;
                    case XmlListType.MPDisplaySkins:
                        _listService.NotifyListeners(ListServiceMessage.SkinItemSelect);
                        break;
                    case XmlListType.MPDisplayStyles:
                        _listService.NotifyListeners(ListServiceMessage.StyleItemSelect);
                        break;
                    case XmlListType.MPDisplayLanguages:
                        _listService.NotifyListeners(ListServiceMessage.LanguageItemSelect);
                        break;
                    default:
                        break;
                }
              
            });
        }

        private void NotifyListLayoutChanged()
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                _listService.NotifyListeners(ListServiceMessage.ListItemLayout);
                GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.ControlVisibilityChanged);
            });
        }

        public enum ListServiceMessage
        {
            ListItems,
            ListItemSelect,
            ListItemLayout,
            MenuItems,
            MenuItemSelect,
            GroupItems,
            GroupItemSelect,
            SkinItems,
            SkinItemSelect,
            StyleItems,
            StyleItemSelect,
            LanguageItems,
            LanguageItemSelect
        }
    }


    public class InfoRepository : IRepository
    {
        #region Singleton Implementation

        private InfoRepository() { }
        private static InfoRepository _instance;
        public static InfoRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new InfoRepository();
                }
                return _instance;
            }
        }

        public static void RegisterMessage<T>(InfoMessageType message, Action<T> callback)
        {
            Instance.InfoService.Register<T>(message, callback);
        }

        public static void DeregisterMessage(InfoMessageType message, object owner)
        {
            Instance.InfoService.Deregister(message, owner);
        }

        public static void NotifyListeners<T>(InfoMessageType message, T value)
        {
            Instance.NotifiyValueChanged<T>(message, value);
        }

        #endregion

        #region Fields

        private MessengerService<InfoMessageType> _infoService = new MessengerService<InfoMessageType>();
        private List<string> _currentEnabledPluginMap = new List<string>();
        private APIPlaybackType _playerType = APIPlaybackType.None;
        private APIPlaybackType _playbackType = APIPlaybackType.None;
        private APIPlaybackState _playbackState = APIPlaybackState.None;
       // private int _mediaPortalPlayerId = -1;
        private bool _isTvRecording = false;
        private bool _isFullscreenVideo = false;
        private int _mediaPortalWindowId = -1;
        private int _mediaPortalDialogId = -1;
        private int _previousWindowId = -1;
        private int _focusedDialogControlId = -1;
        private int _focusedWindowControlId = -1;

        #endregion

        public GUISettings Settings { get; set; }
        public XmlSkinInfo SkinInfo { get; set; }


        public void Initialize(GUISettings settings, XmlSkinInfo skininfo)
        {
            Settings = settings;
            SkinInfo = skininfo;
        }

        public void ClearRepository()
        {

        }

        public void ResetRepository()
        {

        }


        #region Properties

        public MessengerService<InfoMessageType> InfoService
        {
            get { return _infoService; }
        }

        /// <summary>
        /// Gets the current enabled plugin map.
        /// </summary>
        public List<string> EnabledPluginMap
        {
            get { return _currentEnabledPluginMap; }
            set 
            {
                if (_currentEnabledPluginMap != value)
                {
                    _currentEnabledPluginMap = value;
                    NotifiyValueChanged<List<string>>(InfoMessageType.EnabledPluginMap, value);
                    GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.ControlVisibilityChanged);
                }
            }
        }

        /// <summary>
        /// Gets the type of the current media portal player.
        /// </summary>
        public APIPlaybackType PlayerType
        {
            get { return _playerType; }
            set
            {
                if (_playerType != value)
                {
                    _playerType = value;
                    NotifiyValueChanged<APIPlaybackType>(InfoMessageType.PlayerType, value);
                    GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.GlobalVisibilityChanged);
                }
            }
        }

        public APIPlaybackType PlaybackType
        {
            get { return _playbackType; }
            set 
            {
                if (_playbackType != value)
                {
                    _playbackType = value;
                    NotifiyValueChanged<APIPlaybackType>(InfoMessageType.PlaybackType, value);
                    GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.GlobalVisibilityChanged);
                }
            }
        }

        public APIPlaybackState PlaybackState
        {
            get { return _playbackState; }
            set
            {
                if (_playbackState != value)
                {
                    _playbackState = value;
                    NotifiyValueChanged<APIPlaybackState>(InfoMessageType.PlaybackState, value);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is media portal tv recording.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is media portal tv recording; otherwise, <c>false</c>.
        /// </value>
        public bool IsTvRecording
        {
            get { return _isTvRecording; }
            set
            {
                if (_isTvRecording != value)
                {
                    _isTvRecording = value;
                    NotifiyValueChanged<bool>(InfoMessageType.IsTvRecording, value);
                    GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.ControlVisibilityChanged);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is media portal fullscreen video.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is media portal fullscreen video; otherwise, <c>false</c>.
        /// </value>
        public bool IsFullscreenVideo
        {
            get { return _isFullscreenVideo; }
            set 
            {
                if (_isFullscreenVideo != value)
                {
                    _isFullscreenVideo = value;
                    NotifiyValueChanged<bool>(InfoMessageType.IsFullscreenVideo, value);
                    GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.ControlVisibilityChanged);
                }
            }
        }

        /// <summary>
        /// Gets the current media portal window id.
        /// </summary>
        public int WindowId
        {
            get { return _mediaPortalWindowId; }
            set 
            {
                if (_mediaPortalWindowId != value)
                {
                    _mediaPortalWindowId = value;
                    NotifiyValueChanged<int>(InfoMessageType.WindowId, value);
                    GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.GlobalVisibilityChanged);
                }
            }
        }

        /// <summary>
        /// Gets the current media portal dialog id.
        /// </summary>
        public int DialogId
        {
            get { return _mediaPortalDialogId; }
            set
            {
                if (_mediaPortalDialogId != value)
                {
                    _mediaPortalDialogId = value;
                    NotifiyValueChanged<int>(InfoMessageType.DialogId, value);

                }
            }
        }

        /// <summary>
        /// Gets the previous media portal window id.
        /// </summary>
        public int PreviousWindowId
        {
            get { return _previousWindowId; }
            set 
            {
                if (_previousWindowId != value)
                {
                    _previousWindowId = value;
                    NotifiyValueChanged<int>(InfoMessageType.PreviousWindowId, value);
                    GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.GlobalVisibilityChanged);
                }
            }
        }

        /// <summary>
        /// Gets the current focused media portal control id.
        /// </summary>
        public int FocusedWindowControlId
        {
            get { return _focusedWindowControlId; }
            set
            {
                if (_focusedWindowControlId != value)
                {
                    _focusedWindowControlId = value;
                    NotifiyValueChanged<int>(InfoMessageType.FocusedWindowControlId, value);
                }
            }
        }

        /// <summary>
        /// Gets the current focused media portal dialog control id.
        /// </summary>
        public int FocusedDialogControlId
        {
            get { return _focusedDialogControlId; }
            set
            {
                if (_focusedDialogControlId != value)
                {
                    _focusedDialogControlId = value;
                    NotifiyValueChanged<int>(InfoMessageType.FocusedDialogControlId, value);
                }
            }
        }

        #endregion


        public bool IsSkinOptionEnabled(string option)
        {
            var skinOption = SkinInfo.SkinOptions.FirstOrDefault(o => o.Name == option);
            if (skinOption != null)
            {
                return skinOption.IsEnabled;
            }
            return false;
        }


        public void NotifiyValueChanged<T>(InfoMessageType type, T value)
        {
            InfoService.NotifyListeners(type, value);
        }


        public async Task AddInfo(APIInfoMessage message)
        {
            await Task.Factory.StartNew(() =>
            {
                if (message != null)
                {
                    if (message.MessageType == APIInfoMessageType.WindowMessage)
                    {
                        AddWindowMessage(message.WindowMessage);
                    }

                    if (message.MessageType == APIInfoMessageType.DialogMessage)
                    {
                        AddDialogMessage(message.DialogMessage);
                    }

                    if (message.MessageType == APIInfoMessageType.PlayerMessage)
                    {
                        AddPlayerMessage(message.PlayerMessage);
                    }
                }
            });
        }

      

        
        

        private void AddPlayerMessage(APIPlayerMessage message)
        {
            if (message != null)
            {
                PlayerType = message.PlayerPluginType;
                PlaybackType = message.PlaybackType;
                PlaybackState = message.PlaybackState;
            }
        }

        private void AddWindowMessage(APIWindowMessage message)
        {
            if (message != null)
            {
                if (message.MessageType == APIWindowMessageType.WindowId)
                {
                    WindowId = message.WindowId;
                }
            }
        }

        private void AddDialogMessage(APIDialogMessage message)
        {
            if (message != null)
            {

            }
        }
    }

    public enum InfoMessageType
    {
        EnabledPluginMap,
        PlayerType,
        PlaybackType,
        PlaybackState,
        IsTvRecording,
        IsFullscreenVideo,
        WindowId,
        DialogId,
        PreviousWindowId,
        FocusedWindowControlId,
        FocusedDialogControlId,
    }


    public class GenericDataRepository : IRepository
    {
        #region Singleton Implementation

        private GenericDataRepository() { }
        private static GenericDataRepository _instance;
        public static GenericDataRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GenericDataRepository();
                }
                return _instance;
            }
        }


        public static void RegisterEQData(Action<byte[]> callback)
        {
            Instance.RegisterMessage<byte[]>(GenericDataMessageType.EQData, callback);
        }

        public static void DegisterEQData(object owner)
        {
            Instance.DeregisterMessage(owner, GenericDataMessageType.EQData);
        }

        public static int GetEQDataLength(IControlHost host)
        {
           return Instance.GetMaxEQSize(host);
        }

        #endregion

        public GUISettings Settings { get; set; }
        public XmlSkinInfo SkinInfo { get; set; }
        private MessengerService<GenericDataMessageType> _dataService = new MessengerService<GenericDataMessageType>();

        public void Initialize(GUISettings settings, XmlSkinInfo skininfo)
        {
            Settings = settings;
            SkinInfo = skininfo;
        }

        public void ClearRepository()
        {

        }

        public void ResetRepository()
        {

        }


        private void AddDataMessage(APIDataMessage message)
        {
            if (message != null)
            {
                switch (message.DataType)
                {
                    case APIDataMessageType.EQData:
                        _dataService.NotifyListeners(GenericDataMessageType.EQData, message.ByteArray);
                        break;
                    default:
                        break;
                }
            }
        }

        private void RegisterMessage<T>(GenericDataMessageType messageType, Action<T> callback)
        {
            _dataService.Register<T>(messageType, callback);
        }

        private void DeregisterMessage(object owner, GenericDataMessageType messageType)
        {
            _dataService.Deregister(messageType, owner);
        }

        public int GetMaxEQSize(IControlHost controlHost)
        {
            if (controlHost != null)
            {
                var eqs = controlHost.Controls.GetControls().OfType<GUIEqualizer>();
                if (eqs != null && eqs.Any())
                {
                    return eqs.Max(e => e.EQDataLength);
                }
            }
            return -1;
        }
    }

    public enum GenericDataMessageType
    {
        EQData
    }

        //#region Other Info

        //public static void AddDataMessage(APIDataMessage dataMessage)
        //{
        //    if (dataMessage != null)
        //    {
        //        switch (dataMessage.DataType)
        //        {
        //            case APIDataMessageType.EQData:
        //                AddEQData(dataMessage.IntArray);
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //}

        //#region Equalizer

        //private static Dictionary<int,Action<int[]>> _equalizerCallbacks = new Dictionary<int,Action<int[]>>();

        //public static void AddEQData(int[] eqData)
        //{
        //    if (eqData != null)
        //    {
        //        lock (_equalizerCallbacks)
        //        {
        //            foreach (var callback in _equalizerCallbacks.Values)
        //            {
        //                Application.Current.Dispatcher.BeginInvoke((Action)delegate
        //                {
        //                    callback.Invoke(eqData);
        //                });
        //            }
        //        }
        //    }
        //}

        //public static void RegisterEQData(this GUIEqualizer control, Action<int[]> callback)
        //{
        //    if (control != null)
        //    {
        //        if (!_equalizerCallbacks.ContainsKey(control.Id))
        //        {
        //            lock (_equalizerCallbacks)
        //            {
        //                _equalizerCallbacks.Add(control.Id, callback);
        //            }
        //        }
        //    }
        //}

        //public static void DeregisterEQData(this GUIEqualizer control)
        //{
        //    if (control != null)
        //    {
        //        lock (_equalizerCallbacks)
        //        {
        //            _equalizerCallbacks.Remove(control.Id);
        //        }
        //    }
        //}

        //#endregion

      //  #endregion








 

    public class DataRepository<K, V>
    {
        private Dictionary<K, V> _repository = new Dictionary<K, V>();
        private Func<V, V, bool> _valueEquals;

        public DataRepository()
        {
        }

        public DataRepository(Func<V, V, bool> valueEquals)
        {
            this._valueEquals = valueEquals;
        }

        public bool AddOrUpdate(K key, V value)
        {
            if (key != null)
            {
                V exists;
                if (!_repository.TryGetValue(key, out exists))
                {
                    lock (_repository)
                    {
                        _repository.Add(key, value);
                    }
                    return true;
                }
                else
                {
                    if (_valueEquals == null || !_valueEquals.Invoke(value, exists))
                    {
                        lock (_repository)
                        {
                            _repository[key] = value;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public V GetValue(K key)
        {
            return _repository.GetValueOrDefault(key, default(V));
        }

        public V GetValueOrDefault(K key, V defaultValue)
        {
            lock (_repository)
            {
                V exists;
                if (_repository.TryGetValue(key, out exists))
                {
                    return exists;
                }
                return defaultValue;
            }
        }

        public void ClearRepository()
        {
            lock (_repository)
            {
                _repository.Clear();
            }
        }
    }
}
