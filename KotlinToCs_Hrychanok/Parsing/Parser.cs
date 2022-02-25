using KotlinToCs_Hrychanok.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KotlinToCs_Hrychanok.Parsing
{
    class Parser
    {
        public IEnumerable<string> Diagnostics => diagnostics;

        private int position;
        private readonly SyntaxToken[] tokens;
        private List<string> diagnostics = new List<string>();
        private SyntaxToken Current => Peek(0);
        
        public Parser(string text)
        {
            var tokens = new List<SyntaxToken>();
            var lexer = new Lexer(text);
            SyntaxToken token;
            do
            {
                token = lexer.NextToken();
                if (token.Kind != SyntaxKind.WrongToken && token.Kind != SyntaxKind.WhitespaceToken)
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EOFToken);
            this.tokens = tokens.ToArray();
            diagnostics.AddRange(lexer.Diagnostics);
        }
        
        public SyntaxTree Parse()
        {
            var block = ParseBlock();
            var EOFToken = Match(SyntaxKind.EOFToken);
            return new SyntaxTree(diagnostics, block, EOFToken);
        }

        private ExpressionSyntax ParseBlock()
        {
            var definitionsList = new List<ExpressionSyntax>();
            var statementsList = new List<ExpressionSyntax>();
            if (MatchesMultiple(new SyntaxKind[] { SyntaxKind.VarToken, SyntaxKind.DoubleKeywordToken, SyntaxKind.StringKeywordToken }))
            {
                var definition = ParseDefinition();
                definitionsList.Add(definition);
                while (Matches(SyntaxKind.SemicolonToken))
                {
                    Match(SyntaxKind.SemicolonToken);
                    definition = ParseDefinition();
                    definitionsList.Add(definition);
                }
            }
            var statement = ParseStatement();
            statementsList.Add(statement);
            var statementOptions = new SyntaxKind[]  {
                    SyntaxKind.VarToken,
                    SyntaxKind.DoubleKeywordToken,
                    SyntaxKind.StringKeywordToken,
                    SyntaxKind.IdentifierToken,
                    SyntaxKind.IfToken,
                    SyntaxKind.WhileToken,
                    SyntaxKind.DoToken };
            while (MatchesMultiple(statementOptions))
            {
                statement = ParseStatement();
                if (diagnostics.Count > 0) break;
                statementsList.Add(statement);
            }
            return new BlockSyntax(definitionsList, statementsList);
        }
        //nice
        private ExpressionSyntax ParseStatement()
        {
            if (MatchesMultiple(new SyntaxKind[] { SyntaxKind.VarToken, SyntaxKind.DoubleKeywordToken, SyntaxKind.StringKeywordToken }))
            {
                return ParseDefinition();
            }
            if (Matches(SyntaxKind.IdentifierToken))
            {
                if (Peek(1).Kind == SyntaxKind.OpenParenthesisToken)
                {
                    return ParseFunctionCall();
                } else if (Peek(1).Kind == SyntaxKind.EqualsToken)
                {
                    return ParseAssignment();
                }
            }
            if (Matches(SyntaxKind.IfToken))
            {
                return ParseIfStatement();
            }
            if (Matches(SyntaxKind.WhileToken))
            {
                return ParseWhileStatement();
            }
            if (Matches(SyntaxKind.DoToken))
            {
                return ParseDoWhileStatement();
            }
            if (Matches(SyntaxKind.ExclamationToken))
            {
                return ParseCondition();
            }
            diagnostics.Add($"Unexpected token of kind <{Current.Kind}>");
            return new DoWhileStatementSyntax(null, null);
        }
        private ExpressionSyntax ParseFunctionCall()
        {
            var identifier = Match(SyntaxKind.IdentifierToken);
            Match(SyntaxKind.OpenParenthesisToken);
            var parameters = new List<ExpressionSyntax>();
            if (Matches(SyntaxKind.CloseParenthesisToken))
            {
                Match(SyntaxKind.CloseParenthesisToken);
            } else
            {
                parameters.Add(ParseExpression());
                while (Matches(SyntaxKind.CommaToken))
                {
                    Match(SyntaxKind.CommaToken);
                    parameters.Add(ParseExpression());
                }
                Match(SyntaxKind.CloseParenthesisToken);
            }
            return new FunctionCallSyntax(identifier, parameters);
        }

        private ExpressionSyntax ParseWhileStatement()
        {
            var statementsList = new List<ExpressionSyntax>();
            Match(SyntaxKind.WhileToken);
            Match(SyntaxKind.OpenParenthesisToken);
            var condition = ParseCondition();
            Match(SyntaxKind.CloseParenthesisToken);
            Match(SyntaxKind.OpenFigureBracketsToken);
            var statement = ParseStatement();
            statementsList.Add(statement);
            while (Matches(SyntaxKind.SemicolonToken))
            {
                Match(SyntaxKind.SemicolonToken);
                statement = ParseStatement();
                statementsList.Add(statement);
            }
            Match(SyntaxKind.CloseFigureBracketsToken);
            return new WhileStatementSyntax(condition, statementsList);
        }
     
        private ExpressionSyntax ParseDoWhileStatement()
        {
            var statementsList = new List<ExpressionSyntax>();
            Match(SyntaxKind.DoToken);
            Match(SyntaxKind.OpenFigureBracketsToken);
            var statement = ParseStatement();
            statementsList.Add(statement);
            while (Matches(SyntaxKind.SemicolonToken))
            {
                Match(SyntaxKind.SemicolonToken);
                statement = ParseStatement();
                statementsList.Add(statement);
            }
            Match(SyntaxKind.CloseFigureBracketsToken);
            Match(SyntaxKind.WhileToken);
            Match(SyntaxKind.OpenParenthesisToken);
            var condition = ParseCondition();
            Match(SyntaxKind.CloseParenthesisToken);
            return new DoWhileStatementSyntax(condition, statementsList);
        }
        
        private ExpressionSyntax ParseIfStatement()
        {
            var cases = new Dictionary<List<ExpressionSyntax>, ExpressionSyntax>();
            var statements = new List<ExpressionSyntax>();
            List<ExpressionSyntax> elseStatements = null;
            Match(SyntaxKind.IfToken);
            Match(SyntaxKind.OpenParenthesisToken);
            var ifCondition = ParseCondition();
            Match(SyntaxKind.CloseParenthesisToken);
            Match(SyntaxKind.OpenFigureBracketsToken);
            var statement = ParseStatement();
            statements.Add(statement);
            cases.Add(statements, ifCondition);
            while (Matches(SyntaxKind.SemicolonToken))
            {
                Match(SyntaxKind.SemicolonToken);
                statement = ParseStatement();
                statements.Add(statement);
            }
            Match(SyntaxKind.CloseFigureBracketsToken);
            while (Matches(SyntaxKind.ElseIfToken))
            {
                Match(SyntaxKind.ElseIfToken);
                Match(SyntaxKind.OpenParenthesisToken);
                var elifCondition = ParseCondition();
                statements = new List<ExpressionSyntax>();
                cases.Add(statements, elifCondition);
                Match(SyntaxKind.CloseParenthesisToken);
                Match(SyntaxKind.OpenFigureBracketsToken);
                var elifStatement = ParseStatement();
                statements.Add(elifStatement);

                while (Matches(SyntaxKind.SemicolonToken))
                {
                    Match(SyntaxKind.SemicolonToken);
                    statement = ParseStatement();
                    statements.Add(statement);
                }
                Match(SyntaxKind.CloseFigureBracketsToken);
            }
            if (Matches(SyntaxKind.ElseToken))
            {
                Match(SyntaxKind.ElseToken);
                Match(SyntaxKind.OpenFigureBracketsToken);
                elseStatements = new List<ExpressionSyntax>();
                var elseStatement = ParseStatement();
                elseStatements.Add(elseStatement);
                while (Matches(SyntaxKind.SemicolonToken))
                {
                    Match(SyntaxKind.SemicolonToken);
                    statement = ParseStatement();
                    statements.Add(statement);
                }
                Match(SyntaxKind.CloseFigureBracketsToken);
            }
            return new IfStatementSyntax(cases, elseStatements);
        }
        //nice
        private ExpressionSyntax ParseCondition()
        {
            if (Matches(SyntaxKind.ExclamationToken))
            {
                var exclamationToken = Match(SyntaxKind.ExclamationToken);
                var openPar = Match(SyntaxKind.OpenParenthesisToken);
                var condition = ParseCondition();
                var closePar = Match(SyntaxKind.CloseParenthesisToken);
                return new UnaryExpressionSyntax(exclamationToken, condition);
            }
            var left = ParseExpression();
            SyntaxKind[] ops = { SyntaxKind.NotEqualsToken, SyntaxKind.EqualsEqualsToken, SyntaxKind.LessToken, 
                SyntaxKind.LessEqualsToken, SyntaxKind.GreaterToken,SyntaxKind.GreaterEqualsToken};
            var op = MatchMultiple(ops);
            var right = ParseExpression();
            return new BinaryExpressionSyntax(left, op, right);

        }
        //nice
        private ExpressionSyntax ParseDefinition()
        {
            SyntaxToken type = NextToken();
            var varName = Match(SyntaxKind.IdentifierToken);
            var equalsToken = Match(SyntaxKind.EqualsToken);
            var expression = ParseExpression();
            if (type.Kind == SyntaxKind.VarToken)
            {
                return new DefinitionSyntax(varName, expression);
            } else
            {
                return new DefinitionSyntax(varName, expression, type.Kind);
            }
        }
        //nice
        private ExpressionSyntax ParseAssignment()
        {
            var varName = Match(SyntaxKind.IdentifierToken);
            var equalsToken = Match(SyntaxKind.EqualsToken);
            var expression = ParseExpression();
            return new AssignmentSyntax(varName, expression);
        }
        //nice
        private ExpressionSyntax ParseExpression()
        {
            ExpressionSyntax left;
            if (Current.Kind == SyntaxKind.MinusToken || Current.Kind == SyntaxKind.PlusToken)
            {
                var operatorToken = NextToken();
                var operand = ParseTerm();
                left = new UnaryExpressionSyntax(operatorToken, operand);
            }
            else
            {
                left = ParseTerm();
            }
            while (true)
            {
                if (Current.Kind == SyntaxKind.MinusToken || Current.Kind == SyntaxKind.PlusToken)
                {
                    var operatorToken = NextToken();
                    var right = ParseExpression();
                    left = new BinaryExpressionSyntax(left, operatorToken, right);
                }
                else break;
            }
            return left;
        }
        //nice
        private ExpressionSyntax ParseTerm()
        {
            var left = ParseFactor();
            while (Current.Kind == SyntaxKind.StarToken || Current.Kind == SyntaxKind.SlashToken)
            {
                var operatorToken = NextToken();
                var right = ParseFactor();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }
            return left;
        }
        //nice
        private ExpressionSyntax ParseFactor()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenParenthesisToken:
                    var left = NextToken();
                    var expression = ParseExpression();
                    var right = Match(SyntaxKind.CloseParenthesisToken);
                    return new ParenthesizedExpressionSyntax(left, expression, right);
                case SyntaxKind.IdentifierToken:
                    if (Peek(1).Kind == SyntaxKind.OpenParenthesisToken){
                        return ParseFunctionCall();
                    } else
                    {
                        var name = NextToken();
                        return new AccessVariableSyntax(name);
                    }
                case SyntaxKind.StringToken:
                    var stringToken = Match(SyntaxKind.StringToken);
                    return new StringExpressionSyntax(stringToken);
                default:
                    var numberToken = Match(SyntaxKind.NumberToken);
                    return new LiteralExpressionSyntax(numberToken);
            }
        }

        private SyntaxToken Peek(int offset)
        {
            var index = position + offset;
            if (index >= tokens.Length)
                return tokens[tokens.Length - 1];
            return tokens[index];
        }
        private SyntaxToken NextToken()
        {
            var current = Current;
            position++;
            return current;
        }
        private SyntaxToken Match(SyntaxKind type)
        {
            if (Current.Kind == type)
                return NextToken();
            diagnostics.Add($"Error : unexpected token <{Current.Kind}>, expected <{type}>");
            return new SyntaxToken(type, Current.Position, null, null);
        }
        private SyntaxToken MatchMultiple(SyntaxKind[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if(Matches(types[i]))
                    return NextToken();
            }
            string err = $"Error : unexpected token <{Current.Kind}>, expected ";
            for (int i = 0; i < types.Length - 1; i++)
            {
                err += $"<{types[i]}> | ";
            }
            err += $" <{types[types.Length-1]}>";
            return new SyntaxToken(types[0], Current.Position, null, null);
        }
        private bool Matches(SyntaxKind type)
        {
            if (Current.Kind == type)
                return true;
            return false;
        }
        private bool MatchesMultiple(SyntaxKind[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (Matches(types[i]))
                    return true;
            }
            return false;
        }
    }

}
