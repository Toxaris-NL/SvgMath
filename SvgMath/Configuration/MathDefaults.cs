using System.Collections.Generic;

namespace SvgMath
{
    internal class MathDefaults
    {
        public static List<string> Embellishments = new List<string>() { "msub", "msup", "msubsup", "munder", "mover", "munderover", "mmultiscripts" };

        public static Dictionary<int, char> SpecialChars = new Dictionary<int, char>(){
            { '\u2145', 'D' },
            { '\u2146', 'd' },
            { '\u2147', 'e'},
            { '\u2148', 'i'},
            { '\u00A0', ' ' }
        };

        public static Dictionary<string, string> RuleWidthKeywords = new Dictionary<string, string>(){
            { "medium","1" },
            { "thin", "0.5" },
            { "thick", "2"}
        };

        public static Dictionary<string, string> globalDefaults = new Dictionary<string, string>()
        {
			// Font and color properties
			{"mathvariant", "normal"},
            {"mathsize", "12pt"},
            {"mathcolor", "black"},
            {"mathbackground", "transparent"},
            {"displaystyle", "false"},
            {"scriptlevel", "0"},
			// Script size factor and minimum value
			{"scriptsizemultiplier", "0.71"},
            {"scriptminsize", "8pt"},
			// Spaces
			{"veryverythinmathspace", "0.0555556em"},
            {"verythinmathspace", "0.111111em"},
            {"thinmathspace", "0.166667em"},
            {"mediummathspace", "0.222222em"},
            {"thickmathspace", "0.277778em"},
            {"verythickmathspace", "0.333333em"},
            {"veryverythickmathspace", "0.388889em"},
			// Line thickness and slope for mfrac
			{"linethickness", "1"},
            {"bevelled", "false"},
            {"enumalign", "center"},
            {"denomalign", "center"},
			// "quotes for ms
			{"lquote", "\""},
            {"rquote", "\""},
			// Properties for mspace
			{"height", "0ex"},
            {"depth", "0ex"},
            {"width", "0em"},
			// Properties for mfenced
			{"open", "("},
            {"close", ")"},
            {"separators", "},"},
			// Property for menclose
			{"notation", "longdiv"},
			// Properties for mtable
			{"align", "axis"},
            {"rowalign", "baseline"},
            {"columnalign", "center"},
            {"columnwidth", "auto"},
            {"equalrows", "false"},
            {"equalcolumns", "false"},
            {"rowspacing", "1.0ex"},
            {"columnspacing", "0.8em"},
            {"framespacing", "0.4em 0.5ex"},
            {"rowlines", "none"},
            {"columnlines", "none"},
            {"frame", "none"}
        };
    }
}