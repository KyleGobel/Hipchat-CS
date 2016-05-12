using System;
using HipchatApiV2;
using Xunit;

namespace IntegrationTests
{
    [Trait("CreateRoom", "")]
    public class CreateRoomTests : IDisposable
    {
        private const string RoomName = "My Test Room";

        public CreateRoomTests()
        {
            HipchatApiConfig.AuthToken = TestsConfig.AuthToken;
        }
        [Fact(DisplayName = "Can create a room", Skip="Setup auth token")]
        public void CanCreateRoom()
        {
            IHipchatClient client = new HipchatClient();

            var result = client.CreateRoom(RoomName);

            Assert.NotNull(result);
            Assert.NotNull(result.Links);
            Assert.NotNull(result.Links.Self);
        }

        public void Dispose()
        {
            var client = new HipchatClient();
            client.DeleteRoom(RoomName);
        }
    }
}