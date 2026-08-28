namespace Dal;

using System.Runtime.CompilerServices;

public static class Config
{
    //Order
    internal const int startOrderID = 1000;
    private static int nextOrderID = startOrderID;
    internal static int NextOrderID => Interlocked.Increment(ref nextOrderID) - 1;

    //Delivery
    internal const int startDeliveryID = 100000;
    private static int nextDeliveryID = startDeliveryID;
    internal static int NextDeliveryID => Interlocked.Increment(ref nextDeliveryID) - 1;

    //other
    internal static DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)] get;
        [MethodImpl(MethodImplOptions.Synchronized)] set;
    } = DateTime.Now;

    internal static Dictionary<int, string> Managers
    {
        [MethodImpl(MethodImplOptions.Synchronized)] get;
        [MethodImpl(MethodImplOptions.Synchronized)] set;
    } = new()
    {
        { 111111111, "admin123" },
       
    };

    internal static Dictionary<int, string> Couriers
    {
        [MethodImpl(MethodImplOptions.Synchronized)] get;
        [MethodImpl(MethodImplOptions.Synchronized)] set;
    } = new()
    {
        
    };

    internal static string? CompanyAddress
    {
        [MethodImpl(MethodImplOptions.Synchronized)] get;
        [MethodImpl(MethodImplOptions.Synchronized)] set;
    } = "39 Rothschild Street, Tel Aviv";

    internal static double? Latitude
    {
        [MethodImpl(MethodImplOptions.Synchronized)] get;
        [MethodImpl(MethodImplOptions.Synchronized)] set;
    } = 32.064012;

    internal static double? Longitude
    {
        [MethodImpl(MethodImplOptions.Synchronized)] get;
        [MethodImpl(MethodImplOptions.Synchronized)] set;
    } = 34.774101;

    internal static double? MaxDeliveryDistance
    {
        [MethodImpl(MethodImplOptions.Synchronized)] get;
        [MethodImpl(MethodImplOptions.Synchronized)] set;
    } = 30;

    internal static double AverageCarSpeed
    {
        [MethodImpl(MethodImplOptions.Synchronized)] get;
        [MethodImpl(MethodImplOptions.Synchronized)] set;
    } = 0;

    internal static double AverageMotorcycleSpeed
    {
        [MethodImpl(MethodImplOptions.Synchronized)] get;
        [MethodImpl(MethodImplOptions.Synchronized)] set;
    } = 0;

    internal static double AverageBicycleSpeed
    {
        [MethodImpl(MethodImplOptions.Synchronized)] get;
        [MethodImpl(MethodImplOptions.Synchronized)] set;
    } = 0;

    internal static double AverageWalkSpeed
    {
        [MethodImpl(MethodImplOptions.Synchronized)] get;
        [MethodImpl(MethodImplOptions.Synchronized)] set;
    } = 0;

    internal static TimeSpan MaxDeliveryTimeRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)] get;
        [MethodImpl(MethodImplOptions.Synchronized)] set;
    } = TimeSpan.FromHours(1.5);

    internal static TimeSpan RiskRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)] get;
        [MethodImpl(MethodImplOptions.Synchronized)] set;
    } = TimeSpan.FromMinutes(15);

    internal static TimeSpan InactivityTimeRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)] get;
        [MethodImpl(MethodImplOptions.Synchronized)] set;
    } = TimeSpan.FromHours(8);

    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
    {
        Interlocked.Exchange(ref nextOrderID, startOrderID);
        Interlocked.Exchange(ref nextDeliveryID, startDeliveryID);
        Clock = DateTime.Now;

        Managers = new()
        {
            { 111111111, "admin123" },
           
        };

        Couriers = new()
        {
            
        };

        CompanyAddress = "39 Rothschild Street, Tel Aviv";
        Latitude = 32.064012;
        Longitude = 34.774101;
        MaxDeliveryDistance = 30;
        AverageCarSpeed = 0;
        AverageMotorcycleSpeed = 0;
        AverageBicycleSpeed = 0;
        AverageWalkSpeed = 0;
        MaxDeliveryTimeRange = TimeSpan.FromHours(1.5);
        RiskRange = TimeSpan.FromMinutes(15);
        InactivityTimeRange = TimeSpan.FromHours(8);
    }
}
