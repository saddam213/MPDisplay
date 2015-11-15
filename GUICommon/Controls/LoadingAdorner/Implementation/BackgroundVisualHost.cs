﻿using System;
using System.Collections;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace MPDisplay.Common.Controls
{
    public delegate Visual CreateContentFunction();

    public class BackgroundVisualHost : FrameworkElement
    {
        #region Private Fields
        private ThreadedVisualHelper _threadedHelper;
        private HostVisual _hostVisual; 
        #endregion

        #region IsContentShowingProperty
        /// <summary>
        /// Identifies the IsContentShowing dependency property.
        /// </summary>
        public static readonly DependencyProperty IsContentShowingProperty = DependencyProperty.Register(
            "IsContentShowing",
            typeof(bool),
            typeof(BackgroundVisualHost),
            new FrameworkPropertyMetadata(false, OnIsContentShowingChanged));

        /// <summary>
        /// Gets or sets if the content is being displayed.
        /// </summary>
        public bool IsContentShowing
        {
            get { return (bool)GetValue(IsContentShowingProperty); }
            set { SetValue(IsContentShowingProperty, value); }
        }

        static void OnIsContentShowingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bvh = (BackgroundVisualHost)d;

            if (bvh.CreateContent == null) return;

            if ((bool)e.NewValue)
            {
                bvh.CreateContentHelper();
            }
            else
            {
                bvh.HideContentHelper();
            }
        }
        #endregion

        #region CreateContent Property
        /// <summary>
        /// Identifies the CreateContent dependency property.
        /// </summary>
        public static readonly DependencyProperty CreateContentProperty = DependencyProperty.Register(
            "CreateContent",
            typeof(CreateContentFunction),
            typeof(BackgroundVisualHost),
            new FrameworkPropertyMetadata(OnCreateContentChanged));

        /// <summary>
        /// Gets or sets the function used to create the visual to display in a background thread.
        /// </summary>
        public CreateContentFunction CreateContent
        {
            get { return (CreateContentFunction)GetValue(CreateContentProperty); }
            set { SetValue(CreateContentProperty, value); }
        }

        static void OnCreateContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bvh = (BackgroundVisualHost)d;

            if (!bvh.IsContentShowing) return;

            bvh.HideContentHelper();
            if (e.NewValue != null) bvh.CreateContentHelper();
        } 
        #endregion

        protected override int VisualChildrenCount => _hostVisual != null ? 1 : 0;

        protected override Visual GetVisualChild(int index)
        {
            if (_hostVisual != null && index == 0) return _hostVisual;

            throw new IndexOutOfRangeException("index");
        }

        protected override IEnumerator LogicalChildren
        {
            get 
            {
                if (_hostVisual != null)
                    yield return _hostVisual;
            }
        }

        private void CreateContentHelper()
        {
            _threadedHelper = new ThreadedVisualHelper(CreateContent, SafeInvalidateMeasure);
            _hostVisual = _threadedHelper.HostVisual;
        }

        private void SafeInvalidateMeasure()
        {
            Dispatcher.BeginInvoke(new Action(InvalidateMeasure), DispatcherPriority.Loaded);
        }

        private void HideContentHelper()
        {
            if (_threadedHelper == null) return;

            _threadedHelper.Exit();
            _threadedHelper = null;
            InvalidateMeasure();
        }

      

        protected override Size MeasureOverride(Size availableSize)
        {
            return _threadedHelper?.DesiredSize ?? base.MeasureOverride(availableSize);
        }

        private class ThreadedVisualHelper
        {
            private readonly HostVisual _hostVisual;
            private readonly AutoResetEvent _sync = new AutoResetEvent(false);
            private readonly CreateContentFunction _createContent;
            private readonly Action _invalidateMeasure;

            public HostVisual HostVisual => _hostVisual;
            public Size DesiredSize { get; private set; }
            private Dispatcher Dispatcher { get; set; }

            public ThreadedVisualHelper(
                CreateContentFunction createContent, 
                Action invalidateMeasure)
            {
                _hostVisual = new HostVisual();
                _createContent = createContent;
                _invalidateMeasure = invalidateMeasure;

                var backgroundUi = new Thread(CreateAndShowContent);
                backgroundUi.SetApartmentState(ApartmentState.STA);
                backgroundUi.Name = "BackgroundVisualHostThread";
                backgroundUi.IsBackground = true;
                backgroundUi.Start();

                _sync.WaitOne();
            }

            public void Exit()
            {
                Dispatcher.BeginInvokeShutdown(DispatcherPriority.Send);
            }

            private void CreateAndShowContent()
            {
                Dispatcher = Dispatcher.CurrentDispatcher;
                var source = 
                    new VisualTargetPresentationSource(_hostVisual);
                _sync.Set();
                source.RootVisual = _createContent();
                DesiredSize = source.DesiredSize;
                _invalidateMeasure();

                Dispatcher.Run();
                source.Dispose();
            }
        }
    }
}
