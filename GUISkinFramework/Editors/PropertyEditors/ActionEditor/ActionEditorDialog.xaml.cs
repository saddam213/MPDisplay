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
using System.Windows.Shapes;
using GUISkinFramework.Common;
using MPDisplay.Common.Utils;
using GUISkinFramework.ExtensionMethods;

namespace GUISkinFramework.Editor.PropertyEditors
{
    /// <summary>
    /// Interaction logic for VisibleConditionEditorDialog.xaml
    /// </summary>
    public partial class ActionEditorDialog : Window, INotifyPropertyChanged
    {
        private object _instance;
        private XmlAction _selectedAction;
        private ObservableCollection<XmlAction> _xmlActions = new ObservableCollection<XmlAction>();

        public ActionEditorDialog(object instance)
        {
            ItemMoveUp = new RelayCommand(param => XmlActions.MoveItemUp((int)param), param => param == null ? false : XmlActions.CanItemMoveUp((int)param));
            ItemMoveDown = new RelayCommand(param => XmlActions.MoveItemDown((int)param), param => param == null ? false : XmlActions.CanItemMoveDown((int)param));
            ItemAdd= new RelayCommand(param =>  XmlActions.Add(new XmlAction { ActionType = XmlActionType.Empty }));
            ItemRemove = new RelayCommand(param =>  XmlActions.Remove(SelectedAction), param => SelectedAction != null);
            ResultOK = new RelayCommand(param => DialogResult = true);
            ResultCancel = new RelayCommand(param => DialogResult = false);
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            Instance = instance;
        }

        public ICommand ItemMoveUp { get; internal set; }
        public ICommand ItemMoveDown { get; internal set; }
        public ICommand ItemAdd { get; internal set; }
        public ICommand ItemRemove { get; internal set; }
        public ICommand ResultOK { get; internal set; }
        public ICommand ResultCancel { get; internal set; }

        public object Instance
        {
            get { return _instance; }
            set { _instance = value; NotifyPropertyChanged("Instance"); }
        }

        public ObservableCollection<XmlAction> XmlActions
        {
            get { return _xmlActions; }
            set { _xmlActions = value; NotifyPropertyChanged("XmlActions"); }
        }

        public XmlAction SelectedAction
        {
            get { return _selectedAction; }
            set { _selectedAction = value; NotifyPropertyChanged("SelectedAction"); }
        }


        public void SetItems(ObservableCollection<XmlAction> items)
        {
            if (items != null)
            {
                foreach (var xmlaction in items)
                {
                    XmlActions.Add(new XmlAction { ActionType = xmlaction.ActionType, Param1 = xmlaction.Param1, Param2 = xmlaction.Param2 });
                }
            }
        }

        public ObservableCollection<XmlAction> GetItems()
        {
            return new ObservableCollection<XmlAction>(XmlActions.Where(a => a.ActionType != XmlActionType.Empty).ToList());
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
    }



    public class ActionTypeToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isCombo = false;
            if (value is XmlActionType && bool.TryParse(parameter.ToString(), out isCombo))
            {
              
                var action = (XmlActionType)value;

                switch (action)
                {
                    case XmlActionType.MediaPortalWindow:
                    case XmlActionType.ControlVisible:
                    case XmlActionType.OpenWindow:
                    case XmlActionType.OpenDialog:
                    case XmlActionType.RunProgram:
                    case XmlActionType.KillProgram:
                        return !isCombo ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                    case XmlActionType.MediaPortalAction:
                        return isCombo ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                    default:
                        break;
                }

            }
            return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class ActionTypeToDescription : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is XmlActionType)
            {
              var attribute = (XmlActionTypeDetailsAttribute)typeof(XmlActionType)
                    .GetMember(value.ToString()).FirstOrDefault()
                    .GetCustomAttributes(typeof(XmlActionTypeDetailsAttribute), false).FirstOrDefault();
              return attribute == null ? value.ToString() : attribute.ParamName;

            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public enum MediaPortalActions
    {
        ACTION_INVALID = 0,
        ACTION_MOVE_LEFT = 1,
        ACTION_MOVE_RIGHT = 2,
        ACTION_MOVE_UP = 3,
        ACTION_MOVE_DOWN = 4,
        ACTION_PAGE_UP = 5,
        ACTION_PAGE_DOWN = 6,
        ACTION_SELECT_ITEM = 7,
        ACTION_HIGHLIGHT_ITEM = 8,
        ACTION_PARENT_DIR = 9,
        ACTION_PREVIOUS_MENU = 10,
        ACTION_SHOW_INFO = 11,
        ACTION_PAUSE = 12,
        ACTION_STOP = 13,
        ACTION_NEXT_ITEM = 14,
        ACTION_PREV_ITEM = 15,
        ACTION_FORWARD = 16,
        ACTION_REWIND = 17,
        ACTION_SHOW_GUI = 18,
        ACTION_ASPECT_RATIO = 19,
        ACTION_STEP_FORWARD = 20,
        ACTION_STEP_BACK = 21,
        ACTION_BIG_STEP_FORWARD = 22,
        ACTION_BIG_STEP_BACK = 23,
        ACTION_SHOW_OSD = 24,
        ACTION_SHOW_SUBTITLES = 25,
        ACTION_NEXT_AUDIO = 26,
        ACTION_SHOW_CODEC = 27,
        ACTION_NEXT_PICTURE = 28,
        ACTION_PREV_PICTURE = 29,
        ACTION_ZOOM_OUT = 30,
        ACTION_ZOOM_IN = 31,
        ACTION_TOGGLE_SOURCE_DEST = 32,
        ACTION_SHOW_PLAYLIST = 33,
        ACTION_QUEUE_ITEM = 34,
        ACTION_REMOVE_ITEM = 35,
        ACTION_SHOW_FULLSCREEN = 36,
        ACTION_ZOOM_LEVEL_NORMAL = 37,
        ACTION_ZOOM_LEVEL_1 = 38,
        ACTION_ZOOM_LEVEL_2 = 39,
        ACTION_ZOOM_LEVEL_3 = 40,
        ACTION_ZOOM_LEVEL_4 = 41,
        ACTION_ZOOM_LEVEL_5 = 42,
        ACTION_ZOOM_LEVEL_6 = 43,
        ACTION_ZOOM_LEVEL_7 = 44,
        ACTION_ZOOM_LEVEL_8 = 45,
        ACTION_ZOOM_LEVEL_9 = 46,
        ACTION_CALIBRATE_SWAP_ARROWS = 47,
        ACTION_CALIBRATE_RESET = 48,
        ACTION_ANALOG_MOVE = 49,
        ACTION_ROTATE_PICTURE = 50,
        ACTION_CLOSE_DIALOG = 51,
        ACTION_SUBTITLE_DELAY_MIN = 52,
        ACTION_SUBTITLE_DELAY_PLUS = 53,
        ACTION_AUDIO_DELAY_MIN = 54,
        ACTION_AUDIO_DELAY_PLUS = 55,
        ACTION_AUDIO_NEXT_LANGUAGE = 56,
        ACTION_CHANGE_RESOLUTION = 57,
        REMOTE_0 = 58,
        REMOTE_1 = 59,
        REMOTE_2 = 60,
        REMOTE_3 = 61,
        REMOTE_4 = 62,
        REMOTE_5 = 63,
        REMOTE_6 = 64,
        REMOTE_7 = 65,
        REMOTE_8 = 66,
        REMOTE_9 = 67,
        ACTION_PLAY = 68,
        ACTION_OSD_SHOW_LEFT = 69,
        ACTION_OSD_SHOW_RIGHT = 70,
        ACTION_OSD_SHOW_UP = 71,
        ACTION_OSD_SHOW_DOWN = 72,
        ACTION_OSD_SHOW_SELECT = 73,
        ACTION_OSD_SHOW_VALUE_PLUS = 74,
        ACTION_OSD_SHOW_VALUE_MIN = 75,
        ACTION_SMALL_STEP_BACK = 76,
        ACTION_MUSIC_FORWARD = 77,
        ACTION_MUSIC_REWIND = 78,
        ACTION_MUSIC_PLAY = 79,
        ACTION_DELETE_ITEM = 80,
        ACTION_COPY_ITEM = 81,
        ACTION_MOVE_ITEM = 82,
        ACTION_SHOW_MPLAYER_OSD = 83,
        ACTION_OSD_HIDESUBMENU = 84,
        ACTION_TAKE_SCREENSHOT = 85,
        ACTION_INCREASE_TIMEBLOCK = 86,
        ACTION_DECREASE_TIMEBLOCK = 87,
        ACTION_DEFAULT_TIMEBLOCK = 88,
        ACTION_RECORD = 89,
        ACTION_DVD_MENU = 90,
        ACTION_NEXT_CHAPTER = 91,
        ACTION_PREV_CHAPTER = 92,
        ACTION_KEY_PRESSED = 93,
        ACTION_PREV_CHANNEL = 94,
        ACTION_NEXT_CHANNEL = 95,
        ACTION_TVGUIDE_RESET = 96,
        ACTION_EXIT = 97,
        ACTION_REBOOT = 98,
        ACTION_SHUTDOWN = 99,
        ACTION_EJECTCD = 100,
        ACTION_BACKGROUND_TOGGLE = 101,
        ACTION_VOLUME_DOWN = 102,
        ACTION_VOLUME_UP = 103,
        ACTION_TOGGLE_WINDOWED_FULLSCREEN = 104,
        ACTION_PAUSE_PICTURE = 105,
        ACTION_CONTEXT_MENU = 106,
        ACTION_HOME = 109,
        ACTION_END = 110,
        ACTION_LAST_VIEWED_CHANNEL = 111,
        ACTION_IMPORT_TRACK = 112,
        ACTION_IMPORT_DISC = 113,
        ACTION_CANCEL_IMPORT = 114,
        ACTION_SWITCH_HOME = 115,
        ACTION_MOVE_SELECTED_ITEM_UP = 116,
        ACTION_MOVE_SELECTED_ITEM_DOWN = 117,
        ACTION_DELETE_SELECTED_ITEM = 118,
        ACTION_NEXT_SUBTITLE = 119,
        ACTION_SHOW_ACTIONMENU = 120,
        ACTION_TOGGLE_SMS_INPUT = 121,
        ACTION_AUTOZAP = 122,
        ACTION_MPRESTORE = 123,
        ACTION_SMALL_STEP_FORWARD = 124,
        ACTION_JUMP_MUSIC_NOW_PLAYING = 125,
        ACTION_ADD_TO_PLAYLIST = 126,
        ACTION_NEXT_EDITION = 134,
        ACTION_NEXT_VIDEO = 135,
        ACTION_PREV_BOOKMARK = 140,
        ACTION_NEXT_BOOKMARK = 141,
        ACTION_LASTFM_LOVE = 800,
        ACTION_LASTFM_BAN = 801,
        ACTION_POWER_OFF = 991,
        ACTION_SUSPEND = 992,
        ACTION_HIBERNATE = 993,
        ACTION_BD_POPUP_MENU = 1700,
        ACTION_MOUSE_MOVE = 2000,
        ACTION_MOUSE_CLICK = 2001,
        ACTION_MOUSE_DOUBLECLICK = 2002,
        ACTION_AUTOCROP = 9884,
        ACTION_TOGGLE_AUTOCROP = 9885,
        ACTION_TOGGLE_MUSIC_GAP = 9886,
        ACTION_REMOTE_RED_BUTTON = 9975,
        ACTION_REMOTE_GREEN_BUTTON = 9976,
        ACTION_REMOTE_YELLOW_BUTTON = 9977,
        ACTION_REMOTE_BLUE_BUTTON = 9978,
        ACTION_REMOTE_SUBPAGE_UP = 9979,
        ACTION_REMOTE_SUBPAGE_DOWN = 9980,
        ACTION_SHOW_VOLUME = 9981,
        ACTION_VOLUME_MUTE = 9982,
        ACTION_SHOW_CURRENT_TV_INFO = 9983,
        ACTION_NEXT_TELETEXTPAGE = 9984,
        ACTION_PREV_TELETEXTPAGE = 9985,
        ACTION_SWITCH_TELETEXT_HIDDEN = 9986,
        ACTION_SWITCH_TELETEXT_TRANSPARENT = 9987,
        ACTION_SHOW_INDEXPAGE = 9988,
        ACTION_SKIN_NEXT = 9989,
        ACTION_SKIN_PREVIOUS = 9990,
        ACTION_TVGUIDE_INCREASE_DAY = 9991,
        ACTION_TVGUIDE_DECREASE_DAY = 9992,
        ACTION_TVGUIDE_NEXT_GROUP = 9995,
        ACTION_TVGUIDE_PREV_GROUP = 9996,
        ACTION_ROTATE_PICTURE_180 = 9997,
        ACTION_ROTATE_PICTURE_270 = 9998,
    }
}
