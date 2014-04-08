namespace HipchatApiV2.Requests
{
    public class SendRoomNotificationRequest
    {
        public string Message { get; set; }
        public string Color { get; set; }
        public bool Notify { get; set; }
        public string Message_Format { get; set; }
    }
}