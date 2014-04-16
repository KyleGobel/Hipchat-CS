using HipchatApiV2.Enums;

namespace HipchatApiV2.Requests
{
    public class CreateRoomRequest
    {
        public bool GuestAccess { get; set; }
        public string Name { get; set; }
        public string OwnerUserId { get; set; }
        public RoomPrivacy Privacy { get; set; }
    }
}