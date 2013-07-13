﻿using System;
using System.Windows;
using System.Windows.Media;

namespace MPDisplay.Common.Controls
{
    public class VisualTargetPresentationSource : PresentationSource
    {
        private VisualTarget _visualTarget;
        private bool _isDisposed = false;

        public VisualTargetPresentationSource(HostVisual hostVisual)
        {
            _visualTarget = new VisualTarget(hostVisual);
            AddSource();
        }

        public Size DesiredSize { get; private set; }

        public override Visual RootVisual
        {
            get { return _visualTarget.RootVisual; }
            set
            {
                Visual oldRoot = _visualTarget.RootVisual;

                // Set the root visual of the VisualTarget.  This visual will
                // now be used to visually compose the scene.
                _visualTarget.RootVisual = value;

                // Tell the PresentationSource that the root visual has
                // changed.  This kicks off a bunch of stuff like the
                // Loaded event.
                RootChanged(oldRoot, value);

                // Kickoff layout...
                UIElement rootElement = value as UIElement;
                if (rootElement != null)
                {
                    rootElement.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    rootElement.Arrange(new Rect(rootElement.DesiredSize));

                    DesiredSize = rootElement.DesiredSize;
                }
                else
                    DesiredSize = new Size(0, 0);
            }
        }

        protected override CompositionTarget GetCompositionTargetCore()
        {
            return _visualTarget;
        }

        public override bool IsDisposed
        {
            get { return _isDisposed; }
        }

        internal void Dispose()
        {
            RemoveSource();
            _isDisposed = true;
        }
    }
}
