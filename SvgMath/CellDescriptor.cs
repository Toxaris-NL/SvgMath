namespace SvgMath
{
    public class CellDescriptor
    {
        public CellDescriptor(MathNode content, string halign, string valign, int colspan, int rowspan)
        {
            Content = content;
            HAlign = halign;
            VAlign = valign;
            ColSpan = colspan;
            RowSpan = rowspan;
        }

        public MathNode Content;
        public string HAlign;
        public string VAlign;
        public int ColSpan;
        public int RowSpan;
        public double VShift;
    }
}