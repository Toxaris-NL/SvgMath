namespace SvgMath
{
    public class FontMetricRecord
    {
        public FontMetricRecord(string family, GenericFontMetric metric)
        {
            Family = family;
            Metric = metric;
            Used = false;
        }

        public string Family;
        public GenericFontMetric Metric;
        public bool Used;
    }
}