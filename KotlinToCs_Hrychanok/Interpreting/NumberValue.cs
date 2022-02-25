using KotlinToCs_Hrychanok.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KotlinToCs_Hrychanok.Interpreting
{
    class NumberValue : TypedValue
    {
        public double Value { get; private set; }

        public NumberValue(double value)
        {
            Value = value;
            Type = SyntaxKind.NumberToken;
        }

        public NumberValue(double value, Context context) : this(value)
        {
            Context = context;
        }
        public override string ToString()
        {
            return Value.ToString();
        }

        public NumberValue Add(NumberValue n)
        {
            return new NumberValue(Value + n.Value, Context);
        }

        public NumberValue Subtract(NumberValue n)
        {
            return new NumberValue(Value - n.Value, Context);
        }
        public NumberValue Multiply(NumberValue n)
        {
            return new NumberValue(Value * n.Value, Context);
        }
        public NumberValue Divide(NumberValue n)
        {
            return new NumberValue(Value / n.Value, Context);
        }
        public NumberValue Comparison_NotEquals(NumberValue n)
        {
            return new NumberValue(Value == n.Value ? 0 : 1, Context);
        }
        public NumberValue Comparison_Equals(NumberValue n)
        {
            return new NumberValue(Value == n.Value ? 1 : 0, Context);
        }
        public NumberValue Comparison_Less(NumberValue n)
        {
            return new NumberValue(Value < n.Value ? 1 : 0, Context);
        }
        public NumberValue Comparison_LessEquals(NumberValue n)
        {
            return new NumberValue(Value <= n.Value ? 1 : 0, Context);
        }
        public NumberValue Comparison_Greater(NumberValue n)
        {
            return new NumberValue(Value > n.Value ? 1 : 0, Context);
        }
        public NumberValue Comparison_GreaterEquals(NumberValue n)
        {
            return new NumberValue(Value >= n.Value ? 1 : 0, Context);
        }
        public NumberValue Negate()
        {
            return new NumberValue(Value == 1 ? 0 : 1, Context);
        }
        public bool IsTrue()
        {
            return Value != 0;
        }


    }
}
