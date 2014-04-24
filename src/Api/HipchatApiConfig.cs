using System;
using System.Configuration;

namespace HipchatApiV2
{
    public static class HipchatApiConfig
    {
        static HipchatApiConfig()
        {
            AuthToken = ConfigurationManager.AppSettings["hipchat_auth_token"] ??  "";
        }

        public static string AuthToken { get; set; }
    }
}