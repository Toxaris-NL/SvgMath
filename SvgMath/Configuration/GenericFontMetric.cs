using System.Collections.Generic;

namespace SvgMath
{
    public abstract class GenericFontMetric
    {
        public Dictionary<int, CharMetric> CharData;

        protected string m_fontPath;
        public string FontPath { get { return m_fontPath; } }
        protected string m_fontType;
        public string FontType { get { return m_fontType; } set { m_fontType = value; } }
        protected double[] m_boundingBox = { 0F, 0F, 0F, 0F };
        public double[] BoundingBox { get { return m_boundingBox; } set { m_boundingBox = value; } }

        protected string m_fontName;
        public string FontName { get { return m_fontName; } }
        protected string m_fullName;
        public string FullName { get { return m_fullName; } }
        protected string m_family;
        public string Family { get { return m_family; } }
        protected string m_weight;
        public string Weight { get { return m_weight; } }
        protected double m_charWidth;
        public double? CharWidth { get { return m_charWidth; } }
        protected double m_ascender = 0.7;
        public double? Ascender { get { return m_ascender; } }
        protected double m_descender = -0.2;
        public double? Descender { get { return m_descender; } }
        protected double m_xHeight = 0.45;
        public double? XHeight { get { return m_xHeight; } }
        protected double m_capHeight = 0.7;
        public double? CapHeight { get { return m_capHeight; } }
        protected double m_italicAngle = 0;
        public double ItalicAngle { get { return m_italicAngle; } }
        protected double m_undelinePosition;
        public double? UndelinePosition { get { return m_undelinePosition; } }
        protected double? m_underlineThickness;
        public double? UnderlineThickness { get { return m_underlineThickness; } }
        protected double m_axisPosition;

        public double? AxisPosition {
            get {
                return m_axisPosition;
            }
        }

        protected double? m_ruleWidth;

        public double? RuleWidth {
            get {
                return m_ruleWidth;
            }
        }

        protected double m_stdVw = 0.08;

        public double StdVw {
            get {
                return m_stdVw;
            }
        }

        protected double m_vGap;

        public double VGap {
            get {
                return m_vGap;
            }
        }

        protected CharMetric m_missingGlyph;

        public CharMetric MissingGlyph {
            get {
                return m_missingGlyph;
            }
        }
    }
}