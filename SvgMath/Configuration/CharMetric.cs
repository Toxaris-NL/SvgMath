using System.Collections.Generic;

namespace SvgMath
{
    public class CharMetric
    {
        public CharMetric(string charName, List<uint> codes, double width, int height)
        {
            m_charName = charName;
            m_codes = codes;
            m_width = width;
            m_height = height;
        }

        public CharMetric(string charName, List<uint> codes, double width, double[] bbox)
        {
            m_charName = charName;
            m_codes = codes;
            m_width = width;
            m_bbox = bbox;
        }

        public CharMetric(double width)
        {
            m_width = width;
            m_codes = new List<uint>();
        }

        public void SetBBox(double xMin, double yMin, double xMax, double yMax)
        {
            m_bbox = new double[] { xMin, yMin, xMax, yMax };
        }

        private string m_charName;
        private List<uint> m_codes;
        private double m_width;
        private int m_height;
        private double[] m_bbox;

        public double[] BBox {
            get {
                return m_bbox; // xMin, yMin, xMax, yMax
            }
        }

        public string GlyphName {
            get {
                return m_charName;
            }
            set {
                m_charName = value;
            }
        }

        public List<uint> Codes {
            get {
                return m_codes;
            }
        }

        public double Width {
            get {
                return m_width;
            }
        }

        public int Height {
            get {
                return m_height;
            }
        }
    }
}