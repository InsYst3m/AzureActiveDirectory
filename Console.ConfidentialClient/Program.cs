using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Console.ConfidentialClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            System.Console.WriteLine("Hello World!");

            AzureActiveDirectoryOptions options = ReadAzureActiveDirectoryConfiguration();

            AuthenticationResult authenticationResult = await AuthenticateApplicationAsync(options);

            string result = await CallApiAsync(authenticationResult);

            System.Console.WriteLine(result);
        }

        private static async Task<string> CallApiAsync(AuthenticationResult authenticationResult)
        {
            string result = string.Empty;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);

                HttpResponseMessage responseMessage = await client.GetAsync("https://localhost:");
                if (responseMessage.IsSuccessStatusCode)
                {
                    result = await responseMessage.Content.ReadAsStringAsync();
                }
            }

            return result;
        }

        private static async Task<AuthenticationResult> AuthenticateApplicationAsync(AzureActiveDirectoryOptions options)
        {
            IConfidentialClientApplication application =
                ConfidentialClientApplicationBuilder.Create(options.ClientId)
                                                    .WithClientSecret(options.ClientSecret)
                                                    .WithAuthority(options.Authority)
                                                    .Build();

            string[] scopes =
            {
                options.Scope
            };

            AuthenticationResult authenticationResult = null;

            try
            {
                authenticationResult = await application.AcquireTokenForClient(scopes).ExecuteAsync();
            }
            catch (Exception ex)
            {
                // log exception
            }

            return authenticationResult;
        }

        private static AzureActiveDirectoryOptions ReadAzureActiveDirectoryConfiguration()
        {
            IConfigurationRoot configurationRoot = File.Exists("appsettings.development.json")
                ? new ConfigurationBuilder().AddJsonFile("appsettings.development.json").Build()
                : new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            AzureActiveDirectoryOptions options = configurationRoot.GetSection("AzureAd").Get<AzureActiveDirectoryOptions>();

            return options;
        } 
    }
}
