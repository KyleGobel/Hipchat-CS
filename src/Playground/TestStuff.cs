using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HipchatApiV2;
using HipchatApiV2.Exceptions;
using Xunit;

namespace Playground
{
    [Trait("SendMessage", "")]
    public class TheSendMessageMethod
    {
        [Fact (DisplayName = "Throws a AuthenticationException when given an invalid Api Token")]
        public void ThrowsAuthErrors()
        {
            var client = new HipchatClient("invalid Api token");

            Assert.Throws<HipchatAuthenticationException>(() =>
            {
                client.SendMessage(1, "won't work");
            });
        }
    }
}
