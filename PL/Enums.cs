namespace PL;
using BO;
using System;
using System.Collections;
using System.Collections.Generic;

public class DeliveryTransport : IEnumerable<BO.DeliveryTransport>
{
    static readonly IEnumerable<BO.DeliveryTransport> s_enums =
        (Enum.GetValues(typeof(BO.DeliveryTransport)) as IEnumerable<BO.DeliveryTransport>)!;
    public IEnumerator<BO.DeliveryTransport> GetEnumerator() => s_enums.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}
public class DeliveryType : IEnumerable<BO.DeliveryType>
{
    static readonly IEnumerable<BO.DeliveryType> s_enums =
        (Enum.GetValues(typeof(BO.DeliveryType)) as IEnumerable<BO.DeliveryType>)!;
    public IEnumerator<BO.DeliveryType> GetEnumerator() => s_enums.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}

public class DeliveryDoneType : IEnumerable<BO.DeliveryDoneType>
{
    static readonly IEnumerable<BO.DeliveryDoneType> s_enums =
        (Enum.GetValues(typeof(BO.DeliveryDoneType)) as IEnumerable<BO.DeliveryDoneType>)!;

    public IEnumerator<BO.DeliveryDoneType> GetEnumerator() => s_enums.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class OrderType : IEnumerable<BO.OrderType>
{
    static readonly IEnumerable<BO.OrderType> s_enums =
        (Enum.GetValues(typeof(BO.OrderType)) as IEnumerable<BO.OrderType>)!;
    public IEnumerator<BO.OrderType> GetEnumerator() => s_enums.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class OrderStatus : IEnumerable<BO.OrderStatus>
{
    static readonly IEnumerable<BO.OrderStatus> s_enums =
        (Enum.GetValues(typeof(BO.OrderStatus)) as IEnumerable<BO.OrderStatus>)!;
    public IEnumerator<BO.OrderStatus> GetEnumerator() => s_enums.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class PizzaSize : IEnumerable<BO.DeviceType>
{
    static readonly IEnumerable<BO.DeviceType> s_enums =
        (Enum.GetValues(typeof(BO.DeviceType)) as IEnumerable<BO.DeviceType>)!;
    public IEnumerator<BO.DeviceType> GetEnumerator() => s_enums.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class OrderListFilterProperty : IEnumerable<BO.OrderListFilterProperty>
{
    static readonly IEnumerable<BO.OrderListFilterProperty> s_enums =
        (Enum.GetValues(typeof(BO.OrderListFilterProperty)) as IEnumerable<BO.OrderListFilterProperty>)!;
    public IEnumerator<BO.OrderListFilterProperty> GetEnumerator() => s_enums.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class OrderListSortProperty : IEnumerable<BO.OrderListSortProperty>
{
    static readonly IEnumerable<BO.OrderListSortProperty> s_enums =
        (Enum.GetValues(typeof(BO.OrderListSortProperty)) as IEnumerable<BO.OrderListSortProperty>)!;
    public IEnumerator<BO.OrderListSortProperty> GetEnumerator() => s_enums.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class ScheduleStatus : IEnumerable<BO.ScheduleStatus>
{
    static readonly IEnumerable<BO.ScheduleStatus> s_enums =
        (Enum.GetValues(typeof(BO.ScheduleStatus)) as IEnumerable<BO.ScheduleStatus>)!;
    public IEnumerator<BO.ScheduleStatus> GetEnumerator() => s_enums.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
