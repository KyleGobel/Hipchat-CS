using HipchatApiV2;
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

        [Fact(DisplayName = "Can get all webhooks")]
        public void GetAllHooks()
        {
            var client = new HipchatClient();
            var response = client.GetAllWebhooks(TestsConfig.ExistingRoomId.ToString());

            Assert.NotNull(response);
            Assert.NotNull(response.Links.Self);
        }
    }
}