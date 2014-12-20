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
        [Fact(DisplayName = "Can private message a user")]
        public void CanCreateRoom()
        {
            var client = new HipchatClient();

            Assert.DoesNotThrow(() => client.PrivateMessageUser("kgobel@gmail.com", "test message"));
        }

       public void Dispose()
       {
       }

    }
}