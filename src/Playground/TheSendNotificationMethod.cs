using System.Collections.Generic;
using HipchatApiV2;
using HipchatApiV2.Enums;
using HipchatApiV2.Exceptions;
using Xunit;

namespace Playground
{
    [Trait("SendNotification", "")]
    public class TheSendNotificationMethod
    {
           [Fact]
        public void GenerateTokenExample()
        {
            var client = new HipchatClient();

            var token = client.GenerateToken(GrantType.ClientCredentials,
                new List<TokenScope>
                {
                    TokenScope.SendNotification,
                },
                "", /*Auth Id*/
                "" /*Auth Secret*/);

            Assert.NotNull(token);
        }
    }
}
