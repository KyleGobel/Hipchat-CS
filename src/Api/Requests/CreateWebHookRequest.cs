namespace Hipchat.Models.Requests
{
    public class CreateWebHookRequest
    {
        public string Url { get; set; }
        public string Pattern { get; set; }
        public string Event { get; set; }
        public string Name { get; set; }
    }
}