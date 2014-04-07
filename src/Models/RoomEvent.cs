namespace Hipchat.Models
{
    //handy ass type-safe enum pattern here
    public class RoomEvent 
    {
        private readonly string _value;

        private RoomEvent(string value)
        {
            _value = value;
        }

        public static implicit operator string(RoomEvent s)
        {
            return s._value;
        }
            
        public static readonly RoomEvent Message = new RoomEvent("room_message");
        public static readonly RoomEvent Enter = new RoomEvent("room_enter");
        public static readonly RoomEvent Exit = new RoomEvent("room_exit");
        public static readonly RoomEvent Notification = new RoomEvent("room_notification");

        public override string ToString()
        {
            return _value;
        }
    }
}