using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Helpers;
using MediaPortal.Configuration;
using MediaPortal.Player;
using MessageFramework.DataObjects;
using Common.Logging;
using Common.Settings;

namespace MediaPortalPlugin.InfoManagers
{
    public class EqualizerManager
    {
        #region Singleton Implementation

        private static EqualizerManager instance;

        private EqualizerManager()
        {
            Log = Common.Logging.LoggingManager.GetLog(typeof(EqualizerManager));
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


        private Common.Logging.Log Log;
        private bool _isEQRunning;
        private float[] _eqFftData = new float[4096];
        private System.Threading.Timer _eqThread;
        private int _eqDataLength = 50;
        private int _refreshRate = 60;
       private PluginSettings _settings;
        //private bool _isRegistered;

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
                //_isRegistered = true;
                _eqDataLength = eqLength;
            }
            else
            {
                //_isRegistered = true;
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

        // set the flags as follows:
        // get individual fft for 2 channels, else combined fft
        // get data according to sampling freq of the channel
        private int getBassGetDataFlags(int freq, int chans)
        {
            int flags;

            if (chans == 2)
            {
                flags = (int)Un4seen.Bass.BASSData.BASS_DATA_FFT_INDIVIDUAL;
            }
            else
            {
                flags = 0;
            }

            if (freq < 50000)
            {
                flags |= (int)Un4seen.Bass.BASSData.BASS_DATA_FFT1024;
            }
            else if (freq < 100000)
            {
                flags |= (int)Un4seen.Bass.BASSData.BASS_DATA_FFT2048;
            }
            else
            {
                flags |= (int)Un4seen.Bass.BASSData.BASS_DATA_FFT4096;
            }
            return flags;
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
                            Un4seen.BassWasapi.BASS_WASAPI_INFO _wasapiInfo;
                            Un4seen.Bass.BASS_CHANNELINFO _bassInfo;
                            int _lines = 16;                        // number of spectrum lines
                            // indices in fft data for the frequency slots
                            // please note: if # lines is changed indices need to be recalculated!
                            int[] lineIndex = { 1, 2, 3, 4, 5, 8, 12, 18, 28, 42, 64, 97, 147, 223, 338, 500, 511 };
                            int[] lineIndex2 = { 2, 4, 6, 8, 10, 16, 24, 36, 56, 84, 128, 194, 294, 446, 676, 1000, 1022 };
                            float peak, peak2;
                            int index;
                            int eqIndex;
                            int innerIndex;
                            int _eqMultiplier = 255;
                            int length = _eqDataLength * 2;         // pass values for 2 channels
                            byte[] eqData = new byte[length];
                            int chans;
                            int channel;

                            channel = 0;
                            chans = 0;
                            if (Un4seen.BassWasapi.BassWasapi.BASS_WASAPI_IsStarted())
                            {
                                    _wasapiInfo = Un4seen.BassWasapi.BassWasapi.BASS_WASAPI_GetInfo();
                                    chans = _wasapiInfo.chans;
                                    channel = Un4seen.BassWasapi.BassWasapi.BASS_WASAPI_GetData(_eqFftData, getBassGetDataFlags(_wasapiInfo.freq, chans));
                            }
                            else
                            {
                                _bassInfo = Un4seen.Bass.Bass.BASS_ChannelGetInfo((int)g_Player.CurrentAudioStream);
                                chans = _bassInfo.chans;
                                channel = Un4seen.Bass.Bass.BASS_ChannelGetData((int)g_Player.CurrentAudioStream, _eqFftData, getBassGetDataFlags(_bassInfo.freq, chans ));
                            }

                            if (channel > 0)
                            {
                                if (_eqDataLength < _lines) _lines = _eqDataLength;                 // EQ requests less lines than available
                                //compute the spectrum data for 2 channels
                                if (chans == 2)
                                {
                                   for (index = 0; index < _lines; index++)
                                    {
                                        peak = 0;                                                   // determine peak in band range
                                        peak2 = 0;
                                        eqIndex = 2 * index;
                                        for (innerIndex = lineIndex2[index]; innerIndex < lineIndex2[index + 1]; innerIndex += 2)
                                        {
                                            if (peak < _eqFftData[innerIndex]) peak = _eqFftData[innerIndex];
                                            if (peak2 < _eqFftData[innerIndex+1]) peak2 = _eqFftData[innerIndex+1];
                                        }
                                        eqData[eqIndex] = (byte)Math.Min(255, Math.Max(Math.Sqrt((peak) * 2) * _eqMultiplier, 1));
                                        eqData[eqIndex + 1] = (byte)Math.Min(255, Math.Max(Math.Sqrt((peak2) * 2) * _eqMultiplier, 1));
                                    }
                                }
                                else
                                {
                                    for (index = 0; index < _lines; index++)
                                    {
                                        peak = 0;                               // determine peak in band range
                                        eqIndex = 2 * index;
                                        for (innerIndex = lineIndex[index]; innerIndex < lineIndex[index + 1]; innerIndex++)
                                        {
                                            if (peak < _eqFftData[innerIndex]) peak = _eqFftData[innerIndex];
                                        }
                                        eqData[eqIndex] = (byte)Math.Min(255, Math.Max(Math.Sqrt((peak) * 2) * _eqMultiplier, 1));
                                        eqData[eqIndex + 1] = eqData[eqIndex];
                                    }
                                             
                                 }
                                for (index = _lines*2; index < length; index++)                // pad with 0 in case mare lines were requested
                                {
                                    eqData[index] = (byte)0;
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
