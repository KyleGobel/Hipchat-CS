using HipchatApiV2.Responses;

namespace HipchatApiV2
{
    public class HipchatUser
    {
        public string Mention_Name { get; set; }
        public int Id { get; set; }
        public HipchatLink Link { get; set; }
        public string Name { get; set; }
    }
}