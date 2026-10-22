namespace Mundialito.IntegrationTests;

[CollectionDefinition(Name)]
public sealed class ApiCollection : ICollectionFixture<MundialitoApiFactory>
{
    public const string Name = "Api con SQL Server";
}
