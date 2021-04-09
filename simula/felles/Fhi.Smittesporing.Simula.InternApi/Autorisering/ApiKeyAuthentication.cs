using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Optional;
using ISystemClock = Microsoft.AspNetCore.Authentication.ISystemClock;

namespace Fhi.Smittesporing.Simula.InternApi.Autorisering
{
    public class ApiKeyAuthentication
    {
        public const string Scheme = "Bearer";
        public const string AuthorizationHeader = "Authorization";
        public const string AuthenticatedApiClientName = "AuthenticatedApiClient";

        public class AuthenticationHandler : AuthenticationHandler<ApiKeyOptions>
        {
            private readonly ApiKeyOptions _apiKeyOptions;

            public AuthenticationHandler(IOptionsMonitor<ApiKeyOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
            {
                _apiKeyOptions = options.Get(ApiKeyAuthentication.Scheme);
            }

            protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                var apiKeyAuthResult = ExtractApiKey()
                    .Map(apiKey => apiKey == _apiKeyOptions.ApiKey
                        ? SuccessResult()
                        : AuthenticateResult.Fail("Invalid API-key"));

                var authResult = apiKeyAuthResult.ValueOr(AuthenticateResult.NoResult);

                return Task.FromResult(authResult);
            }

            private AuthenticateResult SuccessResult()
            {
                var claims = new[] { new Claim(ClaimTypes.Name, "AuthenticatedApiClient") };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }

            private Option<string> ExtractApiKey()
            {
                return ExtractAuthHeaderValue()
                    .FlatMap(headerValue => headerValue.Scheme == ApiKeyAuthentication.Scheme
                        ? headerValue.Parameter.Some()
                        : Option.None<string>());
            }

            private Option<AuthenticationHeaderValue> ExtractAuthHeaderValue()
            {
                var authHeader = Request.Headers.ContainsKey(AuthorizationHeader)
                    ? Option.Some(Request.Headers[AuthorizationHeader])
                    : Option.None<StringValues>();
                return authHeader.FlatMap(header => AuthenticationHeaderValue.TryParse(header, out var headerValue)
                    ? headerValue.Some()
                    : Option.None<AuthenticationHeaderValue>());
            }
        }

        public class ApiKeyOptions : AuthenticationSchemeOptions
        {
            public string ApiKey { get; set; }
        }
    }

    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddApiKeyAuth(this IServiceCollection services, string apiKey)
        {
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy(Tilgangsregler.Simula, p => p
                    .AddAuthenticationSchemes(ApiKeyAuthentication.Scheme)
                    .RequireClaim(ClaimTypes.Name, ApiKeyAuthentication.AuthenticatedApiClientName));
            });

            services
                .AddAuthentication(ApiKeyAuthentication.Scheme)
                .AddScheme<ApiKeyAuthentication.ApiKeyOptions, ApiKeyAuthentication.AuthenticationHandler>(ApiKeyAuthentication.Scheme,
                    cfg => cfg.ApiKey = apiKey);

            services.AddSingleton<IAuthenticationHandler, ApiKeyAuthentication.AuthenticationHandler>();

            return services;
        }
    }
}