using System;

namespace Console.ConfidentialClient
{
    public class AzureActiveDirectoryOptions
    {
        public string Instance { get; set; }
        public string Domain { get; set; }
        public string ClientId { get; set; }
        public string TenantId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public string Authority { get; set; }
    }
}
