namespace Presentation.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public class CategoryTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string categoryType)
            {
                switch (categoryType)
                {
                    case "Витрати":
                        return Brushes.Red;
                    case "Дохід":
                        return Brushes.Green;
                    case "Баланс":
                        return Brushes.Blue;
                    default:
                        return Brushes.Gray;
                }
            }

            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
