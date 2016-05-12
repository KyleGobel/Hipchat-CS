using HipchatApiV2;
using Xunit;

namespace IntegrationTests
{
    [Trait("DeleteRooms", "")]
    public class DeleteRoomTests
    {
        public DeleteRoomTests()
        {
            HipchatApiConfig.AuthToken = TestsConfig.AuthToken;
        }
        [Fact(DisplayName = "Can delete a room", Skip = "Setup auth token")]
        public void CanDeleteRoom()
        {
            const string testRoomName = "Delete Me";
            var client = new HipchatClient();
            client.CreateRoom(testRoomName);

            var result = client.DeleteRoom(testRoomName);

            Assert.True(result);
        }
    }
}