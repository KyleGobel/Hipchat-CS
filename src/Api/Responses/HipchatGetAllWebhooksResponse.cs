using System.Collections.Generic;

namespace HipchatApiV2.Responses
{
    public class HipchatGetAllWebhooksResponse
    {
        public List<HipchatGetAllWebhooksItems> Items { get; set; }
        public int StartIndex { get; set; }
        public int MaxResults { get; set; }
        public HipchatLink Links { get; set; }
    }
}