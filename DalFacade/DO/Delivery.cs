using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DO
{
    /// <summary>
    /// Represents a delivery operation between a sender and a receiver.
    /// Each delivery has a unique running ID, assigned automatically from the Config class.
    /// </summary>
    /// <param name="Id">Unique running ID for each delivery (auto-generated, from Config)</param>
    /// <param name="OrderedId">ID of the order associated with this delivery</param>
    /// <param name="CourierId">ID of the courier assigned for this delivery</param>
    /// <param name="DeliveryType">Type of delivery (enum: Drone, Motorbike, Truck)</param>
    /// <param name="StartDelivery">Date and time when the delivery started</param>
    /// <param name="ActualDistance">Actual distance covered by the courier during delivery (in kilometers)</param> 
    /// <param name="EndDelivery">Date and time when the delivery ended (nullable if not finished yet)</param> 
    public record Delivery
    (
        int Id,                             // Auto-generated running ID (from Config)
        int OrderedId,                      // Linked order ID
        int CourierId,                      // Courier ID
        DeliveryTransport DeliveryType,     // Delivery type (enum)
        DateTime StartDelivery,             // Delivery start date and time
        double? ActualDistance = null,              // Actual distance covered during delivery
        DeliveryCompletionType? CompletionType = null, // Delivery completion type (enum)
        DateTime? EndDelivery = null        // Nullable - set only when delivery is completed
    )
    {
        /// <summary>
        /// Default constructor required for Stage 3.
        /// </summary>
        public Delivery() : this(0, 0, 0,  DeliveryTransport.Motorcycle ,DateTime.Now)
        {
            status = new object();
        }

        // Fix CS8862: Add 'this' initializer to call the primary constructor.
        // Fix CS8618: Initialize 'status' property to a non-null value.
        // Fix IDE0060: Remove unused parameters 'orderId', 'deliveryDate', 'v'.
        public Delivery(int id, int courierId)
            : this(id, 0, courierId, DeliveryTransport.Motorcycle, DateTime.Now)
        {
            status = new object();
        }

        public object status { get; set; } = new object();
    }
}

