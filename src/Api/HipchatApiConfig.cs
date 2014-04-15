using System;
using ServiceStack.Configuration;

namespace HipchatApiV2
{
    public static class HipchatApiConfig
    {
        static HipchatApiConfig()
        {
            AuthToken = ConfigUtils.GetAppSetting("hipchat_auth_token", "");
        }

        public static string AuthToken { get; set; }
    }
}