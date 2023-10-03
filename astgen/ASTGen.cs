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
        }
    }
}

