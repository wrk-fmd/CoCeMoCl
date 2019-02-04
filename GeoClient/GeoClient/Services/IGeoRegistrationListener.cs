using System;
using System.Collections.Generic;
using System.Text;

namespace GeoClient.Services
{
    public interface IGeoRegistrationListener
    {
        void GeoServerRegistered();

        void GeoServerUnregistered();
    }
}
