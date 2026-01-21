using DalApi;

namespace Dal;

internal class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    public IReadOnlyDictionary<int, string> Managers => Config.Managers;

    public IReadOnlyDictionary<int, string> Couriers => Config.Couriers;

    public string? CompanyAddress
    {
        get => null;
        set { }
    }

    public double? Latitude
    {
        get => Config.Latitude;
        set => Config.Latitude = value;
    }

    public double? Longitude
    {
        get => Config.Longitude;
        set => Config.Longitude = value;
    }

    public double? MaxDeliveryDistance
    {
        get => Config.MaxDeliveryDistance;
        set => Config.MaxDeliveryDistance = value;
    }

    public TimeSpan MaxDeliveryTimeRange
    {
        get => Config.MaxDeliveryTimeRange;
        set => Config.MaxDeliveryTimeRange = value;
    }

    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }

    public TimeSpan InactivityTimeRange
    {
        get => Config.InactivityTimeRange;
        set => Config.InactivityTimeRange = value;
    }

    public void Reset()
    {
        Config.Reset();
    }
}
