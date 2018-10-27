using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace WidgetApi.HMACAuthenticationHandler
{
    public class HMACAuthenticationHandler : AuthenticationHandler<HMACAuthenticationOptions>
    {
        private const string AuthenticationHeaderName = "Authentication";
        private const string TimestampHeaderName = "Timestamp";
        private readonly IHMACAuthenticationService _authenticationService;

        public HMACAuthenticationHandler(
            IOptionsMonitor<HMACAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IHMACAuthenticationService authenticationService)
            : base(options, logger, encoder, clock)
        {
            _authenticationService = authenticationService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(AuthenticationHeaderName))
            {
                return AuthenticateResult.NoResult();
            }

            if (!Request.Headers.ContainsKey(TimestampHeaderName))
            {
                return AuthenticateResult.NoResult();
            }

            // get the tokens from the request header
            var timestampValue = Request.Headers["Timestamp"];
            var authenticationHashValue = Request.Headers["Authentication"];
            
            if (!GetHashTokens(authenticationHashValue, out string username, out string hash))
            {
                return AuthenticateResult.NoResult();
            }

            bool isValidUser = await _authenticationService.IsValidUserAsync(Options, timestampValue, username, hash);

            if (!isValidUser)
            {
                return AuthenticateResult.Fail("Failed authentication");
            }

            var claims = new[] { new Claim(ClaimTypes.Name, username) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }

        private static bool GetHashTokens(string hash, out string username, out string hashString)
        {
            username = string.Empty;
            hashString = string.Empty;

            var tokens = hash.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length != 2)
            {
                return false;
            }

            username = tokens[0];
            hashString = tokens[1];
            return true;
        }
    }
}
