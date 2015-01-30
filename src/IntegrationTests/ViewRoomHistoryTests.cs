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

            _existingRoomName = Guid.NewGuid().ToString("N");
            _existingRoomId = TestHelpers.GetARoomId(_client, _existingRoomName);

            // Add notifications to history
            _client.SendNotification(_existingRoomId, "First entry to history");
            _client.ShareFileWithRoom(_existingRoomId.ToString(), @"..\..\Data\RUv8sSn.png", "Second entry to history with file");
        }

        [Fact(DisplayName = "Can view full room history")]
        public void CanViewFullRoomHistory()
        {
            var roomHistory = _client.ViewRoomHistory(_existingRoomName);

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

        public void Dispose()
        {
            _client.DeleteRoom(_existingRoomId);
        }
    }
}