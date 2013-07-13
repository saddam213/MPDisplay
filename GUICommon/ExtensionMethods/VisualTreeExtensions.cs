using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MPDisplay.Common.ExtensionMethods
{
    public static class VisualTreeExtensions
    {
        public static DependencyObject FirstVisualChild(this Visual visual)
        {
            if (visual == null) return null;
            if (VisualTreeHelper.GetChildrenCount(visual) == 0) return null;
            return VisualTreeHelper.GetChild(visual, 0);
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
            if (element is FrameworkElement)
            {
                (element as FrameworkElement).ApplyTemplate();
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
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
            Visual foundElement = null;
            if (element is FrameworkElement)
            {
                (element as FrameworkElement).ApplyTemplate();
            }
            foundElement = VisualTreeHelper.GetParent(element) as Visual;
            if (foundElement != null)
            {
                return GetAscendantByType<T>(foundElement);
            }
            return (T)foundElement;
        }

        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
