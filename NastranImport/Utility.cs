using System.Globalization;

namespace NastranImport
{
    static class Utility
    {
        public static int ToInt(this string text)
        {
            return int.Parse(text, NumberFormatInfo.InvariantInfo);
        }

        public static double ToDouble(this string text)
        {
            return double.Parse(text, NumberFormatInfo.InvariantInfo);
        }
    }
}
