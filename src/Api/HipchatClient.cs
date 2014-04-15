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
            JsConfig.ThrowOnDeserializationError = true;
            JsConfig.PropertyConvention = PropertyConvention.Lenient;
            JsConfig<RoomColors>.SerializeFn = colors => colors.ToString().ToLower();
            JsConfig<HipchatMessageFormat>.SerializeFn = format => format.ToString().ToLower();

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
                Grant_Type = grantType,
                Password = password,
                Redirect_Uri = redirectUri,
                Refresh_Token = refreshToken,
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

        /// <summary>
        /// Creates a webhook
        /// </summary>
        /// <param name="roomId">the room Id to create a hook for</param>
        /// <param name="url">the url to post the created events to</param>
        /// <param name="pattern">optional regex pattern</param>
        /// <param name="eventType">the type of <seealso cref="RoomEvent">RoomEvent</seealso></param>
        /// <param name="name">name of the hook</param>
        /// <returns></returns>
        public string CreateWebHook(int roomId, string url, string pattern, RoomEvent eventType, string name)
        {
            var request = new CreateWebHookRequest
            {
                Event = eventType,
                Pattern = pattern,
                Url = url,
                Name = name
            };
            return HipchatEndpoints.CreateWebhookEndpoint(roomId, _authToken).PostJsonToUrl(request);
        }

        /// <summary>
        ///  Creates a new room
        /// </summary>
        /// <param name="nameOfRoom">Name of the room.  Valid Length 1-50</param>
        /// <param name="guestAccess">Whether or not to enable guest access for this room</param>
        /// <param name="ownerUserId">The id, email address, or mention name (beginning with an '@') of
        /// the room's owner.  Defaults to the current user.</param>
        /// <param name="privacy">Whether the room is available for access by other users or not</param>
        ///<returns>response containing id and link of the created room</returns>
        public HipchatCreateRoomResponse CreateRoom(string nameOfRoom, bool guestAccess = false, string ownerUserId = null,
            RoomPrivacy privacy = RoomPrivacy.Public)
        {
            if (nameOfRoom.IsEmpty() || nameOfRoom.Length >50)
                throw new ArgumentOutOfRangeException("nameOfRoom", "Name of room must be between 1 and 50 characters.");

            var request = new CreateRoomRequest
            {
                Guest_Access = guestAccess,
                Name = nameOfRoom,
                Owner_User_Id = ownerUserId,
                Privacy = privacy.ToString().ToLower()
            };

            try
            {
                return HipchatEndpoints.CreateRoomEndpoint(_authToken)
                        .PostJsonToUrl(request)
                        .FromJson<HipchatCreateRoomResponse>();
            }
            catch (Exception exception)
            {
                if (exception is WebException)
                    throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "manage_rooms");

                throw ExceptionHelpers.GeneralExceptionHelper(exception, "CreateRoom");
            }
            return null;
        }

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

        public bool SendNotification(string roomIdOrName, SendRoomNotificationRequest request)
        {
            if (request.Message.IsEmpty() || request.Message.Length > 10000)
                throw new ArgumentOutOfRangeException("request", "message length must be between 0 and 10k characters");

            var result = false;
            try
            {
                HipchatEndpoints.SendNotificationEndpointFormat.Fmt(roomIdOrName)
                    .AddHipchatAuthentication()
                    .PostJsonToUrl(request, null, x =>
                    {
                        if (x.StatusCode == HttpStatusCode.Created)
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

        /// <summary>
        /// List non-archived rooms for this group
        /// </summary>
        /// <param name="startIndex">The start index for the result set</param>
        /// <param name="maxResults">The maximum number of results. Valid length 0-100</param>
        /// <param name="includeArchived">Filter rooms</param>
        /// <returns>A HipchatGetAllRoomsResponse</returns>
        public HipchatGetAllRoomsResponse GetAllRooms(int startIndex = 0, int maxResults = 100, bool includeArchived = false)
        {
            if (startIndex > 100)
                throw new ArgumentOutOfRangeException("startIndex", "startIndex must be between 0 and 100");
            if (maxResults > 100)
                throw new ArgumentOutOfRangeException("maxResults", "maxResults must be between 0 and 100");

            var endpoint = HipchatEndpoints.GetAllRoomsEndpoint
                .AddQueryParam("start-index", startIndex)
                .AddQueryParam("max-results", maxResults)
                .AddQueryParam("include-archived", includeArchived)
                .AddQueryParam("auth_token", _authToken);
            try
            {
                return endpoint.GetJsonFromUrl().FromJson<HipchatGetAllRoomsResponse>();
            }
            catch (Exception exception)
            {
                if (exception is WebException)
                    throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "view_group");

                throw ExceptionHelpers.GeneralExceptionHelper(exception, "GetAllRooms");
            }
            return null;
        }
    }
}
