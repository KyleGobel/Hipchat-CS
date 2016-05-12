using System;
using HipchatApiV2;
using Xunit;

namespace IntegrationTests
{
    [Trait("Set Room Topic", "")]
    public class SetRoomTopicTests 
    {
        private readonly IHipchatClient _client;
        public SetRoomTopicTests()
        {
            _client = new HipchatClient(TestsConfig.AuthToken);
        }

        [Fact(DisplayName = "Can set the room topic of an existing room", Skip = "Setup auth token")]
        public void CanSetRoomTopicOfExistingRoom()
        {
            var roomId = TestHelpers.GetARoomId(_client, "Test Set Topic Room");
            var result = _client.SetTopic(roomId, "This is the room's topic");
            Assert.True(result);
        }

    }
}