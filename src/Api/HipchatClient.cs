using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using HipchatApiV2.Enums;
using HipchatApiV2.Requests;
using HipchatApiV2.Responses;
using ServiceStack;
using ServiceStack.Text;

namespace HipchatApiV2
{
    public class HipchatClient
    {
        private readonly string _authToken;
        /// <summary>
        /// Creates a new HipchatClient, you can pass in an optional
        /// authToken, or by default it will look in the web/app config
        /// file appSettings for 'hipchat_auth_token'
        /// </summary>
        /// <param name="authToken">the auth token given by hipchat</param>
        public HipchatClient(string authToken = null)
        {
            _authToken = authToken ?? HipchatApiConfig.AuthToken;

            ConfigureSerializer();
        }

        private void ConfigureSerializer()
        {
            JsConfig<RoomColors>.SerializeFn = colors => colors.ToString().ToLower();
            JsConfig<HipchatMessageFormat>.SerializeFn = format => format.ToString().ToLower();
            JsConfig<RoomPrivacy>.SerializeFn = p =>
            {
                var value = p.ToString().ToLowercaseUnderscore();
                return value;
            };
            JsConfig<RoomEvent>.SerializeFn = rmEvent => rmEvent.ToString().ToLowercaseUnderscore();
            JsConfig<GrantType>.SerializeFn = grant => grant.ToString().ToLowercaseUnderscore();
            JsConfig<RoomEvent>.DeSerializeFn = s =>
            {
                var pascalCase = s.ToTitleCase();
                RoomEvent e;
                RoomEvent.TryParse(s, out e);
                return e;
            };
        }
        private JsConfigScope JsonSerializerConfigScope()
        {
            return JsConfig.With(
                emitLowercaseUnderscoreNames: true, 
                //have to set this to false -- issue in SS
                emitCamelCaseNames: false,
                propertyConvention: PropertyConvention.Lenient);

        }

        #region Get Room
        /// <summary>
        /// Get room details
        /// </summary>
        /// <param name="roomId">The id of the room</param>
        /// <remarks>
        /// Auth required with scope 'view_group'. https://www.hipchat.com/docs/apiv2/method/get_room
        /// </remarks>
        public HipchatGetRoomResponse GetRoom(int roomId)
        {
            return GetRoom(roomId.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Get room details
        /// </summary>
        /// <param name="roomName">The name of the room. Valid length 1-100</param>
        /// <remarks>
        /// Auth required with scope 'view_group'.  https://www.hipchat.com/docs/apiv2/method/get_room
        /// </remarks>
        public HipchatGetRoomResponse GetRoom(string roomName)
        {
            using (JsonSerializerConfigScope())
            {
                if (roomName.IsEmpty() || roomName.Length > 100)
                    throw new ArgumentOutOfRangeException(roomName, "Valid Lengths of roomName is 1 to 100 characters.");
                try
                {
                    return HipchatEndpoints.GetRoomEndpointFormat.Fmt(roomName)
                        .AddHipchatAuthentication(_authToken)
                        .GetJsonFromUrl()
                        .FromJson<HipchatGetRoomResponse>();
                }
                catch (WebException exception)
                {
                    throw ExceptionHelpers.WebExceptionHelper(exception, "view_group");
                }
                catch (Exception exception)
                {
                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "GetRoom");
                }
            }
        }
        #endregion

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
        public HipchatGenerateTokenResponse GenerateToken(
            GrantType grantType, 
            IEnumerable<TokenScope> scopes,
            string username = null,  
            string code = null, 
            string redirectUri = null, 
            string password = null, 
            string refreshToken = null)
        {
            using (JsonSerializerConfigScope())
            {
                var request = new GenerateTokenRequest
                {
                    Username = username,
                    Code = code,
                    GrantType = grantType,
                    Password = password,
                    RedirectUri = redirectUri,
                    RefreshToken = refreshToken,
                    Scope = string.Join(" ", scopes.Select(x => x.ToString()))
                };

                try
                {
                    return HipchatEndpoints.GenerateTokenEndpoint
                        .AddHipchatAuthentication(_authToken)
                        .PostJsonToUrl(request)
                        .FromJson<HipchatGenerateTokenResponse>();
                }
                catch (Exception exception)
                {
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "GenerateToken");
                }
            }
        }

        #region Private Message to User

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
        public void PrivateMessageUser(string idOrEmailOrMention, string message, bool notify = false,
            HipchatMessageFormat messageFormat = HipchatMessageFormat.Text)
        {
            if (idOrEmailOrMention.IsEmpty() || idOrEmailOrMention.Length > 10000)
                throw new ArgumentOutOfRangeException("idOrEmailOrMention", "Valid length range: 1 - 10000.");

            var endpoint = HipchatEndpoints.PrivateMessageUserEnpointFormat
                .Fmt(idOrEmailOrMention);

            var request = new PrivateMessageUserRequest
            {
                Message = message,
                Notify = notify,
                MessageFormat = messageFormat
            };

            try
            {
                using (JsonSerializerConfigScope())
                {
                    endpoint
                        .AddHipchatAuthentication(_authToken)
                        .PostJsonToUrl(request);
                }


                //We could assert that we get a 204 here and return a boolean success / failure
                //but i guess we'll just assume everything went well or we would have got an exception
            }
            catch (Exception x)
            {
                if (x is WebException)
                    throw ExceptionHelpers.WebExceptionHelper(x as WebException, "send_message");

                throw ExceptionHelpers.GeneralExceptionHelper(x, "PrivateMessageUser");
            }

        }
        #endregion
        #region GetEmoticon
        /// <summary>
        /// Get emoticon details
        /// </summary>
        /// <param name="id">The emoticon id</param>
        /// <returns>the emoticon details</returns>
        /// <remarks>
        /// Auth required with scope 'view_group'. https://www.hipchat.com/docs/apiv2/method/get_emoticon
        /// </remarks>
        public HipchatGetEmoticonResponse GetEmoticon(int id = 0)
        {
            using (JsonSerializerConfigScope())
            {
                try
                {
                    return HipchatEndpoints.GetEmoticonEndpoint
                        .Fmt(id)
                        .AddHipchatAuthentication(_authToken)
                        .GetJsonFromUrl()
                        .FromJson<HipchatGetEmoticonResponse>();
                }
                catch (Exception exception)
                {
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "view_group");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "GetEmoticon");
                }
            }
        }

        /// <summary>
        /// Get emoticon details
        /// </summary>
        /// <param name="shortcut">The emoticon shortcut</param>
        /// <returns>the emoticon details</returns>
        /// <remarks>
        /// Auth required with scope 'view_group'. https://www.hipchat.com/docs/apiv2/method/get_emoticon
        /// </remarks>
        public HipchatGetEmoticonResponse GetEmoticon(string shortcut = "")
        {
            using (JsonSerializerConfigScope())
            {
                try
                {
                    return HipchatEndpoints.GetEmoticonEndpoint
                        .Fmt(shortcut)
                        .AddHipchatAuthentication(_authToken)
                        .GetJsonFromUrl()
                        .FromJson<HipchatGetEmoticonResponse>();
                }
                catch (Exception exception)
                {
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "view_group");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "GetEmoticon");
                }
            }
        }

        #endregion

        #region GetAllEmoticons
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
        public HipchatGetAllEmoticonsResponse GetAllEmoticons(int startIndex = 0, int maxResults = 100, EmoticonType type = EmoticonType.All)
        {
            using (JsonSerializerConfigScope())
            {
                try
                {
                    return HipchatEndpoints.GetAllEmoticonsEndpoint
                        .AddHipchatAuthentication(_authToken)
                        .AddQueryParam("start-index", startIndex)
                        .AddQueryParam("max-results", maxResults)
                        .AddQueryParam("type", type)
                        .GetJsonFromUrl()
                        .FromJson<HipchatGetAllEmoticonsResponse>();
                }
                catch (Exception exception)
                {
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "view_group");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "GetAllEmoticons");
                }
            }
        }

        #endregion

        #region GetAllUsers

        public HipchatGetAllUsersResponse GetAllUsers(int startIndex = 0, int maxResults = 100, bool includeGuests = false,
            bool includeDeleted = false)
        {
            using (JsonSerializerConfigScope())
            {
                try
                {
                    return HipchatEndpoints.GetAllUsersEndpoint
                        .AddHipchatAuthentication(_authToken)
                        .AddQueryParam("start-index", startIndex)
                        .AddQueryParam("max-results", maxResults)
                        .AddQueryParam("include-guests", includeGuests)
                        .AddQueryParam("include-deleted", includeDeleted)
                        .GetJsonFromUrl()
                        .FromJson<HipchatGetAllUsersResponse>();
                }
                catch (Exception exception)
                {
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "view_group");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "GetAllUsers");
                }
            }
        }

        #endregion

        #region GetUserInfo
        /// <summary>
        /// Gets information about the requested user
        /// </summary>
        /// <param name="emailOrMentionName">The users email address or mention name beginning with @</param>
        /// <returns>an object with information about the user</returns>
        /// <remarks>
        /// Auth required with scope 'view_group'. https://www.hipchat.com/docs/apiv2/method/view_user
        /// </remarks>
        public HipchatGetUserInfoResponse GetUserInfo(string emailOrMentionName)
        {
            using (JsonSerializerConfigScope())
            {
                try
                {
                    return HipchatEndpoints.GetUserInfoEndpoint
                        .Fmt(emailOrMentionName)
                        .AddHipchatAuthentication(_authToken)
                        .GetJsonFromUrl()
                        .FromJson<HipchatGetUserInfoResponse>();
                }
                catch (Exception exception)
                {
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "view_group");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "GetUserInfo");
                }
            }
        }

        /// <summary>
        /// Gets information about the requested user
        /// </summary>
        /// <param name="userId">The integer Id of the user</param>
        /// <returns>an object with information about the user</returns>
        /// <remarks>
        /// Auth required with scope 'view_group'. https://www.hipchat.com/docs/apiv2/method/view_user
        /// </remarks>
        public HipchatGetUserInfoResponse GetUserInfo(int userId)
        {
            return GetUserInfo(userId.ToString(CultureInfo.InvariantCulture));
        }
        #endregion

        #region UpdateRoom
        /// <summary>
        /// Updates a room
        /// </summary>
        /// <param name="roomId">The room id</param>
        /// <param name="request">The request to send</param>
        /// <returns>true if the call was successful</returns>
        /// <remarks>
        /// Auth required with scope 'admin_room'. https://www.hipchat.com/docs/apiv2/method/update_room 
        /// </remarks>
        public bool UpdateRoom(int roomId, UpdateRoomRequest request)
        {
            return UpdateRoom(roomId.ToString(CultureInfo.InvariantCulture), request);
        }

        /// <summary>
        /// Updates a room
        /// </summary>
        /// <param name="roomName">The room name</param>
        /// <param name="request">The request to send</param>
        /// <returns>true if the call was successful</returns>
        /// <remarks>
        /// Auth required with scope 'admin_room'. https://www.hipchat.com/docs/apiv2/method/update_room 
        /// </remarks>
        public bool UpdateRoom(string roomName, UpdateRoomRequest request)
        {
            using (JsonSerializerConfigScope())
            {
                var result = false;
                try
                {

                    HipchatEndpoints.UpdateRoomEndpointFormat.Fmt(roomName)
                        .AddHipchatAuthentication(_authToken)
                        .PutJsonToUrl(data: request, responseFilter: r =>
                        {
                            if (r.StatusCode == HttpStatusCode.NoContent)
                            {
                                result = true;
                            }
                        });

                }
                catch (Exception exception)
                {
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "admin_room");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "Updateroom");
                }
                return result;
            }
        }
        #endregion

        #region CreateWebHook
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
        public CreateWebHookResponse CreateWebHook(int roomId, string url, string pattern, RoomEvent eventType, string name)
        {
            using (JsonSerializerConfigScope())
            {
                var request = new CreateWebHookRequest
                {
                    Event = eventType,
                    Pattern = pattern,
                    Url = url,
                    Name = name
                };
                return CreateWebHook(roomId.ToString(CultureInfo.InvariantCulture), request);
            }
        }

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
        public CreateWebHookResponse CreateWebHook(string roomName, string url, string pattern, RoomEvent eventType, string name)
        {
            using (JsonSerializerConfigScope())
            {
                var request = new CreateWebHookRequest
                {
                    Event = eventType,
                    Pattern = pattern,
                    Url = url,
                    Name = name
                };
                return CreateWebHook(roomName, request);
            }
        }

        /// <summary>
        /// Creates a webhook
        /// </summary>
        /// <param name="roomName">the name of the room</param>
        /// <param name="request">the request to send</param>
        /// <remarks>
        /// Auth required with scope 'admin_room'. https://www.hipchat.com/docs/apiv2/method/create_webhook
        /// </remarks>
        public CreateWebHookResponse CreateWebHook(string roomName, CreateWebHookRequest request)
        {
            using (JsonSerializerConfigScope())
            {
                try
                {
                    return HipchatEndpoints.CreateWebhookEndpointFormat.Fmt(roomName)
                        .AddHipchatAuthentication(_authToken)
                        .PostJsonToUrl(request)
                        .FromJson<CreateWebHookResponse>();
                }
                catch (Exception exception)
                {
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "admin_room");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "CreateWebHook");
                }
            }
        }
        #endregion

        #region CreateRoom
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
        public HipchatCreateRoomResponse CreateRoom(string nameOfRoom, bool guestAccess = false, string ownerUserId = null,
            RoomPrivacy privacy = RoomPrivacy.Public)
        {
            using (JsonSerializerConfigScope())
            {
                var request = new CreateRoomRequest
                {
                    GuestAccess = guestAccess,
                    Name = nameOfRoom,
                    OwnerUserId = ownerUserId,
                    Privacy = privacy
                };

                return CreateRoom(request);
            }
        }

        /// <summary>
        ///  Creates a new room
        /// </summary>
        /// <returns>response containing id and link of the created room</returns>
        /// <remarks>
        /// Auth required with scope 'manage_rooms'. https://api.hipchat.com/v2/room
        /// </remarks>
        public HipchatCreateRoomResponse CreateRoom(CreateRoomRequest request)
        {
            using (JsonSerializerConfigScope())
            {
                if (request.Name.IsEmpty() || request.Name.Length > 50)
                    throw new ArgumentOutOfRangeException("request", "Name of room must be between 1 and 50 characters.");
                try
                {
                    return HipchatEndpoints.CreateRoomEndpoint
                        .AddHipchatAuthentication(_authToken)
                        .PostJsonToUrl(request)
                        .FromJson<HipchatCreateRoomResponse>();
                }
                catch (Exception exception)
                {
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "manage_rooms");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "CreateRoom");
                }
            }
        }
        #endregion

        #region SendNotification

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
        public bool SendNotification(int roomId, string message, RoomColors backgroundColor = RoomColors.Yellow,
            bool notify = false, HipchatMessageFormat messageFormat = HipchatMessageFormat.Html)
        {
            return SendNotification(roomId.ToString(CultureInfo.InvariantCulture), message, backgroundColor, notify, messageFormat);
        }

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
        public bool SendNotification(string roomName, string message, RoomColors backgroundColor = RoomColors.Yellow,
            bool notify = false, HipchatMessageFormat messageFormat = HipchatMessageFormat.Html)
        {
            using (JsonSerializerConfigScope())
            {
                var request = new SendRoomNotificationRequest
                {
                    Color = backgroundColor,
                    Message = message,
                    MessageFormat = messageFormat,
                    Notify = notify
                };

                return SendNotification(roomName, request);
            }
        }

        /// <summary>
        /// Send a message to a room
        /// </summary>
        /// <param name="roomId">The id of the room</param>
        /// <param name="request">The request containing the info about the notification to send</param>
        /// <returns>true if the message successfully sent</returns>
        /// <remarks>
        /// Auth required with scope 'view_group'. https://www.hipchat.com/docs/apiv2/method/send_room_notification
        /// </remarks>
        public bool SendNotification(int roomId, SendRoomNotificationRequest request)
        {
            return SendNotification(roomId.ToString(), request);
        }

        /// <summary>
        /// Send a message to a room
        /// </summary>
        /// <param name="roomName">The id of the room</param>
        /// <param name="request">The request containing the info about the notification to send</param>
        /// <returns>true if the message successfully sent</returns>
        /// <remarks>
        /// Auth required with scope 'view_group'. https://www.hipchat.com/docs/apiv2/method/send_room_notification
        /// </remarks>
        public bool SendNotification(string roomName, SendRoomNotificationRequest request)
        {
            using (JsonSerializerConfigScope())
            {
                if (request.Message.IsEmpty() || request.Message.Length > 10000)
                    throw new ArgumentOutOfRangeException("request",
                        "message length must be between 0 and 10k characters");

                var result = false;
                try
                {
                    HipchatEndpoints.SendNotificationEndpointFormat
                        .Fmt(roomName)
                        .AddHipchatAuthentication(_authToken)
                        .PostJsonToUrl(request, null, x =>
                        {
                            if (x.StatusCode == HttpStatusCode.NoContent)
                                result = true;
                        });
                }
                catch (Exception exception)
                {
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "send_notification");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "SendNotification");
                }
                return result;
            }
        }
        #endregion

        #region ShareFileWithRoom
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
        public bool ShareFileWithRoom(string roomName, string fileFullPath, string message = null)
        {
            using (JsonSerializerConfigScope())
            {
                var request = new ShareFileWithRoomRequest
                {
                    File = fileFullPath,
                    Message = message
                };

                var result = false;
                try
                {
                    var statusCode = HipchatEndpoints.ShareFileWithRoomEndpointFormat
                        .Fmt(roomName)
                        .AddHipchatAuthentication(_authToken)
                        .PostHttpContentToUrl(request.EncodeMultipartRelatedHipchatRequest());

                    if (statusCode.HasValue && statusCode.Value == HttpStatusCode.NoContent)
                        result = true;
                }
                catch (Exception exception)
                {
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "share_file_with_room");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "ShareFileWithRoom");
                }
                return result;
            }
        }
        #endregion

        #region DeleteRoom
        /// <summary>
        /// Delets a room and kicks the current particpants.
        /// </summary>
        /// <param name="roomId">Id of the room.</param>
        /// <returns>true if the room was successfully deleted</returns>
        /// <remarks>
        /// Authentication required with scope 'manage_rooms'. https://www.hipchat.com/docs/apiv2/method/delete_room
        /// </remarks>
        public bool DeleteRoom(int roomId)
        {
            return DeleteRoom(roomId.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Delets a room and kicks the current particpants.
        /// </summary>
        /// <param name="roomName">Name of the room.</param>
        /// <returns>true if the room was successfully deleted</returns>
        /// <remarks>
        /// Authentication required with scope 'manage_rooms'. https://www.hipchat.com/docs/apiv2/method/delete_room
        /// </remarks>
        public bool DeleteRoom(string roomName)
        {
            using (JsonSerializerConfigScope())
            {
                if (roomName.IsEmpty() || roomName.Length > 100)
                    throw new ArgumentOutOfRangeException("roomName", "Valid roomName length is 1-100.");
                var result = false;
                try
                {
                    HipchatEndpoints.DeleteRoomEndpointFormat.Fmt(roomName)
                        .AddHipchatAuthentication(_authToken)
                        .DeleteFromUrl(responseFilter: x =>
                        {
                            if (x.StatusCode == HttpStatusCode.NoContent)
                                result = true;
                        });
                }
                catch (Exception exception)
                {
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "manage_rooms");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "DeleteRoom");
                }
                return result;
            }
        }
        #endregion

        #region GetAllRooms
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
        public HipchatGetAllRoomsResponse GetAllRooms(int startIndex = 0, int maxResults = 100, bool includePrivate = false, bool includeArchived = false)
        {
            using (JsonSerializerConfigScope())
            {
                if (startIndex > 100)
                    throw new ArgumentOutOfRangeException("startIndex", "startIndex must be between 0 and 100");
                if (maxResults > 1000)
                    throw new ArgumentOutOfRangeException("maxResults", "maxResults must be between 0 and 1000");

                try
                {
                    return HipchatEndpoints.GetAllRoomsEndpoint
                        .AddQueryParam("start-index", startIndex)
                        .AddQueryParam("max-results", maxResults)
                        .AddQueryParam("include-private", includePrivate)
                        .AddQueryParam("include-archived", includeArchived)
                        .AddHipchatAuthentication(_authToken)
                        .GetJsonFromUrl()
                        .FromJson<HipchatGetAllRoomsResponse>();
                }
                catch (Exception exception)
                {
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "view_group");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "GetAllRooms");
                }
            }
        }
        #endregion

        #region ViewRoomHistory
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
        public HipchatViewRoomHistoryResponse ViewRoomHistory(string roomName, string date = "recent", string timezone = "UTC", int startIndex = 0, int maxResults = 100, bool reverse = true)
        {
            using (JsonSerializerConfigScope())
            {
                if (roomName.IsEmpty() || roomName.Length > 100)
                    throw new ArgumentOutOfRangeException("roomName", "Valid roomName length is 1-100.");
                if (date.IsEmpty())
                    throw new ArgumentOutOfRangeException("date", "Valid date should be passed.");
                if (timezone.IsEmpty())
                    throw new ArgumentOutOfRangeException("timezone", "Valid timezone should be passed."); 
                if (startIndex > 100)
                    throw new ArgumentOutOfRangeException("startIndex", "startIndex must be between 0 and 100");
                if (maxResults > 1000)
                    throw new ArgumentOutOfRangeException("maxResults", "maxResults must be between 0 and 1000");

                try
                {
                    return HipchatEndpoints.ViewRoomHistoryEndpoint.Fmt(roomName)
                        .AddQueryParam("date", date)
                        .AddQueryParam("timezone", timezone)
                        .AddQueryParam("start-index", startIndex)
                        .AddQueryParam("max-results", maxResults)
                        .AddQueryParam("reverse", reverse)
                        .AddHipchatAuthentication(_authToken)
                        .GetJsonFromUrl()
                        .FromJson<HipchatViewRoomHistoryResponse>();
                }
                catch (Exception exception)
                {
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "view_group");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "ViewRoomHistory");
                }
            }
        }
        #endregion

        #region ViewRecentRoomHistory
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
        public HipchatViewRoomHistoryResponse ViewRecentRoomHistory(string roomName, string notBefore = "", string timezone = "UTC", int startIndex = 0, int maxResults = 100)
        {
            using (JsonSerializerConfigScope())
            {
                if (roomName.IsEmpty() || roomName.Length > 100)
                    throw new ArgumentOutOfRangeException("roomName", "Valid roomName length is 1-100.");
                if (timezone.IsEmpty())
                    throw new ArgumentOutOfRangeException("timezone", "Valid timezone should be passed."); 
                if (startIndex > 100)
                    throw new ArgumentOutOfRangeException("startIndex", "startIndex must be between 0 and 100");
                if (maxResults > 1000)
                    throw new ArgumentOutOfRangeException("maxResults", "maxResults must be between 0 and 1000");

                try
                {
                    return HipchatEndpoints.ViewRecentRoomHistoryEndpoint.Fmt(roomName)
                        .AddQueryParam("not-before", notBefore)
                        .AddQueryParam("timezone", timezone)
                        .AddQueryParam("start-index", startIndex)
                        .AddQueryParam("max-results", maxResults)
                        .AddHipchatAuthentication(_authToken)
                        .GetJsonFromUrl()
                        .FromJson<HipchatViewRoomHistoryResponse>();
                }
                catch (Exception exception)
                {
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "view_messages");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "ViewRecentRoomHistory");
                }
            }
        }
        #endregion
        
        #region GetAllWebhooks
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
        public HipchatGetAllWebhooksResponse GetAllWebhooks(string roomName, int startIndex = 0, int maxResults = 0)
        {
            using (JsonSerializerConfigScope())
            {
                try
                {
                    return HipchatEndpoints.GetAllWebhooksEndpointFormat.Fmt(roomName)
                        .AddQueryParam("start-index", startIndex)
                        .AddQueryParam("max-results", maxResults)
                        .AddHipchatAuthentication(_authToken)
                        .GetJsonFromUrl()
                        .FromJson<HipchatGetAllWebhooksResponse>();
                }
                catch (Exception exception)
                {
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "admin_room");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "GetAllWebhooks");
                }
            }
        }


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
        public HipchatGetAllWebhooksResponse GetAllWebhooks(int roomId, int startIndex = 0, int maxResults = 0)
        {
            return GetAllWebhooks(roomId.ToString(CultureInfo.InvariantCulture), startIndex, maxResults);
        }
        #endregion

        #region DeleteWebhook
        /// <summary>
        /// Deletes a webhook
        /// </summary>
        /// <param name="roomName">The name of the room</param>
        /// <param name="webHookId">The id of the webhook</param>
        /// <returns>true if the request was successful</returns>
        /// <remarks>
        /// Auth required with scope 'admin_room'. https://www.hipchat.com/docs/apiv2/method/delete_webhook
        /// </remarks>
        public bool DeleteWebhook(string roomName, int webHookId)
        {
            using (JsonSerializerConfigScope())
            {
                var result = false;
                try
                {
                    HipchatEndpoints.DeleteWebhookEndpointFormat.Fmt(roomName, webHookId)
                        .AddHipchatAuthentication(_authToken)
                        .DeleteFromUrl(responseFilter: request =>
                        {
                            if (request.StatusCode == HttpStatusCode.NoContent)
                                result = true;
                        });
                }
                catch (Exception exception)
                {
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "admin_room");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "DeleteWebhook");
                }
                return result;
            }
        }

        /// <summary>
        /// Deletes a webhook
        /// </summary>
        /// <param name="roomId">The id of the room</param>
        /// <param name="webHookId">The id of the webhook</param>
        /// <returns>true if the request was successful</returns>
        /// <remarks>
        /// Auth required with scope 'admin_room'. https://www.hipchat.com/docs/apiv2/method/delete_webhook
        /// </remarks>
        public bool DeleteWebhook(int roomId, int webHookId)
        {
            return DeleteWebhook(roomId.ToString(CultureInfo.InvariantCulture), webHookId);
        }
#endregion

        #region SetTopic
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
        public bool SetTopic(string roomName, string topic)
        {
            using (JsonSerializerConfigScope())
            {
                if (topic == null || topic.Length > 250)
                    throw new ArgumentOutOfRangeException("topic", "Valid length is 0 - 250 characters");

                var result = false;
                try
                {
                    HipchatEndpoints.SetTopicEnpdointFormat
                        .Fmt(roomName)
                        .AddHipchatAuthentication(_authToken)
                        .PutJsonToUrl(topic, responseFilter: resp =>
                        {
                            if (resp.StatusCode == HttpStatusCode.NoContent)
                                result = true;
                        });

                }
                catch (Exception exception)
                {
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "admin_room");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "SetTopic");
                }
                return result;
            }
        }

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
        public bool SetTopic(int roomId, string topic)
        {
            return SetTopic(roomId.ToString(CultureInfo.InvariantCulture), topic);
        }

        #endregion

        #region CreateUser
        /// <summary>
        /// Create a new user
        /// </summary>
        /// <returns>A HipchatCreateUserReponse</returns>
        /// <remarks>
        /// Auth required with scope 'admin_group'. https://www.hipchat.com/docs/apiv2/method/create_user
        /// </remarks>
        public HipchatCreateUserResponse CreateUser(CreateUserRequest request)
        {
            using (JsonSerializerConfigScope())
            {
                if (string.IsNullOrEmpty(request.Name) || request.Name.Length > 50)
                {
                    throw new ArgumentOutOfRangeException("name", "Valid length range: 1 - 50.");
                }

                try
                {
                    return HipchatEndpoints.CreateUserEndpointFormat
                        .AddHipchatAuthentication(_authToken)
                        .PostJsonToUrl(request)
                        .FromJson<HipchatCreateUserResponse>();
                } 
                catch (Exception exception) 
                {
                    Console.WriteLine(exception.Message);
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "admin_group");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "CreateUser");
                }
            }
        }
        #endregion

        #region DeleteUser
        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="idOrEmail">The id, email address, or mention name (beginning with an '@') of the user to delete.</param>
        /// <returns>A HipchatDeleteUserReponse</returns>
        /// <remarks>
        /// Auth required with scope 'admin_group'. https://api.hipchat.com/v2/user/{id_or_email}
        /// </remarks>
        public bool DeleteUser(string idOrEmail)
        {
            using (JsonSerializerConfigScope())
            {
                var result = false;
                try 
                {
                    HipchatEndpoints.DeleteUserEndpointFormat.Fmt(idOrEmail)
                        .AddHipchatAuthentication(_authToken)
                        .DeleteFromUrl(responseFilter: x =>
                        {
                            if (x.StatusCode == HttpStatusCode.NoContent)
                                result = true;
                        });
                }
                catch (Exception exception)
                {
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "admin_group");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "DeleteUser");
                }
                return result;
            }
        }
        #endregion

        #region AddMember
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
        public bool AddMember(string roomName, string idOrEmail)
        {
            using (JsonSerializerConfigScope())
            {
                var result = false;
                try
                {
                    HipchatEndpoints.AddMemberEnpdointFormat
                        .Fmt(roomName, idOrEmail)
                        .AddHipchatAuthentication(_authToken)
                        .PutJsonToUrl(null, responseFilter: resp =>
                        {
                            if (resp.StatusCode == HttpStatusCode.NoContent)
                                result = true;
                        });
                }
                catch (Exception exception)
                {
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "admin_room");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "AddMember");
                }
                return result;
            }
        }
        #endregion

        #region UpdatePhoto
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
        public bool UpdatePhoto(string idOrEmail, string photo)
        {
            using (JsonSerializerConfigScope())
            {
                var result = false;
                try
                {                    
                    HipchatEndpoints.UpdatePhotoEnpdointFormat
                        .Fmt(idOrEmail)
                        .AddHipchatAuthentication(_authToken)
                        .PutJsonToUrl(new { photo = photo }, responseFilter: resp =>
                        {
                            if (resp.StatusCode == HttpStatusCode.NoContent)
                                result = true;
                        });

                }
                catch (Exception exception)
                {
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "admin_room");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "UpdatePhoto");
                }
                return result;
            }
        }
        #endregion
    }
}
