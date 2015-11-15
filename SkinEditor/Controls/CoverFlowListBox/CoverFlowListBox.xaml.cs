using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using GUISkinFramework.Skin;
using MPDisplay.Common;
using MPDisplay.Common.ExtensionMethods;

namespace SkinEditor.Controls
{
    /// <summary>
    /// Interaction logic for CoverFlowListBox.xaml
    /// </summary>
    public partial class CoverFlowListBox : INotifyPropertyChanged
    {
        #region Fields

        private XmlListItemStyle _currentLayout = new XmlListItemStyle();
        private readonly ScrollViewer _scrollViewer;
        private Point _itemMouseDownPoint;
        private XmlListLayout _currentOrientation = XmlListLayout.Vertical;
        private bool _itemMouseDown;
        private DispatcherTimer _selectionTimer;

        private const double Tolerance = 0.0000001;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ListBox3D class.
        /// </summary>
        public CoverFlowListBox()
        {
          
            InitializeComponent();
            _scrollViewer = Listbox.GetDescendantByType<ScrollViewer>();
            MouseTouchDevice.RegisterEvents(Listbox.GetDescendantByType<VirtualizingStackPanel>());
        }

        #endregion

        #region DependencyProperties

        public static readonly DependencyProperty ListItemsProperty = DependencyProperty.Register("ListItems", typeof(ObservableCollection<CoverFlowListBoxItem>), typeof(CoverFlowListBox), new PropertyMetadata(new ObservableCollection<CoverFlowListBoxItem>()));
        public static readonly DependencyProperty LayoutVerticalProperty = DependencyProperty.Register("LayoutVertical", typeof(XmlListItemStyle), typeof(CoverFlowListBox), new PropertyMetadata(new XmlListItemStyle(), (s, e) => ((CoverFlowListBox) s).UpdateItemLayout()));
        public static readonly DependencyProperty LayoutVerticalIconProperty = DependencyProperty.Register("LayoutVerticalIcon", typeof(XmlListItemStyle), typeof(CoverFlowListBox), new PropertyMetadata(new XmlListItemStyle(), (s, e) => ((CoverFlowListBox) s).UpdateItemLayout()));
        public static readonly DependencyProperty LayoutHorizontalProperty = DependencyProperty.Register("LayoutHorizontal", typeof(XmlListItemStyle), typeof(CoverFlowListBox), new PropertyMetadata(new XmlListItemStyle(), (s, e) => ((CoverFlowListBox) s).UpdateItemLayout()));
        public static readonly DependencyProperty LayoutCoverFlowProperty = DependencyProperty.Register("LayoutCoverFlow", typeof(XmlListItemStyle), typeof(CoverFlowListBox), new PropertyMetadata(new XmlListItemStyle(), (s, e) => ((CoverFlowListBox) s).UpdateItemLayout()));
        public static readonly DependencyProperty UserSelectedItemProperty = DependencyProperty.Register("UserSelectedItem", typeof(CoverFlowListBoxItem), typeof(CoverFlowListBox), new PropertyMetadata(null));
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(CoverFlowListBoxItem), typeof(CoverFlowListBox), new PropertyMetadata(new CoverFlowListBoxItem(), OnSelectedItemChanged));
        public static readonly DependencyProperty ListOrientationProperty = DependencyProperty.Register("ListOrientation", typeof(XmlListLayout), typeof(CoverFlowListBox), new PropertyMetadata(XmlListLayout.Vertical, OnListOrientationChanged));
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the list items.
        /// </summary>
        public ObservableCollection<CoverFlowListBoxItem> ListItems
        {
            get { return (ObservableCollection<CoverFlowListBoxItem>)GetValue(ListItemsProperty); }
            set { SetValue(ListItemsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the layout vertical.
        /// </summary>
        public XmlListItemStyle LayoutVertical
        {
            get { return (XmlListItemStyle)GetValue(LayoutVerticalProperty); }
            set { SetValue(LayoutVerticalProperty, value); }
        }

        /// <summary>
        /// Gets or sets the layout verticalIcon.
        /// </summary>
        public XmlListItemStyle LayoutVerticalIcon
        {
            get { return (XmlListItemStyle)GetValue(LayoutVerticalIconProperty); }
            set { SetValue(LayoutVerticalIconProperty, value); }
        }

        /// <summary>
        /// Gets or sets the layout horizontal.
        /// </summary>
        public XmlListItemStyle LayoutHorizontal
        {
            get { return (XmlListItemStyle)GetValue(LayoutHorizontalProperty); }
            set { SetValue(LayoutHorizontalProperty, value); }
        }

        /// <summary>
        /// Gets or sets the layout cover flow.
        /// </summary>
        public XmlListItemStyle LayoutCoverFlow
        {
            get { return (XmlListItemStyle)GetValue(LayoutCoverFlowProperty); }
            set { SetValue(LayoutCoverFlowProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Item selected by the user.
        /// </summary>
        public CoverFlowListBoxItem UserSelectedItem
        {
            get { return (CoverFlowListBoxItem)GetValue(UserSelectedItemProperty); }
            set { SetValue(UserSelectedItemProperty, value); }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public CoverFlowListBoxItem SelectedItem
        {
            get { return (CoverFlowListBoxItem)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Gets or sets the current layout.
        /// </summary>
        public XmlListItemStyle CurrentLayout
        {
            get { return _currentLayout; }
            set { _currentLayout = value; NotifyPropertyChanged("CurrentLayout"); }
        }

        /// <summary>
        /// Gets or sets the list orientation.
        /// </summary>
        public XmlListLayout ListOrientation
        {
            get { return (XmlListLayout)GetValue(ListOrientationProperty); }
            set { SetValue(ListOrientationProperty, value); }
        }

        #endregion

        #region PropertyChanged Events

        /// <summary>
        /// Called when ListOrientation changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnListOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var coverFlowListBox = d as CoverFlowListBox;
            coverFlowListBox?.ChangeLayout((XmlListLayout)e.NewValue);
        }

        /// <summary>
        /// Called when SelectedItem changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var coverFlowListBox = d as CoverFlowListBox;
            coverFlowListBox?.SelectItem(e.NewValue as CoverFlowListBoxItem);
        }

        #endregion

        #region Methods

        private void UpdateItemLayout()
        {
            ChangeLayout(ListOrientation);
        }

        /// <summary>
        /// Selects the item.
        /// </summary>
        /// <param name="item">The item.</param>
        private void SelectItem(CoverFlowListBoxItem item)
        {
            Listbox.ScrollIntoView(item);
            ScrollItemToCenter(item);
            Listbox.SelectedItem = item;
        }

        /// <summary>
        /// Selects the user item.
        /// </summary>
        public void SelectUserItem()
        {
            StopSelectionTimer();

            var newPos = IsLayoutVertical
                ? (_itemMouseDownPoint.Y - Mouse.GetPosition(this).Y)
                : (_itemMouseDownPoint.X - Mouse.GetPosition(this).X);

            if (_itemMouseDown && Math.Abs(newPos) < DragThreshold)
            {
                UserSelectedItem = Listbox.SelectedItem as CoverFlowListBoxItem;
            }
        }

        /// <summary>
        /// Scrolls the item to center.
        /// </summary>
        /// <param name="item">The item.</param>
        public void ScrollItemToCenter(CoverFlowListBoxItem item)
        {
            if (item == null) return;

            var positionFromCenter = Listbox.GetItemOffsetFromCenterOfView(item);

            if (positionFromCenter != null)
            {
                var newpos = positionFromCenter.Value;
                int duration;
                DoubleAnimation animation;

                if (IsLayoutVertical)
                {
                    if (!(Math.Abs(newpos.Y) > Tolerance) ||
                        !(Math.Abs(newpos.Y - _scrollViewer.VerticalOffset) > Tolerance)) return;

                    duration = newpos.Y < _scrollViewer.VerticalOffset
                        ? (int)Math.Min(1000, Math.Max(0, _scrollViewer.VerticalOffset - newpos.Y))
                        : (int)Math.Min(1000, Math.Max(0, newpos.Y - _scrollViewer.VerticalOffset));

                    if (duration == 0) return;
                    animation = new DoubleAnimation(_scrollViewer.VerticalOffset, newpos.Y, new Duration(TimeSpan.FromMilliseconds(duration)));
                    animation.Completed += (s, e) => AdjustItemCenter();
                    _scrollViewer.BeginAnimation(ItemsControlExtensions.AnimatableVerticalOffsetProperty, animation, HandoffBehavior.SnapshotAndReplace);
                    return;
                }
                if (!(Math.Abs(newpos.X) > Tolerance) || !(Math.Abs(newpos.X - _scrollViewer.HorizontalOffset) > Tolerance)) return;

                duration = newpos.X < _scrollViewer.HorizontalOffset
                    ? (int)Math.Min(500, Math.Max(0, (_scrollViewer.HorizontalOffset - newpos.X) / 2))
                    : (int)Math.Min(500, Math.Max(0, (newpos.X - _scrollViewer.HorizontalOffset) / 2));

                if (duration == 0) return;
                animation = new DoubleAnimation(_scrollViewer.HorizontalOffset, newpos.X, new Duration(TimeSpan.FromMilliseconds(duration)));
                animation.Completed += (s, e) => AdjustItemCenter();
                _scrollViewer.BeginAnimation(ItemsControlExtensions.AnimatableHorizontalOffsetProperty, animation, HandoffBehavior.SnapshotAndReplace);
                return;
            }
            _scrollViewer.BeginAnimation(ItemsControlExtensions.AnimatableHorizontalOffsetProperty,null);
        }

        /// <summary>
        /// Adjusts the item center.
        /// </summary>
        private void AdjustItemCenter()
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                Thread.Sleep(150);
                Dispatcher.Invoke(() =>
                {
                    if ((Listbox.SelectedItem as CoverFlowListBoxItem) == GetCenterItem()) return;

                    Listbox.ScrollIntoView(Listbox.SelectedItem);
                    ScrollItemToCenter(Listbox.SelectedItem as CoverFlowListBoxItem);
                }, DispatcherPriority.Background);
            });
        }

        /// <summary>
        /// Changes the layout.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        private void ChangeLayout(XmlListLayout orientation)
        {
            switch (orientation)
            {
                case XmlListLayout.Vertical:
                    CurrentLayout = LayoutVertical;
                    break;
                case XmlListLayout.VerticalIcon:
                    CurrentLayout = LayoutVerticalIcon;
                    break;
                case XmlListLayout.Horizontal:
                    CurrentLayout = LayoutHorizontal;
                    break;
                case XmlListLayout.CoverFlow:
                    CurrentLayout = LayoutCoverFlow;
                    break;
            }

            if (_currentOrientation == orientation) return;

            _currentOrientation = orientation;
            Listbox.Items.Refresh();
        }

        /// <summary>
        /// Starts the selection timer.
        /// </summary>
        private void StartSelectionTimer()
        {
            StopSelectionTimer();
            _selectionTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(4)};
            _selectionTimer.Tick += (s, e) =>
            {
                StopSelectionTimer();
                SelectUserItem();
            };
            _selectionTimer.Start();
        }

        /// <summary>
        /// Stops the selection timer.
        /// </summary>
        private void StopSelectionTimer()
        {
            if (_selectionTimer == null) return;

            _selectionTimer.Stop();
            _selectionTimer = null;
        }

        /// <summary>
        /// Gets the center item.
        /// </summary>
        /// <returns></returns>
        private CoverFlowListBoxItem GetCenterItem()
        {
            var element = Listbox.InputHitTest(new Point((Listbox.ActualWidth / 2.0), (Listbox.ActualHeight / 2.0))) as Border;

            var contentPresenter = element?.Child as ContentPresenter;
            return contentPresenter?.Content as CoverFlowListBoxItem;
        }

        /// <summary>
        /// Gets the drag threshold.
        /// </summary>
        private double DragThreshold => IsLayoutVertical ? CurrentLayout.Height : CurrentLayout.Width;

        /// <summary>
        /// Gets a value indicating whether the layout is vertical.
        /// </summary>
        /// <value>
        /// <c>true</c> if the layout is vertical; otherwise, <c>false</c>.
        /// </value>
        private bool IsLayoutVertical => _currentOrientation == XmlListLayout.Vertical || _currentOrientation == XmlListLayout.VerticalIcon;

        #endregion

        #region Item Events

        /// <summary>
        /// Handles the MouseButtonUp event of the Item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void ListItem_MouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            StopSelectionTimer();
            _itemMouseDown = false;
            var newPos = IsLayoutVertical
                ? (_itemMouseDownPoint.Y - e.GetPosition(this).Y)
                : (_itemMouseDownPoint.X - e.GetPosition(this).X);

            if (Math.Abs(newPos) < DragThreshold)
            {
                ScrollItemToCenter(Listbox.SelectedItem as CoverFlowListBoxItem);
            }
        }

        /// <summary>
        /// Handles the MouseButtonDown event of the Item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void ListItem_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartSelectionTimer();
            _itemMouseDown = true;
            _itemMouseDownPoint = e.GetPosition(this);
        }


        /// <summary>
        /// Handles the MouseLeave event of the Item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void ListItem_MouseLeave(object sender, MouseEventArgs e)
        {
            StopSelectionTimer();
            _itemMouseDown = false;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion

    }

    public class CoverFlowGreaterThanSelectedIndexConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var selectedIndex = int.Parse(values[0].ToString());
                var itemIndex = int.Parse(values[1].ToString());
                return itemIndex > selectedIndex;
            }
            catch
            {
                // ignored
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
