using HipchatApiV2;
using HipchatApiV2.Enums;
using Xunit;

namespace IntegrationTests
{
    [Trait("GetAllWebhooks", "")]
    public class GetAllWebhooksTests
    {
        public GetAllWebhooksTests()
        {
            HipchatApiConfig.AuthToken = TestsConfig.AuthToken;
        }

        [Fact(DisplayName = "Webhooks work")]
        public void Webhooks()
        {
            var client = new HipchatClient();

            var createResult = client.CreateWebHook(TestsConfig.ExistingRoomId, "http://myurl.com", "", RoomEvent.RoomNotification,
                "testWebhook");

            Assert.NotNull(createResult);

            var allWebhooks = client.GetAllWebhooks(TestsConfig.ExistingRoomId);

            Assert.NotNull(allWebhooks);

            var response = client.GetAllWebhooks(TestsConfig.ExistingRoomId.ToString());

            Assert.NotNull(response);
            Assert.NotNull(response.Links.Self);

            client.DeleteWebhook(TestsConfig.ExistingRoomId, createResult.Id);
        }
    }
}