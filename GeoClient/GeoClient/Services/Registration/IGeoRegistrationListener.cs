namespace GeoClient.Services.Registration
{
    public interface IGeoRegistrationListener
    {
        void GeoServerRegistered();

        void GeoServerUnregistered();
    }
}
