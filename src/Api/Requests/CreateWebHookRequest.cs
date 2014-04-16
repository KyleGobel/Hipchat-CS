using HipchatApiV2.Enums;

namespace HipchatApiV2.Requests
{
    public class CreateWebHookRequest
    {
        public string Url { get; set; }
        public string Pattern { get; set; }
        public RoomEvent Event { get; set; }
        public string Name { get; set; }
    }
}