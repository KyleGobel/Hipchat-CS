using System;
using System.Runtime.InteropServices;
using HipchatApiV2;
using HipchatApiV2.Requests;
using Xunit;

namespace IntegrationTests
{
    [Trait("SendNotification", "")]
    public class SendRoomNotification : IDisposable
    {
        private readonly int _existingRoomId;
        private readonly HipchatClient _client;
        public SendRoomNotification()
        {
            HipchatApiConfig.AuthToken = TestsConfig.AuthToken;
            _client = new HipchatClient();
            _existingRoomId = _client.CreateRoom("Send Notification Test Room").Id;

        }
        [Fact(DisplayName = "Can send a room notification")]
        public void CanSendRoomNotification()
        {
            var sendMessageResult = _client.SendNotification(_existingRoomId, "Test message");

            Assert.True(sendMessageResult);
        }

        public void Dispose()
        {
            _client.DeleteRoom(_existingRoomId);
        }
    }
}