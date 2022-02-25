using KotlinToCs_Hrychanok.Lexing;

namespace KotlinToCs_Hrychanok.Interpreting
{
    internal abstract class TypedValue
    {
        public Context Context { get; set; } = null;
        public SyntaxKind Type { get; set; }
    }
}