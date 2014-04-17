using HipchatApiV2.Enums;

namespace HipchatApiV2.Responses
{
    public class HipchatGetAllWebhooksItems
    {
        public string Url { get; set; }
        public string Pattern { get; set; }
        public RoomEvent Event { get; set; }
        public HipchatLink Links { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
    }
}