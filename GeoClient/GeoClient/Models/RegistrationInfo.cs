namespace GeoClient.Models
{
    public class RegistrationInfo
    {
        public string Id { get; }

        public string Token { get; }

        public RegistrationInfo(string id, string token)
        {
            Id = id;
            Token = token;
        }

        public override string ToString()
        {
            return "RegistrationInfo[Id=" + Id + ", Token=" + Token + "]";
        }
    }
}