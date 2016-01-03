using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MPDisplay.Common.Controls
{
    public class CollectionEditor : Control
    {
        #region Properties

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(ObservableCollection<object>), typeof(CollectionEditor), new UIPropertyMetadata(null));
        public ObservableCollection<object> Items
        {
            get { return (ObservableCollection<object>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IList), typeof(CollectionEditor), new UIPropertyMetadata(null, OnItemsSourceChanged));
        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var collectionEditor = (CollectionEditor)d;
            collectionEditor?.OnItemSourceChanged((IList)e.OldValue, (IList)e.NewValue);
        }

        public void OnItemSourceChanged(IList oldValue, IList newValue)
        {
            if (newValue == null) return;

            foreach (var item in newValue)
                Items.Add(CreateClone(item));
        }

        public static readonly DependencyProperty ItemsSourceTypeProperty = DependencyProperty.Register("ItemsSourceType", typeof(Type), typeof(CollectionEditor), new UIPropertyMetadata(null, ItemsSourceTypeChanged));
        public Type ItemsSourceType
        {
            get { return (Type)GetValue(ItemsSourceTypeProperty); }
            set { SetValue(ItemsSourceTypeProperty, value); }
        }

        private static void ItemsSourceTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var collectionEditor = (CollectionEditor)d;
            collectionEditor?.ItemsSourceTypeChanged((Type)e.OldValue, (Type)e.NewValue);
        }

        protected virtual void ItemsSourceTypeChanged(Type oldValue, Type newValue)
        {
            NewItemTypes = GetNewItemTypes(newValue);
        }

        public static readonly DependencyProperty NewItemTypesProperty = DependencyProperty.Register("NewItemTypes", typeof(IList), typeof(CollectionEditor), new UIPropertyMetadata(null));
        public IList<Type> NewItemTypes
        {
            get { return (IList<Type>)GetValue(NewItemTypesProperty); }
            set { SetValue(NewItemTypesProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(CollectionEditor), new UIPropertyMetadata(null));
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        #endregion //Properties

        #region Constructors

        static CollectionEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CollectionEditor), new FrameworkPropertyMetadata(typeof(CollectionEditor)));
        }

        public CollectionEditor()
        {
            Items = new ObservableCollection<object>();
            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, AddNew, CanAddNew));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, Delete, CanDelete));
            CommandBindings.Add(new CommandBinding(ComponentCommands.MoveDown, MoveDown, CanMoveDown));
            CommandBindings.Add(new CommandBinding(ComponentCommands.MoveUp, MoveUp, CanMoveUp));
        }

        #endregion //Constructors

        #region Commands

        private void AddNew(object sender, ExecutedRoutedEventArgs e)
        {
            var newItem = CreateNewItem((Type)e.Parameter);
            Items.Add(newItem);
            SelectedItem = newItem;
        }

        private static void CanAddNew(object sender, CanExecuteRoutedEventArgs e)
        {
            var t = e.Parameter as Type;
            if (t?.GetConstructor(Type.EmptyTypes) != null)
                e.CanExecute = true;
        }

        private void Delete(object sender, ExecutedRoutedEventArgs e)
        {
            Items.Remove(e.Parameter);
        }

        private static void CanDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter != null;
        }

        private void MoveDown(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItem = e.Parameter;
            var index = Items.IndexOf(selectedItem);
            Items.RemoveAt(index);
            Items.Insert(++index, selectedItem);
            SelectedItem = selectedItem;
        }

        private void CanMoveDown(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter != null && Items.IndexOf(e.Parameter) < Items.Count - 1)
                e.CanExecute = true;
        }

        private void MoveUp(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItem = e.Parameter;
            var index = Items.IndexOf(selectedItem);
            Items.RemoveAt(index);
            Items.Insert(--index, selectedItem);
            SelectedItem = selectedItem;
        }

        private void CanMoveUp(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter != null && Items.IndexOf(e.Parameter) > 0)
                e.CanExecute = true;
        }

        #endregion //Commands

        #region Methods

        private static void CopyValues(object source, object destination)
        {
            var myObjectFields = source.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (var fi in myObjectFields)
            {
                fi.SetValue(destination, fi.GetValue(source));
            }
        }

        private static object CreateClone(object source)
        {
            var type = source.GetType();
            var clone = Activator.CreateInstance(type);
            CopyValues(source, clone);

            return clone;
        }

        private IList CreateItemsSource()
        {
            IList list = null;

            if (ItemsSourceType == null) return null;

            var constructor = ItemsSourceType.GetConstructor(Type.EmptyTypes);
            if (constructor != null) list = (IList)constructor.Invoke(null);

            return list;
        }

        private static object CreateNewItem(Type type)
        {
            return Activator.CreateInstance(type);
        }

        private static List<Type> GetNewItemTypes(Type type)
        {
            var newItemTypes = type.GetGenericArguments();
            return newItemTypes.ToList();
        }

        public void PersistChanges()
        {
            var list = ResolveItemsSource();
            if (list == null)
                return;

            //the easiest way to persist changes to the source is to just clear the source list and then add all items to it.
            list.Clear();

            foreach (var item in Items)
            {
                list.Add(item);
            }
        }

        private IList ResolveItemsSource()
        {
            return ItemsSource ?? (ItemsSource = CreateItemsSource());
        }

        #endregion //Methods
    }
}