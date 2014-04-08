using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Hipchat.Models;
using Hipchat.Models.Requests;
using HipchatApiV2.Exceptions;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Text;
using AuthenticationException = System.Security.Authentication.AuthenticationException;

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
            return HipchatEndpoints.CreateRoomEndpoint(roomId, _authToken).PostJsonToUrl(request,null, CatchErrors);
        }

        private void CatchErrors(HttpWebResponse response)
        {
            if (response.IsErrorResponse())
                throw new ApplicationException("Error in the request");
        }

        private void CatchRequestErrors(HttpWebRequest request)
        {
            if (request.IsErrorResponse())
                throw new ApplicationException("Error in the request");
        }

        /// <summary>
        /// Send a message to a room
        /// </summary>
        /// <param name="roomId">room id to send the message to</param>
        /// <param name="message">message to send</param>
        /// <param name="backgroundColor">the background color of the message, only applicable to html format message</param>
        /// <param name="notify">if the message should notify</param>
        /// <param name="messageFormat">the format of the message</param>
        /// <returns></returns>
        public bool SendMessage(int roomId, string message, RoomColors backgroundColor = RoomColors.Yellow,
            bool notify = false, HipchatMessageFormat messageFormat = HipchatMessageFormat.Html)
        {
            if (message.IsEmpty())
                throw new ArgumentException("Message cannot be blank", "message");
            if (message.Length > 10000)
                throw new ArgumentOutOfRangeException("message", "message length is limited to 10k characters");

            var request = new SendMessageRequest
            {
                Message = message,
                Color = backgroundColor.ToString().ToLower(),
                Notify = notify,
                Message_Format = messageFormat.ToString().ToLower()
            };


            var result = false;
            try
            {
                HipchatEndpoints.SendMessageEndpoint(roomId, _authToken)
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

    public class SendMessageRequest
    {
        public string Message { get; set; }
        public string Color { get; set; }
        public bool Notify { get; set; }
        public string Message_Format { get; set; }
    }
}
