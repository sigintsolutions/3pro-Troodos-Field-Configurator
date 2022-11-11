using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3proFieldConfigurator.Tools;

public static class DateTime
{
    public static System.DateTime DateTimeFromUnixTime_Ms(long unixTime_ms)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(unixTime_ms).UtcDateTime;
    }

    public static long DateTimeToUnixTime_ms(System.DateTime dt)
    {
        return ((DateTimeOffset)dt).ToUnixTimeMilliseconds();
    }
}
