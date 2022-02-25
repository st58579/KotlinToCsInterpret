using KotlinToCs_Hrychanok.Lexing;
using System.Collections.Generic;

namespace KotlinToCs_Hrychanok.Parsing
{
    internal class IfStatementSyntax : ExpressionSyntax
    {
        
        public Dictionary<List<ExpressionSyntax>, ExpressionSyntax> Cases { get; private set; }
        public List<ExpressionSyntax> ElseCase { get; private set; }

        public IfStatementSyntax(Dictionary<List<ExpressionSyntax>, ExpressionSyntax> cases, List<ExpressionSyntax> elseStatements)
        {
            Cases = cases;
            ElseCase = elseStatements;
        }

        public override SyntaxKind Kind => SyntaxKind.IfStatement;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            foreach (var c in Cases)
            {
                foreach (var z in c.Key)
                {
                    yield return z;
                }
                yield return c.Value;
            }
            foreach (var s in ElseCase)
            {
                yield return s;
            }

        }
    }
}