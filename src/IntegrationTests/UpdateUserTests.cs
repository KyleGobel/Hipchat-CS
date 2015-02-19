using System;
using HipchatApiV2;
using HipchatApiV2.Requests;
using HipchatApiV2.Responses;
using Xunit;

namespace IntegrationTests
{
    [Trait("UpdateUser", "")]
    public class UpdateUserTests : IDisposable
    {
        private readonly HipchatClient _client;
        private const string UserName = "Test User";
        private const string UserEmail = "Test.User@hipchat-cs.com";

        public UpdateUserTests()
        {
            HipchatApiConfig.AuthToken = TestsConfig.AuthToken;
            _client = new HipchatClient();

            try
            {
                _client.GetUserInfo(UserEmail);
            }
            catch (Exception)
            {
                 _client.CreateUser(new CreateUserRequest() {Email = UserEmail, Name = UserName});
            }
        }

        [Fact(DisplayName = "Can update a user")]
        public void CanUpdateUser()
        {
            var request = new UpdateUserRequest() {Email = UserEmail, Name = UserName, MentionName = "NumberOne",
                                                   Title = "President"};
            var result = _client.UpdateUser(UserEmail, request);

            Assert.True(result);

            var userInfo = _client.GetUserInfo(UserEmail);
            Assert.Equal(request.Title, userInfo.Title);
        }

        public void Dispose()
        {
            _client.DeleteUser(UserEmail);
        }
    }
}