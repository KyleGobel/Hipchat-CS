using System;
using System.Net;
using Hipchat.Models;
using HipchatApiV2.Exceptions;
using HipchatApiV2.Requests;
using HipchatApiV2.Responses;
using HipchatCS.Models;
using ServiceStack;
using ServiceStack.Text;

namespace HipchatApiV2
{
    public enum RoomPrivacy
    {
        Public,
        Private
    }
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
                return
                    HipchatEndpoints.CreateRoomEndpoint(_authToken)
                        .PostJsonToUrl(request)
                        .FromJson<HipchatCreateRoomResponse>();
            }
            catch (WebException exception)
            {
                if (exception.IsUnauthorized())
                    throw new HipchatAuthenticationException(
                        "Authentication required, this call requires scope 'manage_rooms'.  See https://www.hipchat.com/docs/apiv2/auth",
                        exception);
                throw;
            }
            catch (Exception exception)
            {
                throw new HipchatGeneralException("Exception in CreateRoom method.  See Inner Exception for details.", exception);
            }
        }

         /// <summary>
        /// Send a message to a room
        /// </summary>
        /// <param name="roomIdOrName">The id or name of the room</param>
        /// <param name="message">message to send</param>
        /// <param name="backgroundColor">the background color of the message, only applicable to html format message</param>
        /// <param name="notify">if the message should notify</param>
        /// <param name="messageFormat">the format of the message</param>
        /// <returns>true if the message was sucessfully sent</returns>
        public bool SendNotification(string roomIdOrName, string message, RoomColors backgroundColor = RoomColors.Yellow,
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
                HipchatEndpoints.SendMessageEndpoint(roomIdOrName, _authToken)
                    .PostJsonToUrl(request, null, x =>
                    {
                        if (x.StatusCode == HttpStatusCode.Created)
                            result = true;
                    });
            }
            catch (WebException exception)
            {
                if (exception.IsAny400())
                    throw new HipchatAuthenticationException(
                        "Authentication required, this call requires scope 'send_notification'.  See https://www.hipchat.com/docs/apiv2/auth",
                        exception);
                throw new HipchatGeneralException("Exception making SendMessage call.  See InnerException for details.", exception);
            }
            return result;
        }

    }
}
