using KotlinToCs_Hrychanok.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KotlinToCs_Hrychanok.Lexing
{
    class SyntaxToken : SyntaxNode
    {
        public SyntaxToken(SyntaxKind type, int position, string text, object value)
        {
            Kind = type;
            Position = position;
            Text = text;
            Value = value;
        }

        public override SyntaxKind Kind { get; } 

        public int Position { get; }
        public string Text { get; }
        public object Value { get; }

    }

}
