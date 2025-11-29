# Aspire Deploy to VPS with SSH and Docker

[Aspire](https://aspire.dev) makes building distributed applications easier. This template demonstrates one of its powerful features: deploying to any VPS with just Docker and SSH—no cloud vendor lock-in, no Kubernetes required.

## Why Aspire?

[Aspire](https://github.com/dotnet/aspire) is an opinionated stack for building cloud-native applications. It provides:

- **Local orchestration** — Run your entire distributed app with a single command
- **Service discovery** — Automatic connection string management between services
- **Observability** — Built-in dashboard with logs, traces, and metrics
- **Deployment flexibility** — Deploy anywhere: Azure, AWS, Kubernetes, or a simple VPS

## Deploy Anywhere

With SSH deploy support, Aspire can deploy to:

- Any Linux VPS (DigitalOcean, Linode, Hetzner, OVH, etc.)
- On-premises servers
- Raspberry Pi clusters
- Any machine with Docker and SSH

The same AppHost that orchestrates your local development environment generates production-ready Docker Compose deployments.

## Getting Started

1. **Use this template** to create your repository
2. **Add your applications** to the AppHost
3. **Generate the GitHub Actions workflow:**
   ```bash
   aspire do gh-action-dcenv
   ```
4. **Configure your secrets** in GitHub (SSH key, host, etc.)
5. **Push to deploy**

## One Command Deploys

**Deploy directly:**
```bash
aspire deploy
```

**Generate CI/CD:**
```bash
aspire do gh-action-dcenv
```

This generates a complete GitHub Actions workflow that:
- Builds container images from your AppHost
- Pushes to GitHub Container Registry
- SSHs into your VPS
- Deploys via Docker Compose

## The AppHost

```csharp
builder.AddDockerComposeEnvironment("dcenv")
       .WithSshDeploySupport();
```

That's it. Your existing Aspire application gains the ability to deploy anywhere.

## No Vendor Lock-In

- **Container registry**: Use GitHub Container Registry, Docker Hub, or any private registry
- **Target host**: Any server with Docker and SSH access
- **CI/CD**: GitHub Actions workflow generated, but the `aspire deploy` command works from anywhere

## Learn More

- [aspire.dev](https://aspire.dev) — Official documentation
- [Aspire on GitHub](https://github.com/dotnet/aspire) — Source code and issues
- [AspirePipelines](https://github.com/davidfowl/AspirePipelines) — SSH deploy packages used in this template
