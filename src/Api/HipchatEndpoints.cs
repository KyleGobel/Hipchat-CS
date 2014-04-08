using ServiceStack;

namespace HipchatApiV2
{
    public class HipchatEndpoints
    {
        private HipchatEndpoints() {}
        private const string CreateWebhookEndpointFormat = "https://api.hipchat.com/v2/room/{0}/webhook?auth_token={1}";
        private const string SendMessageEndpointFormat = "https://api.hipchat.com/v2/room/{0}/notification?auth_token={1}";
        private const string CreateRoomEndpointFormat = "https://api.hipchat.com/v2/room?auth_token={0}";
        public static readonly string GetAllRoomsEndpoint = "https://api.hipchat.com/v2/room";
        public static string CreateWebhookEndpoint(int roomId, string authToken)
        {
            return CreateWebhookEndpointFormat.Fmt(roomId,authToken);
        }

     

        public static string CreateRoomEndpoint(string authToken)
        {
            return CreateRoomEndpointFormat.Fmt(authToken);
        }

        public static string SendMessageEndpoint(string roomId, string authToken)
        {
            return SendMessageEndpointFormat.Fmt(roomId, authToken);
        }
    }
}