using System;
using System.Security.Cryptography.X509Certificates;
using HipchatApiV2;
using HipchatApiV2.Enums;
using HipchatApiV2.Exceptions;
using HipchatApiV2.Requests;
using ServiceStack;
using Xunit;

namespace IntegrationTests
{

    [Trait("GetRoom", "")]
    public class GetRoomTests : IDisposable
    {
        private readonly HipchatClient _client;
        private readonly int _existingRoomId;
        private readonly string _existingRoomName;
        public GetRoomTests()
        {
            const string roomName = "Test GetRooms";
            HipchatApiConfig.AuthToken = TestsConfig.AuthToken;
            _client = new HipchatClient();
            var room = TestHelpers.GetARoomId(_client, roomName);
            _existingRoomId = room;
            _existingRoomName = "Test GetRooms";
        }

        [Fact(DisplayName = "Trying to get a room that doesn't exist throws a RoomNotFound exception", Skip = "Setup auth token")]
        public void GetRoomThrowsException()
        {
            Assert.Throws<HipchatRoomNotFoundException>(() =>_client.GetRoom("this room doesn't exist"));
        }
        [Fact(DisplayName = "Can get room details by room name", Skip = "Setup auth token")]
        public void CanGetRoomByName()
        {
            var result = _client.GetRoom(_existingRoomName);

            Assert.NotNull(result);
            Assert.NotNull(result.XmppJid);
            Assert.NotNull(result.Links);
            Assert.NotNull(result.Name);
            Assert.NotNull(result.Created);
            Assert.NotNull(result.Topic);
            Assert.NotEqual(0, result.Id);
            Assert.NotNull(result.Owner);
        }

        [Fact(DisplayName = "Can get room details by room id", Skip = "Setup auth token")]
        public void CanGetRoomById()
        {
            var result = _client.GetRoom(_existingRoomId);

            Assert.NotNull(result);
            Assert.NotNull(result.XmppJid);
            Assert.NotNull(result.Links);
            Assert.NotNull(result.Name);
            Assert.NotNull(result.Created);
            Assert.NotNull(result.Topic);
            Assert.NotEqual(0, result.Id);
            Assert.NotNull(result.Owner);
        }

        public void Dispose()
        {
            _client.DeleteRoom(_existingRoomId);
        }
    }
}