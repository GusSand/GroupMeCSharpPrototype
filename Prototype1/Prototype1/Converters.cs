using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Prototype1.Converters
{
    /// <summary>
    /// Used in the control of displaying the dm icon in the conversation panorama item.
    /// 
    /// Will intelligently turn on an indicator showing that the conversation is a dm.
    /// </summary>
    /// 
    public class ConversationToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value == null)
                return Visibility.Collapsed;


            return ((string)parameter == (string)value) ? Visibility.Visible : Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            return null;
        }
    }

}
