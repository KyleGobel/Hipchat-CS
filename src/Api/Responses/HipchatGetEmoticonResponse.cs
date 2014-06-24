using System.Collections.Generic;

namespace HipchatApiV2.Responses
{
    public class HipchatGetEmoticonResponse
    {
        public int Id { get; set; }
        public string Shortcut { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string AudioPath { get; set; } 

    }
}