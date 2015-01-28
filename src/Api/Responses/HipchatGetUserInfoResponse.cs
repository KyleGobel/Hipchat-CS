namespace HipchatApiV2.Responses
{
    public class HipchatGetUserInfoResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string MentionName { get; set; }
        public string Email { get; set; }
        public string PhotoUrl { get; set; }
        public string XmppJid { get; set; }
        public bool IsDeleted { get; set; }
        public string LastActive { get; set; }
        public string Created { get; set; }
        public bool IsGroupAdmin { get; set; }
        public string Timezone { get; set; }
        public bool IsGuest { get; set; }
        public HipchatPresence Presence { get; set; }
    }
}