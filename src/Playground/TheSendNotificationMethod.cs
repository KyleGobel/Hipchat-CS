using HipchatApiV2;
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

            Assert.Throws<HipchatAuthenticationException>(() =>
            {
                client.SendNotification("1", "won't work");
            });
        }
    }
}
