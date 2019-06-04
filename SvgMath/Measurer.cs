using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SvgMath
{
    internal class Measurer
    {
        public Measurer(MathNode node)
        {
            m_node = node;
        }

        public void Measure()
        {
            switch (m_node.ElementName)
            {
                case "mprescripts":
                    MPrescripts();
                    break;

                case ("none"):
                    break;

                case "math":
                    MMath();
                    break;

                case "mphantom":
                    MPhantom();
                    break;

                case "mstyle":
                    MStyle();
                    break;

                case "maction":
                    MAction();
                    break;

                case "mpadded":
                    MPAdded();
                    break;

                case "mfenced":
                    MFenced();
                    break;

                case "mo":
                    Mo();
                    break;

                case "mn":
                    Mn();
                    break;

                case "mi":
                    Mi();
                    break;

                case "mtext":
                    MText();
                    break;

                case "merror":
                    MError();
                    break;

                case "ms":
                    Ms();
                    break;

                case "mspace":
                    MSpace();
                    break;

                case "mrow":
                    MRow();
                    break;

                case "mfrac":
                    MFrac();
                    break;

                case "msqrt":
                    MSqrt();
                    break;

                case "mroot":
                    MRoot();
                    break;

                case "msub":
                    MSub();
                    break;

                case "msup":
                    MSup();
                    break;

                case "msubsup":
                    MSubSup();
                    break;

                case "munder":
                    MUnder();
                    break;

                case "mover":
                    MOver();
                    break;

                case "munderover":
                    MUnderOver();
                    break;

                case "mmultiscripts":
                    MMultiScripts();
                    break;

                case "menclose":
                    MEnclose();
                    break;

                case "mtable":
                    MTable();
                    break;

                case "mtr":
                    MTr();
                    break;

                case "mlabeledtr":
                    MLabeledTr();
                    break;

                case "mtd":
                    MTd();
                    break;

                default:
                    throw new InvalidOperationException(string.Format("MathML element '{0}' is unsupported", m_node.ElementName));
            }
        }

        private void MPrescripts()
        { }

        private void MPhantom()
        {
            MRow();
        }

        private void MMath()
        {
            MRow();
        }

        private void MStyle()
        {
            MRow();
        }

        private void MAction()
        {
            string selectionattr = "1";
            if (m_node.Attributes.ContainsKey("selection"))
                selectionattr = m_node.Attributes["selection"];
            int selection = int.Parse(selectionattr);
            m_node.Base = null;
            if (selection <= 0)
            {
                throw new InvalidOperationException(string.Format("Invalid value '{0}' for 'selection' attribute - not a positive integer", selectionattr));
            }
            else if (m_node.Children.Count == 0)
            {
                throw new InvalidOperationException(string.Format("No valid subexpression inside maction element '{0'} - element ignored", selectionattr));
            }
            else
            {
                if (selection > m_node.Children.Count)
                {
                    //throw new InvalidOperationException(string.Format("No valid subexpression inside maction element '{0'} - element ignored", selectionattr));
                    selection = 1;
                }
                SetNodeBase(m_node, m_node.Children[selection - 1]);
                m_node.Width = m_node.Base.Width;
                m_node.Height = m_node.Base.Height;
                m_node.Depth = m_node.Base.Depth;
                m_node.Ascender = m_node.Base.Ascender;
                m_node.Descender = m_node.Base.Descender;
            }
        }

        private void MPAdded()
        {
            CreateImplicitRow(m_node);
            m_node.Height = GetDimension(m_node, "height", m_node.Base.Height, false);
            m_node.Depth = GetDimension(m_node, "depth", m_node.Base.Depth, false);
            m_node.Ascender = m_node.Base.Ascender;
            m_node.Descender = m_node.Base.Descender;
            m_node.LeftPadding = GetDimension(m_node, "lspace", 0, true);
            m_node.Width = GetDimension(m_node, "width", m_node.Base.Width + m_node.LeftPadding, true);
            if (m_node.Width < 0)
                m_node.Width = 0;
            m_node.LeftSpace = m_node.Base.LeftSpace;
            m_node.RightSpace = m_node.Base.RightSpace;
        }

        private void MFenced()
        {
            //Add fences and separators, and process as a mrow
            List<MathNode> old_children = new List<MathNode>(m_node.Children);
            m_node.Children.Clear();

            string openingFence = m_node.GetProperty("open");
            openingFence = string.Join(" ", openingFence.Split(null));
            if (openingFence.Length > 0)
            {
                MathNode opening = new MathNode("mo", new Dictionary<string, string>() { { "fence", "true" }, { "form", "prefix" } }, m_node.Config, m_node)
                {
                    Text = openingFence
                };
                opening.Measure();
            }

            string separators = String.Join("", m_node.GetProperty("separators").Split(null));
            int sepindex = 0;
            int lastsep = separators.Length - 1;

            foreach (MathNode ch in old_children)
            {
                if (m_node.Children.Count > 1 && lastsep >= 0)
                {
                    MathNode sep = new MathNode("mo", new Dictionary<string, string>() { { "separator", "true" }, { "form", "infix" } }, m_node.Config, m_node)
                    {
                        Text = separators[sepindex].ToString()
                    };
                    sep.Measure();
                    sepindex = Math.Min(sepindex + 1, lastsep);
                }
                m_node.Children.Add(ch);
            }

            string closingFence = m_node.GetProperty("close");
            closingFence = String.Join(" ", closingFence.Split(null));
            if (closingFence.Length > 0)
            {
                MathNode closing = new MathNode("mo", new Dictionary<string, string>() { { "fence", "true" }, { "form", "postfix" } }, m_node.Config, m_node)
                {
                    Text = closingFence
                };
                closing.Measure();
            }

            MRow();
        }

        private void Mo()
        {
            // Normalize operator glyphs
            // Use minus instead of hyphen
            if (m_node.HasGlyph(0x2212))
                m_node.Text = m_node.Text.Replace('-', '\u2212');
            // Use prime instead of apostrophe
            if (m_node.HasGlyph(0x2032))
                m_node.Text = m_node.Text.Replace('\'', '\u2032');
            // Invisible operators produce space nodes
            if (Regex.IsMatch(m_node.Text, "(\u2061|\u2062|\u2063)"))
            {
                m_node.IsSpace = true;
            }
            else
            {
                m_node.MeasureText();
            }

            // Align the operator along the mathematical axis for the respective font
            m_node.AlignToAxis = true;
            m_node.TextShift = -m_node.Axis();
            m_node.Height += m_node.TextShift;
            m_node.Ascender += m_node.TextShift;
            m_node.Depth -= m_node.TextShift;
            m_node.Descender -= m_node.TextShift;
        }

        private void Mn()
        {
            m_node.MeasureText();
        }

        private void Mi()
        {
            m_node.MeasureText();
        }

        private void MText()
        {
            m_node.MeasureText();
            double spacing = m_node.ParseSpace("thinmathspace");
            m_node.LeftSpace = spacing;
            m_node.RightSpace = spacing;
        }

        private void MError()
        {
            CreateImplicitRow(m_node);
            m_node.BorderWidth = m_node.NominalLineWidth();
            m_node.Width = m_node.Base.Width + 2 * m_node.BorderWidth;
            m_node.Height = m_node.Base.Height + m_node.BorderWidth;
            m_node.Depth = m_node.Base.Depth + m_node.BorderWidth;
            m_node.Ascender = m_node.Base.Ascender;
            m_node.Descender = m_node.Base.Descender;
        }

        private void Ms()
        {
            string lq = m_node.GetProperty("lquote");
            string rq = m_node.GetProperty("rquote");
            if (lq != null)
                m_node.Text = m_node.Text.Replace(lq, "\\" + lq);
            if (rq != null && rq != lq)
                m_node.Text = m_node.Text.Replace(rq, "\\" + rq);

            m_node.Text = lq + m_node.Text + rq;
            m_node.MeasureText();
            double spacing = m_node.ParseSpace("thinmathspace");
            m_node.LeftSpace = spacing;
            m_node.RightSpace = spacing;
        }

        private void MSpace()
        {
            m_node.Height = m_node.ParseLength(m_node.GetProperty("height"));
            m_node.Depth = m_node.ParseLength(m_node.GetProperty("depth"));
            m_node.Width = m_node.ParseSpace(m_node.GetProperty("width"));

            // Add ascender/descender values
            m_node.Ascender = m_node.NominalAscender();
            m_node.Descender = m_node.NominalDescender();
        }

        private void MRow()
        {
            if (m_node.Children.Count == 0)
                return;

            // Determine alignment type for the row. If there is a non-axis-aligned,
            // non-space child in the row, the whole row is non-axis-aligned. The row
            // that consists of just spaces is considered a space itself
            m_node.AlignToAxis = true;
            m_node.IsSpace = true;
            m_node.Children.ForEach(x =>
            {
                if (!x.IsSpace)
                {
                    m_node.AlignToAxis = m_node.AlignToAxis && x.AlignToAxis;
                    m_node.IsSpace = false;
                }
            });

            // Process non-marking operators
            for (int i = 0; i < m_node.Children.Count; i++)
            {
                MathNode ch = m_node.Children[i];
                if (ch.Core.ElementName != "mo")
                    continue;

                if (ch.Text == "\u02061" || ch.Text == "\u2062" || ch.Text == "\u2063")
                {
                    ch.Text = "";
                    MathNode ch_prev = null;
                    MathNode ch_next = null;
                    if (i > 0)
                        ch_prev = m_node.Children[i - 1];
                    if (i + 1 < m_node.Children.Count)
                        ch_next = m_node.Children[1 + 1];
                    if (LongText(ch_prev) || LongText(ch_next))
                        ch.Width = ch.ParseSpace("thinmathspace");
                }
            }

            // Calculate extent for vertical stretching
            double[] dds = GetVerticalStretchExtent(m_node.Children, m_node.AlignToAxis, m_node.Axis());
            m_node.Ascender = dds[0];
            m_node.Descender = dds[1];

            // Grow sizeable operators
            foreach (MathNode ch in m_node.Children)
            {
                if (ch.Core.Stretchy)
                {
                    double desiredHeight = m_node.Ascender;
                    double desiredDepth = m_node.Descender;
                    if (ch.AlignToAxis && !m_node.AlignToAxis)
                    {
                        desiredHeight -= m_node.Axis();
                        desiredDepth += m_node.Axis();
                    }
                    desiredHeight -= ch.Core.Ascender - ch.Core.Height;
                    desiredDepth -= ch.Core.Descender - ch.Core.Depth;
                    Stretch(ch, null, desiredHeight, desiredDepth, m_node.AlignToAxis);
                }
            }

            // Recalculate height/depth after growing operators
            double[] grhe = GetRowVerticalExtent(m_node.Children, m_node.AlignToAxis, m_node.Axis());
            m_node.Height = grhe[0];
            m_node.Depth = grhe[1];
            m_node.Ascender = grhe[2];
            m_node.Descender = grhe[3];

            // Finally, calculate width and spacings
            foreach (MathNode ch in m_node.Children)
                m_node.Width += ch.Width + ch.LeftSpace + ch.RightSpace;
            m_node.LeftSpace = m_node.Children.First().LeftSpace;
            m_node.RightSpace = m_node.Children.Last().RightSpace;
            m_node.Width -= m_node.LeftSpace + m_node.RightSpace;
        }

        private void MFrac()
        {
            if (m_node.Children.Count != 2)
            {
                //ToDo: Output error node.error("Invalid content of 'mfrac' element: element should have exactly two children")
                if (m_node.Children.Count < 2)
                {
                    MRow();
                    return;
                }
            }
            m_node.Enumerator = m_node.Children[m_node.Children.Count - 2];
            m_node.Denominator = m_node.Children.Last();
            m_node.AlignToAxis = true;

            string widthAttr = m_node.GetProperty("linethickness");
            if (MathDefaults.RuleWidthKeywords.ContainsKey(widthAttr))
                widthAttr = MathDefaults.RuleWidthKeywords[widthAttr];

            double unitWidth = m_node.NominalLineWidth();
            m_node.RuleWidth = m_node.ParseLength(widthAttr, unitWidth);

            m_node.RuleGap = m_node.NominalLineGap();
            if (m_node.TightSpaces)
                m_node.RuleGap /= 1.41; // more compact style if in scripts/limits

            if (m_node.GetProperty("bevelled") == "true")
            {
                double eh = m_node.Enumerator.Height + m_node.Enumerator.Depth;
                double dh = m_node.Denominator.Height + m_node.Denominator.Depth;
                double vshift = Math.Min(eh, dh) / 2;
                m_node.Height = (eh + dh - vshift) / 2;
                m_node.Depth = m_node.Height;

                m_node.Slope = m_defaultSlope;
                m_node.Width = m_node.Enumerator.Width + m_node.Denominator.Width;
                m_node.Width += vshift / m_node.Slope;
                m_node.Width += (m_node.RuleWidth + m_node.RuleGap) * Math.Sqrt(1 + Math.Pow(m_node.Slope, 2)); //ToDo:Verify
                m_node.LeftSpace = m_node.Enumerator.LeftSpace;
                m_node.RightSpace = m_node.Denominator.RightSpace;
            }
            else
            {
                m_node.Height = m_node.RuleWidth / 2 + m_node.RuleGap + m_node.Enumerator.Height + m_node.Enumerator.Depth;
                m_node.Depth = m_node.RuleWidth / 2 + m_node.RuleGap + m_node.Denominator.Height + m_node.Denominator.Depth;
                m_node.Width = Math.Max(m_node.Enumerator.Width, m_node.Denominator.Width) + 2 * m_node.RuleWidth;
                m_node.LeftSpace = m_node.RuleWidth;
                m_node.RightSpace = m_node.RuleWidth;
            }

            m_node.Ascender = m_node.Height;
            m_node.Descender = m_node.Depth;
        }

        private void MSqrt()
        {
            //Create an explicit mrow if there's more than one child
            CreateImplicitRow(m_node);
            AddRadicalEnclosure(m_node);
        }

        private void MRoot()
        {
            if (m_node.Children.Count != 2)
            {
                //ToDo: m_node.error("Invalid content of 'mroot' element: element should have exactly two children")
            }

            if (m_node.Children.Count < 2)
            {
                m_node.RootIndex = null;
                MSqrt();
            }
            else
            {
                SetNodeBase(m_node, m_node.Children[0]);
                m_node.RootIndex = m_node.Children[1];
                AddRadicalEnclosure(m_node);
                m_node.Width += Math.Max(0, m_node.RootIndex.Width - m_node.CornerWidth);
                m_node.Height += Math.Max(0, m_node.RootIndex.Height + m_node.RootIndex.Depth - m_node.CornerHeight);
                m_node.Ascender = m_node.Height;
            }
        }

        private void MSub()
        {
            if (m_node.Children.Count != 2)
            {
                //ToDo: node.error("Invalid content of 'msub' element: element should have exactly two children")
            }
            if (m_node.Children.Count < 2)
            {
                MRow();
                return;
            }
            MeasureScripts(m_node, new List<MathNode>() { m_node.Children[1] }, null);
        }

        private void MSup()
        {
            if (m_node.Children.Count != 2)
            {
                //ToDo: node.error("Invalid content of 'msup' element: element should have exactly two children")
            }
            if (m_node.Children.Count < 2)
            {
                MRow();
                return;
            }
            MeasureScripts(m_node, null, new List<MathNode>() { m_node.Children[1] });
        }

        private void MSubSup()
        {
            if (m_node.Children.Count != 3)
            {
                //ToDo: node.error("Invalid content of 'msubsup' element: element should have exactly three children")
                if (m_node.Children.Count == 2)
                {
                    MSub();
                    return;
                }
                else if (m_node.Children.Count < 2)
                {
                    MRow();
                    return;
                }
            }
            MeasureScripts(m_node, new List<MathNode>() { m_node.Children[1] }, new List<MathNode>() { m_node.Children[2] });
        }

        private void MUnder()
        {
            if (m_node.Children.Count != 2)
            {
                //ToDo: node.error("Invalid content of 'munder' element: element should have exactly two children")
                if (m_node.Children.Count < 2)
                {
                    MRow();
                    return;
                }
            }
            MeasureLimits(m_node, m_node.Children[1], null);
        }

        private void MOver()
        {
            if (m_node.Children.Count != 2)
            {
                //ToDo: node.error("Invalid content of 'mover' element: element should have exactly two children")
                if (m_node.Children.Count < 2)
                {
                    MRow();
                    return;
                }
            }
            MeasureLimits(m_node, null, m_node.Children[1]);
        }

        private void MUnderOver()
        {
            if (m_node.Children.Count != 3)
            {
                //ToDo: node.error("Invalid content of 'munderover' element: element should have exactly three children")
                if (m_node.Children.Count == 2)
                {
                    MUnder();
                    return;
                }
                else if (m_node.Children.Count < 2)
                {
                    MRow();
                    return;
                }
            }
            MeasureLimits(m_node, m_node.Children[1], m_node.Children[2]);
        }

        private void MMultiScripts()
        {
            if (m_node.Children.Count == 0)
            {
                MRow();
                return;
            }
            // Sort children into sub- and superscripts
            List<MathNode> subscripts = new List<MathNode>();
            List<MathNode> superscripts = new List<MathNode>();
            List<MathNode> presubscripts = new List<MathNode>();
            List<MathNode> presuperscripts = new List<MathNode>();

            bool isPre = false;
            bool isSub = true;

            foreach (MathNode ch in m_node.Children.Skip(1)) //ToDo: validate
            {
                if (ch.ElementName == "mprescripts")
                {
                    //ToDo: implement
                    //if (isPre)
                    //node.error("Repeated 'mprescripts' element inside 'mmultiscripts\n")
                    isPre = true;
                    isSub = true;
                    continue;
                }
                if (isSub)
                {
                    if (isPre)
                        presubscripts.Add(ch);
                    else
                        subscripts.Add(ch);
                }
                else
                {
                    if (isPre)
                        presuperscripts.Add(ch);
                    else
                        superscripts.Add(ch);
                }
                isSub = !isSub;
            }
            MeasureScripts(m_node, subscripts, superscripts, presubscripts, presuperscripts);
        }

        private void MEnclose()
        {
            CreateImplicitRow(m_node);
            List<string> signs = m_node.GetProperty("notation").Split(null).ToList();
            m_node.Width = m_node.Base.Width;
            m_node.Height = m_node.Base.Height;
            m_node.Depth = m_node.Base.Depth;
            m_node.Decoration = null;
            m_node.DecorationData = null;
            m_node.BorderWidth = m_node.NominalLineWidth();
            m_node.HDelta = m_node.NominalLineGap() + m_node.BorderWidth;
            m_node.VDelta = m_node.NominalLineGap() + m_node.BorderWidth;

            // Radical sign - convert to msqrt for simplicity
            if (signs.Contains("radical")) //ToDo: Validate
            {
                WrapChildren(m_node, "msqrt");
                SetNodeBase(m_node.Children[0], m_node.Base);
                SetNodeBase(m_node, m_node.Children[0]);
                m_node.Base.MakeContext();
                m_node.Base.MeasureNode();
                m_node.Width = m_node.Base.Width;
                m_node.Height = m_node.Base.Height;
                m_node.Depth = m_node.Base.Depth;
            }
            // Strikes
            HashSet<string> matchedStrikes = new HashSet<string>() { "horizontalstrike", "verticalstrike", "updiagonalstrike", "downdiagonalstrike" };
            if (signs.Any(x => matchedStrikes.Contains(x)))
            {
                PushEnclosure(m_node);
                m_node.Decoration = "strikes";
                m_node.DecorationData = signs.Select(x => matchedStrikes.Contains(x)).ToList();
                // no size change - really?
            }

            // Rounded box
            if (signs.Contains("roundedbox"))
            {
                PushEnclosure(m_node);
                m_node.Decoration = "roundedbox";
                AddBoxEnclosure(m_node);
            }
            // Square box
            if (signs.Contains("box"))
            {
                PushEnclosure(m_node);
                m_node.Decoration = "box";
                AddBoxEnclosure(m_node);
            }

            // Circle
            if (signs.Contains("circle"))
            {
                PushEnclosure(m_node);
                m_node.Decoration = "circle";
                AddCircleEnclosure(m_node);
            }

            // Borders
            List<string> matchedBorders = new List<string>() { "left", "top", "right", "bottom" };
            List<bool> borders = signs.Select(x => matchedBorders.Contains(x)).ToList();
            if (borders.Any(x => x == true))
            {
                PushEnclosure(m_node);
                if (borders.Any(x => x == false))
                {
                    m_node.Decoration = "borders";
                    AddBorderEnclosure(m_node, borders);
                }
                else
                {
                    m_node.Decoration = "box";
                    AddBoxEnclosure(m_node);
                }
            }

            // Long division
            if (signs.Contains("longdiv"))
            {
                PushEnclosure(m_node);
                m_node.Decoration = "borders";
                AddBorderEnclosure(m_node, new List<bool>() { true, false, true, false }); // left top for now
            }

            // Actuarial
            if (signs.Contains("actuarial"))
            {
                PushEnclosure(m_node);
                m_node.Decoration = "borders";
                AddBorderEnclosure(m_node, new List<bool>() { false, true, true, false }); // right top
            }
        }

        private void MTable()
        {
            m_node.LineWidth = m_node.NominalLineWidth();
            ArrangeCells(m_node);
            ArrangeLines(m_node);

            //Calculate column widths
            CalculateColumnWidths(m_node);

            //Expand stretchy operators horizontally
            foreach (RowDescriptor r in m_node.Rows)
            {
                foreach (var item in r.Cells.Select((value, i) => new { i, value }))
                {
                    CellDescriptor c = item.value;
                    if (c == null || c.Content == null)
                        continue;

                    MathNode content = c.Content;
                    if (content.ElementName == "mtd")
                    {
                        if (content.Children.Count != 1)
                            continue;
                        content = content.Children[0];
                        if (content.Core.Stretchy)
                            c.Content = content;
                    }
                    if (content.Core.Stretchy)
                    {
                        if (c.ColSpan == 1)
                        {
                            Stretch(content, m_node.Columns[item.i].Width, null, null);
                        }
                        else
                        {
                            List<ColumnDescriptor> spannedColumns = m_node.Columns.Skip(item.i).Take(item.i + c.ColSpan).ToList();
                            double cellSize = spannedColumns.Select(x => x.Width).Sum();
                            cellSize += spannedColumns.TakeWhile((x, i) => i < spannedColumns.Count).Select(x => x.SpaceAfter).Sum();
                            Stretch(content, cellSize, null, null);
                        }
                    }
                }
            }

            // Calculate row heights
            CalculateRowHeights(m_node);

            // Expand stretchy operators vertically in all cells
            foreach (var item in m_node.Rows.Select((value, i) => new { i, value }))
            {
                RowDescriptor r = item.value;
                foreach (CellDescriptor c in r.Cells)
                {
                    if (c == null || c.Content == null)
                        continue;
                    MathNode content = c.Content;
                    if (content.ElementName == "mtd")
                    {
                        if (content.Children.Count != 1)
                            continue;
                        content = content.Children[0];
                        if (content.Core.Stretchy)
                            c.Content = content;
                    }
                    if (content.Core.Stretchy)
                    {
                        if (c.RowSpan == 1)
                        {
                            Stretch(content, null, r.Height - c.VShift, r.Depth + c.VShift);
                        }
                        else
                        {
                            List<RowDescriptor> spannedRows = m_node.Rows.Skip(item.i).Take(item.i + c.RowSpan).ToList();
                            double cellSize = spannedRows.Select(x => x.Height + x.Depth).Sum();
                            cellSize += spannedRows.TakeWhile((x, i) => i < spannedRows.Count).Select(x => x.SpaceAfter).Sum();
                            Stretch(content, null, cellSize / 2, cellSize / 2);
                        }
                    }
                }
            }

            //Recalculate widths, to account for stretched cells
            CalculateColumnWidths(m_node);

            //Calculate total width of the table
            m_node.Width = m_node.Columns.Select(x => x.Width + x.SpaceAfter).Sum();
            m_node.Width += 2 * m_node.FrameSpacings[0];

            // Calculate total height of the table
            double vsize = m_node.Rows.Select(x => x.Height + x.Depth + x.SpaceAfter).Sum();
            vsize += 2 * m_node.FrameSpacings[1];

            // Calculate alignment point
            object[] aligndata = GetAlign(m_node);
            string alignType = (string)aligndata[0];
            int? alignRow = (int?)aligndata[1];

            double topLine = 0;
            double bottomLine = vsize;
            double axisLine = vsize / 2;
            double baseLine = axisLine + m_node.Axis();

            if (alignRow != null)
            {
                RowDescriptor row = m_node.Rows[(int)alignRow - 1];
                topLine = m_node.FrameSpacings[1];
                m_node.Rows.TakeWhile((x, i) => i < (int)alignRow).ToList().ForEach(x => topLine += x.Height + x.Depth + x.SpaceAfter); //ToDo: verify
                bottomLine = topLine + row.Height + row.Depth;
                if (row.AlignToAxis)
                {
                    axisLine = topLine + row.Height;
                    baseLine = axisLine + m_node.Axis();
                }
                else
                {
                    baseLine = topLine + row.Height;
                    axisLine = baseLine - m_node.Axis();
                }
            }

            if (alignType == "axis")
            {
                m_node.AlignToAxis = true;
                m_node.Height = axisLine;
            }
            else if (alignType == "baseline")
            {
                m_node.AlignToAxis = false;
                m_node.Height = baseLine;
            }
            else if (alignType == "center")
            {
                m_node.AlignToAxis = false;
                m_node.Height = (topLine + bottomLine) / 2;
            }
            else if (alignType == "top")
            {
                m_node.AlignToAxis = false;
                m_node.Height = topLine;
            }
            else if (alignType == "bottom")
            {
                m_node.AlignToAxis = false;
                m_node.Height = bottomLine;
            }
            else
            {
                // ToDo: node.error("Unrecognized or unsupported table alignment value: " + alignType)
                m_node.AlignToAxis = true;
                m_node.Height = axisLine;
            }

            m_node.Depth = vsize - m_node.Height;
            m_node.Ascender = m_node.Height;
            m_node.Descender = m_node.Depth;
        }

        private void MTr()
        {
            // ToDo: implement
            //if (m_node.Parent == null || m_node.Parent.ElementName != "mtable")
            //    node.error("Misplaced '%s' element: should be child of 'mtable'" % node.elementName)
            //pass # all processing is done on the table
        }

        private void MLabeledTr()
        {
            // Strip the label and treat as an ordinary 'mtr'
            if (m_node.Children.Count == 0)
            {
                //ToDo: implement
                //node.error("Missing label in '%s' element" % node.elementName)
            }
            else
            {
                //ToDo: Implement
                //node.warning("MathML element '%s' is unsupported: label omitted" % node.elementName)
                m_node.Children = m_node.Children.Skip(1).ToList();
            }
            MTr();
        }

        private void MTd()
        {
            List<string> parentEls = new List<string>() { "mtr", "mlabeledtr", "mtable" };
            if (m_node.Parent == null || !parentEls.Any(x => x == m_node.Parent.ElementName))
            {
                //ToDo: implement
                // node.error("Misplaced '%s' element: should be child of 'mtr', 'mlabeledtr', or 'mtable'" % node.elementName)
            }
            MRow();
        }

        private bool LongText(MathNode n)
        {
            if (n == null || n.IsSpace == true)
                return false;
            if (n.Core.ElementName == "ms")
                return true;
            if (n.Core.ElementName == "mo" || n.Core.ElementName == "mi" || n.Core.ElementName == "mtext")
                return (n.Core.Text.Length > 1);
            return false;
        }

        private double[] GetVerticalStretchExtent(List<MathNode> descendands, bool rowAlignToAxis, double axis)
        {
            double ascender = 0;
            double descender = 0;
            foreach (MathNode ch in descendands)
            {
                double asc = 0;
                double desc = 0;
                if (ch.Core.Stretchy)
                {
                    asc = ch.Core.Ascender;
                    desc = ch.Core.Descender;
                }
                else
                {
                    asc = ch.Ascender;
                    desc = ch.Descender;
                }

                if (ch.AlignToAxis && !rowAlignToAxis)
                {
                    asc += axis;
                    desc -= axis;
                }
                else if (!ch.AlignToAxis && rowAlignToAxis)
                {
                    double chaxis = ch.Axis();
                    asc -= chaxis;
                    desc += chaxis;
                }
                ascender = Math.Max(asc, ascender);
                descender = Math.Max(desc, descender);
            }
            return new double[] { ascender, descender };
        }

        private void Stretch(MathNode node, double? toWidth, double? toHeight, double? toDepth, bool symmetric = false)
        {
            if (node == null || !node.Core.Stretchy)
                return;

            if (node != node.Base)
            {
                if (toWidth != null)
                    toWidth -= node.Width - node.Base.Width;

                Stretch(node.Base, toWidth, toHeight, toDepth, symmetric);
                node.MeasureNode();
            }
            else if (node.ElementName == "mo")
            {
                if (node.FontSize == 0)
                    return;
                string maxsizedefault = node.OpDefaults.MaxSize;//["maxsize"];
                string maxsizeattr = node.GetProperty("maxsize", maxsizedefault);
                double? maxScale;
                if (maxsizeattr == "infinity")
                {
                    maxScale = null; //Todo: might be invalid. Check original code
                }
                else
                {
                    maxScale = node.ParseSpaceOrPercent(maxsizeattr, node.FontSize, node.FontSize) / node.FontSize;
                }
                string minsizedefault = node.OpDefaults.MinSize;
                string minsizeattr = node.GetProperty("minsize", minsizedefault);
                double minScale = node.ParseSpaceOrPercent(minsizeattr, node.FontSize, node.FontSize) / node.FontSize;
                if (toWidth == null)
                {
                    StretchVertically(node, (double)toHeight, (double)toDepth, minScale, maxScale, symmetric);
                }
                else
                {
                    StretchHorizontally(node, (double)toWidth, minScale, maxScale);
                }
            }
        }

        private void StretchVertically(MathNode node, double toHeight, double toDepth, double? minScale, double? maxScale, bool symmetric)
        {
            if (node.Ascender + node.Descender == 0)
                return;

            if (node.Scaling == "horizontal")
                return;

            if (symmetric && node.Symmetric)
            {
                toHeight = (toHeight + toDepth) / 2;
                toDepth = toHeight;
            }

            double scale = (toHeight + toDepth) / (node.Ascender + node.Descender);
            if (minScale != null)
                scale = Math.Max(scale, (double)minScale);
            if (maxScale != null)
                scale = Math.Min(scale, (double)maxScale);

            node.FontSize *= scale;
            node.Height *= scale;
            node.Depth *= scale;
            node.Ascender *= scale;
            node.Descender *= scale;
            node.TextShift *= scale;

            double extraShift = ((toHeight - node.Ascender) - (toDepth - node.Descender)) / 2;
            node.TextShift += extraShift;
            node.Height += extraShift;
            node.Ascender += extraShift;
            node.Depth -= extraShift;
            node.Descender -= extraShift;

            if (node.Scaling == "vertical")
            {
                node.TextStretch /= scale;
            }
            else
            {
                node.Width *= scale;
                node.LeftBearing *= scale;
                node.RightBearing *= scale;
            }
        }

        private void StretchHorizontally(MathNode node, double toWidth, double minScale, double? maxScale)
        {
            if (node.Width == 0)
                return;

            if (node.Scaling != "horizontal")
                return;

            double scale = toWidth / node.Width;
            scale = Math.Max(scale, minScale);
            if (maxScale != null)
            {
                scale = Math.Min(scale, (double)maxScale);
            }
            node.Width *= scale;
            node.TextStretch *= scale;
            node.LeftBearing *= scale;
            node.RightBearing *= scale;
        }

        private double[] GetRowVerticalExtent(List<MathNode> descendants, bool rowAlignToAxis, double axis)
        {
            double height = 0;
            double depth = 0;
            double ascender = 0;
            double descender = 0;
            foreach (MathNode ch in descendants)
            {
                double h = ch.Height;
                double d = ch.Depth;
                double asc = ch.Ascender;
                double desc = ch.Descender;
                if (ch.AlignToAxis && !rowAlignToAxis)
                {
                    h += axis;
                    asc += axis;
                    d -= axis;
                    desc -= axis;
                }
                else if (!ch.AlignToAxis && rowAlignToAxis)
                {
                    double chaxis = ch.Axis();
                    h -= chaxis;
                    asc -= chaxis;
                    d += chaxis;
                    desc += chaxis;
                }
                height = Math.Max(h, height);
                depth = Math.Max(d, depth);
                ascender = Math.Max(asc, ascender);
                descender = Math.Max(desc, descender);
            }
            return new double[] { height, depth, ascender, descender };
        }

        private void SetNodeBase(MathNode node, MathNode nbase)
        {
            node.Base = nbase;
            node.Core = nbase.Core;
            node.AlignToAxis = nbase.AlignToAxis;
            node.Stretchy = nbase.Stretchy;
        }

        private void WrapChildren(MathNode node, string wrapperElement)
        {
            List<MathNode> old_children = new List<MathNode>(node.Children);
            node.Children.Clear();
            MathNode nbase = new MathNode(wrapperElement, null, node.Config, node)
            {
                Children = old_children
            };
        }

        private void CreateImplicitRow(MathNode node)
        {
            if (node.Children.Count != 1)
            {
                WrapChildren(node, "mrow");
                node.Children[0].MakeContext();
                node.Children[0].MeasureNode();
            }
            SetNodeBase(node, node.Children[0]);
        }

        private double ParseDimension(MathNode node, string attr, double startValue, bool canUseSpaces)
        {
            double baseValue;
            if (attr.EndsWith(" height"))
            {
                baseValue = node.Base.Height;
                attr = attr.Substring(0, attr.Length - 7);
            }
            else if (attr.EndsWith(" depth"))
            {
                baseValue = node.Base.Depth;
                attr = attr.Substring(0, attr.Length - 6);
            }
            else if (attr.EndsWith(" width"))
            {
                baseValue = node.Base.Width;
                attr = attr.Substring(0, attr.Length - 6);
            }
            else
            {
                baseValue = startValue;
            }

            if (attr.EndsWith("%"))
            {
                attr = attr.Substring(0, attr.Length - 1);
                baseValue /= 100.0;
            }

            if (canUseSpaces)
            {
                return node.ParseSpace(attr, baseValue);
            }
            else
            {
                return node.ParseLength(attr, baseValue);
            }
        }

        private double GetDimension(MathNode node, string attName, double startValue, bool canUseSpaces)
        {
            string attr = null;
            if (node.Attributes.ContainsKey(attName))
                attr = node.Attributes[attName];

            if (attr == null)
                return startValue;

            //attr = attr.Split() ToDo: match original code

            if (attr.StartsWith("+"))
            {
                return startValue + ParseDimension(node, attr.Substring(1), startValue, canUseSpaces);
            }
            else if (attr.StartsWith("-")) // ToDo: does not match original code. bug?
            {
                return startValue - ParseDimension(node, attr.Substring(1), startValue, canUseSpaces);
            }
            else
            {
                return ParseDimension(node, attr, startValue, canUseSpaces);
            }
        }

        private void AddRadicalEnclosure(MathNode node)
        {
            // The below is full of heuristics
            node.LineWidth = node.NominalThinStrokeWidth();
            node.ThickLineWidth = node.NominalThickStrokeWidth();
            node.Gap = node.NominalLineGap();
            if (!node.DisplayStyle)
                node.Gap /= 2; // more compact style if inline

            node.RootHeight = Math.Max(node.Base.Height, node.Base.Ascender);
            node.RootHeight = Math.Max(node.RootHeight, node.NominalAscender());
            node.RootHeight += node.Gap + node.LineWidth;
            node.Height = node.RootHeight;

            node.AlignToAxis = node.Base.AlignToAxis;
            // Root extends to baseline for elements aligned on the baseline,
            // and to the bottom for elements aligned on the axis. An extra
            // line width is added to the depth, to account for radical sign
            // protruding below the baseline.
            if (node.AlignToAxis)
            {
                node.RootDepth = Math.Max(0, node.Base.Depth - node.LineWidth);
                node.Depth = Math.Max(node.Base.Depth, node.RootDepth + node.LineWidth);
            }
            else
            {
                node.RootDepth = 0;
                node.Depth = Math.Max(node.Base.Depth, node.LineWidth);
            }
            node.RootWidth = (node.RootHeight + node.RootDepth) * 0.6;
            node.CornerWidth = node.RootWidth * 0.9 - node.Gap - node.LineWidth / 2;
            node.CornerHeight = (node.RootHeight + node.RootDepth) * 0.5 - node.Gap - node.LineWidth / 2;

            node.Width = node.Base.Width + node.RootWidth + 2 * node.Gap;
            node.Ascender = node.Height;
            node.Descender = node.Base.Descender;
            node.LeftSpace = node.LineWidth;
            node.RightSpace = node.LineWidth;
        }

        private void MeasureScripts(MathNode node, List<MathNode> subscripts, List<MathNode> superscripts, List<MathNode> presubscripts = null, List<MathNode> presuperscripts = null)
        {
            node.SubScripts = subscripts == null ? new List<MathNode>() : subscripts;
            node.SuperScripts = superscripts == null ? new List<MathNode>() : superscripts;
            node.PreSubScripts = presubscripts == null ? new List<MathNode>() : presubscripts;
            node.PreSuperScripts = presuperscripts == null ? new List<MathNode>() : presuperscripts;

            SetNodeBase(node, node.Children[0]);
            node.Width = node.Base.Width;
            node.Height = node.Base.Height;
            node.Depth = node.Base.Depth;
            node.Ascender = node.Base.Ascender;
            node.Descender = node.Base.Descender;

            List<MathNode> both_subs = new List<MathNode>(node.SubScripts);
            both_subs.AddRange(node.PreSubScripts);
            List<MathNode> both_sups = new List<MathNode>(node.SuperScripts);
            both_sups.AddRange(node.PreSuperScripts);
            node.SubScriptAxis = both_subs.Select(x => x.Axis()).Union(new List<double>() { 0 }).Max();
            node.SuperScriptAxis = both_sups.Select(x => x.Axis()).Union(new List<double>() { 0 }).Max();
            List<MathNode> all = new List<MathNode>(both_subs);
            all.AddRange(both_sups);
            double gap = all.Select(x => x.NominalLineGap()).Max(); //ToDo:Validate
            double protrusion = node.ParseLength("0.25ex");
            double scriptMedian = node.Axis();

            double[] v = GetRowVerticalExtent(both_subs, false, node.SubScriptAxis);
            double subHeight = v[0];
            double subDepth = v[1];
            double subAscender = v[2];
            double subDescender = v[3];

            v = GetRowVerticalExtent(both_sups, false, node.SuperScriptAxis);
            double superHeight = v[0];
            double superDepth = v[1];
            double superAscender = v[2];
            double superDescender = v[3];

            node.SubShift = 0;
            if (both_subs.Count > 0)
            {
                string shiftAttr = node.GetProperty("subscriptshift");
                if (shiftAttr == null)
                    shiftAttr = "0.5ex";

                node.SubShift = node.ParseLength(shiftAttr);  // positive shifts down
                node.SubShift = Math.Max(node.SubShift, subHeight - scriptMedian + gap);
                if (node.AlignToAxis)
                    node.SubShift += node.Axis();

                node.SubShift = Math.Max(node.SubShift, node.Base.Depth + protrusion - subDepth);
                node.Height = Math.Max(node.Height, subHeight - node.SubShift);
                node.Depth = Math.Max(node.Depth, subDepth + node.SubShift);
                node.Ascender = Math.Max(node.Ascender, subAscender - node.SubShift);
                node.Descender = Math.Max(node.Descender, subDescender + node.SubShift);
            }

            node.SuperShift = 0;
            if (both_sups.Count > 0)
            {
                string shiftAttr = node.GetProperty("superscriptshift");
                if (shiftAttr == null)
                    shiftAttr = "1ex";

                node.SuperShift = node.ParseLength(shiftAttr); // positive shifts up
                node.SuperShift = Math.Max(node.SuperShift, superDepth + scriptMedian + gap);
                if (node.AlignToAxis)
                    node.SuperShift -= node.Axis();

                node.SuperShift = Math.Max(node.SuperShift, node.Base.Height + protrusion - superHeight);
                node.Height = Math.Max(node.Height, superHeight + node.SuperShift);
                node.Depth = Math.Max(node.Depth, superDepth - node.SuperShift);
                node.Ascender = Math.Max(node.Ascender, superHeight + node.SuperShift);
                node.Descender = Math.Max(node.Descender, superDepth - node.SuperShift);
            }

            node.PostWidths = ParallelWidths(node.SubScripts, node.SuperScripts);
            node.PreWidths = ParallelWidths(node.PreSubScripts, node.PreSuperScripts);
            node.Width += node.PreWidths.Sum() + node.PostWidths.Sum();
        }

        private List<double> ParallelWidths(List<MathNode> nodes1, List<MathNode> nodes2)
        {
            List<double> widths = new List<double>();
            for (int i = 0; i < Math.Max(nodes1.Count, nodes2.Count); i++)
            {
                double w = 0;
                if (i < nodes1.Count)
                    w = Math.Max(w, nodes1[i].Width);
                if (i < nodes2.Count)
                    w = Math.Max(w, nodes2[i].Width);
                widths.Add(w);
            }
            return widths;
        }

        private void MeasureLimits(MathNode node, MathNode underscript, MathNode overscript)
        {
            if (node.Children[0].Core.MoveLimits)
            {
                List<MathNode> subs = new List<MathNode>();
                List<MathNode> supers = new List<MathNode>();
                if (underscript != null)
                    subs.Add(underscript);
                if (overscript != null)
                    supers.Add(overscript);
                MeasureScripts(node, subs, supers);
                return;
            }

            node.UnderScript = underscript;
            node.OverScript = overscript;
            SetNodeBase(node, node.Children[0]);

            node.Width = node.Base.Width;
            if (overscript != null)
                node.Width = Math.Max(node.Width, overscript.Width);

            if (underscript != null)
                node.Width = Math.Max(node.Width, underscript.Width);

            Stretch(node.Base, node.Width, null, null);
            Stretch(overscript, node.Width, null, null);
            Stretch(underscript, node.Width, null, null);

            double gap = node.NominalLineGap();

            if (overscript != null)
            {
                double overscriptBaselineHeight = node.Base.Height + gap + overscript.Depth;
                node.Height = overscriptBaselineHeight + overscript.Height;
                node.Ascender = node.Height;
            }
            else
            {
                node.Height = node.Base.Height;
                node.Ascender = node.Base.Ascender;
            }
            if (underscript != null)
            {
                double underscriptBaselineDepth = node.Base.Depth + gap + underscript.Height;
                node.Depth = underscriptBaselineDepth + underscript.Depth;
                node.Descender = node.Depth;
            }
            else
            {
                node.Depth = node.Base.Depth;
                node.Descender = node.Base.Descender;
            }
        }

        private void PushEnclosure(MathNode node)
        {
            if (node.Decoration == null)
                return;

            WrapChildren(node, "menclose");
            SetNodeBase(node.Children[0], node.Base);
            SetNodeBase(node, node.Children[0]);
            node.Base.Decoration = node.Decoration;
            node.Base.DecorationData = node.DecorationData;
            node.Decoration = null;
            node.DecorationData = null;
            node.Base.Width = node.Width;
            node.Base.Height = node.Height;
            node.Base.Depth = node.Depth;
            node.Base.BorderWidth = node.BorderWidth;
        }

        private void AddBoxEnclosure(MathNode node)
        {
            node.Width += 2 * node.HDelta;
            node.Height += node.VDelta;
            node.Depth += node.VDelta;
            node.Ascender = node.Base.Ascender;
            node.Descender = node.Base.Descender;
        }

        private void AddCircleEnclosure(MathNode node)
        {
            double d = Math.Sqrt(Math.Pow(node.Width, 2) + Math.Pow(node.Height, 2));
            d = Math.Max(d, node.Width + 2 * node.HDelta);
            d = Math.Max(d, node.Height + node.Depth + 2 * node.VDelta);
            double cy = (node.Height - node.Depth) / 2;
            node.Width = d;
            node.Height = d / 2 + cy;
            node.Depth = d / 2 - cy;
            node.Ascender = node.Base.Ascender;
            node.Descender = node.Base.Descender;
        }

        private void AddBorderEnclosure(MathNode node, List<bool> borders)
        {
            if (borders[0])
                node.Width += node.HDelta; //left
            if (borders[1])
                node.Width += node.HDelta; // right
            if (borders[2])
                node.Height += node.VDelta; // top
            if (borders[3])
                node.Depth += node.VDelta; // bottom
            node.DecorationData = borders;
            node.Ascender = node.Base.Ascender;
            node.Descender = node.Base.Descender;
        }

        public static double GetByIndexOrLast(List<double> lst, int idx)
        {
            if (idx < lst.Count)
                return lst[idx];
            else
                return lst.Last();
        }

        public static string GetByIndexOrLast(List<string> lst, int idx)
        {
            if (idx < lst.Count)
                return lst[idx];
            else
                return lst.Last();
        }

        private void ArrangeCells(MathNode node)
        {
            node.Rows = new List<RowDescriptor>();
            node.Columns = new List<ColumnDescriptor>();
            List<int> busycells = new List<int>();

            // Read table-level alignment properties
            List<string> table_rowaligns = node.GetListProperty("rowalign");
            List<string> table_columnaligns = node.GetListProperty("columnalign");

            foreach (MathNode ch in node.Children)
            {
                string rowalign = GetByIndexOrLast(table_rowaligns, node.Rows.Count);
                List<string> row_columnaligns = table_columnaligns;
                List<MathNode> cells = new List<MathNode>();

                if (ch.ElementName == "mtr" || ch.ElementName == "mlabeledtr")
                {
                    cells = ch.Children;
                    if (ch.Attributes.ContainsKey("rowalign"))
                        rowalign = ch.Attributes["rowalign"];
                    if (ch.Attributes.ContainsKey("columnalign"))
                    {
                        List<string> columnaligns = node.GetListProperty("columnalign", ch.Attributes["columnalign"]);
                    }
                }
                else
                {
                    cells.Add(ch);
                }

                RowDescriptor row = new RowDescriptor(node, cells, rowalign, row_columnaligns, busycells);
                node.Rows.Add(row);
                // busycells passes information about cells spanning multiple rows
                busycells = busycells.Select(n => Math.Max(0, n - 1)).ToList();
                while (busycells.Count < row.Cells.Count)
                    busycells.Add(0);

                for (int i = 0; i < row.Cells.Count; i++)
                {
                    CellDescriptor cell = row.Cells[i];
                    if (cell == null)
                        continue;
                    if (cell.RowSpan > 1)
                    {
                        for (int j = i; j <= i + cell.ColSpan; i++)
                        {
                            busycells[j] = cell.RowSpan - 1;
                        }
                    }
                }
            }

            //Pad the table with empty rows until no spanning cell protrudes
            while (busycells.Max() > 0)
            {
                string rowalign = Measurer.GetByIndexOrLast(table_rowaligns, node.Rows.Count);
                node.Rows.Add(new RowDescriptor(node, new List<MathNode>(), rowalign, table_columnaligns, busycells));
                busycells = busycells.Select(x => Math.Max(0, x - 1)).ToList();
            }
        }

        private void ArrangeLines(MathNode node)
        {
            //Get spacings and line styles; expand to cover the table fully
            List<double> spacings = node.GetListProperty("rowspacing").Select(x => node.ParseLength(x)).ToList();
            List<string> lines = node.GetListProperty("rowlines");

            for (int i = 0; i < node.Rows.Count - 1; i++) //Todo: Verify
            {
                node.Rows[i].SpaceAfter = (double)Measurer.GetByIndexOrLast(spacings, i);
                string line = GetByIndexOrLast(lines, i);
                if (line != "none")
                {
                    node.Rows[i].LineAfter = line;
                    node.Rows[i].SpaceAfter += node.LineWidth;
                }
            }

            spacings = node.GetListProperty("columnspacing").Select(x => node.ParseLength(x)).ToList();
            lines = node.GetListProperty("columnlines");

            for (int i = 0; i < node.Columns.Count - 1; i++) //Todo: Verify
            {
                node.Columns[i].SpaceAfter = GetByIndexOrLast(spacings, i);
                string line = GetByIndexOrLast(lines, i);
                if (line != "none")
                {
                    node.Columns[i].LineAfter = line;
                    node.Columns[i].SpaceAfter += node.LineWidth;
                }
            }

            node.FrameSpacings = new List<double>() { 0, 0 };
            node.FrameLines = new List<string>() { null, null };

            spacings = node.GetListProperty("framespacing").Select(x => node.ParseSpace(x)).ToList();
            lines = node.GetListProperty("frame");
            for (int i = 0; i < 2; i++) //Todo: Verify
            {
                string line = GetByIndexOrLast(lines, i);
                if (line != "none")
                {
                    node.FrameSpacings[i] = GetByIndexOrLast(spacings, i);
                    node.FrameLines[i] = line;
                }
            }
        }

        private void CalculateColumnWidths(MathNode node)
        {
            // Get total width
            string fullwidthattr = "auto";
            if (node.Attributes.ContainsKey("width"))
                fullwidthattr = node.Attributes["width"];

            double? fullwidth;
            if (fullwidthattr == "auto")
            {
                fullwidth = null;
            }
            else
            {
                fullwidth = node.ParseLength(fullwidthattr);
                if (fullwidth <= 0)
                    fullwidth = null;
            }

            // Fill fixed column widths
            List<string> columnwidths = node.GetListProperty("columnwidth");
            foreach (var item in node.Columns.Select((value, i) => new { i, value }))
            {
                ColumnDescriptor column = item.value;
                string attr = GetByIndexOrLast(columnwidths, item.i);
                if (attr == "auto" || attr == "fit")
                {
                    column.Fit = (attr == "fit");
                }
                else if (attr.EndsWith("%"))
                {
                    if (fullwidth == null)
                    {
                        //ToDo: node.error("Percents in column widths supported only in tables with explicit width; width of column %d treated as 'auto'" % (i+1))
                    }
                    else
                    {
                        double value = 0.0;
                        bool result = double.TryParse(attr.Substring(0, attr.Length - 1), out value);
                        if (result && value > 0)
                        {
                            column.Width = (double)fullwidth * value / 100;
                            column.Auto = false;
                        }
                    }
                }
                else
                {
                    column.Width = node.ParseSpace(attr);
                    column.Auto = true;
                }
            }

            // Set  initial auto widths for cells with colspan == 1
            foreach (RowDescriptor r in node.Rows)
            {
                foreach (var item in r.Cells.Select((value, i) => new { i, value }))
                {
                    CellDescriptor c = item.value;
                    if (c == null || c.Content == null || c.ColSpan > 1)
                        continue;
                    ColumnDescriptor column = node.Columns[item.i];
                    if (column.Auto)
                        column.Width = Math.Max(column.Width, c.Content.Width);
                }
            }

            // Calculate auto widths for cells with colspan > 1
            while (true)
            {
                List<ColumnDescriptor> adjustedColumns = new List<ColumnDescriptor>();
                double adjustedWidth = 0;

                foreach (RowDescriptor r in node.Rows)
                {
                    foreach (var item in r.Cells.Select((value, i) => new { i, value }))
                    {
                        CellDescriptor c = item.value;
                        if (c == null || c.Content == null || c.ColSpan == 1)
                            continue;

                        List<ColumnDescriptor> columns = node.Columns.Skip(item.i).Take(item.i + c.ColSpan).ToList(); //ToDo: Verify
                        List<ColumnDescriptor> autoColumns = columns.Where(x => x.Auto == true).ToList();
                        if (autoColumns.Count == 0)
                            continue;
                        List<ColumnDescriptor> fixedColumns = columns.Where(x => x.Auto == false).ToList();

                        double fixedWidth = columns.TakeWhile((value, i) => i != columns.Count - 1).Select(x => x.SpaceAfter).Sum();
                        if (fixedColumns.Count > 0)
                            fixedWidth += fixedColumns.Select(x => x.Width).Sum(); //ToDo: Verify
                        double autoWidth = autoColumns.Select(x => x.Width).Sum(); //ToDo: Verify
                        if (c.Content.Width <= fixedWidth + autoWidth)
                            continue; // already fits

                        double requiredWidth = c.Content.Width - fixedWidth;
                        double unitWidth = requiredWidth / autoColumns.Count;

                        while (true)
                        {
                            List<ColumnDescriptor> oversizedColumns = autoColumns.Where(x => x.Width >= unitWidth).ToList();
                            if (oversizedColumns.Count == 0)
                                break;

                            autoColumns = autoColumns.Where(x => x.Width < unitWidth).ToList();
                            if (autoColumns.Count == 0)
                                break; //weird rounding effects

                            requiredWidth -= oversizedColumns.Select(x => x.Width).Sum();
                            unitWidth = requiredWidth / autoColumns.Count;

                            if (autoColumns.Count == 0)
                                continue; //protection against arithmetic overflow

                            //Store the maximum unit width
                            if (unitWidth > adjustedWidth)
                            {
                                adjustedWidth = unitWidth;
                                adjustedColumns = autoColumns;
                            }
                        }
                    }
                }

                if (adjustedColumns.Count == 0)
                    break;
                foreach (ColumnDescriptor col in adjustedColumns)
                    col.Width = adjustedWidth;
            }

            if (node.GetProperty("equalcolumns") == "true")
            {
                double globalWidth = node.Columns.Where(x => x.Auto = true).Select(x => x.Width).Max();
                foreach (ColumnDescriptor col in node.Columns)
                {
                    if (col.Auto == true)
                        col.Width = globalWidth;
                }
            }
            if (fullwidth != null)
            {
                double delta = (double)fullwidth;
                delta -= node.Columns.Select(x => x.Width).Sum();
                delta -= node.Columns.TakeWhile((x, i) => i < node.Columns.Count - 1).Select(x => x.SpaceAfter).Sum(); //ToDo: verify
                delta -= 2 * node.FrameSpacings[0];
                if (delta != 0)
                {
                    List<ColumnDescriptor> sizableColumns = node.Columns.Where(x => x.Fit == true).ToList();
                    if (sizableColumns.Count == 0)
                        sizableColumns = node.Columns.Where(x => x.Auto == true).ToList();

                    if (sizableColumns.Count == 0)
                    {
                        //ToDo: Implement
                        //node.error("Overconstrained table layout: explicit table width specified, but no column has automatic width; table width attribute ignored")
                    }
                    else
                    {
                        delta /= sizableColumns.Count;
                        foreach (ColumnDescriptor col in sizableColumns)
                            col.Width += delta;
                    }
                }
            }
        }

        private void CalculateRowHeights(MathNode node)
        {
            //Set  initial row heights for cells with rowspan == 1
            double commonAxis = node.Axis();
            foreach (RowDescriptor r in node.Rows)
            {
                r.Height = 0;
                r.Depth = 0;
                foreach (CellDescriptor c in r.Cells)
                {
                    if (c == null || c.Content == null || c.RowSpan != 1)
                        continue;
                    double cellAxis = c.Content.Axis();
                    c.VShift = 0;

                    if (c.VAlign == "baseline")
                    {
                        if (r.AlignToAxis == true)
                            c.VShift -= commonAxis;
                        if (c.Content.AlignToAxis == true)
                            c.VShift += cellAxis;
                    }
                    else if (c.VAlign == "axis")
                    {
                        if (!r.AlignToAxis)
                            c.VShift += commonAxis;
                        if (!c.Content.AlignToAxis)
                            c.VShift -= cellAxis;
                    }
                    else
                    {
                        c.VShift = (r.Height - r.Depth - c.Content.Height + c.Content.Depth) / 2;
                    }

                    r.Height = Math.Max(r.Height, c.Content.Height + c.VShift);
                    r.Depth = Math.Max(r.Depth, c.Content.Depth - c.VShift);
                }
            }

            // Calculate heights for cells with rowspan > 1
            while (true)
            {
                List<RowDescriptor> adjustedRows = new List<RowDescriptor>();
                double adjustedSize = 0;

                foreach (var item in node.Rows.Select((value, i) => new { i, value }))
                {
                    RowDescriptor r = item.value;
                    foreach (CellDescriptor c in r.Cells)
                    {
                        if (c == null || c.Content == null || c.RowSpan == 1)
                            continue;

                        List<RowDescriptor> rows = node.Rows.Skip(item.i).Take(item.i + c.RowSpan).ToList();
                        double requiredSize = c.Content.Height + c.Content.Depth;
                        requiredSize -= rows.Where((value, i) => i < rows.Count).Select(x => x.SpaceAfter).Sum();
                        double fullSize = rows.Select(x => x.Height + x.Depth).Sum();
                        if (fullSize >= requiredSize)
                            continue;

                        double unitSize = requiredSize / rows.Count;
                        while (true)
                        {
                            List<RowDescriptor> oversizedRows = rows.Where(x => x.Height + x.Depth > unitSize).ToList(); //ToDo: Verify
                            if (oversizedRows.Count == 0)
                                break;

                            rows = rows.Where(x => x.Height + x.Depth < unitSize).ToList();
                            if (rows.Count == 0)
                                break; // weird rounding effects

                            requiredSize -= oversizedRows.Select(x => x.Height + x.Depth).Sum();
                            unitSize = requiredSize / rows.Count;
                        }

                        if (rows.Count == 0)
                            continue; // protection against arithmetic overflow

                        if (unitSize > adjustedSize)
                        {
                            adjustedSize = unitSize;
                            adjustedRows = rows;
                        }
                    }
                }

                if (adjustedRows.Count == 0)
                    break;

                foreach (RowDescriptor r in adjustedRows)
                {
                    double delta = (adjustedSize - r.Height - r.Depth) / 2;
                    r.Height += delta;
                    r.Depth += delta;
                }
            }

            if (node.GetProperty("equalrows") == "true")
            {
                double maxvsize = node.Rows.Select(x => x.Height + x.Depth).Max();
                foreach (RowDescriptor r in node.Rows)
                {
                    double delta = (maxvsize - r.Height - r.Depth) / 2;
                    r.Height += delta;
                    r.Depth += delta;
                }
            }
        }

        private object[] GetAlign(MathNode node)
        {
            string alignattr = node.GetProperty("align").Trim();
            if (alignattr.Length == 0)
                alignattr = MathDefaults.globalDefaults["align"];

            List<string> splitalign = alignattr.Split(null).ToList();
            string aligntype = splitalign[0];

            int? alignRow = null;
            if (splitalign.Count != 1)
            {
                alignRow = int.Parse(splitalign[1]);
            }
            return new object[] { aligntype, alignRow };
        }

        private readonly MathNode m_node;
        private const double m_defaultSlope = 1.383;
    }
}