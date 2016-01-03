using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MPDisplay.Common.Controls
{
    public class SizeAdorner : Adorner
    {
        private readonly Control _chrome;
        private readonly VisualCollection _visuals;

        protected override int VisualChildrenCount => _visuals.Count;

        public SizeAdorner(UIElement designerItem)
            : base(designerItem)
        {
            SnapsToDevicePixels = true;
            _chrome = new Control {DataContext = designerItem};
            _visuals = new VisualCollection(this) {_chrome};
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            _chrome.Arrange(new Rect(new Point(0.0, 0.0), arrangeBounds));
            return arrangeBounds;
        }
    }
}
