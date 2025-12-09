using BlApi;

namespace BlImplementation
{
    internal class Bl : IBl
    {
        public ICourier Courier { get; } = new CourierImplementation();

        public IOrder order { get; } = new OrderImplementation();

        public IAdmin admin { get; } = new AdminImplementation();
    }
}
