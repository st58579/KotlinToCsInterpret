using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KotlinToCs_Hrychanok.Lexing
{
    public enum SyntaxKind
    {
        //Tokens
        WrongToken,
        EOFToken,
        None, 

        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        EqualsToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        OpenFigureBracketsToken,
        CloseFigureBracketsToken,
        DotToken,
        DoubleQuoteToken,
        ExclamationToken,
        NotEqualsToken,
        LessToken,
        LessEqualsToken,
        GreaterToken,
        GreaterEqualsToken,
        AmpersandToken,
        PipeToken,
        EqualsEqualsToken,
        IdentifierToken,
        NumberToken,
        WhitespaceToken,

        //keywords 
        IfToken,
        ElseToken,
        ElseIfToken,
        DoToken,
        WhileToken,
        SemicolonToken,
        DoubleToken,
        DoubleKeywordToken,
        StringKeywordToken,
        StringToken,
        VarToken,



        //Expressions
        NumberExpression,
        BinaryExpression,
        ParenthesizedExpression,
        UnaryExpression,
        DefinitionSyntax,
        AccessVariableSyntax,
        IfStatement,
        WhileStatement,
        CommaToken,
        FunctionCallSyntax,
        StringExpression,
        DoWhileStatement,
        AssignmentSyntax,
        Block,
    }

}
