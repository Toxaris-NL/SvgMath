using System;
using System.Collections.Generic;
using System.Linq;

namespace SvgMath
{
    internal class MathContext
    {
        public static void DefaultContext(MathNode node)
        {
            if (node.Parent != null)
            {
                node.MathSize = node.Parent.MathSize;
                node.FontSize = node.Parent.FontSize;
                node.MetricList = node.Parent.MetricList;
                node.ScriptLevel = node.Parent.ScriptLevel;
                node.TightSpaces = node.Parent.TightSpaces;
                node.DisplayStyle = node.Parent.DisplayStyle;
                node.Color = node.Parent.Color;
                node.FontFamilies = node.Parent.FontFamilies;
                node.FontWeight = node.Parent.FontWeight;
                node.FontStyle = node.Parent.FontStyle;
                node.NodeDefaults = node.Parent.NodeDefaults;
                node.Parent.MakeChildContext(node);
            }
            else
            {
                node.MathSize = node.ParseLength(node.NodeDefaults["mathsize"]);
                node.FontSize = node.MathSize;
                node.MetricList = null;
                node.ScriptLevel = int.Parse(node.NodeDefaults["scriptlevel"]);
                node.TightSpaces = false;
                node.DisplayStyle = (node.NodeDefaults["displaystyle"] == "true");
                node.Color = node.NodeDefaults["mathcolor"];

                if (!node.Config.Variants.ContainsKey(node.NodeDefaults["mathvariant"]))
                    throw new InvalidOperationException("Default mathvariant not defined in configuration file: configuration is unusable");

                MathVariant defaultVariant = node.Config.Variants[node.NodeDefaults["mathvariant"]];
                node.FontWeight = defaultVariant.Weight;
                node.FontStyle = defaultVariant.Style;
                node.FontFamilies = defaultVariant.Families;
            }

            node.ProcessFontAttributes();
            node.Width = 0;
            node.Height = 0;
            node.Depth = 0;
            node.Ascender = 0;
            node.Descender = 0;
            node.LeftSpace = 0;
            node.RightSpace = 0;
            node.AlignToAxis = false;
            node.Base = node;
            node.Core = node;
            node.Stretchy = false;
            node.Accent = false;
            node.MoveLimits = false;
            node.TextShift = 0;
            node.TextStretch = 1;
            node.LeftBearing = 0;
            node.RightBearing = 0;
            node.IsSpace = false;
            //Reset metrics list to None(so far, we used metrics from the parent)
            node.MetricList = null;
            node.NominalMetric = null;
        }

        public static void DefaultChildContext(MathNode node, MathNode child)
        {
            // stub
            // Default child context processing for a MathML tree node
            // Pass
        }

        public static void CMath(MathNode node)
        {
            DefaultContext(node);
            // Display style: set differently on 'math'
            string attr = null;
            if (node.Attributes.ContainsKey("display"))
            {
                attr = node.Attributes["display"];
                node.DisplayStyle = (node.Attributes["display"] == "block");
            }
            else
            {
                if (node.Attributes.ContainsKey("mode"))
                {
                    attr = node.Attributes["mode"];
                    node.DisplayStyle = (attr == "display");
                }
                else
                {
                    node.DisplayStyle = false;
                }
            }
        }

        public static void CMStyle(MathNode node)
        {
            DefaultContext(node);
            // Avoid redefinition of mathsize - it is inherited anyway.
            // This serves to preserve values of 'big', 'small', and 'normal'
            // throughout the MathML instance.
            if (node.Attributes != null && node.Attributes.ContainsKey("mathsize"))
            {
                node.Attributes.Remove("mathsize");
            }

            if (node.Attributes != null)
            {
                Dictionary<string, string> newDefaults = new Dictionary<string, string>(node.NodeDefaults);
                newDefaults.AddRange(node.Attributes);
                node.NodeDefaults = newDefaults;
            }
        }

        public static void CMTable(MathNode node)
        {
            DefaultContext(node);
            // Display style: no inheritance, default is 'false' unless redefined in 'mstyle'
            node.DisplayStyle = (node.GetProperty("displaystyle") == "true");
        }

        public static void CMi(MathNode node)
        {
            // If the identifier is a single character, make it italic by default.
            // Don't forget surrogate pairs here!
            if (node.Text.Length == 1 || (node.Text.Length == 2 && char.IsHighSurrogate(node.Text[0]) && char.IsHighSurrogate(node.Text[1])))
            {
                node.Attributes["fontstyle"] = "italic";
            }
            DefaultContext(node);
        }

        public static void CMo(MathNode node)
        {
            // Apply special formatting to operators
            OperatorStyle extraStyle = node.Config.OperatorStyles.FirstOrDefault(x => x.OpName == node.Text);//.Styling;
            if (extraStyle != null)
            {
                node.Attributes.AddRange(extraStyle.Styling); //ToDo: verify
            }

            // Consult the operator dictionary, and set the appropriate defaults
            string form = "infix";
            if (node.Parent == null)
            {
                // pass
            }
            else if (new List<string>() { "mrow", "mstyle", "msqrt", "merror", "mpadded", "mphantom", "menclose", "mtd", "math" }.Any(x => x == node.Parent.ElementName))
            {
                //ToDo: verify
                List<MathNode> prevSiblings = node.Parent.Children.TakeWhile((value, i) => i != node.NodeIndex).Where(IsNonSpaceNode).ToList();
                List<MathNode> nextSiblings = node.Parent.Children.Skip(node.NodeIndex + 1).Where(IsNonSpaceNode).ToList();

                if (prevSiblings.Count == 0 && nextSiblings.Count > 0)
                    form = "prefix";
                if (nextSiblings.Count == 0 && prevSiblings.Count > 0)
                    form = "postfix";
            }

            if (node.Attributes.ContainsKey("form"))
                form = node.Attributes["form"];

            node.OpDefaults = node.Config.MathOperators.LookUp(node.Text, form);
            DefaultContext(node);
            string stretchyattr = node.GetProperty("stretchy", node.OpDefaults.Dict()["stretchy"]);
            node.Stretchy = (stretchyattr == "true");
            string symmetricattr = node.GetProperty("symmetric", node.OpDefaults.Dict()["symmetric"]);
            node.Symmetric = (symmetricattr == "true");
            node.Scaling = node.OpDefaults.Dict()["scaling"];
            if (!node.TightSpaces)
            {
                string lspaceattr = node.GetProperty("lspace", node.OpDefaults.Dict()["lspace"]);
                node.LeftSpace = node.ParseSpace(lspaceattr);
                string rspaceattr = node.GetProperty("rspace", node.OpDefaults.Dict()["rspace"]);
                node.RightSpace = node.ParseSpace(rspaceattr);
            }

            if (node.DisplayStyle)
            {
                string value = node.OpDefaults.Dict()["largeop"];
                if (node.GetProperty("largeop", value) == "true")
                    node.FontSize *= 1.41;
            }
            else
            {
                string value = node.OpDefaults.Dict()["movablelimits"];
                node.MoveLimits = (node.GetProperty("movablelimits", value) == "true");
            }
        }

        public static void ChildCMFrac(MathNode node, MathNode child)
        {
            if (node.DisplayStyle)
            {
                child.DisplayStyle = false;
            }
            else
            {
                child.ScriptLevel += 1;
            }
        }

        public static void ChildCMRoot(MathNode node, MathNode child)
        {
            if (child.NodeIndex == 1)
            {
                child.DisplayStyle = false;
                child.ScriptLevel += 2;
                child.TightSpaces = true;
            }
        }

        public static void ChildCMSub(MathNode node, MathNode child)
        {
            MakeScriptContext(child);
        }

        public static void ChildCMSup(MathNode node, MathNode child)
        {
            MakeScriptContext(child);
        }

        public static void ChildCMSubSup(MathNode node, MathNode child)
        {
            MakeScriptContext(child);
        }

        public static void ChildCMMultiScripts(MathNode node, MathNode child)
        {
            MakeScriptContext(child);
        }

        public static void ChildCMUnder(MathNode node, MathNode child)
        {
            if (child.NodeIndex == 1)
                MakeLimitContext(node, child, "accentunder");
        }

        public static void ChildCMOver(MathNode node, MathNode child)
        {
            if (child.NodeIndex == 1)
                MakeLimitContext(node, child, "accent");
        }

        public static void ChildCMUnderOver(MathNode node, MathNode child)
        {
            if (child.NodeIndex == 1)
                MakeLimitContext(node, child, "accentunder");
            if (child.NodeIndex == 2)
                MakeLimitContext(node, child, "accent");
        }

        public static void MakeScriptContext(MathNode child)
        {
            if (child.NodeIndex > 0)
            {
                child.DisplayStyle = false;
                child.TightSpaces = true;
                child.ScriptLevel += 1;
            }
        }

        public static void MakeLimitContext(MathNode node, MathNode child, string accentProperty)
        {
            child.DisplayStyle = false;
            child.TightSpaces = true;
            string accentValue = node.GetProperty(accentProperty);
            if (accentValue == null)
                accentValue = GetAccentValue(child);

            child.Accent = (accentValue == "true");
            if (!child.Accent)
                child.ScriptLevel += 1;
        }

        public static string GetAccentValue(MathNode node)
        {
            if (node.ElementName == "mo")
            {
                if (node.OpDefaults != null)
                    return node.OpDefaults.Dict()["accent"];
                else
                    return "false";
            }
            else if (MathDefaults.Embellishments.Any(x => x == node.ElementName) && node.Children.Count > 0)
            {
                return GetAccentValue(node.Children[0]);
            }
            else
            {
                return "false";
            }
        }

        public static bool IsNonSpaceNode(MathNode node)
        {
            return node.ElementName != "mspace";
        }
    }
}