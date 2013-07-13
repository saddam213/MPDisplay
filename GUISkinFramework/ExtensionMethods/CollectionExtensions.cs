using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUISkinFramework.ExtensionMethods
{
    public static class CollectionExtensions
    {




        public static void MoveItemUp<T>(this ObservableCollection<T> collection, int currentIndex)
        {
            if (collection != null && collection.Any())
            {
                collection.Move(currentIndex, Math.Max(0, currentIndex - 1));
            }
        }

        public static bool CanItemMoveUp<T>(this ObservableCollection<T> collection, int currentIndex)
        {
            return currentIndex > 0;
        }

        public static void MoveItemDown<T>(this ObservableCollection<T> collection, int currentIndex)
        {
            if (collection != null && collection.Any())
            {
                collection.Move(currentIndex, Math.Min(collection.Count - 1, currentIndex + 1));
            }
        }

        public static bool CanItemMoveDown<T>(this ObservableCollection<T> collection, int currentIndex)
        {
            if (collection != null && collection.Any())
            {
                return currentIndex >= 0 && currentIndex < collection.Count - 1;
            }
            return false;
        }
    }
}
