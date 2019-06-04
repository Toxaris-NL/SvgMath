using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace SvgMath
{
    internal static class Generator
    {
        public static XElement DrawImage(MathNode node)
        {
            //Top-level draw function: prepare the canvas, then call the draw method of the root node
            double baseline = 0;
            if (node.AlignToAxis)
                baseline = node.Axis();

            double height = Math.Max(node.Height, node.Ascender);
            double depth = Math.Max(node.Depth, node.Descender);
            double vsize = height + depth;

            Dictionary<string, string> attrs = new Dictionary<string, string>(){
                {"width", string.Format(CultureInfo.InvariantCulture, "{0:F6}pt", node.Width)},
                {"height", string.Format(CultureInfo.InvariantCulture, "{0:F6}pt", vsize)},
                {"viewBox", string.Format(CultureInfo.InvariantCulture, "0 {0:F6} {1:F6} {2:F6}", (-(height+baseline)), node.Width, vsize)}
            };

            XElement nodeElement = creatElement("svg", attrs);
            nodeElement.SetAttributeValue(XNamespace.Xmlns + "svg", SvgNs.NamespaceName);
            nodeElement.SetAttributeValue(XNamespace.Xmlns + "svgmath", SvgMathNs.NamespaceName);

            XElement metadataElement = creatElement("metadata", null, false);
            XElement metricElement = creatElement("metrics", new Dictionary<string, string>(){
                {"baseline", string.Format(CultureInfo.InvariantCulture, "{0}", depth - baseline)},
                {"axis", string.Format(CultureInfo.InvariantCulture, "{0}", depth - baseline + node.Axis())},
                {"top", string.Format(CultureInfo.InvariantCulture, "{0}", depth + node.Height)},
                {"bottom", string.Format(CultureInfo.InvariantCulture, "{0}", depth - node.Depth)}
            }, true);
            metadataElement.Add(metricElement);
            nodeElement.Add(metadataElement);
            DrawTranslatedNode(node, nodeElement, 0, -baseline);
            return nodeElement;
        }

        public static void DefaultDraw(MathNode node, XElement output)
        {
            //Matching Python
            //Pass
        }

        public static void GMath(MathNode node, XElement output)
        {
            GMRow(node, output);
        }

        public static void GMRow(MathNode node, XElement output)
        {
            DrawBox(node, output);
            if (node.Children.Count == 0)
                return;

            double offset = -node.Children.First().LeftSpace;
            foreach (MathNode ch in node.Children)
            {
                offset += ch.LeftSpace;
                double baseline = 0;
                if (ch.AlignToAxis && !node.AlignToAxis) //ToDo: might not behave the same as Python since aligntoaxis might be none
                    baseline = -node.Axis();

                DrawTranslatedNode(ch, output, offset, baseline);
                offset += ch.Width + ch.RightSpace;
            }
        }

        public static void GMPhantom(MathNode node, XElement output)
        {
            //Pass
        }

        public static void GMNone(MathNode node, XElement output)
        {
            //Pass
        }

        public static void GMAction(MathNode node, XElement output)
        {
            if (node.Base != null)
                node.Base.Draw(output);
        }

        public static void GMPrescripts(MathNode node, XElement output)
        {
            //Pass
        }

        public static void GMStyle(MathNode node, XElement output)
        {
            GMRow(node, output);
        }

        public static void GMFenced(MathNode node, XElement output)
        {
            GMRow(node, output);
        }

        public static void GMError(MathNode node, XElement output)
        {
            DrawBox(node, output, node.BorderWidth, "red");
            DrawTranslatedNode(node.Base, output, node.BorderWidth, 0);
        }

        public static void GMPAdded(MathNode node, XElement output)
        {
            DrawBox(node, output);
            DrawTranslatedNode(node.Base, output, node.LeftPadding, 0);
        }

        public static void GMEnclose(MathNode node, XElement output)
        {
            switch (node.Decoration)
            {
                case (null):
                    node.Base.Draw(output);
                    break;

                case ("strikes"):
                    DrawStrikesEnclosure(node, output);
                    break;

                case ("borders"):
                    DrawBordersEnclosure(node, output);
                    break;

                case ("box"):
                    DrawBoxEnclosure(node, output);
                    break;

                case ("roundedbox"):
                    double r = (node.Width - node.Base.Width + node.Height - node.Base.Height + node.Depth - node.Base.Depth) / 4;
                    DrawBoxEnclosure(node, output, r);
                    break;

                case ("circle"):
                    DrawCircleEnclosure(node, output);
                    break;

                default:
                    // ToDo: node.error("Internal error: unhandled decoration %s", str(node.decoration))
                    node.Base.Draw(output);
                    break;
            }
        }

        public static void GMFrac(MathNode node, XElement output)
        {
            DrawBox(node, output);
            if (node.GetProperty("bevelled") == "true")
            {
                DrawTranslatedNode(node.Enumerator, output, 0, node.Enumerator.Height - node.Height);
                DrawTranslatedNode(node.Denominator, output, node.Width - node.Denominator.Width, node.Depth - node.Denominator.Depth);
            }
            else
            {
                double enumalign = GetAlign(node, "enumalign");
                double denomalign = GetAlign(node, "denomalign");
                DrawTranslatedNode(node.Enumerator, output, node.RuleWidth + (node.Width - 2 * node.RuleWidth - node.Enumerator.Width) * enumalign, node.Enumerator.Height - node.Height);
                DrawTranslatedNode(node.Denominator, output, node.RuleWidth + (node.Width - 2 * node.RuleWidth - node.Denominator.Width) * denomalign, node.Depth - node.Denominator.Depth);
            }

            //Draw fraction line
            double eh; double dh; double ruleX; double ruleY;
            double x1; double x2; double y1; double y2;
            if (node.GetProperty("bevelled") == "true")
            {
                eh = node.Enumerator.Height + node.Enumerator.Depth;
                dh = node.Denominator.Height + node.Denominator.Depth;
                // Determine a point lying on the rule
                ruleX = (node.Width + node.Enumerator.Width - node.Denominator.Width) / 2.0;
                if (eh < dh)
                    ruleY = 0.75 * eh - node.Height;
                else
                    ruleY = node.Depth - 0.75 * dh;

                x1 = Math.Max(0, ruleX - (node.Depth - ruleY) / node.Slope);
                x2 = Math.Min(node.Width, ruleX + (ruleY + node.Height) / node.Slope);
                y1 = Math.Min(node.Depth, ruleY + ruleX * node.Slope);
                y2 = Math.Max(-node.Height, ruleY - (node.Width - ruleX) * node.Slope);
            }
            else
            {
                x1 = 0;
                y1 = 0;
                x2 = node.Width;
                y2 = 0;
            }
            DrawLine(output, node.Color, node.RuleWidth, x1, y1, x2, y2, new Dictionary<string, string>() { { "stroke-linecap", "butt" } });
        }

        public static void GMo(MathNode node, XElement output)
        {
            DrawSVGText(node, output);
        }

        public static void GMi(MathNode node, XElement output)
        {
            DrawSVGText(node, output);
        }

        public static void GMn(MathNode node, XElement output)
        {
            DrawSVGText(node, output);
        }

        public static void GMText(MathNode node, XElement output)
        {
            DrawSVGText(node, output);
        }

        public static void GMs(MathNode node, XElement output)
        {
            DrawSVGText(node, output);
        }

        public static void GMSpace(MathNode node, XElement output)
        {
            DrawSVGText(node, output);
        }

        public static void GMSub(MathNode node, XElement output)
        {
            if (node.Children.Count < 2)
            {
                GMRow(node, output);
            }
            else
            {
                DrawScripts(node, output);
            }
        }

        public static void GMSup(MathNode node, XElement output)
        {
            if (node.Children.Count < 2)
            {
                GMRow(node, output);
            }
            else
            {
                DrawScripts(node, output);
            }
        }

        public static void GMSubSup(MathNode node, XElement output)
        {
            if (node.Children.Count < 2)
            {
                GMRow(node, output);
            }
            else
            {
                DrawScripts(node, output);
            }
        }

        public static void GMMultiScripts(MathNode node, XElement output)
        {
            if (node.Children.Count < 2)
            {
                GMRow(node, output);
            }
            else
            {
                DrawScripts(node, output);
            }
        }

        public static void GMUnder(MathNode node, XElement output)
        {
            if (node.Children.Count < 2)
            {
                GMRow(node, output);
            }
            else
            {
                DrawLimits(node, output);
            }
        }

        public static void GMOver(MathNode node, XElement output)
        {
            if (node.Children.Count < 2)
            {
                GMRow(node, output);
            }
            else
            {
                DrawLimits(node, output);
            }
        }

        public static void GMUnderOver(MathNode node, XElement output)
        {
            if (node.Children.Count < 2)
            {
                GMRow(node, output);
            }
            else
            {
                DrawLimits(node, output);
            }
        }

        public static void GMTd(MathNode node, XElement output)
        {
            GMRow(node, output);
        }

        public static void GMSqrt(MathNode node, XElement output)
        {
            DrawBox(node, output);
            DrawTranslatedNode(node.Base, output, node.Width - node.Base.Width - node.Gap, 0);
            // Basic contour
            double x1 = node.Width - node.Base.Width - node.RootWidth - 2 * node.Gap;
            double y1 = (node.RootDepth - node.RootHeight) / 2;
            double x2 = x1 + node.RootWidth * 0.2;
            double y2 = y1;
            double x3 = x1 + node.RootWidth * 0.6;
            double y3 = node.RootDepth;
            double x4 = x1 + node.RootWidth;
            double y4 = (-node.RootHeight) + node.LineWidth / 2;
            double x5 = node.Width;
            double y5 = y4;
            // Thickening
            double slopeA = (x2 - x3) / (y2 - y3);
            double slopeB = (x3 - x4) / (y3 - y4);
            double x2a = x2 + (node.ThickLineWidth - node.LineWidth);
            double y2a = y2;
            double x2c = x2 + node.LineWidth * slopeA / 2;
            double y2c = y2 + node.LineWidth * 0.9;
            double x2b = x2c + (node.ThickLineWidth - node.LineWidth) / 2;
            double y2b = y2c;
            double ytmp = y3 - node.LineWidth / 2;
            double xtmp = x3 - node.LineWidth * (slopeA + slopeB) / 4;
            double y3a = (y2a * slopeA - ytmp * slopeB + xtmp - x2a) / (slopeA - slopeB);
            double x3a = xtmp + (y3a - ytmp) * slopeB;
            double y3b = (y2b * slopeA - ytmp * slopeB + xtmp - x2b) / (slopeA - slopeB);
            double x3b = xtmp + (y3b - ytmp) * slopeB;
            // Lean the left protrusion down
            y1 += (x2 - x1) * slopeA;
            Dictionary<string, string> attrs = new Dictionary<string, string>()
            {
                { "stroke", node.Color },
                { "fill", "none" },
                { "stroke-width", string.Format(CultureInfo.InvariantCulture, "{0:F6}", node.LineWidth)},
                { "stroke-linecap", "butt" },
                { "stroke-linejoin", "miter"},
                { "stroke-miterlimit", "10"},
                { "d", string.Format(CultureInfo.InvariantCulture, "M {0:F6} {1:F6} L {2:F6} {3:F6} L {4:F6} {5:F6} L {6:F6} {7:F6} L {8:F6} {9:F6} L {10:F6} {11:F6} L {12:F6} {13:F6} L {14:F6} {15:F6} L {16:F6} {17:F6}",x1,y1,x2a,y2a,x3a,y3a,x3b,y3b,x2b,y2b,x2c,y2c, x3,y3,x4,y4,x5,y5)}
            };
            output.Add(creatElement("path", attrs));
        }

        public static void GMRoot(MathNode node, XElement output)
        {
            GMSqrt(node, output);
            if (node.RootIndex != null)
            {
                double w = Math.Max(0, node.CornerWidth - node.RootIndex.Width) / 2;
                double h = -node.RootIndex.Depth - node.RootHeight + node.CornerHeight;
                DrawTranslatedNode(node.RootIndex, output, w, h);
            }
        }

        public static void GMTable(MathNode node, XElement output)
        {
            DrawBox(node, output);
            //Draw cells
            double vshift = -node.Height + node.FrameSpacings[1];
            double hshift;
            foreach (var rowItem in node.Rows.Select((value, r) => new { r, value }))
            {
                RowDescriptor row = rowItem.value;
                vshift += row.Height;
                hshift = node.FrameSpacings[0];
                foreach (var cellItem in row.Cells.Select((value, c) => new { c, value }))
                {
                    ColumnDescriptor column = node.Columns[cellItem.c];
                    CellDescriptor cell = row.Cells[cellItem.c];
                    if (cell != null && cell.Content != null)
                    {
                        double cellWidth;
                        // Calculate horizontal alignment
                        if (cell.ColSpan > 1)
                        {
                            cellWidth = node.Columns.Skip(cellItem.c).Take(cellItem.c + cell.ColSpan).Select(x => x.Width).Sum(); //ToDo: verify
                            cellWidth += node.Columns.Skip(cellItem.c).Take(cellItem.c + cell.ColSpan - 1).Select(x => x.SpaceAfter).Sum(); //ToDo: verify
                        }
                        else
                        {
                            cellWidth = column.Width;
                        }
                        double hadjust = (cellWidth - cell.Content.Width) * (alignKeywords.ContainsKey(cell.HAlign) ? alignKeywords[cell.HAlign] : 0.5);

                        // Calculate vertical alignment.
                        double cellHeight;
                        if (cell.RowSpan > 1)
                        {
                            cellHeight = node.Rows.Skip(rowItem.r).Take(rowItem.r + cell.RowSpan).Select(x => x.Height + x.Depth).Sum(); //ToDo: Verify
                            cellHeight += node.Rows.Skip(rowItem.r).Take(rowItem.r + cell.RowSpan - 1).Select(x => x.SpaceAfter).Sum(); //ToDo: Verify
                        }
                        else
                        {
                            cellHeight = row.Height + row.Depth;
                        }

                        double vadjust;
                        if (cell.VAlign == "top")
                        {
                            vadjust = cell.Content.Height - row.Height;
                        }
                        else if (cell.VAlign == "bottom")
                        {
                            vadjust = cellHeight - row.Height - cell.Content.Depth;
                        }
                        else if ((cell.VAlign == "axis" || cell.VAlign == "baseline") && cell.RowSpan == 1)
                        {
                            vadjust = -cell.VShift; // calculated in the measurer
                        }
                        else
                        {
                            vadjust = (cell.Content.Height - cell.Content.Depth + cellHeight) / 2 - row.Height;
                        }
                        DrawTranslatedNode(cell.Content, output, hshift + hadjust, vshift + vadjust);
                    }
                    hshift += column.Width + column.SpaceAfter;
                }
                vshift += row.Depth + row.SpaceAfter;
            }

            //Draw frame
            double x1 = node.LineWidth / 2;
            double y1 = node.LineWidth / 2 - node.Height;
            double x2 = node.Width - node.LineWidth / 2;
            double y2 = node.Depth - node.LineWidth / 2;

            DrawBorder(node, output, x1, y1, x1, y2, node.FrameLines[0]);
            DrawBorder(node, output, x2, y1, x2, y2, node.FrameLines[0]);
            DrawBorder(node, output, x1, y1, x2, y1, node.FrameLines[1]);
            DrawBorder(node, output, x1, y2, x2, y2, node.FrameLines[1]);

            // Draw intermediate lines
            // First, let's make a grid
            hshift = node.FrameSpacings[0];
            List<double> hoffsets = new List<double>();
            foreach (var item in node.Columns.Select((value, c) => new { c, value }))
            {
                double spacing = item.value.SpaceAfter;
                hshift += item.value.Width;
                hoffsets.Add(hshift + spacing / 2);
                hshift += spacing;
            }
            hoffsets[hoffsets.Count - 1] = x2;

            vshift = -node.Height + node.FrameSpacings[1];
            List<double> voffsets = new List<double>();
            foreach (var item in node.Rows.Select((value, r) => new { r, value }))
            {
                double spacing = item.value.SpaceAfter;
                vshift += item.value.Height + item.value.Depth;
                voffsets.Add(vshift + spacing / 2);
                vshift += spacing;
            }
            voffsets[voffsets.Count - 1] = y2;

            List<double> vspans = Enumerable.Repeat<double>(0, node.Columns.Count).ToList();
            for (int r = 0; r < node.Rows.Count - 1; r++)
            {
                RowDescriptor row = node.Rows[r];
                if (row.LineAfter == null)
                    continue;

                foreach (var cellItem in row.Cells.Select((value, c) => new { c, value }))
                {
                    CellDescriptor cell = cellItem.value;
                    if (cell == null || cell.ColSpan == 0)
                        continue;
                    for (int i = cellItem.c; i < cellItem.c + cell.ColSpan; i++)
                        vspans[i] = cell.RowSpan;
                }
                vspans = vspans.Select(x => Math.Max(0, x - 1)).ToList();

                double lineY = voffsets[r];
                double startX = x1;
                double endX = x1;
                foreach (var cellItem in node.Columns.Select((value, c) => new { c, value }))
                {
                    if (vspans[cellItem.c] > 0)
                    {
                        DrawBorder(node, output, startX, lineY, endX, lineY, row.LineAfter);
                        startX = hoffsets[cellItem.c];
                    }
                    endX = hoffsets[cellItem.c];
                }
                DrawBorder(node, output, startX, lineY, endX, lineY, row.LineAfter);
            }

            List<double> hspans = Enumerable.Repeat<double>(0, node.Columns.Count).ToList();
            for (int c = 0; c < node.Columns.Count - 1; c++)
            {
                ColumnDescriptor column = node.Columns[c];
                if (column.LineAfter == null)
                    continue;

                foreach (var rowItem in node.Rows.Select((value, r) => new { r, value }))
                {
                    RowDescriptor row = rowItem.value;
                    if (row.Cells.Count <= c)
                        continue;
                    CellDescriptor cell = row.Cells[c];
                    if (cell == null || cell.Content == null)
                        continue;
                    for (int j = rowItem.r; j < rowItem.r + cell.RowSpan; j++)
                    {
                        hspans[j] = cell.ColSpan;
                    }
                }
                hspans = hspans.Select(x => Math.Max(0, x - 1)).ToList();
                double lineX = hoffsets[c];
                double startY = y1;
                double endY = y1;
                foreach (var item in node.Rows.Select((value, r) => new { r, value }))
                {
                    if (hspans[item.r] > 0)
                    {
                        DrawBorder(node, output, lineX, startY, lineX, endY, column.LineAfter);
                        startY = voffsets[item.r];
                    }
                    endY = voffsets[item.r];
                }
                DrawBorder(node, output, lineX, startY, lineX, endY, column.LineAfter);
            }
        }

        public static void DrawBox(MathNode node, XElement output, double? borderWidth = null, string borderColor = null, double borderRadius = 0)
        {
            string background = GetBackGround(node);
            if (background == "none" && (borderWidth == null || borderWidth == 0))
                return;
            if (borderColor == null)
                borderColor = node.Color;

            Dictionary<string, string> attrs = new Dictionary<string, string>
            {
                { "fill", background },
                { "stroke", "none" },
                { "x", string.Format(CultureInfo.InvariantCulture, "{0}", (double)borderWidth / 2) },
                { "y", string.Format(CultureInfo.InvariantCulture, "{0}", (double)borderWidth / 2 - node.Height) },
                { "width", string.Format(CultureInfo.InvariantCulture, "{0}", node.Width - (double)borderWidth) },
                { "height", string.Format(CultureInfo.InvariantCulture, "{0}", node.Height + node.Depth - (double)borderWidth) }
            };

            if (borderWidth != null && borderColor != null)
            {
                attrs["stroke"] = borderColor;
                attrs["stroke-width"] = string.Format(CultureInfo.InvariantCulture, "{0}", borderWidth);
                if (borderRadius != 0)
                {
                    attrs.Add("rx", string.Format(CultureInfo.InvariantCulture, "{0}", borderRadius));
                    attrs.Add("ry", string.Format(CultureInfo.InvariantCulture, "{0}", borderRadius));
                }
            }
            XElement rect = creatElement("rect", attrs);
            output.Add(rect);
        }

        public static void DrawTranslatedNode(MathNode node, XElement output, double dx, double dy)
        {
            if (dx != 0 || dy != 0)
            {
                XElement svge = creatElement("g", new Dictionary<string, string>() { { "transform", string.Format(CultureInfo.InvariantCulture, "translate({0}, {1})", dx, dy) } });
                node.Draw(svge);
                output.Add(svge);
            }
            else
            {
                node.Draw(output);
            }

            //ToDo:Check implementation
        }

        public static void DrawStrikesEnclosure(MathNode node, XElement output)
        {
            DrawBox(node, output);
            node.Base.Draw(output);

            double mid_x = node.Width / 2;
            double mid_y = (node.Depth - node.Height) / 2;
            // horizontal
            if (node.DecorationData[0])
                DrawStrike(node, output, 0, mid_y, node.Width, mid_y);
            // vert
            if (node.DecorationData[1])
                DrawStrike(node, output, mid_x, -node.Height, mid_x, node.Depth);
            // updiag
            if (node.DecorationData[2])
                DrawStrike(node, output, 0, node.Depth, node.Width, -node.Height);
            // downdiag
            if (node.DecorationData[3])
                DrawStrike(node, output, 0, -node.Height, node.Width, node.Depth);
        }

        public static void DrawStrike(MathNode node, XElement output, double x1, double y1, double x2, double y2)
        {
            DrawLine(output, node.Color, node.BorderWidth, x1, y1, x2, y2);
        }

        public static void DrawLine(XElement output, string color, double width, double x1, double y1, double x2, double y2, Dictionary<string, string> strokeAttrs = null)
        {
            Dictionary<string, string> attrs = new Dictionary<string, string>()
            {
                {"fill", "none" },
                {"stroke", color},
                {"stroke-width", string.Format(CultureInfo.InvariantCulture, "{0}", width) },
                {"stroke-linecap", "square" },
                {"stroke-dasharray","none" },
                {"x1", string.Format(CultureInfo.InvariantCulture, "{0}", x1) },
                {"y1", string.Format(CultureInfo.InvariantCulture, "{0}", y1) },
                {"x2", string.Format(CultureInfo.InvariantCulture, "{0}", x2) },
                {"y2", string.Format(CultureInfo.InvariantCulture, "{0}", y2) }
            };

            if (strokeAttrs != null)
            {
                foreach (string key in strokeAttrs.Keys)
                {
                    if (attrs.ContainsKey(key))
                    {
                        attrs[key] = strokeAttrs[key];
                    }
                    else
                    {
                        attrs.Add(key, strokeAttrs[key]);
                    }
                }
            }

            XElement nxe = creatElement("line", attrs);
            output.Add(nxe);
        }

        public static void DrawBordersEnclosure(MathNode node, XElement output)
        {
            DrawBox(node, output);
            double x1 = node.BorderWidth / 2;
            double y1 = node.BorderWidth / 2 - node.Height;
            double x2 = node.Width - node.BorderWidth / 2;
            double y2 = node.Depth - node.BorderWidth / 2;
            // Left
            if (node.DecorationData[0])
                DrawBorder(node, output, x1, y1, x1, y2);
            // Right
            if (node.DecorationData[1])
                DrawBorder(node, output, x2, y1, x2, y2);
            // top
            if (node.DecorationData[2])
                DrawBorder(node, output, x1, y1, x2, y1);
            // bottom
            if (node.DecorationData[3])
                DrawBorder(node, output, x1, y2, x2, y2);

            double offset;
            //Left
            if (node.DecorationData[0])
            {
                offset = node.Width - node.Base.Width;
                //Right
                if (node.DecorationData[1])
                    offset /= 2;
            }
            else
            {
                offset = 0;
            }
            DrawTranslatedNode(node.Base, output, offset, 0);
        }

        public static void DrawBorder(MathNode node, XElement output, double x1, double y1, double x2, double y2)
        {
            DrawLine(output, node.Color, node.BorderWidth, x1, y2, x2, y2);
        }

        public static void DrawBorder(MathNode node, XElement output, double x1, double y1, double x2, double y2, string lineStyle)
        {
            if (lineStyle == null)
                return;

            if (x1 == x2 && y1 == y2)
                return;
            Dictionary<string, string> extraStyle = null;
            if (lineStyle == "dashed")
            {
                double linelength = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
                double dashoffset = 5 - ((linelength / node.LineWidth + 3) % 10) / 2;
                extraStyle = new Dictionary<string, string>()
                {
                    { "stroke-dasharray", string.Format(CultureInfo.InvariantCulture, "{0:F6},{1:F6}", node.LineWidth * 7, node.LineWidth * 3)},
                    { "stroke-dashoffset", string.Format(CultureInfo.InvariantCulture, "{0:F6}", node.LineWidth * dashoffset) }
                };
            }
            DrawLine(output, node.Color, node.LineWidth, x1, y1, x2, y2, extraStyle);
        }

        public static void DrawCircleEnclosure(MathNode node, XElement output)
        {
            string background = GetBackGround(node);
            double r = (node.Width - node.BorderWidth) / 2;
            double cx = node.Width / 2;
            double cy = (node.Depth - node.Height) / 2;

            Dictionary<string, string> attrs = new Dictionary<string, string>()
            {
                { "fill", background},
                {"stroke", node.Color },
                { "stroke-width", string.Format(CultureInfo.InvariantCulture, "{0}", node.BorderWidth) },
                { "cx", string.Format(CultureInfo.InvariantCulture, "{0}", cx) },
                { "cy", string.Format(CultureInfo.InvariantCulture, "{0}", cy) },
                { "r", string.Format(CultureInfo.InvariantCulture, "{0}", r) }
            };
            output.Add(creatElement("circle", attrs));
            DrawTranslatedNode(node.Base, output, (node.Width - node.Base.Width) / 2, 0);
        }

        public static void DrawBoxEnclosure(MathNode node, XElement output, double roundRadius = 0)
        {
            DrawBox(node, output, node.BorderWidth, null, roundRadius);
            DrawTranslatedNode(node.Base, output, (node.Width - node.Base.Width) / 2, 0);
        }

        public static double GetAlign(MathNode node, string attrName)
        {
            string attrValue = node.GetProperty(attrName, "center");
            if (alignKeywords.ContainsKey(attrValue))
            {
                return alignKeywords[attrValue];
            }
            else
            {
                return 0.5;
                //ToDO: node.error("Bad value %s for %s", attrValue, attrName)
            }
        }

        public static void DrawSVGText(MathNode node, XElement output)
        {
            DrawBox(node, output);
            List<string> fontFamilies = node.FontPool().Where(x => x.Used == true).Select(x => x.Family).ToList();
            if (fontFamilies.Count == 0)
                fontFamilies = node.FontFamilies;

            Dictionary<string, string> attrs = new Dictionary<string, string>()
            {
                { "fill", node.Color },
                { "font-family", string.Join(", ",fontFamilies) },
                { "font-size", string.Format(CultureInfo.InvariantCulture, "{0}", node.FontSize) },
                { "text-anchor", "middle" },
                { "x", string.Format(CultureInfo.InvariantCulture, "{0}",(node.Width + node.LeftBearing - node.RightBearing) / 2 / node.TextStretch)},
                { "y", string.Format(CultureInfo.InvariantCulture, "{0}", -node.TextShift)}
            };

            if (node.FontWeight != "normal")
                attrs["font-weight"] = node.FontWeight;
            if (node.FontStyle != "normal")
                attrs["font-style"] = node.FontStyle;
            if (node.TextStretch != 1)
                attrs["transform"] = string.Format(CultureInfo.InvariantCulture, "scale({0}, 1)", node.TextStretch);

            foreach (KeyValuePair<int, char> ch in MathDefaults.SpecialChars)
            {
                node.Text = node.Text.Replace((char)ch.Key, ch.Value); //ToDo: validate
            }

            XElement nxe = creatElement("text", attrs);
            XText chars = new XText(node.Text);
            nxe.Add(chars);
            output.Add(nxe);
        }

        public static void DrawScripts(MathNode node, XElement output)
        {
            //ToDo: figger wat to match python
            //    if len(node.children) < 2:
            //      draw_mrow(node); return

            double subY = node.SubShift;
            double superY = -node.SuperShift;

            DrawBox(node, output);
            double offset = 0;
            foreach (var item in node.PreWidths.Select((value, i) => new { i, value }))
            {
                offset += node.PreWidths[item.i];
                if (item.i < node.PreSubScripts.Count)
                {
                    MathNode presubscript = node.PreSubScripts[item.i];
                    DrawTranslatedNode(presubscript, output, offset - presubscript.Width, subY - Adjustment(presubscript));
                }
                if (item.i < node.PreSuperScripts.Count)
                {
                    MathNode presuperscript = node.PreSuperScripts[item.i];
                    DrawTranslatedNode(presuperscript, output, offset - presuperscript.Width, superY - Adjustment(presuperscript));
                }
            }
            DrawTranslatedNode(node.Base, output, offset, 0);
            offset += node.Base.Width;

            foreach (var item in node.PostWidths.Select((value, i) => new { i, value }))
            {
                if (item.i < node.SubScripts.Count)
                {
                    MathNode subscript = node.SubScripts[item.i];
                    DrawTranslatedNode(subscript, output, offset, subY - Adjustment(subscript));
                }
                if (item.i < node.SuperScripts.Count)
                {
                    MathNode superscript = node.SuperScripts[item.i];
                    DrawTranslatedNode(superscript, output, offset, superY - Adjustment(superscript));
                }
                offset += node.PostWidths[item.i];
            }
        }

        public static void DrawLimits(MathNode node, XElement output)
        {
            if (node.Core.MoveLimits)
            {
                DrawScripts(node, output);
                return;
            }

            DrawBox(node, output);
            DrawTranslatedNode(node.Base, output, (node.Width - node.Base.Width) / 2, 0);
            if (node.UnderScript != null)
                DrawTranslatedNode(node.UnderScript, output, (node.Width - node.UnderScript.Width) / 2, node.Depth - node.UnderScript.Depth);
            if (node.OverScript != null)
                DrawTranslatedNode(node.OverScript, output, (node.Width - node.OverScript.Width) / 2, node.OverScript.Height - node.Height);
        }

        public static double Adjustment(MathNode script)
        {
            if (script.AlignToAxis)
                return script.Axis();
            else
                return 0;
        }

        public static string GetBackGround(MathNode node)
        {
            foreach (string attr in m_backgroundAttributes)
            {
                if (node.Attributes != null && node.Attributes.ContainsKey(attr))
                {
                    if (node.Attributes[attr] == "transparent")
                        return "none";
                    else
                        return node.Attributes[attr];
                }
            }
            return "none";
        }

        public static XElement creatElement(string localName, Dictionary<string, string> attrs, bool mathNamespace = false)
        {
            XElement newElement;
            if (mathNamespace)
            {
                newElement = new XElement(SvgMathNs + localName);
            }
            else
            {
                newElement = new XElement(SvgNs + localName);
            }
            if (attrs != null)
            {
                foreach (KeyValuePair<string, string> vp in attrs)
                {
                    newElement.SetAttributeValue(vp.Key, vp.Value);
                }
            }
            return newElement;
        }

        // SVG namespace
        public static XNamespace SvgNs = "http://www.w3.org/2000/svg";

        public static XAttribute SvgNsAttr = new XAttribute(XNamespace.Xmlns + "svg", SvgNs.NamespaceName);

        // SVGMath proprietary namespace - used in metadata
        public static XNamespace SvgMathNs = "http://www.grigoriev.ru/svgmath";

        public static XAttribute SvgNsMathAttr = new XAttribute(XNamespace.Xmlns + "svgmath", SvgMathNs.NamespaceName);

        // Handy mapping of horizontal alignment keywords
        public static Dictionary<string, double> alignKeywords = new Dictionary<string, double>() { { "left", 0 }, { "center", 0.5 }, { "right", 1 } };

        private static List<string> m_backgroundAttributes = new List<string>() { "mathbackground", "background-color", "background" };
    }
}