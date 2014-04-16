using HipchatApiV2;
using HipchatApiV2.Requests;
using Xunit;

namespace IntegrationTests
{
    [Trait("SendNotification", "")]
    public class SendRoomNotification
    {
        public SendRoomNotification()
        {
            HipchatApiConfig.AuthToken = TestsConfig.AuthToken;
        }
        [Fact(DisplayName = "Can send a room notification")]
        public void CanSendRoomNotification()
        {
            var client = new HipchatClient();

            var sendMessageResult = client.SendNotification(TestsConfig.ExistingRoomId, "Test message");

            Assert.True(sendMessageResult);
        }
         
    }
}