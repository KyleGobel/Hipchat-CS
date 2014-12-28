using HipchatApiV2.Enums;

namespace HipchatApiV2.Requests
{
    public class ShareFileWithRoomRequest
    {
        public string Message { get; set; }
        public string File { get; set; }
    }
}