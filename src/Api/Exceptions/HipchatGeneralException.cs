using System;

namespace HipchatApiV2.Exceptions
{
    public class HipchatGeneralException : Exception
    {
        public HipchatGeneralException(string message, Exception innerException) : base(message, innerException)
        {
            
        } 
    }
}