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
using GUIFramework.Repositories;
using GUISkinFramework.Skin;
using MessageFramework.DataObjects;
using MPDisplay.Common.ExtensionMethods;

namespace GUIFramework.GUI
{
    /// <summary>
    /// Interaction logic for GUIList.xaml
    /// </summary>
    [GUISkinElement(typeof(XmlList))]
    public partial class GUIList
    {
        #region Fields

        private List<APIListItem> _listItems = new List<APIListItem>();
        private XmlListLayout _listLayoutType;
        private APIListItem _selectedItem;
        private XmlListItemStyle _currentListItemStyle = new XmlListItemStyle();
        private XmlListLayout _currentListLayout = XmlListLayout.Vertical;

        private ListBoxItem _selectedContainer;
        private Storyboard _selectedZoomAnimation;
        private Storyboard _selectedZoomBackAnimation;
        private DoubleAnimation _selectedZoomX;
        private DoubleAnimation _selectedZoomY;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ListBox3D class.
        /// </summary>
        public GUIList()
        {
            InitializeComponent();
            AddHandler(MouseDownEvent, new MouseButtonEventHandler(GUIList_MouseButtonDown), true);
             ScrollViewer = listbox.GetDescendantByType<ScrollViewer>();
            ScrollViewer.ManipulationBoundaryFeedback += (s, e) => e.Handled = true;
            // Check for page updates if ScrollChanged event fires

            // Setup listViewScrollViewer
            ScrollViewer.ScrollChanged += GUIList_ScrollChanged;
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
                   if (Enum.TryParse(action.Param1, out layout))
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
            get { return _currentListLayout == XmlListLayout.Vertical || _currentListLayout == XmlListLayout.VerticalIcon; }
        }

        #endregion

        #region Methods

        protected override void SelectListItem()
        {
            ScrollItemToCenter(listbox.SelectedItem as APIListItem);
            ListRepository.Instance.SelectListControlItem(this, listbox.SelectedItem as APIListItem);
        }

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
                        if (Math.Abs(newpos.Y) > 0.0000001 && Math.Abs(newpos.Y - ScrollViewer.VerticalOffset) > 0.0000001)
                        {
                            int duration = newpos.Y < ScrollViewer.VerticalOffset
                               ? (int)Math.Min(1000, Math.Max(0, ScrollViewer.VerticalOffset - newpos.Y))
                               : (int)Math.Min(1000, Math.Max(0, newpos.Y - ScrollViewer.VerticalOffset));

                            if (duration != 0)
                            {
                                var animation = new DoubleAnimation(ScrollViewer.VerticalOffset, newpos.Y, new Duration(TimeSpan.FromMilliseconds(duration)));
                                animation.Completed += (s, e) => AdjustItemCenter();
                                ScrollViewer.BeginAnimation(ItemsControlExtensions.AnimatableVerticalOffsetProperty, animation, HandoffBehavior.SnapshotAndReplace);
                            }
                        }
                        return;
                    }
                    else
                    {
                        if (Math.Abs(newpos.X) > 0.0000001 && Math.Abs(newpos.X - ScrollViewer.HorizontalOffset) > 0.0000001)
                        {
                            int duration = newpos.X < ScrollViewer.HorizontalOffset
                                ? (int)Math.Min(500, Math.Max(0, (ScrollViewer.HorizontalOffset - newpos.X) / 2))
                                : (int)Math.Min(500, Math.Max(0, (newpos.X - ScrollViewer.HorizontalOffset) / 2));

                            if (duration != 0)
                            {
                                var animation = new DoubleAnimation(ScrollViewer.HorizontalOffset, newpos.X, new Duration(TimeSpan.FromMilliseconds(duration)));
                                animation.Completed += (s, e) => AdjustItemCenter();
                                ScrollViewer.BeginAnimation(ItemsControlExtensions.AnimatableHorizontalOffsetProperty, animation, HandoffBehavior.SnapshotAndReplace);
                            }
                        }
                        return;
                    }
                   
                }
                ScrollViewer.BeginAnimation(ItemsControlExtensions.AnimatableHorizontalOffsetProperty,null);
            }
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
        /// <param name="layout">The layout.</param>
        private void ChangeLayout(XmlListLayout layout)
        {
            switch (layout)
            {
                case XmlListLayout.Vertical:
                    ListLayoutType = XmlListLayout.Vertical;
                    CurrentLayout = SkinXml.VerticalItemStyle;
                    break;
                case XmlListLayout.VerticalIcon: 
                    ListLayoutType = XmlListLayout.VerticalIcon;
                    CurrentLayout = SkinXml.VerticalIconItemStyle;
                    break;
                case XmlListLayout.Horizontal:
                    ListLayoutType = XmlListLayout.Horizontal;
                    CurrentLayout = SkinXml.HorizontalItemStyle;
                    break;
                case XmlListLayout.CoverFlow:
                    ListLayoutType = XmlListLayout.CoverFlow;
                    CurrentLayout = SkinXml.CoverFlowItemStyle;
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
                _selectedZoomX.To = Math.Max(1.0, (x / 100.0));
                _selectedZoomY.To = Math.Max(1.0, (y / 100.0));
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
                var contentPresenter = element.Child as ContentPresenter;
                if (contentPresenter != null)
                    return contentPresenter.Content as APIListItem;
            }
            return null;
        }

        #endregion

        #region Item Events

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
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Event: Mouse Single Click on list item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnListItem_PreviewMouseDown(object sender, RoutedEventArgs e)
        {
            if (IsMouseButtonDown && IsMouseDoubleClick == false)
            {
                // This may be one of the following cases:
                // 1) mouse single click
                // 2) mouse drag from one item to next
                MayBeOutOfSync = true;
            }
            
        }

        /// <summary>
        /// Event: Mouse released on an item: Set Focus on item if list is not scrolling
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnListItem_PreviewMouseUp(object sender, RoutedEventArgs e)
        {
            if (IsScrolling == false && IsMouseDoubleClick == false)
            {
                var listBoxItem = sender as ListBoxItem;
                if (listBoxItem != null)
                {
                    var item = listBoxItem.Content as APIListItem;
                    if (item != null)
                    {
                        ListRepository.Instance.FocusListControlItem(this, item);
                    }
                }
            }
        }

        private void OnListItem_PreviewMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ScrollItemToCenter(listbox.SelectedItem as APIListItem);
            ListRepository.Instance.SelectListControlItem(this, listbox.SelectedItem as APIListItem);
        }

        #endregion

 

    }
}
