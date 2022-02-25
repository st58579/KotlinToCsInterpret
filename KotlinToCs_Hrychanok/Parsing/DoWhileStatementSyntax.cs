using KotlinToCs_Hrychanok.Lexing;
using System.Collections.Generic;

namespace KotlinToCs_Hrychanok.Parsing
{
    internal class DoWhileStatementSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Condition { get; private set; }
        public List<ExpressionSyntax> Body { get; private set; }

        public override SyntaxKind Kind => SyntaxKind.DoWhileStatement;

        public DoWhileStatementSyntax(ExpressionSyntax condition, List<ExpressionSyntax> body)
        {
            Condition = condition;
            Body = body;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Condition;
            foreach (var statement in Body)
            {
                yield return statement;
            }
        }
    }
}