using HipchatApiV2.Responses;

namespace HipchatApiV2.Requests
﻿{
    public class UpdateUserRequest
    {
        /// <summary>
        /// Gets or sets the user's full name.
        /// </summary>
        /// <remarks>Valid length range: 1 - 50</remarks>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the user's title.
        /// </summary>
        /// <remarks>Optional</remarks>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the user's @mention name.
        /// </summary>
        public string MentionName { get; set; }

        /// <summary>
        /// Gets or sets whether or not this user is an admin.
        /// </summary>
        /// <remarks>Optional. Default to false.</remarks>
        public bool IsGroupAdmin { get; set; }

        /// <summary>
        /// Gets or sets the user's timezone.
        /// </summary>
        /// <remarks>Optional. Must be a supported timezone. Default "UTC"</remarks>
        public string TimeZone { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <remarks>
        /// Optional
        /// User's password. If not provided, the existing password is kept
        /// </remarks>>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the user's email.
        /// </summary>
        public string Email { get; set; }
    }
}
