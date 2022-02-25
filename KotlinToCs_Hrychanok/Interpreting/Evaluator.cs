using KotlinToCs_Hrychanok.Lexing;
using KotlinToCs_Hrychanok.Parsing;
using System;
using System.Collections.Generic;

namespace KotlinToCs_Hrychanok.Interpreting
{
    class Evaluator
    {
        public IEnumerable<string> Diagnostics => diagnostics;
        private List<string> diagnostics = new List<string>();
        private Random random = new Random();

        public RTResult Evaluate(ExpressionSyntax node, Context context)
        {
            switch (node.Kind)
            {
                case SyntaxKind.Block:
                    return EvaluateBlock((BlockSyntax)node, context);
                case SyntaxKind.DefinitionSyntax:
                    return EvaluateDefinitionSyntax((DefinitionSyntax)node, context);
                case SyntaxKind.AssignmentSyntax:
                    return EvaluateAssignmentSyntax((AssignmentSyntax)node, context);
                case SyntaxKind.AccessVariableSyntax:
                    return EvaluateAccessVariableSyntax((AccessVariableSyntax)node, context);
                case SyntaxKind.FunctionCallSyntax:
                    return EvaluateFunctionCall((FunctionCallSyntax)node, context);
                case SyntaxKind.BinaryExpression:
                    return EvaluateBinaryExpression((BinaryExpressionSyntax)node, context);
                case SyntaxKind.UnaryExpression:
                    return EvaluateUnaryExpression((UnaryExpressionSyntax)node, context);
                case SyntaxKind.ParenthesizedExpression:
                    return Evaluate(((ParenthesizedExpressionSyntax)node).Expression, context);
                case SyntaxKind.IfStatement:
                    return EvaluateIfStatement((IfStatementSyntax)node, new Context("<IfStatementContext>", context));
                case SyntaxKind.WhileStatement:
                    return EvaluateWhileStatement((WhileStatementSyntax)node, new Context("<WhileStatementContext>", context));
                case SyntaxKind.DoWhileStatement:
                    return EvaluateDoWhileStatement((DoWhileStatementSyntax)node, new Context("<DoWhileStatementContext>", context));
                case SyntaxKind.NumberExpression:
                    return EvaluateNumber((LiteralExpressionSyntax)node, context);
                case SyntaxKind.StringExpression:
                    return EvaluateString((StringExpressionSyntax)node, context);
            }
            diagnostics.Add($"Unexpected syntax node of kind <{node.Kind}>");
            return new RTResult(null);
        }

        private RTResult EvaluateBlock(BlockSyntax node, Context context)
        {
            List<string> errors = new List<string>();
            foreach(var definition in node.DefinitionsList)
            {
                var res = Evaluate(definition, context);
                if (res.Error != "") {
                    errors.Add(res.Error);
                }
            }
            foreach (var statement in node.StatementsList)
            {
                var res = Evaluate(statement, context);
                if (res.Error != "")
                {
                    errors.Add(res.Error);
                }
            }
            if (errors.Count > 0)
            {
                string res = "";
                foreach (var err in errors)
                {
                    if (err != "")
                    {
                        res += err;
                        res += "\n";
                    }
                }
                return new RTResult(null, res);

            }
            return new RTResult(null);
        }

        private RTResult EvaluateFunctionCall(FunctionCallSyntax node, Context context)
        {
            var args = new List<RTResult>();
            var nodeToCall = node.Identifier.Text;
            foreach (var arg in node.Parameters)
            {
                var temp = Evaluate(arg, context);
                if (temp.Error != "") return new RTResult(null, temp.Error);
                args.Add(temp);
            }
            
            return Execute(nodeToCall, args, context);
        }
        public RTResult Execute(string name, List<RTResult> args, Context context)
        {
            switch (name)
            {
                case "print":
                    foreach (var arg in args)
                    {
                        if (arg.Value == null) return new RTResult(null, $"Value of argument <{arg.Type}> was <{arg.Value}>");
                        if (arg.Error != "") return new RTResult(null, arg.Error);
                        Console.Write(arg.Value.ToString());
                    }
                    break;
                case "println":
                    foreach (var arg in args)
                    {
                        if (arg.Value == null) return new RTResult(null, $"Value of argument <{arg.Type}> was <{arg.Value}>");
                        if (arg.Error != "") return new RTResult(null, arg.Error);
                        Console.WriteLine(arg.Value.ToString());
                    }
                    if (args.Count == 0) Console.WriteLine();
                    break;
                case "readLine":
                    if (args.Count != 0) return new RTResult(null, $"readLine function takes 0 parameters, but you have {args.Count}");
                    return new RTResult(new StringValue(Console.ReadLine(), context), SyntaxKind.StringToken);
                case "readNum":
                    if (args.Count != 0) return new RTResult(null, $"readNum function takes 0 parameters, but you have {args.Count}");
                    var line = Console.ReadLine();
                    if (!double.TryParse(line, out var num)) return new RTResult(null, $"<{line}> can not be represented by Double.");
                    return new RTResult(new NumberValue(num, context), SyntaxKind.NumberToken);
                case "pow":
                    if (args.Count != 2 || args[0].Type != SyntaxKind.NumberToken || args[1].Type != SyntaxKind.NumberToken) return new RTResult(null, $"Wrong parameters in pow() function.");
                    return new RTResult(new NumberValue(Math.Pow(((NumberValue)args[0].Value).Value, ((NumberValue)args[1].Value).Value), context), SyntaxKind.NumberToken);
                case "sqrt":
                    if (args.Count != 1 || args[0].Type != SyntaxKind.NumberToken) return new RTResult(null, $"Wrong parameters in sqrt() function.");
                    return new RTResult(new NumberValue(Math.Sqrt(((NumberValue)args[0].Value).Value), context), SyntaxKind.NumberToken);
                case "abs":
                    if (args.Count != 1 || args[0].Type != SyntaxKind.NumberToken) return new RTResult(null, $"Wrong parameters in abs() function.");
                    return new RTResult(new NumberValue(Math.Abs(((NumberValue)args[0].Value).Value), context), SyntaxKind.NumberToken);
                case "rand":
                    if (args.Count != 1 || args[0].Type != SyntaxKind.NumberToken) return new RTResult(null, $"Wrong parameters in rand() function.");
                    return new RTResult(new NumberValue(random.Next(0, (int)((NumberValue)args[0].Value).Value), context), SyntaxKind.NumberToken);
                default:
                    return new RTResult(null, $"Unexpected function name <{name}>");
            }
            return new RTResult(null);
        }

        private RTResult EvaluateDefinitionSyntax(DefinitionSyntax d, Context context)
        {
            var varName = d.VarName.Text;
            var evaluationValue = Evaluate(d.Expression, context);
            if (evaluationValue.Type == SyntaxKind.StringToken)
            {
                var stringValue = ((StringValue)evaluationValue.Value).Value;
                if (d.Type == SyntaxKind.None)
                {
                    context.SymbolTable.SetDynamic(varName, stringValue);
                    return new RTResult(new StringValue(stringValue, context), SyntaxKind.StringToken);
                } else if(d.Type == SyntaxKind.StringKeywordToken)
                {
                    context.SymbolTable.SetStringVar(varName, stringValue);
                    return new RTResult(new StringValue(stringValue, context), SyntaxKind.StringToken);
                }
                return new RTResult(null, $"Type missmatch : String and {d.Type}");
            }
            else if (evaluationValue.Type == SyntaxKind.NumberToken)
            {
                var numberValue = ((NumberValue)evaluationValue.Value).Value;
                if (d.Type == SyntaxKind.None)
                {
                    context.SymbolTable.SetDynamic(varName, numberValue);
                    return new RTResult(new NumberValue(numberValue, context), SyntaxKind.NumberToken);
                } else if (d.Type == SyntaxKind.DoubleKeywordToken)
                {
                    context.SymbolTable.SetDoubleVar(varName, numberValue);
                    return new RTResult(new NumberValue(numberValue, context), SyntaxKind.NumberToken);
                }
                return new RTResult(null, $"Type missmatch : Number and {d.Type}");
            }
            return new RTResult(null, evaluationValue.Error);
        }
        private RTResult EvaluateAssignmentSyntax(AssignmentSyntax node, Context context)
        {
            var varName = node.Name.Text;
            var evaluationValue = Evaluate(node.Expression, context);
            if (evaluationValue.Error == "")
            {
                if (evaluationValue.Type == SyntaxKind.StringToken)
                {
                    context.SymbolTable.SetString(varName, ((StringValue)evaluationValue.Value).Value, out string error);
                    if (error == "")
                    {
                        return new RTResult(evaluationValue.Value);
                    }
                    return new RTResult(null, error);
                }
                else if (evaluationValue.Type == SyntaxKind.NumberToken)
                {
                    context.SymbolTable.SetDouble(varName, ((NumberValue)evaluationValue.Value).Value, out string error);
                    if (error == "")
                    {
                        return new RTResult(evaluationValue.Value);
                    }
                    return new RTResult(null, error);
                }
            }
            return evaluationValue;
        }
        private RTResult EvaluateAccessVariableSyntax(AccessVariableSyntax a, Context context)
        {
            var varName = a.Name.Text;
            var value = context.SymbolTable.Get(varName);
            if (value == null)
            {
                return new RTResult(null, $"Variable with name <{a.Name.Text}> does not exist in context {context.Name}.");
            }
            if (value is double)
            {
                return new RTResult(new NumberValue((double)value, context), SyntaxKind.NumberToken);
            }
            else if (value is string)
            {
                return new RTResult(new StringValue((string)value, context), SyntaxKind.StringToken);
            }
            else return new RTResult(null, $"Unexpected type <{value.GetType()}> of variable <{varName}> with value <{value}>");
        }

        private RTResult EvaluateIfStatement(IfStatementSyntax ifStatement, Context context)
        {
            foreach (var kvp in ifStatement.Cases)
            {
                var evaluationResult = Evaluate(kvp.Value, context);
                if (evaluationResult.Type == SyntaxKind.NumberToken)
                {
                    var conditionValue = (NumberValue)evaluationResult.Value;
                    if (conditionValue.IsTrue())
                    {
                        var error = "";
                        foreach (var statement in kvp.Key)
                        {
                            var statementValue = Evaluate(statement, context);
                            if (statementValue.Error != "")
                            {
                                error += statementValue.Error;
                                error += "\n";
                            }
                        }
                        return new RTResult(null, error);
                    }
                } else return new RTResult(null, evaluationResult.Error);
            }
            if (ifStatement.ElseCase != null)
            {
                var error = "";
                foreach (var elseStatement in ifStatement.ElseCase)
                {
                    var elseStatementValue = Evaluate(elseStatement, context);
                    if (elseStatementValue.Error != "")
                    {
                        error += elseStatementValue.Error;
                        error += "\n";
                    }
                }
                return new RTResult(null, error);
            }
            return new RTResult(null);
        }
        private RTResult EvaluateWhileStatement(WhileStatementSyntax w, Context context)
        {
            RTResult statement = null;
            while (true)
            {
                var evaluationResult = Evaluate(w.Condition, context);
                if (evaluationResult.Type == SyntaxKind.NumberToken)
                {
                    var condition = (NumberValue)evaluationResult.Value;
                    if (!condition.IsTrue()) break;
                    foreach(var stmt in w.Body)
                    {
                        Evaluate(stmt, context);
                    }
                } else
                {
                    return new RTResult(null, $"Unexpected type <{evaluationResult.Type}> with value <{evaluationResult.Value}> while evaluating WhileStatement ");
                }
            }
            return new RTResult(statement?.Value);
        }
        private RTResult EvaluateDoWhileStatement(DoWhileStatementSyntax node, Context context)
        {
            RTResult statement = null;
            var condition = new NumberValue(0);
            do
            {
                foreach (var stmt in node.Body)
                {
                    Evaluate(stmt, context);
                }
                var evaluationResult = Evaluate(node.Condition, context);
                if (evaluationResult.Type == SyntaxKind.NumberToken && evaluationResult.Error == "")
                {
                    condition = ((NumberValue)Evaluate(node.Condition, context).Value);
                }
                else return evaluationResult;
            } while (condition.IsTrue());
            return new RTResult(statement?.Value);
        }

        private RTResult EvaluateBinaryExpression(BinaryExpressionSyntax e, Context context)
        {
            var leftEvaluationResult = Evaluate(e.Left, context);
            var rightEvaluationResult = Evaluate(e.Right, context);
            
            if (leftEvaluationResult.Type == SyntaxKind.NumberToken
                && rightEvaluationResult.Type == SyntaxKind.NumberToken)
            {
                var left = (NumberValue)(leftEvaluationResult.Value);
                var right = (NumberValue)(rightEvaluationResult.Value);
                NumberValue result;

                switch (e.OperatorToken.Kind)
                {
                    case SyntaxKind.PlusToken:
                        result = left.Add(right);
                        break;
                    case SyntaxKind.MinusToken:
                        result = left.Subtract(right);
                        break;
                    case SyntaxKind.StarToken:
                        result = left.Multiply(right);
                        break;
                    case SyntaxKind.SlashToken:
                        if (right.Value != 0)
                            result = left.Divide(right);
                        else
                            return new RTResult(null, "Dividing by zero runtime exception.");
                        break;
                    case SyntaxKind.NotEqualsToken:
                        result = left.Comparison_NotEquals(right);
                        break;
                    case SyntaxKind.EqualsEqualsToken:
                        result = left.Comparison_Equals(right);
                        break;
                    case SyntaxKind.LessToken:
                        result = left.Comparison_Less(right);
                        break;
                    case SyntaxKind.LessEqualsToken:
                        result = left.Comparison_LessEquals(right);
                        break;
                    case SyntaxKind.GreaterToken:
                        result = left.Comparison_Greater(right);
                        break;
                    case SyntaxKind.GreaterEqualsToken:
                        result = left.Comparison_GreaterEquals(right);
                        break;
                    default:
                        return new RTResult(null, $"Unknown operator of kind <{e.OperatorToken.Kind}>.");
                }
                return new RTResult(result, SyntaxKind.NumberToken);
            }
            else if (leftEvaluationResult.Type == SyntaxKind.StringToken && rightEvaluationResult.Type == SyntaxKind.StringToken)
            {
                var left = (StringValue)(leftEvaluationResult.Value);
                var right = (StringValue)(rightEvaluationResult.Value);
                StringValue result;
                switch (e.OperatorToken.Kind)
                {
                    case SyntaxKind.PlusToken:
                        result = left.Add(right);
                        break;
                    case SyntaxKind.NotEqualsToken:
                        return new RTResult(left.Comparison_NotEquals(right), SyntaxKind.NumberToken);
                    case SyntaxKind.EqualsEqualsToken:
                        return new RTResult(left.Comparison_Equals(right), SyntaxKind.NumberToken);
                    default:
                        return new RTResult(null, $"Unknown operator of kind <{e.OperatorToken.Kind}>.");
                }
                return new RTResult(result, SyntaxKind.StringToken);
            }
            return new RTResult(null, $"Type missmatch <{leftEvaluationResult.Type}> and <{rightEvaluationResult.Type}> .");
        }
        private RTResult EvaluateUnaryExpression(UnaryExpressionSyntax u, Context context)
        {
            var evaluationResult = Evaluate(u.Operand, context);
            if (evaluationResult.Type == SyntaxKind.NumberToken)
            {
                var number = (NumberValue)evaluationResult.Value;
                if (u.OperatorToken.Kind == SyntaxKind.MinusToken)
                {
                    number = number.Multiply(new NumberValue(-1));
                }
                else if (u.OperatorToken.Kind == SyntaxKind.ExclamationToken)
                {
                    number = number.Negate();
                }
                return new RTResult(number, SyntaxKind.NumberToken);
            }
            else return new RTResult(null, "Unary operations are not supported on type String.");
        }

        private RTResult EvaluateNumber(LiteralExpressionSyntax n, Context context)
        {
            return new RTResult(new NumberValue(double.Parse(n.LiteralToken.Value.ToString()), context), SyntaxKind.NumberToken);
        }
        private RTResult EvaluateString(StringExpressionSyntax n, Context context)
        {
            return new RTResult(new StringValue(n.StringToken.Text, context), SyntaxKind.StringToken);
        }

    }
}
