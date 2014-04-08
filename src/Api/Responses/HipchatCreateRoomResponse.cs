namespace HipchatApiV2.Responses
{
    public class HipchatCreateRoomResponse
    {
        public int Id { get; set; } 
        public HipchatLinks Linkses { get; set; }
    }

    public class HipchatLinks
    {
        public string Self { get; set; }
        public string Webhooks { get; set; }
        public string Members { get; set; }
        public string Prev { get; set; }
        public string Next { get; set; }
    }
}