using System.Collections.Generic;

namespace HipchatApiV2.Responses
{
    public class HipchatGetAllEmoticonsResponse
    {
        public HipchatGetAllEmoticonsResponse()
        {
            Items = new List<HipchatGetAllEmoticonsResponseItem>();
        }
        public List<HipchatGetAllEmoticonsResponseItem> Items { get; set; } 
        public int StartIndex { get; set; } 
        public int MaxResults { get; set; }
        public HipchatLink Links { get; set; }
    }

    public class HipchatGetAllEmoticonsResponseItem
    {
        public int Id { get; set; }
        public string Shortcut { get; set; }
        public string Url { get; set; }
        public HipchatLink Links { get; set; }
    }

}