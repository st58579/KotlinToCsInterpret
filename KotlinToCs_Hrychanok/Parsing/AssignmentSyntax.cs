using KotlinToCs_Hrychanok.Lexing;
using System.Collections.Generic;

namespace KotlinToCs_Hrychanok.Parsing
{
    internal class AssignmentSyntax : ExpressionSyntax
    {
        public SyntaxToken Name { get; }
        public ExpressionSyntax Expression { get; }

        public AssignmentSyntax(SyntaxToken varName, ExpressionSyntax expression)
        {
            Name = varName;
            Expression = expression;
        }

        public override SyntaxKind Kind => SyntaxKind.AssignmentSyntax;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Name;
            yield return Expression;
        }
    }
}