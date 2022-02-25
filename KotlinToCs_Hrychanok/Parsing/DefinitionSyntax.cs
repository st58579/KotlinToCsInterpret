using System.Collections.Generic;
using KotlinToCs_Hrychanok.Lexing;

namespace KotlinToCs_Hrychanok.Parsing
{
    internal class DefinitionSyntax : ExpressionSyntax
    {
        public SyntaxToken VarName { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxKind Type { get; } = SyntaxKind.None;

        public DefinitionSyntax(SyntaxToken varName, ExpressionSyntax expression)
        {
            VarName = varName;
            Expression = expression;
        }

        public DefinitionSyntax(SyntaxToken varName, ExpressionSyntax expression, SyntaxKind type) : this(varName, expression)
        {
            Type = type;
        }

        public override SyntaxKind Kind => SyntaxKind.DefinitionSyntax;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return VarName;
            yield return Expression;
        }
    }
}