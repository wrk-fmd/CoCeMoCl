using GeoClient.Services.Registration;
using NUnit.Framework;

namespace GeoClientTests
{
    [TestFixture]
    public class RegistrationInfoParserTest
    {
        [Test]
        public void ValidUrl_IdFirst()
        {
            string url = "https://geo.server.local/some-path-with-id-and-token?id=1234&token=456&centerMode";

            var registrationInfo = RegistrationInfoParser.ParseRegistrationInfo(url);

            Assert.AreEqual("https://geo.server.local", registrationInfo.BaseUrl);
            Assert.AreEqual("1234", registrationInfo.Id);
            Assert.AreEqual("456", registrationInfo.Token);
        }

        [Test]
        public void ValidUrl_TokenFirst()
        {
            string url = "https://geo.server.local/some-path-with-id-and-token?token=456&id=1234&centerMode";

            var registrationInfo = RegistrationInfoParser.ParseRegistrationInfo(url);

            Assert.AreEqual("https://geo.server.local", registrationInfo.BaseUrl);
            Assert.AreEqual("1234", registrationInfo.Id);
            Assert.AreEqual("456", registrationInfo.Token);
        }

        [Test]
        public void InvalidUrl_IdMissing()
        {
            string url = "https://geo.server.local/some-path-with-id-and-token?token=456&centerMode";

            var registrationInfo = RegistrationInfoParser.ParseRegistrationInfo(url);

            Assert.IsNull(registrationInfo);
        }

        [Test]
        public void InvalidUrl_TokenMissing()
        {
            string url = "https://geo.server.local/some-path-with-id-and-token?id=456&centerMode";

            var registrationInfo = RegistrationInfoParser.ParseRegistrationInfo(url);

            Assert.IsNull(registrationInfo);
        }

        [Test]
        public void InvalidUrl_WrongFormat()
        {
            string url = "https://geo.server.local/id-and-token-are-not-valid";

            var registrationInfo = RegistrationInfoParser.ParseRegistrationInfo(url);

            Assert.IsNull(registrationInfo);
        }

        [Test]
        public void InvalidUrl_NotAnUrl()
        {
            string url = "foobar-garbage-id=.:::what-is-#this?token=??";

            var registrationInfo = RegistrationInfoParser.ParseRegistrationInfo(url);

            Assert.IsNull(registrationInfo);
        }

        [Test]
        public void InvalidUrl_null()
        {
            var registrationInfo = RegistrationInfoParser.ParseRegistrationInfo(null);

            Assert.IsNull(registrationInfo);
        }
    }
}
