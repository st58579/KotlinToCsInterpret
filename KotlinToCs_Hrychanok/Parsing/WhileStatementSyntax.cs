using KotlinToCs_Hrychanok.Lexing;
using System.Collections.Generic;

namespace KotlinToCs_Hrychanok.Parsing
{
    internal class WhileStatementSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Condition { get; private set; }
        public List<ExpressionSyntax> Body { get; private set; }

        public override SyntaxKind Kind => SyntaxKind.WhileStatement;

        public WhileStatementSyntax(ExpressionSyntax condition, List<ExpressionSyntax> body)
        {
            Condition = condition;
            Body = body;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Condition;
            foreach (var a in Body)
            {
                yield return a;
            }
        }
    }
}