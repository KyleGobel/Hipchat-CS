using System;
using System.IO;
using System.Net;
using HipchatApiV2.Exceptions;
using ServiceStack;
using ServiceStack.Text;

namespace HipchatApiV2
{
    public class ExceptionHelpers
    {
        public static Exception WebExceptionHelper(WebException exception, string scopeRequired = null)
        {

            var errorMessage = "";
            var errorType = "";
            try
            {
                var bodyText = exception.GetResponseBody();
                errorMessage = JsonObject.Parse(bodyText).Object("error").Get("message");
                errorType = JsonObject.Parse(bodyText).Object("error").Get("type");
                if (errorMessage.IsEmpty())
                    errorMessage = bodyText;
            }
            catch { }

            if (!errorMessage.IsEmpty())
            {
                switch (exception.GetStatusCode())
                {
                    case 404:
                        if (errorMessage.Contains("Room not found"))
                            return new HipchatRoomNotFoundException(errorMessage, exception);
                        break;
                }
                return new HipchatWebException("\nMessage: '{0}'\nType: '{1}'".Fmt(errorMessage, errorType), exception); 
            }


            if (exception.IsUnauthorized())
            {
                errorMessage = scopeRequired.IsEmpty()
                    ? "Authentication is required.  See https://www.hipchat.com/docs/apiv2/auth"
                    : "Authentication required, this call requires scope '{0}'.  See https://www.hipchat.com/docs/apiv2/auth"
                        .Fmt(scopeRequired);

                return new HipchatWebException(errorMessage,exception);
            }
            return exception;
        }

        public static Exception GeneralExceptionHelper(Exception exception, string methodName)
        {
            var errorMessage = "";
            var errorType = "";
            try
            {
                var bodyText = exception.GetResponseBody();
                errorMessage = JsonObject.Parse(bodyText).Object("error").Get("message");
                errorType = JsonObject.Parse(bodyText).Object("error").Get("type");
            }
            catch { }

            if (!errorMessage.IsEmpty())
            {

                return new HipchatWebException("Message: '{0}'\nType: '{1}'".Fmt(errorMessage, errorType), exception);
            }
            return new HipchatGeneralException("An unhandled exception was thrown in '{0}'.  See inner exception for details.".Fmt(methodName), exception);
        }
    }
    public static class WebResponseHelpers
    {
        /// <summary>
        /// Gets the response text from a web exception
        /// </summary>
        public static string GetResponseText(this WebException exception)
        {
            var resp = exception.Response;
            using (var stream = resp.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static int GetStatusCode(this WebException exception)
        {
            if (exception.Status == WebExceptionStatus.ProtocolError)
            {
                var resp = exception.Response as HttpWebResponse;
                if (resp != null)
                {
                    return (int)resp.StatusCode;
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the response text from a web exception, this will not throw any exceptions.
        /// If there are any, they will be hidden, and a null string will be returned instead
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetResponseTextSafe(this WebException exception)
        {
            try
            {
                var resp = exception.Response;
                using (var stream = resp.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}