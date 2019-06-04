using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SvgMath
{
    internal class TrueTypeFont : GenericFontMetric
    {
        public TrueTypeFont(string fontPath)
        {
            m_fontPath = fontPath;
            CharData = new Dictionary<int, SvgMath.CharMetric>();
            try
            {
                using (FileStream fs = File.OpenRead(m_fontPath))//new FileStream(m_fontPath, FileMode.Open)
                {
                    m_reader = new BinaryReader(fs);
                    using (m_reader)
                    {
                        if (IsSupported())
                        {
                            ReadFontMetrics();
                            PostParse();
                        }
                        else
                        {
                            throw new NotSupportedException("Not a TTF file");
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        private void PostParse()
        {
            // Get Ascender from the 'd' glyph
            if (CharData.ContainsKey('d'))
                m_ascender = CharData['d'].BBox[3];

            // Get Descender from the 'p' glyph
            if (CharData.ContainsKey('p'))
                m_descender = CharData['p'].BBox[1];

            // Get CapHeight from the 'H' glyph
            if (CharData.ContainsKey('H'))
                m_capHeight = CharData['H'].BBox[3];

            // Get XHeight from the 'x' glyph
            if (CharData.ContainsKey('x'))
                m_capHeight = CharData['x'].BBox[3];

            // Determine the vertical position of the mathematical axis -
            // that is, the quote to which fraction separator lines are raised.
            // We try to deduce it from the median of the following characters:
            // "equal", "minus", "plus", "less", "greater", "periodcentered")
            // Default is CapHeight / 2, or 0.3 if there's no CapHeight.
            foreach (char c in new char[] { '+', '\u2212', '=', '<', '>', '\u00b7' })
            {
                if (CharData.ContainsKey(c))
                {
                    m_axisPosition = (CharData[c].BBox[1] + CharData[c].BBox[3]) / 2;
                    break;
                }
            }

            // Determine the dominant rule width for math
            // TTF fonts will always have this, other fonts not. See python code for other implementation.
            m_ruleWidth = m_underlineThickness;

            if (m_italicAngle == 0)
            {
                if (CharData.ContainsKey('!'))
                    m_stdVw = CharData['!'].BBox[2] - CharData['!'].BBox[0];
                else if (CharData.ContainsKey('.'))
                    m_stdVw = CharData['.'].BBox[2] - CharData['.'].BBox[0];
            }

            // Set rule gap
            m_vGap = -m_undelinePosition;

            m_missingGlyph = CharData['\u0020'] ?? CharData['\u00A0'];
        }

        private bool IsSupported()
        {
            SkipTo(0);
            if (m_reader.ReadUInt32() != c_ttfVersion)
                return false;

            return true;
        }

        private void ReadFontMetrics()
        {
            SkipTo(0);
            // TTF version
            m_reader.ReadUInt32();
            // Number of tables
            int numTables = numTables = m_reader.ReadUInt16();
            numTables = BitConverter.ToInt16(BitConverter.GetBytes(numTables), 1);
            // searchRange
            m_reader.ReadUInt16();
            // entrySelector
            m_reader.ReadUInt16();
            // rangeShift
            m_reader.ReadUInt16();

            m_fontType = c_ttfAbbreviation;

            for (int i = 0; i < numTables; i++)
            {
                string tag = Encoding.UTF8.GetString(BitConverter.GetBytes(m_reader.ReadUInt32()));
                uint checksum = ReadUInt32Bytes();
                uint offset = ReadUInt32Bytes();
                uint length = ReadUInt32Bytes();
                m_tables.Add(tag, new Tuple<uint, uint>(offset, length));
            }

            SwitchTables("head");
            SkipTo(m_currentOffset + 12);
            uint magic = ReadUInt32Bytes();
            if (magic != 0x5F0F3CF5)
                throw new Exception("Magic number in 'head' table does not match the spec");

            Skip(2);
            m_unitsPerEm = ReadUInt16Bytes();
            m_eMScale = 1.0 / Convert.ToDouble(m_unitsPerEm);

            Skip(16);
            double xMin = ReadInt16Bytes() * m_eMScale;
            double yMin = ReadInt16Bytes() * m_eMScale;
            double xMax = ReadInt16Bytes() * m_eMScale;
            double yMax = ReadInt16Bytes() * m_eMScale;
            m_boundingBox = new double[] { xMin, yMin, xMax, yMax };

            Skip(6);
            m_indexToLocFormat = ReadInt16Bytes();

            SwitchTables("maxp");
            SkipTo(m_currentOffset);
            Skip(4);
            m_numGlyphs = ReadUInt16Bytes();

            SwitchTables("name");
            SkipTo(m_currentOffset);
            Skip(2);
            uint numRecords = ReadUInt16Bytes();
            m_storageOffset = ReadUInt16Bytes() + m_currentOffset;

            for (int i = 0; i < numRecords; i++)
            {
                uint platformId = ReadUInt16Bytes();
                uint encodingId = ReadUInt16Bytes();
                uint languageId = ReadUInt16Bytes();
                uint nameId = ReadUInt16Bytes();
                uint nameLength = ReadUInt16Bytes();
                uint nameOffset = ReadUInt16Bytes();

                if (platformId == 3 && encodingId == 1)
                {
                    if (englishCodes.Contains(languageId) || !m_uniNames.ContainsKey(nameId))
                    {
                        m_uniNames[nameId] = new Tuple<uint, uint>(nameOffset, nameLength);
                    }
                }
                else if (platformId == 1 && encodingId == 0)
                {
                    if (languageId == 0 || !m_macNames.ContainsKey(nameId))
                    {
                        m_macNames[nameId] = new Tuple<uint, uint>(nameOffset, nameLength);
                    }
                }
            }

            m_family = GetName(1);
            m_fullName = GetName(4);
            m_fontName = GetName(6);

            SwitchTables("OS/2");
            SkipTo(m_currentOffset);
            uint tableVersion = ReadUInt16Bytes();
            int os2_xAvgCharWidth = ReadInt16Bytes();
            if (os2_xAvgCharWidth > 0)
                m_charWidth = os2_xAvgCharWidth * EmScale;

            uint wght = ReadUInt16Bytes();
            if (wght < 150)
                m_weight = "Thin";
            else if (wght < 250)
                m_weight = "Extra-Light";
            else if (wght < 350)
                m_weight = "Light";
            else if (wght < 450)
                m_weight = "Regular";
            else if (wght < 550)
                m_weight = "Medium";
            else if (wght < 650)
                m_weight = "Demi-Bold";
            else if (wght < 750)
                m_weight = "Bold";
            else if (wght < 850)
                m_weight = "Extra-Bold";
            else
                m_weight = "Black";

            Skip(62);
            m_ascender = ReadInt16Bytes() * EmScale;
            m_descender = ReadInt16Bytes() * EmScale;

            if (tableVersion == 2)
            {
                Skip(14);
                int xh = ReadInt16Bytes();
                if (xh > 0)
                    m_xHeight = xh * EmScale;
                int ch = ReadInt16Bytes();
                if (ch > 0)
                    m_capHeight = ch * EmScale;
            }

            SwitchTables("post");
            SkipTo(m_currentOffset);
            Skip(4);
            m_italicAngle = Convert.ToDouble(ReadFixed32());
            m_undelinePosition = ReadInt16Bytes() * EmScale;
            m_underlineThickness = ReadInt16Bytes() * EmScale;

            SwitchTables("hhea");
            SkipTo(m_currentOffset);
            Skip(34);
            uint numHmtx = ReadUInt16Bytes();

            SwitchTables("hmtx");
            SkipTo(m_currentOffset);
            List<CharMetric> glyphArray = new List<CharMetric>();
            double w = 0;
            for (int i = 0; i < m_numGlyphs; i++)
            {
                if (i < numHmtx)
                {
                    w = ReadUInt16Bytes() * EmScale;
                    Skip(2);
                }
                glyphArray.Add(new CharMetric(w));
            }

            SwitchTables("cmap");
            SkipTo(m_currentOffset);
            Skip(2);
            Dictionary<Tuple<uint, uint>, uint> cmapEncodings = new Dictionary<Tuple<uint, uint>, uint>();
            uint numSubTables = ReadUInt16Bytes();
            for (int i = 0; i < numSubTables; i++)
            {
                uint platformId = ReadUInt16Bytes();
                uint encodingId = ReadUInt16Bytes();
                uint subTableoffset = ReadUInt32Bytes();
                cmapEncodings.Add(new Tuple<uint, uint>(platformId, encodingId), subTableoffset);
            }
            string encodingScheme = "Unicode";
            uint subtableOffset;
            Tuple<uint, uint> preferedSubTable = new Tuple<uint, uint>(3, 1);
            Tuple<uint, uint> symbolSubTable = new Tuple<uint, uint>(3, 0);
            if (!cmapEncodings.ContainsKey(preferedSubTable))
            {
                if (!cmapEncodings.ContainsKey(symbolSubTable))
                    throw new NotSupportedException(string.Format("Cannot use font {0}: no known subtable in 'cmap' table", m_fontName));

                encodingScheme = "Symbol";
                subtableOffset = cmapEncodings[symbolSubTable];
            }
            else
            {
                subtableOffset = cmapEncodings[preferedSubTable];
            }

            SkipTo(m_currentOffset + subtableOffset);
            uint subTableFormat = ReadUInt16Bytes();

            if (subTableFormat != 4)
                throw new NotSupportedException(string.Format("Unsupported format in 'cmap' table: {0}", subtableOffset));

            uint subtableLength = ReadUInt16Bytes();
            Skip(2);
            uint segCount = ReadUInt16Bytes() / 2;
            Skip(6);
            List<uint> endCounts = Enumerable.Range(0, (int)segCount).Select(x => ReadUInt16Bytes()).ToList();
            Skip(2);
            List<uint> startCounts = Enumerable.Range(0, (int)segCount).Select(x => ReadUInt16Bytes()).ToList();
            List<int> idDeltas = Enumerable.Range(0, (int)segCount).Select(x => ReadInt16Bytes()).ToList();
            List<uint> rangeOffsets = Enumerable.Range(0, (int)segCount).Select(x => ReadUInt16Bytes()).ToList();
            uint remainingLength = subtableLength - (8 * segCount) - 16;
            if (remainingLength <= 0)
                remainingLength += 0x10000;

            List<uint> glyphIdArray = Enumerable.Range(0, (int)remainingLength / 2).Select(x => ReadUInt16Bytes()).ToList();
            for (int i = 0; i < segCount; i++)
            {
                for (uint c = startCounts[i]; c < endCounts[i] + 1; c++)
                {
                    if (c == 0xFFFF)
                        continue;

                    uint gid;
                    if (rangeOffsets[i] > 0)
                    {
                        uint idx = (c - startCounts[i]) + (rangeOffsets[i] / 2) - (segCount - (uint)i);
                        gid = glyphIdArray[(int)idx];
                    }
                    else
                    {
                        gid = c + ((uint)idDeltas[i]);
                    }
                    if (gid >= 0x10000)
                        gid -= 0x10000;
                    else if (gid < 0)
                        gid += 0x10000;

                    CharMetric cm = glyphArray[(int)gid];
                    cm.Codes.Add(c);
                    if (encodingScheme == "Symbol" && (0xF020 <= c && c <= 0xF07F))
                        cm.Codes.Add(c - 0xF000);

                    if (string.IsNullOrEmpty(cm.GlyphName))
                        cm.GlyphName = string.Format("u{0:X4}", c);
                }
            }

            SwitchTables("loca");
            SkipTo(m_currentOffset);
            List<uint> glyphIndex = new List<uint>();
            int scalefactor = m_indexToLocFormat + 1;

            if (m_indexToLocFormat == 0)
                glyphIndex.AddRange(Enumerable.Range(0, (int)m_numGlyphs + 1).Select(x => ReadUInt16Bytes() * 2));
            else if (m_indexToLocFormat == 1)
                glyphIndex.AddRange(Enumerable.Range(0, (int)m_numGlyphs + 1).Select(x => ReadUInt32Bytes()));
            else
                throw new InvalidDataException(string.Format("Invalid indexToLocFormat value ({0}) in 'head' table", m_indexToLocFormat));

            SwitchTables("glyf");
            for (int i = 0; i < m_numGlyphs; i++)
            {
                CharMetric cm = glyphArray[i];
                if (glyphIndex[i] == glyphIndex[i + 1])
                {
                    cm.SetBBox(0, 0, 0, 0); // empty glyph
                }
                else
                {
                    SkipTo(m_currentOffset + glyphIndex[i] + 2);
                    double xMinc = ReadInt16Bytes() * EmScale;
                    double yMinc = ReadInt16Bytes() * EmScale;
                    double xMaxc = ReadInt16Bytes() * EmScale;
                    double yMaxc = ReadInt16Bytes() * EmScale;
                    cm.SetBBox(xMinc, yMinc, xMaxc, yMaxc);
                }
                cm.Codes.ForEach(x =>
                {
                    CharMetric ccm = new CharMetric(
                        charName: cm.GlyphName,
                        codes: cm.Codes.Select(y => (uint)y).ToList(),
                        width: cm.Width,
                        bbox: cm.BBox);
                    CharData.Add((int)x, ccm);
                });
            }
        }

        private void SwitchTables(string headId)
        {
            m_currentOffset = m_tables[headId].Item1;
            m_currentLength = m_tables[headId].Item2;
        }

        private string GetName(uint code)
        {
            if (m_macNames.ContainsKey(code))
            {
                uint nameOffset = m_macNames[code].Item1;
                uint nameLength = m_macNames[code].Item2;
                SkipTo(m_storageOffset);
                Skip(Convert.ToInt16(nameOffset));
                return Encoding.UTF8.GetString(m_reader.ReadBytes(Convert.ToInt16(nameLength)));
            }
            else if (m_uniNames.ContainsKey(code))
            {
                uint nameOffset = m_uniNames[code].Item1;
                uint nameLength = m_uniNames[code].Item2;
                byte[] buff = new byte[nameLength];
                SkipTo(m_storageOffset);
                Skip(Convert.ToInt16(nameOffset));
                StringBuilder result = new StringBuilder();
                for (int i = 0; i < nameLength / 2; i++)
                {
                    //result.Append(Array.ConvertAll(m_reader.ReadChars(2), b => b.ToString()));
                    result.Append(m_reader.ReadChars(2).Select(x => x.ToString()).ToArray());
                }
                return result.ToString();
            }

            throw new Exception("Could not read name");
        }

        private void Skip(int size)
        {
            m_reader.ReadBytes(size);
        }

        private void SkipTo(uint offset)
        {
            m_reader.BaseStream.Seek(offset, SeekOrigin.Begin);
        }

        private uint ReadUInt32Bytes()
        {
            return BitConverter.ToUInt32(ConvertBytes(BitConverter.GetBytes(m_reader.ReadUInt32())), 0);
        }

        private uint ReadUInt16Bytes()
        {
            return BitConverter.ToUInt16(ConvertBytes(BitConverter.GetBytes(m_reader.ReadUInt16())), 0);
        }

        private int ReadInt16Bytes()
        {
            return BitConverter.ToInt16(ConvertBytes(BitConverter.GetBytes(m_reader.ReadInt16())), 0);
        }

        private Int32 ReadFixed32()
        {
            return BitConverter.ToInt32(ConvertBytes(BitConverter.GetBytes(m_reader.ReadInt32())), 0) / 65536;
        }

        private byte[] ConvertBytes(byte[] byteBuff)
        {
            byte[] buff = byteBuff;
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buff);

            return buff;
        }

        private uint m_currentOffset = 0;
        private uint m_currentLength = 0;
        private uint m_storageOffset = 0;
        private BinaryReader m_reader;
        private const int c_ttfVersion = 0x100;
        private const string c_otfAbbreviation = "OTTO";
        private const string c_ttfAbbreviation = "TTF";
        private Dictionary<string, Tuple<uint, uint>> m_tables = new Dictionary<string, Tuple<uint, uint>>();
        private uint m_unitsPerEm;
        public uint UnitsPerEm { get { return m_unitsPerEm; } }
        private double m_eMScale;
        public double EmScale { get { return m_eMScale; } }
        private int m_indexToLocFormat = 0;
        private uint m_numGlyphs = 0;
        private Dictionary<uint, Tuple<uint, uint>> m_uniNames = new Dictionary<uint, Tuple<uint, uint>>();
        private Dictionary<uint, Tuple<uint, uint>> m_macNames = new Dictionary<uint, Tuple<uint, uint>>();
        private uint[] englishCodes = new uint[] { 0x409, 0x809, 0xC09, 0x1009, 0x1409, 0x1809 };
        public bool IsItalicFont { get { return m_italicAngle > 0; } }
    }
}