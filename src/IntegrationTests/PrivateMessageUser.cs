using System;
using HipchatApiV2;
using Xunit;

namespace IntegrationTests
{
   [Trait("PrivateMessage", "")]
    public class PrivateMessageUser : IDisposable
    {
        private const string RoomName = "My Test Room";

        public PrivateMessageUser()
        {
            HipchatApiConfig.AuthToken = TestsConfig.AuthToken;
        }
        [Fact(DisplayName = "Can private message a user", Skip = "Setup auth token")]
        public void CanCreateRoom()
        {
            var client = new HipchatClient();

            try
            {
                client.PrivateMessageUser("kgobel@gmail.com", "test message");
            }
            catch
            {
                // they removed does not throw, i don't know better way how to fail an assert
                Assert.True(false);
            }
        }

       public void Dispose()
       {
       }

    }
}