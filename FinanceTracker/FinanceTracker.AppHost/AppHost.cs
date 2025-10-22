var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                            .WithPgAdmin()
                            .WithDataVolume(isReadOnly: false);
                            
var postgresdb = postgres.AddDatabase("postgresdb");

var apiService = builder.AddProject<Projects.FinanceTracker_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.AddProject<Projects.FinanceTracker_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
