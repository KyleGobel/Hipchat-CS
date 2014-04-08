namespace HipchatApiV2.Requests
{
    public class GenerateTokenRequest
    {
        public string Username { get; set; } 
        public string Grant_Type { get; set; }
        public string Code { get; set; }
        public string Redirect_Uri { get; set; }
        public string Scope { get; set; }
        public string Password { get; set; }
        public string Refresh_Token { get; set; }
    }
}