using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
            JsConfig.EmitLowercaseUnderscoreNames = true;
            JsConfig.PropertyConvention = PropertyConvention.Lenient;
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
            if (roomName.IsEmpty() || roomName.Length > 100)
                throw new ArgumentOutOfRangeException(roomName, "Valid Lengths of roomName is 1 to 100 characters.");
            try
            {
                return HipchatEndpoints.GetRoomEndpointFormat.Fmt(roomName)
                    .AddHipchatAuthentication()
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
        #endregion
        /// <summary>
        /// Gets an OAuth token for requested grant type. 
        /// </summary>
        /// <param name="grantType">The type of grant request</param>
        /// <param name="scopes">List of scopes that is requested</param>
        /// <param name="basicAuthUsername">If you supply this, the basicAuthUsername and basicAuthPassword will be passed as credentials in BasicAuthentication format.  (Needed to generate a token for an addon)</param>
        /// <param name="basicAuthPassword">If you supply this, the basicAuthUsername and basicAuthPassword will be passed as credentials in BasicAuthentication format. </param>
        /// <param name="username">The user name to generate a token on behalf of.  Only valid in
        /// the 'Password' and 'ClientCredentials' grant types.</param>
        /// <param name="code">The authorization code to exchange for an access token.  Only valid in the 'AuthorizationCode' grant type</param>
        /// <param name="redirectUri">The Url that was used to generate an authorization code, and it must match that value.  Only valid in the 'AuthorizationCode' grant.</param>
        /// <param name="password">The user's password to use for authentication when creating a token.  Only valid in the 'Password' grant.</param>
        /// <param name="refreshToken">The refresh token to use to generate a new access token.  Only valid in the 'RefreshToken' grant.</param>
        public HipchatGenerateTokenResponse GenerateToken(
            GrantType grantType, 
            IEnumerable<TokenScope> scopes,
            string basicAuthUsername = null, 
            string basicAuthPassword = null, 
            string username = null,  
            string code = null, 
            string redirectUri = null, 
            string password = null, 
            string refreshToken = null)
        {
            var request = new GenerateTokenRequest
            {
                Username = username,
                Code = code,
                GrantType = grantType,
                Password = password,
                RedirectUri = redirectUri,
                RefreshToken = refreshToken,
                Scope = string.Join(" ",scopes.Select(x => x.ToString()))
            };

            Action<HttpWebRequest> requestFilter = x => { };
            if (!basicAuthUsername.IsEmpty() && !basicAuthPassword.IsEmpty())
            {
                var auth = string.Format("{0}:{1}", basicAuthUsername, basicAuthPassword);
                var encrypted = Convert.ToBase64String(Encoding.ASCII.GetBytes(auth));
                var creds = string.Format("{0} {1}", "Basic", encrypted);
                requestFilter = x => x.Headers[HttpRequestHeader.Authorization] = creds;
            }



            var endpoint = HipchatEndpoints.GenerateTokenEndpoint;

            var form = request.SerializeAndFormat();
            try
            {
                var response = endpoint
                    .PostToUrl(request.FormEncodeHipchatRequest(),requestFilter: requestFilter)
                    .FromJson<HipchatGenerateTokenResponse>();
                return response;
            }
            catch (Exception exception)
            {
                if (exception is WebException)
                    throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "");

                throw ExceptionHelpers.GeneralExceptionHelper(exception, "GenerateToken");
            }

        }

        #region GetAllUsers

        public HipchatGetAllUsersResponse GetAllUsers(int startIndex = 0, int maxResults = 100, bool includeGuests = false,
            bool includeDeleted = false)
        {
            try
            {
                return HipchatEndpoints.GetAllUsersEndpoint
                    .AddHipchatAuthentication()
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
            var result = false;
            try
            {
             
                HipchatEndpoints.UpdateRoomEndpoingFormat.Fmt(roomName)
                    .AddHipchatAuthentication()
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
            var request = new CreateWebHookRequest
            {
                Event = eventType,
                Pattern = pattern,
                Url = url,
                Name = name
            };
            return CreateWebHook(roomId.ToString(CultureInfo.InvariantCulture), request);
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
            var request = new CreateWebHookRequest
            {
                Event = eventType,
                Pattern = pattern,
                Url = url,
                Name = name
            };
            return CreateWebHook(roomName, request);
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
            try
            {
                return HipchatEndpoints.CreateWebhookEndpointFormat.Fmt(roomName)
                    .AddHipchatAuthentication()
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
            var request = new CreateRoomRequest
            {
                GuestAccess = guestAccess,
                Name = nameOfRoom,
                OwnerUserId = ownerUserId,
                Privacy = privacy 
            };

            return CreateRoom(request);
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
            if (request.Name.IsEmpty() || request.Name.Length >50)
                throw new ArgumentOutOfRangeException("request", "Name of room must be between 1 and 50 characters.");
            try
            {
                return HipchatEndpoints.CreateRoomEndpoint
                    .AddHipchatAuthentication()
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
            var request = new SendRoomNotificationRequest
            {
                Color = backgroundColor,
                Message = message,
                MessageFormat = messageFormat,
                Notify = notify
            };

            return SendNotification(roomName, request);
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
            if (request.Message.IsEmpty() || request.Message.Length > 10000)
                throw new ArgumentOutOfRangeException("request", "message length must be between 0 and 10k characters");

            var result = false;
            try
            {
                HipchatEndpoints.SendNotificationEndpointFormat
                    .Fmt(roomName)
                    .AddHipchatAuthentication()
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
            if (roomName.IsEmpty() || roomName.Length > 100)
                throw new ArgumentOutOfRangeException("roomName", "Valid roomName length is 1-100.");
            var result = false;
            try
            {
                HipchatEndpoints.DeleteRoomEndpointFormat.Fmt(roomName)
                    .AddHipchatAuthentication()
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
        public HipchatGetAllRoomsResponse GetAllRooms(int startIndex = 0, int maxResults = 100, bool includeArchived = false)
        {
            if (startIndex > 100)
                throw new ArgumentOutOfRangeException("startIndex", "startIndex must be between 0 and 100");
            if (maxResults > 100)
                throw new ArgumentOutOfRangeException("maxResults", "maxResults must be between 0 and 100");

            try
            {
                return HipchatEndpoints.GetAllRoomsEndpoint
                    .AddQueryParam("start-index", startIndex)
                    .AddQueryParam("max-results", maxResults)
                    .AddQueryParam("include-archived", includeArchived)
                    .AddHipchatAuthentication()
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
            try
            {
                return HipchatEndpoints.GetAllWebhooksEndpointFormat.Fmt(roomName)
                    .AddQueryParam("start-index", startIndex)
                    .AddQueryParam("max-results", maxResults)
                    .AddHipchatAuthentication()
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
            var result = false;
            try
            {
                HipchatEndpoints.DeleteWebhookEndpointFormat.Fmt(roomName, webHookId)
                    .AddHipchatAuthentication()
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
            if (topic == null || topic.Length > 250)
                throw new ArgumentOutOfRangeException("topic", "Valid length is 0 - 250 characters");

            var result = false;
            try
            {
                HipchatEndpoints.SetTopicEnpdointFormat
                    .Fmt(roomName)
                    .AddHipchatAuthentication()
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
    }
}
