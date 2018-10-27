using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WidgetApi.HMACAuthenticationHandler
{
    public interface IHMACAuthenticationService
    {
        Task<bool> IsValidUserAsync(HMACAuthenticationOptions options, string timestampValue, string username, string hash);
    }

    public class HMACAuthenticationService : IHMACAuthenticationService
    {
        private readonly double _replayAttackDelayInSeconds = 15;

        public Task<bool> IsValidUserAsync(HMACAuthenticationOptions options, string timestampValue, string username, string hash)
        {
            if (!IsValidTimestamp(timestampValue, out DateTime timestamp))
            {
                return Task.FromResult(false);
            }

            if (!PassesThresholdCheck(timestamp))
            {
                return Task.FromResult(false);
            }

            if (!ComputeHash(options.Key, timestamp, hash))
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private static bool IsValidTimestamp(string timestampValue, out DateTime timestamp)
        {
            // Parse a string representing UTC. E.g.: "2013-01-12T16:11:20.0904778Z";
            // Client should create the timestamp like this: var timestampValue = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
            var ts = DateTime.TryParseExact(timestampValue, "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out timestamp);
            return ts;
        }

        private bool PassesThresholdCheck(DateTime timestamp)
        {
            // make sure call is made within the allowed threshold
            var ts = DateTime.UtcNow.Subtract(timestamp);
            return ts.TotalSeconds <= _replayAttackDelayInSeconds;
        }

        private static bool ComputeHash(string privateKey, DateTime timestamp, string authenticationHash)
        {
            string hashString;
            var ticks = timestamp.Ticks.ToString(CultureInfo.InvariantCulture);
            var key = Encoding.UTF8.GetBytes(privateKey.ToUpper());
            using (var hmac = new HMACSHA256(key))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(ticks));
                hashString = Convert.ToBase64String(hash);
            }

            return hashString.Equals(authenticationHash);
        }
    }
}
