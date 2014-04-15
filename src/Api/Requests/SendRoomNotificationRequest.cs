using HipchatApiV2.Enums;

namespace HipchatApiV2.Requests
{
    public class SendRoomNotificationRequest
    {
        public string Message { get; set; }
        public RoomColors Color { get; set; }
        public bool Notify { get; set; }
        public HipchatMessageFormat MessageFormat { get; set; }
    }
}