using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO;

/// <summary>
/// Represents a courier responsible for delivering orders.
/// </summary>
/// <param name="Id">Unique identifier for the courier.</param>
/// <param name="Name">Full name of the courier.</param>
/// <param name="Phone">Primary contact phone number.</param>
/// <param name="Email">Contact email address.</param>
/// <param name="Password">
/// Authentication secret for the courier. Store securely (hashed) in production — do not keep plain-text passwords.
/// </param>
/// <param name="IsActive">True when the courier is currently active/available for assignments.</param>
/// <param name="Transport">Primary delivery transport used by the courier (see <see cref="DeliveryTransport"/>).</param>
/// <param name="MaxDistance">
/// Optional maximum delivery distance in kilometers. A null value indicates no explicit distance limit.
/// </param>
public record Courier
(
    int Id,
    string Name,
    string Phone,
    string Email,
    string Password,
    bool IsActive,
    DeliveryTransport Transport,
    DateTime StartWorkingDate,
    double? MaxDistance = null
)
{
    public readonly string? Address;

    /// <summary>
    /// Parameterless constructor that initializes a courier with safe defaults.
    /// </summary>
    /// <remarks>
    /// Default values:
    /// - Id = 0
    /// - Name, Phone, Email, Password = empty string
    /// - IsActive = false
    /// - Transport = <see cref="DeliveryTransport.Car"/>
    /// - MaxDistance = null
    /// </remarks>
    public Courier() : this(0, "", "", "", "", false, DeliveryTransport.Car, default, null) { }
}