namespace Dal;

using System.Runtime.CompilerServices;

public static class Config
{
    internal const string ConfigFileName = "data-config.xml";
    internal const string CouriersFileName = "couriers.xml";
    internal const string OrdersFileName = "orders.xml";
    internal const string DeliveriesFileName = "deliveries.xml";

    internal static int NextOrderID
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetAndIncreaseConfigIntVal(ConfigFileName, "NextOrderID");
        [MethodImpl(MethodImplOptions.Synchronized)]
        private set => XMLTools.SetConfigIntVal(ConfigFileName, "NextOrderID", value);
    }

    internal static int NextDeliveryID
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetAndIncreaseConfigIntVal(ConfigFileName, "NextDeliveryID");
        [MethodImpl(MethodImplOptions.Synchronized)]
        private set => XMLTools.SetConfigIntVal(ConfigFileName, "NextDeliveryID", value);
    }

    internal static DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDateVal(ConfigFileName, "Clock");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDateVal(ConfigFileName, "Clock", value);
    }

    internal static TimeSpan InactivityTimeRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigTimeSpanVal(ConfigFileName, "InactivityTimeRange");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigTimeSpanVal(ConfigFileName, "InactivityTimeRange", value);
    }

    internal static TimeSpan RiskRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigTimeSpanVal(ConfigFileName, "RiskRange");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigTimeSpanVal(ConfigFileName, "RiskRange", value);
    }

    internal static TimeSpan MaxDeliveryTimeRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigTimeSpanVal(ConfigFileName, "MaxDeliveryTimeRange");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigTimeSpanVal(ConfigFileName, "MaxDeliveryTimeRange", value);
    }

    internal static double? MaxDeliveryDistance
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDoubleVal(ConfigFileName, "MaxDeliveryDistance");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDoubleVal(ConfigFileName, "MaxDeliveryDistance", value);
    }

    internal static Dictionary<int, string> Managers
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetManagers(ConfigFileName);
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetManagers(ConfigFileName, value);
    }

    internal static Dictionary<int, string> Couriers
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetCouriers(ConfigFileName);
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetCouriers(ConfigFileName, value);
    }

    internal static double? Latitude
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDoubleVal(ConfigFileName, "Latitude");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDoubleVal(ConfigFileName, "Latitude", value);
    }

    internal static double? Longitude
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDoubleVal(ConfigFileName, "Longitude");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDoubleVal(ConfigFileName, "Longitude", value);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
    {
        NextOrderID = 1000;
        NextDeliveryID = 100000;
        Clock = DateTime.Now;
        Managers = new() { { 204857392, "admin123" }, };
        Couriers = new() { { 312458962, "courier123" }, };
        MaxDeliveryDistance = null;
        MaxDeliveryTimeRange = TimeSpan.Zero;
        RiskRange = TimeSpan.Zero;
        InactivityTimeRange = TimeSpan.Zero;

        Latitude = 0;
        Longitude = 0;
    }
}
