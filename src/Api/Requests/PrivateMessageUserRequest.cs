using HipchatApiV2.Enums;

namespace HipchatApiV2.Requests
{
    public class PrivateMessageUserRequest
    {

        public string Message { get; set; } //1-10000 chars - required

        public bool? Notify { get; set; } //default: false
        public HipchatMessageFormat MessageFormat { get; set; }

    }
}