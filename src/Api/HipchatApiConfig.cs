using System;
using ServiceStack.Configuration;

namespace HipchatApiV2
{
    public sealed class HipchatApiConfig
    {
        #region Lazy Singleton Pattern
        private static readonly Lazy<HipchatApiConfig> LazyInstance = new Lazy<HipchatApiConfig>(() => new HipchatApiConfig()); 
        public static HipchatApiConfig Instance {
            get { return LazyInstance.Value; }
        }
        #endregion

        //private constructor makes sure this never gets instanciated
        private HipchatApiConfig()
        {
            //default values
            AuthToken = ConfigUtils.GetAppSetting("hipchat_auth_token", "");
        }

        public string AuthToken { get; set; }
    }
}