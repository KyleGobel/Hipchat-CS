using System;
using System.Linq;
using HipchatApiV2;
using HipchatApiV2.Requests;
using HipchatApiV2.Responses;
using Xunit;

namespace IntegrationTests
{
    [Trait("UpdateRoom", "")]
    public class UpdateRoomTests : IDisposable
    {
        private readonly int _existingRoomId;
        private readonly HipchatClient _client;
        private readonly HipchatUser _owner;
        public UpdateRoomTests()
        {
            HipchatApiConfig.AuthToken = TestsConfig.AuthToken;
            _client = new HipchatClient();
             _existingRoomId = TestHelpers.GetARoomId(_client, "TestUpdateRoom");

            var getRoomResponse = _client.GetRoom(_existingRoomId);
            _owner = getRoomResponse.Owner;
        }

        [Fact(DisplayName = "Can update room name")]
        public void CanUpdateRoom()
        {
            var request = new UpdateRoomRequest
            {
                Name = "My new Room",
                Owner = _owner
            };

            var result = _client.UpdateRoom(_existingRoomId, request);

            Assert.True(result);
        }
        public void Dispose()
        {
            _client.DeleteRoom(_existingRoomId);
        }
    }
}