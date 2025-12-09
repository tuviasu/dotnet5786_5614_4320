
namespace BlApi
{
    public interface IBl
    {
        ICourier Courier { get; }
        IOrder order { get; }
        IAdmin admin { get; }
    }
}
