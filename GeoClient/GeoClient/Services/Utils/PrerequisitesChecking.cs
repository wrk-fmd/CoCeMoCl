﻿namespace GeoClient.Services.Utils
{
    public delegate bool BooleanDelegate();

    public static class PrerequisitesChecking
    {
        public static BooleanDelegate IsDataSaverBlockingBackgroundData = () => false;

        public static BooleanDelegate IsDeveloperModeActive = () => false;
    }
}
