using HipchatApiV2;
using Xunit;

namespace IntegrationTests
{
    [Trait( "GetAllUsers", "")]
    public class GetAllUsersTests
    {
        public GetAllUsersTests()
        {
            HipchatApiConfig.AuthToken = TestsConfig.AuthToken;
        }
        [Fact(DisplayName = "Can get all users")]
        public void CanGetAllUsers()
        {
            var client = new HipchatClient();

            var users = client.GetAllUsers();
            
            Assert.NotNull(users);
        }
    }
}