using System.Collections.Generic;
using HipchatApiV2;
using HipchatApiV2.Enums;
using Xunit;

namespace IntegrationTests
{
    public class GenerateAuthTokenExample
    {
        //[Fact]
        public void GenerateAuthToken()
        {
            //insert your authId and auth Secret here
            const string authId = "";
            const string authSecret = "";

            var client = new HipchatClient();

            var token = client.GenerateToken(GrantType.ClientCredentials,
                new List<TokenScope> {TokenScope.SendNotification}, authId, authSecret);

            Assert.NotNull(token);
        }
    }
}