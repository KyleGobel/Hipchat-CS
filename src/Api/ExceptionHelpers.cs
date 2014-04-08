using System;
using System.IO;
using System.Net;
using HipchatApiV2.Exceptions;
using ServiceStack;

namespace HipchatApiV2
{
    public class ExceptionHelpers
    {
        public static Exception HandleWebException(WebException exception, string scopeRequired)
        {
            if (exception.IsUnauthorized())
                return new HipchatAuthenticationException(
                    "Authentication required, this call requires scope '{0}'.  See https://www.hipchat.com/docs/apiv2/auth".Fmt(scopeRequired),
                    exception);

            var responseText = "";
            try
            {
                var resp = exception.Response;
                using (var stream = resp.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    responseText = reader.ReadToEnd();

                }
                return new WebException("Error calling hipchat service.  Returned Text:\n\n {0}".Fmt(responseText), exception);

            }
            catch (Exception)
            {
                
                throw exception;
            }
            
            return exception;
        }

        public static void HandleGeneralException(Exception exception, string methodName)
        {
            throw new HipchatGeneralException("An unhandled exception was thrown in '{0}'.  See inner exception for details.".Fmt(methodName), exception);
        }
    }
}