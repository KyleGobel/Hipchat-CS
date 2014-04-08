namespace HipchatApiV2.Enums
{
    public class GrantType
    {
        private readonly string _value;

        private GrantType(string value)
        {
            _value = value;
        }

        public static implicit operator string(GrantType s)
        {
            return s._value;
        }
            
        public static readonly GrantType AuthorizationCode = new GrantType("authorization_code");
        public static readonly GrantType RefreshToken = new GrantType("refresh_token");
        public static readonly GrantType Password = new GrantType("password");
        public static readonly GrantType ClientCredentials = new GrantType("client_credentials");
        public static readonly GrantType Personal = new GrantType("personal");
        public static readonly GrantType RoomNotification = new GrantType("room_notification");

        public override string ToString()
        {
            return _value;
        } 
    }
}