using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using MPDisplay.Common.Utils;
using MPDisplay.Common.ExtensionMethods;
using System.Windows.Media.Animation;
using System.Threading;
using GUISkinFramework.Controls;

namespace SkinEditor.Controls
{
    /// <summary>
    /// Interaction logic for CoverFlowListBox.xaml
    /// </summary>
    public partial class CoverFlowListBox : UserControl, INotifyPropertyChanged
    {
        #region Fields

        private XmlListItemStyle _currentLayout = new XmlListItemStyle();
        private ScrollViewer _scrollViewer;
        private Point _itemMouseDownPoint = new Point();
        private XmlListLayout _currentOrientation = XmlListLayout.Vertical;
        private bool _itemMouseDown;
        private DispatcherTimer _selectionTimer;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ListBox3D"/> class.
        /// </summary>
        public CoverFlowListBox()
        {
          
            InitializeComponent();
            _scrollViewer = listbox.GetDescendantByType<ScrollViewer>();
            MouseTouchDevice.RegisterEvents(listbox.GetDescendantByType<VirtualizingStackPanel>());
        }

        #endregion

        #region DependencyProperties

        public static readonly DependencyProperty ListItemsProperty = DependencyProperty.Register("ListItems", typeof(ObservableCollection<CoverFlowListBoxItem>), typeof(CoverFlowListBox), new PropertyMetadata(new ObservableCollection<CoverFlowListBoxItem>()));
        public static readonly DependencyProperty LayoutVerticalProperty = DependencyProperty.Register("LayoutVertical", typeof(XmlListItemStyle), typeof(CoverFlowListBox), new PropertyMetadata(new XmlListItemStyle(), (s, e) => (s as CoverFlowListBox).UpdateItemLayout()));
        public static readonly DependencyProperty LayoutHorizontalProperty = DependencyProperty.Register("LayoutHorizontal", typeof(XmlListItemStyle), typeof(CoverFlowListBox), new PropertyMetadata(new XmlListItemStyle(), (s, e) => (s as CoverFlowListBox).UpdateItemLayout()));
        public static readonly DependencyProperty LayoutCoverFlowProperty = DependencyProperty.Register("LayoutCoverFlow", typeof(XmlListItemStyle), typeof(CoverFlowListBox), new PropertyMetadata(new XmlListItemStyle(), (s, e) => (s as CoverFlowListBox).UpdateItemLayout()));
        public static readonly DependencyProperty UserSelectedItemProperty = DependencyProperty.Register("UserSelectedItem", typeof(CoverFlowListBoxItem), typeof(CoverFlowListBox), new PropertyMetadata(null));
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(CoverFlowListBoxItem), typeof(CoverFlowListBox), new PropertyMetadata(new CoverFlowListBoxItem(), new PropertyChangedCallback(OnSelectedItemChanged)));
        public static readonly DependencyProperty ListOrientationProperty = DependencyProperty.Register("ListOrientation", typeof(XmlListLayout), typeof(CoverFlowListBox), new PropertyMetadata(XmlListLayout.Vertical, new PropertyChangedCallback(OnListOrientationChanged)));
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
            (d as CoverFlowListBox).ChangeLayout((XmlListLayout)e.NewValue);
        }

        /// <summary>
        /// Called when SelectedItem changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CoverFlowListBox).SelectItem(e.NewValue as CoverFlowListBoxItem);
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
            listbox.ScrollIntoView(item);
            ScrollItemToCenter(item);
            listbox.SelectedItem = item;
        }

        /// <summary>
        /// Selects the user item.
        /// </summary>
        public void SelectUserItem()
        {
            StopSelectionTimer();

            double newPos = IsLayoutVertical
                ? (_itemMouseDownPoint.Y - Mouse.GetPosition(this).Y)
                : (_itemMouseDownPoint.X - Mouse.GetPosition(this).X);

            if (_itemMouseDown && Math.Abs(newPos) < DragThreshold)
            {
                UserSelectedItem = listbox.SelectedItem as CoverFlowListBoxItem;
            }
        }

        /// <summary>
        /// Scrolls the item to center.
        /// </summary>
        /// <param name="item">The item.</param>
        public void ScrollItemToCenter(CoverFlowListBoxItem item)
        {
            if (item != null)
            {
                Point? positionFromCenter = listbox.GetItemOffsetFromCenterOfView(item);
                if (positionFromCenter != null)
                {
                    Point newpos = positionFromCenter.Value;
                    if (IsLayoutVertical)
                    {
                        if (newpos.Y != 0 && newpos.Y != _scrollViewer.VerticalOffset)
                        {
                            int duration = newpos.Y < _scrollViewer.VerticalOffset
                               ? (int)Math.Min(1000, Math.Max(0, _scrollViewer.VerticalOffset - newpos.Y))
                               : (int)Math.Min(1000, Math.Max(0, newpos.Y - _scrollViewer.VerticalOffset));

                            if (duration != 0)
                            {
                                var animation = new DoubleAnimation(_scrollViewer.VerticalOffset, newpos.Y, new Duration(TimeSpan.FromMilliseconds(duration)));
                                animation.Completed += (s, e) => AdjustItemCenter();
                                _scrollViewer.BeginAnimation(ItemsControlExtensions.AnimatableVerticalOffsetProperty, animation, HandoffBehavior.SnapshotAndReplace);
                            }
                        }
                        return;
                    }
                    else
                    {
                        if (newpos.X != 0 && newpos.X != _scrollViewer.HorizontalOffset)
                        {
                            int duration = newpos.X < _scrollViewer.HorizontalOffset
                                ? (int)Math.Min(500, Math.Max(0, (_scrollViewer.HorizontalOffset - newpos.X) / 2))
                                : (int)Math.Min(500, Math.Max(0, (newpos.X - _scrollViewer.HorizontalOffset) / 2));

                            if (duration != 0)
                            {
                                var animation = new DoubleAnimation(_scrollViewer.HorizontalOffset, newpos.X, new Duration(TimeSpan.FromMilliseconds(duration)));
                                animation.Completed += (s, e) => AdjustItemCenter();
                                _scrollViewer.BeginAnimation(ItemsControlExtensions.AnimatableHorizontalOffsetProperty, animation, HandoffBehavior.SnapshotAndReplace);
                            }
                        }
                        return;
                    }
                   
                }
                _scrollViewer.BeginAnimation(ItemsControlExtensions.AnimatableHorizontalOffsetProperty,null);
            }
        }

        /// <summary>
        /// Adjusts the item center.
        /// </summary>
        /// <param name="item">The item.</param>
        private void AdjustItemCenter()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Thread.Sleep(150);
                Dispatcher.Invoke(() =>
                {
                    if ((listbox.SelectedItem as CoverFlowListBoxItem) != GetCenterItem())
                    {
                        listbox.ScrollIntoView(listbox.SelectedItem);
                        ScrollItemToCenter(listbox.SelectedItem as CoverFlowListBoxItem);
                    }
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
                case XmlListLayout.Horizontal:
                    CurrentLayout = LayoutHorizontal;
                    break;
                case XmlListLayout.CoverFlow:
                    CurrentLayout = LayoutCoverFlow;
                    break;
                default:
                    break;
            }

            if (_currentOrientation != orientation)
            {
                _currentOrientation = orientation;
                listbox.Items.Refresh();
            }
        }

        private void RefreshList()
        {
            var items = ListItems;
            ListItems.Clear();
            foreach (var item in items)
            {
                ListItems.Add(item);
            }
        }

        /// <summary>
        /// Starts the selection timer.
        /// </summary>
        private void StartSelectionTimer()
        {
            StopSelectionTimer();
            _selectionTimer = new DispatcherTimer();
            _selectionTimer.Interval = TimeSpan.FromSeconds(4);
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
            if (_selectionTimer != null)
            {
                _selectionTimer.Stop();
                _selectionTimer = null;
            }
        }

        /// <summary>
        /// Gets the center item.
        /// </summary>
        /// <returns></returns>
        private CoverFlowListBoxItem GetCenterItem()
        {
            var element = listbox.InputHitTest(new Point((listbox.ActualWidth / 2.0), (listbox.ActualHeight / 2.0))) as Border;
            if (element != null)
            {
                return (element.Child as ContentPresenter).Content as CoverFlowListBoxItem;
            }
            return null;
        }

        /// <summary>
        /// Gets the drag threshold.
        /// </summary>
        private double DragThreshold
        {
            get { return IsLayoutVertical ? CurrentLayout.Height : CurrentLayout.Width; }
        }

        /// <summary>
        /// Gets a value indicating whether the layout is vertical.
        /// </summary>
        /// <value>
        /// <c>true</c> if the layout is vertical; otherwise, <c>false</c>.
        /// </value>
        private bool IsLayoutVertical
        {
            get { return _currentOrientation == XmlListLayout.Vertical; }
        }

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
            double newPos = IsLayoutVertical
                ? (_itemMouseDownPoint.Y - e.GetPosition(this).Y)
                : (_itemMouseDownPoint.X - e.GetPosition(this).X);

            if (Math.Abs(newPos) < DragThreshold)
            {
                ScrollItemToCenter(listbox.SelectedItem as CoverFlowListBoxItem);
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
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }

    public class CoverFlowGreaterThanSelectedIndexConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var selectedIndex = int.Parse(values[0].ToString());
                var itemIndex = int.Parse(values[1].ToString());
                return itemIndex > selectedIndex;
            }
            catch { }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
