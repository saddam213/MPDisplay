using System;
using System.Collections.Generic;
using System.Linq;
using Common.MessengerService;
using Common.Settings;
using GUIFramework.GUI;
using GUIFramework.Utils;
using GUISkinFramework.Skin;
using MessageFramework.Messages;

namespace GUIFramework.Repositories
{
    public class GenericDataRepository : IRepository
    {
        #region Singleton Implementation

        private GenericDataRepository() { }
        private static GenericDataRepository _instance;
        public static GenericDataRepository Instance
        {
            get { return _instance ?? (_instance = new GenericDataRepository()); }
        }


        public static void RegisterEQData(Action<byte[]> callback)
        {
            Instance.DataService.Register(GenericDataMessageType.EQData, callback);
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
            Instance.DataService.Register(message, callback);
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
            ClearRepository();
            Settings = null;
            SkinInfo = null;
        }


        public void AddDataMessage(APIDataMessage message)
        {
            if (message == null) return;

            switch (message.DataType)
            {
                case APIDataMessageType.KeepAlive:
                    break;
                case APIDataMessageType.EQData:
                    DataService.NotifyListeners(GenericDataMessageType.EQData, message.ByteArray);
                    break;
                case APIDataMessageType.MPActionId:
                    DataService.NotifyListeners(GenericDataMessageType.MPActionId, message.IntValue);
                    break;
            }
        }


        public int GetMaxEQSize(IControlHost controlHost)
        {
            if (controlHost == null) return -1;

            var eqs = controlHost.Controls.GetControls().OfType<GUIEqualizer>();
            var guiEqualizers = eqs as IList<GUIEqualizer> ?? eqs.ToList();
            if (guiEqualizers.Any())
            {
                return guiEqualizers.Max(e => e.EQDataLength);
            }
            return -1;
        }
    }

    public enum GenericDataMessageType
    {
        EQData,
        MPActionId
    }
}
