namespace GeoClient.Models
{
    public class RegistrationInfo
    {
        public string Id { get; }

        public string Token { get; }

        public string BaseUrl { get; }

        public RegistrationInfo(string id, string token, string baseUrl)
        {
            Id = id;
            Token = token;
            BaseUrl = baseUrl;
        }

        public string GetMapViewUrl()
        {
            return $"{BaseUrl}/?id={Id}&token={Token}";
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Token)}: {Token}, {nameof(BaseUrl)}: {BaseUrl}";
        }
    }
}