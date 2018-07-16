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
            Console.WriteLine("(1/3) Lexing");
            var lexer = new ScripLexer(new AntlrFileStream(args[0]));

            var tokens = new CommonTokenStream(lexer);

            Console.WriteLine("(2/3) Parsing");
            var parser = new ScripParser(tokens);
            var tree = parser.paragraphs();

            Console.WriteLine("(3/3) Walking:");
            var walker = new ParseTreeWalker();
            using (var listener = new ScripToHtml(Path.ChangeExtension(args[0], ".html")))
            {
                walker.Walk(listener, tree);
            }

            Console.Write("\nPress any key to continue.");
            Console.ReadKey();
        }
    }
}