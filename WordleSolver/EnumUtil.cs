
// (c) 2021 Kazuki KOHZUKI

using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Wordle
{
    internal static class EnumUtil
    {
        internal static string GetDescription(this Enum value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            if (fieldInfo == null) return value.ToString();

            var attrs = fieldInfo.GetCustomAttributes<DescriptionAttribute>() as DescriptionAttribute[];

            try
            {
                return (attrs?.Length ?? 0) > 0
                         ? attrs.First().Description
                         : value.ToString();
            }
            catch
            {
                return attrs.FirstOrDefault()?.Description ?? value.ToString();
            }
        } // internal static string GetDescription (this Enum)
    } // internal static class EnumUtil
} // namespace Wordle
