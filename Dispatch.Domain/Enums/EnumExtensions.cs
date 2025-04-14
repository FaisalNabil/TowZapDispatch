using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Domain.Enums
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            return value.GetType()
                .GetField(value.ToString())
                ?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                is DescriptionAttribute[] attrs && attrs.Length > 0
                ? attrs[0].Description
                : value.ToString();
        }
    }

}
