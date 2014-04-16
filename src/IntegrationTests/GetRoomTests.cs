using System.Security.Cryptography.X509Certificates;
using HipchatApiV2;
using Xunit;

namespace IntegrationTests
{

    [Trait("GetRoom", "")]
    public class GetRoomTests
    {
        public GetRoomTests()
        {
            HipchatApiConfig.AuthToken = TestsConfig.AuthToken;
        }

        [Fact(DisplayName = "Can get room details by room name")]
        public void CanGetRoomByName()
        {
            //Assumes the room already exists
            const string roomName = "GPS";

            var client = new HipchatClient();
            var result = client.GetRoom(roomName);

            Assert.NotNull(result);
            Assert.NotNull(result.XmppJid);
            Assert.NotNull(result.Links);
            Assert.NotNull(result.Name);
            Assert.NotNull(result.LastActive);
            Assert.NotNull(result.Created);
            Assert.NotNull(result.Topic);
            Assert.NotEqual(0, result.Id);
            Assert.NotNull(result.Owner);
        }


        [Fact(DisplayName = "Can get room details by room id")]
        public void CanGetRoomById()
        {
            const int roomId = 510675;

            var client = new HipchatClient();
            var result = client.GetRoom(roomId);

            Assert.NotNull(result);
            Assert.NotNull(result.XmppJid);
            Assert.NotNull(result.Links);
            Assert.NotNull(result.Name);
            Assert.NotNull(result.LastActive);
            Assert.NotNull(result.Created);
            Assert.NotNull(result.Topic);
            Assert.NotEqual(0, result.Id);
            Assert.NotNull(result.Owner);
        }

    }
}