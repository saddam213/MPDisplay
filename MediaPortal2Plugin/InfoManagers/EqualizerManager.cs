using System;
using System.Runtime.Remoting.Channels;
using System.Threading;
using Common.Log;
using Common.Settings;
using MediaPortal.UI.Presentation.Players;
using MediaPortal.UI.Players.BassPlayer;
using MessageFramework.Messages;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Cd;
using Un4seen.BassWasapi;
using MediaPortal.Common;

namespace MediaPortal2Plugin.InfoManagers
{
    public class EqualizerManager
    {
        #region Singleton Implementation

        private static EqualizerManager _instance;

        private EqualizerManager()
        {
            _log = LoggingManager.GetLog(typeof(EqualizerManager));
        }

        public static EqualizerManager Instance => _instance ?? (_instance = new EqualizerManager());

        #endregion

        private readonly Log _log;
        private bool _isEqRunning;
        private readonly float[] _eqFftData = new float[4096];
        private Timer _eqThread;
        private int _eqDataLength = 50;
        private const int RefreshRate = 60;

        //private bool _isRegistered;

        public void Initialize(PluginSettings settings)
        {
        }

        public void Shutdown()
        {
            StopEqThread();
        }

    

        public void RegisterEqualizer(int eqLength)
        {
            _eqDataLength = eqLength > 0 ? eqLength : 0;
        }

        public void StartEqualizer()
        {
            if (!_isEqRunning)
            {
                StartEqThread();
            }
        }

        public void StopEqualizer()
        {
            StopEqThread();
        }


        /// <summary>
        /// Starts the EQ thread.
        /// </summary>
        private void StartEqThread()
        {
            StopEqThread();
            _log.Message(LogLevel.Info, "[EQManager]-[StartEQThread] - Starting equalizer data thread.");
            if (_eqThread == null)
            {
                _eqThread = new Timer(GetBassFftData, null, 500, RefreshRate);
            }
            _isEqRunning = true;
        }

        /// <summary>
        /// Stops the EQ thread.
        /// </summary>
        private void StopEqThread()
        {
            if (!_isEqRunning) return;
            _log.Message(LogLevel.Info, "[EQManager]-[StopEQThread] - Stopping equalizer data thread.");
            _eqThread = null;
            _isEqRunning = false;
        }

        // set the flags as follows:
        // get individual fft for 2 channels, else combined fft
        // get data according to sampling freq of the channel
        private static int GetBassGetDataFlags(int freq, int chans)
        {
            int flags;

            if (chans == 2)
            {
                flags = (int)BASSData.BASS_DATA_FFT_INDIVIDUAL;
            }
            else
            {
                flags = 0;
            }

            if (freq < 50000)
            {
                flags |= (int)BASSData.BASS_DATA_FFT1024;
            }
            else if (freq < 100000)
            {
                flags |= (int)BASSData.BASS_DATA_FFT2048;
            }
            else
            {
                flags |= (int)BASSData.BASS_DATA_FFT4096;
            }
            return flags;
        }

        /// <summary>
        /// Gets the Bass.Net FFT data.
        /// </summary>
        /// <param name="state">The state.</param>
        private void GetBassFftData(object state)
        {
            try
            {
                var player = WindowManager.Instance.AudioPlayer;
                if (player?.AVType != AVType.Audio) return;
   
                if (player.PlaybackState != PlaybackState.Playing) return;

                var bassplayer = (BassPlayer) player.CurrentPlayer;

                // TODO: get the handle for Bass stream!
                if (bassplayer == null) return;

                lock (_eqFftData)
                {
                    var lines = 16;                        // number of spectrum lines
                    // indices in fft data for the frequency slots
                    // please note: if # lines is changed indices need to be recalculated!
                    int[] lineIndex = { 1, 2, 3, 4, 5, 8, 12, 18, 28, 42, 64, 97, 147, 223, 338, 500, 511 };
                    int[] lineIndex2 = { 2, 4, 6, 8, 10, 16, 24, 36, 56, 84, 128, 194, 294, 446, 676, 1000, 1022 };
                    const int eqMultiplier = 255;
                    var length = _eqDataLength * 2;         // pass values for 2 channels
                    var eqData = new byte[length];

                    if (!bassplayer.GetFFTData(_eqFftData)) return;
                    if (_eqDataLength < lines) lines = _eqDataLength;                 // EQ requests less lines than available
                    //compute the spectrum data for 2 channels
                  int index;
                  for (index = 0; index < lines; index++)
                    {
                        float peak = 0;
                        float peak2 = 0;
                        var eqIndex = 2 * index;
                      int innerIndex;
                      for (innerIndex = lineIndex2[index]; innerIndex < lineIndex2[index + 1]; innerIndex += 2)
                        {
                            if (peak < _eqFftData[innerIndex]) peak = _eqFftData[innerIndex];
                            if (peak2 < _eqFftData[innerIndex+1]) peak2 = _eqFftData[innerIndex+1];
                        }
                        eqData[eqIndex] = (byte)Math.Min(255, Math.Max(Math.Sqrt(peak * 2) * eqMultiplier, 1));
                        eqData[eqIndex + 1] = (byte)Math.Min(255, Math.Max(Math.Sqrt(peak2 * 2) * eqMultiplier, 1));
                    }
 

                    for (index = lines*2; index < length; index++)                // pad with 0 in case mare lines were requested
                    {
                        eqData[index] = 0;
                    }
                    MessageService.Instance.SendDataMessage(new APIDataMessage { DataType = APIDataMessageType.EQData, ByteArray = eqData });
                }
            }
            catch (Exception ex)
            {
                _log.Message(LogLevel.Error, "[Equalizer]-[GetBassFFTData] - An exception occured processing Equalizer data " + Environment.NewLine + ex);
            }
        }
    }
}
