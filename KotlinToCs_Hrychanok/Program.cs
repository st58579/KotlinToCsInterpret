using KotlinToCs_Hrychanok.Interpreting;
using KotlinToCs_Hrychanok.Parsing;
using System;
using System.IO;
using System.Linq;

namespace KotlinToCs_Hrychanok
{
    class Program
    {
        private static bool showTree = false;

        static void Main(string[] args)
        {
            while (true)
            {
                SyntaxTree syntaxTree;
                SymbolTable globalSymbolTable = new SymbolTable();
                Console.Write("> ");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    break;
                else if (line == "cl")
                {
                    Console.Clear();
                    continue;
                }
                else if (line == "st")
                {
                    Console.WriteLine("Showing parse trees.");
                    showTree = false;
                    continue;
                }
                else if (line == "e1")
                {
                    line = OpenExample(1);
                    syntaxTree = SyntaxTree.Parse(line);
                }
                else if (line == "e2")
                {
                    line = OpenExample(2);
                    syntaxTree = SyntaxTree.Parse(line);
                }
                else if (line == "e3")
                {
                    line = OpenExample(3);
                    syntaxTree = SyntaxTree.Parse(line);
                }
                else
                {
                    syntaxTree = SyntaxTree.Parse(line);
                }

                // Parse program
                if (syntaxTree.Reports.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    for (int i = 0; i < syntaxTree.Reports.Count; i++)
                    {
                        Console.WriteLine(syntaxTree.Reports[i]);
                    }
                    Console.ResetColor();
                    continue;
                }

                var treeRoot = syntaxTree.Root;
                if (showTree) Print(treeRoot);

                //Evaluate program
                var evaluator = new Evaluator();
                var context = new Context("<Global>")
                {
                    SymbolTable = globalSymbolTable
                };
                var evaluationResult = evaluator.Evaluate(treeRoot, context);
                if (evaluationResult.Error == null && evaluationResult.Value != null)
                {
                    Console.WriteLine(evaluationResult.Value.ToString());
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(evaluationResult.Error);
                    Console.ResetColor();
                }
            }
            Console.ReadKey();
        }
        
        private static string OpenExample(int num)
        {
            string result = "";
            try
            {
                using (StreamReader writer = new StreamReader(File.Open($"../../Examples/Example{num}.txt", FileMode.Open)))
                {
                    string line;
                    while ((line = writer.ReadLine()) != null)
                    {
                        result += line;
                        result += "\n";
                    }
                    Console.WriteLine(result);
                }
            }
            catch (Exception)
            {
                throw new Exception("Source file is not found."); 
            }
            return result;
        }

        private static void Print(SyntaxNode node)
        {
            if (node is ExpressionSyntax)
            {
                foreach (var child in ((ExpressionSyntax)node).GetChildren())
                {
                    if (child != null)
                    {
                        Console.WriteLine(child.Kind);
                        Print(child);
                    }
                }
            }
        }
    }
}
