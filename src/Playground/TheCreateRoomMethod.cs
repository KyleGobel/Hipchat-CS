using HipchatApiV2;
using Xunit;

namespace Playground
{
    public class TheCreateRoomMethod
    {
        [Fact]
        public void ThrowsAuthException()
        {
            var client = new HipchatClient("2xv9SIieULT3wxNrmlTU3AtyNtLLFupQFsWFhqry");

            var result = client.GetAllRooms();
            Assert.NotNull(result);
        }
         
    }
}