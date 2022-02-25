using KotlinToCs_Hrychanok.Lexing;
using System.Collections.Generic;

namespace KotlinToCs_Hrychanok.Parsing
{
    abstract class ExpressionSyntax : SyntaxNode
    {
        public abstract IEnumerable<SyntaxNode> GetChildren();
    }
}
