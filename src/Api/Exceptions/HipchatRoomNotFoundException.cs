using System;

namespace HipchatApiV2.Exceptions
{
    public class HipchatRoomNotFoundException : Exception
    {
        public HipchatRoomNotFoundException(string message, Exception innerException) : base(message, innerException)
        {}
    }
}