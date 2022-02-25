using KotlinToCs_Hrychanok.Lexing;

namespace KotlinToCs_Hrychanok.Parsing
{
    abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }
    }
}
