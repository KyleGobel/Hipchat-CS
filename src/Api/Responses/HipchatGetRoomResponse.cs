using System;
using System.Collections.Generic;
using HipchatApiV2.Enums;

namespace HipchatApiV2.Responses
{
    public class HipchatGetRoomResponse
    {
        public string XmppJid { get; set; }
        public HipchatLink Links { get; set; }
        public string Name { get; set; }
        public DateTime? LastActive { get; set; }
        public DateTime Created { get; set; }
        public bool IsArchived { get; set; }
        public RoomPrivacy Privacy { get; set; }
        public bool IsGuestAccessible { get; set; }
        public string Topic { get; set; }
        public List<HipchatUser> Participants { get; set; }
        public HipchatUser Owner { get; set; }
        public int Id { get; set; }
        public string GuestAccessUrl { get; set; }
    }
}