namespace HipchatApiV2.Responses
{
    public class HipchatPresence
    {
        public string Status { get; set; }
        public int Idle { get; set; }
        public string Show { get; set; }
        public HipchatClientInfo Client { get; set; }
        public bool IsOnline { get; set; }
    }
}