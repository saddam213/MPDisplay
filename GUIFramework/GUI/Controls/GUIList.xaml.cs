using System;
using System.Collections.Generic;
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
using GUISkinFramework.Animations;
using GUISkinFramework.Controls;
using GUIFramework.Managers;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Threading;
using MPDisplay.Common.ExtensionMethods;
using System.Windows.Media.Animation;
using MPDisplay.Common.Utils;
using MessageFramework.DataObjects;
using GUISkinFramework.Skin;
using GUISkinFramework.Common;

namespace GUIFramework.GUI.Controls
{
    /// <summary>
    /// Interaction logic for GUIButton.xaml
    /// </summary>
    [XmlSkinType(typeof(XmlList))]  
    public partial class GUIList : GUIControl
    {
        private List<APIListItem> _listItems = new List<APIListItem>();
        private XmlListLayout _listLayoutType;
        private APIListItem _selectedItem;
        private XmlListItemStyle _currentListItemStyle = new XmlListItemStyle();
        private XmlListLayout _currentListLayout = XmlListLayout.Vertical;

        private ScrollViewer _scrollViewer;
        private Point _itemMouseDownPoint = new Point();
        private bool _itemMouseDown;
        private DispatcherTimer _selectionTimer;

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

        public XmlList SkinXml
        {
            get { return BaseXml as XmlList; }
        }

        public XmlListType ListType
        {
            get { return SkinXml != null ? SkinXml.ListType : XmlListType.None; }
        }

        public List<APIListItem> ListItems
        {
            get { return _listItems; }
            set { _listItems = value; NotifyPropertyChanged("ListItems"); }
        }

        public override void OnWindowOpen()
        {
            base.OnWindowOpen();
            if (SkinXml.ListLayout != XmlListLayout.Auto)
            {
                ChangeLayout(SkinXml.ListLayout);
            }
        }

        public override void CreateControl()
        {
            base.CreateControl();
            ChangeLayout(SkinXml.ListLayout == XmlListLayout.Auto ? XmlListLayout.Vertical : SkinXml.ListLayout);
        }


        public override void RegisterInfoData()
        {
            base.RegisterInfoData();
            ListRepository.RegisterListMessage(this, SkinXml.ListType);
            GUIActionManager.RegisterAction(XmlActionType.ChangeListView, OnUserViewChange);
        }

     
        public override void DeregisterInfoData()
        {
            base.DeregisterInfoData();
            ListRepository.DeregisterListMessage(this, SkinXml.ListType);
            GUIActionManager.DeregisterAction(this, XmlActionType.ChangeListView);
        }

        public async override void UpdateInfoData()
        {
            base.UpdateInfoData();
            ListItems = await ListRepository.GetCurrentListItems(SkinXml.ListType);
        }

        public override void ClearInfoData()
        {
            base.ClearInfoData();
         
        }

        public async void OnListItemsReceived()
        {
             await Dispatcher.InvokeAsync(OnPropertyChanging);
        }


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

        public async void OnSelectedItemReceived()
        {
              var selectedItem = await ListRepository.GetCurrentSelectedListItem(SkinXml.ListType);
              if (selectedItem != null && ListItems != null)
              {
                  var item = ListItems.FirstOrDefault(x => x.Label == selectedItem.ItemText && x.Index == selectedItem.ItemIndex)
                      ?? ListItems.FirstOrDefault(x => x.Label == selectedItem.ItemText);
                  if (item != null)
                  {
                      await  Dispatcher.InvokeAsync(() =>
                      {
                          SelectItem(item);
                      });
                  }
              }
        
        }



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
                ScrollItemToCenter(listbox.SelectedItem as APIListItem);
                ListRepository.Instance.FocusListControlItem(this, listbox.SelectedItem as APIListItem);
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




      
    }
}
