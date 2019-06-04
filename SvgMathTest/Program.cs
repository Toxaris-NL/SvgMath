using SvgMath;
using System;
using System.IO;

namespace SvgMathTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Running conversions.");
            Process();
            Console.WriteLine("Conversions complete.");
            Console.ReadKey(true);
        }

        private static void Process()
        {
            int i = 1;
            foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.mml"))
            {
                MakeSvg(file);
                Console.WriteLine("Converted testfile {0}", i);
                i++;
            }
        }

        private static void MakeSvg(string file)
        {
            Mml m = new Mml(file);
            m.MakeSvg().Save(Path.ChangeExtension(file, ".svg"));
        }
    }
}