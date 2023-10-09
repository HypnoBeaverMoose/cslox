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
                "Ternary : Expr Condition, Token Operator, Expr Left, Expr Right",
                "Binary : Expr Left, Token Operator, Expr Right",
                "Grouping : Expr Expression",
                "Literal : object? Value",
                "Unary : Token Operator, Expr Right",
            });

            DefineAST(outputDir, "Stmt", 
            new string[] 
            {
                "Expression : Expr Expr",
                "Print : Expr Expr",
            });
        }

        public static void DefineAST(string outputDir, string baseClassName, IEnumerable<string> types)
        {
            System.IO.TextWriter tw = System.IO.File.CreateText(outputDir + "/" + baseClassName + ".cs");
            tw.WriteLine("namespace Lox");
            tw.WriteLine("{");
            tw.WriteLine($"    public abstract class {baseClassName}");
            tw.WriteLine("    {");

            tw.WriteLine();
            tw.WriteLine("        public abstract T Accept<T>(Visitor<T> visitor);");
            tw.WriteLine();

            DefineVisitor(tw, baseClassName, types.Select(t => t.Split(':')[0].Trim()).ToArray());

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

        public static void DefineVisitor(TextWriter writer, string baseClassName, string[] types)
        {
            writer.WriteLine("        public interface Visitor<T>");
            writer.WriteLine("        {");

            foreach (var type in types)
            {
                var trimmedType = type.Trim();
                writer.WriteLine($"            T Visit{trimmedType}({trimmedType} {baseClassName.ToLower()});");
                writer.WriteLine();
            }

            writer.WriteLine("        }");
            writer.WriteLine();
        }

        public static void DefineType(TextWriter writer, string baseClassName, string className, string[] fields)
        {
            writer.WriteLine($"        public class {className} : {baseClassName}");
            writer.WriteLine("        {");
            foreach (var field in fields)
            {
                writer.WriteLine($"            public {field.Trim()};");
                writer.WriteLine();
            }

            writer.WriteLine("            public override T Accept<T>(Visitor<T> visitor)");
            writer.WriteLine("            {");
            writer.WriteLine($"                return visitor.Visit{className}(this);");
            writer.WriteLine("            }");

            writer.WriteLine("        }");
            writer.WriteLine();
        }
    }
}

