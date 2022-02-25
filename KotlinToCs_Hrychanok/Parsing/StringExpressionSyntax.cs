using KotlinToCs_Hrychanok.Lexing;
using System.Collections.Generic;

namespace KotlinToCs_Hrychanok.Parsing
{
    internal class StringExpressionSyntax : ExpressionSyntax
    {
        public SyntaxToken StringToken { get; set; }

        public override SyntaxKind Kind => SyntaxKind.StringExpression;

        public StringExpressionSyntax(SyntaxToken stringToken)
        {
            StringToken = stringToken;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return StringToken;
        }
    }
}