using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace ecommerce.Auth.Configuration
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new ApiScope[]
            {
                new ApiScope("ecommerceapi", "Scope For Dash API", new string [] { "role", "Admin", "Customer", "Vendor"}),
                new ApiScope(IdentityServerConstants.LocalApi.ScopeName)
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                // OpenID Connect hybrid flow client (MVC)
                new Client
                {
                    ClientId = "mvcecommercedashboard",
                    ClientName = "Dashboard",
                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets =
                    {
                        new Secret("1EolkPONWaAa0pz2VI9QPJpAhnHM1jzV".Sha256())
                    },
                    //We can use here IsDebuger.
                   
                    RedirectUris           =  ClientUrl("mvcecommercedashboard", "RedirectUris"),
                    PostLogoutRedirectUris =  ClientUrl("mvcecommercedashboard", "PostLogoutRedirectUris"),
                    LogoUri = "~/icon.png",
                    RequireConsent = false,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "ecommerceapi",
                        IdentityServerConstants.LocalApi.ScopeName
                    },
                    AllowOfflineAccess = true,
                    AlwaysSendClientClaims = true,
                    AlwaysIncludeUserClaimsInIdToken = true
                },
                 new Client
                {
                    ClientId = "mvcecommercefront",
                    ClientName = "Website",
                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets =
                    {
                        new Secret("1EolkPONWaAa0pz2VI9QPJpAhnHM1jzV".Sha256())
                    },
                    //We can use here IsDebuger.
                    RedirectUris           =  ClientUrl("mvcecommercefront", "RedirectUris"),
                    PostLogoutRedirectUris =  ClientUrl("mvcecommercefront", "PostLogoutRedirectUris"),
                    LogoUri = "~/icon.png",
                    RequireConsent = false,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "ecommerceapi",
                        IdentityServerConstants.LocalApi.ScopeName
                    },
                    AllowOfflineAccess = true,
                    AlwaysSendClientClaims = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    //7200 is 2 hrs
                    AccessTokenLifetime = 7200,
                }, new Client
                {
                    ClientId = "servertoserver",
                    ClientName = "Server to Server auth",
                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("1EolkPONWaAa0pz2VI9QPJpAhnHM1jzV".Sha256())
                    },
                    // scopes that client has access to
                    AllowedScopes = { "ecommerceapi" }
                },
                new Client
                {
                    ClientId = "mobilecommerce",
                    ClientName = "Mobile Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequireClientSecret = false,

                    RedirectUris =           { "http://localhost:19006" },
                    AllowedCorsOrigins =     { "http://localhost:19006" },
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true,
                    RequirePkce = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "ecommerceapi"
                    }
                }
            };
        }

        private static ICollection<string> ClientUrl(string cleintId, string uris)
        {
            ICollection<string> iUri = new List<string>();
            if (Debugger.IsAttached)
            {
                //DashClient
                if (cleintId.Equals("mvcecommercedashboard"))
                {
                    if (uris.Equals("RedirectUris"))
                    {
                        iUri.Add("https://localhost:44377/signin-oidc");
                    }
                    if (uris.Equals("PostLogoutRedirectUris"))
                    {
                        iUri.Add("https://localhost:44377/signout-callback-oidc");
                    }
                }
                //Front Client
                if (cleintId.Equals("mvcecommercefront"))
                {
                    if (uris.Equals("RedirectUris"))
                    {
                        iUri.Add("https://localhost:44322/signin-oidc");
                    }
                    if (uris.Equals("PostLogoutRedirectUris"))
                    {
                        iUri.Add("https://localhost:44322/signout-callback-oidc");
                    }
                }
            }
            else
            {
                //Live config
                if (cleintId.Equals("mvcecommercedashboard"))
                {
                    if (uris.Equals("RedirectUris"))
                    {
                        iUri.Add("https://dash.Example.com/signin-oidc");
                        iUri.Add("https://www.dash.Example.com/signin-oidc");
                    }
                    if (uris.Equals("PostLogoutRedirectUris"))
                    {
                        iUri.Add("https://dash.Example.com/signout-callback-oidc");
                        iUri.Add("https://www.dash.Example.com/signout-callback-oidc");
                    }
                }
                //Front Client
                if (cleintId.Equals("mvcecommercefront"))
                {
                    if (uris.Equals("RedirectUris"))
                    {
                        iUri.Add("https://Example.com/signin-oidc");
                        iUri.Add("https://www.Example.com/signin-oidc");
                    }
                    if (uris.Equals("PostLogoutRedirectUris"))
                    {
                        iUri.Add("https://Example.com/signout-callback-oidc");
                        iUri.Add("https://www.Example.com/signout-callback-oidc");
                    }
                }
            }
            return iUri;
        }
    }

}