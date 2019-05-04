using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel;

namespace TestIdentityServer4Client
{
    class Program
    {
        static void Main(string[] args)
        {

            // discover endpoints from metadata
            var client = new HttpClient();

            var disco = client.GetDiscoveryDocumentAsync("https://localhost:5123").Result;
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }
            Console.WriteLine("Endpoint found!");

            // request token
            var tokenResponse = client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "secret",

                UserName = "bob",
                Password = "password",
                Scope = $"api1.full_access openid profile custom.profile"
            }).Result;

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.AccessToken);
            Console.WriteLine("\n\n");

            //Get User Infomation with access token 
            Console.WriteLine(disco.UserInfoEndpoint);
            var userinfoClient = new HttpClient();
            var userinfo = userinfoClient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = disco.UserInfoEndpoint,
                Token = tokenResponse.AccessToken
            }).Result;

            if (userinfo.IsError)
            {
                Console.WriteLine(userinfo.Error);
                return;
            }

            Console.WriteLine("UserInfo retrieved");
            Console.WriteLine(userinfo.Raw);



            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var response = apiClient.GetAsync("https://localhost:5003/api/Values/").Result;
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(content);
                //Console.WriteLine(JArray.Parse(content));
            }
        }
    }
}
