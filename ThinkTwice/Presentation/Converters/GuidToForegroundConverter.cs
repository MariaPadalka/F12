namespace Presentation.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public class GuidToForegroundConverter : IValueConverter
    {
        private readonly string emptyID = "1122F421-1716-410A-A1F2-334C3DC17096";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Guid id && id == new Guid(this.emptyID))
            {
                return Brushes.White;
            }

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
