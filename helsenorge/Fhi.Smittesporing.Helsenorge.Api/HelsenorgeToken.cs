using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;

namespace Fhi.Smittesporing.Helsenorge.Api
{
    public class HelsenorgeToken
    {
        public string SubjektPersonIdent { get; set; }
        public string AktorSubjektPersonIdent { get; set; }
        public string AktorSubjectName { get; set; }
        public string Telefonnummer { get; set; }

        public HelsenorgeToken(string subjektPersonIdent, string aktorSubjektPersonIdent, string telefon)
        {
            SubjektPersonIdent = subjektPersonIdent;
            AktorSubjektPersonIdent = aktorSubjektPersonIdent;
            Telefonnummer = telefon;
        }

        public static HelsenorgeToken OpprettFraRequest(HttpRequest request, bool validateToken, string thumbprint, ILogger log)
        {
            if (!request.Headers.ContainsKey(HeaderNames.Authorization))
            {
                return null;
            };

            var authorization = AuthenticationHeaderValue.Parse(request.Headers[HeaderNames.Authorization]);

            if (authorization.Scheme != "Bearer" || string.IsNullOrEmpty(authorization.Parameter))
            {
                log.LogError($"Feil i Authorization headeren: {authorization}");
                throw new NotAuthorizedException();
            }
            log.LogDebug($"JWT: {authorization.Parameter}");
            if (validateToken)
            {
                ValidateToken(authorization.Parameter, thumbprint, log);
            }
            string subjektPersonIdent;
            string aktorSubjektPersonIdent;
            string telefon;
            string givenName;
            string middleName;
            string familyName;
            string act_type;
            try
            {
                var securityToken = new JwtSecurityToken(authorization.Parameter);
                var payload = securityToken.Payload;
                subjektPersonIdent = payload["sub"] as string;
                aktorSubjektPersonIdent = payload["act_sub"] as string;
                telefon = payload["act_phone_number"] as string;
                givenName = payload["act_given_name"] as string;
                middleName = payload["act_middle_name"] as string;
                familyName = payload["act_family_name"] as string;
                act_type = payload["act_type"] as string;
            }
            catch (Exception ex)
            {
                log.LogError($"Lesing av token feilet: {ex}");
                throw new NotAuthorizedException();
            }
            
            var token = new HelsenorgeToken(subjektPersonIdent, aktorSubjektPersonIdent, telefon);

            var name = string.Join(" ", givenName, middleName, familyName).Trim();

            if (!string.IsNullOrWhiteSpace(name))
            {
                token.AktorSubjectName = name;
            }

            return token;
        }

        private static void ValidateToken(string tokenString, string thumbprint, ILogger log)
        {
            var validationParameters = new TokenValidationParameters
            {
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                IssuerSigningKey = GetTokenSigningKey(thumbprint),
                ValidateActor = true,
                ValidateAudience = false,
                ValidAudience = "fhi",
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true
            };

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(tokenString, validationParameters, out SecurityToken validatedToken);
            }
            catch (Exception ex)
            {
                log.LogError($"Kunne ikke autorisere request med JWT '{tokenString}': {ex}");
                throw new NotAuthorizedException();
            }
        }

        public static SecurityKey GetTokenSigningKey(string thumbprint)
        {
            return new X509SecurityKey(GetCertificate(thumbprint));
        }

        public static X509Certificate2 GetCertificate(string thumbprint)
        {
            using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: false);
                if (certificates.Count == 0)
                {
                    throw new Exception($"Finner ikke certificate med thumbprint '{thumbprint}'");
                }
                store.Close();
                return certificates[0];
            }
        }
    }

    public class NotAuthorizedException : Exception
    {
    }
}
