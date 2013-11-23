using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using GUIFramework.Managers;
using GUISkinFramework.Common;
using GUISkinFramework.Controls;
using GUISkinFramework.Skin;
using MessageFramework.DataObjects;
using MPDisplay.Common;
using MPDisplay.Common.Utils;
using MPDisplay.Common.ExtensionMethods;

namespace GUIFramework.GUI.Controls
{
    /// <summary>
    /// Interaction logic for GUIList.xaml
    /// </summary>
    [GUISkinElement(typeof(XmlList))]  
    public partial class GUIList : GUIControl
    {
        #region Fields

        private List<APIListItem> _listItems = new List<APIListItem>();
        private XmlListLayout _listLayoutType;
        private APIListItem _selectedItem;
        private XmlListItemStyle _currentListItemStyle = new XmlListItemStyle();
        private XmlListLayout _currentListLayout = XmlListLayout.Vertical;

        private ScrollViewer _scrollViewer;
        private Point _itemMouseDownPoint = new Point();
        private bool _itemMouseDown;
        private DispatcherTimer _selectionTimer;

        private ListBoxItem _selectedContainer;
        private Storyboard _selectedZoomAnimation;
        private Storyboard _selectedZoomBackAnimation;
        private DoubleAnimation _selectedZoomX;
        private DoubleAnimation _selectedZoomY; 

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ListBox3D"/> class.
        /// </summary>
        public GUIList() : base()
        {
            InitializeComponent();
            _scrollViewer = listbox.GetDescendantByType<ScrollViewer>();
            _scrollViewer.ManipulationBoundaryFeedback += (s, e) => e.Handled = true;
            MouseTouchDevice.RegisterEvents(listbox.GetDescendantByType<VirtualizingStackPanel>());
        }

        #endregion

        #region Control Base

        /// <summary>
        /// Gets the skin XML.
        /// </summary>
        public XmlList SkinXml
        {
            get { return BaseXml as XmlList; }
        }

        /// <summary>
        /// Gets the type of the list.
        /// </summary>
        /// <value>
        /// The type of the list.
        /// </value>
        public XmlListType ListType
        {
            get { return SkinXml != null ? SkinXml.ListType : XmlListType.None; }
        }

        /// <summary>
        /// Gets or sets the list items.
        /// </summary>
        /// <value>
        /// The list items.
        /// </value>
        public List<APIListItem> ListItems
        {
            get { return _listItems; }
            set { _listItems = value; NotifyPropertyChanged("ListItems"); }
        }

        /// <summary>
        /// Called when [window open].
        /// </summary>
        public override void OnWindowOpen()
        {
            base.OnWindowOpen();
            if (SkinXml.ListLayout != XmlListLayout.Auto)
            {
                ChangeLayout(SkinXml.ListLayout);
            }
        }

        /// <summary>
        /// Creates the control.
        /// </summary>
        public override void CreateControl()
        {
            base.CreateControl();
            CreateZoomAnimation();
            ChangeLayout(SkinXml.ListLayout == XmlListLayout.Auto ? XmlListLayout.Vertical : SkinXml.ListLayout);
            SetZoomAnimation(CurrentLayout.SelectionZoomX, CurrentLayout.SelectionZoomY);
        }

        /// <summary>
        /// Registers the info data.
        /// </summary>
        public override void OnRegisterInfoData()
        {
            base.OnRegisterInfoData();
            ListRepository.RegisterListMessage(this, SkinXml.ListType);
            GUIActionManager.RegisterAction(XmlActionType.ChangeListView, OnUserViewChange);
        }


        /// <summary>
        /// Deregisters the info data.
        /// </summary>
        public override void OnDeregisterInfoData()
        {
            base.OnDeregisterInfoData();
            ListRepository.DeregisterListMessage(this, SkinXml.ListType);
            GUIActionManager.DeregisterAction(this, XmlActionType.ChangeListView);
        }

        /// <summary>
        /// Updates the info data.
        /// </summary>
        public async override void UpdateInfoData()
        {
            base.UpdateInfoData();
            ListItems = await ListRepository.GetCurrentListItems(SkinXml.ListType);
            OnSelectedItemReceived();
        }

        /// <summary>
        /// Clears the info data.
        /// </summary>
        public override void ClearInfoData()
        {
            base.ClearInfoData();
        }

        /// <summary>
        /// Called when [list items received].
        /// </summary>
        public async void OnListItemsReceived()
        {
             await Dispatcher.InvokeAsync(OnPropertyChanging);
             OnSelectedItemReceived();
        }

        /// <summary>
        /// Called when [list layout received].
        /// </summary>
        public async void OnListLayoutReceived()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                if (SkinXml.ListLayout == XmlListLayout.Auto)
                {
                    ChangeLayout(ListRepository.GetCurrentMediaPortalListLayout(SkinXml.ListType));
                    GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.ControlVisibilityChanged);
                }
            });
        }

        /// <summary>
        /// Called when [selected item received].
        /// </summary>
        public async void OnSelectedItemReceived()
        {
            var selectedItem = await ListRepository.GetCurrentSelectedListItem(SkinXml.ListType);
            if (selectedItem != null && ListItems != null)
            {
                var item = ListItems.FirstOrDefault(x => x.Label == selectedItem.ItemText && x.Index == selectedItem.ItemIndex)
                    ?? ListItems.FirstOrDefault(x => x.Label == selectedItem.ItemText)
                ?? ListItems.FirstOrDefault(x => x.Index == selectedItem.ItemIndex);
                if (item != null)
                {
                    await Dispatcher.InvokeAsync(() =>
                    {
                        SelectItem(item);
                    });
                }
            }
        }

        /// <summary>
        /// Called when [user view change].
        /// </summary>
        /// <param name="action">The action.</param>
        private void OnUserViewChange(XmlAction action)
        {
            if (action != null)
            {
               Dispatcher.InvokeAsync(() =>
               {
                   XmlListLayout layout = XmlListLayout.Vertical;
                   if (Enum.TryParse<XmlListLayout>(action.Param1, out layout))
                   {
                       ChangeLayout(layout);
                   }
               });
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public APIListItem SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; NotifyPropertyChanged("SelectedItem"); }
        }

        /// <summary>
        /// Gets or sets the current layout.
        /// </summary>
        public XmlListItemStyle CurrentLayout
        {
            get { return _currentListItemStyle; }
            set { _currentListItemStyle = value; NotifyPropertyChanged("CurrentLayout"); }
        }

        /// <summary>
        /// Gets or sets the list orientation.
        /// </summary>
        public XmlListLayout ListLayoutType
        {
            get { return _listLayoutType; } 
            set { _listLayoutType = value; NotifyPropertyChanged("ListLayoutType"); }
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
            get { return _currentListLayout == XmlListLayout.Vertical; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Selects the item.
        /// </summary>
        /// <param name="item">The item.</param>
        private void SelectItem(APIListItem item)
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
                ListRepository.Instance.SelectListControlItem(this, listbox.SelectedItem as APIListItem);
            }
        }

        /// <summary>
        /// Scrolls the item to center.
        /// </summary>
        /// <param name="item">The item.</param>
        public void ScrollItemToCenter(APIListItem item)
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
                    if ((listbox.SelectedItem as APIListItem) != GetCenterItem())
                    {
                        listbox.ScrollIntoView(listbox.SelectedItem);
                        ScrollItemToCenter(listbox.SelectedItem as APIListItem);
                    }
                }, DispatcherPriority.Background);
            });
        }

        /// <summary>
        /// Changes the layout.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        private void ChangeLayout(XmlListLayout layout)
        {
            switch (layout)
            {
                case XmlListLayout.Vertical:
                    ListLayoutType = XmlListLayout.Vertical;
                    CurrentLayout = SkinXml.VerticalItemStyle;
                 
                    break;
                case XmlListLayout.Horizontal:
                    ListLayoutType = XmlListLayout.Horizontal;
                    CurrentLayout = SkinXml.HorizontalItemStyle;
                    break;
                case XmlListLayout.CoverFlow:
                    ListLayoutType = XmlListLayout.CoverFlow;
                    CurrentLayout = SkinXml.CoverFlowItemStyle;
                    break;
                default:
                    break;
            }
        
            if (_currentListLayout != layout)
            {
                _currentListLayout = layout;
                listbox.Items.Refresh();
                SetZoomAnimation(CurrentLayout.SelectionZoomX, CurrentLayout.SelectionZoomY);
            }
        }

        /// <summary>
        /// Creates the zoom animation.
        /// </summary>
        private void CreateZoomAnimation()
        {
            if (_selectedZoomBackAnimation == null)
            {
                Duration duration = new Duration(TimeSpan.FromMilliseconds(CurrentLayout.SelectionZoomDuration));
                _selectedZoomX = new DoubleAnimation();
                _selectedZoomY = new DoubleAnimation();
                DoubleAnimation outX = new DoubleAnimation();
                DoubleAnimation outY = new DoubleAnimation();
                _selectedZoomX.Duration = duration;
                _selectedZoomY.Duration = duration;
                outX.Duration = duration;
                outY.Duration = duration;
                Storyboard sbIn = new Storyboard();
                sbIn.Duration = duration;
                sbIn.Children.Add(_selectedZoomX);
                sbIn.Children.Add(_selectedZoomY);
                Storyboard sbOut = new Storyboard();
                sbOut.Duration = duration;
                sbOut.Children.Add(outX);
                sbOut.Children.Add(outY);
                Storyboard.SetTargetProperty(_selectedZoomX, new PropertyPath("LayoutTransform.ScaleX"));
                Storyboard.SetTargetProperty(_selectedZoomY, new PropertyPath("LayoutTransform.ScaleY"));
                Storyboard.SetTargetProperty(outX, new PropertyPath("LayoutTransform.ScaleX"));
                Storyboard.SetTargetProperty(outY, new PropertyPath("LayoutTransform.ScaleY"));
                outX.To = 1;
                outY.To = 1;
                _selectedZoomAnimation = sbIn;
                _selectedZoomBackAnimation = sbOut;
            }
        }

        /// <summary>
        /// Sets the zoom animation.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        private void SetZoomAnimation(int x, int y)
        {
            if (_selectedZoomX != null && _selectedZoomY != null)
            {
                _selectedZoomX.To = Math.Max(1.0, ((double)x / 100.0));
                _selectedZoomY.To = Math.Max(1.0, ((double)y / 100.0));
            }
        }

        /// <summary>
        /// Starts the selection timer.
        /// </summary>
        private void StartSelectionTimer()
        {
            StopSelectionTimer();
            _selectionTimer = new DispatcherTimer();
            _selectionTimer.Interval = TimeSpan.FromSeconds(2);
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
        private APIListItem GetCenterItem()
        {
            var element = listbox.InputHitTest(new Point((listbox.ActualWidth / 2.0), (listbox.ActualHeight / 2.0))) as Border;
            if (element != null)
            {
                return (element.Child as ContentPresenter).Content as APIListItem;
            }
            return null;
        }

        #endregion

        #region Item Events

        /// <summary>
        /// Handles the MouseButtonUp event of the Item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnListItem_MouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            StopSelectionTimer();
            _itemMouseDown = false;
            double newPos = IsLayoutVertical
                ? (_itemMouseDownPoint.Y - e.GetPosition(this).Y)
                : (_itemMouseDownPoint.X - e.GetPosition(this).X);

            if (Math.Abs(newPos) < DragThreshold)
            {
                ScrollItemToCenter(listbox.SelectedItem as APIListItem);
                ListRepository.Instance.FocusListControlItem(this, listbox.SelectedItem as APIListItem);
            }
        }

        /// <summary>
        /// Handles the MouseButtonDown event of the Item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnListItem_MouseButtonDown(object sender, MouseButtonEventArgs e)
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
        private void OnListItem_MouseLeave(object sender, MouseEventArgs e)
        {
            StopSelectionTimer();
            _itemMouseDown = false;
        }

        /// <summary>
        /// Called when [list item_ selected].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnListItem_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedContainer != null)
                {
                    _selectedZoomBackAnimation.Begin(_selectedContainer);
                }

                var currentContainer = sender as ListBoxItem;
                if (currentContainer != null)
                {
                    currentContainer.LayoutTransform = new ScaleTransform(1, 1);
                    currentContainer.RenderTransformOrigin = new Point(0.5, 0.5);
                    _selectedZoomAnimation.Begin(currentContainer);
                    _selectedContainer = currentContainer;
                }
            }
            catch { }
        }

        #endregion
    }
}
