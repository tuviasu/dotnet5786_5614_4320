using System.Reflection;
using System.Text;
using BO;

namespace Helpers
{
    /// <summary>
    /// General-purpose logical helper functions used across BL layer.
    /// All methods here are internal and static and do NOT store any state.
    /// </summary>
    internal static class Tools
    {

        public static string ToStringProperty<T>(this T t)
        {
            if (t == null)
                return "null";

            StringBuilder sb = new StringBuilder();

            Type type = typeof(T);
            sb.Append($"{type.Name}:\n");

            // Loop through all public properties of the type
            foreach (PropertyInfo prop in type.GetProperties())
            {
                object? value = prop.GetValue(t);
                sb.Append($"  {prop.Name} = {value}\n");
            }

            return sb.ToString();
        }
        // ---------------------------------------------------------
        // 1. AIR DISTANCE (Haversine formula)
        // ---------------------------------------------------------

        /// <summary>
        /// Calculates the aerial (straight-line) distance in kilometers 
        /// between two points given by their latitude and longitude.
        /// Uses Haversine formula.
        /// </summary>
        public static double AirDistance(
            double lat1, double lon1,
            double lat2, double lon2)
        {
            const double R = 6371.0; // Radius of Earth in km

            double dLat = DegreesToRadians(lat2 - lat1);
            double dLon = DegreesToRadians(lon2 - lon1);

            lat1 = DegreesToRadians(lat1);
            lat2 = DegreesToRadians(lat2);

            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // result in kilometers
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        private static double DegreesToRadians(double degrees)
            => degrees * Math.PI / 180.0;


        // ---------------------------------------------------------
        // 2. GET COORDINATES FROM ADDRESS (dummy implementation)
        // ---------------------------------------------------------

        /// <summary>
        /// Converts a full address string into geographical coordinates.
        /// In the real world this would call an online geocoding API.
        /// Here we either simulate or validate address correctness.
        /// Throws exception if the address is invalid.
        /// </summary>
        public static (double Latitude, double Longitude) GetCoordinates(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new BO.BlInvalidAddressException("Address cannot be empty.");

            // This is a placeholder. 
            // In a real system this would call a geocoding API.
            // For your project we typically generate deterministic fake coordinates
            // based on string hash to keep consistency.

            int hash = address.GetHashCode();

            double lat = 31.0 + (hash % 1000) / 10000.0; // around Israel
            double lon = 35.0 + (hash % 1500) / 10000.0;

            return (lat, lon);
        }


        // ---------------------------------------------------------
        // 3. TRAVEL / WALKING DISTANCE
        // ---------------------------------------------------------

        /// <summary>
        /// Calculates travel distance based on a chosen type (walking or driving).
        /// Uses straight-line distance multiplied by a coefficient.
        /// </summary>
        public static double TravelDistance(
            double lat1, double lon1,
            double lat2, double lon2,
            BO.DistanceType type)
        {
            double air = AirDistance(lat1, lon1, lat2, lon2);

            // You can replace multipliers with real logic if needed.
            return type switch
            {
                BO.DistanceType.Driving => air * 1.25,  // typically roads are 25% longer
                BO.DistanceType.Walking => air * 1.10,  // walking paths slightly longer
                _ => air
            };
        }


        // ---------------------------------------------------------
        // 4. TIME CHECK UTILITIES
        // ---------------------------------------------------------

        /// <summary>
        /// Checks if time1 is earlier than time2, safely handling null values.
        /// </summary>
        public static bool EarlierThan(DateTime? t1, DateTime? t2)
        {
            if (t1 == null || t2 == null)
                return false;

            return t1.Value < t2.Value;
        }

        /// <summary>
        /// Returns the difference between two DateTime values in minutes.
        /// Returns null if one of the arguments is null.
        /// </summary>
        public static double? MinutesBetween(DateTime? t1, DateTime? t2)
        {
            if (t1 == null || t2 == null)
                return null;

            return (t2.Value - t1.Value).TotalMinutes;
        }


        // ---------------------------------------------------------
        // 5. VALIDATION HELPERS
        // ---------------------------------------------------------

        /// <summary>
        /// Validates that ID (Teudat Zehut) is positive and form-correct.
        /// </summary>
        public static void ValidateId(int id)
        {
            if (id <= 0)
                throw new BO.BlInvalidDateException("ID must be a positive number.");
        }

        /// <summary>
        /// Validates that a string is not empty or null.
        /// </summary>
        public static void ValidateRequiredString(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BO.BlInvalidDateException($"{fieldName} cannot be empty.");
        }

        internal static object AirDistance(double latitude1, double longitude1, object latitude2, object longitude2)
        {
            throw new NotImplementedException();
        }
    }

}
