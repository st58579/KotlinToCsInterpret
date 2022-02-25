using System.Collections.Generic;
using KotlinToCs_Hrychanok.Lexing;

namespace KotlinToCs_Hrychanok.Parsing
{
    internal class BlockSyntax : ExpressionSyntax
    {
        public List<ExpressionSyntax> DefinitionsList { get; }
        public List<ExpressionSyntax> StatementsList { get; }

        public BlockSyntax(List<ExpressionSyntax> definitionsList, List<ExpressionSyntax> statementsList)
        {
            this.DefinitionsList = definitionsList;
            this.StatementsList = statementsList;
        }

        public override SyntaxKind Kind => SyntaxKind.Block;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            for (int i = 0; i < DefinitionsList.Count; i++)
            {
                yield return DefinitionsList[i];
            }
            for (int i = 0; i < StatementsList.Count; i++)
            {
                yield return StatementsList[i];
            }
        }
    }
}