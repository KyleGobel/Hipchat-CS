using System;
using System.Net;

namespace HipchatApiV2.Exceptions
{
    public class HipchatAuthenticationException : WebException
    {
        public HipchatAuthenticationException(string message, Exception innerException)
            : base(message,innerException)
        {
           
        }

    }
}