namespace Helpers;

using System.Collections.Concurrent;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public static class Tools
{
    /// <summary>
    /// Returns the SHA-256 hex digest of the given password.
    /// Use this when storing or comparing passwords — never store plaintext.
    /// </summary>
    public static string HashPassword(string password)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public static string ToStringProperty<T>(this T t)
    {
        var props = typeof(T).GetProperties();
        string result = $"{typeof(T).Name} Details:\n";
        foreach (var prop in props)
        {
            var value = prop.GetValue(t, null) ?? "null";
            result += $"{prop.Name}: {value}\n";
        }
        return result;
    }

    internal enum TravelMode
    {
        Driving,
        Walking
    }

    private readonly record struct DistanceCacheKey(TravelMode Mode, double LatSrc, double LonSrc, double LatDest, double LonDest);

    private static readonly ConcurrentDictionary<DistanceCacheKey, Task<double?>> s_distanceCache = new();

    internal static Task<double?> CalcNetworkDistanceKmAsync(
        double? latitudeSrc,
        double? longitudeSrc,
        double? latitudeDest,
        double? longitudeDest,
        TravelMode mode,
        CancellationToken cancellationToken = default)
    {
        if (latitudeSrc is null || longitudeSrc is null || latitudeDest is null || longitudeDest is null)
            return Task.FromResult<double?>(null);

        var key = new DistanceCacheKey(mode, latitudeSrc.Value, longitudeSrc.Value, latitudeDest.Value, longitudeDest.Value);

        return s_distanceCache.GetOrAdd(key, k => CalculateOsrmDistanceKmAsync(k, cancellationToken));
    }

    private static async Task<double?> CalculateOsrmDistanceKmAsync(DistanceCacheKey key, CancellationToken cancellationToken)
    {
        string profile = key.Mode switch
        {
            TravelMode.Driving => "driving",
            TravelMode.Walking => "walking",
            _ => "driving"
        };

        string requestUrl = $"https://router.project-osrm.org/route/v1/{profile}/{key.LonSrc},{key.LatSrc};{key.LonDest},{key.LatDest}?overview=false";

        using HttpClient client = new();
        using HttpResponseMessage response = await client.GetAsync(requestUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        // OSRM structure: { routes: [ { distance: <meters>, ... } ], ... }
        if (!doc.RootElement.TryGetProperty("routes", out var routes) || routes.GetArrayLength() == 0)
            return null;

        var first = routes[0];
        if (!first.TryGetProperty("distance", out var distanceMetersElem))
            return null;

        double meters = distanceMetersElem.GetDouble();
        if (meters < 0)
            return null;

        return meters / 1000.0;
    }
}
