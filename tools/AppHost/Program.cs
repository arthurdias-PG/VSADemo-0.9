using AppHost.Extensions;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder
    .AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithHostPort(50123);

var db = sqlServer
    .AddDatabase("app-db")
    .WithDropDatabaseCommand();

var migrationService = builder.AddProject<MigrationService>("migrations")
    .WithReference(db)
    .WaitFor(sqlServer);

builder
    .AddProject<WebApi>("web-api")
    .WithEndpoint("https", endpoint => endpoint.IsProxied = false)
    .WithReference(db)
    .WaitForCompletion(migrationService);

builder.Build().Run();
