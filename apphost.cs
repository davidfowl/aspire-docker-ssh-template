#:sdk Aspire.AppHost.Sdk@13.0.1
#:package Aspire.Hosting.Docker@13.0.1-preview.1.25575.3
#:package Aspire.Hosting.Docker.SshDeploy@0.1.0-ci.126
#:package Aspire.Hosting.JavaScript@13.0.1
#:package Aspire.Hosting.Yarp@13.0.1

using Aspire.Hosting.Yarp;
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

// Configure Docker Compose environment with SSH deployment support
builder.AddDockerComposeEnvironment("dcenv")
       .WithSshDeploySupport();

// Build your application here
var vite = builder.AddViteApp("app", "frontend");

// Configure YARP to serve the static files and handle routing
if (builder.ExecutionContext.IsPublishMode)
{
    var yarp = builder.AddYarp("site")
           .PublishWithStaticFiles(vite);

    if (!builder.Configuration.GetValue<bool>("EnableHttps"))
    {
        // HTTP only
        yarp.WithHostPort(80)
            .WithExternalHttpEndpoints();
    }
    else
    {
        // HTTPS with Let's Encrypt using Certbot
        // Certbot
        var domain = builder.AddParameter("domain");
        var letsEncryptEmail = builder.AddParameter("letsencrypt-email");

        // Shared volume name for certificates
        var volumeName = "letsencrypt";

        var certbot = builder.AddContainer("certbot", "certbot/certbot")
             // Shared volume for certificates - both certbot and YARP mount this
             .WithVolume(volumeName, "/etc/letsencrypt")
             // Port 80 must be published to host for Let's Encrypt to reach the ACME challenge
             .WithHttpEndpoint(port: 80, targetPort: 80)
             .WithExternalHttpEndpoints()
             .WithArgs(
                "certonly",
                "--standalone",
                "--non-interactive",
                "--agree-tos",
                "-v",
                "--keep-until-expiring",
                // Fix permissions so non-root containers (like YARP) can read the certs
                "--deploy-hook",
                "chmod -R 755 /etc/letsencrypt/live && chmod -R 755 /etc/letsencrypt/archive",
                "--email",
                letsEncryptEmail.Resource,
                "-d",
                domain.Resource
            );

        yarp.WaitForCompletion(certbot)
            // Mount the shared certificate volume (read-only since YARP only reads certs)
            .WithVolume(volumeName, "/etc/letsencrypt", isReadOnly: true)
            .WithHostPort(80)
            .WithHttpsEndpoint(443)
            .WithExternalHttpEndpoints()
            .WithEnvironment(context =>
            {
                // Configure YARP to use the Let's Encrypt certificates
                context.EnvironmentVariables["Kestrel__Certificates__Default__Path"] =
                    ReferenceExpression.Create($"/etc/letsencrypt/live/{domain}/fullchain.pem");
                context.EnvironmentVariables["Kestrel__Certificates__Default__KeyPath"] =
                    ReferenceExpression.Create($"/etc/letsencrypt/live/{domain}/privkey.pem");

                var yarpResource = (YarpResource)context.Resource;

                // Configure URLs for both HTTP and HTTPS
                var httpEndpoint = yarpResource.GetEndpoint("http");
                var httpsEndpoint = yarpResource.GetEndpoint("https");

                context.EnvironmentVariables["ASPNETCORE_URLS"] =
                    ReferenceExpression.Create($"http://+:{httpEndpoint.Property(EndpointProperty.TargetPort)};https://+:{httpsEndpoint.Property(EndpointProperty.TargetPort)}");
            });
    }
}

builder.Build().Run();
