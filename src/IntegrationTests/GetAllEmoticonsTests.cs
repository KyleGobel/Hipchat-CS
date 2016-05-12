using HipchatApiV2;
using System;
using Xunit;

namespace IntegrationTests
{
    [Trait( "GetAllEmoticons", "")]
    public class GetAllEmoticonsTests
    {
        public GetAllEmoticonsTests()
        {
            HipchatApiConfig.AuthToken = TestsConfig.AuthToken;
        }
        [Fact(DisplayName = "Can get all emoticons", Skip = "Setup auth token")]
        public void CanGetAllEmoticons()
        {
            var client = new HipchatClient();

            var emoticons = client.GetAllEmoticons();

            Assert.Equal(100, emoticons.Items.Count);
        }
    }
}