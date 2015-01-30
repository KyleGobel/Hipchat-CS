using System.Collections.Generic;
using HipchatApiV2.Enums;
using HipchatApiV2.Requests;
using HipchatApiV2.Responses;

namespace HipchatApiV2
{
    public interface IHipchatClient
    {
        /// <summary>
        /// Get room details
        /// </summary>
        /// <param name="roomId">The id of the room</param>
        /// <remarks>
        /// Auth required with scope 'view_group'. https://www.hipchat.com/docs/apiv2/method/get_room
        /// </remarks>
        HipchatGetRoomResponse GetRoom(int roomId);

        /// <summary>
        /// Get room details
        /// </summary>
        /// <param name="roomName">The name of the room. Valid length 1-100</param>
        /// <remarks>
        /// Auth required with scope 'view_group'.  https://www.hipchat.com/docs/apiv2/method/get_room
        /// </remarks>
        HipchatGetRoomResponse GetRoom(string roomName);

        /// <summary>
        /// Gets an OAuth token for requested grant type. 
        /// </summary>
        /// <param name="grantType">The type of grant request</param>
        /// <param name="scopes">List of scopes that is requested</param>
        /// <param name="username">The user name to generate a token on behalf of.  Only valid in
        /// the 'Password' and 'ClientCredentials' grant types.</param>
        /// <param name="code">The authorization code to exchange for an access token.  Only valid in the 'AuthorizationCode' grant type</param>
        /// <param name="redirectUri">The Url that was used to generate an authorization code, and it must match that value.  Only valid in the 'AuthorizationCode' grant.</param>
        /// <param name="password">The user's password to use for authentication when creating a token.  Only valid in the 'Password' grant.</param>
        /// <param name="refreshToken">The refresh token to use to generate a new access token.  Only valid in the 'RefreshToken' grant.</param>
        HipchatGenerateTokenResponse GenerateToken(
            GrantType grantType, 
            IEnumerable<TokenScope> scopes,
            string username = null,  
            string code = null, 
            string redirectUri = null, 
            string password = null, 
            string refreshToken = null);

        /// <summary>
        /// Sends a user a private message. 
        /// </summary>
        /// <param name="idOrEmailOrMention">The id, email address, or mention name (beginning with an '@') of the user to send a message to.</param>
        /// <param name="message">The message body. Valid length range: 1 - 10000.</param>
        /// <param name="notify">Whether this message should trigger a user notification (change the tab color, play a sound, notify mobile phones, etc). Each recipient's notification preferences are taken into account.</param>
        /// <param name="messageFormat">Determines how the message is treated by our server and rendered inside HipChat applications</param>
        /// <remarks>
        ///  Auth required with scope 'send_message'. https://www.hipchat.com/docs/apiv2/method/private_message_user
        /// </remarks>
        void PrivateMessageUser(string idOrEmailOrMention, string message, bool notify = false,
            HipchatMessageFormat messageFormat = HipchatMessageFormat.Text);

        /// <summary>
        /// Get emoticon details
        /// </summary>
        /// <param name="id">The emoticon id</param>
        /// <returns>the emoticon details</returns>
        /// <remarks>
        /// Auth required with scope 'view_group'. https://www.hipchat.com/docs/apiv2/method/get_emoticon
        /// </remarks>
        HipchatGetEmoticonResponse GetEmoticon(int id = 0);

        /// <summary>
        /// Get emoticon details
        /// </summary>
        /// <param name="shortcut">The emoticon shortcut</param>
        /// <returns>the emoticon details</returns>
        /// <remarks>
        /// Auth required with scope 'view_group'. https://www.hipchat.com/docs/apiv2/method/get_emoticon
        /// </remarks>
        HipchatGetEmoticonResponse GetEmoticon(string shortcut = "");

        /// <summary>
        /// Gets all emoticons for the current group
        /// </summary>
        /// <param name="startIndex">The start index for the result set</param>
        /// <param name="maxResults">The maximum number of results</param>
        /// <param name="type">The type of emoticons to get</param>
        /// <returns>the matching set of emoticons</returns>
        /// <remarks>
        /// Auth required with scope 'view_group'. https://www.hipchat.com/docs/apiv2/method/get_all_emoticons
        /// </remarks>
        HipchatGetAllEmoticonsResponse GetAllEmoticons(int startIndex = 0, int maxResults = 100, EmoticonType type = EmoticonType.All);

        HipchatGetAllUsersResponse GetAllUsers(int startIndex = 0, int maxResults = 100, bool includeGuests = false,
            bool includeDeleted = false);

        /// <summary>
        /// Gets information about the requested user
        /// </summary>
        /// <param name="emailOrMentionName">The users email address or mention name beginning with @</param>
        /// <returns>an object with information about the user</returns>
        /// <remarks>
        /// Auth required with scope 'view_group'. https://www.hipchat.com/docs/apiv2/method/view_user
        /// </remarks>
        HipchatGetUserInfoResponse GetUserInfo(string emailOrMentionName);

        /// <summary>
        /// Gets information about the requested user
        /// </summary>
        /// <param name="userId">The integer Id of the user</param>
        /// <returns>an object with information about the user</returns>
        /// <remarks>
        /// Auth required with scope 'view_group'. https://www.hipchat.com/docs/apiv2/method/view_user
        /// </remarks>
        HipchatGetUserInfoResponse GetUserInfo(int userId);

        /// <summary>
        /// Updates a room
        /// </summary>
        /// <param name="roomId">The room id</param>
        /// <param name="request">The request to send</param>
        /// <returns>true if the call was successful</returns>
        /// <remarks>
        /// Auth required with scope 'admin_room'. https://www.hipchat.com/docs/apiv2/method/update_room 
        /// </remarks>
        bool UpdateRoom(int roomId, UpdateRoomRequest request);

        /// <summary>
        /// Updates a room
        /// </summary>
        /// <param name="roomName">The room name</param>
        /// <param name="request">The request to send</param>
        /// <returns>true if the call was successful</returns>
        /// <remarks>
        /// Auth required with scope 'admin_room'. https://www.hipchat.com/docs/apiv2/method/update_room 
        /// </remarks>
        bool UpdateRoom(string roomName, UpdateRoomRequest request);

        /// <summary>
        /// Creates a webhook
        /// </summary>
        /// <param name="roomId">the id of the room</param>
        /// <param name="url">the url to send the webhook POST to</param>
        /// <param name="pattern">optional regex pattern to match against messages.  Only applicable for message events</param>
        /// <param name="eventType">The event to listen for</param>
        /// <param name="name">label for this webhook</param>
        /// <remarks>
        /// Auth required with scope 'admin_room'. https://www.hipchat.com/docs/apiv2/method/create_webhook
        /// </remarks>
        CreateWebHookResponse CreateWebHook(int roomId, string url, string pattern, RoomEvent eventType, string name);

        /// <summary>
        /// Creates a webhook
        /// </summary>
        /// <param name="roomName">the name of the room</param>
        /// <param name="url">the url to send the webhook POST to</param>
        /// <param name="pattern">optional regex pattern to match against messages.  Only applicable for message events</param>
        /// <param name="eventType">The event to listen for</param>
        /// <param name="name">label for this webhook</param>
        /// <remarks>
        /// Auth required with scope 'admin_room'. https://www.hipchat.com/docs/apiv2/method/create_webhook
        /// </remarks>
        CreateWebHookResponse CreateWebHook(string roomName, string url, string pattern, RoomEvent eventType, string name);

        /// <summary>
        /// Creates a webhook
        /// </summary>
        /// <param name="roomName">the name of the room</param>
        /// <param name="request">the request to send</param>
        /// <remarks>
        /// Auth required with scope 'admin_room'. https://www.hipchat.com/docs/apiv2/method/create_webhook
        /// </remarks>
        CreateWebHookResponse CreateWebHook(string roomName, CreateWebHookRequest request);

        /// <summary>
        ///  Creates a new room
        /// </summary>
        /// <param name="nameOfRoom">Name of the room.  Valid Length 1-50</param>
        /// <param name="guestAccess">Whether or not to enable guest access for this room</param>
        /// <param name="ownerUserId">The id, email address, or mention name (beginning with an '@') of
        /// the room's owner.  Defaults to the current user.</param>
        /// <param name="privacy">Whether the room is available for access by other users or not</param>
        /// <returns>response containing id and link of the created room</returns>
        /// <remarks>
        /// Auth required with scope 'manage_rooms'. https://api.hipchat.com/v2/room
        /// </remarks>
        HipchatCreateRoomResponse CreateRoom(string nameOfRoom, bool guestAccess = false, string ownerUserId = null,
            RoomPrivacy privacy = RoomPrivacy.Public);

        /// <summary>
        ///  Creates a new room
        /// </summary>
        /// <returns>response containing id and link of the created room</returns>
        /// <remarks>
        /// Auth required with scope 'manage_rooms'. https://api.hipchat.com/v2/room
        /// </remarks>
        HipchatCreateRoomResponse CreateRoom(CreateRoomRequest request);

        /// <summary>
        /// Send a message to a room
        /// </summary>
        /// <param name="roomId">The id of the room</param>
        /// <param name="message">message to send</param>
        /// <param name="backgroundColor">the background color of the message, only applicable to html format message</param>
        /// <param name="notify">if the message should notify</param>
        /// <param name="messageFormat">the format of the message</param>
        /// <returns>true if the message was sucessfully sent</returns>
        /// <remarks>
        /// Auth required with scope 'view_group'. https://www.hipchat.com/docs/apiv2/method/send_room_notification
        /// </remarks>
        bool SendNotification(int roomId, string message, RoomColors backgroundColor = RoomColors.Yellow,
            bool notify = false, HipchatMessageFormat messageFormat = HipchatMessageFormat.Html);

        /// <summary>
        /// Send a message to a room
        /// </summary>
        /// <param name="roomName">The name of the room</param>
        /// <param name="message">message to send</param>
        /// <param name="backgroundColor">the background color of the message, only applicable to html format message</param>
        /// <param name="notify">if the message should notify</param>
        /// <param name="messageFormat">the format of the message</param>
        /// <returns>true if the message was sucessfully sent</returns>
        /// <remarks>
        /// Auth required with scope 'view_group'. https://www.hipchat.com/docs/apiv2/method/send_room_notification
        /// </remarks>
        bool SendNotification(string roomName, string message, RoomColors backgroundColor = RoomColors.Yellow,
            bool notify = false, HipchatMessageFormat messageFormat = HipchatMessageFormat.Html);

        /// <summary>
        /// Send a message to a room
        /// </summary>
        /// <param name="roomId">The id of the room</param>
        /// <param name="request">The request containing the info about the notification to send</param>
        /// <returns>true if the message successfully sent</returns>
        /// <remarks>
        /// Auth required with scope 'view_group'. https://www.hipchat.com/docs/apiv2/method/send_room_notification
        /// </remarks>
        bool SendNotification(int roomId, SendRoomNotificationRequest request);

        /// <summary>
        /// Send a message to a room
        /// </summary>
        /// <param name="roomName">The id of the room</param>
        /// <param name="request">The request containing the info about the notification to send</param>
        /// <returns>true if the message successfully sent</returns>
        /// <remarks>
        /// Auth required with scope 'view_group'. https://www.hipchat.com/docs/apiv2/method/send_room_notification
        /// </remarks>
        bool SendNotification(string roomName, SendRoomNotificationRequest request);

        /// <summary>
        /// Share a file with a room
        /// </summary>
        /// <param name="roomName">The id or name of the room</param>
        /// <param name="fileFullPath">The full path of the file.</param>
        /// <param name="message">The optional message.</param>
        /// <returns>
        /// true if the file was successfully shared
        /// </returns>
        /// <remarks>
        /// Auth required with scope 'send_message'. https://www.hipchat.com/docs/apiv2/method/share_file_with_room
        /// </remarks>
        bool ShareFileWithRoom(string roomName, string fileFullPath, string message = null);

        /// <summary>
        /// Delets a room and kicks the current particpants.
        /// </summary>
        /// <param name="roomId">Id of the room.</param>
        /// <returns>true if the room was successfully deleted</returns>
        /// <remarks>
        /// Authentication required with scope 'manage_rooms'. https://www.hipchat.com/docs/apiv2/method/delete_room
        /// </remarks>
        bool DeleteRoom(int roomId);

        /// <summary>
        /// Delets a room and kicks the current particpants.
        /// </summary>
        /// <param name="roomName">Name of the room.</param>
        /// <returns>true if the room was successfully deleted</returns>
        /// <remarks>
        /// Authentication required with scope 'manage_rooms'. https://www.hipchat.com/docs/apiv2/method/delete_room
        /// </remarks>
        bool DeleteRoom(string roomName);

        /// <summary>
        /// List non-archived rooms for this group
        /// </summary>
        /// <param name="startIndex">The start index for the result set</param>
        /// <param name="maxResults">The maximum number of results. Valid length 0-100</param>
        /// <param name="includeArchived">Filter rooms</param>
        /// <returns>A HipchatGetAllRoomsResponse</returns>
        /// <remarks>
        /// Auth required with scope 'view_group'. https://www.hipchat.com/docs/apiv2/method/get_all_rooms
        /// </remarks>
        HipchatGetAllRoomsResponse GetAllRooms(int startIndex = 0, int maxResults = 100, bool includePrivate = false, bool includeArchived = false);

        /// <summary>
        /// Fetch chat history for this room
        /// </summary>
        /// <param name="roomName">Name of the room.</param>
        /// <param name="date">Either the latest date to fetch history for in ISO-8601 format, or 'recent' to fetch the latest 75 messages. Note, paging isn't supported for 'recent', however they are real-time values, whereas date queries may not include the most recent messages.</param>
        /// <param name="timezone">Your timezone. Must be a supported timezone name, please see wikipedia TZ database page.</param>
        /// <param name="startIndex">The start index for the result set</param>
        /// <param name="maxResults">The maximum number of results. Valid length 0-100</param>
        /// <param name="reverse">Reverse the output such that the oldest message is first. For consistent paging, set to <c>false</c>.</param>
        /// <returns>
        /// A HipchatGetAllRoomsResponse
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// roomName;Valid roomName length is 1-100.
        /// or
        /// date;Valid date should be passed.
        /// or
        /// timezone;Valid timezone should be passed.
        /// or
        /// startIndex;startIndex must be between 0 and 100
        /// or
        /// maxResults;maxResults must be between 0 and 1000
        /// </exception>
        /// <remarks>
        /// Authentication required, with scope view_group, view_messages. https://www.hipchat.com/docs/apiv2/method/view_room_history
        /// </remarks>
        HipchatViewRoomHistoryResponse ViewRoomHistory(string roomName, string date = "recent", string timezone = "UTC", int startIndex = 0, int maxResults = 100, bool reverse = true);

        /// <summary>
        /// Fetch latest chat history for this room
        /// </summary>
        /// <param name="roomName">Name of the room.</param>
        /// <param name="notBefore">The id of the message that is oldest in the set of messages to be returned. The server will not return any messages that chronologically precede this message.</param>
        /// <param name="timezone">Your timezone. Must be a supported timezone name, please see wikipedia TZ database page.</param>
        /// <param name="startIndex">The start index for the result set</param>
        /// <param name="maxResults">The maximum number of results. Valid length 0-100</param>
        /// <returns>
        /// A HipchatGetAllRoomsResponse
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// roomName;Valid roomName length is 1-100.
        /// or
        /// timezone;Valid timezone should be passed.
        /// or
        /// startIndex;startIndex must be between 0 and 100
        /// or
        /// maxResults;maxResults must be between 0 and 1000
        /// </exception>
        /// <remarks>
        /// Authentication required, with scope view_messages. https://www.hipchat.com/docs/apiv2/method/view_recent_room_history
        /// </remarks>
        HipchatViewRoomHistoryResponse ViewRecentRoomHistory(string roomName, string notBefore = "", string timezone = "UTC", int startIndex = 0, int maxResults = 100);

        /// <summary>
        /// Gets all webhooks for this room
        /// </summary>
        /// <param name="roomName">The name of the room</param>
        /// <param name="startIndex">The start index for the result set</param>
        /// <param name="maxResults">The maximum number of results</param>
        /// <returns>A GetAllWebhooks Response</returns>
        /// <remarks>
        /// Auth required, with scope 'admin_room'. https://www.hipchat.com/docs/apiv2/method/get_all_webhooks
        /// </remarks>
        HipchatGetAllWebhooksResponse GetAllWebhooks(string roomName, int startIndex = 0, int maxResults = 0);

        /// <summary>
        /// Gets all webhooks for this room
        /// </summary>
        /// <param name="roomId">The id of the room</param>
        /// <param name="startIndex">The start index for the result set</param>
        /// <param name="maxResults">The maximum number of results</param>
        /// <returns>A GetAllWebhooks Response</returns>
        /// <remarks>
        /// Auth required, with scope 'admin_room'. https://www.hipchat.com/docs/apiv2/method/get_all_webhooks
        /// </remarks>
        HipchatGetAllWebhooksResponse GetAllWebhooks(int roomId, int startIndex = 0, int maxResults = 0);

        /// <summary>
        /// Deletes a webhook
        /// </summary>
        /// <param name="roomName">The name of the room</param>
        /// <param name="webHookId">The id of the webhook</param>
        /// <returns>true if the request was successful</returns>
        /// <remarks>
        /// Auth required with scope 'admin_room'. https://www.hipchat.com/docs/apiv2/method/delete_webhook
        /// </remarks>
        bool DeleteWebhook(string roomName, int webHookId);

        /// <summary>
        /// Deletes a webhook
        /// </summary>
        /// <param name="roomId">The id of the room</param>
        /// <param name="webHookId">The id of the webhook</param>
        /// <returns>true if the request was successful</returns>
        /// <remarks>
        /// Auth required with scope 'admin_room'. https://www.hipchat.com/docs/apiv2/method/delete_webhook
        /// </remarks>
        bool DeleteWebhook(int roomId, int webHookId);

        /// <summary>
        /// Set a room's topic.  Useful for displaying statistics, important links, server status, you name it!
        /// </summary>
        /// <param name="roomName">The name of the room</param>
        /// <param name="topic">The topic body. (Valid length 0 - 250)</param>
        /// <returns>true if the call succeeded.  There may be slight delay before topic change appears in the room </returns>
        /// <remarks>
        /// Auth required with scope 'admin_room'.
        /// https://www.hipchat.com/docs/apiv2/method/set_topic
        /// </remarks>
        bool SetTopic(string roomName, string topic);

        /// <summary>
        /// Set a room's topic.  Useful for displaying statistics, important links, server status, you name it!
        /// </summary>
        /// <param name="roomId">The id of the room</param>
        /// <param name="topic">The topic body. (Valid length 0 - 250)</param>
        /// <returns>true if the call succeeded.  There may be slight delay before topic change appears in the room </returns>
        /// <remarks>
        /// Auth required with scope 'admin_room'.
        /// https://www.hipchat.com/docs/apiv2/method/set_topic
        /// </remarks>
        bool SetTopic(int roomId, string topic);

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <returns>A HipchatCreateUserReponse</returns>
        /// <remarks>
        /// Auth required with scope 'admin_group'. https://www.hipchat.com/docs/apiv2/method/create_user
        /// </remarks>
        HipchatCreateUserResponse CreateUser(CreateUserRequest request);

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="idOrEmail">The id, email address, or mention name (beginning with an '@') of the user to delete.</param>
        /// <returns>A HipchatDeleteUserReponse</returns>
        /// <remarks>
        /// Auth required with scope 'admin_group'. https://api.hipchat.com/v2/user/{id_or_email}
        /// </remarks>
        bool DeleteUser(string idOrEmail);

        /// <summary>
        /// Add a member to a private room
        /// </summary>
        /// <param name="roomName">The name of the room</param>
        /// <param name="idOrEmail">The id, email address, or mention name (beginning with an '@') of the user to delete.</param>
        /// <returns>true if the call succeeded. </returns>
        /// <remarks>
        /// Auth required with scope 'admin_room'.
        /// https://www.hipchat.com/docs/apiv2/method/add_member
        /// </remarks>
        bool AddMember(string roomName, string idOrEmail);

        /// <summary>
        /// Update a user photo
        /// </summary>
        /// <param name="idOrEmail">The id, email address, or mention name (beginning with an '@') of the user to delete.</param>
        /// <param name="photo"> Base64 string of the photo</param>
        /// <returns>true if the call succeeded.   </returns>
        /// <remarks>
        /// Auth required with scope 'admin_room'.
        /// https://www.hipchat.com/docs/apiv2/method/update_photo
        /// </remarks>
        bool UpdatePhoto(string idOrEmail, string photo);
    }
}