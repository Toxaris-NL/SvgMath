using System.Globalization;

namespace SvgMath
{
    internal static class StringExtension
    {
        public static double? DoubleOrNull(this string value)
        {
            double dbl;
            bool result = double.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out dbl);
            return result ? (double?)dbl : null;
        }
    }
}