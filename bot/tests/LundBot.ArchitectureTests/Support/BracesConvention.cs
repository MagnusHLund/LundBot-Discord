using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LundBot.ArchitectureTests.Support;

internal static class BracesConvention
{
    public static IEnumerable<StatementSyntax> UnbracedBodies(SyntaxNode root) =>
        root.DescendantNodes().Select(node => node switch
        {
            IfStatementSyntax statement => statement.Statement,
            ElseClauseSyntax { Statement: not IfStatementSyntax } clause => clause.Statement,
            ForStatementSyntax statement => statement.Statement,
            CommonForEachStatementSyntax statement => statement.Statement,
            WhileStatementSyntax statement => statement.Statement,
            DoStatementSyntax statement => statement.Statement,
            UsingStatementSyntax statement => statement.Statement,
            LockStatementSyntax statement => statement.Statement,
            FixedStatementSyntax statement => statement.Statement,
            _ => null
        }).OfType<StatementSyntax>().Where(statement => statement is not BlockSyntax);
}
