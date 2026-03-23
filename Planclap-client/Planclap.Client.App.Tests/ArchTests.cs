using System.Reflection;
using NetArchTest.Rules;

namespace Planclap.Client.App.Tests;

public class ArchTests
{
    private const string RootNamespace = "Planclap.Client";
    private const string AppAssembly = RootNamespace + ".App";
    private const string ViewsAssembly = RootNamespace + ".Views";
    private const string InfrastructuresAssembly = RootNamespace + ".Infrastructures";
    private const string PresentationsAssembly = RootNamespace + ".Presentations";
    private const string DomainsAssembly = RootNamespace + ".Domains";

    private static readonly Assembly[] Assemblies =
    [
        Assembly.Load(AppAssembly),
        Assembly.Load(ViewsAssembly),
        Assembly.Load(InfrastructuresAssembly),
        Assembly.Load(PresentationsAssembly),
        Assembly.Load(DomainsAssembly)
    ];


    [Test]
    public void Views_Should_Depend_On_Presentation()
    {
        var rules = Types.InAssembly(Assemblies[1])
            .That()
            .ResideInNamespaceStartingWith(RootNamespace)
            .Should()
            .ResideInNamespaceStartingWith(ViewsAssembly)
            .And()
            .NotHaveDependencyOnAll(AppAssembly, InfrastructuresAssembly, DomainsAssembly, "Ical.Net", "MySql.Data");

        Assert.That(rules.GetResult().IsSuccessful, Is.True);
    }


    [Test]
    public void Infrastructures_Should_Depends_On_Domains()
    {
        var rules = Types.InAssembly(Assemblies[2])
            .That()
            .ResideInNamespaceStartingWith(RootNamespace)
            .Should()
            .ResideInNamespaceStartingWith(InfrastructuresAssembly)
            .And()
            .NotHaveDependencyOnAny(AppAssembly, ViewsAssembly, PresentationsAssembly);

        Assert.That(rules.GetResult().IsSuccessful, Is.True);
    }


    [Test]
    public void Presentations_Should_Depend_Only_On_Domains()
    {
        var rules = Types.InAssembly(Assemblies[3])
            .That()
            .ResideInNamespaceStartingWith(RootNamespace)
            .Should()
            .ResideInNamespaceStartingWith(PresentationsAssembly)
            .And()
            .NotHaveDependencyOnAny(AppAssembly, ViewsAssembly, InfrastructuresAssembly, "Serilog", "Ical.Net", "MySql.Data");

        Assert.That(rules.GetResult().IsSuccessful, Is.True);
    }


    [Test]
    public void Domain_Should_Not_Depend_On_Other_Assemblies()
    {
        var rules = Types.InAssembly(Assemblies[4])
            .That()
            .ResideInNamespaceStartingWith(RootNamespace)
            .Should()
            .ResideInNamespaceStartingWith(DomainsAssembly)
            .And()
            .NotHaveDependencyOnAny(AppAssembly, ViewsAssembly, InfrastructuresAssembly, PresentationsAssembly, "Serilog", "Ical.Net", "MySql.Data");

        Assert.That(rules.GetResult().IsSuccessful, Is.True);
    }
}
