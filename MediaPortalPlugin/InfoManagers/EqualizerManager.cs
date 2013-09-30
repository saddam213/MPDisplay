using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Helpers;
using MediaPortal.Configuration;
using MediaPortal.Player;
using MessageFramework.DataObjects;
using MPDisplay.Common.Log;
using MPDisplay.Common.Settings;

namespace MediaPortalPlugin.InfoManagers
{
    public class EqualizerManager
    {
        #region Singleton Implementation

        private static EqualizerManager instance;

        private EqualizerManager()
        {
            Log = MPDisplay.Common.Log.LoggingManager.GetLog(typeof(EqualizerManager));
        }

        public static EqualizerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EqualizerManager();
                }
                return instance;
            }
        }

        #endregion


        private MPDisplay.Common.Log.Log Log;
        private bool _isEQRunning;
        private float[] _eqFftData = new float[1280];
        private System.Threading.Timer _eqThread;
        private int _eqDataLength = 50;
        private int _refreshRate = 60;
        private int _eqMultiplier = 255;
        private PluginSettings _settings;
        private bool _isRegistered = false;


        public void Initialize(PluginSettings settings)
        {
            _settings = settings;
        }

        public void Shutdown()
        {
            StopEQThread();
        }

    

        public void RegisterEqualizer(int eqLength)
        {
            if (eqLength > 0)
            {
                _isRegistered = true;
                _eqDataLength = eqLength;
            }
            else
            {
                _isRegistered = true;
                _eqDataLength = 0;
            }
        }

        public void StartEqualizer()
        {
            if (!_isEQRunning)
            {
                StartEQThread();
            }
        }

        public void StopEqualizer()
        {
            StopEQThread();
        }


        /// <summary>
        /// Starts the EQ thread.
        /// </summary>
        private void StartEQThread()
        {
            StopEQThread();
            Log.Message(LogLevel.Info, "[EQManager]-[StartEQThread] - Starting equalizer data thread.");
            if (_eqThread == null)
            {
                _eqThread = new System.Threading.Timer(GetBassFFTData, null, 500, _refreshRate);
            }
            _isEQRunning = true;
        }

        /// <summary>
        /// Stops the EQ thread.
        /// </summary>
        private void StopEQThread()
        {
            if (_isEQRunning)
            {
                Log.Message(LogLevel.Info, "[EQManager]-[StopEQThread] - Stopping equalizer data thread.");
                if (_eqThread != null)
                {
                    _eqThread = null;
                }
                _isEQRunning = false;
            }
        }


       

     
        /// <summary>
        /// Gets the Bass.Net FFT data.
        /// </summary>
        /// <param name="state">The state.</param>
        private void GetBassFFTData(object state)
        {
            try
            {
                if (g_Player.Playing)
                {
                    if (g_Player.CurrentAudioStream != 0 && g_Player.CurrentAudioStream != -1)
                    {
                        lock (_eqFftData)
                        {
                            int channel = Un4seen.Bass.Bass.BASS_ChannelGetData((int)g_Player.CurrentAudioStream, _eqFftData, int.MinValue);
                            if (channel > 0)
                            {
                                int length = _eqDataLength;
                                byte[] eqData = new byte[length];
                                for (int i = 0; i < length; i++)
                                {
                                    eqData[i] = (byte)Math.Min(255, Math.Max(Math.Sqrt((_eqFftData[i]) * 2) * _eqMultiplier, 1));
                                }
                                MessageService.Instance.SendDataMessage(new APIDataMessage { DataType = APIDataMessageType.EQData, ByteArray = eqData });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Message(LogLevel.Error, "[Equalizer]-[GetBassFFTData] - An exception occured processing Equalizer data " + Environment.NewLine + ex.ToString());
            }
        }
    }
}
