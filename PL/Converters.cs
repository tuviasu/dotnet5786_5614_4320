using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PL.Converters
{
    public class ConvertTransportToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BO.DeliveryTransport transport)
            {
                return transport switch
                {
                    BO.DeliveryTransport.Foot => Brushes.LightGray,
                    BO.DeliveryTransport.Bicycle => Brushes.LightGreen,
                    BO.DeliveryTransport.Motorcycle => Brushes.Orange,
                    BO.DeliveryTransport.Car => Brushes.LightBlue,
                    BO.DeliveryTransport.None => Brushes.White,
                    _ => Brushes.White
                };
            }

            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
