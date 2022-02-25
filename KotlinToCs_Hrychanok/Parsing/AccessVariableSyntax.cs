using System.Collections.Generic;
using KotlinToCs_Hrychanok.Lexing;

namespace KotlinToCs_Hrychanok.Parsing
{
    internal class AccessVariableSyntax : ExpressionSyntax
    {
        public SyntaxToken Name { get; }

        public AccessVariableSyntax(SyntaxToken name)
        {
            Name = name;
        }

        public override SyntaxKind Kind => SyntaxKind.AccessVariableSyntax;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Name;
        }
    }
}