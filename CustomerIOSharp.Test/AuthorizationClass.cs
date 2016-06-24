namespace CustomerIOSharp.Test
{
    using System;

    public abstract class AuthorizationClass
    {
        protected AuthorizationClass()
        {
            if (string.IsNullOrWhiteSpace(SiteId))
            {
                throw new Exception("SiteId is missing from environment variables.");
            }

            if (string.IsNullOrWhiteSpace(ApiKey))
            {
                throw new Exception("ApiKey is missing from environment variables.");
            }
        }

        public static string SiteId => Environment.GetEnvironmentVariable("CustomerIOSharp_SiteId") ?? "";
        public static string ApiKey => Environment.GetEnvironmentVariable("CustomerIOSharp_ApiKey") ?? "";
    }
}