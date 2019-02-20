using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace GeoClient.Views.Utils
{
    public static class EnumExtension
    {
        public static string GetDescription<T>(this T enumValue) where T : IConvertible
        {
            if (!(enumValue is Enum))
                return "";

            Type enumType = enumValue.GetType();
            Array values = Enum.GetValues(enumType);

            foreach (int val in values)
            {
                if (val != enumValue.ToInt32(CultureInfo.InvariantCulture))
                    continue;

                var memInfo = enumType.GetMember(enumType.GetEnumName(val));
                if (memInfo[0]
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .FirstOrDefault() is DescriptionAttribute descriptionAttribute)
                {
                    return descriptionAttribute.Description;
                }
            }

            return "";
        }
    }
}