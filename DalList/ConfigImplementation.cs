namespace Dal;
using DalApi;
using System;

public class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    public int ManagerId
    {
        get => Config.ManagerId;
        set => Config.ManagerId = value;
    }

    public string ManagerPassword
    {
        get => Config.ManagerPassword;
        set => Config.ManagerPassword = value;
    }

    public string? CompanyAddress
    {
        get => Config.CompanyAddress;
        set => Config.CompanyAddress = value;
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

    public int MaxRange
    {
        get => Config.MaxRange ?? throw new InvalidOperationException("MaxRange is not set.");
        set => Config.MaxRange = value;
    }

    public double AvgCarSpeed
    {
        get => Config.AvgCarSpeed;
        set => Config.AvgCarSpeed = value;
    }

    public double AvgMotorbikeSpeed
    {
        get => Config.AvgMotorbikeSpeed;
        set => Config.AvgMotorbikeSpeed = value;
    }

    public double AvgBicycleSpeed
    {
        get => Config.AvgBicycleSpeed;
        set => Config.AvgBicycleSpeed = value;
    }

    public double AvgWalkingSpeed
    {
        get => Config.AvgWalkingSpeed;
        set => Config.AvgWalkingSpeed = value;
    }

    public TimeSpan MaxDeliveryTime
    {
        get => Config.MaxDeliveryTime;
        set => Config.MaxDeliveryTime = value;
    }

    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }

    public TimeSpan InactivityRange
    {
        get => Config.InactivityRange;
        set => Config.InactivityRange = value;
    }

    public int NextOrderId
    {
        get => Config.NextOrderId;
        set => throw new NotSupportedException("NextOrderId is read-only.");
    }

    public int NextDeliveryId
    {
        get => Config.NextDeliverId;
        set => throw new NotSupportedException("NextDeliveryId is read-only.");
    }

    public void Reset()
    {
        Config.Reset();
    }
}
