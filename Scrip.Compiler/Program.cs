using System;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Scrip.Compiler
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ScripToHtml.ConvertToHtml(args[0]);

            Console.Write("\nPress any key to continue.");
            Console.ReadKey();
        }
    }
}