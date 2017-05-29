using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using HipchatApiV2.Enums;
using HipchatApiV2.Requests;
using HipchatApiV2.Responses;
using ServiceStack;
using ServiceStack.Text;

namespace HipchatApiV2
{
    public class HipchatClient : IHipchatClient
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
			JsConfig<CardStyle>.SerializeFn = cardStyle => cardStyle.ToString().ToLowercaseUnderscore();
			JsConfig<CardFormat>.SerializeFn = cardFormat => cardFormat.ToString().ToLowercaseUnderscore();
			JsConfig<CardAttributeValueStyle>.SerializeFn = cavStyle => cavStyle.ToString().ToLowercaseUnderscore();
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
        public HipchatGetRoomResponse GetRoom(int roomId)
        {
            return GetRoom(roomId.ToString(CultureInfo.InvariantCulture));
        }

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
        public HipchatGetUserInfoResponse GetUserInfo(int userId)
        {
            return GetUserInfo(userId.ToString(CultureInfo.InvariantCulture));
        }
        #endregion

        #region UpdateRoom
        public bool UpdateRoom(int roomId, UpdateRoomRequest request)
        {
            return UpdateRoom(roomId.ToString(CultureInfo.InvariantCulture), request);
        }

    
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

        public bool SendNotification(int roomId, string message, RoomColors backgroundColor = RoomColors.Yellow,
            bool notify = false, HipchatMessageFormat messageFormat = HipchatMessageFormat.Html)
        {
            return SendNotification(roomId.ToString(CultureInfo.InvariantCulture), message, backgroundColor, notify, messageFormat);
        }
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

     
        public bool SendNotification(int roomId, SendRoomNotificationRequest request)
        {
            return SendNotification(roomId.ToString(), request);
        }

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
        public bool DeleteRoom(int roomId)
        {
            return DeleteRoom(roomId.ToString(CultureInfo.InvariantCulture));
        }

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


        public HipchatGetAllWebhooksResponse GetAllWebhooks(int roomId, int startIndex = 0, int maxResults = 0)
        {
            return GetAllWebhooks(roomId.ToString(CultureInfo.InvariantCulture), startIndex, maxResults);
        }
        #endregion

        #region DeleteWebhook
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

       
        public bool DeleteWebhook(int roomId, int webHookId)
        {
            return DeleteWebhook(roomId.ToString(CultureInfo.InvariantCulture), webHookId);
        }
#endregion

        #region SetTopic
   
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
                        .PutJsonToUrl(new {topic}, responseFilter: resp =>
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

        
        public bool SetTopic(int roomId, string topic)
        {
            return SetTopic(roomId.ToString(CultureInfo.InvariantCulture), topic);
        }

        #endregion

        #region CreateUser
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

        #region UpdateUser

        public bool UpdateUser(string idOrEmail, UpdateUserRequest request)
        {
            using (JsonSerializerConfigScope())
            {
                var result = false;
                if (string.IsNullOrEmpty(idOrEmail))
                {
                    throw new ArgumentOutOfRangeException("idOrEmail", "Valid id, email address, or mention name (beginning with an '@') of the user required.");
                }
                if (string.IsNullOrEmpty(request.Name) || request.Name.Length > 50)
                {
                    throw new ArgumentOutOfRangeException("name", "Valid length range: 1 - 50.");
                }
                if (string.IsNullOrEmpty(request.Email))
                {
                    throw new ArgumentOutOfRangeException("email", "Valid email of the user required.");
                }

                try
                {
                    HipchatEndpoints.UpdateUserEndpointFormat
                    .Fmt(idOrEmail)
                    .AddHipchatAuthentication(_authToken)
                    .PutJsonToUrl(data:request, responseFilter: resp =>
                    {
                        if (resp.StatusCode == HttpStatusCode.NoContent)
                            result = true;
                    });
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    if (exception is WebException)
                        throw ExceptionHelpers.WebExceptionHelper(exception as WebException, "admin_group");

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "UpdateUser");
                }

                return result;
            }
        }

        #endregion

        #region DeleteUser

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


        #region ViewPrivateChatHistory
        public HipchatViewRoomHistoryResponse ViewPrivateChatHistory(string user, string date = "recent", string timezone = "UTC", int startIndex = 0, int maxResults = 100, bool reverse = true)
        {
            using (JsonSerializerConfigScope())
            {
                if (user.IsEmpty() || user.Length > 100)
                    throw new ArgumentOutOfRangeException("user", "Valid roomName length is 1-100.");
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
                    return HipchatEndpoints.ViewPrivateChatHistoryEndpoint.Fmt(user)
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

                    throw ExceptionHelpers.GeneralExceptionHelper(exception, "ViewPrivateChatHistory");
                }
            }
        }
        #endregion
    }
}
