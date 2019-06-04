using System.Collections.Generic;
using System.Linq;

namespace SvgMath
{
    public class MathOperators
    {
        public MathOperators()
        {
            OperatorList = new List<MathOperator>
            {
                new MathOperator(""),
                new MathOperator("(") { Form = "prefix", Fence = "true", Stretchy = "true", Scaling = "vertical", LSpace = "0em", RSpace = "0em" },
                new MathOperator(")") { Form = "postfix", Fence = "true", Stretchy = "true", Scaling = "vertical", LSpace = "0em", RSpace = "0em" },
                new MathOperator("[") { Form = "prefix", Fence = "true", Stretchy = "true", Scaling = "vertical", LSpace = "0em", RSpace = "0em" },
                new MathOperator("]") { Form = "postfix", Fence = "true", Stretchy = "true", Scaling = "vertical", LSpace = "0em", RSpace = "0em" },
                new MathOperator("{") { Form = "prefix", Fence = "true", Stretchy = "true", Scaling = "vertical", LSpace = "0em", RSpace = "0em" },
                new MathOperator("}") { Form = "postfix", Fence = "true", Stretchy = "true", Scaling = "vertical", LSpace = "0em", RSpace = "0em" },
                new MathOperator("\u222C") { Form = "prefix", LargeOp = "true", Stretchy = "true", Scaling = "uniform", LSpace = "0em", RSpace = "0em" }, // Double Integral
                new MathOperator("\u23DE") { Form = "postfix", LargeOp = "true", Stretchy = "true", Scaling = "horizontal", LSpace = "0em", RSpace = "0em" }, // Double Integral
                new MathOperator("\u201D") { Form = "postfix", Fence = "true", LSpace = "0em", RSpace = "0em" }, // CloseCurlyDoubleQuote
                new MathOperator("\u2019") { Form = "postfix", Fence = "true", LSpace = "0em", RSpace = "0em" }, // CloseCurlyQuote
                new MathOperator("\u2329") { Form = "prefix", Fence = "true", Stretchy = "true", Scaling = "vertical", LSpace = "0em", RSpace = "0em" }, // LeftAngleBracket
                new MathOperator("\u2308") { Form = "prefix", Fence = "true", Stretchy = "true", Scaling = "vertical", LSpace = "0em", RSpace = "0em" }, // LeftCeiling
                new MathOperator("\u301A") { Form = "prefix", Fence = "true", Stretchy = "true", Scaling = "vertical", LSpace = "0em", RSpace = "0em" }, // LeftDoubleBracket
                new MathOperator("\u230A") { Form = "prefix", Fence = "true", Stretchy = "true", Scaling = "vertical", LSpace = "0em", RSpace = "0em" }, // LeftFloor
                new MathOperator("\u201C") { Form = "prefix", Fence = "true", LSpace = "0em", RSpace = "0em" }, // OpenCurlyDoubleQuote
                new MathOperator("\u2018") { Form = "prefix", Fence = "true", LSpace = "0em", RSpace = "0em" }, // OpenCurlyQuote
                new MathOperator("\u232A") { Form = "postfix", Fence = "true", Stretchy = "true", Scaling = "vertical", LSpace = "0em", RSpace = "0em" }, // RightAngleBracket
                new MathOperator("\u2309") { Form = "postfix", Fence = "true", Stretchy = "true", Scaling = "vertical", LSpace = "0em", RSpace = "0em" }, // RightCeiling
                new MathOperator("\u301B") { Form = "postfix", Fence = "true", Stretchy = "true", Scaling = "vertical", LSpace = "0em", RSpace = "0em" }, // RightDoubleBracket
                new MathOperator("\u230B") { Form = "postfix", Fence = "true", Stretchy = "true", Scaling = "vertical", LSpace = "0em", RSpace = "0em" }, // RightFloor
                new MathOperator("\u2063") { Form = "infix", Separator = "true", LSpace = "0em", RSpace = "0em" }, // InvisibleComma
                new MathOperator(",") { Form = "infix", Separator = "true", LSpace = "0em", RSpace = "verythickmathspace" },
                new MathOperator("\u2500") { Form = "infix", Stretchy = "true", Scaling = "horizontal", MinSize = "0", LSpace = "0em", RSpace = "0em" }, // HorizontalLine
                                                                                                                                                         // Commented out: collides with '|'. See http://lists.w3.org/Archives/Public/www-math/2004Mar/0028.html
                                                                                                                                                         // OperatorList.Add(new MathOperator("|"){Form="infix", Stretchy="true", Scaling="vertical", minsize="0", LSpace="0em", RSpace="0em" }); // VerticalLine
                new MathOperator(";") { Form = "infix", Separator = "true", LSpace = "0em", RSpace = "thickmathspace" },
                new MathOperator(";") { Form = "postfix", Separator = "true", LSpace = "0em", RSpace = "0em" },
                new MathOperator(":=") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" },
                new MathOperator("\u2254") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // Assign
                new MathOperator("\u2235") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // Because
                new MathOperator("\u2234") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // Therefore
                new MathOperator("\u2758") { Form = "infix", Stretchy = "true", Scaling = "vertical", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // VerticalSeparator
                new MathOperator("//") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" },
                // Commented out: collides with Proportional
                // OperatorList.Add(new MathOperator("\u2237"){Form="infix", LSpace="thickmathspace", RSpace="thickmathspace" }); // Colon
                new MathOperator("&") { Form = "prefix", LSpace = "0em", RSpace = "thickmathspace" },
                new MathOperator("&") { Form = "postfix", LSpace = "thickmathspace", RSpace = "0em" },
                new MathOperator("*=") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" },
                new MathOperator("-=") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" },
                new MathOperator("+=") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" },
                new MathOperator("/=") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" },
                new MathOperator("->") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" },
                new MathOperator(":") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" },
                new MathOperator("..") { Form = "postfix", LSpace = "mediummathspace", RSpace = "0em" },
                new MathOperator("...") { Form = "postfix", LSpace = "mediummathspace", RSpace = "0em" },
                // Commented out: collides with ReverseElement
                // OperatorList.Add(new MathOperator("\u220B"){Form="infix", LSpace="thickmathspace", RSpace="thickmathspace" }); // SuchThat
                new MathOperator("\u2AE4") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // DoubleLeftTee
                new MathOperator("\u22A8") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // DoubleRightTee
                new MathOperator("\u22A4") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // DownTee
                new MathOperator("\u22A3") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // LeftTee
                new MathOperator("\u22A2") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // RightTee
                                                                                                                     // Commented out: collides with DoubleRightArrow
                                                                                                                     // OperatorList.Add(new MathOperator("\u21D2"){Form="infix", Stretchy="true", Scaling="horizontal", LSpace="thickmathspace", RSpace="thickmathspace") // Implies
                new MathOperator("\u2970") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // RoundImplies
                new MathOperator("|") { Form = "infix", Stretchy = "true", Scaling = "vertical", LSpace = "thickmathspace", RSpace = "thickmathspace" },
                new MathOperator("||") { Form = "infix", LSpace = "mediummathspace", RSpace = "mediummathspace" },
                new MathOperator("\u2A54") { Form = "infix", Stretchy = "true", Scaling = "uniform", LSpace = "mediummathspace", RSpace = "mediummathspace" }, // Or
                new MathOperator("&&") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" },
                new MathOperator("\u2A53") { Form = "infix", Stretchy = "true", Scaling = "uniform", LSpace = "mediummathspace", RSpace = "mediummathspace" }, // And
                new MathOperator("&") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" },
                new MathOperator("!") { Form = "prefix", LSpace = "0em", RSpace = "thickmathspace" },
                new MathOperator("\u2AEC") { Form = "prefix", LSpace = "0em", RSpace = "thickmathspace" }, // Not
                new MathOperator("\u2203") { Form = "prefix", LSpace = "0em", RSpace = "thickmathspace" }, // Exists
                new MathOperator("\u2200") { Form = "prefix", LSpace = "0em", RSpace = "thickmathspace" }, // ForAll
                new MathOperator("\u2204") { Form = "prefix", LSpace = "0em", RSpace = "thickmathspace" }, // NotExists
                new MathOperator("\u2208") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // Element
                new MathOperator("\u2209") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotElement
                new MathOperator("\u220C") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotReverseElement
                new MathOperator("\u228F\u0338") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotSquareSubset
                new MathOperator("\u22E2") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotSquareSubsetEqual
                new MathOperator("\u2290\u0338") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotSquareSuperset
                new MathOperator("\u22E3") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotSquareSupersetEqual
                new MathOperator("\u2282\u20D2") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotSubset
                new MathOperator("\u2288") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotSubsetEqual
                new MathOperator("\u2283\u20D2") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotSuperset
                new MathOperator("\u2289") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotSupersetEqual
                new MathOperator("\u220B") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // ReverseElement
                new MathOperator("\u228F") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // SquareSubset
                new MathOperator("\u2291") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // SquareSubsetEqual
                new MathOperator("\u2290") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // SquareSuperset
                new MathOperator("\u2292") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // SquareSupersetEqual
                new MathOperator("\u22D0") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // Subset
                new MathOperator("\u2286") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // SubsetEqual
                new MathOperator("\u2283") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // Superset
                new MathOperator("\u2287") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // SupersetEqual
                new MathOperator("\u21D0") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // DoubleLeftArrow
                new MathOperator("\u21D4") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // DoubleLeftRightArrow
                new MathOperator("\u21D2") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // DoubleRightArrow
                new MathOperator("\u2950") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // DownLeftRightVector
                new MathOperator("\u295E") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // DownLeftTeeVector
                new MathOperator("\u21BD") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // DownLeftVector
                new MathOperator("\u2956") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // DownLeftVectorBar
                new MathOperator("\u295F") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // DownRightTeeVector
                new MathOperator("\u21C1") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // DownRightVector
                new MathOperator("\u2957") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // DownRightVectorBar
                new MathOperator("\u2190") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // LeftArrow
                new MathOperator("\u21E4") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // LeftArrowBar
                new MathOperator("\u21C6") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // LeftArrowRightArrow
                new MathOperator("\u2194") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // LeftRightArrow
                new MathOperator("\u294E") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // LeftRightVector
                new MathOperator("\u21A4") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // LeftTeeArrow
                new MathOperator("\u295A") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // LeftTeeVector
                new MathOperator("\u21BC") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // LeftVector
                new MathOperator("\u2952") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // LeftVectorBar
                new MathOperator("\u2199") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // LowerLeftArrow
                new MathOperator("\u2198") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // LowerRightArrow
                new MathOperator("\u2192") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // RightArrow
                new MathOperator("\u21E5") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // RightArrowBar
                new MathOperator("\u21C4") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // RightArrowLeftArrow
                new MathOperator("\u21A6") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // RightTeeArrow
                new MathOperator("\u295B") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // RightTeeVector
                new MathOperator("\u21C0") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // RightVector
                new MathOperator("\u2953") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // RightVectorBar
                                                                                                                                                                // Commented out: collides with LeftArrow
                                                                                                                                                                // OperatorList.Add(new MathOperator("\u2190"){Form="infix", LSpace="thickmathspace", RSpace="thickmathspace" }); // ShortLeftArrow
                                                                                                                                                                // Commented out: collides with RightArrow
                                                                                                                                                                // OperatorList.Add(new MathOperator("\u2192"){Form="infix", LSpace="thickmathspace", RSpace="thickmathspace" }); // ShortRightArrow
                new MathOperator("\u2196") { Form = "infix", Stretchy = "true", Scaling = "uniform", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // UpperLeftArrow
                new MathOperator("\u2197") { Form = "infix", Stretchy = "true", Scaling = "uniform", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // UpperRightArrow
                new MathOperator("=") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" },
                new MathOperator("<") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" },
                new MathOperator(">") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" },
                new MathOperator("!=") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" },
                new MathOperator("==") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" },
                new MathOperator("<=") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" },
                new MathOperator(">=") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" },
                new MathOperator("\u2261") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // Congruent
                new MathOperator("\u224D") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // CupCap
                new MathOperator("\u2250") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // DotEqual
                new MathOperator("\u2225") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // DoubleVerticalBar
                new MathOperator("\u2A75") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // Equal
                new MathOperator("\u2242") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // EqualTilde
                new MathOperator("\u21CC") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // Equilibrium
                new MathOperator("\u2265") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // GreaterEqual
                new MathOperator("\u22DB") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // GreaterEqualLess
                new MathOperator("\u2267") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // GreaterFullEqual
                new MathOperator("\u2AA2") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // GreaterGreater
                new MathOperator("\u2277") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // GreaterLess
                new MathOperator("\u2A7E") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // GreaterSlantEqual
                new MathOperator("\u2273") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // GreaterTilde
                new MathOperator("\u224E") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // HumpDownHump
                new MathOperator("\u224F") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // HumpEqual
                new MathOperator("\u22B2") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // LeftTriangle
                new MathOperator("\u29CF") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // LeftTriangleBar
                new MathOperator("\u22B4") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // LeftTriangleEqual
                new MathOperator("\u2264") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // le
                new MathOperator("\u22DA") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // LessEqualGreater
                new MathOperator("\u2266") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // LessFullEqual
                new MathOperator("\u2276") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // LessGreater
                new MathOperator("\u2AA1") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // LessLess
                new MathOperator("\u2A7D") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // LessSlantEqual
                new MathOperator("\u2272") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // LessTilde
                new MathOperator("\u226B") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NestedGreaterGreater
                new MathOperator("\u226A") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NestedLessLess
                new MathOperator("\u2262") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotCongruent
                new MathOperator("\u226D") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotCupCap
                new MathOperator("\u2226") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotDoubleVerticalBar
                new MathOperator("\u2260") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotEqual
                new MathOperator("\u2242\u0338") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotEqualTilde
                new MathOperator("\u226F") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotGreater
                new MathOperator("\u2271") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotGreaterEqual
                new MathOperator("\u2266\u0338") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotGreaterFullEqual
                new MathOperator("\u226B\u0338") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotGreaterGreater
                new MathOperator("\u2279") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotGreaterLess
                new MathOperator("\u2A7E\u0338") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotGreaterSlantEqual
                new MathOperator("\u2275") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotGreaterTilde
                new MathOperator("\u224E\u0338") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotHumpDownHump
                new MathOperator("\u224F\u0338") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotHumpEqual
                new MathOperator("\u22EA") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotLeftTriangle
                new MathOperator("\u29CF\u0338") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotLeftTriangleBar
                new MathOperator("\u22EC") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotLeftTriangleEqual
                new MathOperator("\u226E") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotLess
                new MathOperator("\u2270") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotLessEqual
                new MathOperator("\u2278") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotLessGreater
                new MathOperator("\u226A\u0338") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotLessLess
                new MathOperator("\u2A7D\u0338") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotLessSlantEqual
                new MathOperator("\u2274") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotLessTilde
                new MathOperator("\u2AA2\u0338") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotNestedGreaterGreater
                new MathOperator("\u2AA1\u0338") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotNestedLessLess
                new MathOperator("\u2280") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotPrecedes
                new MathOperator("\u2AAF\u0338") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotPrecedesEqual
                new MathOperator("\u22E0") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotPrecedesSlantEqual
                new MathOperator("\u22EB") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotRightTriangle
                new MathOperator("\u29D0\u0338") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotRightTriangleBar
                new MathOperator("\u22ED") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotRightTriangleEqual
                new MathOperator("\u2281") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotSucceeds
                new MathOperator("\u2AB0\u0338") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotSucceedsEqual
                new MathOperator("\u22E1") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotSucceedsSlantEqual
                new MathOperator("\u227F\u0338") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotSucceedsTilde
                new MathOperator("\u2241") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotTilde
                new MathOperator("\u2244") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotTildeEqual
                new MathOperator("\u2247") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotTildeFullEqual
                new MathOperator("\u2249") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotTildeTilde
                new MathOperator("\u2224") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // NotVerticalBar
                new MathOperator("\u227A") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // Precedes
                new MathOperator("\u2AAF") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // PrecedesEqual
                new MathOperator("\u227C") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // PrecedesSlantEqual
                new MathOperator("\u227E") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // PrecedesTilde
                new MathOperator("\u2237") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // Proportion
                new MathOperator("\u221D") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // Proportional
                new MathOperator("\u21CB") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // ReverseEquilibrium
                new MathOperator("\u22B3") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // RightTriangle
                new MathOperator("\u29D0") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // RightTriangleBar
                new MathOperator("\u22B5") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // RightTriangleEqual
                new MathOperator("\u227B") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // Succeeds
                new MathOperator("\u2AB0") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // SucceedsEqual
                new MathOperator("\u227D") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // SucceedsSlantEqual
                new MathOperator("\u227F") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // SucceedsTilde
                new MathOperator("\u223C") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // Tilde
                new MathOperator("\u2243") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // TildeEqual
                new MathOperator("\u2245") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // TildeFullEqual
                new MathOperator("\u2248") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // TildeTilde
                new MathOperator("\u22A5") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // UpTee
                new MathOperator("\u2223") { Form = "infix", LSpace = "thickmathspace", RSpace = "thickmathspace" }, // VerticalBar
                new MathOperator("\u2294") { Form = "infix", Stretchy = "true", Scaling = "uniform", LSpace = "mediummathspace", RSpace = "mediummathspace" }, // SquareUnion
                new MathOperator("\u22C3") { Form = "infix", Stretchy = "true", Scaling = "uniform", LSpace = "mediummathspace", RSpace = "mediummathspace" }, // Union
                new MathOperator("\u228E") { Form = "infix", Stretchy = "true", Scaling = "uniform", LSpace = "mediummathspace", RSpace = "mediummathspace" }, // UnionPlus
                new MathOperator("-") { Form = "infix", LSpace = "mediummathspace", RSpace = "mediummathspace" },
                // Added an entry for minus sign, separate from hyphen
                new MathOperator("\u2212") { Form = "infix", LSpace = "mediummathspace", RSpace = "mediummathspace" },
                new MathOperator("+") { Form = "infix", LSpace = "mediummathspace", RSpace = "mediummathspace" },
                new MathOperator("\u22C2") { Form = "infix", Stretchy = "true", Scaling = "uniform", LSpace = "mediummathspace", RSpace = "mediummathspace" }, // Intersection
                new MathOperator("\u2213") { Form = "infix", LSpace = "mediummathspace", RSpace = "mediummathspace" }, // MinusPlus
                new MathOperator("\u00B1") { Form = "infix", LSpace = "mediummathspace", RSpace = "mediummathspace" }, // PlusMinus
                new MathOperator("\u2293") { Form = "infix", Stretchy = "true", Scaling = "uniform", LSpace = "mediummathspace", RSpace = "mediummathspace" }, // SquareIntersection
                new MathOperator("\u22C1") { Form = "prefix", LargeOp = "true", MovableLimits = "true", Stretchy = "true", Scaling = "uniform", LSpace = "0em", RSpace = "thinmathspace" }, // Vee
                new MathOperator("\u2296") { Form = "prefix", LargeOp = "true", MovableLimits = "true", LSpace = "0em", RSpace = "thinmathspace" }, // CircleMinus
                new MathOperator("\u2295") { Form = "prefix", LargeOp = "true", MovableLimits = "true", LSpace = "0em", RSpace = "thinmathspace" }, // CirclePlus
                new MathOperator("\u2211") { Form = "prefix", LargeOp = "true", MovableLimits = "true", Stretchy = "true", Scaling = "uniform", LSpace = "0em", RSpace = "thinmathspace" }, // Sum
                new MathOperator("\u22C3") { Form = "prefix", LargeOp = "true", MovableLimits = "true", Stretchy = "true", Scaling = "uniform", LSpace = "0em", RSpace = "thinmathspace" }, // Union
                new MathOperator("\u228E") { Form = "prefix", LargeOp = "true", MovableLimits = "true", Stretchy = "true", Scaling = "uniform", LSpace = "0em", RSpace = "thinmathspace" }, // UnionPlus
                new MathOperator("lim") { Form = "prefix", MovableLimits = "true", LSpace = "0em", RSpace = "thinmathspace" },
                new MathOperator("max") { Form = "prefix", MovableLimits = "true", LSpace = "0em", RSpace = "thinmathspace" },
                new MathOperator("min") { Form = "prefix", MovableLimits = "true", LSpace = "0em", RSpace = "thinmathspace" },
                new MathOperator("\u2296") { Form = "infix", LSpace = "thinmathspace", RSpace = "thinmathspace" }, // CircleMinus
                new MathOperator("\u2295") { Form = "infix", LSpace = "thinmathspace", RSpace = "thinmathspace" }, // CirclePlus
                new MathOperator("\u2232") { Form = "prefix", LargeOp = "true", Stretchy = "true", Scaling = "uniform", LSpace = "0em", RSpace = "0em" }, // ClockwiseContourIntegral
                new MathOperator("\u222E") { Form = "prefix", LargeOp = "true", Stretchy = "true", Scaling = "uniform", LSpace = "0em", RSpace = "0em" }, // ContourIntegral
                new MathOperator("\u2233") { Form = "prefix", LargeOp = "true", Stretchy = "true", Scaling = "uniform", LSpace = "0em", RSpace = "0em" }, // CounterClockwiseContourIntegral
                new MathOperator("\u222F") { Form = "prefix", LargeOp = "true", Stretchy = "true", Scaling = "uniform", LSpace = "0em", RSpace = "0em" }, // DoubleContourIntegral
                new MathOperator("\u222B") { Form = "prefix", LargeOp = "true", Stretchy = "true", Scaling = "uniform", LSpace = "0em", RSpace = "0em" }, // Integral
                new MathOperator("\u22D3") { Form = "infix", LSpace = "thinmathspace", RSpace = "thinmathspace" }, // Cup
                new MathOperator("\u22D2") { Form = "infix", LSpace = "thinmathspace", RSpace = "thinmathspace" }, // Cap
                new MathOperator("\u2240") { Form = "infix", LSpace = "thinmathspace", RSpace = "thinmathspace" }, // VerticalTilde
                new MathOperator("\u22C0") { Form = "prefix", LargeOp = "true", MovableLimits = "true", Stretchy = "true", Scaling = "uniform", LSpace = "0em", RSpace = "thinmathspace" }, // Wedge
                new MathOperator("\u2297") { Form = "prefix", LargeOp = "true", MovableLimits = "true", LSpace = "0em", RSpace = "thinmathspace" }, // CircleTimes
                new MathOperator("\u2210") { Form = "prefix", LargeOp = "true", MovableLimits = "true", Stretchy = "true", Scaling = "uniform", LSpace = "0em", RSpace = "thinmathspace" }, // Coproduct
                new MathOperator("\u220F") { Form = "prefix", LargeOp = "true", MovableLimits = "true", Stretchy = "true", Scaling = "uniform", LSpace = "0em", RSpace = "thinmathspace" }, // Product
                new MathOperator("\u22C2") { Form = "prefix", LargeOp = "true", MovableLimits = "true", Stretchy = "true", Scaling = "uniform", LSpace = "0em", RSpace = "thinmathspace" }, // Intersection
                new MathOperator("\u2210") { Form = "infix", LSpace = "thinmathspace", RSpace = "thinmathspace" }, // Coproduct
                new MathOperator("\u22C6") { Form = "infix", LSpace = "thinmathspace", RSpace = "thinmathspace" }, // Star
                new MathOperator("\u2299") { Form = "prefix", LargeOp = "true", MovableLimits = "true", LSpace = "0em", RSpace = "thinmathspace" }, // CircleDot
                new MathOperator("*") { Form = "infix", LSpace = "thinmathspace", RSpace = "thinmathspace" },
                new MathOperator("\u2062") { Form = "infix", LSpace = "0em", RSpace = "0em" }, // InvisibleTimes
                new MathOperator("\u00B7") { Form = "infix", LSpace = "thinmathspace", RSpace = "thinmathspace" }, // CenterDot
                new MathOperator("\u2297") { Form = "infix", LSpace = "thinmathspace", RSpace = "thinmathspace" }, // CircleTimes
                new MathOperator("\u22C1") { Form = "infix", LSpace = "thinmathspace", RSpace = "thinmathspace" }, // Vee
                new MathOperator("\u22C0") { Form = "infix", LSpace = "thinmathspace", RSpace = "thinmathspace" }, // Wedge
                new MathOperator("\u22C4") { Form = "infix", LSpace = "thinmathspace", RSpace = "thinmathspace" }, // Diamond
                new MathOperator("\u2216") { Form = "infix", Stretchy = "true", Scaling = "uniform", LSpace = "thinmathspace", RSpace = "thinmathspace" }, // Backslash
                new MathOperator("/") { Form = "infix", Stretchy = "true", Scaling = "uniform", LSpace = "thinmathspace", RSpace = "thinmathspace" },
                new MathOperator("-") { Form = "prefix", LSpace = "0em", RSpace = "veryverythinmathspace" },
                // Added an entry for minus sign, separate from hyphen
                new MathOperator("\u2212") { Form = "prefix", LSpace = "0em", RSpace = "veryverythinmathspace" },
                new MathOperator("+") { Form = "prefix", LSpace = "0em", RSpace = "veryverythinmathspace" },
                new MathOperator("\u2213") { Form = "prefix", LSpace = "0em", RSpace = "veryverythinmathspace" }, // MinusPlus
                new MathOperator("\u00B1") { Form = "prefix", LSpace = "0em", RSpace = "veryverythinmathspace" }, // PlusMinus
                new MathOperator(".") { Form = "infix", LSpace = "0em", RSpace = "0em" },
                new MathOperator("\u2A2F") { Form = "infix", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // Cross
                new MathOperator("**") { Form = "infix", LSpace = "verythinmathspace", RSpace = "verythinmathspace" },
                new MathOperator("\u2299") { Form = "infix", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // CircleDot
                new MathOperator("\u2218") { Form = "infix", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // SmallCircle
                new MathOperator("\u25A1") { Form = "prefix", LSpace = "0em", RSpace = "verythinmathspace" }, // Square
                new MathOperator("\u2207") { Form = "prefix", LSpace = "0em", RSpace = "verythinmathspace" }, // Del
                new MathOperator("\u2202") { Form = "prefix", LSpace = "0em", RSpace = "verythinmathspace" }, // PartialD
                new MathOperator("\u2145") { Form = "prefix", LSpace = "0em", RSpace = "verythinmathspace" }, // CapitalDifferentialD
                new MathOperator("\u2146") { Form = "prefix", LSpace = "0em", RSpace = "verythinmathspace" }, // DifferentialD
                new MathOperator("\u221A") { Form = "prefix", Stretchy = "true", Scaling = "uniform", LSpace = "0em", RSpace = "verythinmathspace" }, // Sqrt
                new MathOperator("\u21D3") { Form = "infix", Stretchy = "true", Scaling = "vertical", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // DoubleDownArrow
                new MathOperator("\u27F8") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // DoubleLongLeftArrow
                new MathOperator("\u27FA") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // DoubleLongLeftRightArrow
                new MathOperator("\u27F9") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // DoubleLongRightArrow
                new MathOperator("\u21D1") { Form = "infix", Stretchy = "true", Scaling = "vertical", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // DoubleUpArrow
                new MathOperator("\u21D5") { Form = "infix", Stretchy = "true", Scaling = "vertical", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // DoubleUpDownArrow
                new MathOperator("\u2193") { Form = "infix", Stretchy = "true", Scaling = "vertical", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // DownArrow
                new MathOperator("\u2913") { Form = "infix", Stretchy = "true", Scaling = "vertical", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // DownArrowBar
                new MathOperator("\u21F5") { Form = "infix", Stretchy = "true", Scaling = "vertical", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // DownArrowUpArrow
                new MathOperator("\u21A7") { Form = "infix", Stretchy = "true", Scaling = "vertical", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // DownTeeArrow
                new MathOperator("\u2961") { Form = "infix", Stretchy = "true", Scaling = "uniform", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // LeftDownTeeVector
                new MathOperator("\u21C3") { Form = "infix", Stretchy = "true", Scaling = "uniform", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // LeftDownVector
                new MathOperator("\u2959") { Form = "infix", Stretchy = "true", Scaling = "uniform", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // LeftDownVectorBar
                new MathOperator("\u2951") { Form = "infix", Stretchy = "true", Scaling = "uniform", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // LeftUpDownVector
                new MathOperator("\u2960") { Form = "infix", Stretchy = "true", Scaling = "uniform", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // LeftUpTeeVector
                new MathOperator("\u21BF") { Form = "infix", Stretchy = "true", Scaling = "uniform", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // LeftUpVector
                new MathOperator("\u2958") { Form = "infix", Stretchy = "true", Scaling = "uniform", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // LeftUpVectorBar
                new MathOperator("\u27F5") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // LongLeftArrow
                new MathOperator("\u27F7") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // LongLeftRightArrow
                new MathOperator("\u27F6") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // LongRightArrow
                new MathOperator("\u296F") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // ReverseUpEquilibrium
                new MathOperator("\u295D") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // RightDownTeeVector
                new MathOperator("\u21C2") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // RightDownVector
                new MathOperator("\u2955") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // RightDownVectorBar
                new MathOperator("\u294F") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // RightUpDownVector
                new MathOperator("\u295C") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // RightUpTeeVector
                new MathOperator("\u21BE") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // RightUpVector
                new MathOperator("\u2954") { Form = "infix", Stretchy = "true", Scaling = "horizontal", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // RightUpVectorBar
                                                                                                                                                                      // Commented out: collides with DownArrow
                                                                                                                                                                      // OperatorList.Add(new MathOperator("\u2193"){Form="infix", LSpace="verythinmathspace", RSpace="verythinmathspace" }); // ShortDownArrow
                                                                                                                                                                      // Commented out: collides with UpArrow
                                                                                                                                                                      // OperatorList.Add(new MathOperator("\u2191"){Form="infix", LSpace="verythinmathspace", RSpace="verythinmathspace" }); // ShortUpArrow
                new MathOperator("\u2191") { Form = "infix", Stretchy = "true", Scaling = "vertical", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // UpArrow
                new MathOperator("\u2912") { Form = "infix", Stretchy = "true", Scaling = "vertical", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // UpArrowBar
                new MathOperator("\u21C5") { Form = "infix", Stretchy = "true", Scaling = "vertical", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // UpArrowDownArrow
                new MathOperator("\u2195") { Form = "infix", Stretchy = "true", Scaling = "vertical", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // UpDownArrow
                new MathOperator("\u296E") { Form = "infix", Stretchy = "true", Scaling = "vertical", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // UpEquilibrium
                new MathOperator("\u21A5") { Form = "infix", Stretchy = "true", Scaling = "vertical", LSpace = "verythinmathspace", RSpace = "verythinmathspace" }, // UpTeeArrow
                new MathOperator("^") { Form = "infix", LSpace = "verythinmathspace", RSpace = "verythinmathspace" },
                new MathOperator("<>") { Form = "infix", LSpace = "verythinmathspace", RSpace = "verythinmathspace" },
                new MathOperator("'") { Form = "postfix", LSpace = "verythinmathspace", RSpace = "0em" },
                // Added an entry for prime, separate from apostrophe
                new MathOperator("\u2032") { Form = "postfix", LSpace = "verythinmathspace", RSpace = "0em" },
                new MathOperator("!") { Form = "postfix", LSpace = "verythinmathspace", RSpace = "0em" },
                new MathOperator("!!") { Form = "postfix", LSpace = "verythinmathspace", RSpace = "0em" },
                new MathOperator("~") { Form = "infix", LSpace = "verythinmathspace", RSpace = "verythinmathspace" },
                new MathOperator("@") { Form = "infix", LSpace = "verythinmathspace", RSpace = "verythinmathspace" },
                new MathOperator("--") { Form = "postfix", LSpace = "verythinmathspace", RSpace = "0em" },
                new MathOperator("--") { Form = "prefix", LSpace = "0em", RSpace = "verythinmathspace" },
                new MathOperator("++") { Form = "postfix", LSpace = "verythinmathspace", RSpace = "0em" },
                new MathOperator("++") { Form = "prefix", LSpace = "0em", RSpace = "verythinmathspace" },
                new MathOperator("\u2061") { Form = "infix", LSpace = "0em", RSpace = "0em" }, // ApplyFunction
                new MathOperator("?") { Form = "infix", LSpace = "verythinmathspace", RSpace = "verythinmathspace" },
                new MathOperator("_") { Form = "infix", LSpace = "verythinmathspace", RSpace = "verythinmathspace" },
                new MathOperator("\u02D8") { Form = "postfix", Accent = "true", LSpace = "0em", RSpace = "0em" }, // Breve
                new MathOperator("\u00B8") { Form = "postfix", Accent = "true", LSpace = "0em", RSpace = "0em" }, // Cedilla
                new MathOperator("`") { Form = "postfix", Accent = "true", LSpace = "0em", RSpace = "0em" }, // DiacriticalGrave
                new MathOperator("\u02D9") { Form = "postfix", Accent = "true", LSpace = "0em", RSpace = "0em" }, // DiacriticalDot
                new MathOperator("\u02DD") { Form = "postfix", Accent = "true", LSpace = "0em", RSpace = "0em" }, // DiacriticalDoubleAcute
                new MathOperator("\u2190") { Form = "postfix", Accent = "true", Stretchy = "true", Scaling = "horizontal", LSpace = "0em", RSpace = "0em" }, // LeftArrow
                new MathOperator("\u2194") { Form = "postfix", Accent = "true", Stretchy = "true", Scaling = "horizontal", LSpace = "0em", RSpace = "0em" }, // LeftRightArrow
                new MathOperator("\u294E") { Form = "postfix", Accent = "true", Stretchy = "true", Scaling = "horizontal", LSpace = "0em", RSpace = "0em" }, // LeftRightVector
                new MathOperator("\u21BC") { Form = "postfix", Accent = "true", Stretchy = "true", Scaling = "horizontal", LSpace = "0em", RSpace = "0em" }, // LeftVector
                new MathOperator("\u00B4") { Form = "postfix", Accent = "true", LSpace = "0em", RSpace = "0em" }, // DiacriticalAcute
                new MathOperator("\u2192") { Form = "postfix", Accent = "true", Stretchy = "true", Scaling = "horizontal", LSpace = "0em", RSpace = "0em" }, // RightArrow
                new MathOperator("\u21C0") { Form = "postfix", Accent = "true", Stretchy = "true", Scaling = "horizontal", LSpace = "0em", RSpace = "0em" }, // RightVector
                new MathOperator("\u02DC") { Form = "postfix", Accent = "true", Stretchy = "true", Scaling = "horizontal", LSpace = "0em", RSpace = "0em" }, // DiacriticalTilde
                new MathOperator("\u00A8") { Form = "postfix", Accent = "true", LSpace = "0em", RSpace = "0em" }, // DoubleDot
                new MathOperator("\u0311") { Form = "postfix", Accent = "true", LSpace = "0em", RSpace = "0em" }, // DownBreve
                new MathOperator("\u02C7") { Form = "postfix", Accent = "true", Stretchy = "true", Scaling = "horizontal", LSpace = "0em", RSpace = "0em" }, // Hacek
                new MathOperator("^") { Form = "postfix", Accent = "true", Stretchy = "true", Scaling = "horizontal", LSpace = "0em", RSpace = "0em" }, // Hat
                new MathOperator("\u00AF") { Form = "postfix", Accent = "true", Stretchy = "true", Scaling = "horizontal", LSpace = "0em", RSpace = "0em" }, // OverBar
                new MathOperator("\uFE37") { Form = "postfix", Accent = "true", Stretchy = "true", Scaling = "horizontal", LSpace = "0em", RSpace = "0em" }, // OverBrace
                new MathOperator("\u23B4") { Form = "postfix", Accent = "true", Stretchy = "true", Scaling = "horizontal", LSpace = "0em", RSpace = "0em" }, // OverBracket
                new MathOperator("\uFE35") { Form = "postfix", Accent = "true", Stretchy = "true", Scaling = "horizontal", LSpace = "0em", RSpace = "0em" }, // OverParenthesis
                new MathOperator("\u20DB") { Form = "postfix", Accent = "true", LSpace = "0em", RSpace = "0em" }, // TripleDot
                new MathOperator("\u0332") { Form = "postfix", Accent = "true", Stretchy = "true", Scaling = "horizontal", LSpace = "0em", RSpace = "0em" }, // UnderBar
                new MathOperator("\uFE38") { Form = "postfix", Accent = "true", Stretchy = "true", Scaling = "horizontal", LSpace = "0em", RSpace = "0em" }, // UnderBrace
                new MathOperator("\u23B5") { Form = "postfix", Accent = "true", Stretchy = "true", Scaling = "horizontal", LSpace = "0em", RSpace = "0em" }, // UnderBracket
                new MathOperator("\uFE36") { Form = "postfix", Accent = "true", Stretchy = "true", Scaling = "horizontal", LSpace = "0em", RSpace = "0em" } // UnderParenthesis
            };
        }

        public MathOperator LookUp(string op, string form = "infix")
        {
            MathOperator mop = OperatorList.Where(x => x.Key() == (op + form)).FirstOrDefault();
            if (mop != null)
                return mop;

            foreach (string dform in m_forms)
            {
                mop = OperatorList.Where(x => x.Key() == (op + dform)).FirstOrDefault();
                if (mop != null)
                    return mop;
            }
            return OperatorList.Where(x => x.Key() == ("infix")).FirstOrDefault();
        }

        public readonly List<MathOperator> OperatorList;
        private readonly List<string> m_forms = new List<string>() { "infix", "postfix", "prefix" };
    }
}