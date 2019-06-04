using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SvgMath
{
    public class MathConfig
    {
        public MathConfig(string configFile)
        {
            if (configFile == null)
            {
                FileInfo configFileInfo = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "svgmath.xml"));
                if (!configFileInfo.Exists)
                    throw new InvalidOperationException("default configuration file not found");

                configFile = configFileInfo.FullName;
            }

            m_configDoc = XDocument.Load(configFile);

            m_verbose = bool.Parse(m_configDoc.Element("config").Attributes().FirstOrDefault(x => x.Name == "verbose").Value);
            m_configDoc.Descendants("defaults").ToList().ForEach(SetDefaults);
            m_configDoc.Descendants("fallback").ToList().ForEach(SetFallBackFamilies);
            m_configDoc.Descendants("mathvariant").ToList().ForEach(SetMathVariant);
            m_configDoc.Descendants("operator-style").ToList().ForEach(SetOperatorStyle);
            m_configDoc.Descendants("family").ToList().ForEach(SetFontFamily);
        }

        private void SetDefaults(XElement defaultsElement)
        {
            defaultsElement.Attributes().ToList().ForEach(attr => Defaults[attr.Name.LocalName] = attr.Value);
        }

        private void SetFallBackFamilies(XElement fallbackElement)
        {
            FallbackFamilies.AddRange(fallbackElement.Attribute("family").Value.Split(',').Select(x => x.Trim()).ToList());
        }

        private void SetFontFamiliesFromStaticFiles()
        {
            DirectoryInfo fontFolder = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "configuration", "fontdata"));
            foreach (FileInfo fontFile in fontFolder.EnumerateFiles("*.xml"))
            {
                XDocument fontXDoc = XDocument.Load(fontFile.FullName);

                string familyName = fontXDoc.Descendants("fontfamily").FirstOrDefault().Value;
                string weight = fontXDoc.Descendants("weight").FirstOrDefault().Value.ToLower();
                if (weight == null || weight == "regular")
                    weight = "normal";
                //figger style
                double italicangle;
                bool result = double.TryParse(fontXDoc.Descendants("italicangle").FirstOrDefault().Value, out italicangle);
                string style = result && italicangle != 0 ? "italic" : "normal";
                string key = string.Format("{0} {1} {2}", weight, style, string.Join("", familyName.Trim().ToLower().Split(null)));

                if (!m_fontFamilies.ContainsKey(key))
                {
                    m_fontFamilies.Add(key, new FontMetric(fontXDoc));
                }
                else
                {
                    throw new InvalidOperationException(string.Format("{0} already has metric", key));
                }
            }
        }

        private void SetFontFamily(XElement fontFamilyElement)
        {
            string familyName = fontFamilyElement.Attribute("name").Value;
            foreach (XElement font in fontFamilyElement.Descendants("font"))
            {
                string ttf = font.Attribute("ttf")?.Value;
                if (!string.IsNullOrEmpty(ttf))
                {
                    string weight = font.Attribute("weight")?.Value;
                    if (weight == null || weight == "regular")
                        weight = "normal";
                    string style = font.Attribute("style")?.Value ?? "normal";
                    string key = string.Format("{0} {1} {2}", weight, style, string.Join("", familyName.Trim().ToLower().Split(null)));
                    if (!m_fontFamilies.ContainsKey(key))
                    {
                        m_fontFamilies.Add(key, new TrueTypeFont(ttf));
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("{0} already has metric", key));
                    }
                }
            }
        }

        private void SetMathVariant(XElement mathVariant)
        {
            string name = mathVariant.Attribute("name").Value;
            string weight = mathVariant.Attributes("weight").FirstOrDefault() != null ? mathVariant.Attribute("weight").Value : "normal";
            string style = mathVariant.Attributes("style").FirstOrDefault() != null ? mathVariant.Attribute("style").Value : "normal";
            MathVariant mv = new MathVariant(name, weight, style);
            mathVariant.Attribute("family").Value.Split(',').Select(x => x.Trim()).ToList().ForEach(mv.AddFamily);
            Variants.Add(name, mv);
        }

        private void SetOperatorStyle(XElement opElement)
        {
            string name = opElement.Attribute("operator").Value;
            OperatorStyle os = new OperatorStyle(name);
            opElement.Attributes().Where(x => x.Name != "operator").ToList().ForEach(x => os.AddStyle(x.Name.LocalName, x.Value));
            OperatorStyles.Add(os);
        }

        public GenericFontMetric FindFont(string weight, string style, string family)
        {
            if (string.IsNullOrEmpty(weight))
                weight = "normal";
            if (string.IsNullOrEmpty(style))
                style = "normal";
            if (string.IsNullOrEmpty(family))
                family = "";
            weight = weight.Trim();
            style = style.Trim();
            family = string.Join("", family.Trim().ToLower().Split(null));
            foreach (string w in new string[] { weight, "normal" })
            {
                foreach (string s in new string[] { style, "normal" })
                {
                    if (m_fontFamilies.ContainsKey(w + " " + s + " " + family))
                        return m_fontFamilies[w + " " + s + " " + family];
                }
            }
            return null;
        }

        public List<string> FallbackFamilies = new List<string>();
        public Dictionary<string, string> Defaults = MathDefaults.globalDefaults;
        public Dictionary<string, MathVariant> Variants = new Dictionary<string, MathVariant>();
        public List<OperatorStyle> OperatorStyles = new List<OperatorStyle>();
        public MathOperators MathOperators = new MathOperators();
        private readonly XDocument m_configDoc;
        private readonly bool m_verbose;
        private Dictionary<string, GenericFontMetric> m_fontFamilies = new Dictionary<string, GenericFontMetric>();
    }
}