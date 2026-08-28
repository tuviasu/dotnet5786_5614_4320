namespace DalApi;
using DO;

public interface IConfig
{
    DateTime Clock { get; set; }

    IReadOnlyDictionary<int, string> Managers { get; }

    // Fixed courier credentials (similar to managers)
    IReadOnlyDictionary<int, string> Couriers { get; }

    string? CompanyAddress { get; set; }
    double? Latitude { get; set; }
    double? Longitude { get; set; }

    double? MaxDeliveryDistance { get; set; }
    TimeSpan MaxDeliveryTimeRange { get; set; }
    TimeSpan RiskRange { get; set; }
    TimeSpan InactivityTimeRange { get; set; }
    void Reset();
}
