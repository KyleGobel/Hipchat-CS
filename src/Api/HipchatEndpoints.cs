using ServiceStack;

namespace HipchatApiV2
{
    public class HipchatEndpoints
    {
        private HipchatEndpoints() {}
        public static readonly string CreateWebhookEndpointFormat = "https://api.hipchat.com/v2/room/{0}/webhook";
        private const string SendMessageEndpointFormat = "https://api.hipchat.com/v2/room/{0}/notification?auth_token={1}";
        public static readonly string CreateRoomEndpoint = "https://api.hipchat.com/v2/room";
        public static readonly string GetAllRoomsEndpoint = "https://api.hipchat.com/v2/room";
        public static readonly string GenerateTokenEndpoint = "https://api.hipchat.com/v2/oauth/token";
        public static readonly string SendNotificationEndpointFormat = "https://api.hipchat.com/v2/room/{0}/notification";
        public static readonly string GetRoomEndpointFormat = "https://api.hipchat.com/v2/room/{0}";
        public static readonly string DeleteRoomEndpointFormat = "https://api.hipchat.com/v2/room/{0}";
        public static readonly string GetAllWebhooksEndpointFormat = "https://api.hipchat.com/v2/room/{0}/webhook";
        public static readonly string DeleteWebhookEndpointFormat = "https://api.hipchat.com/v2/room/{0}/webhook/{1}";
        public static readonly string UpdateRoomEndpoingFormat = "https://api.hipchat.com/v2/room/{0}";
        public static readonly string GetAllUsersEndpoint = "https://api.hipchat.com/v2/user";
        public static readonly string SetTopicEnpdointFormat = "https://api.hipchat.com/v2/room/{0}/topic";
        public static readonly string GetAllEmoticonsEndpoint = "https://api.hipchat.com/v2/emoticon";
        public static readonly string GetEmoticonEndpoint = "https://api.hipchat.com/v2/emoticon/{0}";
        public static readonly string CreateUserEndpointFormat = "https://api.hipchat.com/v2/user";
        public static readonly string DeleteUserEndpointFormat = "https://api.hipchat.com/v2/user/{0}";
    }
}
