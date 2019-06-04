using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace SvgMath
{
    internal class FontMetric : GenericFontMetric
    {
        public FontMetric()
        {
        }

        public FontMetric(XDocument fontXDoc)
        {
            m_fontName = fontXDoc.Descendants("fontname").FirstOrDefault().Value;
            m_fullName = fontXDoc.Descendants("fullname").FirstOrDefault().Value;
            m_family = fontXDoc.Descendants("fontfamily").FirstOrDefault().Value;
            m_weight = fontXDoc.Descendants("weight").FirstOrDefault().Value;
            m_boundingBox[0] = fontXDoc.Descendants("fontbbox").Where(x => x.Name == "xmin").Select(x => double.Parse(x.Value)).FirstOrDefault();
            m_boundingBox[1] = fontXDoc.Descendants("fontbbox").Where(x => x.Name == "ymin").Select(x => double.Parse(x.Value)).FirstOrDefault();
            m_boundingBox[2] = fontXDoc.Descendants("fontbbox").Where(x => x.Name == "xmax").Select(x => double.Parse(x.Value)).FirstOrDefault();
            m_boundingBox[3] = fontXDoc.Descendants("fontbbox").Where(x => x.Name == "ymax").Select(x => double.Parse(x.Value)).FirstOrDefault();
            m_capHeight = (double)fontXDoc.Descendants("capheight").FirstOrDefault().Value.DoubleOrNull();
            m_xHeight = (double)fontXDoc.Descendants("xheight").FirstOrDefault().Value.DoubleOrNull();
            m_ascender = (double)fontXDoc.Descendants("ascender").FirstOrDefault().Value.DoubleOrNull();
            m_descender = (double)fontXDoc.Descendants("descender").FirstOrDefault().Value.DoubleOrNull();
            m_ruleWidth = fontXDoc.Descendants("rulewidth").FirstOrDefault().Value.DoubleOrNull();
            m_vGap = double.Parse(fontXDoc.Descendants("vgap").FirstOrDefault().Value);
            // m_stdHw = (double) fontXDoc.Descendants("stdhw").FirstOrDefault().Value.DoubleOrNull();
            m_stdVw = (double)fontXDoc.Descendants("stdvw").FirstOrDefault().Value.DoubleOrNull();
            m_undelinePosition = (double)fontXDoc.Descendants("underlineposition").FirstOrDefault().Value.DoubleOrNull();
            m_underlineThickness = (double)fontXDoc.Descendants("underlinethickness").FirstOrDefault().Value.DoubleOrNull();
            m_italicAngle = (double)fontXDoc.Descendants("italicangle").FirstOrDefault().Value.DoubleOrNull();
            m_charWidth = (double)fontXDoc.Descendants("charwidth").FirstOrDefault().Value.DoubleOrNull();
            m_axisPosition = (double)fontXDoc.Descendants("axisposition").FirstOrDefault().Value.DoubleOrNull();

            CharData = fontXDoc.Descendants("char")
                .Select(x => new KeyValuePair<int, CharMetric>(
                    int.Parse(x.Element("charid").Value.Substring(2), NumberStyles.HexNumber),
                    new CharMetric(
                        charName: x.Element("name").Value,
                        codes: x.Descendants("charid").Select(z => (uint)int.Parse(z.Value.Substring(2), NumberStyles.HexNumber)).ToList(),
                        width: double.Parse(x.Element("w").Value),
                        bbox: new double[]
                            {
                                double.Parse(x.Element("b").Element("xmin").Value),
                                double.Parse(x.Element("b").Element("ymin").Value),
                                double.Parse(x.Element("b").Element("xmax").Value),
                                double.Parse(x.Element("b").Element("ymax").Value)
                            }))).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            XElement mg = fontXDoc.Descendants("missingglyph").FirstOrDefault();
            m_missingGlyph = new CharMetric(
                charName: mg.Element("name").Value,
                width: double.Parse(mg.Element("w").Value),
                codes: mg.Descendants("charid").Select(x => (uint)int.Parse(x.Value)).ToList(),
                bbox: new double[]
                    {
                            double.Parse(mg.Element("b").Element("xmin").Value),
                            double.Parse(mg.Element("b").Element("ymin").Value),
                            double.Parse(mg.Element("b").Element("xmax").Value),
                            double.Parse(mg.Element("b").Element("ymax").Value)
                    });
        }
    }
}