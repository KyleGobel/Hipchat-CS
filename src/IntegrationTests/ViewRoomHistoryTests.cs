using System;
using System.Linq;
using HipchatApiV2;
using Xunit;

namespace IntegrationTests
{
    [Trait( "ViewRoomHistory", "")]
    public class ViewRoomHistoryTests : IDisposable
    {
        private readonly HipchatClient _client;
        private readonly int _existingRoomId;
        private readonly string _existingRoomName;
        
        public ViewRoomHistoryTests()
        {
            const string roomName = "Test ViewRoomHistory";
            HipchatApiConfig.AuthToken = TestsConfig.AuthToken;
            _client = new HipchatClient();

            var room = _client.CreateRoom(roomName);
            _existingRoomId = room.Id;
            _existingRoomName = "Test ViewRoomHistory";

            // Add to history
            var message = _client.SendNotification(_existingRoomId, "First entry to history");
        }

        [Fact(DisplayName = "Can view full room history")]
        public void CanViewFullRoomHistory()
        {
            var roomHistory = _client.ViewRoomHistory(_existingRoomName);

            Assert.NotNull(roomHistory);
            Assert.Single(roomHistory.Items);
            var historyItem = roomHistory.Items.Single();
            Assert.NotNull(historyItem.Id);
            Assert.NotNull(historyItem.Color);
            Assert.NotNull(historyItem.Date);
            Assert.NotNull(historyItem.From);
            Assert.NotNull(historyItem.Message);
            Assert.NotNull(historyItem.MessageFormat);
            Assert.NotNull(historyItem.Type);
        }

        public void Dispose()
        {
            _client.DeleteRoom(_existingRoomId);
        }
    }
}