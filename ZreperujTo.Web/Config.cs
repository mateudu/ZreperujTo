using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Hosting;

namespace ZreperujTo.Web
{
    public class Config
    {
        //public const string App_URL = @"http://localhost:5000";
        public const string App_URL = @"https://zreperujto.azurewebsites.net";
        public static IEnumerable<Scope> GetScopes()
        {
            return new List<Scope>
            {
                StandardScopes.OpenId,
                StandardScopes.Profile,
                StandardScopes.OfflineAccess,

                new Scope
                {
                    Name = "zreperuj_to_api",
                    DisplayName = "Zreperuj.To API access",
                    Description = "Zreperuj.To API"
                },
                new Scope
                {
                    Name = "api1",
                    DisplayName = "API1 access",
                    Description = "My API"
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                //new Client
                //{
                //    ClientId = "client",
                //    AllowedGrantTypes = GrantTypes.ClientCredentials,

                //    ClientSecrets = new List<Secret>
                //    {
                //        new Secret("secret".Sha256())
                //    },
                //    AllowedScopes = new List<string>
                //    {
                //        "api1"
                //    }
                //},

                // resource owner password grant client
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = new List<string>
                    {
                        "zreperuj_to_api",
                        "api"
                    }
                },

                // OpenID Connect hybrid flow and client credentials client (MVC)
                new Client
                {
                    ClientId = "mobile",
                    ClientName = "Klient mobilny Zreperuj.To",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    AllowAccessTokensViaBrowser = true,

                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris = new List<string>
                    {
                        "http://localhost:5002/signin-oidc",
                        "http://localhost:5000",
                        App_URL,
                        App_URL + @"/api/Profile",
                        "https://win10-zreperujto-oidc/redirect"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://localhost:5002",
                        App_URL
                    },

                    AllowedScopes = new List<string>
                    {
                        StandardScopes.OpenId.Name,
                        StandardScopes.Profile.Name,
                        StandardScopes.OfflineAccess.Name,
                        "api1",
                        "zreperuj_to_api"
                    }
                }
            };
        }
    }
}
