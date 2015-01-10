using System.Collections.Generic;
using System.Linq;
using HipchatApiV2;
using HipchatApiV2.Enums;
using Xunit;

namespace IntegrationTests
{
    public class GenerateAuthTokenExample
    {
        //[Fact]
        public void GenerateAuthTokenWithUsernameAndPassword()
        {
            //insert your username and password here
            const string username = "";
            const string password = "";

            var client = new HipchatClient("YourToken");

            var token = client.GenerateToken(GrantType.Password, Enumerable.Empty<TokenScope>(),
                username, password:password);

            Assert.NotNull(token);
        }
    }
}