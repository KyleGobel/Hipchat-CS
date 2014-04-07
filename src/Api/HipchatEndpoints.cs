using ServiceStack;

namespace HipchatApiV2
{
    public class HipchatEndpoints
    {
        private HipchatEndpoints() {}
        private const string CreateRoomEndpointFormat = "https://api.hipchat.com/v2/room/{0}/webhook?auth_token={1}";
        private const string SendMessageEndpointFormat = "https://api.hipchat.com/v2/room/{0}/notification?auth_token={1}";

        public static string CreateRoomEndpoint(int roomId, string authToken)
        {
            return CreateRoomEndpointFormat.Fmt(roomId,authToken);
        }

        public static string SendMessageEndpoint(int roomId, string authToken)
        {
            return SendMessageEndpointFormat.Fmt(roomId, authToken);
        }
    }
}