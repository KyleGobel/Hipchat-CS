using System;
using HipchatApiV2;
using HipchatApiV2.Enums;
using Xunit;

namespace IntegrationTests
{
    [Trait("Webhooks", "")]
    public class GetAllWebhooksTests : IDisposable
    {
        private readonly int _existingRoomId;
        private readonly HipchatClient _client;
        public GetAllWebhooksTests()
        {
            HipchatApiConfig.AuthToken = TestsConfig.AuthToken;
            _client = new HipchatClient();

            var room = _client.CreateRoom("Test Webhooks Room");
            _existingRoomId = room.Id;

        }

        [Fact(DisplayName = "Webhooks Tests")]
        public void Webhooks()
        {
            var createResult = _client.CreateWebHook(_existingRoomId, "http://myurl.com", "", RoomEvent.RoomNotification,
                "testWebhook");

            Assert.NotNull(createResult);

            var allWebhooks = _client.GetAllWebhooks(_existingRoomId);

            Assert.NotNull(allWebhooks);

            var response = _client.GetAllWebhooks(_existingRoomId.ToString());

            Assert.NotNull(response);
            Assert.NotNull(response.Links.Self);

            _client.DeleteWebhook(_existingRoomId, createResult.Id);
        }

        public void Dispose()
        {
            //apparently delete web hook also just deletes the room already??
            //_client.DeleteRoom(_existingRoomId);
        }
    }
}