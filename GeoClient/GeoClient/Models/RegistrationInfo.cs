namespace GeoClient.Models
{
    public class RegistrationInfo
    {
        public string Id { get; }

        public string Token { get; }

        public string Url { get; }

        public RegistrationInfo(string id, string token, string url)
        {
            Id = id;
            Token = token;
            Url = url;
        }

        public override string ToString()
        {
            return "RegistrationInfo[Id=" + Id + ", Token=" + Token + "]";
        }
    }
}