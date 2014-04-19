using System.Collections.Generic;

namespace HipchatApiV2.Responses
{
    public class HipchatGetAllUsersResponse
    {
        public List<HipchatUser> Items { get; set; }
        public int StartIndex { get; set; }
        public int MaxResults { get; set; }
        public HipchatLink Links { get; set; }
    }
}