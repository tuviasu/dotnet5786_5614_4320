using DalApi;
using System.Linq;

namespace Helpers
{
    internal static class CourierManager
    {
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
                DeliveredCountOnTime = 0, // You may need to calculate this if data is available
                DeliveredCountLate = 0,   // You may need to calculate this if data is available
                ActiveOrderId = null,     // You may need to set this if data is available
               
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

                MaxRange = courier.MaxDistance, // <-- FIXED: use MaxDistance from DO.Courier
               
            };
        }

        // -------------------------------------------------------------
        // CREATE COURIER
        // -------------------------------------------------------------
        internal static void CreateCourier(int requesterId, BO.Courier courier)
        {
            var newCourier = new DO.Courier
            {
                Id = courier.Id,
                Name = courier.Name,
                Phone = courier.Phone,
                Email = courier.Email,
                Password = "1234", // If not using passwords  
                IsActive = courier.IsActive,
                Transport = (DO.DeliveryTransport)courier.Transport,



                StartWorkingDate = AdminManager.Now,
                MaxDistance = courier.MaxRange // <-- FIXED: use MaxDistance for DO.Courier
            };
            dal.Courier.Create(newCourier);
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

            // Filtering
            if (activeFilter != null)
                list = list.Where(c => c.IsActive == activeFilter.Value);

            // Sorting
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
                MaxDistance = courier.MaxRange // <-- FIXED: use MaxDistance for DO.Courier
            });
        }

        // -------------------------------------------------------------
        // DELETE COURIER
        // -------------------------------------------------------------
        internal static void DeleteCourier(int requesterId, int courierId)
        {
            var deliveries = dal.Delivery.ReadAll(d => d!.CourierId == courierId);

            if (deliveries.Any())
                throw new BO.BLIllegalActionException("Courier cannot be deleted because he already handled deliveries.");

            dal.Courier.Delete(courierId);
        }

        // -------------------------------------------------------------
        // LOGIN
        // -------------------------------------------------------------
        internal static string LoginCourier(String username)
        {
            var courier = dal.Courier.ReadAll(c => c!.Email == username).FirstOrDefault()
                ?? throw new BO.BlDoesNotExistException("Courier not found.");

            return courier.Name; // Or role if needed
        }

        // -------------------------------------------------------------
        // PERIODIC ACTIVITY CHECK (optional)
        // -------------------------------------------------------------
        internal static void PeriodicCourierUpdates()
        {
            var config = AdminManager.GetConfig();

            foreach (var courier in dal.Courier.ReadAll())
            {
                var deliveries = dal.Delivery.ReadAll(d => d!.CourierId == courier.Id);

                // no deliveries → not active
                if (!deliveries.Any())
                {
                    dal.Courier.Update(courier with { IsActive = false });
                    continue;
                }

                var lastEnd = deliveries
                    .Where(d => d.EndDelivery != null)
                    .OrderByDescending(d => d.EndDelivery)
                    .FirstOrDefault()?.EndDelivery;

                if (lastEnd != null &&
                    AdminManager.Now - lastEnd.Value > config.InactivityRange)
                {
                    dal.Courier.Update(courier with { IsActive = false });
                }
            }
        }
    }
}
