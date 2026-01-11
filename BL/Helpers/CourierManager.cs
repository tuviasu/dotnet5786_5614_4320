using DalApi;
using System.Linq;

namespace Helpers
{
    internal static class CourierManager
    {
        internal static ObserverManager Observers = new(); //stage 5 
        private static readonly IDal dal = DalApi.Factory.Get;

        // -------------------------------------------------------------
        // Convert DO.Courier → BO.CourierInList
        // -------------------------------------------------------------
        internal static BO.CourierInList ConvertToCourierInList(DO.Courier courier)
        {
            int deliveriesCount = dal.Delivery
                .ReadAll(d => d!.CourierId == courier.Id)
                .Count();

            return new BO.CourierInList
            {
                Id = courier.Id,
                Name = courier.Name,
                IsActive = courier.IsActive,
                Transport = (BO.DeliveryTransport)courier.Transport,
                StartWorkingDate = courier.StartWorkingDate,
                DeliveredCountOnTime = 0,
                DeliveredCountLate = 0,
                ActiveOrderId = null,
            };
        }

        // -------------------------------------------------------------
        // Convert DO.Courier → BO.Courier (full object)
        // -------------------------------------------------------------
        internal static BO.Courier ConvertToCourier(DO.Courier courier)
        {
            var deliveries = dal.Delivery.ReadAll(d => d!.CourierId == courier.Id).ToList();

            return new BO.Courier
            {
                Id = courier.Id,
                Name = courier.Name,
                Phone = courier.Phone,
                Email = courier.Email,
                IsActive = courier.IsActive,
                Transport = (BO.DeliveryTransport)courier.Transport,
                MaxRange = courier.MaxDistance,
            };
        }

        // -------------------------------------------------------------
        // CREATE COURIER
        // -------------------------------------------------------------
        internal static void CreateCourier(int requesterId, BO.Courier courier)
        {
            //int newId = dal.Config.NextCourierId;
            var newCourier = new DO.Courier
            {
                //Id = newId,
                Id = courier.Id,
                Name = courier.Name,
                Phone = courier.Phone,
                Email = courier.Email,
                Password = "1234",
                IsActive = courier.IsActive,
                Transport = (DO.DeliveryTransport)courier.Transport,
                StartWorkingDate = AdminManager.Now,
                MaxDistance = courier.MaxRange
            };
            dal.Courier.Create(newCourier);

            Observers.NotifyListUpdated(); //stage 5
        }

        // -------------------------------------------------------------
        // READ COURIER
        // -------------------------------------------------------------
        internal static BO.Courier GetCourier(int requesterId, int courierId)
        {
            var doCourier = dal.Courier.Read(courierId)
                ?? throw new BO.BlDoesNotExistException($"Courier {courierId} not found."); 

            return ConvertToCourier(doCourier);
        }

        // -------------------------------------------------------------
        // READ ALL COURIERS
        // -------------------------------------------------------------
        internal static IEnumerable<BO.CourierInList> GetCouriersList(
            int requesterId,
            bool? activeFilter,
            BO.CourierListSortBy? sortBy)
        {
            var list = dal.Courier.ReadAll().Select(c => ConvertToCourierInList(c!));

            if (activeFilter != null)
                list = list.Where(c => c.IsActive == activeFilter.Value);

            if (sortBy != null)
            {
                list = sortBy switch
                {
                    BO.CourierListSortBy.Id => list.OrderBy(c => c.Id),
                    BO.CourierListSortBy.Name => list.OrderBy(c => c.Name),
                    BO.CourierListSortBy.DeliveriesCount => list.OrderBy(c => c.DeliveredCountOnTime + c.DeliveredCountLate),
                    BO.CourierListSortBy.ActiveStatus => list.OrderBy(c => c.IsActive),
                    _ => list
                };
            }

            return list;
        }

        // -------------------------------------------------------------
        // UPDATE COURIER
        // -------------------------------------------------------------
        internal static void UpdateCourier(int requesterId, BO.Courier courier)
        {
            var existing = dal.Courier.Read(courier.Id)
                ?? throw new BO.BlDoesNotExistException($"Courier {courier.Id} not found.");

            dal.Courier.Update(existing with
            {
                Name = courier.Name,
                Phone = courier.Phone,
                Email = courier.Email,
                IsActive = courier.IsActive,
                Transport = (DO.DeliveryTransport)courier.Transport,
                MaxDistance = courier.MaxRange
            });

            Observers.NotifyItemUpdated(courier.Id); //stage 5
            Observers.NotifyListUpdated(); //stage 5
        }

        // -------------------------------------------------------------
        // DELETE COURIER
        // -------------------------------------------------------------
        internal static void DeleteCourier(int requesterId, int courierId)
        {
            var deliveries = dal.Delivery.ReadAll(d => d!.CourierId == courierId);

            if (deliveries.Any())
                throw new InvalidOperationException("Courier cannot be deleted because he already handled deliveries.");

            dal.Courier.Delete(courierId);

            Observers.NotifyItemUpdated(courierId); //stage 5
            Observers.NotifyListUpdated(); //stage 5
        }

        // -------------------------------------------------------------
        // LOGIN
        // -------------------------------------------------------------
        internal static string LoginCourier(String username)
        {
            var courier = dal.Courier.ReadAll(c => c!.Email == username).FirstOrDefault()
                ?? throw new BO.BlDoesNotExistException("Courier not found.");

            return courier.Name;
        }

        // -------------------------------------------------------------
        // PERIODIC ACTIVITY CHECK (optional)
        // -------------------------------------------------------------
        internal static void PeriodicCourierUpdates()
        {
            var config = AdminManager.GetConfig();
            bool courierUpdated = false; //stage 5

            foreach (var courier in dal.Courier.ReadAll())
            {
                var deliveries = dal.Delivery.ReadAll(d => d!.CourierId == courier.Id);

                if (!deliveries.Any())
                {
                    courierUpdated = true;
                    dal.Courier.Update(courier with { IsActive = false });

                    Observers.NotifyItemUpdated(courier.Id); //stage 5
                    continue;
                }

                var lastEnd = deliveries
                    .Where(d => d.EndDelivery != null)
                    .OrderByDescending(d => d.EndDelivery)
                    .FirstOrDefault()?.EndDelivery;

                if (lastEnd != null &&
                    AdminManager.Now - lastEnd.Value > config.InactivityRange)
                {
                    courierUpdated = true;
                    dal.Courier.Update(courier with { IsActive = false });

                    Observers.NotifyItemUpdated(courier.Id); //stage 5
                }
            }

            if (courierUpdated)
                Observers.NotifyListUpdated(); //stage 5
        }

        internal static void GetCouriersList(int id)
        {
            var exists = dal.Courier.Read(id) != null;
            if (!exists) throw new BO.BlDoesNotExistException($"Courier {id} not found.");
            // or return something meaningful if originally intended
        }
    }
}

