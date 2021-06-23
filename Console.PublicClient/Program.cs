using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Threading.Tasks;

namespace Console.PublicClient
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

            System.Console.ReadKey();
        }

        private static AzureActiveDirectoryOptions ReadAzureActiveDirectoryConfiguration()
        {
            IConfigurationRoot configurationRoot = File.Exists("appsettings.development.json")
                ? new ConfigurationBuilder().AddJsonFile("appsettings.development.json").Build()
                : new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            AzureActiveDirectoryOptions options = configurationRoot.GetSection("AzureAd").Get<AzureActiveDirectoryOptions>();

            return options;
        }

        private static async Task<AuthenticationResult> AuthenticateApplicationAsync(AzureActiveDirectoryOptions options)
        {
            IPublicClientApplication application =
                PublicClientApplicationBuilder.Create(options.ClientId)
                                              .WithB2CAuthority(options.Authority)
                                              .Build();

            string[] scopes =
            {
                options.Scope
            };

            AuthenticationResult authenticationResult = await GetAzureActiveDirectoryTokenByCredentialsAsync(application, scopes);

            return authenticationResult;
        }

        private static async Task<AuthenticationResult> GetAzureActiveDirectoryTokenByCredentialsAsync(IPublicClientApplication application, string[] scopes)
        {
            SecureString password = new();
            AuthenticationResult authenticationResult = null;
            List<IAccount> accounts = await application.GetAccountsAsync() as List<IAccount>;

            if (accounts.Any())
            {
                try
                {
                    authenticationResult = await application.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).ExecuteAsync();
                }
                catch (Exception ex)
                {
                    // log exception
                }
            }

            if (authenticationResult is null)
            {
                authenticationResult = await application.AcquireTokenByUsernamePassword(scopes, "username", password).ExecuteAsync();
            }

            return authenticationResult;
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
    }
}
