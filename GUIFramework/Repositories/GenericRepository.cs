using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUIFramework.GUI;
using GUIFramework.GUI.Controls;
using GUISkinFramework;
using MessageFramework.DataObjects;
using MPDisplay.Common;
using MPDisplay.Common.Settings;

namespace GUIFramework.Managers
{
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
            Instance.DataService.Register<byte[]>(GenericDataMessageType.EQData, callback);
        }

        public static void DegisterEQData(object owner)
        {
            Instance.DataService.Deregister(GenericDataMessageType.EQData, owner);
        }

        public static int GetEQDataLength(IControlHost host)
        {
            return Instance.GetMaxEQSize(host);
        }


        public static void RegisterMessage(GenericDataMessageType message, Action callback)
        {
            Instance.DataService.Register(message, callback);
        }

        public static void RegisterMessage<T>(GenericDataMessageType message, Action<T> callback)
        {
            Instance.DataService.Register<T>(message, callback);
        }

        public static void DeregisterMessage(GenericDataMessageType message, object owner)
        {
            Instance.DataService.Deregister(message, owner);
        }

        #endregion

        public MessengerService<GenericDataMessageType> DataService
        {
            get { return _dataService; }
        }

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


        public void AddDataMessage(APIDataMessage message)
        {
            if (message != null)
            {
                switch (message.DataType)
                {
                    case APIDataMessageType.KeepAlive:
                        break;
                    case APIDataMessageType.EQData:
                        DataService.NotifyListeners(GenericDataMessageType.EQData, message.ByteArray);
                        break;
                    case APIDataMessageType.ResetIteraction:
                        DataService.NotifyListeners(GenericDataMessageType.ResetIteraction);
                        break;
                    case APIDataMessageType.MPActionId:
                        DataService.NotifyListeners(GenericDataMessageType.MPActionId, message.IntValue);
                        break;
                    default:
                        break;
                }
            }
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
        EQData,
        LastMPAction,
        LastUserMPAction,
        ResetIteraction,
        MPActionId
    }
}
