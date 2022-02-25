using KotlinToCs_Hrychanok.Lexing;
using System.Collections.Generic;

namespace KotlinToCs_Hrychanok.Parsing
{
    internal class FunctionCallSyntax : ExpressionSyntax
    {
        public SyntaxToken Identifier { get; private set; }
        public List<ExpressionSyntax> Parameters { get; private set; }

        public FunctionCallSyntax(SyntaxToken identifier, List<ExpressionSyntax> parameters)
        {
            Identifier = identifier;
            Parameters = parameters;
        }

        public override SyntaxKind Kind => SyntaxKind.FunctionCallSyntax;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Identifier;
        }
    }
}