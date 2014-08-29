namespace HipchatApiV2.Requests
﻿{
    public class CreateUserRequest
    {
        public string Name { get; set; }        // User's full name. Valid length range: 1 - 50
        public string Email { get; set; }
        public string Password { get; set; }    // Optional. If not provided, a randomly generated password will be returned
        public string Title { get; set; }        // Optional. User's title
        public string MentionName { get; set; } // Optional. User's @mention name
        public bool IsGroupAdmin { get; set; }  // Optional. Default false
        public string TimeZone { get; set; }    // Optional. User's timezone. Must be a supported timezone. Default "UTC"
    }
}
