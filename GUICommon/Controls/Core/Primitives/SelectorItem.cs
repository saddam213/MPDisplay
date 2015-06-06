using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MPDisplay.Common.Controls.Core
{
    public class SelectorItem : ContentControl
    {
        #region Constructors

        static SelectorItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectorItem), new FrameworkPropertyMetadata(typeof(SelectorItem)));
        }

        public SelectorItem()
        {
            AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseDown));
        }

        #endregion //Constructors

        #region Properties

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(SelectorItem), new UIPropertyMetadata(false, OnIsSelectedChanged));
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        private static void OnIsSelectedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var selectorItem = o as SelectorItem;
            if (selectorItem != null)
                selectorItem.OnIsSelectedChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
            RaiseSelectionChangedEvent(newValue
                ? new RoutedEventArgs(Selector.SelectedEvent, this)
                : new RoutedEventArgs(Selector.UnSelectedEvent, this));
        }

        internal Selector ParentSelector
        {
            get { return ItemsControl.ItemsControlFromItemContainer(this) as Selector; }
        }

        #endregion //Properties

        #region Events

        public static readonly RoutedEvent SelectedEvent = Selector.SelectedEvent.AddOwner(typeof(SelectorItem));
        public static readonly RoutedEvent UnselectedEvent = Selector.UnSelectedEvent.AddOwner(typeof(SelectorItem));

        #endregion

        #region Event Hanlders

        void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsSelected = !IsSelected;
        }

        #endregion //Event Hanlders

        #region Methods

        private void RaiseSelectionChangedEvent(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        #endregion //Methods
    }
}
