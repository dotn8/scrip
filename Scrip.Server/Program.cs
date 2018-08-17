using GraphQL;
using GraphQL.Types;
using System;

namespace Scrip.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var schema = Schema.For(@"
  type Query {
    hello: String
  }
");

            var root = new { Hello = "Hello World!" };
            var json = schema.Execute(_ =>
            {
                _.Query = "{ hello }";
                _.Root = root;
            });

            Console.WriteLine(json);
        }
    }
}
