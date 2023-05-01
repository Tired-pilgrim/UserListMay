using System.Windows;
using media = System.Windows.Media;

namespace ViewLib
{
    public static class VisualTreeHelper
    {
        public static T? FindAncestor<T>(this DependencyObject dobj)
            where T : DependencyObject
        {
            while (dobj is not null)
            {
                if (dobj is T t)
                    return t;

                dobj = media.VisualTreeHelper.GetParent(dobj);
            }
            return null;
        }

        public static FrameworkElement? FindDataAncestor<TData>(this DependencyObject dobj)
        {
            while (dobj is not null)
            {
                if (dobj is FrameworkElement element and { DataContext: TData })
                    return element;

                dobj = media.VisualTreeHelper.GetParent(dobj);
            }
            return null;
        }

        public static TData? FindData<TData>(this DependencyObject dobj)
        {
            FrameworkElement? element = dobj.FindDataAncestor<TData>();
            if (element == null)
                return default;
            return (TData)element.DataContext;
        }
    }
}
