namespace BO;

public class Config
{
    public DateTime Clock { get; set; } = DateTime.Now;

    public string? CompanyAddress { get; set; } = null;
    public double? Latitude { get; set; } = null;
    public double? Longitude { get; set; } = null;
    public double? MaxDeliveryDistance { get; set; } = null;
    public double AverageCarSpeed { get; set; } = 0;
    public double AverageMotorcycleSpeed { get; set; } = 0;
    public double AverageBicycleSpeed { get; set; } = 0;
    public double AverageWalkSpeed { get; set; } = 0;
    public TimeSpan MaxDeliveryTimeRange { get; set; } = TimeSpan.Zero;
    public TimeSpan RiskRange { get; set; } = TimeSpan.Zero;
    public TimeSpan InactivityTimeRange { get; set; } = TimeSpan.Zero;

    public void Reset()
    {
        Clock = DateTime.Now;
        CompanyAddress = null;
        Latitude = 0;
        Longitude = 0;
        MaxDeliveryDistance = null;
        AverageCarSpeed = 0;
        AverageMotorcycleSpeed = 0;
        AverageBicycleSpeed = 0;
        AverageWalkSpeed = 0;
        MaxDeliveryTimeRange = TimeSpan.Zero;
        RiskRange = TimeSpan.Zero;
        InactivityTimeRange = TimeSpan.Zero;
    }
}
