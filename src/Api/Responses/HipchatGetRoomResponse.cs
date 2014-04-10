using System;
using System.Collections.Generic;
using HipchatApiV2.Enums;

namespace HipchatApiV2.Responses
{
    public class HipchatGetRoomResponse
    {
        public string Xmpp_Jid { get; set; }
        public HipchatLink Links { get; set; }
        public string Name { get; set; }
        public DateTime? Last_Active { get; set; }
        public DateTime Created { get; set; }
        public bool Is_Archived { get; set; }
        public RoomPrivacy Privacy { get; set; }
        public bool Is_Guest_Accessible { get; set; }
        public string Topic { get; set; }
        public List<HipchatUser> Participants { get; set; }
        public HipchatUser Owner { get; set; }
        public int Id { get; set; }
        public string Guest_Access_Url { get; set; }
    }
}