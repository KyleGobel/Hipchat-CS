using System;
using System.Security.Cryptography.X509Certificates;
using HipchatApiV2;
using HipchatApiV2.Enums;
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

            var room = _client.CreateRoom(roomName);
            _existingRoomId = room.Id;
            _existingRoomName = "Test GetRooms";
        }

        [Fact(DisplayName = "Can get room details by room name")]
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

        [Fact(DisplayName = "Can get room details by room id")]
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