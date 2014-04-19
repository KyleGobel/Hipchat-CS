using HipchatApiV2.Enums;
using HipchatApiV2.Responses;

namespace HipchatApiV2.Requests
{
    public class UpdateRoomRequest
    {
        public UpdateRoomRequest()
        {
            Owner = new HipchatUser {Id = 0};
            Topic = "";
        }
        public string Name { get; set; }
        public RoomPrivacy Privacy { get; set; }
        public bool IsArchived { get; set; }
        public bool IsGuestAccessible { get; set; }
        public string Topic { get; set; }
        public HipchatUser Owner { get; set; }
    }
}