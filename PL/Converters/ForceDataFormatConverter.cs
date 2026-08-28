using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PL;

public class ForceDataFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is string str && DateTime.TryParseExact(str, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
        {
            return dateTime;
        }
        return value;
    }
}
