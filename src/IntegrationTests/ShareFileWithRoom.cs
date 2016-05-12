using System;
using System.Runtime.InteropServices;
using HipchatApiV2;
using HipchatApiV2.Requests;
using Xunit;

namespace IntegrationTests
{
    [Trait("ShareFileWithRoom", "")]
    public class ShareFileWithRoom : IDisposable
    {
        private readonly int _existingRoomId;
        private readonly HipchatClient _client;
        public ShareFileWithRoom()
        {

            HipchatApiConfig.AuthToken = TestsConfig.AuthToken;
            _client = new HipchatClient();
            _existingRoomId = TestHelpers.GetARoomId(_client, "Share File With Room Test Room");
        }

        [Fact(DisplayName = "Can share file with room without a message", Skip = "Setup auth token")]
        public void CanShareFileWithRoomWithoutMessage()
        {
            var shareFileWithRoomResult = _client.ShareFileWithRoom(_existingRoomId.ToString(), @"..\..\Data\RUv8sSn.png");

            Assert.True(shareFileWithRoomResult);
        }

        [Fact(DisplayName = "Can share file with room with a message", Skip = "Setup auth token")]
        public void CanShareFileWithRoomWithMessage()
        {
            var shareFileWithRoomResult = _client.ShareFileWithRoom(_existingRoomId.ToString(), @"..\..\Data\RUv8sSn.png", "Uploaded image file.");

            Assert.True(shareFileWithRoomResult);
        }

        public void Dispose()
        {
            _client.DeleteRoom(_existingRoomId);
        }
    }
}