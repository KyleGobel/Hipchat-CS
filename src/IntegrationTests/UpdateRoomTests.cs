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
        private readonly int _createdRoomId;
        private readonly HipchatClient _client;
        private readonly HipchatUser _owner;
        public UpdateRoomTests()
        {
            HipchatApiConfig.AuthToken = TestsConfig.AuthToken;
            _client = new HipchatClient();
            var room = _client.CreateRoom("TestUpdateRoom");
            _createdRoomId = room.Id;

            var getRoomResponse = _client.GetRoom(_createdRoomId);
            _owner = getRoomResponse.Owner;
        }

        [Fact(DisplayName = "Can update room name", Skip = "Setup auth token")]
        public void CanUpdateRoom()
        {
            var request = new UpdateRoomRequest
            {
                Name = "My new Room",
                Owner = _owner
            };

            var result = _client.UpdateRoom(_createdRoomId, request);

            Assert.True(result);
        }
        public void Dispose()
        {
            _client.DeleteRoom(_createdRoomId);
        }
    }
}