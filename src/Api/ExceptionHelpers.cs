using System;
using System.Net;
using HipchatApiV2.Exceptions;
using ServiceStack;

namespace HipchatApiV2
{
    public class ExceptionHelpers
    {
        public static void HandleWebException(WebException exception, string scopeRequired)
        {
            if (exception.IsUnauthorized())
                throw new HipchatAuthenticationException(
                    "Authentication required, this call requires scope '{0}'.  See https://www.hipchat.com/docs/apiv2/auth".Fmt(scopeRequired),
                    exception);
            throw exception;
        }

        public static void HandleGeneralException(Exception exception, string methodName)
        {
            throw new HipchatGeneralException("An unhandled exception was thrown in '{0}'.  See inner exception for details.".Fmt(methodName), exception);
        }
    }
}