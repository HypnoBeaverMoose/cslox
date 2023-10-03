using System.Formats.Tar;

namespace ASTGen
{
    public static class ASTGen
    {
        public static void Main(string[] args)
        {
            if(args.Length != 1)
            {
                Console.WriteLine("Usage: astgen <output directory>");
                System.Environment.Exit(64);
            }
            var outputDir = args[0];
            DefineAST(outputDir, "Expr", 
            new string[] 
            {
                "Binary : Expr Left, Token Operator, Expr Right",
                "Grouping : Expr Expression",
                "Literal : Object Value",
                "Unary : Token Operator, Expr Right",
            });
        }

        public static void DefineAST(string outputDir, string baseClassName, IEnumerable<string> types)
        {
            System.IO.TextWriter tw = System.IO.File.CreateText(outputDir + "/" + baseClassName + ".cs");
            tw.WriteLine("namespace Lox");
            tw.WriteLine("{");
            tw.WriteLine($"    public abstract class {baseClassName}");
            tw.WriteLine("    {");
            foreach (var type in types)
            {
                var split = type.Split(':', ',');
                DefineType(tw, baseClassName, split[0].Trim(), split.Skip(1).ToArray());
            }
            tw.WriteLine("    }");
            tw.WriteLine("}");
            tw.Flush();
            tw.Close();
        }

        public static void DefineType(TextWriter writer, string baseClassName, string className, string[] fields)
        {
            writer.WriteLine($"        public class {className} : {baseClassName}");
            writer.WriteLine("        {");
            foreach (var field in fields)
            {
                writer.WriteLine($"            public {field.Trim()};");
            }
            writer.WriteLine("        }");
            writer.WriteLine();
        }
    }
}

