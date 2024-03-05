using System.Text;
using System.Text.RegularExpressions;

namespace ASTGen
{
    public static class ASTGen
    {
        private static string _astTemplate = @"namespace Lox
{
    public abstract class {className}
    {
        public abstract T Accept<T>(Visitor<T> visitor);

        public interface Visitor<T>
        {
{visitorMethods}
        }

{types}
    }
}";
        private static string _visitorMethodTemplate =
                        @"            T Visit{type}({type} {className});";
        private static string _typeTemplate = @"
        public class {type} : {className}
        {
{fields}
{constructor}
            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.Visit{type}(this);
            }
        }";
        private static string _typeFieldTemplate =
                @"            public {field};";
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: astgen <output directory>");
                System.Environment.Exit(64);
            }

            var outputDir = args[0];
            for (int i = 1; i < args.Length; i++)
            {
                var astDef = File.ReadAllText(args[i]).Split('\n');
                var baseClassName = Path.GetFileNameWithoutExtension(args[i]);
                WriteAST(outputDir, baseClassName, BuildAST(baseClassName, astDef));
            }
        }

        private static void WriteAST(string outputDir, string baseClassName, string ast)
        {
            var writer = System.IO.File.CreateText(outputDir + "/" + baseClassName + ".cs");
            writer.Write(ast);
            writer.Flush();
            writer.Close();
        }

        private static string BuildAST(string baseClassName, IEnumerable<string> types)
        {
            var typeArray = types.Select(t => t.Split(':')[0].Trim()).ToArray();
            var ast = Regex.Replace(_astTemplate, "{visitorMethods}", BuildVisitorMethods(baseClassName, typeArray));
            ast = Regex.Replace(ast, "{types}", BuildTypes(types));
            ast = Regex.Replace(ast, "{className}", baseClassName);

            return ast;
        }

        private static string BuildVisitorMethods(string baseClassName, string[] types)
        {
            var sBuilder = new StringBuilder();
            foreach (var type in types)
            {
                var trimmedType = type.Trim();
                var result = Regex.Replace(_visitorMethodTemplate, "{type}", type);
                result = Regex.Replace(result, "{className}", baseClassName.ToLower());
                sBuilder.AppendLine(result).AppendLine();
            }
            return sBuilder.ToString();
        }

        private static string BuildTypes(IEnumerable<string> types)
        {
            var builder = new StringBuilder();
            foreach (var typ in types)
            {
                var split = typ.Split(':', ',');
                var result = BuildType(split[0].Trim(), split.Skip(1).ToArray());
                builder.AppendLine(result).AppendLine();
            }

            return builder.ToString();
        }

        private static string BuildType(string className, string[] fields)
        {
            var type = Regex.Replace(_typeTemplate, "{type}", className);
            type = Regex.Replace(type, "{fields}", BuildTypeFields(fields));
            type = Regex.Replace(type, "{constructor}", BuildTypeConstructor());
            return type;
        }

        private static string BuildTypeFields(string[] fields)
        {
            var sBuilder = new StringBuilder();
            foreach (var field in fields)
            {
                sBuilder.
                AppendLine(Regex.Replace(_typeFieldTemplate, "{field}", field.Trim())).
                AppendLine();
            }
            return sBuilder.ToString();
        }

        private static string BuildTypeConstructor()
        {
            return "";
        }
    }
}

