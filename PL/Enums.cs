using System.Collections;
using System.Diagnostics.PerformanceData;
using BO;
namespace PL.Enums;


/// <summary>
/// Collection that exposes all CourierType enum values
/// for binding in ComboBox
/// </summary>
public class CourierTypesCollection : IEnumerable
{
    static readonly IEnumerable<BO.DeliveryTransport> s_enums =
        (Enum.GetValues(typeof(BO.DeliveryTransport)) as IEnumerable<BO.DeliveryTransport>)!;

    public IEnumerator GetEnumerator() =>
        s_enums.GetEnumerator();
}



