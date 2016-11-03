using HipchatApiV2.Enums;
using HipchatApiV2.Models;

namespace HipchatApiV2.Requests
{
    public class SendRoomNotificationRequest
    {
        public string Message { get; set; }
        public RoomColors Color { get; set; }
        public bool Notify { get; set; }
        public HipchatMessageFormat MessageFormat { get; set; }
        public Card Card { get; set; }
    }
}