using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace SvgMath
{
    /* Minimal effort port of http://sourceforge.net/projects/svgmath/?source=navbar
	 * See also License.txt
	 */

    public class Mml
    {
        public Mml(string mmlFile, string configFile = null)
        {
            m_configFileName = configFile;
            m_mathDocument = XDocument.Load(mmlFile);
        }

        public Mml(XElement mmlContent, string configFile = null)
        {
            m_configFileName = configFile;
            m_mathDocument = new XDocument(mmlContent);
        }

        public XElement MakeSvg()
        {
            // need to enforce point as decimal seperator, otherwise it will fail.
            CultureInfo customCulture = CultureInfo.CurrentCulture.Clone() as CultureInfo;
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            CultureInfo.CurrentCulture = customCulture;

            m_mathConfig = new MathConfig(m_configFileName);
            MathNode currentNode = new MathNode(
                m_mathDocument.Root.Name.LocalName,
                m_mathDocument.Root.Attributes().ToDictionary(kvp => kvp.Name.ToString(), kvp => kvp.Value),
                m_mathConfig,
                null);
            ParseMML(m_mathDocument.Root, currentNode, 0);
            return currentNode.MakeImage();
        }

        private void ParseMML(XElement root, MathNode parentNode, int depth)
        {
            int recDepth = depth + 1;
            foreach (XElement element in root.Elements())
            {
                MathNode mn = new MathNode(
                    element.Name.LocalName,
                    element.Attributes().ToDictionary(kvp => kvp.Name.ToString(), kvp => kvp.Value),
                    m_mathConfig, parentNode);

                element.Nodes()
                    .Where(x => x.NodeType == System.Xml.XmlNodeType.Text || x.NodeType == System.Xml.XmlNodeType.Whitespace)
                    .ToList()
                    .ForEach(x => mn.Text = mn.Text + string.Join(" ", ((XText)x).Value.Split(null)));

                ParseMML(element, mn, recDepth);
            }
        }

        private readonly string m_configFileName;
        private readonly XDocument m_mathDocument;
        private MathConfig m_mathConfig;
    }
}