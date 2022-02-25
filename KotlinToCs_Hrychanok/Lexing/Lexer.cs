using System.Collections.Generic;

namespace KotlinToCs_Hrychanok.Lexing
{
    class Lexer
    {
        private readonly string code;
        private int position;
        private List<string> diagnostics = new List<string>();

        private char Current => position >= code.Length ? '\0' : code[position];

        public IEnumerable<string> Diagnostics => diagnostics;

        public Lexer(string code)
        {
            this.code = code;
        }

        public SyntaxToken NextToken()
        {
            if (position >= code.Length)
            {
                return new SyntaxToken(SyntaxKind.EOFToken, position, "\0", null);
            }
            if (char.IsDigit(Current))
            {
                return LexNumber();
            }
            if (char.IsWhiteSpace(Current))
            {
                return LexWhiteSpace();
            }
            if (char.IsLetter(Current))
            {
                return LexWord();
            }
            return LexSymbol();
        }

        private SyntaxToken LexNumber()
        {
            var start = position;

            while (char.IsDigit(Current) || Current == ',')
                Next();

            var length = position - start;
            var text = code.Substring(start, length);
            if (!double.TryParse(text, out double value))
                diagnostics.Add($"The number {text} can not be represented by Double.");
            return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
        }
        private SyntaxToken LexWord()
        {
            var start = position;
            while (char.IsLetterOrDigit(Current))
                Next();
            var length = position - start;
            var text = code.Substring(start, length);
            switch (text)
            {
                case "if":
                    return new SyntaxToken(SyntaxKind.IfToken, start, text, null);
                case "else":
                    if (char.IsWhiteSpace(Current) && Peek(1) == 'i' && Peek(2) == 'f' && !char.IsLetterOrDigit(Peek(3)))
                    {
                        Next(3);
                        return new SyntaxToken(SyntaxKind.ElseIfToken, start, "else if", null);
                    }
                    else return new SyntaxToken(SyntaxKind.ElseToken, start, text, null);
                case "while":
                    return new SyntaxToken(SyntaxKind.WhileToken, start, text, null);
                case "do":
                    return new SyntaxToken(SyntaxKind.DoToken, start, text, null);
                case "var":
                    return new SyntaxToken(SyntaxKind.VarToken, start, text, null);
                case "Double":
                    return new SyntaxToken(SyntaxKind.DoubleKeywordToken, start, text, null);
                case "String":
                    return new SyntaxToken(SyntaxKind.StringKeywordToken, start, text, null);
                default:
                    return new SyntaxToken(SyntaxKind.IdentifierToken, start, text, null);
            }
        }

        private SyntaxToken LexWhiteSpace()
        {
            var start = position;
            while (char.IsWhiteSpace(Current))
                Next();

            var length = position - start;
            var text = code.Substring(start, length);
            return new SyntaxToken(SyntaxKind.WhitespaceToken, start, text, null);
        }
        private SyntaxToken LexSymbol()
        {
            switch (Current)
            {
                case '+':
                    return new SyntaxToken(SyntaxKind.PlusToken, position++, "+", null);
                case '-':
                    return new SyntaxToken(SyntaxKind.MinusToken, position++, "-", null);
                case '*':
                    return new SyntaxToken(SyntaxKind.StarToken, position++, "*", null);
                case '/':
                    return new SyntaxToken(SyntaxKind.SlashToken, position++, "/", null);
                case '(':
                    return new SyntaxToken(SyntaxKind.OpenParenthesisToken, position++, "(", null);
                case ')':
                    return new SyntaxToken(SyntaxKind.CloseParenthesisToken, position++, ")", null);
                case '{':
                    return new SyntaxToken(SyntaxKind.OpenFigureBracketsToken, position++, "{", null);
                case '}':
                    return new SyntaxToken(SyntaxKind.CloseFigureBracketsToken, position++, "}", null);
                case '.':
                    return new SyntaxToken(SyntaxKind.DotToken, position++, ".", null);
                case ',':
                    return new SyntaxToken(SyntaxKind.CommaToken, position++, ",", null);
                case ';':
                    return new SyntaxToken(SyntaxKind.SemicolonToken, position++, ";", null);
                case '"':
                    Next();
                    var start = position;
                    while (char.IsLetterOrDigit(Current) || Current == ' ' || Current == '\t' || Current == '?' 
                        || Current == '!' || Current == '.' || Current == ':' || Current == ';')
                        Next();
                    if (Current == '"')
                    {
                        var length = position - start;
                        var text = code.Substring(start, length);
                        Next();
                        return new SyntaxToken(SyntaxKind.StringToken, position, text, null);
                    }
                    else
                    {
                        diagnostics.Add($"Error : bad character in input: '{Current}");
                        return new SyntaxToken(SyntaxKind.StringToken, position, "", null);
                    }
                case '<':
                    if (Peek(1) == '=')
                    {
                        Next();
                        return new SyntaxToken(SyntaxKind.LessEqualsToken, position++, "<=", null);
                    }
                    else return new SyntaxToken(SyntaxKind.LessToken, position++, "<", null);
                case '>':
                    if (Peek(1) == '=')
                    {
                        Next();
                        return new SyntaxToken(SyntaxKind.GreaterEqualsToken, position++, ">=", null);
                    }
                    else return new SyntaxToken(SyntaxKind.GreaterToken, position++, ">", null);
                case '=':
                    if (Peek(1) == '=')
                    {
                        Next();
                        return new SyntaxToken(SyntaxKind.EqualsEqualsToken, position++, "==", null);
                    }
                    return new SyntaxToken(SyntaxKind.EqualsToken, position++, "=", null);
                case '!':
                    if (Peek(1) == '=')
                    {
                        Next();
                        return new SyntaxToken(SyntaxKind.NotEqualsToken, position++, "!=", null);
                    }
                    else return new SyntaxToken(SyntaxKind.ExclamationToken, position++, "!", null);
            }
            diagnostics.Add($"Error : bad character in input: '{Current}");
            return new SyntaxToken(SyntaxKind.WrongToken, position++, code[position - 1].ToString(), null);
        }



        private void Next()
        {
            position++;
        }

        private void Next(int offset)
        {
            position += offset;
        }

        private char Peek(int offset)
        {
            if (position + offset < code.Length)
            {
                return code[position + offset];
            }
            else return code[code.Length];
        }
    }

}
