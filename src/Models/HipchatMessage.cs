using System;
using System.Collections.Generic;

namespace Hipchat.Models
{
    public class HipchatMessage
    {
        public HipchatMessage()
        {
           Mentions = new List<HipchatUser>(); 
        }
        public int RoomId { get; set; } 
        public string RoomName { get; set; }
        public string MessageSent { get; set; }
        public List<HipchatUser> Mentions { get; set; }
        public DateTime DateSent { get; set; }
        public string FileUrl { get; set; }
        public HipchatUser From { get; set; }
    }

    public class HipchatUser
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string MentionName { get; set; }
    }
}