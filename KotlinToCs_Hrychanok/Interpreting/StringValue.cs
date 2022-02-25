using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KotlinToCs_Hrychanok.Interpreting
{
    class StringValue : TypedValue
    {
        public string Value { get; private set; }

        public StringValue(string value, Context context)
        {
            Value = value;
            Type = Lexing.SyntaxKind.StringToken;
            Context = context;
        }

        public StringValue Add(StringValue right)
        {
            return new StringValue(Value + right.Value, Context);
        }

        public NumberValue Comparison_NotEquals(StringValue right)
        {
            return new NumberValue(Value.Equals(right.Value) ? 0 : 1, Context);
        }

        public NumberValue Comparison_Equals(StringValue right)
        {
            return new NumberValue(Value.Equals(right.Value) ? 1 : 0, Context);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
