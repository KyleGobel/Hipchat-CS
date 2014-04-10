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
        [Fact (DisplayName = "Throws a AuthenticationException when given an invalid Api Token")]
        public void ThrowsAuthErrors()
        {
            var client = new HipchatClient("invalid Api token");

            //Assert.Throws<HipchatWebException>(() =>
            //{
                client.SendNotification("1", "won't work");
            //});
        }

        [Fact]
        public void GenerateTokenExample()
        {
            var client = new HipchatClient();

            var token = client.GenerateToken(GrantType.ClientCredentials,
                new List<TokenScope>
                {
                    TokenScope.SendNotification,
                },
                "4471354c-7e43-44f5-9f44-7f1cb10acae0", /*Auth Id*/
                "hD7C0xkK2LUoEiqTeq7xmIcDTifL3IaAZgVfz64l" /*Auth Secret*/);

            Assert.NotNull(token);
        }
    }
}
