using HipchatApiV2;
using Xunit;

namespace IntegrationTests
{
    [Trait("GetUserInfo", "")]
    public class GetUserInfoTests
    {
        public GetUserInfoTests()
        {
            HipchatApiConfig.AuthToken = TestsConfig.AuthToken;
        }

        [Fact(DisplayName = "Can get user info", Skip = "Requires an actual userId attached to the auth token to pass")]
        public void CanGetUserInfo()
        {
            var userId = 42494;
            var client = new HipchatClient();

            var userInfo = client.GetUserInfo(userId);

            Assert.NotNull(userInfo);
        }

    }  
}