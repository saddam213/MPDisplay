using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace MPDisplay.Common.ExtensionMethods
{

    /// <summary>
    /// Defines an interface that must be implemented to generate the LinqToTree methods
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILinqTree<out T>
    {
        IEnumerable<T> Children();

        T Parent { get; }
    }

    /// <summary>
    /// Adapts a DependencyObject to provide methods required for generate
    /// a Linq To Tree API
    /// </summary>
    public class VisualTreeAdapter : ILinqTree<DependencyObject>
    {
        private DependencyObject _item;

        public VisualTreeAdapter(DependencyObject item)
        {
            _item = item;
        }

        public IEnumerable<DependencyObject> Children()
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(_item);
            for (var i = 0; i < childrenCount; i++)
            {
                yield return VisualTreeHelper.GetChild(_item, i);
            }
        }

        public DependencyObject Parent
        {
            get
            {
                return VisualTreeHelper.GetParent(_item);
            }
        }
    }

    public static class VisualTreeExtensions
    {
        public static DependencyObject FirstVisualChild(this Visual visual)
        {
            if (visual == null) return null;
            return VisualTreeHelper.GetChildrenCount(visual) == 0 ? null : VisualTreeHelper.GetChild(visual, 0);
        }

        public static T GetDescendantByType<T>(this Visual element) where T : Visual
        {
            if (element == null)
            {
                return default(T);
            }
            if (element.GetType() == typeof(T))
            {
                return (T)element;
            }
            Visual foundElement = null;
            var frameworkElement = element as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.ApplyTemplate();
            }
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantByType<T>(visual);
                if (foundElement != null)
                {
                    break;
                }
            }
            return (T)foundElement;
        }

        public static T GetAscendantByType<T>(this Visual element) where T : Visual
        {
            if (element == null)
            {
                return default(T);
            }
            if (element.GetType() == typeof(T))
            {
                return (T)element;
            }
            var frameworkElement = element as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.ApplyTemplate();
            }
            var foundElement = VisualTreeHelper.GetParent(element) as Visual;
            // ReSharper disable once TailRecursiveCall
            return foundElement != null ? GetAscendantByType<T>(foundElement) : null;
        }

        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                var children = child as T;
                if (children != null)
                {
                    yield return children;
                }

                foreach (var childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }
    }









    public static class TreeExtensions
    {
        /// <summary>
        /// Returns a collection of descendant elements.
        /// </summary>
        public static IEnumerable<DependencyObject> Descendants(this DependencyObject item)
        {
            ILinqTree<DependencyObject> adapter = new VisualTreeAdapter(item);
            foreach (var child in adapter.Children())
            {
                yield return child;

                foreach (var grandChild in child.Descendants())
                {
                    yield return grandChild;
                }
            }
        }

        /// <summary>
        /// Returns a collection containing this element and all descendant elements.
        /// </summary>
        public static IEnumerable<DependencyObject> DescendantsAndSelf(this DependencyObject item)
        {
            yield return item;

            foreach (var child in item.Descendants())
            {
                yield return child;
            }
        }

        /// <summary>
        /// Returns a collection of ancestor elements.
        /// </summary>
        public static IEnumerable<DependencyObject> Ancestors(this DependencyObject item)
        {
            ILinqTree<DependencyObject> adapter = new VisualTreeAdapter(item);

            var parent = adapter.Parent;
            while (parent != null)
            {
                yield return parent;
                adapter = new VisualTreeAdapter(parent);
                parent = adapter.Parent;
            }
        }

        /// <summary>
        /// Returns a collection containing this element and all ancestor elements.
        /// </summary>
        public static IEnumerable<DependencyObject> AncestorsAndSelf(this DependencyObject item)
        {
            yield return item;

            foreach (var ancestor in item.Ancestors())
            {
                yield return ancestor;
            }
        }

        /// <summary>
        /// Returns a collection of child elements.
        /// </summary>
        public static IEnumerable<DependencyObject> Elements(this DependencyObject item)
        {
            ILinqTree<DependencyObject> adapter = new VisualTreeAdapter(item);
            return adapter.Children();
        }

        /// <summary>
        /// Returns a collection of the sibling elements before this node, in document order.
        /// </summary>
        public static IEnumerable<DependencyObject> ElementsBeforeSelf(this DependencyObject item)
        {
            if (item.Ancestors().FirstOrDefault() == null)
                yield break;
            foreach (var child in item.Ancestors().First().Elements().TakeWhile(child => !child.Equals(item)))
            {
                yield return child;
            }
        }

        /// <summary>
        /// Returns a collection of the after elements after this node, in document order.
        /// </summary>
        public static IEnumerable<DependencyObject> ElementsAfterSelf(this DependencyObject item)
        {
            if (item.Ancestors().FirstOrDefault() == null)
                yield break;
            var afterSelf = false;
            foreach (var child in item.Ancestors().First().Elements())
            {
                if (afterSelf)
                    yield return child;

                if (child.Equals(item))
                    afterSelf = true;
            }
        }

        /// <summary>
        /// Returns a collection containing this element and all child elements.
        /// </summary>
        public static IEnumerable<DependencyObject> ElementsAndSelf(this DependencyObject item)
        {
            yield return item;

            foreach (var child in item.Elements())
            {
                yield return child;
            }
        }

        /// <summary>
        /// Returns a collection of descendant elements which match the given type.
        /// </summary>
        public static IEnumerable<DependencyObject> Descendants<T>(this DependencyObject item)
        {
            return item.Descendants().Where(i => i is T);
        }



        /// <summary>
        /// Returns a collection of the sibling elements before this node, in document order
        /// which match the given type.
        /// </summary>
        public static IEnumerable<DependencyObject> ElementsBeforeSelf<T>(this DependencyObject item)
        {
            return item.ElementsBeforeSelf().Where(i => i is T);
        }

        /// <summary>
        /// Returns a collection of the after elements after this node, in document order
        /// which match the given type.
        /// </summary>
        public static IEnumerable<DependencyObject> ElementsAfterSelf<T>(this DependencyObject item)
        {
            return item.ElementsAfterSelf().Where(i => i is T);
        }

        /// <summary>
        /// Returns a collection containing this element and all descendant elements
        /// which match the given type.
        /// </summary>
        public static IEnumerable<DependencyObject> DescendantsAndSelf<T>(this DependencyObject item)
        {
            return item.DescendantsAndSelf().Where(i => i is T);
        }

        /// <summary>
        /// Returns a collection of ancestor elements which match the given type.
        /// </summary>
        public static IEnumerable<DependencyObject> Ancestors<T>(this DependencyObject item)
        {
            return item.Ancestors().Where(i => i is T);
        }

        /// <summary>
        /// Returns a collection containing this element and all ancestor elements
        /// which match the given type.
        /// </summary>
        public static IEnumerable<DependencyObject> AncestorsAndSelf<T>(this DependencyObject item)
        {
            return item.AncestorsAndSelf().Where(i => i is T);
        }

        /// <summary>
        /// Returns a collection of child elements which match the given type.
        /// </summary>
        public static IEnumerable<DependencyObject> Elements<T>(this DependencyObject item)
        {
            return item.Elements().Where(i => i is T);
        }

        /// <summary>
        /// Returns a collection containing this element and all child elements.
        /// which match the given type.
        /// </summary>
        public static IEnumerable<DependencyObject> ElementsAndSelf<T>(this DependencyObject item)
        {
            return item.ElementsAndSelf().Where(i => i is T);
        }

    }



    public static class EnumerableTreeExtensions
    {
        /// <summary>
        /// Applies the given function to each of the items in the supplied
        /// IEnumerable.
        /// </summary>
        private static IEnumerable<DependencyObject> DrillDown(this IEnumerable<DependencyObject> items,
            Func<DependencyObject, IEnumerable<DependencyObject>> function)
        {
            return items.SelectMany(function);
        }


        /// <summary>
        /// Applies the given function to each of the items in the supplied
        /// IEnumerable, which match the given type.
        /// </summary>
        public static IEnumerable<DependencyObject> DrillDown<T>(this IEnumerable<DependencyObject> items,
            Func<DependencyObject, IEnumerable<DependencyObject>> function)
            where T : DependencyObject
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var item in items)
            {
                foreach (var down in function(item).OfType<T>())
                {
                    yield return down;
                }
            }
        }


        /// <summary>
        /// Returns a collection of descendant elements.
        /// </summary>
        public static IEnumerable<DependencyObject> Descendants(this IEnumerable<DependencyObject> items)
        {
            return items.DrillDown(i => i.Descendants());
        }

        /// <summary>
        /// Returns a collection containing this element and all descendant elements.
        /// </summary>
        public static IEnumerable<DependencyObject> DescendantsAndSelf(this IEnumerable<DependencyObject> items)
        {
            return items.DrillDown(i => i.DescendantsAndSelf());
        }

        /// <summary>
        /// Returns a collection of ancestor elements.
        /// </summary>
        public static IEnumerable<DependencyObject> Ancestors(this IEnumerable<DependencyObject> items)
        {
            return items.DrillDown(i => i.Ancestors());
        }

        /// <summary>
        /// Returns a collection containing this element and all ancestor elements.
        /// </summary>
        public static IEnumerable<DependencyObject> AncestorsAndSelf(this IEnumerable<DependencyObject> items)
        {
            return items.DrillDown(i => i.AncestorsAndSelf());
        }

        /// <summary>
        /// Returns a collection of child elements.
        /// </summary>
        public static IEnumerable<DependencyObject> Elements(this IEnumerable<DependencyObject> items)
        {
            return items.DrillDown(i => i.Elements());
        }

        /// <summary>
        /// Returns a collection containing this element and all child elements.
        /// </summary>
        public static IEnumerable<DependencyObject> ElementsAndSelf(this IEnumerable<DependencyObject> items)
        {
            return items.DrillDown(i => i.ElementsAndSelf());
        }


        /// <summary>
        /// Returns a collection of descendant elements which match the given type.
        /// </summary>
        public static IEnumerable<DependencyObject> Descendants<T>(this IEnumerable<DependencyObject> items)
            where T : DependencyObject
        {
            return items.DrillDown<T>(i => i.Descendants());
        }

        /// <summary>
        /// Returns a collection containing this element and all descendant elements.
        /// which match the given type.
        /// </summary>
        public static IEnumerable<DependencyObject> DescendantsAndSelf<T>(this IEnumerable<DependencyObject> items)
            where T : DependencyObject
        {
            return items.DrillDown<T>(i => i.DescendantsAndSelf());
        }

        /// <summary>
        /// Returns a collection of ancestor elements which match the given type.
        /// </summary>
        public static IEnumerable<DependencyObject> Ancestors<T>(this IEnumerable<DependencyObject> items)
            where T : DependencyObject
        {
            return items.DrillDown<T>(i => i.Ancestors());
        }

        /// <summary>
        /// Returns a collection containing this element and all ancestor elements.
        /// which match the given type.
        /// </summary>
        public static IEnumerable<DependencyObject> AncestorsAndSelf<T>(this IEnumerable<DependencyObject> items)
            where T : DependencyObject
        {
            return items.DrillDown<T>(i => i.AncestorsAndSelf());
        }

        /// <summary>
        /// Returns a collection of child elements which match the given type.
        /// </summary>
        public static IEnumerable<DependencyObject> Elements<T>(this IEnumerable<DependencyObject> items)
            where T : DependencyObject
        {
            return items.DrillDown<T>(i => i.Elements());
        }

        /// <summary>
        /// Returns a collection containing this element and all child elements.
        /// which match the given type.
        /// </summary>
        public static IEnumerable<DependencyObject> ElementsAndSelf<T>(this IEnumerable<DependencyObject> items)
            where T : DependencyObject
        {
            return items.DrillDown<T>(i => i.ElementsAndSelf());
        }
    }


}
