using System;

namespace HipchatApiV2
{
    public static class HipchatApiConfig
    {
        static HipchatApiConfig()
        {
            AuthToken = System.Environment.GetEnvironmentVariable("hipchat_auth_token") ??  "";
        }

        public static string AuthToken { get; set; }
    }
}