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
        public IEnumerable<BO.CourierInList> ReadAll(
    int requesterId,
    BO.CourierFieldFilter filterField,
    object filterValue)
        {
            var couriers = CourierManager.GetCouriersList(requesterId, null, null);

            return filterField switch
            {
                BO.CourierFieldFilter.Transport =>
                    couriers.Where(c => c.Transport == (BO.DeliveryTransport)filterValue),

                BO.CourierFieldFilter.IsActive =>
                    couriers.Where(c => c.IsActive == (bool)filterValue),

                _ => couriers
            };
        }


        // ---------------------------------------------------------
        // UPDATE COURIER
        // ---------------------------------------------------------
        public void Update(int requesterId, BO.Courier courier)
        {
            CourierManager.UpdateCourier(requesterId, courier);
        }

        #region Stage 5
        public void AddObserver(Action listObserver) =>
                   CourierManager.Observers.AddListObserver(listObserver); //stage 5
        public void AddObserver(int id, Action observer) =>
                   CourierManager.Observers.AddObserver(id, observer); //stage 5
        public void RemoveObserver(Action listObserver) =>
                   CourierManager.Observers.RemoveListObserver(listObserver); //stage 5
        public void RemoveObserver(int id, Action observer) =>
                  CourierManager.Observers.RemoveObserver(id, observer); //stage 5
        #endregion Stage 5


    }
}
