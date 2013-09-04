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


        public void AddDataMessage(APIDataMessage message)
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
}
