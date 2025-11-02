using System.IO;
using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sqlserver")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var assemblyDirectory = Path.GetDirectoryName(typeof(Program).Assembly.Location)!;
var initScriptPath = Path.Join(assemblyDirectory, "init.sql");

var database = sqlServer.AddDatabase("myapp")
    .WithCreationScript(File.ReadAllText(initScriptPath));

var apiService = builder.AddProject<Projects.MyApp_ApiService>("apiservice")
    .WithReference(database)
    .WaitFor(database);

builder.AddProject<Projects.MyApp_WebApp>("webapp")
    .WithReference(apiService)
    .WaitFor(apiService);

var keycloack = builder.AddKeycloak("keycloak", 8090)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

builder.Build().Run();
