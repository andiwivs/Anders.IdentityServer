using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Claim = System.Security.Claims.Claim;

[assembly: OwinStartup(typeof(Anders.WebFormSite.Startup))]

namespace Anders.WebFormSite
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        private void ConfigureAuth(IAppBuilder app)
        {

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = "Cookies",
                ExpireTimeSpan = TimeSpan.FromMinutes(10),
                SlidingExpiration = true
            });

            JwtSecurityTokenHandler.InboundClaimTypeMap.Clear();

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                AuthenticationType = "oidc",
                SignInAsAuthenticationType = "Cookies",

                Authority = "http://localhost:5000",

                ClientId = "webformsite",

                ResponseType = "id_token token",
                Scope = "openid profile email",

                RedirectUri = "http://localhost:5004/",
                PostLogoutRedirectUri = "http://localhost:5004/",

                UseTokenLifetime = false,

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = async n =>
                    {
                        var claims_to_exclude = new[]
                        {
                            "aud", "iss", "nbf", "exp", "nonce", "iat", "at_hash"
                        };

                        var claims_to_keep =
                            n.AuthenticationTicket.Identity.Claims
                            .Where(x => false == claims_to_exclude.Contains(x.Type)).ToList();
                        claims_to_keep.Add(new Claim("id_token", n.ProtocolMessage.IdToken));

                        if (n.ProtocolMessage.AccessToken != null)
                        {
                            claims_to_keep.Add(new Claim("access_token", n.ProtocolMessage.AccessToken));

                            var userInfoClient = new UserInfoClient("http://localhost:5000/connect/userinfo"); //, n.ProtocolMessage.AccessToken);
                            var userInfoResponse = await userInfoClient.GetAsync(n.ProtocolMessage.AccessToken);
                            var userInfoClaims = userInfoResponse.Claims
                                .Where(x => x.Type != "sub") // filter sub since we're already getting it from id_token
                                .Select(x => new Claim(x.Type, x.Value));
                            claims_to_keep.AddRange(userInfoClaims);
                        }

                        // Eg: inject app-specific claims here, or roles (use sparingly as this goes into cookie!)
                        claims_to_keep.Add(new Claim("thomas_agent_id", "12345"));
                        claims_to_keep.Add(new Claim("role", "Developer"));
                        claims_to_keep.Add(new Claim("role", "Admin"));

                        var ci = new ClaimsIdentity(
                            n.AuthenticationTicket.Identity.AuthenticationType,
                            "name", "role");
                        ci.AddClaims(claims_to_keep);

                        n.AuthenticationTicket = new Microsoft.Owin.Security.AuthenticationTicket(
                            ci, n.AuthenticationTicket.Properties
                        );
                    },
                    RedirectToIdentityProvider = n =>
                    {
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                        {
                            var id_token = n.OwinContext.Authentication.User.FindFirst("id_token")?.Value;
                            n.ProtocolMessage.IdTokenHint = id_token;
                        }

                        return Task.FromResult(0);
                    }
                }
            });

            // use this for IIS hosting to denote where our OWIN middleware should execute as part of the underlying asp.net pipeline...
            app.UseStageMarker(PipelineStage.Authenticate);
        }
    }
}
