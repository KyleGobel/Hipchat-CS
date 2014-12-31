namespace HipchatApiV2.Enums
{
    public class TokenScope
    {
        private string _value;

        private TokenScope(string value)
        {
            _value = value;
        }

        public static implicit operator string(TokenScope s)
        {
            return s._value;
        }
        public override string ToString()
        {
            return _value;
        }

        public static readonly TokenScope AdminGroup = new TokenScope("admin_group");
        public static readonly TokenScope AdminRoom = new TokenScope("admin_room");
        public static readonly TokenScope ManageRooms = new TokenScope("manage_rooms");
        public static readonly TokenScope SendMessage = new TokenScope("send_message");
        public static readonly TokenScope SendNotification = new TokenScope("send_notification");
        public static readonly TokenScope ViewGroup = new TokenScope("view_group");
        public static readonly TokenScope ViewMessages = new TokenScope("view_messages");
    }
}