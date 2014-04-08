namespace HipchatApiV2.Requests
{
    public class CreateRoomRequest
    {
        public bool Guest_Access { get; set; }
        public string Name { get; set; }
        public string Owner_User_Id { get; set; }
        public string Privacy { get; set; }
    }
}