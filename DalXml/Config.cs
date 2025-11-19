namespace Dal;

internal static class Config
{
    // ─────────────────────────────────────────────
    // XML file names
    // ─────────────────────────────────────────────

    // The main configuration XML file
    internal const string s_data_config_xml = "data-config.xml";

    // Database XML files for entities
    internal const string s_orders_xml = "orders.xml";
    internal const string s_deliveries_xml = "deliveries.xml";
    internal const string s_couriers_xml = "couriers.xml";



    // ─────────────────────────────────────────────
    // Auto-increment fields (running IDs)
    // ─────────────────────────────────────────────

    // Gets the current Order ID from config XML, returns it,
    // and automatically increases it by +1 inside the file.
    internal static int NextOrderId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextOrderId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextOrderId", value);
    }

    // Same logic for delivery IDs
    internal static int NextDeliveryId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextDeliveryId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextDeliveryId", value);
    }


    // ─────────────────────────────────────────────
    // General configuration values
    // (all of these are stored and loaded from data-config.xml)
    // ─────────────────────────────────────────────

    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }

    internal static int ManagerId
    {
        get => XMLTools.GetConfigIntVal(s_data_config_xml, "ManagerId");
        set => XMLTools.SetConfigIntVal(s_data_config_xml, "ManagerId", value);
    }

    internal static string ManagerPassword
    {
        get => XMLTools.GetConfigStringVal(s_data_config_xml, "ManagerPassword");
        set => XMLTools.SetConfigStringVal(s_data_config_xml, "ManagerPassword", value);
    }

    internal static string? CompanyAddress
    {
        get => XMLTools.GetConfigStringNullableVal(s_data_config_xml, "CompanyAddress");
        set => XMLTools.SetConfigStringVal(s_data_config_xml, "CompanyAddress", value ?? string.Empty);
    }

    internal static double? Latitude
    {
        get => XMLTools.GetConfigDoubleNullableVal(s_data_config_xml, "Latitude");
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "Latitude", value ?? 0);
    }

    internal static double? Longitude
    {
        get => XMLTools.GetConfigDoubleNullableVal(s_data_config_xml, "Longitude");
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "Longitude", value ?? 0);
    }

    internal static int MaxRange
    {
        get => XMLTools.GetConfigIntVal(s_data_config_xml, "MaxRange");
        set => XMLTools.SetConfigIntVal(s_data_config_xml, "MaxRange", value);
    }

    internal static double AvgCarSpeed
    {
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgCarSpeed");
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgCarSpeed", value);
    }

    internal static double AvgMotorbikeSpeed
    {
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgMotorbikeSpeed");
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgMotorbikeSpeed", value);
    }

    internal static double AvgBicycleSpeed
    {
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgBicycleSpeed");
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgBicycleSpeed", value);
    }

    internal static double AvgWalkingSpeed
    {
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgWalkingSpeed");
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgWalkingSpeed", value);
    }

    internal static TimeSpan MaxDeliveryTime
    {
        get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "MaxDeliveryTime");
        set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "MaxDeliveryTime", value);
    }

    internal static TimeSpan RiskRange
    {
        get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "RiskRange");
        set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "RiskRange", value);
    }

    internal static TimeSpan InactivityRange
    {
        get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "InactivityRange");
        set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "InactivityRange", value);
    }


    // ─────────────────────────────────────────────
    // Reset: resets all configuration values
    // back to initial defaults inside the XML file
    // ─────────────────────────────────────────────

    internal static void Reset()
    {
        NextOrderId = 1;
        NextDeliveryId = 1;

        Clock = DateTime.Now;

        ManagerId = 0;
        ManagerPassword = "";
        CompanyAddress = "";

        Latitude = null;
        Longitude = null;

        MaxRange = 100;

        AvgCarSpeed = 50;
        AvgMotorbikeSpeed = 40;
        AvgBicycleSpeed = 20;
        AvgWalkingSpeed = 5;

        MaxDeliveryTime = TimeSpan.FromHours(3);
        RiskRange = TimeSpan.FromHours(1);
        InactivityRange = TimeSpan.FromHours(2);
    }
}
