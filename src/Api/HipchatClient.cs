using System;
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
            _authToken = authToken ?? HipchatApiConfig.Instance.AuthToken;
            JsConfig.EmitCamelCaseNames = true;
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
            catch (WebException exception)
            {
                ExceptionHelpers.HandleWebException(exception, "manage_rooms");
            }
            catch (Exception exception)
            {
                ExceptionHelpers.HandleGeneralException(exception, "CreateRoom");
            }
            return null;
        }


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
            return SendNotification(roomId.ToString(), message, backgroundColor, notify, messageFormat);
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
            if (message.IsEmpty())
                throw new ArgumentException("Message cannot be blank", "message");
            if (message.Length > 10000)
                throw new ArgumentOutOfRangeException("message", "message length is limited to 10k characters");

            var request = new SendRoomNotificationRequest
            {
                Message = message,
                Color = backgroundColor.ToString().ToLower(),
                Notify = notify,
                Message_Format = messageFormat.ToString().ToLower()
            };


            var result = false;
            try
            {
                HipchatEndpoints.SendMessageEndpoint(roomName, _authToken)
                    .PostJsonToUrl(request, null, x =>
                    {
                        if (x.StatusCode == HttpStatusCode.Created)
                            result = true;
                    });
            }
            catch (WebException exception)
            {
                ExceptionHelpers.HandleWebException(exception, "send_notification");
            }
            catch (Exception exception)
            {
                ExceptionHelpers.HandleGeneralException(exception, "SendNotification");
            }
            return result;
        }

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
            catch (WebException exception)
            {
                ExceptionHelpers.HandleWebException(exception, "view_group");
            }
            catch (Exception exception)
            {
                ExceptionHelpers.HandleGeneralException(exception, "GetAllRooms");
            }
            return null;
        }
    }
}
