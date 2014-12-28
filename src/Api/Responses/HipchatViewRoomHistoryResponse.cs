using System;
using System.Collections.Generic;
using HipchatApiV2.Enums;

namespace HipchatApiV2.Responses
{
    public class HipchatViewRoomHistoryResponse
    {
        public HipchatViewRoomHistoryResponse()
        {
            Items = new List<HipchatViewRoomHistoryResponseItems>();
        }
        public List<HipchatViewRoomHistoryResponseItems> Items { get; set; } 
        public int StartIndex { get; set; } 
        public int MaxResults { get; set; }
    }

    public class HipchatViewRoomHistoryResponseItems
    {
        public string Id { get; set; }
        public RoomColors Color { get; set; }
        public DateTime Date { get; set; }
        public string From { get; set; } 
        public string Message { get; set; }
        public HipchatMessageFormat MessageFormat { get; set; }
        public MessageType Type { get; set; }
        public HipchatFile File { get; set; }
        public HipchatLink Links { get; set; }

        // TODO: Mentions
    }
}