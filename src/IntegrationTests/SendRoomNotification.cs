using System;
using HipchatApiV2;
using HipchatApiV2.Requests;
using Xunit;

namespace IntegrationTests
{
    [Trait("SendNotification", "")]
    public class SendRoomNotification : IDisposable
    {
        private readonly int _existingRoomId;
        public SendRoomNotification()
        {
            HipchatApiConfig.AuthToken = TestsConfig.AuthToken;
            HipchatClient client = new HipchatClient();

            _existingRoomId = client.CreateRoom("Test Send Message Room").Id;
        }
        [Fact(DisplayName = "Can send a room notification")]
        public void CanSendRoomNotification()
        {
            var client = new HipchatClient();

            var sendMessageResult = client.SendNotification(_existingRoomId, "Test message");

            Assert.True(sendMessageResult);
        }

        public void Dispose()
        {
            HipchatClient client = new HipchatClient();
            client.DeleteRoom(_existingRoomId);
        }
    }
}