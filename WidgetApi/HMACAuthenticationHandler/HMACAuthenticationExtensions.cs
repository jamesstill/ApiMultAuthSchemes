using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace WidgetApi.HMACAuthenticationHandler
{
    public static class HMACAuthenticationExtensions
    {
        public static AuthenticationBuilder AddHMACAuthentication<T>(this AuthenticationBuilder builder)
            where T : class, IHMACAuthenticationService
        {
            return AddHMACAuthentication<T>(builder, HMACAuthenticationDefaults.AuthenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddBasicAuthentication<T>(this AuthenticationBuilder builder, string authenticationScheme)
            where T : class, IHMACAuthenticationService
        {
            return AddHMACAuthentication<T>(builder, authenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddHMACAuthentication<T>(this AuthenticationBuilder builder, Action<HMACAuthenticationOptions> configureOptions)
            where T : class, IHMACAuthenticationService
        {
            return AddHMACAuthentication<T>(builder, HMACAuthenticationDefaults.AuthenticationScheme, configureOptions);
        }

        public static AuthenticationBuilder AddHMACAuthentication<T>(this AuthenticationBuilder builder, string authenticationScheme, Action<HMACAuthenticationOptions> configureOptions)
            where T : class, IHMACAuthenticationService
        {
            builder.Services.AddTransient<IHMACAuthenticationService, T>();

            return builder.AddScheme<HMACAuthenticationOptions, HMACAuthenticationHandler>(
                authenticationScheme, configureOptions);
        }
    }
}
