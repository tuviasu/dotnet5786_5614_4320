using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PL;

public class ConverterUpdateToTrue: IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string strValue && strValue == "Update")
        {
            return true;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}

public class ConverterAddToTrue: IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string strValue && strValue == "Add")
        {
            return true;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}

public class ConverterAddToFalse : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string strValue && strValue == "Add")
            return false;

        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}

public class ConverterUpdateToVisible : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string strValue && strValue == "Update")
        {
            return Visibility.Hidden;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}

public class ConvertAddToVisibleKey : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string strValue && strValue == "Add")
        {
            return Visibility.Visible;
        }
        return Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}
public class ConvertAddToNotVisibleKey : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string strValue && strValue == "Add")
        {
            return Visibility.Hidden;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}

public class TransportTypeToBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BO.DeliveryTransport transportType)
        {
            return transportType switch
            {
                BO.DeliveryTransport.Car => new SolidColorBrush(Colors.LightBlue),
                BO.DeliveryTransport.Motorcycle => new SolidColorBrush(Colors.LightGreen),
                BO.DeliveryTransport.Bicycle => new SolidColorBrush(Colors.LightYellow),
                BO.DeliveryTransport.Walk => new SolidColorBrush(Colors.LightPink),
                BO.DeliveryTransport.All => new SolidColorBrush(Colors.White),
                _ => new SolidColorBrush(Colors.White)
            };
        }
        return new SolidColorBrush(Colors.White);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class DeliveryTypeToBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BO.DeliveryType deliveryType)
        {
            return deliveryType switch
            {
                BO.DeliveryType.Express => new SolidColorBrush(Colors.Gold),
                BO.DeliveryType.Regular => new SolidColorBrush(Colors.LimeGreen),
                _ => new SolidColorBrush(Colors.White)
            };
        }
        return new SolidColorBrush(Colors.White);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool flag = value is bool b && b;
        return flag ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility v)
            return v == Visibility.Visible;
        return false;
    }
}

public class InverseBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool flag = value is bool b && b;
        return flag ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility v)
            return v != Visibility.Visible;
        return false;
    }
}

public class OrderStatusToCancelVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BO.OrderStatus status)
            return (status == BO.OrderStatus.Delivered || status == BO.OrderStatus.Cancelled)
                ? Visibility.Collapsed
                : Visibility.Visible;

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value == null ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Formats a TimeSpan into a compact, human-readable string.
/// Negative spans (overdue) collapse to "00:00:00 (Overdue)" so the UI never
/// shows raw negative TimeSpan dumps like "-458.16:01:58".
/// Spans >= 1 day render as "2d 3h 15m"; shorter spans render as "hh:mm:ss".
/// </summary>
public class TimeSpanToReadableConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TimeSpan ts)
        {
            if (ts < TimeSpan.Zero)
                return "00:00:00 (Overdue)";

            if (ts.TotalDays >= 1)
            {
                int days = (int)ts.TotalDays;
                int hours = ts.Hours;
                int minutes = ts.Minutes;
                return $"{days}d {hours}h {minutes}m";
            }

            return ts.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);
        }

        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // One-way only.
        throw new NotImplementedException();
    }
}