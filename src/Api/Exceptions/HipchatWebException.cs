using System;
using System.Net;

namespace HipchatApiV2.Exceptions
{
    public class HipchatWebException : WebException
    {
        public HipchatWebException(string message, Exception innerException)
            : base(message,innerException)
        {
           
        }

    }
}