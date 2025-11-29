#:sdk Aspire.AppHost.Sdk@13.0.1
#:package Aspire.Hosting.Docker@13.0.1-preview.1.25575.3
#:package Aspire.Hosting.Docker.SshDeploy@0.1.0-ci.108
#:package Aspire.Hosting.JavaScript@13.0.1
#:package Aspire.Hosting.Yarp@13.0.1

var builder = DistributedApplication.CreateBuilder(args);

// Configure Docker Compose environment with SSH deployment support
builder.AddDockerComposeEnvironment("dcenv")
       .WithSshDeploySupport();

// Build your application here
var vite = builder.AddViteApp("app", "frontend");

// Configure YARP to serve the static files and handle routing
if (builder.ExecutionContext.IsPublishMode)
{
       builder.AddYarp("site")
              .WithEndpoint("http", e => e.Port = 80)
              .WithExternalHttpEndpoints()
              .PublishWithStaticFiles(vite);
}

builder.Build().Run();
