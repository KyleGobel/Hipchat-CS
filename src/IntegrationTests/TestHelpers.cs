using System;
using HipchatApiV2;
using HipchatApiV2.Exceptions;

namespace IntegrationTests
{
    public class TestHelpers
    {
        public static int GetARoomId(IHipchatClient client, string roomName)
        {
            try
            {
                var room = client.GetRoom(roomName);
                return room.Id;
            }
            catch (HipchatRoomNotFoundException)
            {
                var room = client.CreateRoom(roomName);
                return room.Id;
            }
        }
    }
}