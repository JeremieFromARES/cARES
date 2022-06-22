using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cARES
{
    /// <summary>
    /// Collection of helper methods
    /// </summary>
    public static class Lib
    {
        public static void Print(string text)
        {
            Console.WriteLine(text);
        }
        public static void Raise(long line_number, string error_desc)
        {
            Console.WriteLine("/!\\PARSING ERROR/!\\ : Syntax error at line " + line_number + ", " + error_desc);
        }
    }
}
