using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TiqSoft.ScreenAssistant.ScreenInfoRecognition.Recognizers.ApexLegends;

namespace TiqSoft.ScreenAssistant.Helpers.Converters
{
    public class ModuleTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Brush))
                throw new ArgumentException("The target must be a Brush");

            Debug.Assert(value != null, nameof(value) + " != null");

            var moduleType = (WeaponModuleType)value;
            switch (moduleType)
            {
                case WeaponModuleType.None:
                    return new SolidColorBrush(Colors.White);
                case WeaponModuleType.Common:
                    return new SolidColorBrush(Colors.DimGray);
                case WeaponModuleType.Rare:
                    return new SolidColorBrush(Colors.CornflowerBlue);
                case WeaponModuleType.Epic:
                    return new SolidColorBrush(Colors.DarkMagenta);
                case WeaponModuleType.Legendary:
                    return new SolidColorBrush(Colors.Goldenrod);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}