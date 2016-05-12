using System;
using System.Linq;
using HipchatApiV2;
using Xunit;

namespace IntegrationTests
{
    [Trait( "ViewRecentRoomHistory", "")]
    public class ViewRecentRoomHistoryTests : IDisposable
    {
        private readonly HipchatClient _client;
        private readonly int _existingRoomId;
        private readonly string _existingRoomName;

        public ViewRecentRoomHistoryTests()
        {
            const string roomName = "Test ViewRecentRoomHistory";
            HipchatApiConfig.AuthToken = TestsConfig.AuthToken;
            _client = new HipchatClient();

            _existingRoomId = TestHelpers.GetARoomId(_client, roomName);
            _existingRoomName = roomName;

            // Add notifications to history
            _client.SendNotification(_existingRoomId, "First entry to history");
            _client.ShareFileWithRoom(_existingRoomId.ToString(), @"..\..\Data\RUv8sSn.png", "Second entry to history with file");
        }

        [Fact(DisplayName = "Can view recent room history", Skip = "Setup auth token")]
        public void CanViewRecentRoomHistory()
        {
            var roomHistory = _client.ViewRecentRoomHistory(_existingRoomName);

            Assert.NotNull(roomHistory);
            Assert.Equal(2, roomHistory.Items.Count);
            var historyItem = roomHistory.Items.First();
            Assert.NotNull(historyItem.Id);
            Assert.NotNull(historyItem.Color);
            Assert.NotNull(historyItem.Date);
            Assert.NotNull(historyItem.From);
            Assert.NotNull(historyItem.Message);
            Assert.NotNull(historyItem.MessageFormat);
            Assert.NotNull(historyItem.Type);
            Assert.Null(historyItem.File);

            historyItem = roomHistory.Items.Last();
            Assert.NotNull(historyItem.Id);
            Assert.NotNull(historyItem.Color);
            Assert.NotNull(historyItem.Date);
            Assert.NotNull(historyItem.From);
            Assert.NotNull(historyItem.Message);
            Assert.NotNull(historyItem.MessageFormat);
            Assert.NotNull(historyItem.Type);
            Assert.NotNull(historyItem.File);
        }

        [Fact(DisplayName = "Can view recent room history starting at a passed message", Skip = "Setup auth token")]
        public void CanViewRecentRoomHistoryNotBefore()
        {
            var roomHistory = _client.ViewRecentRoomHistory(_existingRoomName);

            Assert.NotNull(roomHistory);
            Assert.Equal(2, roomHistory.Items.Count);
            var historyItem = roomHistory.Items.Last();
            Assert.NotNull(historyItem.Id);

            // Now request the history starting at this element
            roomHistory = _client.ViewRecentRoomHistory(_existingRoomName, historyItem.Id);
            Assert.Equal(1, roomHistory.Items.Count);
            Assert.Equal(historyItem.Id, roomHistory.Items.Last().Id);
        }

        public void Dispose()
        {
            _client.DeleteRoom(_existingRoomId);
        }
    }
}