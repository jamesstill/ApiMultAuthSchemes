using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace WidgetApi.BasicAuthenticationHandler
{
    public static class BasicAuthenticationExtensions
    {
        public static AuthenticationBuilder AddBasicAuthentication<T>(this AuthenticationBuilder builder)
            where T : class, IBasicAuthenticationService
        {
            return AddBasicAuthentication<T>(builder, BasicAuthenticationDefaults.AuthenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddBasicAuthentication<T>(this AuthenticationBuilder builder, string authenticationScheme)
            where T : class, IBasicAuthenticationService
        {
            return AddBasicAuthentication<T>(builder, authenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddBasicAuthentication<T>(this AuthenticationBuilder builder, Action<BasicAuthenticationOptions> configureOptions)
            where T : class, IBasicAuthenticationService
        {
            return AddBasicAuthentication<T>(builder, BasicAuthenticationDefaults.AuthenticationScheme, configureOptions);
        }

        public static AuthenticationBuilder AddBasicAuthentication<T>(this AuthenticationBuilder builder, string authenticationScheme, Action<BasicAuthenticationOptions> configureOptions)
            where T : class, IBasicAuthenticationService
        {
            builder.Services.AddTransient<IBasicAuthenticationService, T>();

            return builder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(
                authenticationScheme, configureOptions);
        }
    }
}
