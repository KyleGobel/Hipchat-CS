using HipchatApiV2.Enums;

namespace HipchatApiV2.Requests
{
    public class GenerateTokenRequest
    {
        public string Username { get; set; } 
        public GrantType GrantType { get; set; }
        public string Code { get; set; }
        public string RedirectUri { get; set; }
        public string Scope { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
    }
}