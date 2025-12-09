namespace BlImplementation
{
    using BlApi;
    using BO;
    using Helpers;
    using System.Collections.Generic;

    internal class CourierImplementation : ICourier
    {
        // ---------------------------------------------------------
        // CREATE COURIER
        // ---------------------------------------------------------
        public void Create(int requesterId, BO.Courier courier)
        {
            CourierManager.CreateCourier(requesterId, courier);
        }

        // ---------------------------------------------------------
        // DELETE COURIER
        // ---------------------------------------------------------
        public void Delete(int requesterId, int courierId)
        {
            CourierManager.DeleteCourier(requesterId, courierId);
        }

        // ---------------------------------------------------------
        // LOGIN (by username)
        // ---------------------------------------------------------
        public string Login(string username)
        {
            return CourierManager.LoginCourier(username);
        }

        // ---------------------------------------------------------
        // READ COURIER BY ID
        // ---------------------------------------------------------
        public BO.Courier Read(int requesterId, int courierId)
        {
            return CourierManager.GetCourier(requesterId, courierId);
        }

        // ---------------------------------------------------------
        // READ ALL COURIERS
        // ---------------------------------------------------------
        public IEnumerable<BO.CourierInList> ReadAll(
            int requesterId,
            bool? activeFilter = null,
            CourierListSortBy? sortBy = null)
        {
            return CourierManager.GetCouriersList(requesterId, activeFilter, sortBy);
        }

        // ---------------------------------------------------------
        // UPDATE COURIER
        // ---------------------------------------------------------
        public void Update(int requesterId, BO.Courier courier)
        {
            CourierManager.UpdateCourier(requesterId, courier);
        }
    }
}
