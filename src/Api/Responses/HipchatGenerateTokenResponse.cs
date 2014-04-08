namespace HipchatApiV2.Responses
{
    public class HipchatGenerateTokenResponse
    {
        public string Access_Token { get; set; } 
        public long Expires_In { get; set; }
        public string Group_Name { get; set; }
        public string Token_Type { get; set; }
        public string Scope { get; set; }
        public string Group_Id { get; set; }
    }
}