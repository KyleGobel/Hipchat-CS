using ServiceStack;
using System;
using System.Configuration;

namespace HipchatApiV2
{
    public class HipchatEndpoints
    {
        private static string EndpointHost;

        private const string HipChatApi = "api.hipchat.com";

        static HipchatEndpoints()
        {
            EndpointHost = ConfigurationManager.AppSettings["hipchat_endpoint_host"] ?? HipChatApi;
        }

        public static void SetEndpointHost(string host)
        {
            EndpointHost = host ?? HipChatApi;
        }

        private HipchatEndpoints() {}
        public static string CreateWebhookEndpointFormat { get { return String.Format("https://{0}/v2/room/{{0}}/webhook", EndpointHost); } }
        private string SendMessageEndpointFormat { get { return String.Format("https://{0}/v2/room/{{0}}/notification?auth_token={{1}}", EndpointHost); } }
        public static string CreateRoomEndpoint { get { return String.Format("https://{0}/v2/room", EndpointHost); } }
        public static string GetAllRoomsEndpoint { get { return String.Format("https://{0}/v2/room", EndpointHost); } }
        public static string ViewRoomHistoryEndpoint { get { return String.Format("https://{0}/v2/room/{{0}}/history", EndpointHost); } }
        public static string ViewRecentRoomHistoryEndpoint { get { return String.Format("https://{0}/v2/room/{{0}}/history/latest", EndpointHost); } }
        public static string GenerateTokenEndpoint { get { return String.Format("https://{0}/v2/oauth/token", EndpointHost); } }
        public static string SendNotificationEndpointFormat { get { return String.Format("https://{0}/v2/room/{{0}}/notification", EndpointHost); } }
        public static string ShareFileWithRoomEndpointFormat { get { return String.Format("https://{0}/v2/room/{{0}}/share/file", EndpointHost); } }
        public static string GetRoomEndpointFormat { get { return String.Format("https://{0}/v2/room/{{0}}", EndpointHost); } }
        public static string DeleteRoomEndpointFormat { get { return String.Format("https://{0}/v2/room/{{0}}", EndpointHost); } }
        public static string GetAllWebhooksEndpointFormat { get { return String.Format("https://{0}/v2/room/{{0}}/webhook", EndpointHost); } }
        public static string DeleteWebhookEndpointFormat { get { return String.Format("https://{0}/v2/room/{{0}}/webhook/{{1}}", EndpointHost); } }
        public static string UpdateRoomEndpointFormat { get { return String.Format("https://{0}/v2/room/{{0}}", EndpointHost); } }
        public static string GetAllUsersEndpoint { get { return String.Format("https://{0}/v2/user", EndpointHost); } }
        public static string GetUserInfoEndpoint { get { return String.Format("https://{0}/v2/user/{{0}}", EndpointHost); } }
        public static string SetTopicEnpdointFormat { get { return String.Format("https://{0}/v2/room/{{0}}/topic", EndpointHost); } }
        public static string GetAllEmoticonsEndpoint { get { return String.Format("https://{0}/v2/emoticon", EndpointHost); } }
        public static string GetEmoticonEndpoint { get { return String.Format("https://{0}/v2/emoticon/{{0}}", EndpointHost); } }
        public static string CreateUserEndpointFormat { get { return String.Format("https://{0}/v2/user", EndpointHost); } }
        public static string UpdateUserEndpointFormat { get { return String.Format("https://{0}/v2/user/{{0}}", EndpointHost); } }
        public static string DeleteUserEndpointFormat { get { return String.Format("https://{0}/v2/user/{{0}}", EndpointHost); } }
        public static string PrivateMessageUserEnpointFormat {get { return string.Format("https://{0}/v2/user/{{0}}/message", EndpointHost); }}
        public static string AddMemberEnpdointFormat { get { return String.Format("https://{0}/v2/room/{{0}}/member/{{1}}", EndpointHost); } }
        public static string UpdatePhotoEnpdointFormat { get { return String.Format("https://{0}/v2/user/{{0}}/photo", EndpointHost); } }
    }
}
