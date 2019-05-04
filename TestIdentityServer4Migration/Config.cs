using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Test;
using IdentityServer4;
using System.Security.Claims;
using IdentityModel;


namespace TestIdentityServer4AuthorizationServer
{
     public static class Config
    {
        public static List<TestUser> GetUsers()
        {
            var alice = new TestUser
                {
                    SubjectId = "1",
                    Username = "alice",
                    Password = "password"

                };
                alice.Claims.Add(new Claim(JwtClaimTypes.Name,"Alice"));
                alice.Claims.Add(new Claim(JwtClaimTypes.Email,"alice@google.com")); 
                alice.Claims.Add(new Claim("Class","4B"));

            var bob = new TestUser
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "password"

                };
                bob.Claims.Add(new Claim(JwtClaimTypes.Name,"Bob"));
                bob.Claims.Add(new Claim(JwtClaimTypes.Email,"bob@google.com")); 
                bob.Claims.Add(new Claim("Class","7D"));   

            return new List<TestUser>
            {
                alice,bob
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            var customProfile = new IdentityResource(
            name: "custom.profile",
            displayName: "Custom profile",
            claimTypes: new[] { JwtClaimTypes.Name
                        ,JwtClaimTypes.Email,"Class"});

            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                customProfile
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource{
                    Name = "api1",

                    Scopes =
                    {
                        new Scope()
                        {
                            Name = "api1.full_access",
                            DisplayName = "Full access to API 2",
                            UserClaims = {
                                JwtClaimTypes.Name
                               ,JwtClaimTypes.Email
                               ,"Class"
                            
                            }
                            
                        },
                        new Scope
                        {
                            Name = "api1.read_only",
                            DisplayName = "Read only access to API 2"
                        }
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { 
                        "api1.full_access"
                        ,IdentityServerConstants.StandardScopes.OpenId
                        ,IdentityServerConstants.StandardScopes.Profile 
                        ,"custom.profile"
                    }
                },
                // resource owner password grant client
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    AllowedScopes = { 
                        "api1.read_only"
                        ,IdentityServerConstants.StandardScopes.OpenId
                        ,IdentityServerConstants.StandardScopes.Profile 
                    }
                }
            };
        }
    }

}