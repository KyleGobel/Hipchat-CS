using HipchatApiV2;
using System;
using Xunit;

namespace IntegrationTests
{
    [Trait( "GetEmoticon", "")]
    public class GetEmoticonTests
    {
        public GetEmoticonTests()
        {
            HipchatApiConfig.AuthToken = TestsConfig.AuthToken;
        }

        [Fact(DisplayName = "Can get the emoticon by id")]
        public void CanGetEmoticonById()
        {
            var client = new HipchatClient();
            var emoticons = client.GetAllEmoticons();
            var firstEmoticon = emoticons.Items[0];
            var emoticonById = client.GetEmoticon(firstEmoticon.Id);

            Assert.Equal(firstEmoticon.Id, emoticonById.Id);
            Assert.True(emoticonById.Height > 0);
            Assert.True(emoticonById.Width > 0);
        }

        [Fact(DisplayName = "Can get the emoticon by shortcut")]
        public void CanGetEmoticonByShortcut()
        {
            var client = new HipchatClient();
            var emoticons = client.GetAllEmoticons();
            var firstEmoticon = emoticons.Items[0];
            var emoticonByShortcut = client.GetEmoticon(firstEmoticon.Shortcut);

            Assert.Equal(firstEmoticon.Shortcut, emoticonByShortcut.Shortcut);
            Assert.True(emoticonByShortcut.Height > 0);
            Assert.True(emoticonByShortcut.Width > 0);
        }

        [Fact(DisplayName = "Can get the emoticon by known shortcut")]
        public void CanGetEmoticonByKnownShortcut()
        {
            var client = new HipchatClient();
            var emoticonByShortcut = client.GetEmoticon("allthethings");

            Assert.True(emoticonByShortcut.Height > 0);
            Assert.True(emoticonByShortcut.Width > 0);
        }
    }
}