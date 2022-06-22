using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static cARES.ARES_std;
using static cARES.Lib;
using static cARES.Parser;

namespace cARES
{
    public static class Program
    {
        public static List<string> source_lines = new();
        public static List<Line> parsed_lines_pass1 = new();
        public static List<Line> parsed_lines_pass2 = new();
        public static List<string> cpp_lines = new();

        static int Main(string[] args)
        {
            Print("            ___   ____  ____ ");
            Print("   -|- //| || \\\\ ||___ //__  ");
            Print("      //|| ||_// ||       \\\\ ");
            Print("     // || || \\\\ ||___ \\\\_// ");

            Print("");
            Print("Source file (.ares) absolute path :");

            string? source_path = Console.ReadLine();
            if (source_path == null) { return -1; }
            source_path = source_path.Replace("\"", "");
            source_lines = File.ReadLines(source_path).ToList();

            Print("");
            Print("# Parsing ...");
            Parser.FirstPass();
            Parser.SecondPass();
            //Parser.ThirdPass();
            Print("# Finished parsing");

            return 1;
        }
    }
}