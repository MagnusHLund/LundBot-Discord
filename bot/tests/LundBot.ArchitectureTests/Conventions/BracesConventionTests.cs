using LundBot.ArchitectureTests.Support;
using Xunit;

namespace LundBot.ArchitectureTests.Conventions;

public sealed class BracesConventionTests
{
    [Fact]
    public void Control_flow_statement_bodies_use_braces()
    {
        ArchitectureAssertions.NoViolations(ProductionSourceCatalog.Sources.SelectMany(source =>
            BracesConvention.UnbracedBodies(source.Tree.GetRoot())
                .Select(statement => $"{source.Location(statement)} must enclose its statement body in braces.")));
    }
}
