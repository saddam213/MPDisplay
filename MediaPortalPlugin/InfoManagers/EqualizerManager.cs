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
        private bool _isMusicPlaying = false;
        private int _refreshRate = 60;
        private int _eqMultiplier = 255;
        private PluginSettings _settings;



        public void Initialize(PluginSettings settings)
        {
            _settings = settings;
            g_Player.PlayBackStarted += new g_Player.StartedHandler(Player_PlayBackStarted);
            g_Player.PlayBackEnded += new g_Player.EndedHandler(Player_PlayBackEnded);
            g_Player.PlayBackStopped += new g_Player.StoppedHandler(Player_PlayBackStopped);
        }

        public void Shutdown()
        {
            g_Player.PlayBackStarted -= new g_Player.StartedHandler(Player_PlayBackStarted);
            g_Player.PlayBackEnded -= new g_Player.EndedHandler(Player_PlayBackEnded);
            g_Player.PlayBackStopped -= new g_Player.StoppedHandler(Player_PlayBackStopped);
        }

        public void RegisterEqualizer(int eqLength)
        {
            if (eqLength > 0)
            {
                StopEQThread();
                _eqDataLength = eqLength;
                StartEQThread();
            }
            else
            {
                if (_eqDataLength != 0)
                {
                    StopEQThread();
                    _eqDataLength = 0;
                }
            }
        }
  
        /// <summary>
        /// Player play back stopped.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="stoptime">The stoptime.</param>
        /// <param name="filename">The filename.</param>
        private void Player_PlayBackStopped(g_Player.MediaType type, int stoptime, string filename)
        {
            _isMusicPlaying = false;
            StopEQThread();
        }

        /// <summary>
        /// Player play back ended.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="filename">The filename.</param>
        private void Player_PlayBackEnded(g_Player.MediaType type, string filename)
        {
            _isMusicPlaying = false;
            StopEQThread();
        }

        /// <summary>
        /// Player play back started.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="filename">The filename.</param>
        private void Player_PlayBackStarted(g_Player.MediaType type, string filename)
        {
            if (IsMusicPlayback(type))
            {
                _isMusicPlaying = true;
                StartEQThread();
            }
            else
            {
                _isMusicPlaying = false;
                StopEQThread();
            }
        }

        /// <summary>
        /// Starts the EQ thread.
        /// </summary>
        private void StartEQThread()
        {
            if (!_isEQRunning && _isMusicPlaying)
            {
                Log.Message(LogLevel.Info, "[EQManager]-[StartEQThread] - Starting equalizer data thread.");
                if (_eqThread == null)
                {
                    _eqThread = new System.Threading.Timer(GetBassFFTData, null, 500, _refreshRate);
                }
                _isEQRunning = true;
            }
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
                                byte[] eqData = new byte[_eqDataLength];
                                for (int i = 0; i < _eqDataLength; i++)
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

        /// <summary>
        /// Determines whether current playback type is music.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if is music playback; otherwise, <c>false</c>.
        /// </returns>
        private bool IsMusicPlayback(g_Player.MediaType type)
        {
            switch (type)
            {
                case g_Player.MediaType.Music:
                case g_Player.MediaType.Radio:
                case g_Player.MediaType.RadioRecording:
                case g_Player.MediaType.Unknown:
                    return true;
                default:
                    break;
            }
            return false;
        }

    }
}
