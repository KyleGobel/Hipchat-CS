using System.Collections.Generic;

namespace HipchatApiV2.Responses
{
    public class HipchatGetAllRoomsResponse
    {
        public HipchatGetAllRoomsResponse()
        {
            Items = new List<HipchatGetAllRoomsResponseItems>();
        }
        public List<HipchatGetAllRoomsResponseItems> Items { get; set; } 
        public int StartIndex { get; set; } 
        public int MaxResults { get; set; }
        
    }

    public class HipchatGetAllRoomsResponseItems
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public HipchatLinks Links { get; set; }
    }

}