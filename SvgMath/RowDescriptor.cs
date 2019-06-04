using System.Collections.Generic;

namespace SvgMath
{
    public class RowDescriptor
    {
        public RowDescriptor(MathNode node, List<MathNode> childCells, string rowalign, List<string> columnaligns, List<int> busycells)
        {
            AlignToAxis = (rowalign == "axis");
            Height = 0;
            Depth = 0;
            SpaceAfter = 0;
            Cells = new List<CellDescriptor>();

            foreach (MathNode c in childCells)
            {
                // Find first free cell
                while (busycells.Count > Cells.Count && busycells[Cells.Count] > 0)
                {
                    Cells.Add(null);
                }

                string halign = Measurer.GetByIndexOrLast(columnaligns, Cells.Count);
                string valign = rowalign;
                int colspan = 1;
                int rowspan = 1;

                if (c.ElementName == "mtd")
                {
                    if (c.Attributes.ContainsKey("columnalign"))
                        halign = c.Attributes["columnalign"];

                    if (c.Attributes.ContainsKey("rowalign"))
                        valign = c.Attributes["rowalign"];

                    colspan = int.Parse(c.Attributes.ContainsKey("colspan") ? c.Attributes["colspan"] : "1");
                    rowspan = int.Parse(c.Attributes.ContainsKey("rowspan") ? c.Attributes["rowspan"] : "1");
                }

                while (Cells.Count >= node.Columns.Count)
                    node.Columns.Add(new ColumnDescriptor());

                Cells.Add(new CellDescriptor(c, halign, valign, colspan, rowspan));

                for (int i = 1; i < colspan; i++)
                {
                    Cells.Add(null);
                }

                while (Cells.Count > node.Columns.Count)
                    node.Columns.Add(new ColumnDescriptor());
            }
        }

        public bool AlignToAxis;
        public double Height;
        public double Depth;
        public double SpaceAfter;
        public string LineAfter;
        public List<CellDescriptor> Cells;
    }
}