using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MPDisplay.Common.Log
{
    public class WindowLogger : Logger
    {
        private Window _logWindow;
        private TextBox _logBox;
        private ScrollViewer _scrollViewer;

        public WindowLogger(string name) : base()
        {
            _logBox = new TextBox();
            _scrollViewer = new ScrollViewer
            {
                Content = _logBox,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            _logWindow = new Window
            {
                Title = "WindowLogger: " + name,
                Content = _scrollViewer
            };
            _logWindow.Show();
        }

        protected override void LogQueuedMessage(string message)
        {
            try
            {
                _logBox.Dispatcher.Invoke((Action)delegate
                  {
                      _logBox.AppendText(message + Environment.NewLine);
                      _scrollViewer.ScrollToBottom();
                  }, DispatcherPriority.Background);
            }
            catch { }
        }

        public override void Dispose()
        {
            _logBox.Clear();
            _logBox = null;
            _scrollViewer = null;
            _logWindow.Dispatcher.Invoke((Action)delegate { _logWindow.Close(); });
            _logWindow = null;
           
            base.Dispose();
        }
    }
}
