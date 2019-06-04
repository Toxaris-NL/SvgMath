using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SvgMath
{
    public class MathNode
    {
        public MathNode(string elementName, Dictionary<string, string> attributes, MathConfig config, MathNode parent)
        {
            ElementName = elementName;
            Config = config;
            Text = "";
            Children = new List<MathNode>();
            Attributes = attributes;
            Parent = parent;
            MetricList = null;
            FontFamilies = new List<string>();
            NominalMetric = null;
            if (parent != null)
            {
                NodeIndex = parent.Children == null ? 0 : parent.Children.Count;
                NodeDefaults = parent.NodeDefaults;
                Parent.Children.Add(this);
            }
            else
            {
                NodeDefaults = MathDefaults.globalDefaults;
                Config.Defaults.Keys.ToList().ForEach(x => NodeDefaults[x] = Config.Defaults[x]);
                NodeIndex = 0;
            }
        }

        public void MakeContext()
        {
            switch (ElementName)
            {
                case "math":
                    MathContext.CMath(this);
                    break;

                case "mstyle":
                    MathContext.CMStyle(this);
                    break;

                case "mtable":
                    MathContext.CMTable(this);
                    break;

                case "mi":
                    MathContext.CMi(this);
                    break;

                case "mo":
                    MathContext.CMo(this);
                    break;

                default:
                    MathContext.DefaultContext(this);
                    break;
            }
        }

        public void MakeChildContext(MathNode child)
        {
            switch (ElementName)
            {
                case "mfrac":
                    MathContext.ChildCMFrac(this, child);
                    break;

                case "mroot":
                    MathContext.ChildCMRoot(this, child);
                    break;

                case "msub":
                    MathContext.ChildCMSub(this, child);
                    break;

                case "msup":
                    MathContext.ChildCMSup(this, child);
                    break;

                case "msubsup":
                    MathContext.ChildCMSubSup(this, child);
                    break;

                case "mmultiscripts":
                    MathContext.ChildCMMultiScripts(this, child);
                    break;

                case "munder":
                    MathContext.ChildCMUnder(this, child);
                    break;

                case "mover":
                    MathContext.ChildCMOver(this, child);
                    break;

                case "munderover":
                    MathContext.ChildCMUnderOver(this, child);
                    break;

                default:
                    MathContext.DefaultChildContext(this, child);
                    break;
            }
        }

        public void Measure()
        {
            MakeContext();
            Children.ForEach(x => x.Measure());
            MeasureNode();
        }

        public void MeasureNode()
        {
            if (m_measurer == null)
                m_measurer = new Measurer(this);

            m_measurer.Measure();
            ///ToDo: Verify
        }

        public XElement MakeImage()
        {
            if (ElementName != "math")
                Console.WriteLine("Root element in MathML document must be 'math'");

            Measure();
            return Generator.DrawImage(this);
        }

        public void Draw(XElement output)
        {
            switch (ElementName)
            {
                case ("math"):
                    Generator.GMath(this, output);
                    break;

                case "mrow":
                    Generator.GMRow(this, output);
                    break;

                case "mphantom":
                    Generator.GMPhantom(this, output);
                    break;

                case "none":
                    Generator.GMNone(this, output);
                    break;

                case "maction":
                    Generator.GMAction(this, output);
                    break;

                case "mprescripts":
                    Generator.GMPrescripts(this, output);
                    break;

                case "mstyle":
                    Generator.GMStyle(this, output);
                    break;

                case "mfenced":
                    Generator.GMFenced(this, output);
                    break;

                case "merror":
                    Generator.GMError(this, output);
                    break;

                case "mpadded":
                    Generator.GMPAdded(this, output);
                    break;

                case "menclose":
                    Generator.GMEnclose(this, output);
                    break;

                case ("mfrac"):
                    Generator.GMFrac(this, output);
                    break;

                case "mo":
                    Generator.GMo(this, output);
                    break;

                case "mi":
                    Generator.GMi(this, output);
                    break;

                case "mn":
                    Generator.GMn(this, output);
                    break;

                case "mtext":
                    Generator.GMText(this, output);
                    break;

                case "ms":
                    Generator.GMs(this, output);
                    break;

                case "mspace":
                    Generator.GMSpace(this, output);
                    break;

                case "msqrt":
                    Generator.GMSqrt(this, output);
                    break;

                case "mroot":
                    Generator.GMRoot(this, output);
                    break;

                case "msub":
                    Generator.GMSub(this, output);
                    break;

                case "msup":
                    Generator.GMSup(this, output);
                    break;

                case "msubsup":
                    Generator.GMSubSup(this, output);
                    break;

                case "mmultiscripts":
                    Generator.GMSubSup(this, output);
                    break;

                case "munder":
                    Generator.GMUnder(this, output);
                    break;

                case "mover":
                    Generator.GMOver(this, output);
                    break;

                case "munderover":
                    Generator.GMUnderOver(this, output);
                    break;

                case "mtd":
                    Generator.GMTd(this, output);
                    break;

                case "mtr":
                    break;

                case "mlabeledtr":
                    break;

                case "mtable":
                    Generator.GMTable(this, output);
                    break;

                default:
                    Console.WriteLine("{0} not found in generator", ElementName);
                    Generator.DefaultDraw(this, output);
                    break;
            }
        }

        public double ParseSpace(string spaceattr, double unitlessScale = 0.75)
        {
            spaceattr = spaceattr.Trim();
            if (spaceattr.EndsWith("mathspace"))
            {
                if (spaceattr.StartsWith("negative"))
                {
                    spaceattr = spaceattr.Substring(8);
                }
                string realspaceattr = "0em";
                if (NodeDefaults.ContainsKey(spaceattr))
                {
                    realspaceattr = NodeDefaults[spaceattr];
                    //throw new InvalidOperationException(string.Format("Bad space token: '{0}'", spaceattr));
                }
                return ParseLength(realspaceattr, unitlessScale);
            }
            return ParseLength(spaceattr, unitlessScale);
        }

        public double ParseLength(string lenattr, double unitlessScale = 0.75)
        {
            lenattr = lenattr.Trim();
            string unit = lenattr.Length > 2 ? lenattr.Substring(lenattr.Length - 2) : "";
            double unitValue = double.Parse(lenattr.Substring(0, lenattr.Length - unit.Length));
            switch (unit)
            {
                case "pt":
                    return unitValue;

                case "mm":
                    return unitValue * 72.0 / 25.4;

                case "cm":
                    return unitValue * 72.0 / 2.54;

                case "in":
                    return unitValue * 72.0;

                case "pc":
                    return unitValue * 12.0;

                case "px":
                    return unitValue * 72.0 / 96.0;

                case "em":
                    return unitValue * FontSize;

                case "ex":
                    return unitValue * FontSize * (double)Metric().XHeight;

                default:
                    return unitValue * unitlessScale;
            }
        }

        private double ParsePercent(string lenattr, double percentBase)
        {
            lenattr = lenattr.Trim();
            double unitValue = double.Parse(lenattr.Substring(0, lenattr.Length - 1));
            return percentBase * unitValue / 100;
        }

        public double ParseLengthOrPercent(string lenattr, double percentBase, double unitlessScale = 0.75)
        {
            lenattr = lenattr.Trim();
            if (lenattr.EndsWith("%"))
                return ParsePercent(lenattr, percentBase);
            return ParseLength(lenattr);
        }

        public double ParseSpaceOrPercent(string lenattr, double percentBase, double unitlessScale = 0.75)
        {
            lenattr = lenattr.Trim();
            if (lenattr.EndsWith("%"))
            {
                return ParsePercent(lenattr, percentBase);
            }
            else
            {
                return ParseLength(lenattr, unitlessScale);
            }
        }

        private GenericFontMetric Metric()
        {
            if (NominalMetric == null)
            {
                NominalMetric = FontPool()[0].Metric;
                foreach (FontMetricRecord fd in MetricList)
                {
                    if (fd.Used)
                    {
                        NominalMetric = fd.Metric;
                        break;
                    }
                }
            }
            return NominalMetric;
        }

        public List<FontMetricRecord> FontPool()
        {
            if (MetricList == null)
            {
                MetricList = FillMetricList(FontFamilies);
                if (MetricList == null)
                {
                    FontFamilies = Config.FallbackFamilies;
                    MetricList = FillMetricList(FontFamilies);
                }
            }
            if (MetricList == null)
                throw new InvalidOperationException("Fatal error: cannot find any font metric for the node; fallback font families misconfiguration");
            return MetricList;
        }

        private List<FontMetricRecord> FillMetricList(List<string> familyList)
        {
            List<FontMetricRecord> metricList = new List<FontMetricRecord>();
            foreach (string family in familyList)
            {
                GenericFontMetric metric = Config.FindFont(FontWeight, FontStyle, family);
                if (metric != null)
                {
                    metricList.Add(new FontMetricRecord(family, metric));
                }
            }
            //Todo: match python implementation
            if (metricList.Count == 0)
                throw new InvalidOperationException(string.Format("Cannot find any font metric for family {0}", string.Join(", ", familyList)));
            return metricList;
        }

        public void ProcessFontAttributes()
        {
            if (Attributes != null && Attributes.ContainsKey("displaystyle"))
            {
                DisplayStyle = (Attributes["displaystyle"] == "true");
            }

            string scriptlevelattr = null;
            if (Attributes != null && Attributes.ContainsKey("scriptlevel"))
            {
                scriptlevelattr = Attributes["scriptlevel"].Trim();
                if (scriptlevelattr.StartsWith("+"))
                {
                    ScriptLevel += int.Parse(scriptlevelattr.Substring(1));
                }
                else if (scriptlevelattr.StartsWith("-"))
                {
                    ScriptLevel -= int.Parse(scriptlevelattr.Substring(1)); ;
                }
                else
                {
                    ScriptLevel = int.Parse(scriptlevelattr);
                }
                ScriptLevel = Math.Max(ScriptLevel, 0);
            }

            if (Attributes != null && Attributes.ContainsKey("mathcolor"))
            {
                Color = Attributes["mathcolor"];
            }
            else if (Attributes != null && Attributes.ContainsKey("color"))
            {
                Color = Attributes["color"];
            }

            // Calculate font attributes
            if (Attributes != null && Attributes.ContainsKey("mathvariant"))
            {
                string mathvariantattr = Attributes["mathvariant"];
                if (!Config.Variants.ContainsKey(mathvariantattr))
                    throw new InvalidOperationException(string.Format("Ignored mathvariant attribute: value '{0}' not defined in the font configuration file", mathvariantattr));
                MathVariant mathvariant = Config.Variants[mathvariantattr];
                FontFamilies = mathvariant.Families;
                FontStyle = mathvariant.Style;
                FontWeight = mathvariant.Weight;
            }
            else
            {
                if (Attributes != null && Attributes.ContainsKey("fontfamily"))
                    FontFamilies = Attributes["fontfamily"].Split(',').Select(x => x.Trim()).ToList();
                if (Attributes != null && Attributes.ContainsKey("fontstyle"))
                    FontStyle = Attributes["fontstyle"];
                if (Attributes != null && Attributes.ContainsKey("fontweight"))
                    FontWeight = Attributes["fontweight"];
            }

            // Calculate font size
            string mathsizeattr = null;
            if (Attributes != null && Attributes.ContainsKey("mathsize"))
            {
                mathsizeattr = Attributes["mathsize"];
                double pLength = ParseLength(NodeDefaults["mathsize"]);
                switch (mathsizeattr)
                {
                    case "normal":
                        MathSize = pLength;
                        break;

                    case "big":
                        MathSize = pLength * 1.41;
                        break;

                    case "small":
                        MathSize = pLength / 1.41;
                        break;

                    default:
                        double mathsize = ParseLengthOrPercent(mathsizeattr, MathSize);
                        if (mathsize > 0)
                        {
                            MathSize = mathsize;
                        }
                        else
                        {
                            throw new InvalidOperationException(string.Format("Value of attribute 'mathsize' ignored - not a positive length: {0}", mathsizeattr));
                        }
                        break;
                }
            }

            FontSize = MathSize;

            if (ScriptLevel > 0)
            {
                double scriptsizemultiplier = double.Parse(NodeDefaults["scriptsizemultiplier"]);
                if (scriptsizemultiplier <= 0)
                    throw new InvalidOperationException(string.Format("Bad inherited value of 'scriptsizemultiplier' attribute: {0}; using default value", mathsizeattr));
                scriptsizemultiplier = double.Parse(MathDefaults.globalDefaults["scriptsizemultiplier"]);
                FontSize *= Math.Pow(scriptsizemultiplier, ScriptLevel);
            }

            if (Attributes != null && Attributes.ContainsKey("fontsize") && mathsizeattr == null)
            {
                string fontsizeattr = Attributes["fontsize"];
                double fontSizeOverride = ParseLengthOrPercent(fontsizeattr, FontSize);
                if (fontSizeOverride > 0)
                {
                    MathSize *= fontSizeOverride / FontSize;
                    FontSize = fontSizeOverride;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Value of attribute 'fontsize' ignored - not a positive length: {0}", fontsizeattr));
                }
            }

            double scriptminsize = ParseLength(NodeDefaults["scriptminsize"]);
            FontSize = Math.Max(FontSize, scriptminsize);
            OriginalFontSize = FontSize;
        }

        public double Axis()
        {
            return (double)Metric().AxisPosition * FontSize;
        }

        public string GetProperty(string key, string defValue = null)
        {
            if (Attributes.ContainsKey(key))
            {
                return Attributes[key];
            }
            else if (NodeDefaults.ContainsKey(key))
            {
                return NodeDefaults[key];
            }
            else
            {
                return defValue;
            }
        }

        public List<string> GetListProperty(string attr, string value = null)
        {
            if (value == null)
                value = GetProperty(attr);
            List<string> splitvalue = value.Split(null).ToList();
            if (splitvalue.Count > 0)
                return splitvalue;
            //ToDo: self.error("Bad value for '%s' attribute: empty list" % attr)
            return NodeDefaults[attr].Split(null).ToList();
        }

        public bool HasGlyph(int ch)
        {
            foreach (FontMetricRecord fdesc in FontPool())
            {
                if (fdesc.Metric.CharData.ContainsKey(ch))
                    return true;
            }
            return false;
        }

        public void MeasureText()
        {
            if (Text.Length == 0)
            {
                IsSpace = true;
                return;
            }

            CharMetric cm0 = null;
            CharMetric cm1 = null;
            CharMetric cm = null;
            int[] ucstext = GetUCSText();
            foreach (int chcode in ucstext)
            {
                object[] chardesc = FindChar(chcode);
                if (chardesc == null)
                {
                    Width += Metric().MissingGlyph.Width;
                }
                else
                {
                    cm = (CharMetric)chardesc[0];
                    FontMetricRecord fd = (FontMetricRecord)chardesc[1];
                    fd.Used = true;
                    if (chcode == ucstext.First())  //ToDo: Verify correct translation
                        cm0 = cm;
                    if (chcode == ucstext.Last()) //ToDo: Verify correct translation
                        cm1 = cm;
                    Width += cm.Width;
                    if (Height + Depth == 0)
                    {
                        Height = cm.BBox[3];
                        Depth = -(cm.BBox[1]);
                    }
                    else if (cm.BBox[3] != cm.BBox[1])
                    {
                        Height = Math.Max(Height, cm.BBox[3]);
                        Depth = Math.Max(Depth, -(cm.BBox[1]));
                    }
                }
            }
            // Normalize to the font size
            Width *= FontSize;
            Depth *= FontSize;
            Height *= FontSize;

            // Add ascender/ descender values
            Ascender = NominalAscender();
            Descender = NominalDescender();

            // Shape correction
            if (cm0 != null)
                LeftBearing = Math.Max(0, -cm0.BBox[0]) * FontSize;
            if (cm1 != null)
                RightBearing = Math.Max(0, cm1.BBox[2] - cm.Width) * FontSize;
            Width += LeftBearing + RightBearing;

            //Reset nominal metric
            NominalMetric = null;
        }

        public object[] FindChar(int ch)
        {
            foreach (FontMetricRecord fd in FontPool())
            {
                if (fd.Metric.CharData.ContainsKey(ch))
                {
                    CharMetric cm = fd.Metric.CharData[ch];
                    return new object[] { cm, fd };
                }
                else
                {
                    if (0 < ch && ch < 0xFFFF && MathDefaults.SpecialChars.ContainsKey(ch))
                    {
                        return FindChar(MathDefaults.SpecialChars[ch]); //ToDo: Verify correct working
                    }
                    return null;// new object[] { null, null };
                }
            }
            return null;// new object[] { null, null }; //ToDo: verify correct working
        }

        private int[] GetUCSText()
        {
            return Text.Select(x => (int)x).ToArray(); //Todo: Verify correct operation
        }

        public double NominalAscender()
        {
            return (double)Metric().Ascender * FontSize;
        }

        public double NominalDescender()
        {
            return -((double)Metric().Descender) * FontSize;
        }

        public double NominalLineWidth()
        {
            return (double)Metric().RuleWidth * FontSize;
        }

        public double NominalLineGap()
        {
            return Metric().VGap * FontSize;
        }

        public double NominalThickStrokeWidth()
        {
            return 0.08 * OriginalFontSize;
        }

        public double NominalThinStrokeWidth()
        {
            return 0.04 * OriginalFontSize;
        }

        public double NominalMediumStrokeWidth()
        {
            return 0.06 * OriginalFontSize;
        }

        public readonly string ElementName;
        public readonly MathConfig Config;
        public string Text;
        public List<MathNode> Children;
        public Dictionary<string, string> Attributes = new Dictionary<string, string>();
        public MathNode Parent;
        public List<FontMetricRecord> MetricList;
        public GenericFontMetric NominalMetric;
        public readonly int NodeIndex;
        public Dictionary<string, string> NodeDefaults;
        private Measurer m_measurer;
        public MathOperator OpDefaults;

        //Context
        public double MathSize;

        public double FontSize;
        public double OriginalFontSize; // save a copy - font size may change in scaling
        public int ScriptLevel;
        public bool TightSpaces;
        public bool DisplayStyle;
        public string Color;
        public List<string> FontFamilies;
        public string FontWeight;
        public string FontStyle;
        public double Width;
        public double Height;
        public double Depth;
        public double Ascender;
        public double Descender;
        public string Scaling; //Todo: this is a MathOperator property. move it to a more better place
        public bool Symmetric;  //Todo: this is a MathOperator property. move it to a more better place
        public double RuleWidth;  //Todo: this is a MathOperator property? move it to a more better place?
        public double RuleGap;  //Todo: this is a MathOperator property? move it to a more better place?
        public double LeftSpace;
        public double RightSpace;
        public double Slope;
        public double LineWidth;
        public double ThickLineWidth;
        public double RootHeight;
        public double RootDepth;
        public double RootWidth;
        public double CornerWidth;
        public double CornerHeight;
        public double Gap;
        public MathNode Base;
        public MathNode Core;
        public MathNode RootIndex;
        public MathNode UnderScript;
        public MathNode OverScript;
        public bool Stretchy;
        public bool Accent;
        public double TextShift;
        public double TextStretch;
        public double LeftBearing;
        public double RightBearing;
        public double LeftPadding;
        public double BorderWidth;
        public MathNode Enumerator;
        public MathNode Denominator;
        public List<ColumnDescriptor> Columns;
        public bool AlignToAxis = false;
        public bool IsSpace = false;
        public List<MathNode> SubScripts;
        public List<MathNode> SuperScripts;
        public List<MathNode> PreSubScripts;
        public List<MathNode> PreSuperScripts;
        public double SubScriptAxis;
        public double SuperScriptAxis;
        public double SubShift;
        public double SuperShift;
        public List<double> PostWidths;
        public List<double> PreWidths;
        public bool MoveLimits;
        public string Decoration;
        public List<bool> DecorationData;
        public double HDelta;
        public double VDelta;
        public List<RowDescriptor> Rows;
        public List<double> FrameSpacings;
        public List<string> FrameLines;
    }
}